using System.Collections.Generic;
using System.Linq;
using Unity.InferenceEngine;
using UnityEngine;
using UnityEngine.UI;
using static RunYOLO;
using static Unity.VisualScripting.Member;

public class FaceDetectionTester : MonoBehaviour
{
    [Header("Model Configuration")]
    public ModelAsset blazeFaceModelAsset;
    public TextAsset anchorsAsset;
    public FaceEmbeddingTester faceEmbedder;

    [Header("Test Input")]
    public Texture2D inputImage;
    public RawImage displayImage;
    public Texture2D borderTexture;
    public Font font;
    public RawImage cropImage;

    private Model runtimeModel;
    private Worker inferenceWorker;

    private Transform displayLocation;
    private Sprite borderSprite;
    private GameObject boundingBox;

    private List<Vector2> anchors;

    void Start() {
        anchors = new();
        foreach (var line in anchorsAsset.text.Split('\n')) {
            var values = line.Split(',');

            if (values.Length >= 2) {
                anchors.Add(new Vector2(float.Parse(values[0]), float.Parse(values[1])));
            }
        }

        displayLocation = displayImage.transform;
        borderSprite = Sprite.Create(
            borderTexture, 
            new Rect(0, 0, borderTexture.width, borderTexture.height), 
            new Vector2(borderTexture.width / 2, borderTexture.height / 2)
        );

        boundingBox = CreateNewBox(Color.yellow);

        InitializeDetector();

        //TryIdentifyFaceFromPhoto(inputImage);
    }

    private void OnDestroy() {
        inferenceWorker?.Dispose();
    }

    public string TryIdentifyFaceFromPhoto(Texture2D photo) {
        displayImage.texture = photo;
        ClearAnnotations();

        Rect faceRect = DetectFace(photo);
        DrawBox(faceRect, photo.width, photo.height);

        Texture2D faceTexture = CropAndResize(photo, faceRect, 112, 112);
        cropImage.texture = faceTexture;
        // TODO Destroy faceTexture

        return faceEmbedder.IdentifyFace(faceTexture);
    }

    void InitializeDetector() {
        if (blazeFaceModelAsset == null) {
            Debug.LogError("[FaceDet] BlazeFace model asset is missing. Assign the .onnx file in the Inspector.");
            return;
        }

        runtimeModel = ModelLoader.Load(blazeFaceModelAsset);

        inferenceWorker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    Texture2D CropAndResize(Texture2D source, Rect faceRect, int targetWidth, int targetHeight) {
        float maxDim = Mathf.Max(faceRect.width, faceRect.height);

        int cropSize = Mathf.RoundToInt(maxDim);
        int cropX = Mathf.RoundToInt(faceRect.x - (maxDim - faceRect.width) / 2f);
        int cropY = Mathf.RoundToInt(source.height - faceRect.y - (maxDim + faceRect.height) / 2f);

        Texture2D cropped = new (cropSize, cropSize, source.format, false);

        Graphics.CopyTexture(source, 0, 0, cropX, cropY, cropSize, cropSize,
            cropped, 0, 0, 0, 0);

        RenderTexture tempRT = RenderTexture.GetTemporary(
            targetWidth,
            targetHeight,
            0,
            RenderTextureFormat.ARGB32
        );

        Graphics.Blit(cropped, tempRT);

        Destroy(cropped);

        Texture2D result = new (targetWidth, targetHeight, source.format, false);

        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = tempRT;

        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = previousActive;
        RenderTexture.ReleaseTemporary(tempRT);

        return result;
    }

    public Rect DetectFace(Texture2D inputImage) {
        var layoutSettings = new TextureTransform().SetTensorLayout(TensorLayout.NHWC);

        using var inputTensor = new Tensor<float>(new TensorShape(1, 128, 128, 3));
        TextureConverter.ToTensor(inputImage, inputTensor, layoutSettings);

        inferenceWorker.Schedule(inputTensor);

        // BlazeFace yields two output tensors:
        // Output Index 0: Bounding Box / Keypoint coordinates, shape (1, 896, 16)
        // Output Index 1: Anchor Confidence Scores, shape (1, 896, 1)
        using var rawBoxesTensor = (inferenceWorker.PeekOutput(0) as Tensor<float>).ReadbackAndClone();
        using var rawScoresTensor = (inferenceWorker.PeekOutput(1) as Tensor<float>).ReadbackAndClone();

        float[] boxesData = rawBoxesTensor.DownloadToArray();
        float[] scoresData = rawScoresTensor.DownloadToArray();
        int topIndex = FindMaxScoreIndex(scoresData);

        float imageWidth = inputImage.width;
        float imageHeight = inputImage.height;

        Debug.Log($"Width: {imageWidth} Height: {imageHeight}");

        float inputResolution = 128f;

        float rawDx = boxesData[topIndex * 16 + 0];
        float rawDy = boxesData[topIndex * 16 + 1];
        float rawDw = boxesData[topIndex * 16 + 2];
        float rawDh = boxesData[topIndex * 16 + 3];

        float normalizedDx = rawDx / inputResolution;
        float normalizedDy = rawDy / inputResolution;
        float normalizedW = rawDw / inputResolution;
        float normalizedH = rawDh / inputResolution;

        float cx = anchors[topIndex].x + normalizedDx;
        float cy = anchors[topIndex].y + normalizedDy;

        Rect rect = new(
            (cx - normalizedW / 2f) * imageWidth, 
            (cy - normalizedH / 2f) * imageHeight, 
            normalizedW * imageWidth, 
            normalizedH * imageHeight);

        return rect;
    }

    int FindMaxScoreIndex(float[] scores) {
        float maxScore = 0f;
        int topIndex = 0;

        for (int i = 0; i < scores.Length; i++) {
            if (scores[i] > maxScore) {
                maxScore = scores[i];
                topIndex = i;
            }
        }

        return topIndex;
    }

    public void DrawBox(Rect box, int sourceWidth, int sourceHeight) {
        RectTransform displayTransform = displayImage.rectTransform;
        float displayWidth = displayTransform.rect.width;
        float displayHeight = displayTransform.rect.height;
        float pivotX = displayTransform.pivot.x;
        float pivotY = displayTransform.pivot.y;

        float scaledX = box.x / sourceWidth * displayWidth;
        float scaledY = box.y / sourceHeight * displayHeight;
        float scaledW = box.width / sourceWidth * displayWidth;
        float scaledH = box.height / sourceHeight * displayHeight;

        Debug.Log($"{scaledX} {scaledY} {scaledW} {scaledH} {displayWidth} {displayHeight} {pivotX} {pivotY}");

        boundingBox.SetActive(true);
        boundingBox.transform.localPosition = new Vector3(scaledX - (displayWidth * pivotX), -(scaledY - (displayHeight * (1f - pivotY))));

        RectTransform rt = boundingBox.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(scaledW, scaledH);
    }

    public GameObject CreateNewBox(Color color) {
        var panel = new GameObject("ObjectBox");
        panel.AddComponent<CanvasRenderer>();
        Image img = panel.AddComponent<Image>();
        img.color = color;
        img.sprite = borderSprite;
        img.type = Image.Type.Sliced;
        panel.transform.SetParent(displayLocation, false);

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);

        return panel;
    }

    public void ClearAnnotations() {
        boundingBox.SetActive(false);
    }
}
