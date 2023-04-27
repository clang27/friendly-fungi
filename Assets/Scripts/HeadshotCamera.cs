/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HeadshotCamera : MonoBehaviour {
	#region Attributes
		public Texture HeadshotTexture => _headshotTexture;
	#endregion
	
	#region Components
		private Camera _camera;
		private Transform _mushroomTransform;
	#endregion
	
	#region Private Data
		private RenderTexture _renderTexture;
		private Texture _headshotTexture;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_camera = GetComponentInChildren<Camera>();
			_mushroomTransform = transform;
			
			_renderTexture = new RenderTexture(108, 108, 0) {
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
			_headshotTexture = _renderTexture;
			_camera.gameObject.SetActive(false);
		}
	#endregion
}
