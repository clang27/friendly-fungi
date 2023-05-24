/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Threading.Tasks;
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
		private Image _backdropImage, _borderImage;
		private int _index;
	#endregion
	
	#region Private Data

	private Vector3 _localStartingPosition;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_rectTransform = GetComponent<RectTransform>();
			_backdropImage = GetComponent<Image>();
			_borderImage = transform.GetChild(0).GetComponent<Image>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_header = GetComponentsInChildren<TextMeshProUGUI>()[0];
			_body = GetComponentsInChildren<TextMeshProUGUI>()[1];
			_answerUi = FindObjectOfType<AnswerUi>();

			DOTween.Init();
		}

		private void Start() {
			_localStartingPosition = _rectTransform.localPosition;
			_index = int.Parse(name[^1].ToString()) - 1;
		}
		
	#endregion
	
	#region Other Methods
		public void ShowCard(bool b) {
			_canvasGroup.alpha = b ? 0.8f : 0f;
		}
		
		public void MakeCardInteractable(bool b) {
			_canvasGroup.interactable = b;
			_canvasGroup.blocksRaycasts = b;
		}

		public void SetQuestion(Question q) {
			Question = q;
			
			Selectable = true;
			
			_backdropImage.color = GetBGColor(q);
			_borderImage.color = GetBorderColor(q);
			_header.text = Question.Header;
			_header.color = GetTextColor(q);
			_body.text = Question.ReplaceNameTemplate();
		}
		public void MoveRectX(Single x) {
			_rectTransform.DOLocalMoveX(x, 0.5f).SetEase(Ease.OutQuad);
		}

		public void HideToRightOfScreen() {
			_rectTransform.position = new Vector3(Screen.width+_rectTransform.rect.width/2f, Screen.height/2f, 0f);
		}

		public void PlayFlyInAnimation(in Sequence s) {
			s.PrependCallback(() => { _canvasGroup.alpha = 1f; });
			s.Append(
				_rectTransform.DOJump(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f), 20f, 3, 2f)
			);
			s.AppendInterval(1f);
			s.Append(
				_rectTransform.DOLocalMove(_localStartingPosition, 0.5f).SetEase(Ease.Linear)
			);
			s.AppendCallback(() => _canvasGroup.DOFade(0.8f, 0.5f));
		}

		public void HighlightCard(bool b) {
			for (var i = 0; i < CardManager.AllCards.Count; i++) {
				if (!CardManager.AllCards[i]._canvasGroup.interactable) continue; // Don't change alpha of unselected cards
				
				if (!b)
					CardManager.AllCards[i]._canvasGroup.DOFade(0.8f, 0.5f);
				else 
					CardManager.AllCards[i]._canvasGroup.DOFade((i == _index) ? 1f : 0.5f, 0.5f);
			}

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
			_backdropImage.color = Color.HSVToRGB(b ? 120f / 360f : 0f, .27f, 1f);
		}

		private Color GetBorderColor(Question q) {
			if (q.Header.Equals("What")) {
				return Color.HSVToRGB(216f / 360f, 1f, 0.8f);
			}
			if (q.Header.Equals("When")) {
				return Color.HSVToRGB(266f / 360f, 1f, 0.8f);
			}
			if (q.Header.Equals("Where")) {
				return Color.HSVToRGB(166f / 360f, 1f, 0.8f);
			}
				
			return Color.HSVToRGB(116f / 360f, 1f, 0.8f);
		}
		
		private Color GetTextColor(Question q) {
			if (q.Header.Equals("What")) {
				return Color.HSVToRGB(216f / 360f, 0.48f, 1f);
			}
			if (q.Header.Equals("When")) {
				return Color.HSVToRGB(266f / 360f, 0.48f, 1f);
			}
			if (q.Header.Equals("Where")) {
				return Color.HSVToRGB(166f / 360f, 0.48f, 1f);
			}
				
			return Color.HSVToRGB(116f / 360f, 0.48f, 1f);
		}
		
		private Color GetBGColor(Question q) {
			if (q.Header.Equals("What")) {
				return Color.HSVToRGB(216f / 360f, 0.05f, 0.9f);
			}
			if (q.Header.Equals("When")) {
				return Color.HSVToRGB(266f / 360f, 0.05f, 0.9f);
			}
			if (q.Header.Equals("Where")) {
				return Color.HSVToRGB(166f / 360f, 0.05f, 0.9f);
			}
				
			return Color.HSVToRGB(116f / 360f, 0.05f, 0.9f);
		}
		
	#endregion
}
