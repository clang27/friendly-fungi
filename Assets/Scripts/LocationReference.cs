/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using UnityEngine;

public class LocationReference : MonoBehaviour, Highlightable {
	#region Serialized Fields
		[SerializeField] private List<Location> locationsPointingAt;
	#endregion

	#region Components
		private QuickOutline _outline;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_outline = GetComponent<QuickOutline>();
		}

	#endregion
	
	#region Other Methods
		public void Click() {
			AudioManager.Instance.PlayUiSound(UiSound.ButtonClick);
			GameManager.Instance.OpenSign(locationsPointingAt);
		}
		
		public void Highlight(bool b) {
			if (b)
				AudioManager.Instance.PlayUiSound(UiSound.Hover);
			_outline.enabled = b;
		}
	#endregion
}
