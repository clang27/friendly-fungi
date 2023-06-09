/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour, Highlightable {
	#region Serialized Fields
		[SerializeField] private Mushroom mushroomDependency;
		[SerializeField] private LocationType locationType;
		[SerializeField] private bool known;
		[SerializeField] private int index;
	#endregion
	
	#region Attributes
		public static List<Location> All { get; } = new();
		public string Name => (mushroomDependency) ?
			mushroomDependency.Data.Name + "\'s " + LocationData.AllData[index].Suffix(locationType) :
			LocationData.AllData[index].Name + " " + LocationData.AllData[index].Suffix(locationType);
		public bool Known => known;
		public string CurrentGuess { get; set; }
	#endregion
	
	#region Components
		private QuickOutline _outline;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_outline = GetComponent<QuickOutline>();
			
			All.Add(this);
		}

		private void OnDestroy() {
			All.Remove(this);
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
