/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private List<Question> Questions;
		[SerializeField] private List<CardUi> Cards;
	#endregion
	
	#region Attributes
		// public float AttributeOne { get; set; }
	#endregion
	
	#region Components
		public static List<CardUi> AllCards { get; private set; }
	#endregion
	
	#region Private Data
	#endregion
	
	#region Unity Methods
		private void Awake() {
			AllCards = Cards;
		}
		private void Start() {
			ResetQuestions();
		}
		
	#endregion
	
	#region Other Methods
		public void PickRandomQuestions(int num) {
			var index = num-1;
			while (index >= 0) {
				var question = Questions.First(q1 => 
					!Cards.Select(c => c.Question).Any(q2 => q2 && q2.Equals(q1)));
				Cards[index].SetQuestion(question);
				Cards[index].ShowCard(true);
				index--;
			}
		}

		public void ResetQuestions() {
			foreach (var card in Cards) {
				card.ClearQuestion();
				card.HighlightCard(false);
				card.ShowCard( false);
				card.MoveRectX(-20f);
			}
		}
	#endregion
}
