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
		[SerializeField] private TMP_Dropdown dropdown;
		[SerializeField] private Incrementer incrementer;
		[SerializeField] private TimeSlider timeSlider;
	#endregion
	
	#region Attributes
		private static CardUi PartneredCard { get; set; }
	#endregion

	#region Other Methods
		public void SetCard(CardUi cardUi) {
			PartneredCard = cardUi;
			var q = PartneredCard.Question;
			
			header.text = q.Header + " " + q.ReplaceNameTemplate() + "?";
			dropdown.gameObject.SetActive(false);
			timeSlider.gameObject.SetActive(false);
			incrementer.gameObject.SetActive(false);
			
			switch (q.Header) {
				case "Where":
					dropdown.ClearOptions();
					dropdown.AddOptions(Location.All
						.Select(m => new TMP_Dropdown.OptionData(m.Name, null))
						.ToList());
					dropdown.gameObject.SetActive(true);
					break;
				case "What":
					dropdown.ClearOptions();
					dropdown.AddOptions(What.Choices);
					dropdown.gameObject.SetActive(true);
					break;
				case "Can":
					dropdown.ClearOptions();
					dropdown.AddOptions(Can.Choices);
					dropdown.gameObject.SetActive(true);
					break;
				case "How Many":
					incrementer.gameObject.SetActive(true);
					incrementer.SetValueWithoutNotify(0);
					break;
				case "Who":
					dropdown.ClearOptions();
					dropdown.AddOptions(Mushroom.All
						.Select(m => new TMP_Dropdown.OptionData(m.Data.Name, null))
						.ToList());
					dropdown.gameObject.SetActive(true);
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
				"Where" => dropdown.options[dropdown.value].text,
				"What" => dropdown.options[dropdown.value].text,
				"Can" => dropdown.options[dropdown.value].text,
				"How Many" => incrementer.Value,
				"Who" => dropdown.options[dropdown.value].text,
				"When" => Utility.FormatTime(timeSlider.CurrentTime+TimeManager.HourOffset),
				_ => ""
			};

			var isCorrect = q.IsRightAnswer(answer);
			
			GameManager.Instance.GuessAnswer(isCorrect);
			PartneredCard.Finished(isCorrect);
		}
	#endregion
}
