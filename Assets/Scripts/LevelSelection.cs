/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Level[] levels;
	#endregion
	
	#region Attributes
		public static Level CurrentLevel { get; private set; }
		public static bool LevelLoaded { get; set; }
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		private int _currentSelection = 0;
		private Button _leftButton, _rightButton;
		private TextMeshProUGUI _levelNameText;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			CurrentLevel = levels[0];
			_levelNameText = GetComponentsInChildren<TextMeshProUGUI>()[0];
			_leftButton = GetComponentsInChildren<Button>()[1];
			_rightButton = GetComponentsInChildren<Button>()[0];
		}

		private void UpdateUI() {
			_levelNameText.text = CurrentLevel.LevelName;
			_levelNameText.color = CurrentLevel.Unlocked() ? Color.white : Color.red;
			_leftButton.gameObject.SetActive(_currentSelection == 0);
			_rightButton.gameObject.SetActive(_currentSelection == levels.Length-1);
		}
		
		private void Start() {
			StartCoroutine(GameManager.Instance.LoadLevel(CurrentLevel));
			UpdateUI();
		}
	#endregion
	
	#region Other Methods
		public void NextLevel() {
			_currentSelection++;
			StartCoroutine(GameManager.Instance.LoadLevel(levels[_currentSelection]));
			CurrentLevel = levels[_currentSelection];
			UpdateUI();
		}
		
		public void PreviousLevel() {
			_currentSelection--;
			StartCoroutine(GameManager.Instance.LoadLevel(levels[_currentSelection]));
			CurrentLevel = levels[_currentSelection];
			UpdateUI();
		}
	#endregion
}
