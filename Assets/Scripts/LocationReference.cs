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
	
	#region Attributes
		QuickOutline Highlightable.Outline => _outline;
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
			GameManager.Instance.OpenSign(locationsPointingAt);
		}
	#endregion
}
