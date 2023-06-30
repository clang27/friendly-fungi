/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {
    #region Components
        [SerializeField] private TextMeshProUGUI currentTimeTextMesh;
        [SerializeField] private Slider hourSlider;
        [SerializeField] private UnityEvent<float> onUpdate;
    #endregion
    
    #region Attributes
        public float CurrentTime { get; private set; }
        public float StartTime { get; private set; }
        public float EndTime { get; private set; }
    #endregion

    #region Other Methods
        public void UpdateTimeUi(float f) {
            CurrentTime = f;
            if (!Mathf.Approximately(hourSlider.value, f))
                hourSlider.SetValueWithoutNotify(f);
            
            currentTimeTextMesh.text = Utility.FormatTime(CurrentTime+TimeManager.HourOffset);
        }

        public void SetLevelTime(Level l) {
            StartTime = l.StartTime;
            EndTime = l.EndTime;
            
            //Stupid workaround bc minvalue triggers on update
            RemoveOnUpdate();
            hourSlider.minValue = l.StartTime;
            hourSlider.maxValue = l.EndTime;
            UpdateTimeUi(l.StartTime);
            AddOnUpdate();
        }
        public void RemoveOnUpdate() {
            hourSlider.onValueChanged.RemoveAllListeners();
        }
        
        public void AddOnUpdate() {
            hourSlider.onValueChanged.AddListener(onUpdate.Invoke);
        }

    #endregion
}