/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUi : MonoBehaviour {
	#region Attributes
		public Question Question { get; private set; }
		private bool Selectable { get; set; }
	#endregion
	
	#region Components
		private RectTransform _rectTransform;
		private CanvasGroup _canvasGroup;
		private TextMeshProUGUI _header, _body;
		private AnswerUi _answerUi;
		private Image _image;
		private int _index;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_rectTransform = GetComponent<RectTransform>();
			_image = GetComponent<Image>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_header = GetComponentsInChildren<TextMeshProUGUI>()[0];
			_body = GetComponentsInChildren<TextMeshProUGUI>()[1];
			_answerUi = FindObjectOfType<AnswerUi>();

			DOTween.Init();
		}

		private void Start() {
			_index = int.Parse(name[^1].ToString()) - 1;
		}
		
	#endregion
	
	#region Other Methods
		public void ShowCard(bool b) {
			_canvasGroup.alpha = b ? 1f : 0f;
			_canvasGroup.interactable = b;
			_canvasGroup.blocksRaycasts = b;
		}

		public void SetQuestion(Question q) {
			Question = q;
			
			Selectable = true;
			
			_image.color = Color.HSVToRGB(60f / 360f, .27f, 1f);
			_header.text = Question.Header;
			_body.text = Question.ReplaceNameTemplate();
		}
		public void MoveRectX(Single x) {
			_rectTransform.DOMoveX(x, 0.5f);
			DOTween.Play(_rectTransform);
		}

		public void HighlightCard(bool b) {
			for (var i = 0; i < CardManager.AllCards.Count; i++) {
				if (!CardManager.AllCards[i]._canvasGroup.interactable) continue; // Don't change alpha of unselected cards
				CardManager.AllCards[i]._canvasGroup.DOFade((!b || i == _index) ? 1f : 0.5f, 0.5f);
			}
			DOTween.PlayAll();

			if (b) {
				transform.SetAsLastSibling();
			} else {
				transform.SetSiblingIndex(_index);
			}
		}

		public void SelectCard() {
			if (!Selectable) return;
			
			_answerUi.SetCard(this);
		}

		public void Finished(bool b) {
			Selectable = false;
			_image.color = Color.HSVToRGB(b ? 120f / 360f : 0f, .27f, 1f);
		}
		
	#endregion
}
