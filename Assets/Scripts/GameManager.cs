/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private Level startingLevel;
	#endregion
	
	#region Attributes
		private bool InMainMenu { get; set; } = true;
	#endregion
	
	#region Components
		private TimeManager _timeManager;
		private MapScaler _mapScaler;
		private CameraController _cameraController;
		private AudioManager _audioManager;
		private UiManager _uiManager;
	#endregion
	
	#region Private Data
		//private Level _currentLevel;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			Settings.ReadData();
			
			_timeManager = FindObjectOfType<TimeManager>();
			_audioManager = FindObjectOfType<AudioManager>();
			_uiManager = FindObjectOfType<UiManager>();
		}

		private void Start() {
			_audioManager.MainMenuTheme();
			_uiManager.ShowTopBar(false);
		}
	#endregion
	
	#region Other Methods
		public void QuitGame() {
			InMainMenu = true;
			
			_uiManager.OpenMainMenu();
			_uiManager.ShowTopBar(false);
			
			_audioManager.MainMenuTheme();
			_cameraController.Enabled = false;
			_cameraController.AutoRotate = true;
			
			_timeManager.Pause();
			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);
		}
		public void OpenSettings() {
			if (InMainMenu) {
				_uiManager.CloseMainMenu();
			} else {
				_timeManager.Pause();
				_uiManager.ShowTopBar(false);
				_cameraController.Enabled = false;
				_cameraController.AutoRotate = true;
			}
				
			_uiManager.OpenSettings();
		}
		public void CloseSettings() {
			if (InMainMenu) {
				_uiManager.OpenMainMenu();
			} else {
				_timeManager.Play();
				_uiManager.ShowTopBar(true);
				_cameraController.Enabled = true;
				_cameraController.AutoRotate = false;
			}
				
			_uiManager.CloseSettings();
		}

		public IEnumerator SwapLevels(Level lvl) {
			var asyncUnload = SceneManager.UnloadSceneAsync(LevelSelection.CurrentLevel.SceneName);
			_uiManager.ShowLoadingScreen(true);
			_uiManager.DisableButtonsOnLoading(true);
			
			while (!asyncUnload.isDone) {
				yield return null;
			}

			StartCoroutine(LoadLevel(lvl));
		}
		
		public IEnumerator LoadLevel(Level lvl) {
			var asyncLoad = SceneManager.LoadSceneAsync(lvl.SceneName, LoadSceneMode.Additive);
			_uiManager.ShowLoadingScreen(true);
			_uiManager.DisableButtonsOnLoading(true);

			while (!asyncLoad.isDone) {
				yield return null;
			}
			
			_uiManager.ShowLoadingScreen(false);
			_cameraController = FindObjectOfType<CameraController>();
			_mapScaler = FindObjectOfType<MapScaler>();
			
			if (LevelSelection.CurrentLevel.Unlocked()) {
				_uiManager.DisableButtonsOnLoading(false);
				_uiManager.ChangeStartButton("Start", StartLevel);
			} else {
				_uiManager.ChangeStartButton("Locked", () => { });
			}
			
			_timeManager.SetLevelTime(lvl);
			_mapScaler.GenerateMap();
			_cameraController.AutoRotate = true;
			
			while (!_mapScaler.MapReady) {
				yield return new WaitForSeconds(0.1f);
			}
		}
		public void StartLevel() {
			InMainMenu = false;
			_uiManager.CloseMainMenu();
			_audioManager.LevelTheme(LevelSelection.CurrentLevel);
			_cameraController.Enabled = true;
			
			StartCoroutine(FinishLevelGenerationThenStartTime());
		}

		private IEnumerator FinishLevelGenerationThenStartTime() {
			_cameraController.AutoRotate = false;
			_cameraController.ResetWorldPosition();
			
			while (!_mapScaler.MapReady) {
				yield return new WaitForSeconds(0.1f);
			}
			
			_uiManager.ShowTopBar(true);
			_timeManager.Play();
		}

		public void UpdateMouseRotateSensitivity(float f) {
			_audioManager.PlayIntervalScrollSound();
			Settings.SetData(SettingsType.MouseRotateSensitivity, f);
		}
		
		public void UpdateRotateSpeed(float f) {
			_audioManager.PlayIntervalScrollSound();
			Settings.SetData(SettingsType.RotateSpeed, f);
		}
		
		public void UpdateBoostMultiplier(float f) {
			_audioManager.PlayIntervalScrollSound();
			Settings.SetData(SettingsType.BoostMultiplier, f);
		}
		
		public void UpdateInvertWorldRotation(bool b) {
			Settings.SetData(SettingsType.InvertWorldRotation, b ? 1 : 0);
		}
		
		public void UpdateInvertLookY(bool b) {
			Settings.SetData(SettingsType.InvertLookY, b ? 1 : 0);
		}
		
	#endregion
}
