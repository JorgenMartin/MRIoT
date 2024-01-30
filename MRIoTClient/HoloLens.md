# Microsoft HoloLens 2 Client

The purpose of the HoloLens 2 Client is to act as the tracker to map out the devices and obstructions the Meta Quest 3 cannot detect and track at this time.


# Setup Help


## Dependencies

This project was set up following [this article by Microsoft](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk3-overview/getting-started/setting-up/setup-new-project).

For a full list of dependencies, see [README.md](./README.md).


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


# Permissions/Capabilities

The HoloLensClient requests the following capabilities from the OS:
- Internet (Client)
- Internet (Client & Server)
- Private Network (Client & Server)
- WebCam
- SpatialPerception
- GazeInput
