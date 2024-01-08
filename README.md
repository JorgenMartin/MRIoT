# Mixed Reality - Internet of Things


## Setup


### Unity Editor and modules

- Unity Editor v2023.2.4f1
  - Modules: "Android Build Support", "Universal Windows Platform Build Support"
  - Visual Studio with "Game Development with Unity"


### TextMeshPro-issues

> Due to changes in the 2023 version of Unity, TextMeshPro needs to manually be removed from dependencies

1. Download a tarball of the Meta MR/XR packages depending on TextMeshPro from [Meta NPM Server](https://npm.developer.oculus.com/):
   - [Meta XR Interaction SDK](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.sdk.interaction)
   - [Meta MR Utility Kit](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.mrutilitykit)
2. Extract the tarballs to a sensible location (e.g. `<User>/.apps/UnityPackageManager`), since it cannot be deleted/moved after import
2. Edit the `Package.json` of each package
   - Remove `com.unity.textmeshpro` from the dependencies:
     ```
     "dependencies": {
       ...
       "com.unity.textmeshpro": "3.0.6", <--- DELETE THIS LINE
       ...
     }
     ```
3. Open the project in Unity and open the Package Manager (Window > Package Manager)
4. Import the modified dependencies manually from file (+ > Install package from disk...)
   - __NB:__ Import in the order shown in step 1, due to order of inter-dependency relations
   - To import from disk select the `Package.json` of each package (one at a time)

