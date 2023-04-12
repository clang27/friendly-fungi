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
		// [SerializeField] private Button startButton;
	#endregion
	
	#region Attributes
		private bool Paused { get; set; }
	#endregion
	
	#region Components
		private TimeManager _timeManager;
		private MapScaler _mapScaler;
		private CameraController _cameraController;
		private AudioManager _audioManager;
		private UiManager _uiManager;
	#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			Settings.ReadSaveData();
			
			_timeManager = FindObjectOfType<TimeManager>();
			_cameraController = FindObjectOfType<CameraController>();
			_audioManager = FindObjectOfType<AudioManager>();
			_uiManager = FindObjectOfType<UiManager>();
		}

		private void Start() {
			_audioManager.MainMenuTheme();
			StartCoroutine(LoadScene("LevelOne"));
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (_mapScaler && _mapScaler.MapReady)
					EndLevel();
				else if (_mapScaler)
					StartLevel();
			} else if (Input.GetKeyDown(KeyCode.M) && _mapScaler && _mapScaler.MapReady) {
				Pause(!Paused);
			}
		}
	#endregion
	
	#region Other Methods
		private void Pause(bool b) {
			Paused = b;
			
			if (b) {
				_timeManager.Pause();
				_uiManager.OpenMenu();
			} else {
				_uiManager.CloseMenu();
			}

			_timeManager.ShowUI(!b);
			_cameraController.Enabled = !b;
			_cameraController.AutoRotate = b;
		}
		private IEnumerator LoadScene(string sceneName) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			_uiManager.DisableButtonsOnLoading(true);

			while (!asyncLoad.isDone) {
				yield return null;
			}
			
			_mapScaler = FindObjectOfType<MapScaler>();
			_uiManager.DisableButtonsOnLoading(false);
			_uiManager.ChangeStartButton("Start", StartLevel);

		}
		public void StartLevel() {
			_uiManager.CloseMenu();
			_uiManager.ChangeStartButton("Resume", () => Pause(false));
			_cameraController.Init();
			_cameraController.Enabled = true;
			StartCoroutine(GenerateLevel());
		}

		public void EndLevel() {
			_timeManager.Pause();
			_timeManager.ShowUI(false);
			_mapScaler.RemoveMap();
		}

		private IEnumerator GenerateLevel() {
			_mapScaler.GenerateMap();
			while (!_mapScaler.MapReady) {
				yield return new WaitForSeconds(0.1f);
			}
			_timeManager.ShowUI(true);
			_timeManager.Play();
		}
	#endregion
}
