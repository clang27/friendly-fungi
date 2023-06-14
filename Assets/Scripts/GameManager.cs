/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		
		public bool InBinoculars { get; private set; }
	#endregion

	#region Components
		private TimeManager _timeManager;
		private TutorialManager _tutorialManager;
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
			_tutorialManager = GetComponent<TutorialManager>();
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
			_timeManager.enabled = false;
			DisableEverythingForPrompt(true, false);
				
			_uiManager.OpenPrompt(
				"Are you sure you want to exit to menu?\n\nYou will lose this level's progress.",
				"Yes", "No",
				QuitGame, () => {
					DisableEverythingForPrompt(false);
					_uiManager.ClosePrompt();
					_timeManager.enabled = true;
				}
			);
		}

		public void QuitGame() {
			InMainMenu = true;

			_tutorialManager.EndTutorial();
			
			_cameraController.AutoRotate = true;
			_cameraController.ResetWorldPositionToMenu();

			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ShowLevelComplete(false);
			_uiManager.ClosePrompt();
			_uiManager.OpenMainMenu();
			
			_audioManager.MainMenuTheme();
			_audioManager.StopAmbience();
			
			_timeManager.SlideTimeToLevel(LevelSelection.CurrentLevel);
			_timeManager.PlayParticles();
			
			_victoryParticles.Activate(false);
			
			_mushroomManager.Clear();
			
			foreach (var b in FindObjectsOfType<MiscAnimal>())
				b.EnableRenderers(false);
		}

		private void RestartLevel() {
			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ClosePrompt();
			_timeManager.SetLevelTime(LevelSelection.CurrentLevel);
			
			StartLevel();
		}
		
		private void DisableEverythingForPrompt(bool b, bool autoRotate = true, float blurBgAmount = 1f) {
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
			_uiManager.ShowBackgroundBlur(b, blurBgAmount);
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
			
			foreach (var m in FindObjectsOfType<Mushroom>())
				m.EnableRenderers(false);

			foreach (var b in FindObjectsOfType<MiscAnimal>())
				b.EnableRenderers(false);
			
			Loading = false;
			
			while (!_mapScaler.MapReady) {
				yield return new WaitForSeconds(0.1f);
			}
			
			if (LevelSelection.CurrentLevel.Unlocked) {
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
			
			if (!LevelSelection.CurrentLevel.Tutorial)
				_cardManager.StartQuestionCardIntro();
			
			while (!_mapScaler.MapReady || !_cameraController.Ready || (!_cardManager.Ready && !LevelSelection.CurrentLevel.Tutorial)) {
				yield return new WaitForSeconds(0.1f);
			}
			
			_cameraController.Enabled = true;
			
			if (!LevelSelection.CurrentLevel.Tutorial) {
				_uiManager.ShowTopBar(true);
				
				foreach (var b in FindObjectsOfType<MiscAnimal>())
					b.EnableRenderers(true);
				
				_timeManager.enabled = true;
				_timeManager.Play();
			} else {
				_tutorialManager.StartTutorial();
			}
		}

		public void OpenAnswer() {
			_uiManager.ShowBackgroundBlur(true);
			_uiManager.ShowAnswerPanel(true);
			_timeManager.Pause();
			_timeManager.PauseParticles();
			_uiManager.ShowCardPanel(false);
			_uiManager.ShowTopBar(false);
			_cameraController.Enabled = false;
			_timeManager.enabled = false;
		}
		
		public void CloseAnswer() {
			_uiManager.ShowBackgroundBlur(false);
			_uiManager.ShowAnswerPanel(false);
			if (!TimeManager.PausedFlag)
				_timeManager.Play();
			_uiManager.ShowCardPanel(true);
			_uiManager.ShowTopBar(true);
			_cameraController.Enabled = true;
			_timeManager.enabled = true;
		}

		public void ShowBinoculars() {
			InBinoculars = true;
			_volume.profile.components[1].active = true;
			_uiManager.ShowBinoculars(true);
		}

		public void HideBinoculars() {
			InBinoculars = false;
			_volume.profile.components[1].active = false;
			_uiManager.ShowBinoculars(false);
		}
		
		public void GuessAnswer(bool correct) {
			if (correct) {
				_correctGuesses++;
				if (_correctGuesses == LevelSelection.CurrentLevel.NumberOfCorrectGuesses) {
					_audioManager.VictoryTheme();
					foreach (var b in FindObjectsOfType<MiscAnimal>())
						b.EnableRenderers(false);
					_timeManager.SlideTimeToNight();
					_victoryParticles.Activate(true);
					_cameraController.ResetWorldPositionToVictory();
					_uiManager.ShowAnswerPanel(false);
					DisableEverythingForPrompt(true, true, 0.4f);
					_uiManager.ShowLevelComplete(true);
					if (LevelSelection.NextLevel)
						LevelSelection.NextLevel.SaveLevelComplete();
				} else {
					_audioManager.PlayCorrect(true);
					CloseAnswer();
				}
			} else {
				_incorrectGuesses++;
				if (_incorrectGuesses == QuestionQueue.AllQuestions.Count - LevelSelection.CurrentLevel.NumberOfCorrectGuesses + 1) {
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
