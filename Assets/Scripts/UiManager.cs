/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Button startButton, audioButton, gameplayButton, graphicsButton;
		[SerializeField] private RectTransform logo, leftClickInstruction;
		
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
			backgroundBlurPanel,
			levelCompletePanel;

		[SerializeField] private Image binocularImage;
		[SerializeField] private List<CanvasGroup> thingsToHideWhenUsingBinoculars;
	#endregion

	#region Private Data
		private CanvasGroup _activePanel, _lastPanelOpen;
		private Journal _journal;
		private Sign _sign;
		private RectTransform _binocularRectTransform;
		private Vector3 _leftClickStartPosition;
	#endregion

	#region Unity Methods
		private void Awake() {
			_lastPanelOpen = audioPanel;
			_journal = journalPanel.GetComponent<Journal>();
			_sign = signPanel.GetComponent<Sign>();
			_binocularRectTransform = binocularImage.GetComponent<RectTransform>();
			_leftClickStartPosition = leftClickInstruction.localPosition;
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
				rect.DOLocalMoveX(480f, 0.8f).SetEase(Ease.OutCirc);
			} else {
				rect.DOLocalMoveX(1000f, 0.35f).SetEase(Ease.InCirc).OnComplete(() => OpenPanel(loadingPanel, false));
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
			rect.DOMoveX(-Screen.width/2f, 1f).SetEase(Ease.OutBounce).OnComplete(() => OpenPanel(menuPanel, false));
		}
		public void OpenSettings() {
			OpenPanel(settingsPanel, true);
			OpenPanel(_lastPanelOpen, true);
			SetActivePanel(_lastPanelOpen);
		}

		public void CloseSettings() {
			_lastPanelOpen = _activePanel;
			CloseActivePanel();
			OpenPanel(settingsPanel, false);
			Settings.SaveAllData();
		}
		
		public void OpenJournal() {
			_journal.GoToHomePage();
			GetJournalSequence(true, 0.5f, Ease.OutCubic).Play();
		}
		
		public void OpenJournalToMushroomPage(Mushroom m) {
			_journal.GoToMushroomPage(m.Data.Name);
			GetJournalSequence(true, 0.5f, Ease.OutCubic).Play();
		}
		
		public void CloseJournal() {
			GetJournalSequence(false, 0.5f, Ease.OutCubic).Play();
		}

		private Sequence GetJournalSequence(bool open, float dur, Ease ease) {
			var s = DOTween.Sequence();
			var rt = journalPanel.GetComponent<RectTransform>();
			
			s.Append(rt.DOScale(Vector3.one * (open ? 1f : 1.5f), dur).SetEase(ease));
			s.Join(rt.DOLocalMoveY((open ? -10f : 732f), dur).SetEase(ease));
			if (!open) {
				s.AppendCallback(() => {
					OpenPanelQuick(journalPanel, false);
				});
			} else {
				OpenPanelQuick(journalPanel, true);
			}
			
			return s;
		}

		public void OpenSign(Location l) {
			_sign.Show(l);
			OpenPanel(signPanel, true);

			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.localPosition.Set(0f, -390f, 0f);
			signRect.DOLocalMoveY(-100f, 0.5f);
		}
		
		public void OpenSign(List<Location> l) {
			_sign.ShowReference(l);
			OpenPanel(signPanel, true);

			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.localPosition.Set(0f, -390f, 0f);
			signRect.DOLocalMoveY(-100f, 0.5f);
		}

		public void CloseSign() {
			var signRect = signPanel.GetComponent<RectTransform>();

			signRect.DOKill();
			signRect.DOLocalMoveY(-390f, 0.5f).OnComplete(() => OpenPanel(signPanel, false));
		}

		public void OpenAudio() {
			CloseActivePanel();
			OpenPanelQuick(audioPanel, true);
			SetActivePanel(audioPanel);
		}
		public void OpenGameplay() {
			CloseActivePanel();
			OpenPanelQuick(gameplayPanel, true);
			SetActivePanel(gameplayPanel);
		}
		public void OpenGraphics() {
			CloseActivePanel();
			OpenPanelQuick(graphicsPanel, true);
			SetActivePanel(graphicsPanel);
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
		public void HideAllButTime(bool b) {
			foreach (var cg in thingsToHideWhenUsingBinoculars) {
				OpenPanelQuick(cg, !b);
			}
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
				cg.DOFade(b || !cg.interactable ? 0f : 1f, 0.5f);
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
			panel.DOKill();
			panel.DOFade(b ? 1f : 0f, 0.2f);
			panel.interactable = b;
			panel.blocksRaycasts = b;
		}
		
		private void OpenPanelQuick(CanvasGroup panel, bool b) {
			panel.DOKill();
			panel.alpha = b ? 1f : 0f;
			panel.interactable = b;
			panel.blocksRaycasts = b;
		}
		
		public void ShowBackgroundBlur(bool b, float amount = 1f) {
			backgroundBlurPanel.DOKill();
			backgroundBlurPanel.DOFade(b ? amount : 0f, b ? 0.2f : 0.5f);
			backgroundBlurPanel.blocksRaycasts = b;
		}

		private void CloseActivePanel() {
			if (!_activePanel) return;
				
			OpenPanelQuick(_activePanel, false);
			_activePanel = null;
		}

		public void ShakePlaySign() {
			if (!startButton.interactable) return;

			var im = startButton.GetComponent<Image>();
			var ogPadding = im.raycastPadding;
			im.raycastPadding = Vector4.zero;
			
			var signRect = startButton.GetComponent<RectTransform>();
			if (!DOTween.IsTweening(signRect)) {
				signRect.DOPunchRotation(Vector3.forward * 8f, 0.25f, 1, 12f)
					.OnComplete(() => im.raycastPadding = ogPadding);
			}
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
		
		private void SetActivePanel(CanvasGroup cg) {
			_activePanel = cg;
			
			audioButton.interactable = !cg.Equals(audioPanel);
			gameplayButton.interactable = !cg.Equals(gameplayPanel);
			graphicsButton.interactable = !cg.Equals(graphicsPanel);
		}

		public void ShowLevelComplete(bool b) {
			OpenPanel(levelCompletePanel, b);
			
			var tmp = levelCompletePanel.GetComponentInChildren<TextMeshProUGUI>();
			tmp.text = "";
			
			if (b) {
				StartCoroutine(LevelCompleteBanner(tmp, 0.1f));				
			}
		}

		private IEnumerator LevelCompleteBanner(TextMeshProUGUI tmp, float time) {
			foreach (var c in "Level Complete") {
				tmp.text += c;
				yield return new WaitForSeconds(time);
			}
		}
		public void ShowLeftClickInstruction(bool b, string message = "") {
			var img = leftClickInstruction.GetComponent<CanvasGroup>();
			var textmesh = img.GetComponentInChildren<TextMeshProUGUI>();
			
			if (img.alpha < 0.5f && !b)
				return;
			
			leftClickInstruction.DOKill();
			img.DOKill();
			
			if (b) {
				textmesh.text = message;
				leftClickInstruction.localScale = Vector3.one;
				leftClickInstruction.localPosition = _leftClickStartPosition;
				img.DOFade(1f, 1f);
				leftClickInstruction.DOScale(Vector3.one * 0.6f, 0.7f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
				leftClickInstruction.DOLocalMoveY(_leftClickStartPosition.y + 22f, 0.7f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
				leftClickInstruction.DOLocalMoveX(_leftClickStartPosition.x - 6f, 0.7f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
			} else {
				img.DOFade(0f, 0.2f);
			}
		}
	#endregion
}
