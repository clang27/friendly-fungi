/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour {
	#region Serialized Fields
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
			ResetCards();
		}
		
	#endregion
	
	#region Other Methods
		public void Init() {
			var randomQuestions = new List<Question>();
			
			for (var i = 0; i < LevelSelection.CurrentLevel.NumberOfQuestions; i++) {
				var randomQuestion = QuestionQueue.AllQuestions[Random.Range(0, QuestionQueue.AllQuestions.Count)];
				while (randomQuestions.Contains(randomQuestion)) {
					randomQuestion = QuestionQueue.AllQuestions[Random.Range(0, QuestionQueue.AllQuestions.Count)];
				}

				randomQuestions.Add(randomQuestion);
			}
			
			var index = 0;
			foreach (var q in randomQuestions) {
				Cards[index].SetQuestion(q);
				Cards[index].ShowCard(true);
				index++;
			}
		}

		public void ResetCards() {
			foreach (var card in Cards) {
				card.HighlightCard(false);
				card.ShowCard( false);
				card.MoveRectX(-20f);
			}
		}
	#endregion
}
