/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	#region Serialized Fields
		[SerializeField] private UniversalRenderPipelineAsset[] urpQualities;
	#endregion
	
	#region Attributes
		public static GameManager Instance { get; private set; }
		public bool Loading { get; private set; }
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
		private VictoryParticles _victoryParticles;
		private Volume _volume;
	#endregion
	
	#region Private Data
		private int _correctGuesses, _incorrectGuesses;
	#endregion

	#region Unity Methods
		private void Awake() {
			Instance = this;

			Settings.ReadData();
			MushroomData.Init();
			LocationData.Init();

			_timeManager = GetComponent<TimeManager>();
			_audioManager = FindObjectOfType<AudioManager>();
			_uiManager = GetComponent<UiManager>();
			_cardManager = GetComponent<CardManager>();
			_mushroomManager = GetComponent<MushroomManager>();
			_volume = GetComponent<Volume>();
		}

		private void Start() {
			UpdateQuality(Settings.Quality);
			
			_audioManager.MainMenuTheme();
			_uiManager.ShowTopBar(false);
			_uiManager.ShowCardPanel(false);
			_uiManager.ShowAnswerPanel(false);
			_uiManager.ClosePrompt();
			_uiManager.CloseJournal();
			_uiManager.CloseSign();

			_timeManager.enabled = false;
			
			ShowLoading();
		}

	#endregion

	#region Other Methods
		private void ResetData() {
			Journal.ClearData();
			MushroomData.DeleteAllSaves();
			LocationData.DeleteAllSaves();

			MushroomData.Init();
			LocationData.Init();
			
			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ClosePrompt();
		}
		public void OpenResetDataPrompt() {
			_uiManager.ShowBackgroundBlur(true);

			_uiManager.OpenPrompt(
				"This refreshes & randomizes the Shroos' names, colors, and locations.\n\nDo you want to do this?",
				"Yes", "No",
				ResetData, () => {
					_uiManager.ShowBackgroundBlur(false);
					_uiManager.ClosePrompt();
				});
		}
		public void OpenQuitPrompt() {
			DisableEverythingForPrompt(true, false);
				
			_uiManager.OpenPrompt(
				"Are you sure you want to exit to menu?\n\nYou will lose this level's progress.",
				"Yes", "No",
				QuitGame, () => {
					DisableEverythingForPrompt(false);
					_uiManager.ClosePrompt();
				}
			);
		}

		private void QuitGame() {
			InMainMenu = true;

			_cameraController.AutoRotate = true;
			_cameraController.ResetWorldPositionToMenu();

			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ClosePrompt();
			_uiManager.OpenMainMenu();
			
			_audioManager.MainMenuTheme();
			_audioManager.StopAmbience();

			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);
			_timeManager.PlayParticles();
			
			_victoryParticles.Activate(false);
			
			_mushroomManager.Clear();
		}

		private void RestartLevel() {
			_uiManager.ClosePrompt();
			DisableEverythingForPrompt(false);
			
			_audioManager.LevelTheme(LevelSelection.CurrentLevel);
			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);

			_correctGuesses = 0;
			_incorrectGuesses = 0;

			_victoryParticles.Activate(false);
			_cameraController.ResetWorldPositionToPlay();
			
			_mushroomManager.Init();
			
			_cardManager.ResetCards();
			_cardManager.Init();
		}
		
		private void DisableEverythingForPrompt(bool b, bool autoRotate = true) {
			if (b) {
				_timeManager.Pause();
				_timeManager.PauseParticles();
			} else if (!TimeManager.PausedFlag) {
				_timeManager.Play();
				_timeManager.PlayParticles();
			}
			
			_cameraController.Enabled = !b;
			_cameraController.AutoRotate = b && autoRotate;
			
			_uiManager.ShowTopBar(!b);
			_uiManager.ShowCardPanel(!b);
			_uiManager.ShowBackgroundBlur(b);
		}

		public void OpenSettings() {
			if (!InMainMenu)
				DisableEverythingForPrompt(true);
			else
				_uiManager.ShowBackgroundBlur(true);

			_uiManager.OpenSettings();
		}
		public void CloseSettings() {
			if (!InMainMenu)
				DisableEverythingForPrompt(false);
			else
				_uiManager.ShowBackgroundBlur(false);

			_uiManager.CloseSettings();
		}
		
		public void OpenJournal() {
			if (InMainMenu) {
				_uiManager.CloseMainMenu();
			} else {
				DisableEverythingForPrompt(true, false);
			}
				
			_uiManager.OpenJournal();
		}
		public void OpenJournalToMushroomPage(Mushroom m) {
			DisableEverythingForPrompt(true, false);

			_uiManager.OpenJournalToMushroomPage(m);
		}
		
		public void OpenSign(Location l) {
			DisableEverythingForPrompt(true, false);

			_uiManager.OpenSign(l);
		}

		public void CloseSign() {
			DisableEverythingForPrompt(false);
			
			_uiManager.CloseSign();
		}
		
		public void OpenSign(List<Location> l) {
			DisableEverythingForPrompt(true, false);

			_uiManager.OpenSign(l);
		}
		
		public void CloseJournal() {
			if (InMainMenu) {
				_uiManager.OpenMainMenu();
			} else {
				DisableEverythingForPrompt(false);
			}
				
			_uiManager.CloseJournal();
		}

		public void ShowLoading() {
			Loading = true;
			
			_uiManager.ShowLoadingScreen(true);
			_uiManager.DisableButtonsOnLoading(true);
		}

		public IEnumerator LoadLevel(Level currentLevel, Level nextLevel) {
			if (currentLevel) {
				var asyncUnload = SceneManager.UnloadSceneAsync(currentLevel.SceneName);
				while (!asyncUnload.isDone) {
					yield return null;
				}
			}

			var asyncLoad = SceneManager.LoadSceneAsync(nextLevel.SceneName, LoadSceneMode.Additive);
			while (!asyncLoad.isDone) {
				yield return null;
			}

			_uiManager.ShowLoadingScreen(false);
			_cameraController = FindObjectOfType<CameraController>();
			_mapScaler = FindObjectOfType<MapScaler>();
			_victoryParticles = FindObjectOfType<VictoryParticles>();

			_timeManager.Init(_mapScaler.transform);
			_timeManager.SetLevelTime(nextLevel);
			
			_mapScaler.GenerateMap();
			_cameraController.AutoRotate = true;

			Loading = false;
			
			while (!_mapScaler.MapReady) {
				yield return new WaitForSeconds(0.1f);
			}
			
			if (LevelSelection.CurrentLevel.Unlocked()) {
				_uiManager.DisableButtonsOnLoading(false);
				_uiManager.ChangeStartButton("Start", StartLevel);
			} else {
				_uiManager.ChangeStartButton("Locked", () => { });
			}
		}
		public void StartLevel() {
			InMainMenu = false;
			_uiManager.CloseMainMenu();
			_cardManager.ResetCards();
			
			_audioManager.LevelTheme(LevelSelection.CurrentLevel);
			_audioManager.StartAmbience();
			
			_cameraController.AutoRotate = false;

			_correctGuesses = 0;
			_incorrectGuesses = 0;

			StartCoroutine(FinishLevelGenerationThenStartTime());
		}

		private IEnumerator FinishLevelGenerationThenStartTime() {
			_cameraController.ResetWorldPositionToPlay();
			
			_mushroomManager.Init();
			_uiManager.ShowCardPanel(true);
			_cardManager.Init();
			_cardManager.StartQuestionCardIntro();
			
			while (!_mapScaler.MapReady || !_cameraController.Ready || !_cardManager.Ready) {
				yield return new WaitForSeconds(0.1f);
			}
			
			_cameraController.Enabled = true;
			_uiManager.ShowTopBar(true);

			_timeManager.enabled = true;
			_timeManager.Play();
		}

		public void OpenAnswer() {
			_uiManager.ShowBackgroundBlur(true);
			_uiManager.ShowAnswerPanel(true);
			_timeManager.Pause();
			_timeManager.PauseParticles();
			_uiManager.ShowCardPanel(false);
			_cameraController.Enabled = false;
		}
		
		public void CloseAnswer() {
			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ShowAnswerPanel(false);
			if (!TimeManager.PausedFlag)
				_timeManager.Play();
			_uiManager.ShowCardPanel(true);
			_cameraController.Enabled = true;
		}

		public void ShowBinoculars() {
			_volume.profile.components[1].active = true;
			_uiManager.ShowBinoculars(true);
		}

		public void HideBinoculars() {
			_volume.profile.components[1].active = false;
			_uiManager.ShowBinoculars(false);
		}
		
		public void GuessAnswer(bool correct) {
			if (correct) {
				_correctGuesses++;
				if (_correctGuesses == LevelSelection.CurrentLevel.NumberOfCorrectGuesses) {
					_audioManager.VictoryTheme();
					_victoryParticles.Activate(true);
					_uiManager.ShowAnswerPanel(false);
					DisableEverythingForPrompt(true);
					_uiManager.OpenPrompt("Congrats!", 
						"Restart", "Menu",
						RestartLevel, QuitGame);
				} else {
					_audioManager.PlayCorrect(true);
					CloseAnswer();
				}
			} else {
				_incorrectGuesses++;
				if (_incorrectGuesses == LevelSelection.CurrentLevel.NumberOfQuestions - LevelSelection.CurrentLevel.NumberOfCorrectGuesses + 1) {
					_audioManager.DefeatTheme();
					_uiManager.ShowAnswerPanel(false);
					DisableEverythingForPrompt(true);
					_uiManager.OpenPrompt("You made one too many incorrect guesses!\n\nTry again?", 
						"Yes", "No",
						RestartLevel, QuitGame);
				} else {
					_audioManager.PlayCorrect(false);
					CloseAnswer();
				}
			}
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
		
		public void UpdateQuality(int i) {
			QualitySettings.renderPipeline = urpQualities[i];
			Settings.SetData(SettingsType.Quality, i);
			infiniteParticleLifeSM.UpdateQuality();
		}
		
	#endregion
}
