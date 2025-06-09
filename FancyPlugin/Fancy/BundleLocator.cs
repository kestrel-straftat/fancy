using System.Collections;
using System.IO;
using UnityEngine;

namespace Fancy;

public static class BundleLocator
{
    // *somewhat* unique code that represents installed bundles and their versions
    public static uint InstalledCode { get; private set; }

    private static string m_bundleDirectory = Path.Combine(BepInEx.Paths.PluginPath, "CosmeticBundles");
    
    public static IEnumerator DiscoverAndLoadBundles() {
        if (!Directory.Exists(m_bundleDirectory))
            Directory.CreateDirectory(m_bundleDirectory);
        
        // we search every directory in plugins even though we have a specific bundle dir in order
        // to allow for packages installed from thunderstore to have their cosmetic bundles registered.
        // it's slightly inefficient but i don't know of a way to get a ts package to *only* install to
        // the CosmeticBundles dir.
        foreach (var path in Directory.GetFiles(BepInEx.Paths.PluginPath, "*.cbundle", SearchOption.AllDirectories)) {
            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            var fileName = Path.GetFileName(path);
            
            if (request.assetBundle) {
                if (!RegisterCosmeticsFromBundle(request.assetBundle)) {
                    Plugin.Logger.LogError($"Cosmetic manifest not found for \"{fileName}\", it will be skipped.");
                }
            }
            else {
                Plugin.Logger.LogError($"Failed to load bundle from \"{fileName}\", it will be skipped.");
            }
        }
        
        Plugin.Instance.StartCoroutine(CosmeticLoader.LoadAllWhenReady());
    }
    
    #pragma warning disable CS0618 // Type or member is obsolete (mainAsset)
    private static bool RegisterCosmeticsFromBundle(AssetBundle bundle) {
        var manifest = bundle.mainAsset as CosmeticBundleManifest;
        if (!manifest) return false;
        
        Plugin.Logger.LogInfo($"Registering cosmetics from {manifest.bundleGuid} (\"{manifest.bundleName}\") version {manifest.bundleVersion}");
        
        // hash guid + version and combine with the installed code
        var bundleHash = (manifest.bundleGuid + manifest.bundleVersion).GetHashCode();
        // max value will be 999999999 for a max of 9 digits in the code to ensure at least some usability
        InstalledCode = (InstalledCode + (uint)bundleHash) % 1000000000;

        foreach (var reference in manifest.cosmeticReferences) {
            CosmeticLoader.RegisterCosmetic(reference);
        }
        
        return true;
    }
    #pragma warning restore CS0618
}