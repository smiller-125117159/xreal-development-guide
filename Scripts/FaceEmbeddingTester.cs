using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;

public class FaceEmbeddingTester : MonoBehaviour
{
    [System.Serializable]
    public struct RefPhoto {
        public string Name;
        public Texture2D Photo;
    }

    [Header("Model Configuration")]
    public ModelAsset mobileFaceNetModelAsset;

    [Header("Test Input")]
    public RefPhoto[] referencePhotos; // 112x112 pixel crop of a face
    public Texture2D[] testPhotos;
    public float matchingThreshold = 0.70f;

    private Model runtimeModel;
    private Worker inferenceWorker;

    private readonly Dictionary<string, float[]> knownFaceRegistry = new();

    void Start() {
        Debug.Log("Starting face embedding test.");

        InitializeInferenceEngine();
        InitializeDatabase();

        ExecutePipelineTest();
    }

    private void OnDestroy() {
        inferenceWorker?.Dispose();
    }

    public string IdentifyFace(Texture2D face) {
        if (face.width != 112 ||  face.height != 112) {
            return null;
        }

        return MatchEmbeddingToRegistry(GenerateEmbedding(face));
    }

    void InitializeDatabase() {
        foreach (var person in referencePhotos) {
            knownFaceRegistry.Add(person.Name, GenerateEmbedding(person.Photo));
        }

        Debug.Log($"[FaceAuth] Database populated with {knownFaceRegistry.Count} profiles.");
    }

    void InitializeInferenceEngine() {
        if (mobileFaceNetModelAsset == null) {
            Debug.LogError("[FaceAuth] Model asset is missing. Assign the .onnx file in the Inspector.");
            return;
        }

        runtimeModel = ModelLoader.Load(mobileFaceNetModelAsset);

        inferenceWorker = new Worker(runtimeModel, BackendType.GPUCompute);
    }

    void ExecutePipelineTest() {
        foreach (var photo in testPhotos) {
            if (!photo) continue;

            float[] liveEmbedding = GenerateEmbedding(photo);

            string identifiedPerson = MatchEmbeddingToRegistry(liveEmbedding);

            Debug.Log($"[FaceAuth] Pipeline complete. Match Result: {identifiedPerson}");
        }
    }

    string MatchEmbeddingToRegistry(float[] inputVector) {
        string bestMatchName = "Unknown";
        float highestScore = -1.0f;

        foreach (var profile in knownFaceRegistry) {
            float similarity = ComputeCosineSimilarity(inputVector, profile.Value);

            if (similarity > highestScore) {
                highestScore = similarity;
                bestMatchName = profile.Key;
            }
        }

        if (highestScore < matchingThreshold) {
            return $"Unknown Face Detected (Best candidate: {bestMatchName} scored {highestScore:F2})";
        }

        return $"{bestMatchName} (Confidence: {highestScore:F2})";
    }

    float ComputeCosineSimilarity(float[] vectorA, float[] vectorB) {
        if (vectorA.Length != vectorB.Length) return 0f;

        float dotProduct = 0f;
        for (int i = 0; i < vectorA.Length; i++) {
            dotProduct += vectorA[i] * vectorB[i];
        }
        return dotProduct;
    }

    float[] GenerateEmbedding(Texture2D image) {
        if (inferenceWorker == null) return null;

        using var inputTensor = new Tensor<float>(new TensorShape(1, 3, 112, 112));
        TextureConverter.ToTensor(image, inputTensor, default);

        inferenceWorker.Schedule(inputTensor);

        using var outputTensor = (inferenceWorker.PeekOutput() as Tensor<float>).ReadbackAndClone();

        float[] liveEmbedding = outputTensor.DownloadToArray();
        Normalize(liveEmbedding);

        return liveEmbedding;
    }

    void Normalize(float[] vector) {
        float magnitude = 0f;

        for (int i = 0; i < 128; i++) {
            magnitude += vector[i] * vector[i];
        }

        magnitude = Mathf.Sqrt(magnitude);
        for (int i = 0; i < 128; i++) {
            vector[i] /= magnitude;
        }
    }
}
