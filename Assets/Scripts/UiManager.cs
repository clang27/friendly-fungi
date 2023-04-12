/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Button startButton;
		[SerializeField] private CanvasGroup menuPanel, mainPanel, settingsPanel, audioPanel, gameplayPanel, graphicsPanel;
	#endregion
	
	#region Attributes
		// public float AttributeOne { get; set; }
	#endregion
	
	#region Components
		private Transform _transform;
		// private Rigidbody2D _rigidbody;
		// private Collider2D _collider;
	#endregion
	
	#region Private Data
		private CanvasGroup _activePanel, _lastPanelOpen;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_lastPanelOpen = audioPanel;
		}
	#endregion
	
	#region Other Methods

		public void ChangeStartButton(string s, UnityAction a) {
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = s;
			startButton.onClick.RemoveAllListeners();
			startButton.onClick.AddListener(a);
		}
		
		public void DisableButtonsOnLoading(bool b) {
			startButton.interactable = !b;
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = b ? "Loading..." : "Start";
		}
		
		public void OpenMenu() {
			OpenPanel(menuPanel, true);
		}
		
		public void CloseMenu() {
			OpenPanel(menuPanel, false);
		}
		
		public void OpenSettings() {
			OpenPanel(settingsPanel, true);
			OpenPanel(mainPanel, false);
			OpenPanel(_lastPanelOpen, true);
		}

		public void OpenAudio() {
			CloseActivePanel();
			OpenPanel(audioPanel, true);
			_activePanel = audioPanel;
		}
			
		public void OpenGameplay() {
			CloseActivePanel();
			OpenPanel(gameplayPanel, true);
			_activePanel = gameplayPanel;
		}
			
		public void OpenGraphics() {
			CloseActivePanel();
			OpenPanel(graphicsPanel, true);
			_activePanel = graphicsPanel;
		}

		public void AcceptSettings() {
			_lastPanelOpen = _activePanel;
			CloseActivePanel();
			OpenPanel(settingsPanel, false);
			OpenPanel(mainPanel, true);
		}
			
		private void OpenPanel(CanvasGroup panel, bool b) {
			panel.alpha = b ? 1f : 0f;
			panel.interactable = b;
			panel.blocksRaycasts = b;
		}

		private void CloseActivePanel() {
			if (!_activePanel) return;
				
			OpenPanel(_activePanel, false);
			_activePanel = null;
		}
	#endregion
}
