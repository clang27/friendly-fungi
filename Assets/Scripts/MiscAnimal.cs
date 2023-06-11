/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class MiscAnimal : MonoBehaviour {
	#region Serialized Data
		[SerializeField] private Color[] bodyTints;
		[SerializeField] private SkinnedMeshRenderer bodyMeshRenderer;
		[SerializeField] private int materialIndexToTint = 1;
	#endregion

	#region Private Data
		private Renderer[] _meshRenderers;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_meshRenderers = GetComponentsInChildren<Renderer>();
		}

		private void Start() {
			ApplyUniqueMaterials();
		}

	#endregion
	
	#region Other Methods
		private void ApplyUniqueMaterials() {
			bodyMeshRenderer.materials[materialIndexToTint].color = bodyTints[Random.Range(0, bodyTints.Length)];
		}
		
		public void EnableRenderers(bool b) {
			foreach (var mr in _meshRenderers)
				mr.enabled = b;
		}
	#endregion
}
