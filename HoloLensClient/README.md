# Microsoft HoloLens 2 Client

The purpose of the HoloLens 2 Client is to act as the tracker to map out the devices and obstructions the Meta Quest 3 cannot detect and track at this time.


# Setup Help


## Dependencies

This project was set up following [this article by Microsoft](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/setting-up/setup-new-project).

- Build software necessary ([Microsoft article](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/install-the-tools?tabs=unity)):
  - Windows 10 or 11 (non N-version)
  - Visual Studio 2022, with
    - .NET desktop development
    - Desktop development with C++
    - Universal Windows Platform (UWP) development, with
      - Windows 10 SDK version 10.0.19041.0
      - USB Device Connectivity
      - C++ (v142) Universal Windows Platform tools
    - Game developmend with Unity
  - Unity Hub with Unity Editor 2022.3.16f1, with
    - Universal Windows Platform Build Support
- From [Mixed Reality Feature Tool](https://aka.ms/mrfeaturetool) (may need to be run separately, see the [documentation](https://learn.microsoft.com/en-gb/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool)):
  - Mixed Reality OpenXR Plugin (1.10.0, under Platform Support)
  - Microsoft Spatializer (2.0.47, under Spatial Audio)
  - MRTK Input (3.0.0, under MRTK3)
  - MRTK UX Components (3.1.0, under MRTK3)
  - MRTK UX Components (Non-Canvas) (3.1.0, under MRTK3)
- From NuGet (handled by included Unity plugin [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)):
  - [Microsoft.MixedReality.QR](https://www.nuget.org/Packages/Microsoft.MixedReality.QR) (0.5.3037)
- Other:
  - EXACT (copied over from https://github.com/DagSvanaes/Unity-Arduino-IoT.git)
  - NaughtyAttributes (installed from git: https://github.com/dbrizov/NaughtyAttributes.git#upm)


## Run on the HoloLens 2

Several options:

- [Run in Unity and connect to the HoloLens 2 with the Holographic Remoting Player](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/unity/preview-and-debug-your-app?tabs=openxr)
- [Build and deploy to the HoloLens 2 through Visual Studio](#build-and-deploy-using-visual-studio)
- Build and manually install on the HoloLens 2


### Build and Deploy using Visual Studio

[Microsoft article](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/test-and-deploy/hololens2-deployment)


#### Connect to the HoloLens 2 Device Portal

_Might be necessary for USB connection..._

[Microsoft article](https://learn.microsoft.com/en-us/windows/mixed-reality/develop/advanced-concepts/using-the-windows-device-portal)

- Connect the HoloLens 2 to the computer with a USB C cable
- Open `Settings > Updadate & Security > For Developers` on the HoloLens 2
- Scroll down and click on `Pair` on the HoloLens 2
- Open http://169.254.99.94/devicepair.htm on the computer
- Enter the pin from the HoloLens 2 on the computer, and create (and remember) username and password


# Development Help


## QRCodePrefabs

_QRCodePrefabs_ are the prefabs which a QRCode may be mapped to for Unity to instantiate a digital representation
of real-world objects. We will first cover how you may add a new QRCodePrefab, and then how you map a QRCode to a
specific prefab.


### Add a new QRCodePrefab

1. Create a new prefab in `/Assets/Prefabs/QRCodePrefabs`
2. Add the component `SpatialGraphNodeTracker` to the prefab
   - This will also add the component `QRDataVisualizer`, which is required for all
     QRCodePrefabs.
     - It may be replaced later to apply logic based on the content of the QR Code.
     - See `DebuggerPrefab` for an example.
     - To replace it, first add the new one, then remove the `QRDataVisualizer`.
3. Set the prefabs X-rotation to `180` degrees: `Transform.Rotation` to `180, 0, 0`
   - Lets us use a coordinate system where X, Y, Z is the width, height and (inverted) depth when looking at the side
     with the QR Code (see step 6 below)
4. Open the prefab
5. Add a `Cube` (Right-click in the Hierarchy-panel and select `3D Object > Cube`)
6. Set the cubes X-rotation to `-180` degrees: `Transform.Rotation` to `-180, 0, 0`
   - X will now point right along the QR Code from the upper left corner
   - Y will now point down along the QR Code from the upper left corner
   - Z will now point __out__ of the QR Code (Z-position will therefore need to be negative, see below)
7. Set the cubes `Transform.Scale` to reflect the physical size it represents in the real-world (in meters)
   - X: The width of the side with the QR Code
   - Y: The height of the side with the QR Code
   - Z: The depth of the object when looking at the side with the QR Code
   - Example from _Meta Quest 3 Box_
     - X, Y, Z = 21, 21.7, 12 cm = 0.21, 0.217, 0.12 meters
8. Set the cubes `Transform.Position` relative to the QR Codes real-world position:
   - Use positioning relative to where the QR Code is placed on the real-world object
     - `xScale`, `yScale` and `zScale` from `Transform.Scale`
     - `xMargin` represents the margin on the left side of the QR Code
     - `yMargin` represents the margin above the QR Code
   - X: `xScale / 2 - xMargin`
   - Y: `yScale / 2 - yMargin`
   - Z: `- zScale / 2`
     - NB: Z is negative since the cube should appear behind/below the QR Code
   - Example from using a QR Code placed with upper-left margins of 4.3cm above and 4.1cm to the left
     - X: xScale / 2 - 0.041
     - Y: yScale / 2 - 0.043
     - Z: - zScale / 2
9. You can now [map a QR Code to the new QRCodePrefab](#map-a-qr-code-to-a-qrcodeprefab)

### Map a QR Code to a QRCodePrefab

1. First make sure the QRCodePrefab you want to use exists, and contains the necessary components
   (see [above](#add-a-new-qrcodeprefab))
2. Select the `QRCodesManager` in the Scene
3. Add a new entry to the `Visualizer Prefab List` (`QRCodesManager > QRCodesVisualizer > VisualizerPrefabsList > +`)
   - Key: the text-value of the QR Code
   - Value: the `QRCodePrefab` to use
4. Save the Scene
