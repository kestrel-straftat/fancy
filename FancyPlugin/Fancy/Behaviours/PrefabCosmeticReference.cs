using UnityEngine;

namespace Fancy
{
    public enum PrefabCosmeticType : byte
    {
        Hat,
        Cig
    }
    
    public class PrefabCosmeticReference : CosmeticReferenceBase
    {
        [Tooltip("The type of the cosmetic. Only determines which menu it will be shown in ingame, as both cigs and hats use the same transform root.")]
        public PrefabCosmeticType type;
        [Tooltip("The prefab to instantiate~ this will be instantiated ingame with its parent as the cosmetics anchor.")]
        public GameObject prefab;
    }
}