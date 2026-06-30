# Basics of XREAL development in Unity

- [Set up a scene from scratch](#set-up-a-scene-from-scratch)
- [Respond to the App button](#respond-to-the-app-button)

## Set up a scene from scratch

1. Create a new scene in your Unity project.
2. Delete the "Main Camera" object.<br>
   The "Directional Light" can stay.
3. Add the "XR Interaction Setup" prefab (`Packages/XREAL XR Plugin/Runtime/Prefabs/`) to the scene.
4. Create a new empty GameObject in the scene at (0,0,0).<br>
   Rename it to "XRManager" (or similar).
5. (Optional) Add the "Lazy Follow" component to the SceneManager.
6. Create a new Canvas object as a child of the XRManager.
7. Set its scale on all axes to 0.001.
8. Set its width to 364 and height to 205.
9. Set the Render Mode to World Space.
10. Add a Tracked Device Graphic Raycaster component.

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
