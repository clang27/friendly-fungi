/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class UiSetAwake : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private SettingsType settingsType;
	#endregion

	#region Components
		private Slider _slider;
		private Toggle _toggle;
	#endregion
	
	
	#region Unity Methods
		private void Awake() {
			_slider = GetComponent<Slider>();
			_toggle = GetComponent<Toggle>();
		}
		
		private void Start() {
			if (_slider) {
				_slider.value = settingsType switch {
					SettingsType.AmbienceVolume => Settings.AmbienceVolume,
					SettingsType.MasterVolume => Settings.MasterVolume,
					SettingsType.MusicVolume => Settings.MusicVolume,
					SettingsType.SfxVolume => Settings.SfxVolume,
					SettingsType.BoostMultiplier => Settings.BoostMultiplier,
					SettingsType.RotateSpeed => Settings.RotateSpeed,
					SettingsType.MouseRotateSensitivity => Settings.MouseRotateSensitivity,
					_ => throw new ArgumentOutOfRangeException()
				};
			} else if (_toggle) {
				_toggle.isOn = settingsType switch {
					SettingsType.InvertWorldRotation => Settings.InvertWorldRotation,
					SettingsType.InvertLookY => Settings.InvertLookY,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}
		
	#endregion
}
