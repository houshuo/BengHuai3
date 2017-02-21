namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class PressWithCallBack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IPointerExitHandler
    {
        [SerializeField]
        private bool _isPress;
        public OnPress onPress;

        public void OnPointerDown(PointerEventData eventData)
        {
            this.IsPress = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.IsPress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            this.IsPress = false;
        }

        public bool IsPress
        {
            get
            {
                return this._isPress;
            }
            set
            {
                this._isPress = value;
                if (this.onPress != null)
                {
                    this.onPress(base.transform, this._isPress);
                }
            }
        }

        public delegate void OnPress(Transform trans, bool isPress);
    }
}

