/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using UnityEngine;

[Serializable]
public enum UiSound {
	Hover, ButtonClick, TabClick, Scroll, Checkbox
}

public class AudioManager : MonoBehaviour {
	#region Serialized Fields
		[Header("Music")]
		[SerializeField] private AudioClip musicMainMenu;
		
		[Header("UI")]
		[SerializeField] private AudioClip uiButtonClick;
		[SerializeField] private AudioClip uiTabClick, uiHover, uiScoll, uiCheckbox;
	#endregion
	
	#region Attributes
		private static bool SoundCooldown { get; set; }
		private const float CooldownThreshold = 0.2f;
	#endregion
	
	#region Components
		private AudioSource _musicSource, _sfxSource, _ambienceSource;
	#endregion

	#region Unity Methods
		private void Awake() {
			_musicSource = GetComponentsInChildren<AudioSource>()[0];
			_sfxSource = GetComponentsInChildren<AudioSource>()[1];
			_ambienceSource = GetComponentsInChildren<AudioSource>()[2];
		}
		
		private void Start() {
			_musicSource.volume = Settings.MusicVolume * Settings.MasterVolume;
			_sfxSource.volume = Settings.SfxVolume * Settings.MasterVolume;
			_ambienceSource.volume = Settings.AmbienceVolume * Settings.MasterVolume;
		}

		#endregion
	
	#region Other Methods
		public void MainMenuTheme() {
			ChangeSong(musicMainMenu);
		}

		public void LevelTheme(Level l) {
			ChangeSong(l.Song);
		}

		private void ChangeSong(AudioClip ac) {
			if (_musicSource.isPlaying)
				_musicSource.Stop();
			
			_musicSource.clip = ac;
			_musicSource.Play();
		}

		public void UpdateMasterVolume(float f) {
			PlayIntervalScrollSound();			
			Settings.SetData(SettingsType.MasterVolume, f);
			_musicSource.volume = Settings.MusicVolume * f;
			_sfxSource.volume = Settings.SfxVolume * f;
			_ambienceSource.volume = Settings.AmbienceVolume * f;
		}
		
		public void UpdateMusicVolume(float f) {
			PlayIntervalScrollSound();			
			Settings.SetData(SettingsType.MusicVolume, f);
			_musicSource.volume = f * Settings.MasterVolume;
		}
		
		public void UpdateSfxVolume(float f) {
			PlayIntervalScrollSound();			
			Settings.SetData(SettingsType.SfxVolume, f);
			_sfxSource.volume = f * Settings.MasterVolume;
		}
		
		public void UpdateAmbienceVolume(float f) {
			PlayIntervalScrollSound();			
			Settings.SetData(SettingsType.AmbienceVolume, f);
			_ambienceSource.volume = f * Settings.MasterVolume;
		}
		
		public void PlayUiSound(UiSound s) {
			switch (s) {
				case UiSound.Checkbox:
					_sfxSource.PlayOneShot(uiCheckbox, 1f); break;
				case UiSound.ButtonClick:
					_sfxSource.PlayOneShot(uiButtonClick, 1f); break;
				case UiSound.TabClick:
					_sfxSource.PlayOneShot(uiTabClick, 1f); break;
				case UiSound.Scroll:
					_sfxSource.PlayOneShot(uiScoll, 1f); break;
				case UiSound.Hover:
					_sfxSource.PlayOneShot(uiHover, 1f); break;
				default:
					throw new ArgumentOutOfRangeException(nameof(s), s, null);
			}
		}
		
		public void PlayUiSound(int s) {
			PlayUiSound((UiSound) s);
		}

		public void PlayIntervalScrollSound() {
			if (SoundCooldown) return;
			
			StartCoroutine(Cooldown());
			PlayUiSound(UiSound.Scroll);
		}
		
		private static IEnumerator Cooldown() {
			SoundCooldown = true;
			yield return new WaitForSeconds(CooldownThreshold);
			SoundCooldown = false;
		}
		
		
	#endregion
}
