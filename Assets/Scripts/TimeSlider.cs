/*
 * Copyright (c) Knitwit Studios LLC
 * https://www.knitwitstudios.com/
 */

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour {
    #region Components
        [SerializeField] private TextMeshProUGUI currentTimeTextMesh;
        [SerializeField] private Slider hourSlider;
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
            CurrentTime = l.StartTime;

            currentTimeTextMesh.text = Utility.FormatTime(StartTime+TimeManager.HourOffset);

            hourSlider.minValue = l.StartTime;
            hourSlider.maxValue = l.EndTime;
            hourSlider.SetValueWithoutNotify(l.StartTime);
        }

    #endregion
}