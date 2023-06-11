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
        [SerializeField] private Button playButton, oneMultiButton, twoMultiButton, fourMultiButton;
        [SerializeField] private Sprite playSprite, pauseSprite;
        [SerializeField] private TimeSlider _timeSlider;
    #endregion
    
    #region Attributes
        public static int HourOffset => 2;
        public static bool Running { get; private set; }
        public static float Hour { get; private set; } = 9f;
        public static bool PausedFlag { get; private set; }

    #endregion

    #region Private Data
        private ParticleSystem[] _particleSystems;
        private float _secondMultiplier = 1.0f;
        private TimeManipulation[] _manipulations;
    #endregion

    #region Unity Methods
        private void Start() {
            playButton.onClick.AddListener(Pause);
            UpdateMultiplier(1f);
        }
        private void Update() {
            if (!Running) return;
            
            UpdateTime(Hour + Time.deltaTime * multiplier * _secondMultiplier);
        }

        private void LateUpdate() {
            foreach (var m in _manipulations)
                m.SetTimeline((Hour - _timeSlider.StartTime) * 60f);
        }
        
    #endregion

    #region Other Methods
        public void UpdateMultiplier(float f) {
            _secondMultiplier = f;
            
            oneMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 1f);
            twoMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 2f);
            fourMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 4f);
        }
        public void UpdateTime(float f) {
            Hour = f;
            if (Hour >= _timeSlider.EndTime) {
                Hour = _timeSlider.EndTime;
                Pause();
                PausedFlag = true;
                PauseParticles();
            }
            
            _timeSlider.UpdateTimeUi(Hour);
        }
        public void Init(Transform t) {
            _particleSystems = t.GetComponentsInChildren<ParticleSystem>();
            _manipulations = FindObjectsOfType<TimeManipulation>();
        }
        public void SetLevelTime(Level l) {
            Hour = l.StartTime;
            _timeSlider.SetLevelTime(l);
        }

        public void Play() {
            Running = true;
            PausedFlag = false;
            playButton.GetComponent<Image>().sprite = pauseSprite;
            
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Pause);
            playButton.onClick.AddListener(() => { PausedFlag = true; });
            playButton.onClick.AddListener(PauseParticles);
        }

        public void Pause() {
            Running = false;
            playButton.GetComponent<Image>().sprite = playSprite;
            
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Play);
            playButton.onClick.AddListener(PlayParticles);
        }
        public void PlayParticles() {
            foreach (var ps in _particleSystems) {
                ps.Play();
            }
        }
        
        public void PauseParticles() {
            foreach (var ps in _particleSystems) {
                ps.Pause();
            }
        }

    #endregion
}