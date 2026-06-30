# Getting started with the XREAL Eye

- [Test the XREAL Eye](#test-the-xreal-eye)

## Test the XREAL Eye

1. In the Unity Package Manager, import the Camera Features sample from the XREAL XR Plugin package.
2. Open the RGBCameraAndCapture demo scene (`Assets/Samples/XREAL XR Plugin/3.1.0/Camera Features/RGBCameraAndCapture`).
3. In Project Settings, under ~XR Plug-In Management > XREAL~, expand the "Additional Permissions" list and add `RECORD_AUDIO`
    and `FOREGROUND_SERVICE_MEDIA_PROJECTION`.
4. Build and deploy the app.

## Set up a scene from scratch

1. Create new scene
2. Delete Main Camera; Directional Light can stay
3. Add the "XR Interaction Setup" prefab (`Packages/XREAL XR Plugin/Runtime/Prefabs/`) to the scene.
4. Create new empty GameObject in scene at (0,0,0)
5. (Optional) Add Lazy Follow component
5. Create new Canvas as child of above
6. Set scale on all axes to 0.001
7. Set width to 364 and height to 205
8. Set Render Mode to World Space
9. Add Tracked Device Graphic Raycaster component
10. Add RawImage to Canvas & set color

## Add a live feed

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

## Respond to the App button

1. Open `Assets/InputSystem_Actions`.
2. Click the "+" icon in the Action Maps header.
3. Rename the new map appropriately (e.g. "XREAL").
4. Select the map and rename the default action it contains appropriately (e.g. "App Button").<br>
   Also ensure the action type is set to Button.

5. Expand the action, select "\<No binding>", then set "Path" to `XR Controller > XREAL Controller > ButtonId0`.
6. Respond to the button via code such as the following:
```csharp
[SerializeField] private InputActionReference appButtonAction;

private void OnEnable() {
    if (appButtonAction != null) {
        appButtonAction.action.Enable();
        appButtonAction.action.performed += OnAppButtonPressed;
    }
}

private void OnDisable() {
    if (appButtonAction != null) {
        appButtonAction.action.performed -= OnAppButtonPressed;
        appButtonAction.action.Disable();
    }
}

private void OnAppButtonPressed(InputAction.CallbackContext context) {
    if (context.ReadValueAsButton()) {
        // Respond to button press
    }
}
```
