/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class Location : MonoBehaviour, Highlightable {
	#region Serialized Fields
		[SerializeField] private int locationNumber;
		[SerializeField] private LocationType locationType;
		[SerializeField] private bool known;
	#endregion
	
	#region Attributes
		public string Name => LocationData.AllData[locationNumber].Name + " " + locationType;
		public bool Known => known;
		public string CurrentGuess { get; set; }
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
			GameManager.Instance.OpenSign(this);
		}
		
		public void Highlight(bool b) {
			if (b)
				AudioManager.Instance.PlayUiSound(UiSound.Hover);
			_outline.enabled = b;
		}
	#endregion
}
