namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class MonoJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler
    {
        private Button _button;
        private BaseAvatarInputController _controller;
        [ShowInInspector]
        private int _controlPointerID = -253;
        private float _currentAngle;
        private Vector2 _currentScreenHitPos;
        private RectTransform _imageTrans;
        private bool _isPointerDown;
        private float _lastAnalogInputX;
        private float _lastAnalogInputY;
        private bool _pointerLeaveTrigger;
        private Camera _uiCamera;
        private bool _updateAfterEnable;
        private bool _useVirtualJoyStick;
        private const int POINTER_NONE = -253;

        private void Awake()
        {
            this._button = base.GetComponent<Button>();
            this._uiCamera = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().GetComponent<Canvas>().worldCamera;
            this._imageTrans = base.transform.Find("Image") as RectTransform;
        }

        public void InitJoystick(BaseAvatarInputController controller)
        {
            this._controller = controller;
            base.gameObject.SetActive(true);
        }

        private void OnActive(Vector2 hitPos)
        {
            this._currentAngle = this.SetGraphicRotate(hitPos);
            this.IsMoving = true;
            this._controller.TryMove(this.IsMoving, this.CurrentAngleV2);
            this._button.image.overrideSprite = this._button.spriteState.pressedSprite;
            this._useVirtualJoyStick = true;
        }

        private void OnDisable()
        {
            this._isPointerDown = false;
            this._pointerLeaveTrigger = false;
            this._currentAngle = 0f;
            this._controlPointerID = -253;
            this._lastAnalogInputX = 0f;
            this._lastAnalogInputY = 0f;
            this.IsMoving = false;
            this.ResetGraphicRotate();
        }

        public void OnDrag(PointerEventData data)
        {
            if (((this._controlPointerID != -253) && (data.pointerId == this._controlPointerID)) && this._isPointerDown)
            {
                this._currentScreenHitPos = data.position;
                this.OnActive(this._currentScreenHitPos);
            }
        }

        private void OnEnable()
        {
            this._updateAfterEnable = true;
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (this._controlPointerID == -253)
            {
                this._controlPointerID = data.pointerId;
            }
            else if (this._controlPointerID != data.pointerId)
            {
                return;
            }
            this._isPointerDown = true;
            this._currentScreenHitPos = data.position;
        }

        public void OnPointerUp(PointerEventData data)
        {
            if ((this._controlPointerID != -253) && (this._controlPointerID == data.pointerId))
            {
                this._controlPointerID = -253;
                this._isPointerDown = false;
                this._pointerLeaveTrigger = true;
            }
        }

        private void OnUnactive()
        {
            this._isPointerDown = false;
            this._pointerLeaveTrigger = false;
            this._currentAngle = 0f;
            this._controlPointerID = -253;
            this._lastAnalogInputX = 0f;
            this._lastAnalogInputY = 0f;
            this.IsMoving = false;
            this.ResetGraphicRotate();
            this._controller.TryMove(this.IsMoving, this.CurrentAngleV2);
            this._useVirtualJoyStick = false;
        }

        private void ResetGraphicRotate()
        {
            this._imageTrans.SetLocalEulerAnglesZ(0f);
        }

        private float SetGraphicRotate(Vector2 hitPos)
        {
            Vector3 vector = (Vector3) RectTransformUtility.WorldToScreenPoint(this._uiCamera, base.transform.position);
            float x = hitPos.x - vector.x;
            float y = hitPos.y - vector.y;
            float z = (Mathf.Atan2(y, x) * 57.29578f) - 90f;
            this._imageTrans.SetLocalEulerAnglesZ(z);
            return z;
        }

        public void Update()
        {
            if (this._updateAfterEnable)
            {
                this._updateAfterEnable = false;
                this.OnUnactive();
            }
            if (this._isPointerDown && (Input.touchCount == 0))
            {
                this._controlPointerID = -253;
                this._isPointerDown = false;
                this._pointerLeaveTrigger = true;
            }
            if (this._isPointerDown)
            {
                this.OnActive(this._currentScreenHitPos);
            }
            else if (this._pointerLeaveTrigger)
            {
                this._pointerLeaveTrigger = false;
                this.OnUnactive();
            }
        }

        private void UpdateRealJoyStick()
        {
            if (!Singleton<LevelManager>.Instance.IsPaused())
            {
                Button component = base.GetComponent<Button>();
                float axisRaw = Input.GetAxisRaw("Horizontal");
                float num2 = Input.GetAxisRaw("Vertical");
                if ((axisRaw == 0f) && (num2 == 0f))
                {
                    if ((axisRaw == this._lastAnalogInputX) && (num2 == this._lastAnalogInputY))
                    {
                        return;
                    }
                    this._imageTrans.SetLocalEulerAnglesZ(0f);
                    this._currentAngle = 0f;
                    this.IsMoving = false;
                    this._controller.TryMove(this.IsMoving, this.CurrentAngleV2);
                    component.image.overrideSprite = null;
                }
                else
                {
                    if ((axisRaw > 0f) && (num2 > 0f))
                    {
                        this._currentAngle = -45f;
                    }
                    else if ((axisRaw > 0f) && (num2 < 0f))
                    {
                        this._currentAngle = -135f;
                    }
                    else if ((axisRaw > 0f) && (num2 == 0f))
                    {
                        this._currentAngle = -90f;
                    }
                    else if ((axisRaw < 0f) && (num2 > 0f))
                    {
                        this._currentAngle = 45f;
                    }
                    else if ((axisRaw < 0f) && (num2 < 0f))
                    {
                        this._currentAngle = -225f;
                    }
                    else if ((axisRaw < 0f) && (num2 == 0f))
                    {
                        this._currentAngle = -270f;
                    }
                    else if ((axisRaw == 0f) && (num2 > 0f))
                    {
                        this._currentAngle = 0f;
                    }
                    else if ((axisRaw == 0f) && (num2 < 0f))
                    {
                        this._currentAngle = -180f;
                    }
                    this._imageTrans.SetLocalEulerAnglesZ(this._currentAngle);
                    this.IsMoving = true;
                    this._controller.TryMove(this.IsMoving, this.CurrentAngleV2);
                    component.image.overrideSprite = component.spriteState.pressedSprite;
                }
                this._lastAnalogInputX = axisRaw;
                this._lastAnalogInputY = num2;
            }
        }

        public float CurrentAngleV2
        {
            get
            {
                return ((this._currentAngle >= -180f) ? this._currentAngle : (this._currentAngle + 360f));
            }
        }

        public bool IsMoving { get; private set; }
    }
}

