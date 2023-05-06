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
	#endregion
	
	#region Attributes
		public static Level CurrentLevel { get; private set; }
	#endregion
	
	#region Components
		private Animator _animator;
	#endregion
	
	#region Private Data
		private int _currentSelection = 0;
		private Button _leftButton, _rightButton;
		private TextMeshProUGUI _levelNameText;
		private static readonly int Spin = Animator.StringToHash("Spin");
	#endregion
	
	#region Unity Methods
		private void Awake() {
			CurrentLevel = levels[0];
			_levelNameText = GetComponentsInChildren<TextMeshProUGUI>()[0];
			_leftButton = transform.GetChild(0).GetComponent<Button>();
			_rightButton = transform.GetChild(2).GetComponent<Button>();
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
			_levelNameText.color = CurrentLevel.Unlocked() ? Color.white : Color.red;
		}
		public void UpdateButtonsUI() {
			_rightButton.gameObject.SetActive(_currentSelection < levels.Length-1);
			_leftButton.gameObject.SetActive(_currentSelection > 0);
		}

		private IEnumerator IncreaseSelection(int amount) {
			_animator.ResetTrigger(Spin);
			_leftButton.gameObject.SetActive(false);
			_rightButton.gameObject.SetActive(false);
			_animator.SetTrigger(Spin);
			
			_currentSelection+=amount;
			CurrentLevel = levels[_currentSelection];
			UpdateNameUI();
			
			GameManager.Instance.ShowLoading();
			
			yield return new WaitForSeconds(0.8f);
			
			StartCoroutine(GameManager.Instance.LoadLevel(levels[_currentSelection-amount], levels[_currentSelection]));

			while (GameManager.Instance.Loading)
				yield return null;

			UpdateButtonsUI();
		}
		public void NextLevel() {
			StartCoroutine(IncreaseSelection(1));
		}
		
		public void PreviousLevel() {
			StartCoroutine(IncreaseSelection(-1));
		}
	#endregion
}
