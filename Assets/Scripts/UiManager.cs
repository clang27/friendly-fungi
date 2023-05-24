/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Button startButton;
		[SerializeField] private RectTransform logo;
		
		[SerializeField] private CanvasGroup menuPanel,
			loadingPanel,
			cardPanel,
			settingsPanel,
			audioPanel,
			gameplayPanel,
			graphicsPanel,
			topBar,
			answerPanel,
			journalPanel,
			signPanel,
			promptPanel,
			backgroundBlurPanel;

		[SerializeField] private Image binocularImage;
		[SerializeField] private List<CanvasGroup> thingsToHideWhenUsingBinoculars;
		#endregion

	#region Private Data
		private CanvasGroup _activePanel, _lastPanelOpen;
		private Journal _journal;
		private Sign _sign;
		private RectTransform _binocularRectTransform;
	#endregion

	#region Unity Methods

		private void Awake() {
			_lastPanelOpen = audioPanel;
			_journal = journalPanel.GetComponent<Journal>();
			_sign = signPanel.GetComponent<Sign>();
			_binocularRectTransform = binocularImage.GetComponent<RectTransform>();
		}

		private void Start() {
			logo.DOScale(1.08f, 1.8f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
			logo.DOLocalMoveY(logo.localPosition.y - 8f, 1.8f).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
		}

	#endregion

	#region Other Methods
		public void ShowLoadingScreen(bool b) {
			var rect = loadingPanel.GetComponent<RectTransform>();
			rect.DOKill();
			
			if (b) {
				OpenPanel(loadingPanel, true);
				rect.DOLocalMoveX(480f, 1f).SetEase(Ease.InOutCirc);
			} else {
				rect.DOLocalMoveX(1000f, 1f).SetEase(Ease.InOutCirc).OnComplete(() => OpenPanel(loadingPanel, false));
			}
		}

		public void ChangeStartButton(string s, UnityAction a) {
			startButton.onClick.RemoveAllListeners();
			startButton.onClick.AddListener(a);
		}

		public void DisableButtonsOnLoading(bool b) {
			startButton.interactable = !b;
		}

		public void OpenMainMenu() {
			OpenPanel(menuPanel, true);
			var rect = menuPanel.GetComponent<RectTransform>();
			rect.DOKill();
			rect.DOMoveX(0f, 1f).SetEase(Ease.OutBounce);
		}

		public void CloseMainMenu() {
			var rect = menuPanel.GetComponent<RectTransform>();
			rect.DOKill();
			rect.DOMoveX(-480f, 1f).SetEase(Ease.OutBounce).OnComplete(() => OpenPanel(menuPanel, false));
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
			_journal.GoToHomePage();
			OpenPanel(journalPanel, true);
		}
		
		public void OpenJournalToMushroomPage(Mushroom m) {
			_journal.GoToMushroomPage(m.Data.Name);
			OpenPanel(journalPanel, true);
		}
		
		public void OpenSign(Location l) {
			_sign.Show(l);
			OpenPanel(signPanel, true);

			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.localPosition.Set(0f, -390f, 0f);
			signRect.DOLocalMove(Vector3.up * -90f, 0.5f);
		}
		
		public void OpenSign(List<Location> l) {
			_sign.ShowReference(l);
			OpenPanel(signPanel, true);

			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.localPosition.Set(0f, -390f, 0f);
			signRect.DOLocalMove(Vector3.up * -90f, 0.5f);
		}

		public void CloseSign() {
			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.DOLocalMove(Vector3.up * -390f, 0.5f).OnComplete(() => OpenPanel(signPanel, false));
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

		public void ShowBinoculars(bool b) {
			_binocularRectTransform.DOKill();
			binocularImage.DOKill();

			foreach (var cg in thingsToHideWhenUsingBinoculars) {
				cg.DOKill();
				cg.DOFade(b ? 0f : 1f, 0.5f);
			}
			
			if (b) {
				binocularImage.DOFade(1f, 0.25f);
				_binocularRectTransform.DOScale(Vector3.one, 0.7f);
			} else {
				binocularImage.DOFade(0f, 0.5f);
				_binocularRectTransform.DOScale(Vector3.one * 2f, 0.4f);
			}
		}
		
		private void OpenPanel(CanvasGroup panel, bool b) {
			panel.alpha = b ? 1f : 0f;
			panel.interactable = b;
			panel.blocksRaycasts = b;
		}
		public void ShowBackgroundBlur(bool b) {
			backgroundBlurPanel.DOKill();
			backgroundBlurPanel.DOFade(b ? 1f : 0f, 0.5f);
			backgroundBlurPanel.blocksRaycasts = b;
		}

		private void CloseActivePanel() {
			if (!_activePanel) return;
				
			OpenPanel(_activePanel, false);
			_activePanel = null;
		}

		public void ShakePlaySign() {
			if (!startButton.interactable) return;

			var im = startButton.GetComponent<Image>();
			var ogPadding = im.raycastPadding;
			im.raycastPadding = Vector4.zero;
			startButton.GetComponent<RectTransform>().DOPunchRotation(Vector3.forward * 3f, 0.2f, 1, 10f)
				.OnComplete(() => im.raycastPadding = ogPadding);
		}

		public void HoverButtonShrink(RectTransform rt) {
			var im = rt.GetComponent<Image>();
			im.raycastPadding = Vector4.one * -8f;
			rt.DOScale(Vector3.one * 0.8f, 0.3f).SetEase(Ease.OutCubic).OnComplete(() => im.raycastPadding = Vector4.zero);
		}
		
		public void UnhoverButtonGrow(RectTransform rt) {
			var im = rt.GetComponent<Image>();
			im.raycastPadding = Vector4.one * 8f;
			rt.DOScale(Vector3.one, 0.3f).OnComplete(() => im.raycastPadding = Vector4.zero);
			
		}
	#endregion
}
