using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
namespace Fancy
{
    [CustomEditor(typeof(CameraScreenshot))]
    public class CameraScreenshotEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Take Screenshot"))
                ((CameraScreenshot)target).TakeAndSaveScreenshot();
        }
    }
}
#endif