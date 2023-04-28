/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class MushroomClicker : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private LayerMask mushroomLayerMask;
	#endregion

	#region Private Data
		private RaycastHit[] _hits = new RaycastHit[1];
	#endregion

	#region Other Methods
		public void CheckForMushroomClick(Ray ray) {
			if (Physics.RaycastNonAlloc(ray, _hits, 100f, mushroomLayerMask) <= 0) return;
			
			var mushroom = _hits[0].transform.GetComponent<Mushroom>();
			GameManager.Instance.OpenJournalToMushroomPage(mushroom);
		}
		
	#endregion
}
