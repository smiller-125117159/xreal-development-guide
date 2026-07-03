# Basics of XREAL development in Unity

- [Prerequisites](#prerequisites)
- [Set up a scene from scratch](#set-up-a-scene-from-scratch)
- [Respond to the App button](#respond-to-the-app-button)

## Prerequisites

- A Unity project configured as described in [setup.md](setup.md)

## Set up a scene from scratch

1. Create a new scene in your Unity project.
2. Delete the "Main Camera" object. (The "Directional Light" can stay.)
3. Add the "XR Interaction Setup" prefab (`Packages/XREAL XR Plugin/Runtime/Prefabs/`) to the scene.
4. Create a new empty GameObject in the scene at (0,0,0). Rename it to "XRManager" (or similar).
5. (Optional) Add the "Lazy Follow" component to the XRManager. This will cause any children of the object to follow the user's vision. See [here](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/lazy-follow.html) for more information.
6. Create a new Canvas object as a child of the XRManager.
7. Set the Canvas' width to 1920 and height to 1080.
8. Set the Canvas' scale on all axes to 0.0002. This value was determined through experimentation to allow a 1920x1080 canvas to approximately fill the entire display of the XREAL glasses, given the default follow distance of the Lazy Follow component (z=0.5). Calculations involving the glasses' FOV may yield a more precise value.
9. Set the Canvas' Render Mode to World Space.
10. Add a Tracked Device Graphic Raycaster component to the Canvas. This provides support for use of the phone as a pointer to interact with UI elements.

## Respond to the App button

1. Open `Assets/InputSystem_Actions`, imported with the Starter Assets sample of the XR Interaction Toolkit (or, create a new input system via `Right click > Create > Input Actions`).
2. Click the "+" icon in the "Action Maps" header.
3. Rename the new map appropriately (e.g. "XREAL").
4. Select the map and rename the default action it contains appropriately (e.g. "App Button"). Also ensure the action type is set to Button.
5. Expand the action, select "\<No binding>", then set "Path" to `XR Controller > XREAL Controller > ButtonId0`.
6. Respond to the button via a script such as the following:
```csharp
using UnityEngine.InputSystem;

public class AppButtonExample : MonoBehaviour {
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
}
```
7. Attach the script as a component to an object in the scene. Then, with this object selected, expand the `InputSystem_Actions` asset in the project explorer and drag the newly created action (e.g. `XREAL/App Button`) to the `appButtonAction` field of the component.

For more information on responding to and customizing the phone controller, see [here](https://docs.xreal.com/Input%20and%20Interactions/Controller).
