namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Text))]
    public class MonoNumberGrow : MonoBehaviour
    {
        private Text _text;
        private float _timer;
        private float _value;
        public float duration = 1f;
        public bool isInt;
        public bool play;
        public float valueAfter;
        public float valueBefore;

        private void Awake()
        {
            this.play = false;
            this._text = base.GetComponent<Text>();
        }

        public void Play(float valueBefore, float valueAfter, float duration, bool isInt = true)
        {
            if (valueBefore != valueAfter)
            {
                this.play = true;
                this.isInt = isInt;
                this.valueBefore = valueBefore;
                this.valueAfter = valueAfter;
                this.duration = duration;
                this._timer = 0f;
                this._value = valueBefore;
                this._text.text = this._value.ToString();
            }
        }

        private void Update()
        {
            if (this.play)
            {
                this._timer += Time.deltaTime;
                this._value = Mathf.Lerp(this.valueBefore, this.valueAfter, this._timer / this.duration);
                if (Mathf.Approximately(this._value, this.valueAfter))
                {
                    this.play = false;
                    this._value = this.valueAfter;
                }
                if (this.isInt)
                {
                    this._text.text = (this.valueAfter <= this.valueBefore) ? Mathf.CeilToInt(this._value).ToString() : Mathf.FloorToInt(this._value).ToString();
                }
                else
                {
                    this._text.text = this._value.ToString();
                }
            }
        }
    }
}

