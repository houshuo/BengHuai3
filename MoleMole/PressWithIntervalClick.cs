namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class PressWithIntervalClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
    {
        private Button _btn;
        private bool _isPointDown;
        private float _timer;
        public float intervalSeconds = 0.2f;

        private void Awake()
        {
            this._btn = base.GetComponent<Button>();
            this._timer = 0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (this._btn.interactable)
            {
                this._isPointDown = true;
                this._timer = this.intervalSeconds;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this._isPointDown = false;
            this._timer = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (this._btn.interactable)
            {
                this._isPointDown = false;
                this._timer = 0f;
            }
        }

        private void Update()
        {
            if (this._btn.interactable && (this._isPointDown && (this._timer > 0f)))
            {
                this._timer -= Time.deltaTime;
                if (this._timer <= 0f)
                {
                    this._btn.onClick.Invoke();
                    this._timer = this.intervalSeconds;
                }
            }
        }
    }
}

