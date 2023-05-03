/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Mushroom : MonoBehaviour {
	#region Attributes
		public MushroomData Data => MushroomData.AllData[MushroomManager.AllActive.IndexOf(this)];
		public MeshRenderer MeshRenderer => _meshRenderer;
		public HeadshotCamera HeadshotCamera => _headshotCamera;
	#endregion
	
	#region Components
		private Transform _transform;
		private PlayableDirector _playableDirector;
		private HeadshotCamera _headshotCamera;
		private QuickOutline _outline;
	#endregion
	
	#region Private Data
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private Material _clonedHeadMaterial, _clonedBodyMaterial;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_meshRenderer = GetComponent<MeshRenderer>();
			_outline = GetComponent<QuickOutline>();
			_meshFilter = GetComponent<MeshFilter>();
			_playableDirector = GetComponent<PlayableDirector>();
			_headshotCamera = GetComponent<HeadshotCamera>();
		}

		private void Start() {
			_meshRenderer.enabled = false;
		}

	#endregion
	
	#region Other Methods
		public void Highlight(bool b) {
			_outline.enabled = b;
		}
		public void SetMesh(MushroomModel modelData) {
			_meshFilter.mesh = modelData.Mesh;
			_meshRenderer.materials[1].mainTexture = modelData.HeadTextures[Data.HeadColorIndex];
			_meshRenderer.materials[0].mainTexture = modelData.BodyTextures[Data.BodyColorIndex];
		}
		public void SetTimeline(float f) {
			_playableDirector.time = f;
			_playableDirector.DeferredEvaluate();
		}
		
		public IEnumerator TakeHeadshot(float gap) {
			transform.position = new Vector3(gap, gap, gap);
			
			while (!MeshRenderer.isVisible) {
				yield return null;
			}

			StartCoroutine(HeadshotCamera.TakeHeadshot());
		}

	#endregion
}
