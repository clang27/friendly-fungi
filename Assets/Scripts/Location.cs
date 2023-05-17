/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class Location : MonoBehaviour, Highlightable {
	#region Serialized Fields
		[SerializeField] private int locationNumber;
		[SerializeField] private LocationType locationType;
	#endregion
	
	#region Attributes
		public string Name => LocationData.AllData[locationNumber].Name + " " + locationType;
		QuickOutline Highlightable.Outline => _outline;
	#endregion
	
	#region Components
		private QuickOutline _outline;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_outline = GetComponent<QuickOutline>();
		}
		
		private void Start() {
			
		}
	
	#endregion
	
	#region Other Methods
		public void Click() {
			Debug.Log(Name);
		}
	#endregion
}
