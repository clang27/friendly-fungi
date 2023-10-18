/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
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
        public static float SecondMultiplier => _secondMultiplier;

    #endregion

    #region Private Data
        private ParticleSystem[] _particleSystems;
        private static float _secondMultiplier = 1.0f;
        private Tweener _timeTransition;
        private PlayableDirector _playableDirector;
    #endregion

    #region Unity Methods
        private void Start() {
            playButton.onClick.AddListener(Pause);
            UpdateMultiplier(1f);
        }
        private void Update() {
            if (Running) {
                UpdateTime(Hour + Time.deltaTime * multiplier * _secondMultiplier);
            }
            
            SetTimeline((Hour - _timeSlider.StartTime) * 60f);
        }
        
    #endregion

    #region Other Methods
        public void UpdateMultiplier(float f) {
            _secondMultiplier = f;
            
            oneMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 1f);
            twoMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 3f);
            fourMultiButton.interactable = !Mathf.Approximately(_secondMultiplier, 6f);
        }
        public void UpdateTime(float f) {
            Hour = f;
            if (Hour >= _timeSlider.EndTime) {
                Hour = _timeSlider.EndTime;
                Pause();
                PausedFlag = true;
            }

            _timeSlider.UpdateTimeUi(Hour);
        }
        public void Init(Transform t) {
            _particleSystems = t.GetComponentsInChildren<ParticleSystem>();
            
            _playableDirector = FindObjectOfType<PlayableDirector>();
        }
        public void SetLevelTime(Level l) {
            Hour = l.StartTime;
            _timeSlider.SetLevelTime(l);
        }

        public void SlideTimeToNight() {
            if (_timeTransition != null && _timeTransition.IsActive() && _timeTransition.IsPlaying())
                _timeTransition.Kill();
            
            _timeTransition = DOVirtual.Float(Hour, 22f, 2f, value => Hour = value);
        }

        public void SlideTimeToLevel(Level l) {
            if (_timeTransition != null && _timeTransition.IsActive() && _timeTransition.IsPlaying())
                _timeTransition.Kill();
            
            _timeTransition = DOVirtual.Float(Hour, l.StartTime, 2f, value => Hour = value);
            _timeSlider.SetLevelTime(l);
        }

        public void Play() {
            Running = true;
            PausedFlag = false;
            playButton.GetComponent<Image>().sprite = pauseSprite;

            PlayParticles();
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Pause);
            playButton.onClick.AddListener(() => { PausedFlag = true; });
        }

        public void Pause() {
            Running = false;
            playButton.GetComponent<Image>().sprite = playSprite;
            
            PauseParticles();
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(Play);
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
        
        private void SetTimeline(float f) {
            //Debug.Log("Setting time to " + f);
            _playableDirector.time = f;
            _playableDirector.DeferredEvaluate();
        }

    #endregion
}