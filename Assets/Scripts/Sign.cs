/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sign : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private int _normalPointSize = 64, _refPointSize = 42;
	#endregion
	
	#region Components
		private TMP_InputField _nameInput;
		private GameObject _normalSign, _refSign;
	#endregion
	
	#region Private Data
		private Location _currentLocationShowing;
	#endregion

	#region Unity Methods
		private void Awake() {
			_nameInput = transform.GetChild(2).GetComponent<TMP_InputField>();
			_normalSign = transform.GetChild(0).gameObject;
			_refSign = transform.GetChild(1).gameObject;
		}

	#endregion
	
	#region Other Methods
		public void UpdateGuess(string s) {
			_currentLocationShowing.CurrentGuess = s;
		}
		public void Show(Location l) {
			_currentLocationShowing = l;
			
			_normalSign.SetActive(true);
			_refSign.SetActive(false);
			
			_nameInput.SetTextWithoutNotify(l.Known ? l.Name : l.CurrentGuess);
			_nameInput.interactable = !l.Known;
			_nameInput.pointSize = _normalPointSize;
			_nameInput.readOnly = l.Known;

			if (!l.Known) {
				_nameInput.Select();
				_nameInput.caretPosition = _nameInput.text.Length;
			}
		}
		
		public void ShowReference(List<Location> l) {
			_normalSign.SetActive(false);
			_refSign.SetActive(true);

			_nameInput.interactable = false;
			_nameInput.pointSize = _refPointSize;
			_nameInput.readOnly = true;
			var t = "";
			
			foreach (var loc in l) {
				t += loc.Name;
				if (l.IndexOf(loc) != l.Count - 1)
					t += " &\n";
			}
			
			_nameInput.SetTextWithoutNotify(t);
		}
		
	#endregion
}
