using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Fancy
{
    [CustomEditor(typeof(CosmeticBundleManifest))]
    public class CosmeticBundleManifestEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Build Bundle"))
                Build((CosmeticBundleManifest)target);
        }

        private static void Build(CosmeticBundleManifest manifest) {
            if (manifest.bundleName == string.Empty) {
                Debug.LogError("[Fancy] Cannot build: bundle does not have a name!");
                return;
            }
            if (manifest.bundleGuid == string.Empty) {
                Debug.LogError("[Fancy] Cannot build: bundle does not have a guid!");
                return;
            }
            
            if (!Directory.Exists(manifest.outputDirectory)) {
                Directory.CreateDirectory(manifest.outputDirectory);
            }
            
            // since only prefabs can be packed into assetbundles, generate a temporary prefab for the bundle using PrefabUtility
            if (!PrefabUtility.IsPartOfAnyPrefab(manifest) || PrefabUtility.IsPartOfPrefabInstance(manifest)) {
                Debug.Log("[Fancy] Requested build on non-prefab, generating one..");
                var prefabPath = Path.Combine(manifest.outputDirectory, manifest.bundleGuid + ".prefab");
                Build(PrefabUtility.SaveAsPrefabAsset(manifest.gameObject, prefabPath).GetComponent<CosmeticBundleManifest>());
                AssetDatabase.DeleteAsset(prefabPath);
                return;
            }
            
            Debug.Log($"[Fancy] Building bundle \"{manifest.bundleGuid}\"");
            
            for (int i = 0; i < manifest.cosmeticReferences.Length; ++i) {
                var reference = manifest.cosmeticReferences[i];
                
                // check for common mistakes
                if (!ValidateReference(reference, i, out var error)) {
                    Debug.LogError("[Fancy] " + error);
                    return;
                }
                reference.sourceGuid = manifest.bundleGuid;
            }

            var outputPath = Path.Combine(manifest.outputDirectory, manifest.bundleGuid + ".cbundle");
            
            #pragma warning disable CS0618 // Type or member is obsolete
            BuildPipeline.BuildAssetBundle(manifest, null, outputPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.StandaloneWindows);
            #pragma warning restore CS0618
            
            Debug.Log($"[Fancy] Finished building bundle \"{manifest.bundleGuid}\"");
            EditorUtility.RevealInFinder(outputPath);
        }

        private static bool ValidateReference(CosmeticReferenceBase reference, int index, out string error) {
            switch (reference) {
                case null:
                    error = $"Missing cosmetic reference at index {index}! (is the object referred to a child of your manifest?)";
                    return false;
                case PrefabCosmeticReference pcr:
                    // check that the cosmetic contains a non null prefab
                    if (!pcr.prefab) {
                        error = $"Prefab cosmetic reference at index {index} (\"{reference.cosmeticName}\") is missing a prefab!";
                        return false;
                    }

                    // check that the reference has not been assigned to itself, which proved to be a fairly common issue in testing
                    if (pcr.prefab.GetComponent<PrefabCosmeticReference>()) {
                        error = $"Prefab cosmetic reference at index {index} (\"{reference.cosmeticName}\") has been assigned " +
                                "to itself as a prefab! The prefab field should contain the actual gameobject of the cosmetic.";
                        return false;
                    }

                    // check that hats have colliders. this ensures that all custom hats fall off properly when shot
                    if (pcr.type == PrefabCosmeticType.Hat && !pcr.prefab.GetComponentInChildren<Collider>()) {
                        error = $"No colliders are present on the hat \"{reference.cosmeticName}\" (or they are disabled)! " +
                                "Colliders are required for the hat to register hits from gunshots. Add one then try again.";
                        return false;
                    }
                    
                    break;
                case SuitCosmeticReference scr:
                    if (!scr.suitMaterial || !scr.armsMaterial) {
                        error = $"Suit cosmetic reference at index {index} (\"{reference.cosmeticName}\") is missing a material!";
                        return false;
                    }
                    
                    break;
            }

            error = string.Empty;
            return true;
        }
    }
}
#endif