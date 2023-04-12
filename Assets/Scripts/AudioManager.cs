/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using UnityEngine;

public class AudioManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private AudioClip mainMenuTheme;
	#endregion
	
	#region Attributes
		// public float AttributeOne { get; set; }
	#endregion
	
	#region Components
		private AudioSource _musicSource, _sfxSource, _ambienceSource;
	#endregion
	
	#region Private Data
		//private AudioClip _currentSong;
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
			ChangeSong(mainMenuTheme);
		}

		private void ChangeSong(AudioClip ac) {
			if (_musicSource.isPlaying)
				_musicSource.Stop();
			
			_musicSource.clip = ac;
			_musicSource.Play();
		}
	#endregion
}
