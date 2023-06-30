/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum UiSound {
	Hover, ButtonClick, TabClick, Scroll, Checkbox, Mushroom
}

[Serializable]
public enum ShrooSound {
	Cheer, Fright, Disapproval, Greet
}

public class AudioManager : MonoBehaviour {
	#region Serialized Fields
		[Header("Music")]
		[SerializeField] private AudioClip musicMainMenu;
		[SerializeField] private AudioClip victoryMusic, defeatMusic;
		
		[Header("UI")]
		[SerializeField] private AudioClip uiButtonClick;
		[SerializeField] private AudioClip uiTabClick, uiHover, uiScoll, uiCheckbox, uiMushroom;
		[SerializeField] private AudioClip playGameSound, playTimeSound;
		
		[Header("SFX")]
		[SerializeField] private AudioClip correctAnswer;
		[SerializeField] private AudioClip incorrectAnswer;
		[SerializeField] private AudioClip[] grassFootsteps, otherFootsteps;

		[Header("Ambience")]
		[SerializeField] private AudioClip daytimeAmbience;
		[SerializeField] private AudioClip nighttimeAmbience;
		[SerializeField] private AudioClip[] frogRibbits;
	#endregion
	
	#region Attributes
		public static AudioManager Instance { get; private set; }
		private bool AmbienceEnabled { get; set; }
		private static bool SoundCooldown { get; set; }
		private const float CooldownThreshold = 0.2f;
		
		private bool PlayingFrogSound { get; set; }
	#endregion
	
	#region Components
		private AudioSource _musicSource, _sfxSource, _ambienceSource;
	#endregion

	#region Unity Methods
		private void Awake() {
			Instance = this;
			
			_musicSource = GetComponentsInChildren<AudioSource>()[0];
			_sfxSource = GetComponentsInChildren<AudioSource>()[1];
			_ambienceSource = GetComponentsInChildren<AudioSource>()[2];
		}

		private void FixedUpdate() {
			if (!AmbienceEnabled) return;
			
			if (TimeManager.Hour < 17f)
				DaytimeAmbience();
			else
				NighttimeAmbience();

			if (LevelSelection.CurrentLevel.HasFrogs && !PlayingFrogSound) {
				PlayingFrogSound = true;
				DOVirtual.DelayedCall(Random.Range(6f, 12f), () => {
					PlayingFrogSound = false;
					PlayAmbientSound(frogRibbits[Random.Range(0, frogRibbits.Length)], 0.5f);
				});
			}
				
		}

	#endregion
	
	#region Other Methods
		public void MainMenuTheme() {
			ChangeClip(musicMainMenu, _musicSource);
		}

		public void PlayGameSfx() {
			PlaySfx(playGameSound, 1f);
		}
		
		public void ResumeTimeSfx() {
			PlaySfx(playTimeSound, 1f);
		}
		
		public void VictoryTheme() {
			ChangeClip(victoryMusic, _musicSource, 0.1f, false);
		}
		
		public void DefeatTheme() {
			ChangeClip(defeatMusic, _musicSource, 0.1f, false);
		}

		public void LevelTheme(Level l) {
			ChangeClip(l.Song, _musicSource);
		}

		private void ChangeClip(AudioClip ac, AudioSource a, float duration = 0.5f, bool loop = true) {
			float maxVolume;
			
			if (a.Equals(_musicSource))
				maxVolume = Settings.MusicVolume * Settings.MasterVolume;
			else if (a.Equals(_sfxSource))
				maxVolume = Settings.SfxVolume * Settings.MasterVolume;
			else
				maxVolume = Settings.AmbienceVolume * Settings.MasterVolume;
			
			if (a.isPlaying) {
				a.DOFade(0f, duration).OnComplete(() => {
					a.Stop();
					a.clip = ac;
					a.loop = loop;
					a.Play();
					a.DOFade(maxVolume, duration);
				});
			} else {
				a.clip = ac;
				a.loop = loop;
				a.volume = maxVolume;
				a.Play();
			}
			
		}
		public void StartAmbience() {
			_ambienceSource.Play();
			AmbienceEnabled = true;
		}
		
		public void StopAmbience() {
			_ambienceSource.Stop();
			AmbienceEnabled = false;
		}
		private void DaytimeAmbience() {
			if (_ambienceSource.clip == daytimeAmbience)
				return;
			
			ChangeClip(daytimeAmbience, _ambienceSource);
		}
		
		private void NighttimeAmbience() {
			if (_ambienceSource.clip == nighttimeAmbience)
				return;
			
			ChangeClip(nighttimeAmbience, _ambienceSource);
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
			PlaySfx(b ? correctAnswer : incorrectAnswer);
		}

		public void PlaySfx(AudioClip ac, float volume = 1f, float pitch = 1f) {
			_sfxSource.pitch = pitch;
			_sfxSource.PlayOneShot(ac, volume);
		}
		
		public void PlayAmbientSound(AudioClip ac, float volume = 1f, float pitch = 1f) {
			_ambienceSource.pitch = pitch;
			_ambienceSource.PlayOneShot(ac, volume);
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
				case UiSound.Mushroom:
					_sfxSource.PlayOneShot(uiMushroom, 1f); break;
				default:
					throw new ArgumentOutOfRangeException(nameof(s), s, null);
			}
		}
		
		public void PlayUiSound(int s) {
			PlayUiSound((UiSound) s);
		}

		public void PlayRandomFootstep(bool grass) {
			var clip = grass
				? grassFootsteps[Random.Range(0, grassFootsteps.Length)]
				: otherFootsteps[Random.Range(0, otherFootsteps.Length)];
			var volumeModifer = GameManager.Instance.InBinoculars ? 1f : 0.3f;
			
			PlaySfx(clip, (grass ? 0.15f : 0.25f) * volumeModifer);
		}

		public void PlayIntervalScrollSound() {
			if (SoundCooldown) return;
			
			StartCoroutine(Cooldown());
			PlayUiSound(UiSound.TabClick);
		}
		
		private static IEnumerator Cooldown() {
			SoundCooldown = true;
			yield return new WaitForSeconds(CooldownThreshold);
			SoundCooldown = false;
		}
		
		
	#endregion
}
