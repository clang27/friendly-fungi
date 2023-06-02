/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private List<CardUi> Cards;
		[SerializeField] private CanvasGroup QuestionBackdrop;
	#endregion
	
	#region Attributes
		public static List<CardUi> AllCards { get; private set; }
		public bool Ready { get; private set; }
	#endregion

	#region Private Data
		private List<Question> _randomQuestions = new();
		private TextMeshProUGUI _prompt;
		private RectTransform _promptRect;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_prompt = QuestionBackdrop.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
			_promptRect = _prompt.GetComponent<RectTransform>();
			AllCards = Cards;
		}
		private void Start() {
			ResetCards();
		}
		
	#endregion
	
	#region Other Methods
		public void Init() {
			_randomQuestions.Clear();
			
			for (var i = 0; i < LevelSelection.CurrentLevel.NumberOfQuestions; i++) {
				var randomQuestion = QuestionQueue.AllQuestions[Random.Range(0, QuestionQueue.AllQuestions.Count)];
				while (_randomQuestions.Contains(randomQuestion)) {
					randomQuestion = QuestionQueue.AllQuestions[Random.Range(0, QuestionQueue.AllQuestions.Count)];
				}

				_randomQuestions.Add(randomQuestion);
			}
		}

		public void ResetCards() {
			foreach (var card in Cards) {
				card.HighlightCard(false);
				card.ShowCard( false);
				card.MakeCardInteractable(false);
			}
		}
		
		public void StartQuestionCardIntro() {
			ResetCards();
			Ready = false;

			_promptRect.anchoredPosition = new Vector2(0f, 40f);
			_prompt.text = "<swing>Answer  </swing><bounce><cyan>" + LevelSelection.CurrentLevel.NumberOfCorrectGuesses + 
			               "</cyan></bounce><swing>  of these to win!</swing>";
			
			var i = 0;
			var s1 = DOTween.Sequence();
			var s2 = DOTween.Sequence();
			foreach (var q in _randomQuestions) {
				Cards[i].SetQuestion(q);
				Cards[i].ShowCard(true);
				Cards[i].HideToRightOfScreen();
				Cards[i].PlayFlyInAnimation(s2);
				i++;
			}

			s2.PrependInterval(1.5f);
			s2.AppendCallback(() => {
				Ready = true;
				var j = 0;
				foreach (var q in _randomQuestions) {
					Cards[j].MakeCardInteractable(true);
					j++;
				}

				QuestionBackdrop.DOFade(0f, 2f).OnComplete(() => s1.Kill());
			});

			s1.Join(QuestionBackdrop.DOFade(1f, 0.5f));
			s1.Append(_promptRect.DOAnchorPosY(-100f, 1f));		
			s1.AppendCallback(() => s2.Play());
			
			s1.Play();
		}
	#endregion
}
