namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [SerializeField]
    public class MonoMaskSlider : MonoBehaviour
    {
        public Slider.Direction dirction;
        public RectTransform fillRect;
        public RectTransform maskRect;
        public float maxValue = 1f;
        public float minValue;
        public Action<float, float> onValueChanged;
        public float sliderGrowTime = 0.2f;
        public float value;

        private float GetRatio()
        {
            if (this.maxValue == this.minValue)
            {
                return 1f;
            }
            return ((this.value - this.minValue) / (this.maxValue - this.minValue));
        }

        public void UpdateValue(float value, float maxValue, float minValue = 0)
        {
            float ratio = this.GetRatio();
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = (value <= maxValue) ? value : maxValue;
            Rect rect = this.maskRect.rect;
            float num2 = this.GetRatio();
            float width = 0f;
            int num4 = 1;
            switch (this.dirction)
            {
                case Slider.Direction.LeftToRight:
                    width = rect.width;
                    break;

                case Slider.Direction.RightToLeft:
                    width = rect.width;
                    num4 = -1;
                    break;

                case Slider.Direction.BottomToTop:
                    width = rect.height;
                    break;

                case Slider.Direction.TopToBottom:
                    width = rect.height;
                    num4 = -1;
                    break;
            }
            float x = (num4 * width) * (1f - num2);
            this.maskRect.anchoredPosition = new Vector2(-x, this.maskRect.anchoredPosition.y);
            this.fillRect.anchoredPosition = new Vector2(x, this.fillRect.anchoredPosition.y);
            if (this.onValueChanged != null)
            {
                this.onValueChanged(ratio, num2);
            }
        }
    }
}

