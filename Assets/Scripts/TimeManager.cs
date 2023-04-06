/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
    #region Components
        [SerializeField] private float multiplier = 0.1f;
    #endregion
    
    #region Attributes
        public static bool Running { get; private set; }
        public static float Hour { get; private set; }

    #endregion

    #region Private Data
        private Slider _hourSlider;
        private Button _playButton;
        private TextMeshProUGUI _buttonText;
    #endregion

    
    #region Unity Methods

        private void Awake() {
            _hourSlider = GetComponent<Slider>();
            _playButton = GetComponentInChildren<Button>();
            _buttonText = _playButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start() {
            Hour = _hourSlider.value;
            _playButton.onClick.AddListener(Play);
            Play();
        }
        private void Update() {
            if (!Running) return;

            Hour += Time.deltaTime * multiplier;
            _hourSlider.value = Hour;
        }
    #endregion

    #region Other Methods
        public void UpdateHour(float f) {
            Hour = f;
        }

        private void Play() {
            Running = true;
            _buttonText.text = "||";
            _playButton.onClick.RemoveListener(Play);
            _playButton.onClick.AddListener(Pause);
        }

        private void Pause() {
            Running = false;
            _buttonText.text = ">";
            _playButton.onClick.RemoveListener(Pause);
            _playButton.onClick.AddListener(Play);
        }

    #endregion
}