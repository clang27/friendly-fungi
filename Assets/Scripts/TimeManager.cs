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
        [SerializeField] private Button playButton;
        [SerializeField] private TimeSlider _timeSlider;
    #endregion
    
    #region Attributes
        public static int HourOffset => 2;
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
            
            UpdateTime(Hour + Time.deltaTime * multiplier);
        }
    #endregion

    #region Other Methods
        public void UpdateTime(float f) {
            Hour = f;
            if (Hour >= _timeSlider.EndTime) {
                Hour = _timeSlider.EndTime;
                Pause();
            }
            
            _timeSlider.UpdateTimeUi(Hour);
            
            foreach (var m in MushroomManager.AllActive) {
                m.SetTimeline((Hour - _timeSlider.StartTime) * 60f);
            }
        }

        public void SetLevelTime(Level l) {
            Hour = l.StartTime;
            _timeSlider.SetLevelTime(l);
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