/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;

public class AnswerUi : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private TextMeshProUGUI header;
		[SerializeField] private TMP_Dropdown nameDropdown;
		[SerializeField] private TimeSlider timeSlider;
	#endregion
	
	#region Attributes
		private static CardUi PartneredCard { get; set; }
	#endregion
	
	#region Components
		private GameManager _gameManager;
	#endregion

	#region Other Methods
		private void Awake() {
			_gameManager = FindObjectOfType<GameManager>();
		}
		public void SetCard(CardUi cardUi) {
			PartneredCard = cardUi;
			var q = PartneredCard.Question;
			
			header.text = q.Header + " " + q.ReplaceNameTemplate();
			nameDropdown.gameObject.SetActive(false);
			timeSlider.gameObject.SetActive(false);
			
			switch (q.Header) {
				case "Who":
					nameDropdown.ClearOptions();
					nameDropdown.AddOptions(MushroomManager.AllActiveNames);
					nameDropdown.gameObject.SetActive(true);
					break;
				case "When":
					timeSlider.SetLevelTime(LevelSelection.CurrentLevel);
					timeSlider.gameObject.SetActive(true);
					break;
			}
			
			_gameManager.OpenAnswer();
		}
		public void Guess() {
			var q = PartneredCard.Question;
			
			var answer = q.Header switch {
				"Who" => nameDropdown.options[nameDropdown.value].text,
				"When" => Utility.FormatTime(timeSlider.CurrentTime+TimeManager.HourOffset),
				_ => ""
			};

			var isCorrect = q.IsRightAnswer(answer);
			_gameManager.GuessAnswer(isCorrect);
			_gameManager.CloseAnswer();
			
			PartneredCard.Finished(isCorrect);
		}
	#endregion
}
