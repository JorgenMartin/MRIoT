# Microsoft HoloLens 2 Client

The purpose of the HoloLens 2 Client is to act as the tracker to map out the devices and obstructions the Meta Quest 3 cannot detect and track at this time.


# Setup


## Unity Editor and modules

- Unity Editor v2023.2.4f1
  - Modules: "Android Build Support", "Universal Windows Platform Build Support"
  - Visual Studio with "Game Development with Unity"


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
- From [Mixed Reality Feature Tool](https://aka.ms/mrfeaturetool) (must be run separately, see the [documentation](https://learn.microsoft.com/en-gb/windows/mixed-reality/develop/unity/welcome-to-mr-feature-tool)):
  - Mixed Reality OpenXR Plugin (1.10.0, under Platform Support)
  - Microsoft Spatializer (2.0.47, under Spatial Audio)
  - MRTK Input (3.0.0, under MRTK3)
  - MRTK UX Components (3.1.0, under MRTK3)
  - MRTK UX Components (Non-Canvas) (3.1.0, under MRTK3)
- From NuGet (handled by included Unity plugin [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)):
  - [Microsoft.MixedReality.QR](https://www.nuget.org/Packages/Microsoft.MixedReality.QR) (0.5.3037)
