/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		private CardManager _cardManager;
		private MushroomManager _mushroomManager;
	#endregion

	#region Private Data
		//private Level _currentLevel;
	#endregion

	#region Unity Methods

	private void Awake() {
		Settings.ReadData();
		MushroomData.Init();

		_timeManager = FindObjectOfType<TimeManager>();
		_audioManager = FindObjectOfType<AudioManager>();
		_uiManager = FindObjectOfType<UiManager>();
		_cardManager = FindObjectOfType<CardManager>();
		_mushroomManager = FindObjectOfType<MushroomManager>();
	}

	private void Start() {
		_audioManager.MainMenuTheme();
		_uiManager.ShowTopBar(false);
		_uiManager.ShowCardPanel(false);
		_uiManager.ShowAnswerPanel(false);
		_uiManager.ClosePrompt();
	}

	#endregion

	#region Other Methods
		public void OpenQuitPrompt() {
			_timeManager.Pause();
			_cameraController.Enabled = false;
			_cameraController.AutoRotate = true;
			
			_uiManager.ShowTopBar(false);
			_uiManager.ShowCardPanel(false);
				
			_uiManager.OpenPrompt(
				"Are you sure you want to exit to menu? You will lose this level's progress.",
				"Yes", "No",
				QuitGame, CloseQuitPrompt
			);
		}

		private void CloseQuitPrompt() {
			_timeManager.Play();
			_cameraController.Enabled = true;
			_cameraController.AutoRotate = false;
			
			_uiManager.ShowTopBar(true);
			_uiManager.ShowCardPanel(true);
			
			_uiManager.ClosePrompt();
		}

		private void QuitGame() {
			InMainMenu = true;
			
			_uiManager.ClosePrompt();
			_uiManager.OpenMainMenu();
			
			_audioManager.MainMenuTheme();

			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);
			_mushroomManager.UnpopulateWorld();
		}
		public void OpenSettings() {
			if (InMainMenu) {
				_uiManager.CloseMainMenu();
			} else {
				_timeManager.Pause();
				_uiManager.ShowTopBar(false);
				_uiManager.ShowCardPanel(false);
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
				_uiManager.ShowCardPanel(true);
				_cameraController.Enabled = true;
				_cameraController.AutoRotate = false;
			}
				
			_uiManager.CloseSettings();
		}

		public IEnumerator LoadLevel(Level lvl) {
			_uiManager.ShowLoadingScreen(true);
			_uiManager.DisableButtonsOnLoading(true);

			if (LevelSelection.LevelLoaded) {
				//Debug.Log("Unloading " + LevelSelection.CurrentLevel.SceneName);

				var asyncUnload = SceneManager.UnloadSceneAsync(LevelSelection.CurrentLevel.SceneName);
				while (!asyncUnload.isDone || _uiManager.LoadingFadingIn()) {
					yield return null;
				}
			}

			//Debug.Log("Loading " + LevelSelection.CurrentLevel.SceneName);
			LevelSelection.LevelLoaded = false;
			var asyncLoad = SceneManager.LoadSceneAsync(lvl.SceneName, LoadSceneMode.Additive);
			while (!asyncLoad.isDone) {
				yield return null;
			}

			LevelSelection.LevelLoaded = true;
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
			_cardManager.ResetQuestions();
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
			_uiManager.ShowCardPanel(true);
			
			_mushroomManager.PopulateWorld();
			
			_timeManager.Play();
			_cardManager.PickRandomQuestions();
		}

		public void OpenAnswer() {
			_uiManager.ShowAnswerPanel(true);
			_timeManager.Pause();
			_uiManager.ShowCardPanel(false);
			_cameraController.Enabled = false;
		}
		
		public void CloseAnswer() {
			_uiManager.ShowAnswerPanel(false);
			_timeManager.Play();
			_uiManager.ShowCardPanel(true);
			_cameraController.Enabled = true;
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
