using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.Setting
{
    [CreateAssetMenu(fileName = "DisplaySetting", menuName = "Display/Setting", order = 0)]
    public class SettingsDisplay : ScriptableObject
    {
        public Camera verticalDisplay;
        public Camera globusDisplay;

        public float zoomSpeed;
        public float rotationSpeed;

        private void Awake()
        {
            SetSpeedZoom(PlayerPrefs.GetFloat(nameof(zoomSpeed)));
            SetRotationSpeed(PlayerPrefs.GetFloat(nameof(rotationSpeed)));
        }

        public void SetSpeedZoom(float speedZoom)
        {
            zoomSpeed = speedZoom;
            SetPlayerPerfs(nameof(zoomSpeed),zoomSpeed);
        }

        public void SetRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
            SetPlayerPerfs(nameof(this.rotationSpeed), rotationSpeed);
        }

        public void SetPlayerPerfs(string name,float value) => PlayerPrefs.SetFloat(name,value);

        public void SetDisplay(int value)
        {
            verticalDisplay.targetDisplay = value == 0 ? 1 : 2;
            globusDisplay.targetDisplay = value == 0 ? 2 : 1;
            
        }
    }
}