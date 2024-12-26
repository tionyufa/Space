using System;
using Controllers;
using Controllers.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Infrastructure.Setting
{
    public class UISettingDisplay : MonoBehaviour
    {
        [SerializeField] private SettingsDisplay _settingsDisplay;
        [SerializeField] private Slider _sliderZoom;
        [SerializeField] private Slider _sliderRotation;
        [SerializeField] private TextMeshProUGUI _zoomText;
        [SerializeField] private TextMeshProUGUI _rotationText;
        private MobileInput _input;
        private void Awake()
        {
            OnChangeSlideSpeedRotation(_settingsDisplay.rotationSpeed);
            OnChangeSlideSpeedZoom(_settingsDisplay.zoomSpeed);
            _sliderZoom.value = _settingsDisplay.zoomSpeed;
            _sliderRotation.value = _settingsDisplay.rotationSpeed;
            _sliderZoom.onValueChanged.AddListener(OnChangeSlideSpeedZoom);
            _sliderRotation.onValueChanged.AddListener(OnChangeSlideSpeedRotation);
        }

        private void OnChangeSlideSpeedZoom(float arg0)
        {
            _settingsDisplay.SetSpeedZoom(arg0);
            if (_zoomText)
                _zoomText.text = "Zoom - " + arg0.ToString("F1");
        }

        private void OnChangeSlideSpeedRotation(float arg0)
        {
            _settingsDisplay.SetRotationSpeed(arg0);
            if (_rotationText)
                _rotationText.text = "Rotation - " + arg0.ToString("F1");
        }

        public void Construct(SettingsDisplay settingsDisplay)
        {
            _settingsDisplay = settingsDisplay;
        }
        public void OnChange(int value) => _settingsDisplay?.SetDisplay(value);

        public void Quit() => Application.Quit();
    }
}