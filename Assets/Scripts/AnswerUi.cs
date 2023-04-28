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

	#region Other Methods
		public void SetCard(CardUi cardUi) {
			PartneredCard = cardUi;
			var q = PartneredCard.Question;
			
			header.text = q.Header + " " + q.ReplaceNameTemplate();
			nameDropdown.gameObject.SetActive(false);
			timeSlider.gameObject.SetActive(false);
			
			switch (q.Header) {
				case "Who":
					nameDropdown.ClearOptions();
					var options = MushroomManager.AllActive
							.Select(m => new TMP_Dropdown.OptionData(m.Data.Name, m.HeadshotCamera.HeadshotSprite))
							.ToList();
					nameDropdown.AddOptions(options);
					nameDropdown.gameObject.SetActive(true);
					break;
				case "When":
					timeSlider.SetLevelTime(LevelSelection.CurrentLevel);
					timeSlider.gameObject.SetActive(true);
					break;
			}
			
			GameManager.Instance.OpenAnswer();
		}
		public void Guess() {
			var q = PartneredCard.Question;
			
			var answer = q.Header switch {
				"Who" => nameDropdown.options[nameDropdown.value].text,
				"When" => Utility.FormatTime(timeSlider.CurrentTime+TimeManager.HourOffset),
				_ => ""
			};

			var isCorrect = q.IsRightAnswer(answer);
			
			GameManager.Instance.GuessAnswer(isCorrect);
			PartneredCard.Finished(isCorrect);
		}
	#endregion
}
