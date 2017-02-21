namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoReportList : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
    {
        private bool _isFullColor;
        private float _timer;
        private const float FADE_OUT_TIME = 3f;

        private void DoSetFullColor(bool fullColor)
        {
            foreach (MonoBattleReportRow row in base.transform.GetComponentsInChildren<MonoBattleReportRow>(true))
            {
                if (fullColor)
                {
                    row.SetFullColorText();
                }
                else
                {
                    row.SetNoColorText();
                }
            }
        }

        public void Init()
        {
            this.DoSetFullColor(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.SetFullColor(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this._timer = 3f;
        }

        public void SetFullColor(bool fullColor)
        {
            if (this._isFullColor != fullColor)
            {
                this._isFullColor = fullColor;
                this.DoSetFullColor(this._isFullColor);
            }
        }

        public void Update()
        {
            if (this._timer >= 0f)
            {
                this._timer -= Time.unscaledDeltaTime;
                if (this._timer < 0f)
                {
                    this.SetFullColor(false);
                }
            }
        }
    }
}

