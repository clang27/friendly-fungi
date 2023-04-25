/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using DG.Tweening;
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
		
		[Header("SFX")]
		[SerializeField] private AudioClip correctAnswer;
		[SerializeField] private AudioClip incorrectAnswer;
		
		[Header("Ambience")]
		[SerializeField] private AudioClip[] daytimeAmbience;
		[SerializeField] private AudioClip[] nighttimeAmbience;
		[SerializeField] private AudioClip[] riverAmbience;
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

	#endregion
	
	#region Other Methods
		public void MainMenuTheme() {
			ChangeClip(musicMainMenu, _musicSource);
		}

		public void LevelTheme(Level l) {
			ChangeClip(l.Song, _musicSource);
		}

		private void ChangeClip(AudioClip ac, AudioSource a) {
			float maxVolume;
			
			if (a.Equals(_musicSource))
				maxVolume = Settings.MusicVolume * Settings.MasterVolume;
			else if (a.Equals(_sfxSource))
				maxVolume = Settings.SfxVolume * Settings.MasterVolume;
			else
				maxVolume = Settings.AmbienceVolume * Settings.MasterVolume;

			if (a.isPlaying) {
				a.DOFade(0f, 1f).OnComplete(() => {
					a.Stop();
					a.clip = ac;
					a.Play();
					a.DOFade(maxVolume, 1f);
				});
			} else {
				a.clip = ac;
				a.volume = maxVolume;
				a.Play();
			}
			
		}
		public void StartAmbience() {
			SelectAmbienceBasedOnTime(TimeManager.Hour);
			_ambienceSource.Play();
		}
		
		public void StopAmbience() {
			_ambienceSource.Stop();
		}

		private void SelectAmbienceBasedOnTime(float hour) {
			ChangeClip(hour is >= 7 and <= 17 ? daytimeAmbience[0] : nighttimeAmbience[0], _ambienceSource);
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

		public void PlayCorrect(bool b) {
			_sfxSource.PlayOneShot(b ? correctAnswer : incorrectAnswer, 1f);
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
