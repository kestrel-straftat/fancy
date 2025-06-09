using UnityEngine;

namespace Fancy
{
    public class SuitTestDummy : MonoBehaviour
    {
        public SkinnedMeshRenderer[] suitTargets;
        public SkinnedMeshRenderer[] armTargets;
        public GameObject arms;
    
        public void ToggleArms() => arms?.SetActive(!arms.activeSelf);
    }
}
