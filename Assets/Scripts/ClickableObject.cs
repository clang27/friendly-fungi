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
		private RaycastHit[] _groundHits = new RaycastHit[1];
	#endregion

	#region Other Methods
		public Transform TouchingRay(Ray ray) {
			var targetHit = Physics.RaycastNonAlloc(ray, _hits, 200f, layerMask);
			
			if (targetHit == 0 || !TutorialManager.JournalTabsCanOperate)
				return null;
			
			var groundHit = Physics.RaycastNonAlloc(ray, _groundHits, 200f, Utility.GroundMask);
			
			if (groundHit == 0)
				return _hits[0].transform;

			var distanceBetweenCameraAndGroundPoint = Vector3.Distance(ray.origin, _groundHits[0].point);
			var distanceBetweenCameraAndHitPoint = Vector3.Distance(ray.origin, _hits[0].point);

			return distanceBetweenCameraAndGroundPoint < distanceBetweenCameraAndHitPoint ? null : _hits[0].transform;
		}
		
	#endregion
}
