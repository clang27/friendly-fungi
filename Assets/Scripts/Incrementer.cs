/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Incrementer : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private string[] values;
		[SerializeField] private UnityEvent<int> OnUpdate;
	#endregion
	
	#region Attributes
		public int ChoiceIndex { get; private set; }
		public string Value { get; private set; }
	#endregion
	
	#region Components
		private Button _leftButton, _rightButton;
		private TextMeshProUGUI _textMesh;
	#endregion

	#region Unity Methods
		private void Awake() {
			_leftButton = transform.GetChild(0).GetComponent<Button>();
			_rightButton = transform.GetChild(2).GetComponent<Button>();
			_textMesh = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		}

		private void Start() {
			_leftButton.onClick.AddListener(DecrementValue);
			_rightButton.onClick.AddListener(IncrementValue);
		}
	#endregion
	
	#region Other Methods
		private void SetValue(int i) {
			if (i >= values.Length || i < 0) return;
			
			ChoiceIndex = i;
			Value = values[ChoiceIndex];
			
			_textMesh.text = Value;
			OnUpdate.Invoke(ChoiceIndex);
			UpdateButtonsInteractable();
		}
		
		public void SetValueWithoutNotify(int i) {
			if (i >= values.Length || i < 0) return;
			
			ChoiceIndex = i;
			Value = values[ChoiceIndex];
			
			_textMesh.text = Value;
			UpdateButtonsInteractable();
		}

		private void IncrementValue() {
			SetValue(ChoiceIndex + 1);
		}
		
		private void DecrementValue() {
			SetValue(ChoiceIndex - 1);
		}

		private void UpdateButtonsInteractable() {
			//Debug.Log("ChoiceIndex is " + ChoiceIndex);
			_leftButton.interactable = ChoiceIndex > 0;
			_rightButton.interactable = ChoiceIndex < values.Length - 1;
		}

	#endregion
}
