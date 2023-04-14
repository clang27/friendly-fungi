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
        [SerializeField] private Button playButton;
        [SerializeField] private Slider hourSlider;
    #endregion
    
    #region Attributes
        public static bool Running { get; private set; }
        public static float Hour { get; private set; } = 9f;

    #endregion

    #region Private Data
        private TextMeshProUGUI _buttonText;
    #endregion

    #region Unity Methods
        private void Awake() {
            _buttonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
        }

        private void Start() {
            playButton.onClick.AddListener(Play);
        }
        private void Update() {
            if (!Running) return;

            Hour += Time.deltaTime * multiplier;
            if (Hour >= hourSlider.maxValue) {
                Hour = hourSlider.maxValue;
                Pause();
            }

            currentTime.text = Utility.FormatTime(Hour+hourOffset);
            hourSlider.value = Hour;
        }
    #endregion

    #region Other Methods
    
        public void SetLevelTime(Level l) {
            Hour = l.StartTime;
                
            startTime.text = Utility.FormatTime(l.StartTime+hourOffset);
            endTime.text = Utility.FormatTime(l.EndTime+hourOffset);
            currentTime.text = Utility.FormatTime(l.StartTime+hourOffset);

            hourSlider.minValue = l.StartTime;
            hourSlider.maxValue = l.EndTime;
            hourSlider.value = l.StartTime;
        }
        public void UpdateHour(float f) {
            Hour = f;
            currentTime.text = Utility.FormatTime(Hour+hourOffset);
        }

        public void Play() {
            Running = true;
            _buttonText.text = "||";
            playButton.onClick.RemoveListener(Play);
            playButton.onClick.AddListener(Pause);
        }

        public void Pause() {
            Running = false;
            _buttonText.text = ">";
            playButton.onClick.RemoveListener(Pause);
            playButton.onClick.AddListener(Play);
        }

    #endregion
}