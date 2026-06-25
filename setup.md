# Getting Started with XREAL Development

## Hardware used

- XREAL One AR Glasses (model X1111)
- Samsung Galaxy S25
    - OS: Android 16
- Lenovo V15 G4 ABP Laptop
    - OS: Windows 11 Pro

## Setting up the phone and glasses

1. Update the firmware of the XREAL glasses (see [here](https://www.xreal.com/ota/))[^1].
2. Enable developer mode on the phone (see [here](https://www.android.com/intl/en_uk/articles/enable-android-developer-settings/)).
    This will be important for installing and debugging the app(s) we will develop.
3. Switch Samsung DeX from Extended to Mirrored mode (`Connected devices > Samsung DeX > Connected display`)[^2].
    It seems that Extended mode conflicts with the Control Glasses app.
    > // TODO Verify this
4. Install the Control Glasses (GlassesControl?) app provided by XREAL on the app store ([here](https://play.google.com/store/apps/details?id=com.xreal.glassescontrol.store&hl=en-US)).
    Note: Do not use the version provided on the XREAL SDK website, as it is not properly aligned for modern Android versions.
5. In the Control Glasses app, ensure the Refresh Rate is set to 60hz and Screen Mirroring is disabled.
    Note: Camera Access can remain enabled.
6. Connect the XREAL glasses to the phone. If successful, the Control Glasses app should open automatically
    and (after a moment) display a green check and a message starting with "SDK ready" under Status Monitor.
    The glasses' firmware version should also be displayed under Firmware Management.

### Notes

[^1]: It appears this *must* be done using Google Chrome as your browser.
[^2]: This can only be done while the glasses are connected to the phone. Once this step is complete, disconnect the glasses.

## Setting up a Unity project

1. Install Unity Hub and the Unity 6.0 (6000.0.X) LTS version of the editor. Ensure the Android Build Support module is also
    installed.
    Note: Later editor versions seem presently incompatible with the XREAL SDK.
2. Create a new Unity project. Ensure the Editor version is set to 6000.0.X. Select the "3D (Built-In Render Pipeline)"
    project template. Set the project name and location and click "Create project".
    Note: The Universal Render Pipeline (URP) should be supported by the XREAL SDK; however, it seems rather finnicky to get
    working correctly, so I strongly suggest sticking with the Built-In Render Pipeline unless and until you need URP.
3. Go to ~File > Build Profiles~, select Android in the list on the left, and click ~Switch Platform~.
4. Go to ~Window > Package Manager~, select the Unity Registry, and search for and install "XR Interaction Toolkit".
    Once installed, import Starter Assets and Hands Interaction Demo from the Samples tab of the package.
    Note: The XREAL SDK claims the Hands Demo is optional, but seems required for HelloMR. // TODO Verify
5. Download XREAL Unity SDK 3.1.0 (see [here](https://developer.xreal.com/download/)). In the Unity Package Manager, click the
    "+" dropdown menu in the top left, then select "Install package from tarball..." and install the downloaded file. Once
    installed, import Interaction Basics from the Samples tab.
6. Go to ~Edit > Project Settings...~, select "XR Plug-In Management" on the left, select the Android platform tab, and enable 
    XREAL.
7. Now select "Project Validation" on the left (under "XR Plug-In Management") and click "Fix All" if there are any issues.
8. (Optional) Select "Player" on the left and customize the Company Name, Product Name, and Version.
9. Go through the table of settings [here](https://docs.xreal.com/Getting%20Started%20with%20XREAL%20SDK#manual-configuration) to
    verify the project settings are correct. In particular, make sure the Minimum API Level is set to Android 10.0 or higher.
10. Also verify that Scripting Backend is set to IL2CPP and ARM64 is selected under Target Architectures.

## (Optional) Improving build speeds

Android builds can be slow. To speed them up while testing, consider the following:

1. Go to ~Edit > Project Settings...~, select "Player" on the left, select the Android platform tab, and set "IL2CPP Code
    Generation" to "Optimize for code size and build time" (~Other Settings > Configuration~).
2. Set "C++ Compiler Configuration" to "Debug".

See [here](https://github.com/juicycleff/flutter-unity-view-widget/issues/643) for more information.

## (Optional) Setting up wireless debugging

Note: While optional, this step is highly recommended to simplify on-device testing.

1. Enable wireless debugging on the phone (Settings > Developer options > Wireless debugging).
2. Install adb (Android Debug Bridge) on the computer (see [here](https://developer.android.com/tools/adb)).
    Note: Unity installs the Android SDK as part of the Android build tools. This means you can use its version of
    adb, which is located at ~C:\Program Files\Unity\Hub\Editor\6000.0.77f1\Editor\Data\PlaybackEngines\AndroidPlayer\SDK\platform-tools~.
    I added this location to my Path for ease of use.
3. Pair the phone with the computer (see [here](https://developer.android.com/tools/adb#connect-to-a-device-over-wi-fi)).
4. Activate the connection by entering `adb connect <phone IP address and port>`.
    Note: The IP address and port can be found at the top of the Wireless debugging settings page.

## Building and deploying the HelloMR sample scene

1. In the Unity project created above, open the ~HelloMR~ scene (~Assets/Samples/XREAL XR Plugin/3.0.0/Interaction Basics/HelloMR.unity~).
2. Go to ~File > Build Profiles~, select "Scene List" on the left, click "Add Open Scenes", and deselect any scenes other than
    HelloMR.
3. Now select "Android" on the left, click "Build", and select a location for the resulting `.apk` file.
4. Once the build is complete (which may take several minutes), open a terminal and run `adb install -r <apk file path>`. This
    should install the app on the phone.
5. Connect the XREAL glasses to the phone. Once the Control Glasses app has registered the glasses, launch the newly created app.

## Testing the XREAL Eye

1. In the Unity Package Manager, import the Camera Features sample from the XREAL XR Plugin package.
2. Open the RGBCameraAndCapture demo scene (~Assets/Samples/XREAL XR Plugin/3.1.0/Camera Features/RGBCameraAndCapture~).
3. In Project Settings, under ~XR Plug-In Management > XREAL~, expand the "Additional Permissions" list and add `RECORD_AUDIO`
    and `FOREGROUND_SERVICE_MEDIA_PROJECTION`.
