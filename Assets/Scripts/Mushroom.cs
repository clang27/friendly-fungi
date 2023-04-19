/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Linq;
using UnityEngine;

public class Mushroom : MonoBehaviour {
	#region Attributes
		public MushroomData Data => MushroomData.AllData[MushroomManager.AllActive.IndexOf(this)];
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
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
		}

		private void Start() {
			var modelData = MushroomManager.Models.First(mo => mo.Type == Data.Type);
			
			//Based off of data, retrieve mesh and textures from manager
			_meshFilter.mesh = modelData.Mesh;
			_meshRenderer.materials[1].mainTexture = modelData.HeadTextures[Data.HeadColorIndex];
			_meshRenderer.materials[0].mainTexture = modelData.BodyTextures[Data.BodyColorIndex];
		}

		#endregion
	
	#region Other Methods
		
	#endregion
}
