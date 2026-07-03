# Getting started with the XREAL Eye

- [Prerequisites](#prerequisites)
- [Project configuration](#project-configuration)
- [(Optional) Test with a sample scene](#optional-test-with-a-sample-scene)
- [Capture a live feed](#capture-a-live-feed)

## Prerequisites

- Unity project configured as described in [setup.md](setup.md)

## Project configuration

1. In the Unity Package Manager, import the Camera Features sample from the XREAL XR Plugin package.
2. In Project Settings, under `XR Plug-In Management > XREAL`, expand the "Additional Permissions" list and add `RECORD_AUDIO` and `FOREGROUND_SERVICE_MEDIA_PROJECTION`.

## (Optional) Test with a sample scene

Before creating your own scene, you may wish to test the XREAL Eye functionality with the provided RGBCameraAndCapture sample scene.

1. Open the RGBCameraAndCapture demo scene (`Assets/Samples/XREAL XR Plugin/3.1.0/Camera Features/RGBCameraAndCapture`).
2. [Build and deploy the app](setup.md#build-and-deploy-a-sample-scene).

## Capture a live feed

1. [Set up a new scene](basics.md#set-up-a-scene-from-scratch).
2. Add a RawImage as a child of your Canvas (`Right click > UI > Raw Image`).
3. Scale & position the RawImage on the Canvas as desired.
4. Set the RawImage's material to "YUVTexture" (imported with the Camera Features sample at `Assets/Samples/XREAL XR Plugin/3.1.0/Camera Features/RGBCameraAndCapture/Materials`).
5. Create a new script named "XREALEyeTest":
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
        if (yuvTextures != null && yuvTextures.Length >= 3) {
            YUVImage.texture = yuvTextures[0];
            YUVImage.material.SetTexture("_UTex", yuvTextures[1]);
            YUVImage.material.SetTexture("_VTex", yuvTextures[2]);
        }
    }

    void OnDestroy() {
        StopCapture();
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
6.  Attach the above script as a component to the XRManager object and, with the XRManager selected, drag the RawImage to the `YUVImage` field of the component.
7. [Build and deploy the app](setup.md#build-and-deploy-a-sample-scene).
