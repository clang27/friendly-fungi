/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Linq;
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
					var options = MushroomManager.AllActiveOptions
							.Select(o => new TMP_Dropdown.OptionData(o.Key, o.Value))
							.ToList();
					nameDropdown.AddOptions(options);
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
			PartneredCard.Finished(isCorrect);
		}
	#endregion
}
