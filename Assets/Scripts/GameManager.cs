/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	#region Attributes
		public static GameManager Instance { get; private set; }
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
		private MushroomClicker _mushroomClicker;
	#endregion
	
	#region Private Data
		private int _correctGuesses, _incorrectGuesses;
	#endregion

	#region Unity Methods
		private void Awake() {
			Instance = this;
			
			Settings.ReadData();
			MushroomData.Init();

			_timeManager = GetComponent<TimeManager>();
			_audioManager = FindObjectOfType<AudioManager>();
			_uiManager = GetComponent<UiManager>();
			_cardManager = GetComponent<CardManager>();
			_mushroomManager = GetComponent<MushroomManager>();
			_mushroomClicker = GetComponent<MushroomClicker>();
		}

		private void Start() {
			_audioManager.MainMenuTheme();
			_uiManager.ShowTopBar(false);
			_uiManager.ShowCardPanel(false);
			_uiManager.ShowAnswerPanel(false);
			_uiManager.ClosePrompt();
			_uiManager.CloseJournal();
		}

	#endregion

	#region Other Methods
		public void OpenQuitPrompt() {
			DisableEverythingForPrompt(true, false);
				
			_uiManager.OpenPrompt(
				"Are you sure you want to exit to menu? You will lose this level's progress.",
				"Yes", "No",
				QuitGame, CloseQuitPrompt
			);
		}

		private void CloseQuitPrompt() {
			DisableEverythingForPrompt(false);
			
			_uiManager.ClosePrompt();
		}

		private void QuitGame() {
			InMainMenu = true;

			_cameraController.AutoRotate = true;
			
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
			_cameraController.ResetWorldPosition();
			
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
		}
		
		public void OpenSettings() {
			if (InMainMenu) {
				_uiManager.CloseMainMenu();
			} else {
				DisableEverythingForPrompt(true);
			}
				
			_uiManager.OpenSettings();
		}
		public void CloseSettings() {
			if (InMainMenu) {
				_uiManager.OpenMainMenu();
			} else {
				DisableEverythingForPrompt(false);
			}
				
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
		public void CloseJournal() {
			if (InMainMenu) {
				_uiManager.OpenMainMenu();
			} else {
				DisableEverythingForPrompt(false);
			}
				
			_uiManager.CloseJournal();
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
			_victoryParticles = FindObjectOfType<VictoryParticles>();
			
			if (LevelSelection.CurrentLevel.Unlocked()) {
				_uiManager.DisableButtonsOnLoading(false);
				_uiManager.ChangeStartButton("Start", StartLevel);
			} else {
				_uiManager.ChangeStartButton("Locked", () => { });
			}
			
			_timeManager.Init(_mapScaler.transform);
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
			_cardManager.ResetCards();
			
			_audioManager.LevelTheme(LevelSelection.CurrentLevel);
			_audioManager.StartAmbience();
			
			_cameraController.Enabled = true;
			_cameraController.AutoRotate = false;

			_correctGuesses = 0;
			_incorrectGuesses = 0;

			StartCoroutine(FinishLevelGenerationThenStartTime());
		}

		private IEnumerator FinishLevelGenerationThenStartTime() {
			_cameraController.ResetWorldPosition();
			
			_mushroomManager.Init();
			_cardManager.Init();
			
			while (!_mapScaler.MapReady || !_cameraController.Ready) {
				yield return new WaitForSeconds(0.1f);
			}
			
			_uiManager.ShowTopBar(true);
			_uiManager.ShowCardPanel(true);

			_timeManager.Play();
		}

		public void OpenAnswer() {
			_uiManager.ShowAnswerPanel(true);
			_timeManager.Pause();
			_timeManager.PauseParticles();
			_uiManager.ShowCardPanel(false);
			_cameraController.Enabled = false;
		}
		
		public void CloseAnswer() {
			_uiManager.ShowAnswerPanel(false);
			if (!TimeManager.PausedFlag)
				_timeManager.Play();
			_uiManager.ShowCardPanel(true);
			_cameraController.Enabled = true;
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
		
	#endregion
}
