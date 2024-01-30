# Mixed Reality - Internet of Things

The project is split into a HoloLens portion and a Meta Quest portion.
This is handled by using different scenes (`Assets/Scenes/HoloLens` and `MetaQuest`) and build target (`UWP` and `Android`).


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
  - Unity Hub with Unity Editor 2023.2.4f1, with
    - Universal Windows Platform Build Support
    - Android Build Support
- From [Mixed Reality Feature Tool](https://aka.ms/mrfeaturetool) (should be included when using Git LFS):
  - Mixed Reality OpenXR Plugin (1.10.0, under Platform Support)
  - Microsoft Spatializer (2.0.47, under Spatial Audio)
  - MRTK Input (3.0.0, under MRTK3)
  - MRTK UX Components (3.1.0, under MRTK3)
  - MRTK UX Components (Non-Canvas) (3.1.0, under MRTK3)
- From [Unity Asset Store](https://assetstore.unity.com) (handled by Unity):
  - [AutoSave by EckTech Games](https://assetstore.unity.com/packages/tools/utilities/autosave-43605) (1.1)
  - [Colourful Hierarchy Category GameObject by M STUDIO HUB](https://assetstore.unity.com/packages/p/colourful-hierarchy-category-gameobject-205934) (1.2)
  - [Lamp Model by HarpetStudio](https://assetstore.unity.com/packages/3d/props/interior/lamp-model-110960)
  - [NuGetForUnity (from Git)](https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity)
  - Meta XR SDKs (also see _Modified Packages_ below)
    - [Meta XR Audio SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-audio-sdk-264557) (60.0.0)
    - [Meta XR Haptics SDK](https://assetstore.unity.com/packages/tools/integration/meta-xr-haptics-sdk-272446) (60.0.0)
    - [Meta XR Interaction SDK OVR Integration](https://assetstore.unity.com/packages/tools/integration/meta-xr-interaction-sdk-ovr-integration-265014) (60.0.0)
- From NuGet (handled by included Unity package: [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)):
  - [Microsoft.MixedReality.QR](https://www.nuget.org/Packages/Microsoft.MixedReality.QR) (0.5.3037)
- Modified Packages (see [LocalPackages](./LocalPackages/README.md) for details)
  - Meta XR Core SDK (60.0.0)
  - Meta XR Interaction SDK (60.0.0)
  - MR Utility Kit (60.0.0)
  - MRTK UX Core Scripts (3.1.0)
- Other:
  - EXACT (copied over from https://github.com/DagSvanaes/Unity-Arduino-IoT.git)
  - NaughtyAttributes (installed from git: https://github.com/dbrizov/NaughtyAttributes.git#upm)


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


# Known Issues


## Meta XR Feature Group

The Feature Group for Meta XR must be __disabled__ under `OpenXR Feature Groups` for `Windows, Mac, Linux settings`-tab (aka. standalone) in `Project Settings > XR Plug-in Management > OpenXR`.

> _not tested_: This probably doesn't affect building for Android (Meta Quest), but the project will likely not work with QuestLink.
> However, this is necessary in order to run in-editor for the HoloLens (using Holographic Remoting).
>
> __NB:__ The code needs to check for UWP vs. Android when using libraries from each...
