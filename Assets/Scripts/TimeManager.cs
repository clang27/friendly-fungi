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
        [SerializeField] private TextMeshProUGUI startTime, currentTime, endTime;
        [SerializeField] private float hourOffset = 2f;
    #endregion
    
    #region Attributes
        public static bool Running { get; private set; }
        public static float Hour { get; private set; } = 9f;

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
            startTime.text = Utility.FormatTime(_hourSlider.minValue+hourOffset);
            endTime.text = Utility.FormatTime(_hourSlider.maxValue+hourOffset);
            currentTime.text = Utility.FormatTime(_hourSlider.value+hourOffset);
            ShowUI(false);
        }
        private void Update() {
            if (!Running) return;

            Hour += Time.deltaTime * multiplier;
            if (Hour >= _hourSlider.maxValue) {
                Hour = _hourSlider.maxValue;
                Pause();
            }

            currentTime.text = Utility.FormatTime(Hour+hourOffset);
            _hourSlider.value = Hour;
        }
    #endregion

    #region Other Methods
        public void ShowUI(bool b) {
            _hourSlider.gameObject.SetActive(b);
            _playButton.gameObject.SetActive(b);
        }
        public void UpdateHour(float f) {
            Hour = f;
            currentTime.text = Utility.FormatTime(Hour+hourOffset);
        }

        public void Play() {
            Running = true;
            _buttonText.text = "||";
            _playButton.onClick.RemoveListener(Play);
            _playButton.onClick.AddListener(Pause);
        }

        public void Pause() {
            Running = false;
            _buttonText.text = ">";
            _playButton.onClick.RemoveListener(Pause);
            _playButton.onClick.AddListener(Play);
        }

    #endregion
}