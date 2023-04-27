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
	#endregion
	
	#region Components
		private Camera _camera;
		private Transform _mushroomTransform;
	#endregion
	
	#region Private Data
		private RenderTexture _renderTexture;
		private Texture2D _headshotTexture;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_camera = GetComponentInChildren<Camera>();
			_mushroomTransform = transform;
			
			_renderTexture = new RenderTexture(Utility.HeadshotDimension, Utility.HeadshotDimension, 0) {
				dimension = TextureDimension.Tex2D,
				antiAliasing = 2
			};

			_camera.targetTexture = _renderTexture;
		}

		private void Start() {
			_camera.backgroundColor = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
			_camera.gameObject.SetActive(false);
		}

	#endregion
	
	#region Other Methods
		public IEnumerator TakeHeadshot(Vector3 shotLocation) {
			_camera.gameObject.SetActive(true);
			_mushroomTransform.position = shotLocation;
			yield return new WaitForEndOfFrame();
			
			_headshotTexture = new Texture2D(Utility.HeadshotDimension, Utility.HeadshotDimension, TextureFormat.RGB24, false);
			RenderTexture.active = _renderTexture;
			_headshotTexture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
			_headshotTexture.Apply();
			
			_camera.gameObject.SetActive(false);
		}
	#endregion
}
