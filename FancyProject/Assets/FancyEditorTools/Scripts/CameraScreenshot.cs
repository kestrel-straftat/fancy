using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fancy
{
    // A simple script that takes screenshots from a camera to use when making thumbnails for cosmetics.
    [RequireComponent(typeof(Camera))]
    public class CameraScreenshot : MonoBehaviour
    {
        public int width = 256;
        public int height = 256;
        public string saveLocaton = "Assets/Textures";
        public string fileName = "thumb.png";

        private Camera m_cam;
        private Texture2D m_screenshotTexture;

        private void OnValidate() {
            m_cam = GetComponent<Camera>();
            m_cam.clearFlags = CameraClearFlags.SolidColor;
            m_cam.backgroundColor = Color.clear;
        }

        public void TakeAndSaveScreenshot() {
            TakeScreenshot();
            SaveScreenshot();
        }

        private void TakeScreenshot() {
            m_screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var screenRenderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            var camRenderTexture = m_cam.targetTexture;

            m_cam.targetTexture = screenRenderTexture;
            m_cam.Render();
            m_cam.targetTexture = camRenderTexture;

            RenderTexture.active = screenRenderTexture;
            m_screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            m_screenshotTexture.Apply();

            RenderTexture.ReleaseTemporary(screenRenderTexture);
        }

        private void SaveScreenshot() {
            if (!Directory.Exists(saveLocaton))
                Directory.CreateDirectory(saveLocaton);

            byte[] bytes = m_screenshotTexture.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(saveLocaton, fileName), bytes);
        }
    }
}
