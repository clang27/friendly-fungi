/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Level[] levels;
		[SerializeField] private Button leftButton, rightButton;
		[SerializeField] private CanvasGroup lockedPanel;
	#endregion
	
	#region Attributes
		public static Level CurrentLevel { get; private set; }
		public static Level NextLevel { get; private set; }
		public static Level[] AllLevels { get; private set; }
	#endregion
	
	#region Components
		private Animator _animator;
	#endregion
	
	#region Private Data
		private int _currentSelection = 0;
		private TextMeshProUGUI _levelNameText;
		private static readonly int Spin = Animator.StringToHash("Spin");
	#endregion
	
	#region Unity Methods
		private void Awake() {
			AllLevels = levels;
			CurrentLevel = AllLevels[0];
			NextLevel = AllLevels[1];
				
			_levelNameText = GetComponentsInChildren<TextMeshProUGUI>()[0];
			_animator = GetComponent<Animator>();
		}

		private void Start() {
			StartCoroutine(GameManager.Instance.LoadLevel(null, CurrentLevel));
			UpdateNameUI();
			UpdateButtonsUI();
		}
	#endregion
	
	#region Other Methods
		public void UpdateNameUI() {
			_levelNameText.text = CurrentLevel.LevelName;
			// _levelNameText.color = CurrentLevel.Unlocked() ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}
		public void UpdateButtonsUI() {
			lockedPanel.alpha = CurrentLevel.Unlocked ? 0f : 1f;
			rightButton.interactable = _currentSelection < AllLevels.Length - 1;
			leftButton.interactable = _currentSelection > 0;
		}

		private IEnumerator IncreaseSelection(int amount) {
			var currentLevel = _currentSelection;
			
			_animator.ResetTrigger(Spin);
			rightButton.interactable = false;
			leftButton.interactable = false;
			_animator.SetTrigger(Spin);
			
			_currentSelection+=amount;
			CurrentLevel = AllLevels[_currentSelection];
			NextLevel = (_currentSelection != AllLevels.Length - 1) ? AllLevels[_currentSelection+1] : null;
			
			UpdateNameUI();
			
			GameManager.Instance.ShowLoading();
			
			yield return new WaitForSeconds(0.9f);
			
			StartCoroutine(GameManager.Instance.LoadLevel(AllLevels[currentLevel], AllLevels[_currentSelection]));

			while (GameManager.Instance.Loading)
				yield return null;

			UpdateButtonsUI();
		}
		
		public IEnumerator GoToLevel(int levelNumber) {
			var currentLevel = _currentSelection;
			
			_animator.ResetTrigger(Spin);
			rightButton.interactable = false;
			leftButton.interactable = false;
			_animator.SetTrigger(Spin);
			
			_currentSelection=levelNumber;
			CurrentLevel = AllLevels[_currentSelection];
			NextLevel = (_currentSelection != AllLevels.Length - 1) ? AllLevels[_currentSelection+1] : null;
			
			UpdateNameUI();
			
			GameManager.Instance.ShowLoading();
			
			yield return new WaitForSeconds(0.9f);
			
			StartCoroutine(GameManager.Instance.LoadLevel(AllLevels[currentLevel], AllLevels[_currentSelection]));

			while (GameManager.Instance.Loading)
				yield return null;

			UpdateButtonsUI();
		}
		
		public void SelectNextLevel() {
			StartCoroutine(IncreaseSelection(1));
		}
		
		public void SelectPreviousLevel() {
			StartCoroutine(IncreaseSelection(-1));
		}
	#endregion
}
