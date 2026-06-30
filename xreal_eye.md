# Getting started with the XREAL Eye

- [Test the XREAL Eye](#test-the-xreal-eye)
- [Capture a live feed](#capture-a-live-feed)

## Test the XREAL Eye

1. In the Unity Package Manager, import the Camera Features sample from the XREAL XR Plugin package.
2. Open the RGBCameraAndCapture demo scene (`Assets/Samples/XREAL XR Plugin/3.1.0/Camera Features/RGBCameraAndCapture`).
3. In Project Settings, under ~XR Plug-In Management > XREAL~, expand the "Additional Permissions" list and add `RECORD_AUDIO`
    and `FOREGROUND_SERVICE_MEDIA_PROJECTION`.
4. Build and deploy the app.

## Capture a live feed

1. Set RawImage material to YUVTexture
2. Create new XREALEyeTest script:
```csharp
using Unity.XR.XREAL;
using UnityEngine;
using UnityEngine.UI;

public class XREALEyeTest : MonoBehaviour
{
    [SerializeField] private RawImage YUVImage;

    private XREALRGBCameraTexture RGBCameraTexture;

    void Start() {
        RGBCameraTexture = XREALRGBCameraTexture.CreateSingleton();
        StartCapture();
    }

    void Update() {
        var yuvTextures = RGBCameraTexture.GetYUVFormatTextures();
        if (yuvTextures[0] != null) {
            YUVImage.texture = yuvTextures[0];
            YUVImage.material.SetTexture("_UTex", yuvTextures[1]);
            YUVImage.material.SetTexture("_VTex", yuvTextures[2]);
        }
    }

    void StartCapture() {
        if (!RGBCameraTexture.IsCapturing) {
            RGBCameraTexture.StartCapture();
        }
    }

    void StopCapture() {
        if (RGBCameraTexture.IsCapturing) {
            RGBCameraTexture.StopCapture();
        }
    }
}
```
3. Attach to parent GameObject
4. Connect the RawImage to YUVImage
5. Scale & position the RawImage appropriately
