/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;
using UnityEngine.Playables;

public class Mushroom : MonoBehaviour {
	#region Serialized Fields
		private Question[] questions;
	#endregion
	
	#region Attributes
		public MushroomData Data => MushroomData.AllData[MushroomManager.AllActive.IndexOf(this)];
		public MeshRenderer MeshRenderer => _meshRenderer;
		public Question[] Questions => questions;
	#endregion
	
	#region Components
		private Transform _transform;
		private PlayableDirector _playableDirector;
	#endregion
	
	#region Private Data
		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private Material _clonedHeadMaterial, _clonedBodyMaterial;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshFilter = GetComponent<MeshFilter>();
			_playableDirector = GetComponent<PlayableDirector>();
		}

		private void Start() {
			_meshRenderer.enabled = false;
		}

	#endregion
	
	#region Other Methods
		public void SetMesh(MushroomModel modelData) {
			_meshFilter.mesh = modelData.Mesh;
			_meshRenderer.materials[1].mainTexture = modelData.HeadTextures[Data.HeadColorIndex];
			_meshRenderer.materials[0].mainTexture = modelData.BodyTextures[Data.BodyColorIndex];
		}
		public void SetTimeline(float f) {
			_playableDirector.time = f;
			_playableDirector.DeferredEvaluate();
		}

	#endregion
}
