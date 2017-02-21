namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoMaskSliderGrow : MonoBehaviour
    {
        private int _count;
        private float _ratioAfter;
        private float _ratioBefore;
        private float _value;
        public Text currentValueText;
        private Action<Transform> everyFullAction;
        private int growTimes;
        public MonoMaskSlider maskSlider;
        private List<float> maxList;
        private float maxValueAfter;
        private float maxValueBefore;
        public Text MaxValueText;
        private Action<Transform> overAction;
        private bool play;
        private float speed = 0.5f;
        private float valueAfter;
        private float valueBefore;

        public void Play(float valBefore, float valAfter, List<float> maxList, Action<Transform> firstFullAction = null, Action<Transform> overAction = null)
        {
            this.everyFullAction = firstFullAction;
            this.overAction = overAction;
            this.maxList = maxList;
            this.growTimes = maxList.Count;
            this.valueAfter = valAfter;
            float num = maxList[0];
            float num2 = maxList[maxList.Count - 1];
            this.speed = ((((num - valBefore) / num) + (valAfter / num2)) + ((this.growTimes <= 2) ? 0f : ((float) (this.growTimes - 2)))) / (60f * this.maskSlider.sliderGrowTime);
            this._ratioBefore = valBefore / num;
            this._ratioAfter = valAfter / num2;
            this._value = this._ratioBefore;
            this.maskSlider.UpdateValue(this._ratioBefore, 1f, 0f);
            if (this.currentValueText != null)
            {
                this.currentValueText.text = valBefore.ToString();
            }
            if (this.MaxValueText != null)
            {
                this.MaxValueText.text = num.ToString();
            }
            this._count = 1;
            this.play = true;
        }

        private void Update()
        {
            if (this.play)
            {
                this._value += this.speed;
                if ((this._count >= this.growTimes) && (this._value >= this._ratioAfter))
                {
                    this.play = false;
                    this.maskSlider.UpdateValue(this._ratioAfter, 1f, 0f);
                    if (this.currentValueText != null)
                    {
                        this.currentValueText.text = this.valueAfter.ToString();
                    }
                    if (this.MaxValueText != null)
                    {
                        float num = this.maxList[this.maxList.Count - 1];
                        if (this._count < this.maxList.Count)
                        {
                            num = this.maxList[this._count - 1];
                        }
                        this.MaxValueText.text = num.ToString();
                    }
                    if (this.overAction != null)
                    {
                        this.overAction(base.transform);
                    }
                }
                else
                {
                    if (this._value >= 1f)
                    {
                        if (this.everyFullAction != null)
                        {
                            this.everyFullAction(base.transform);
                        }
                        this._count++;
                        if (this.MaxValueText != null)
                        {
                            float num2 = this.maxList[this.maxList.Count - 1];
                            if (this._count < this.maxList.Count)
                            {
                                num2 = this.maxList[this._count - 1];
                            }
                            this.MaxValueText.text = num2.ToString();
                        }
                        this._value = 0f;
                    }
                    float num3 = this.maxList[this._count - 1];
                    if (this.currentValueText != null)
                    {
                        this.currentValueText.text = Mathf.FloorToInt(this._value * num3).ToString();
                    }
                    this.maskSlider.UpdateValue(this._value, 1f, 0f);
                }
            }
        }
    }
}

