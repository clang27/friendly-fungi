/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalTab : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private float popOutDistance = 6f;
	#endregion
	
	#region Components
		private RectTransform _rectTransform;
		private Button _button;
		private TextMeshProUGUI _textMesh;
	#endregion
	
	#region Private Data
		private float _startingWidth, _startingX;
		private bool _hiding;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_rectTransform = GetComponent<RectTransform>();
			_textMesh = GetComponentInChildren<TextMeshProUGUI>();
			_button = GetComponent<Button>();
			
			_startingWidth = _rectTransform.sizeDelta.x;
			_startingX = _rectTransform.localPosition.x;
		}
	#endregion
	
	#region Other Methods
		public void PopOut() {
			if (_hiding) return;
			
			_rectTransform.DOKill();
			_rectTransform.DOLocalMoveX(_startingX + popOutDistance, 0.5f);
			_rectTransform.DOSizeDelta(new Vector2(_startingWidth + popOutDistance, _rectTransform.sizeDelta.y), 0.5f);
		}
		
		public void PopIn() {
			if (_hiding) return;
			
			_rectTransform.DOKill();
			_rectTransform.DOLocalMoveX(_startingX, 0.5f);
			_rectTransform.DOSizeDelta(new Vector2(_startingWidth, _rectTransform.sizeDelta.y), 0.5f);
		}

		public void Hide() {
			_rectTransform.DOKill();
			_rectTransform.DOLocalMoveX(_startingX - _startingWidth, 0.5f);
			_rectTransform.DOSizeDelta(new Vector2(0f, _rectTransform.sizeDelta.y), 0.5f);
		}

		public void Enable(bool b) {
			if (_hiding == !b) return;
			
			_hiding = !b;
			
			if (!b)
				Hide();
			else
				PopIn();

			_button.interactable = b;
			_textMesh.gameObject.SetActive(b);
		}
	#endregion
}
