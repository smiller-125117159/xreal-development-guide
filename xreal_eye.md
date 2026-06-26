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
5. (optional) Add Lazy Follow component
5. Create new Canvas as child of above
6. Set scale on all axes to 0.001
7. Set width to 960 and height to 540
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
    [SerializeField]
    private RawImage m_YUVImage;

    private XREALRGBCameraTexture m_RGBCameraTexture;

    void Start() {
        m_RGBCameraTexture = XREALRGBCameraTexture.CreateSingleton();
        StartCapture();
    }

    void Update() {
        var yuvTextures = m_RGBCameraTexture.GetYUVFormatTextures();
        if (yuvTextures[0] != null) {
            m_YUVImage.texture = yuvTextures[0];
            m_YUVImage.material.SetTexture("_UTex", yuvTextures[1]);
            m_YUVImage.material.SetTexture("_VTex", yuvTextures[2]);
        }
    }

    void StartCapture() {
        if (!m_RGBCameraTexture.IsCapturing) {
            m_RGBCameraTexture.StartCapture();
        }
    }

    void StopCapture() {
        if (m_RGBCameraTexture.IsCapturing) {
            m_RGBCameraTexture.StopCapture();
        }
    }
}
```
3. Attach to parent GameObject
4. Connect the RawImage to m_YUVImage
5. Scale & position the RawImage appropriately

## Respond to the App button

Right click > Create > Input Actions

Action Maps +, New action map, New action, \<No binding>, Path, XR Controller, XREAL Controller, ButtonId0

```csharp
[SerializeField] private InputActionReference voiceToggleAction;

private void OnEnable() {
    if (voiceToggleAction != null) {
        // Activate tracing threads for this asset node
        voiceToggleAction.action.Enable();
        // Subscribe your target execution function to the event pipeline
        voiceToggleAction.action.performed += OnVoiceActionExecuted;
    }
}

private void OnDisable() {
    if (voiceToggleAction != null) {
        voiceToggleAction.action.performed -= OnVoiceActionExecuted;
        voiceToggleAction.action.Disable();
    }
}

private void OnVoiceActionExecuted(InputAction.CallbackContext context) {
    // Guard checking ensuring this only fires on the down-press execution window
    if (context.ReadValueAsButton()) {
        Debug.Log("Input Map Match: Calling Voice Registration Toggle.");
        ToggleSpeechRegistration();
    }
}
```
