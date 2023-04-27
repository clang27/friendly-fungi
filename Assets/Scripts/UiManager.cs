/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	#region Serialized Fields

	[SerializeField] private RawImage loadingScreen;
	[SerializeField] private Button startButton;

	[SerializeField] private CanvasGroup menuPanel,
		cardPanel,
		settingsPanel,
		audioPanel,
		gameplayPanel,
		graphicsPanel,
		topBar,
		answerPanel,
		journalPanel,
		promptPanel;

	#endregion

	#region Private Data
		private CanvasGroup _activePanel, _lastPanelOpen;
		private Journal _journal;
	#endregion

	#region Unity Methods

		private void Awake() {
			_lastPanelOpen = audioPanel;
			_journal = journalPanel.GetComponent<Journal>();
		}

	#endregion

	#region Other Methods
		public void ShowLoadingScreen(bool b) {
			loadingScreen.DOFade(b ? 1f : 0f, b ? 0.2f : 1f);
			DOTween.Play(loadingScreen);
		}

		public bool LoadingFadingIn() {
			return DOTween.IsTweening(loadingScreen);
		}

		public void ChangeStartButton(string s, UnityAction a) {
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = s;
			startButton.onClick.RemoveAllListeners();
			startButton.onClick.AddListener(a);
		}

		public void DisableButtonsOnLoading(bool b) {
			startButton.interactable = !b;
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = b ? "Loading..." : "Start";
		}

		public void OpenMainMenu() {
			OpenPanel(menuPanel, true);
		}

		public void CloseMainMenu() {
			OpenPanel(menuPanel, false);
		}
		public void OpenSettings() {
			OpenPanel(settingsPanel, true);
			OpenPanel(_lastPanelOpen, true);
			_activePanel = _lastPanelOpen;
		}

		public void CloseSettings() {
			_lastPanelOpen = _activePanel;
			CloseActivePanel();
			OpenPanel(settingsPanel, false);
			Settings.SaveAllData();
		}
		
		public void OpenJournal() {
			_journal.PopulateHeadshots();
			OpenPanel(journalPanel, true);
		}

		public void CloseJournal() {
			OpenPanel(journalPanel, false);
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
		public void OpenPrompt(string question, string optionOneLabel, string optionTwoLabel, UnityAction optionOneAction, UnityAction optionTwoAction) {
			OpenPanel(promptPanel, true);
			
			var textMeshes = promptPanel.GetComponentsInChildren<TextMeshProUGUI>();
			var buttons = promptPanel.GetComponentsInChildren<Button>();

			textMeshes[0].text = question;
			
			textMeshes[1].text = optionOneLabel;
			buttons[0].onClick.RemoveAllListeners();
			buttons[0].onClick.AddListener(optionOneAction);
			
			textMeshes[2].text = optionTwoLabel;
			buttons[1].onClick.RemoveAllListeners();
			buttons[1].onClick.AddListener(optionTwoAction);
		}
		public void ClosePrompt() {
			OpenPanel(promptPanel, false);
		}
		public void ShowTopBar(bool b) {
			OpenPanel(topBar, b);
		}
		
		public void ShowCardPanel(bool b) {
			OpenPanel(cardPanel, b);
		}
		
		public void ShowAnswerPanel(bool b) {
			OpenPanel(answerPanel, b);
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
