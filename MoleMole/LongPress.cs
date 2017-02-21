namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class LongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
    {
        private bool isLongPressTrigged;
        private bool isPointDown;
        private float LongPressThreshold = 1f;
        private Action<UnityEngine.Object> OnLongPress;
        private float pressStarTime;

        public void OnPointerDown(PointerEventData eventData)
        {
            this.isPointDown = true;
            this.pressStarTime = Time.time;
            this.isLongPressTrigged = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.isPointDown = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.isPointDown = false;
        }

        public void SetLongPressAction(Action<UnityEngine.Object> action)
        {
            this.OnLongPress = action;
        }

        public void SetLongPressThreshold(float threshold)
        {
            this.LongPressThreshold = threshold;
        }

        private void Start()
        {
        }

        private void Update()
        {
            if ((this.isPointDown && !this.isLongPressTrigged) && ((Time.time - this.pressStarTime) >= this.LongPressThreshold))
            {
                this.isLongPressTrigged = true;
                if (this.OnLongPress != null)
                {
                    this.OnLongPress(null);
                }
            }
        }
    }
}

