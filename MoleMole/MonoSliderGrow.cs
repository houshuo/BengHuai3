namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Slider))]
    public class MonoSliderGrow : MonoBehaviour
    {
        private int _count;
        private float _ratioAfter;
        private float _ratioBefore;
        private Slider _slider;
        public int growTimes;
        public float maxValueAfter;
        public float maxValueBefore;
        public bool play;
        private const float SPEED = 1f;
        public float valueAfter;
        public float valueBefore;

        private void Awake()
        {
            this.play = false;
            this._slider = base.GetComponent<Slider>();
        }

        public void Play(float valBefore, float maxBefore, float valAfter, float maxValAfter, int growTimes)
        {
            this.play = true;
            this.valueBefore = valBefore;
            this.maxValueBefore = maxBefore;
            this.valueAfter = valAfter;
            this.maxValueAfter = maxValAfter;
            this.growTimes = growTimes;
            this._ratioBefore = this.valueBefore / this.maxValueBefore;
            this._ratioAfter = this.valueAfter / this.maxValueAfter;
            this._slider.maxValue = 1f;
            this._slider.value = this._ratioBefore;
            this._count = 0;
        }

        private void Update()
        {
            if (this.play)
            {
                float num = this._slider.value + (1f * Time.deltaTime);
                if ((this._count == this.growTimes) && (num >= this._ratioAfter))
                {
                    this.play = false;
                    this._slider.maxValue = this.maxValueAfter;
                    this._slider.value = this.valueAfter;
                }
                else
                {
                    if (num >= 1f)
                    {
                        this._count++;
                        num = 0f;
                    }
                    this._slider.value = num;
                }
            }
        }
    }
}

