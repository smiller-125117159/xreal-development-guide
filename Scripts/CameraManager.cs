using Unity.XR.XREAL;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Material YUVMaterial;

    private XREALRGBCameraTexture RGBCameraTexture;
    private RenderTexture TargetRT;

    void Start() {
        TargetRT = new RenderTexture(1280, 720, 0);
        RGBCameraTexture = XREALRGBCameraTexture.CreateSingleton();
    }

    private void OnDestroy() {
        StopCapture();
    }

    public void StartCapture() {
        if (!RGBCameraTexture.IsCapturing) {
            RGBCameraTexture.StartCapture();
        }
    }

    public void StopCapture() {
        if (RGBCameraTexture.IsCapturing) {
            RGBCameraTexture.StopCapture();
        }
    }
    
    public Texture2D[] GetYUVTextures() {
        return RGBCameraTexture.GetYUVFormatTextures();
    }

    public Texture2D RenderFrame() {
        var yuvTextures = RGBCameraTexture.GetYUVFormatTextures();

        if (yuvTextures == null || yuvTextures[0] == null) {
            return null;
        }

        YUVMaterial.SetTexture("_UTex", yuvTextures[1]);
        YUVMaterial.SetTexture("_VTex", yuvTextures[2]);

        Graphics.Blit(yuvTextures[0], TargetRT, YUVMaterial);

        Texture2D frame = new(TargetRT.width, TargetRT.height, TextureFormat.RGB24, false);
        frame.ReadPixels(new Rect(0, 0, TargetRT.width, TargetRT.height), 0, 0);
        frame.Apply();

        return frame;
    }
}
