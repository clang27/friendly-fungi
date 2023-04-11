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
		[SerializeField] private Button startButton;
	#endregion
	
	#region Attributes
		// public float AttributeOne { get; set; }
	#endregion
	
	#region Components
		private TimeManager _timeManager;
		private MapScaler _mapScaler;
		private CameraController _cameraController;
		#endregion
	
	#region Private Data
		// private float _dataOne, _dataTwo;
	#endregion
	
	#region Unity Methods
		private void Awake() {
			_timeManager = FindObjectOfType<TimeManager>();
			_cameraController = FindObjectOfType<CameraController>();
		}

		private void Start() {
			StartCoroutine(LoadScene("LevelOne"));
		}

		private void Update() {
			if (Input.GetKeyDown(KeyCode.Space)) {
				if (_mapScaler && _mapScaler.MapReady)
					EndLevel();
				else if (_mapScaler)
					StartLevel();
			}
		}
	#endregion
	
	#region Other Methods
		private IEnumerator LoadScene(string sceneName) {
			var asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Loading...";

			while (!asyncLoad.isDone) {
				yield return null;
			}
			
			_mapScaler = FindObjectOfType<MapScaler>();
			startButton.interactable = true;
			startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
			
		}
		public void StartLevel() {
			_cameraController.Enable(true);
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
