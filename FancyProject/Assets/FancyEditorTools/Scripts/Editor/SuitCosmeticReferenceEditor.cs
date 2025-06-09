using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Fancy
{
    [CustomEditor(typeof(SuitCosmeticReference))]
    public class SuitCosmeticReferenceEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var suitRef = (SuitCosmeticReference)target;

            if (GUILayout.Button("Apply Suit To Dummy")) {
                if (suitRef.dummy)
                    suitRef.ApplyToDummy();
                else {
                    Debug.LogError("[Fancy] Cannot apply suit to dummy: no dummy referenced!");
                }
            }

            if (GUILayout.Button("Toggle First Person Arms")) 
                suitRef.dummy.ToggleArms();
        }
    }
}
#endif