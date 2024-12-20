using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.Setting
{
    public class UISettingDisplay : MonoBehaviour
    {
        [SerializeField] private SettingsDisplay _settingsDisplay;

        public void Construct(SettingsDisplay settingsDisplay) => _settingsDisplay = settingsDisplay;

        public void OnChange(int value) => _settingsDisplay?.SetDisplay(value);

        public void Quit() => Application.Quit();
    }
}