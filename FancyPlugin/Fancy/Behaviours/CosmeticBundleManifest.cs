using UnityEngine;

namespace Fancy
{
    public class CosmeticBundleManifest : MonoBehaviour
    {
        [Tooltip("The name of the cosmetic bundle.")] 
        public string bundleName;
        [Tooltip("The author of the cosmetic bundle. Use your Thunderstore team name if applicable.")] 
        public string bundleAuthor;
        [Tooltip("A *UNIQUE* string that identifies your cosmetic bundle. It's suggested that this follows the format <author>.<bundle name> in all lowercase without spaces.\n" +
                 "For example: \"kestrel.examplebundle\"")]
        public string bundleGuid;
        [Tooltip("The version of the cosmetic bundle. It's suggested that you use semantic versioning (<major>.<minor>.<patch>)")]
        public string bundleVersion = "1.0.0";
    
        [Tooltip("A list of references to every cosmetic the bundle includes. Only cosmetics referenced here will be loaded ingame.")]
        public CosmeticReferenceBase[] cosmeticReferences;
    
        public string outputDirectory = "Assets/BuiltCosmeticBundles";
    }
}
