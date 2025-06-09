using UnityEngine;

namespace Fancy
{
    public class SuitCosmeticReference : CosmeticReferenceBase
    {
        [Tooltip("The material that will be applied to the suit.")]
        public Material suitMaterial;
        [Tooltip("The material that will be applied to the suit's first person arms.")]
        public Material armsMaterial;

        [Tooltip("The dummy used to test these suits. Optional.")]
        public SuitTestDummy dummy;
    
        public void ApplyToDummy() {
            if (!dummy) {
                Debug.LogError("[Fancy] No dummy assigned- assign one to test suits.");
                return;
            }
            foreach (var target in dummy.suitTargets)
                target.material = suitMaterial;
        
            foreach (var target in dummy.armTargets)
                target.material = armsMaterial;
        }
    }
}