/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class ClickableObject : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private LayerMask layerMask;
	#endregion

	#region Private Data
		private RaycastHit[] _hits = new RaycastHit[1];
	#endregion

	#region Other Methods
		public Transform TouchingRay(Ray ray) {
			return Physics.RaycastNonAlloc(ray, _hits, 100f, layerMask) <= 0 ? 
				null : _hits[0].transform;
		}
		
	#endregion
}
