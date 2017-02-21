namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode]
    public class MonoDeviceAspectAdapter : MonoBehaviour
    {
        public FixFunction fixFunction;
        public ScreenMode screenMode;

        public void FixLevelPanel()
        {
            Vector2 cellSize = base.transform.GetComponent<GridLayoutGroup>().cellSize;
            if (this.screenMode == ScreenMode.ANDROID_16_10_MODE)
            {
                cellSize.y = 700f;
            }
            else if ((this.screenMode == ScreenMode.IPAD_SCREEN_MODE) || (this.screenMode == ScreenMode.IPHONE_SCREEN_MODE))
            {
                cellSize.y = 900f;
            }
            base.transform.GetComponent<GridLayoutGroup>().cellSize = cellSize;
        }

        private ScreenMode GetScreenMode()
        {
            float num = (1f * Mathf.Max(Screen.width, Screen.height)) / ((float) Mathf.Min(Screen.width, Screen.height));
            if (num > 1.76)
            {
                return ScreenMode.IPHONE_5_SCREEN_MODE;
            }
            if (num >= 1.6)
            {
                return ScreenMode.ANDROID_16_10_MODE;
            }
            if (num > 1.49)
            {
                return ScreenMode.IPHONE_SCREEN_MODE;
            }
            return ScreenMode.IPAD_SCREEN_MODE;
        }

        private void OnEnable()
        {
            this.screenMode = this.GetScreenMode();
            if (this.fixFunction != null)
            {
                this.fixFunction.Invoke();
            }
        }

        public enum ScreenMode
        {
            IPHONE_5_SCREEN_MODE,
            ANDROID_16_10_MODE,
            IPHONE_SCREEN_MODE,
            IPAD_SCREEN_MODE
        }
    }
}

