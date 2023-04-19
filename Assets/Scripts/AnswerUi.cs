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

	#region Other Methods
		public void SetAnswerFormat(Question q) {
			header.text = q.Type + " " + q.GetVerse(MushroomManager.AllActiveData[q.MushroomIndex]);
			nameDropdown.gameObject.SetActive(false);
			timeSlider.gameObject.SetActive(false);
			
			switch (q.Type) {
				case QuestionType.Who:
					nameDropdown.ClearOptions();
					nameDropdown.AddOptions(MushroomManager.AllActiveNames);
					nameDropdown.gameObject.SetActive(true);
					break;
				case QuestionType.When:
					timeSlider.gameObject.SetActive(true);
					break;
			}
		}
	#endregion
}
