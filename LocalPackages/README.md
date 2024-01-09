# Local Dependencies


## TextMeshPro-issues

> Due to changes in the 2023 version of Unity, TextMeshPro needs to manually be removed from dependencies

1. Download a tarball of the Meta MR/XR packages depending on TextMeshPro from [Meta NPM Server](https://npm.developer.oculus.com/):
   - [Meta XR Interaction SDK](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.sdk.interaction)
   - [Meta MR Utility Kit](https://npm.developer.oculus.com/-/web/detail/com.meta.xr.mrutilitykit)
2. Edit the `Package.json` of each package
   - Remove `com.unity.textmeshpro` from the dependencies:
     ```
     "dependencies": {
       ...
       "com.unity.textmeshpro": "3.0.6", <--- DELETE THIS LINE
       ...
     }
     ```
3. Repack each package as tarballs:
   - Assuming the tarballs is extracted as `<TarballName>/<TarballName>/package/...`
   - Navigate to `<TarballName>/<TarballName>/`
   - Run `tar -cvzf <TarballName>.tar.gz package`
4. Move the tarball into this directory (`<GitRepo>/LocalPackages`)
5. Open the project in Unity and open the Package Manager (Window > Package Manager)
6. Import the modified dependencies manually from file (+ > Install package from tarball...)
   - __NB:__ Make sure imported dependencies are imported in the correct order to avoid inter-dependency leading to downloads from UPM-registry
   - Select the tarball from `<GitRepo>/LocalPackages`
7. Make sure you add new tarballs to Git
   - Other users should now be able to clone the repo and not have to modify each dependency manually

