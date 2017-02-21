namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Scrollbar)), RequireComponent(typeof(CanvasGroup))]
    public class MonoScrollBarAutoHide : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private float _fadeOutTimer;
        private bool _isFadingOut;
        public float fadeOutTimeSpan = 0.5f;
        public bool hidebyDefault;

        private void Awake()
        {
            this._isFadingOut = false;
            this._canvasGroup = base.GetComponent<CanvasGroup>();
            this._canvasGroup.alpha = 0f;
            this._fadeOutTimer = 0f;
        }

        private void Update()
        {
            if (this._isFadingOut)
            {
                this._fadeOutTimer += Time.deltaTime;
                if (this._fadeOutTimer < this.fadeOutTimeSpan)
                {
                    this._canvasGroup.alpha = 1f - (this._fadeOutTimer / this.fadeOutTimeSpan);
                }
                else
                {
                    this._canvasGroup.alpha = 0f;
                    this._isFadingOut = false;
                }
            }
        }

        public void UpdateStatus(bool visible)
        {
            if (this._canvasGroup == null)
            {
                this._canvasGroup = base.GetComponent<CanvasGroup>();
            }
            this._canvasGroup.alpha = !visible ? 0f : 1f;
        }

        public void UpdateStatus(float velocity)
        {
            if (Mathf.Abs(velocity) > 2f)
            {
                this._canvasGroup.alpha = 1f;
                this._isFadingOut = false;
            }
            else if (!this._isFadingOut)
            {
                this._isFadingOut = true;
                this._fadeOutTimer = 0f;
            }
        }
    }
}

