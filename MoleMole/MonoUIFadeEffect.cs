namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoUIFadeEffect : MonoBehaviour
    {
        private float _duration;
        private Image _image;
        private float _time;
        [SerializeField]
        public AnimationCurve AlphaCurve;

        public void Init(float duration)
        {
            this._duration = duration;
            this._time = 0f;
        }

        private void Start()
        {
            this._image = base.transform.Find("Panel").GetComponent<Image>();
        }

        private void Update()
        {
            this._time += Time.deltaTime;
            float time = this._time / this._duration;
            if (this._time <= 1f)
            {
                Color color = this._image.color;
                color.a = this.AlphaCurve.Evaluate(time);
                this._image.color = color;
            }
            else
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
    }
}

