/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnswerUi : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private TextMeshProUGUI header;
		[SerializeField] private TMP_Dropdown nameDropdown;
		[SerializeField] private Slider timeSlider;
	#endregion
	
	#region Attributes
		// public float AttributeOne { get; set; }
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Start() {
			nameDropdown.ClearOptions();
			nameDropdown.AddOptions(MushroomData.UsedNames);
		}


	#endregion
	
	#region Other Methods
		public void SetAnswerFormat(Question q) {
			header.text = q.Type + " " + q.GetVerse(MushroomManager.AllShrooms[q.MushroomIndex]);
			nameDropdown.gameObject.SetActive(false);
			timeSlider.gameObject.SetActive(false);
			
			switch (q.Type) {
				case QuestionType.Who:
					nameDropdown.gameObject.SetActive(true);
					break;
				case QuestionType.When:
					timeSlider.gameObject.SetActive(true);
					break;
			}
		}
	#endregion
}
