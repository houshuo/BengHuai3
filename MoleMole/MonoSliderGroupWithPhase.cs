namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoSliderGroupWithPhase : MonoBehaviour
    {
        private int _currentPhase;
        private float _perPhaseRatio;
        private float _perSegmentRatio;
        private int _segmentNum;
        private MonoMaskSlider[] _sliders;
        private const int MAX_PHASE = 4;
        public int maxPhase = 1;
        public float maxValue = 1f;
        public float minValue;
        public Sprite[] spriteList;
        public float value;

        public void Init()
        {
            this._sliders = base.GetComponentsInChildren<MonoMaskSlider>();
            this._segmentNum = this._sliders.Length;
            this._perPhaseRatio = 1f / ((float) this.maxPhase);
            this._perSegmentRatio = 1f / ((float) this._segmentNum);
            Material material = this._sliders[0].GetComponentInChildren<ImageForSmoothMask>().material;
            for (int i = 0; i < this._segmentNum; i++)
            {
                this._sliders[i].maxValue = 1f;
                this._sliders[i].GetComponent<Image>().material = material;
                this._sliders[i].GetComponentInChildren<ImageForSmoothMask>().material = material;
            }
            this._currentPhase = this.maxPhase;
            this.SetupPhaseView();
        }

        private void OnDestroy()
        {
            this._sliders = null;
            this.spriteList = null;
        }

        private void SetupPhaseView()
        {
            foreach (MonoMaskSlider slider in this._sliders)
            {
                int index = Mathf.Max(0, this._currentPhase - 1);
                slider.transform.GetComponent<Image>().sprite = this.spriteList[index];
                slider.transform.Find("Slider/Fill").GetComponent<Image>().sprite = this.spriteList[this._currentPhase];
            }
        }

        public void UpdateMaxPhase(int newMaxPhase)
        {
            if (this.maxPhase != newMaxPhase)
            {
                this.maxPhase = newMaxPhase;
                this._perPhaseRatio = 1f / ((float) this.maxPhase);
                this.UpdateValue(this.value, this.maxValue, this.minValue);
            }
        }

        public void UpdateValue(float value, float maxValue, float minValue = 0)
        {
            float num;
            if (this._sliders == null)
            {
                this.Init();
            }
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.value = (value <= maxValue) ? value : maxValue;
            if (maxValue == 0f)
            {
                num = 1f;
            }
            else
            {
                num = (this.value - this.minValue) / (this.maxValue - this.minValue);
            }
            int num2 = Mathf.CeilToInt(num / this._perPhaseRatio);
            if (this._currentPhase != num2)
            {
                this._currentPhase = num2;
                this.SetupPhaseView();
            }
            float num3 = (num / this._perPhaseRatio) - Mathf.Max(0, this._currentPhase - 1);
            int num4 = Mathf.FloorToInt(num3 / this._perSegmentRatio);
            float num5 = (num3 / this._perSegmentRatio) - num4;
            for (int i = 0; i < this._segmentNum; i++)
            {
                if (i < num4)
                {
                    this._sliders[i].UpdateValue(1f, 1f, 0f);
                }
                else if (i == num4)
                {
                    this._sliders[i].UpdateValue(num5, 1f, 0f);
                }
                else
                {
                    this._sliders[i].UpdateValue(0f, 1f, 0f);
                }
            }
        }
    }
}

