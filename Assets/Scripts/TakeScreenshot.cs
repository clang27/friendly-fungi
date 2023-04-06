using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Pinwheel.Jupiter {
    public class TakeScreenshot : MonoBehaviour {
        [SerializeField] private KeyCode hotKey;
        [SerializeField] private string fileNamePrefix;

        public KeyCode HotKey {
            get => hotKey;
            set => hotKey = value;
        }

        public string FileNamePrefix {
            get => fileNamePrefix;
            set => fileNamePrefix = value;
        }

        private void Reset() {
            HotKey = KeyCode.F9;
            FileNamePrefix = "Screenshot";
        }

        private void Update() {
            if (Input.GetKeyDown(HotKey))
                StartCoroutine(CrTakeScreenshot());
        }

        private IEnumerator CrTakeScreenshot() {
            // wait for graphics to render
            yield return new WaitForEndOfFrame();

            // create a texture to pass to encoding
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            yield return 0;

            var bytes = texture.EncodeToPNG();
            var d = DateTime.Now;
            var timeString = $"{d.Year}-{d.Month}-{d.Day}-{d.Hour}-{d.Minute}-{d.Second}";
            var fileName = $"{FileNamePrefix}{(FileNamePrefix == null ? "" : "-")}{timeString}.png";
            var filePath = Application.dataPath + "/" + fileName;
            File.WriteAllBytes(filePath, bytes);

            Destroy(texture);
            Debug.Log("Screenshot saved at: " + filePath);
        }
    }
}