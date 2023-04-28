/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HeadshotCamera : MonoBehaviour {
	#region Attributes
		public Texture2D HeadshotTexture => _headshotTexture;
		public Sprite HeadshotSprite => _headshotSprite;
	#endregion
	
	#region Components
		private Camera _camera;
	#endregion
	
	#region Private Data
		private RenderTexture _renderTexture;
		private Texture2D _headshotTexture;
		private Sprite _headshotSprite;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_camera = GetComponentInChildren<Camera>();
			
			_renderTexture = new RenderTexture(Utility.HeadshotDimension, Utility.HeadshotDimension, 0) {
				dimension = TextureDimension.Tex2D
			};

			_camera.targetTexture = _renderTexture;
		}

		private void Start() {
			_camera.backgroundColor = Color.HSVToRGB(Random.Range(0f, 1f), 0.2f, 0.8f);
			_camera.gameObject.SetActive(false);
		}

	#endregion
	
	#region Other Methods
		public IEnumerator TakeHeadshot() {
			_camera.gameObject.SetActive(true);
			yield return new WaitForEndOfFrame();
			
			_headshotTexture = new Texture2D(Utility.HeadshotDimension, Utility.HeadshotDimension, TextureFormat.RGB24, false);
			RenderTexture.active = _renderTexture;
			_headshotTexture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
			_headshotTexture.Apply();

			_headshotSprite = Sprite.Create(_headshotTexture, new Rect(0f, 0f, Utility.HeadshotDimension, 
							Utility.HeadshotDimension), Vector2.zero);
			
			_camera.gameObject.SetActive(false);
		}
	#endregion
}
