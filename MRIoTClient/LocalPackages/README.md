# Local Dependencies


## TextMeshPro-issues

> Due to changes in the 2023 version of Unity, TextMeshPro needs to manually be removed from dependencies

1. Download a tarball of the packages depending on TextMeshPro:
    - Meta MR/XR packages depending on TextMeshPro from [Meta NPM Server](https://npm.developer.oculus.com/):
        - [Meta XR Interaction SDK](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.sdk.interaction)
        - [Meta MR Utility Kit](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.mrutilitykit)
    - MRTK packages depending on TextMeshPro from Micorosft's [Mixed Reality Feature Tool](https://aka.ms/mrfeaturetool)
        - Mixed Reality Toolkit UX Core (`org.mixedrealitytoolkit.uxcore`)
2. Extract the tarball
   - E.g. using `tar -xvzf <TarballName>.tar.gz`
2. Edit `package/Package.json` of each package
   - Remove `com.unity.textmeshpro` from the dependencies:
     ```
     "dependencies": {
       ...
       "com.unity.textmeshpro": "3.0.6", <--- DELETE THIS LINE
       ...
     }
     ```
3. Repack each package as tarballs:
   - Ensure you're in the directory with the root `package` folder
   - Run `tar -cvzf <TarballName>.tar.gz package`
4. Move the tarball into this directory (`<GitRepo>/LocalPackages`)
5. Open the project in Unity and open the Package Manager (Window > Package Manager)
6. Import the modified dependencies manually from file (+ > Install package from tarball...)
   - __NB:__ Make sure imported dependencies are imported in the correct order to avoid inter-dependency leading to downloads from UPM-registry
   - Select the tarball from `<GitRepo>/LocalPackages`
7. Make sure you add new tarballs to Git
   - Other users should now be able to clone the repo and not have to modify each dependency manually


## Meta XR SDK warning

> The Meta XR SDK has an annoying warning dialog asking you to enable the OpenXR Feature Group for Standalone, even though it not required to build for Android target.
> The following fix changes the package to only request Android, not Standalone.

1. Download package
    - [Meta XR Core SDK](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.sdk.core)
2. Extract the tarball
   - E.g. using `tar -xvzf <TarballName>.tar.gz`
3. Edit `package/Editor/OpenXRFeatures/MetaXRFeatureEnables.cs`
    - From:
        ```
        if (needEnable && !unityRunningInBatchmode)
        {
            bool result =
                EditorUtility.DisplayDialog("Enable Meta XR Feature Set",
                    "Meta XR Feature Set must be enabled in OpenXR Feature Groups to support Oculus Utilities. Do you want to enable it now?",
                    "Enable", "Cancel");
            if (!result)
            {
                needEnable = false;
                EditorUtility.DisplayDialog("Meta XR Feature not enabled",
                    "You can enable Meta XR Feature Set in XR Plugin-in Management / OpenXR for using Oculus Utilities functionalities. Please enable it in both Standalone and Android settings.",
                    "Ok");
            }
        }

        if (needEnable)
        {
            if (featureSetStandalone != null && !featureSetStandalone.isEnabled)
            {
                Debug.Log("Meta XR Feature Set enabled on Standalone");
                featureSetStandalone.isEnabled = true;
                OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(BuildTargetGroup.Standalone);
            }

            if (featureSetAndroid != null && !featureSetAndroid.isEnabled)
            {
                Debug.Log("Meta XR Feature Set enabled on Android");
                featureSetAndroid.isEnabled = true;
                OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(BuildTargetGroup.Android);
            }
        }
        ```
    - To:
        ```
        if (needEnable && !unityRunningInBatchmode && featureSetAndroid != null && !featureSetAndroid.isEnabled)
        {
            bool result =
                EditorUtility.DisplayDialog("Enable Meta XR Feature Set for Android",
                    "Meta XR Feature Set must be enabled in OpenXR Feature Group for Android to support Oculus Utilities. Do you want to enable it now?",
                    "Enable", "Cancel");
            if (result)
            {
                Debug.Log("Meta XR Feature Set enabled on Android");
                featureSetAndroid.isEnabled = true;
                OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(BuildTargetGroup.Android);
            }
            else
            {
                needEnable = false;
                EditorUtility.DisplayDialog("Meta XR Feature not enabled",
                    "You can enable Meta XR Feature Set in XR Plugin-in Management / OpenXR for using Oculus Utilities functionalities.",
                    "Ok");
            }
        }
        ```
3. Repack each package as tarballs:
   - Ensure you're in the directory with the root `package` folder
   - Run `tar -cvzf <TarballName>.tar.gz package`
4. Move the tarball into this directory (`<GitRepo>/LocalPackages`)
5. Open the project in Unity and open the Package Manager (Window > Package Manager)
6. Import the modified dependencies manually from file (+ > Install package from tarball...)
   - __NB:__ Make sure imported dependencies are imported in the correct order to avoid inter-dependency leading to downloads from UPM-registry
   - Select the tarball from `<GitRepo>/LocalPackages`
7. Make sure you add new tarballs to Git
   - Other users should now be able to clone the repo and not have to modify each dependency manually
