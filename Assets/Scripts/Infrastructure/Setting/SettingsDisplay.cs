using UnityEngine;

namespace Infrastructure.Setting
{
    [CreateAssetMenu(fileName = "DisplaySetting", menuName = "Display/Setting", order = 0)]
    public class SettingsDisplay : ScriptableObject
    {
        public Camera verticalDisplay;
        public Camera globusDisplay;


        public void SetDisplay(int value)
        {
            if (value == 0)
            {
                verticalDisplay.targetDisplay = 1;
                globusDisplay.targetDisplay = 2;
            }
            else if (value == 1)
            {
                verticalDisplay.targetDisplay = 2;
                globusDisplay.targetDisplay = 1;
            }
        }
    }
}