# Getting Started with XREAL Development

- [Hardware used](#hardware-used)
- [Set up the development environment](#set-up-the-development-environment)
- [Set up a Unity project](#set-up-a-unity-project)
- [(Optional) Improve build speeds](#optional-improve-build-speeds)
- [(Optional) Set up wireless debugging](#optional-set-up-wireless-debugging)
- [Build and deploy a sample scene](#build-and-deploy-a-sample-scene)

## Hardware used

- XREAL One AR Glasses (model X1111)
- Samsung Galaxy S25
    - OS: Android 16
- Lenovo V15 G4 ABP Laptop
    - OS: Windows 11 Pro

## Set up the development environment

1. Update the firmware of the XREAL glasses to the latest version (see [here](https://www.xreal.com/ota/)).<br>
   **Note**: It appears this *must* be done via Google Chrome.
2. Enable developer mode on the phone (see [here](https://www.android.com/intl/en_uk/articles/enable-android-developer-settings/)).
3. Connect the glasses to the phone and go to `Settings > Connected devices > Samsung DeX > Connected display`. Switch from "Extended" to "Mirrored" mode
   (Extended mode conflicts with the GlassesControl app).<br>
   Once this step is complete, disconnect the glasses (otherwise the GlassesControl app will keep trying to connect to them).
4. Install the GlassesControl (aka Control Glasses) app from the app store (see [here](https://play.google.com/store/apps/details?id=com.xreal.glassescontrol.store&hl=en-US))[^1.1].
5. In the GlassesControl app, ensure the Refresh Rate is set to 60hz and Screen Mirroring is disabled.
   Camera Access can remain enabled.
6. Connect the glasses to the phone. If successful, the GlassesControl app should open automatically
    and (after a moment) display a green check and a message starting with "SDK ready" under Status Monitor.
    The glasses' firmware version should also be displayed under Firmware Management.
7. Install `adb` (Android Debug Bridge) on the computer (see [here](https://developer.android.com/tools/adb))[^1.2].
8. (Optional) Add the install location of `adb` to the system Path.

[^1.1]: Do not use the GlassesControl APK provided on the XREAL SDK website, as it is not properly aligned for modern Android versions.
[^1.2]: Unity installs the Android SDK as part of the Android Build Support module (see step 1 of [Set up a Unity project](#set-up-a-unity-project)).
This means you can instead use its version of `adb`, which on my machine ended up at
`C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools`.

## Set up a Unity project

1. Install Unity Hub (see [here](https://unity.com/download); requires a Unity account) and the Unity 6.0 (6000.0.X) LTS version of the editor. Ensure the Android Build Support module is also
    installed.<br>
    Later editor versions seem presently incompatible with the XREAL SDK.
2. Create a new Unity project. Ensure the Editor version is set to 6000.0.X and select the "3D (Built-In Render Pipeline)"
    project template[^2.1]. Set the project name and location and click "Create project".
3. Go to `File > Build Profiles`, select Android in the list on the left, and click "Switch Platform".
4. Go to `Window > Package Manager`, select the Unity Registry, and search for and install "XR Interaction Toolkit".
    Once installed, import the Starter Assets and Hands Interaction Demo samples from the Samples tab of the package[^2.2].
5. Download XREAL Unity SDK 3.1.0 (see [here](https://developer.xreal.com/download/)). In the Unity Package Manager, click the
    "+" dropdown menu in the top left, then select "Install package from tarball..." and install the downloaded file. Once
    installed, import the Interaction Basics sample from the Samples tab.
6. Go to `Edit > Project Settings...`, select "XR Plug-In Management" on the left, select the Android platform tab, and enable 
    XREAL.
7. Select "Project Validation" on the left (under "XR Plug-In Management") and click "Fix All" if there are any issues.
8. (Optional) Select "Player" on the left and customize the Company Name, Product Name, and Version.
9. Go through the table of settings [here](https://docs.xreal.com/Getting%20Started%20with%20XREAL%20SDK#manual-configuration) to
    verify the project settings are correct. In particular, make sure "Minimum API Level" is set to Android 10.0 or higher.
10. Also verify that "Scripting Backend" is set to IL2CPP and ARM64 is selected under "Target Architectures".

[^2.1]: The Universal Render Pipeline (URP) should be supported by the XREAL SDK; however, it seems rather finnicky to get
working correctly, so sticking with the Built-In Render Pipeline is recommended unless and until you need URP.
[^2.2]: The XREAL SDK claims the Hands Demo is optional, but seems required for building the HelloMR sample scene.

## (Optional) Improve build speeds

Android builds can be (very) slow in Unity. To speed them up a bit while testing, consider the following:

1. Go to `Edit > Project Settings...`, select "Player" on the left, select the Android platform tab, and under
  `Other Settings > Configuration`, set "IL2CPP Code Generation" to "Optimize for code size and build time".
2. Set "C++ Compiler Configuration" to "Debug".

See [here](https://github.com/juicycleff/flutter-unity-view-widget/issues/643) for more information.

## (Optional) Set up wireless debugging

**Note**: While technically optional, this step is highly recommended to simplify on-device testing.

1. Enable wireless debugging on the phone (`Settings > Developer options > Wireless debugging`).
2. Pair the phone with the computer (see [here](https://developer.android.com/tools/adb#connect-to-a-device-over-wi-fi)).
3. Activate the connection by entering `adb connect <phone IP address and port>`.<br>
   The phone's IP address and port can be found at the top of the "Wireless debugging" settings page from step 1.

## Build and deploy a sample scene

To verify everything has been set up properly, it is recommended to test with the HelloMR sample scene provided by XREAL.

1. In the Unity project created above, open the "HelloMR" scene (`Assets/Samples/XREAL XR Plugin/3.0.0/Interaction Basics/HelloMR.unity`).
2. Go to `File > Build Profiles`, select "Scene List" on the left, click "Add Open Scenes", and deselect any scenes other than HelloMR.
3. Select "Android" on the left, click "Build", and select a location for the resulting APK.
4. Once the build is complete (which may take several minutes), open a terminal and run `adb install -r <apk file path>`. This
   should install the app on the phone after a bit of time.<br>
   **Note**: The `-r` flag causes any previous version of the app to be overwritten, so you do not need to uninstall the old version each
   time you want to test a new one.
5. Connect the XREAL glasses to the phone. Once the GlassesControl app has registered the glasses, launch the newly created app.
6. (Optional) Set the app to launch automatically when the glasses are first connected via the GlassesControl app.
