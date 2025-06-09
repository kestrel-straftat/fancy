using UnityEngine;

namespace Fancy
{
    public class CosmeticReferenceBase : MonoBehaviour
    {
        [Tooltip("The name of the cosmetic.")]
        public string cosmeticName;
        [Tooltip("The icon of the cosmetic.")]
        public Sprite cosmeticIcon;
        [Tooltip("Whether the cosmetic requires the DLC to unlock.")]
        public bool dlcOnly;
        
        // used to order cosmetics while loading ingame
        [HideInInspector]
        public string sourceGuid;
    }
}