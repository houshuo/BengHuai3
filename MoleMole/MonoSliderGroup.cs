namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoSliderGroup : MonoBehaviour
    {
        private LocalAvatarHealthMode _healthyMode;
        private float _perRatio;
        private int _segmentNum;
        private MonoMaskSlider[] _sliders;
        public Sprite healthyHPSprite;
        public float maxValue = 1f;
        public float minValue;
        public Sprite unhealthyHPSprite;
        public float value;

        public void Init()
        {
            this._sliders = base.GetComponentsInChildren<MonoMaskSlider>();
            this._segmentNum = this._sliders.Length;
            this._healthyMode = LocalAvatarHealthMode.Healthy;
            this._perRatio = 1f / ((float) this._segmentNum);
            Material material = this._sliders[0].GetComponentInChildren<ImageForSmoothMask>().material;
            for (int i = 0; i < this._segmentNum; i++)
            {
                this._sliders[i].maxValue = 1f;
                this._sliders[i].GetComponent<Image>().material = material;
                this._sliders[i].GetComponentInChildren<ImageForSmoothMask>().material = material;
            }
        }

        private void OnDestroy()
        {
            this._sliders = null;
            this.healthyHPSprite = null;
            this.unhealthyHPSprite = null;
        }

        public void SetupInDanageView(LocalAvatarHealthMode mode)
        {
            if (mode != this._healthyMode)
            {
                this._healthyMode = mode;
                Sprite sprite = (this._healthyMode != LocalAvatarHealthMode.Healthy) ? this.unhealthyHPSprite : this.healthyHPSprite;
                foreach (MonoMaskSlider slider in this._sliders)
                {
                    slider.transform.Find("Slider/Fill").GetComponent<Image>().sprite = sprite;
                }
            }
        }

        public void UpdateValue(float value, float maxValue, float minValue = 0)
        {
            if (this._sliders == null)
            {
                this.Init();
            }
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = (value <= maxValue) ? value : maxValue;
            float num = (this.value - this.minValue) / (this.maxValue - this.minValue);
            int num2 = Mathf.FloorToInt(num / this._perRatio);
            float num3 = (num / this._perRatio) - num2;
            for (int i = 0; i < this._segmentNum; i++)
            {
                if (i < num2)
                {
                    this._sliders[i].UpdateValue(1f, 1f, 0f);
                }
                else if (i == num2)
                {
                    this._sliders[i].UpdateValue(num3, 1f, 0f);
                }
                else
                {
                    this._sliders[i].UpdateValue(0f, 1f, 0f);
                }
            }
        }
    }
}

