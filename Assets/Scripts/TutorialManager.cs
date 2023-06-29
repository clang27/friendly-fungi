/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Button[] buttonsToDisableOnJournalPage;
		[SerializeField] private JournalTab[] tabsToDisableOnJournalPage;
		[SerializeField] private Ease easeType = Ease.InCubic;
		[SerializeField] private float easeDuration = 0.8f;
		[SerializeField] private float scaleMultiplier = 0.6f;
	
		[SerializeField] private GameObject panel;
		[SerializeField] private CanvasGroup[] pages;
	#endregion

	#region Attributes
		public static bool JournalTabsCanOperate = true;
	#endregion

	#region Components
		private TimeManager _timeManager;
		private UiManager _uiManager;
		private CardManager _cardManager;
	#endregion
	
	#region Private Data	
		private int _currentPage = -1;
		private Sequence _pageTurnSequence;
		private bool _turningPage;

		// Page one
		private RectTransform _leftArrow, _rightArrow;
		private GameObject _leftCheckmark, _rightCheckmark;
		
		// Page two
		private RectTransform _rightClick;
		private GameObject _rightClickCheckMark;
		
		// Page three
		private bool _changedTime;
		private RectTransform _leftClickTime;
		private Vector3 _leftClickTimeStartPoint;
		private GameObject _leftClickTimeCheckMark;
		
		// Page four
		private bool _exittedJournal;
		private RectTransform _leftClickJournal;
		private Vector3 _leftClickJournalStartPoint;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_timeManager = GetComponent<TimeManager>();
			_uiManager = GetComponent<UiManager>();
			_cardManager = GetComponent<CardManager>();
		}

		private void Start() {
			// Page one
			var left = pages[0].transform.GetChild(0);
			var right = pages[0].transform.GetChild(1);

			_leftArrow = left.GetChild(0).GetComponent<RectTransform>();
			_rightArrow = right.GetChild(0).GetComponent<RectTransform>();
			_leftCheckmark = left.GetChild(1).gameObject;
			_rightCheckmark = right.GetChild(1).gameObject;
			
			// Page two
			var rc = pages[1].transform.GetChild(0);

			_rightClick = rc.GetComponent<RectTransform>();
			_rightClickCheckMark = rc.GetChild(1).gameObject;
			
			// Page three
			var lc = pages[2].transform.GetChild(0);

			_leftClickTime = lc.GetComponent<RectTransform>();
			_leftClickTimeStartPoint = _leftClickTime.localPosition;
			_leftClickTimeCheckMark = lc.GetChild(0).gameObject;
			
			// Page four
			var lcj = pages[3].transform.GetChild(0);

			_leftClickJournal = lcj.GetComponent<RectTransform>();
			_leftClickJournalStartPoint = _leftClickJournal.localPosition;
			
			HideAllPages();
		}

		private void Update() {
			if (_currentPage == -1 || _turningPage) return;

			switch (_currentPage) {
				case 0:
					if (Input.GetKeyDown(KeyCode.A))
						_leftCheckmark.SetActive(true);
					if (Input.GetKeyDown(KeyCode.D))
						_rightCheckmark.SetActive(true);
					if (_leftCheckmark.activeSelf && _rightCheckmark.activeSelf)
						TurnPage(() => {
							_rightClick.DOScale(Vector3.one * scaleMultiplier, easeDuration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
						});
					break;
				case 1:
					if (Input.GetMouseButtonUp(1))
						_rightClickCheckMark.SetActive(true);
					if (_rightClickCheckMark.activeSelf) {
						TurnPage(() => {
							_uiManager.ShowTopBar(true);
							_uiManager.HideAllButTime(true);
							
							foreach (var b in FindObjectsOfType<MiscAnimal>())
								b.EnableRenderers(true);
							
							_timeManager.enabled = true;
							_timeManager.Play();
							
							_leftClickTime.DOScale(Vector3.one * scaleMultiplier, easeDuration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
							_leftClickTime.DOMoveX(_leftClickTime.transform.position.x + (Screen.width * 0.425f), 2f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Restart);
						});
					}
					break;
				case 2:
					if (_changedTime) {
						_leftClickTimeCheckMark.SetActive(true);
						TurnPage(() => {
							_uiManager.HideAllButTime(false);
							
							_leftClickJournal.DOScale(Vector3.one * 0.6f, 0.5f).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
							_leftClickJournal.DOMoveY(_leftClickJournal.transform.position.y + (Screen.height * 0.04f), 0.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
							_leftClickJournal.DOMoveX(_leftClickJournal.transform.position.x - (Screen.height * 0.01f), 0.5f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
						});
					}
					break;
				case 3:
					if (_exittedJournal) {
						EndTutorial();
						StartCoroutine(StartTutorialCardQuestion());
					}
					break;
			}
		}

	#endregion
	
	#region Other Methods
		private IEnumerator StartTutorialCardQuestion() {
			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);
			_cardManager.Init();
			_cardManager.StartQuestionCardIntro();
						
			_uiManager.ShowTopBar(false);
			_timeManager.enabled = false;
			FindObjectOfType<CameraController>().Enabled = false;
			
			while (!_cardManager.Ready) {
				yield return new WaitForSeconds(0.1f);
			}
			
			_uiManager.ShowTopBar(true);
			_timeManager.enabled = true;
			_timeManager.Play();
			FindObjectOfType<CameraController>().Enabled = true;
		}
		private void TurnPage(UnityAction callback) {
			_turningPage = true;
			
			Debug.Log($"Turning tutorial from {_currentPage} to {_currentPage+1}");
			_pageTurnSequence = DOTween.Sequence();
			
			_pageTurnSequence.Append(pages[_currentPage].DOFade(0f, 2f));
			_pageTurnSequence.AppendCallback(() => {
				pages[_currentPage].interactable = false;
				pages[_currentPage].blocksRaycasts = false;
				_currentPage++;
				callback(); 
				pages[_currentPage].interactable = true;
				pages[_currentPage].blocksRaycasts = true;
				_turningPage = false;
			});
			_pageTurnSequence.Append(pages[_currentPage+1].DOFade(1f, 2f));
			_pageTurnSequence.Play();
		}
		private void HideAllPages() {
			foreach (var p in pages) {
				p.alpha = 0f;
				p.interactable = false;
				p.blocksRaycasts = false;
			}
			
			_pageTurnSequence?.Kill();
			_turningPage = false;
			
			// Page one
			_leftArrow.DOKill();
			_rightArrow.DOKill();
			_leftArrow.localScale = Vector3.one;
			_rightArrow.localScale = Vector3.one;
			_leftCheckmark.SetActive(false);
			_rightCheckmark.SetActive(false);
			
			// Page two
			_rightClick.DOKill();
			_rightClick.localScale = Vector3.one;
			_rightClickCheckMark.SetActive(false);
			
			// Page three
			_leftClickTime.DOKill();
			_leftClickTimeCheckMark.SetActive(false);
			_leftClickTime.localScale = Vector3.one;
			_leftClickTime.localPosition = _leftClickTimeStartPoint;
			
			// Page four
			_leftClickJournal.DOKill();
			_leftClickJournal.localScale = Vector3.one;
			_leftClickJournal.GetComponent<Image>().DOFade(1f, 0f);
			_leftClickJournal.localPosition = _leftClickJournalStartPoint;
		}
		public void StartTutorial() {
			Debug.Log("Starting Tutorial");

			JournalTabsCanOperate = false;
			_currentPage = 0;
			panel.SetActive(true);
			OpenPage(_currentPage, true, 2f);

			// Page one
			_leftArrow.DOScale(Vector3.one * scaleMultiplier, easeDuration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
			_rightArrow.DOScale(Vector3.one * scaleMultiplier, easeDuration).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
			
			foreach (var b in buttonsToDisableOnJournalPage)
				b.interactable = false;

			foreach (var t in tabsToDisableOnJournalPage)
				t.Enable(false);
			
			var tmp = _leftClickJournal.GetComponentInChildren<TextMeshProUGUI>();
			tmp.text = "Journal";
		}
	
		private void OpenPage(int i, bool b, float time) {
			var p = pages[i];
			
			p.DOKill();
			p.DOFade(b ? 1f : 0f, time);
			p.interactable = b;
			p.blocksRaycasts = b;
		}

		public void EndTutorial() {
			_currentPage = -1;
			_changedTime = false;
			_exittedJournal = false;
			_timeManager.Pause();

			HideAllPages();
			JournalTabsCanOperate = true;
			panel.SetActive(false);
		}

		public void ChangedTime() {
			_changedTime = _currentPage == 2;
		}

		public void ClickedJournal() {
			if (_exittedJournal || _currentPage != 3) return;

			var tmp = _leftClickJournal.GetComponentInChildren<TextMeshProUGUI>();
			_leftClickJournal.DOKill();
			_leftClickJournal.DOScale(Vector3.one, 0.4f);
			tmp.text = "";
			
			_leftClickJournal.DOLocalMove(new Vector3(-124.6f, 8.8f, 0f), 0.5f)
				.SetEase(Ease.InOutSine)
				.OnComplete(() => {
					tmp.text = "Info";
					_leftClickJournal.DOScale(Vector3.one * 0.6f, 0.5f).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
					_leftClickJournal.DOMoveY(_leftClickJournal.transform.position.y + (Screen.height * 0.04f), 0.5f)
						.SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
					_leftClickJournal.DOMoveX(_leftClickJournal.transform.position.x - (Screen.height * 0.01f), 0.5f)
						.SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
				});
		}
		
		public void ClickedTutorialInfo() {
			if (_exittedJournal || _currentPage != 3) return;
			
			var tmp = _leftClickJournal.GetComponentInChildren<TextMeshProUGUI>();
			_leftClickJournal.DOKill();
			tmp.text = "";
			
			_leftClickJournal.GetComponent<Image>().DOFade(0f, 0.05f)
				.OnComplete(() => {
					foreach (var b in buttonsToDisableOnJournalPage)
						b.interactable = true;
					
					foreach (var t in tabsToDisableOnJournalPage)
						t.Enable(true);

					JournalTabsCanOperate = true;
				});
		}

		public void ExittedJournal() {
			_exittedJournal = _currentPage == 3;
		}
	#endregion
}
