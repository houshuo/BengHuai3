namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoAvatarRotatePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Vector3 _autoRotateTargetDirection;
        private const float _autoRotateThreshold = 0.001f;
        private AvatarDataItem _avatarDataItem;
        private Transform _avatarModel;
        private float _currentAutoRotateSpeed;
        private float _currentRotateSpeed;
        private const float _deltaTimePerFrame = 0.016f;
        private bool _inAutoRotate;
        private bool _inSlowDown;
        private bool _isPointerDown;
        private bool _isPointerInPanel;
        private bool _isRotateWhenDragOutPanel = true;
        private List<float> _lastSeveralFramesSpeed = new List<float>();
        private bool _rotateRight;
        private float _rotateSpeedRatio = 100f;
        private const float _stopRotateThreshold = 0.001f;
        public float autoRotateSpeedLerpRatio = 10f;
        [NonSerialized]
        public bool enableDrawGizmos = true;
        [NonSerialized]
        public bool enableManualRotate = true;
        public int lastFrameCountForInertia = 5;
        public float maxAutoRotateSpeed = 0.5f;
        public float maxRotateSpeed = 20f;
        public float minAutoRotateSpeed = 0.03f;
        public float speedLerpRatio = 15f;

        private void AddRotateSpeedToList(float rotateSpeed)
        {
            if (this._lastSeveralFramesSpeed.Count < this.lastFrameCountForInertia)
            {
                this._lastSeveralFramesSpeed.Add(rotateSpeed);
            }
            else
            {
                this._lastSeveralFramesSpeed.Add(rotateSpeed);
                this._lastSeveralFramesSpeed.RemoveAt(0);
            }
        }

        private bool CheckBeginStopGradually(Vector2 pointerPosition)
        {
            if (this._isPointerDown || (this._currentRotateSpeed <= 0f))
            {
                return false;
            }
            if (!this._isRotateWhenDragOutPanel)
            {
                return this._isPointerInPanel;
            }
            return true;
        }

        private bool CheckNeedAutoRotate()
        {
            return this._inAutoRotate;
        }

        private bool CheckNeedStopGradually()
        {
            return (((this.enableManualRotate && !this._inAutoRotate) && this._inSlowDown) && (this._currentRotateSpeed > 0f));
        }

        private void DoAutoRotate()
        {
            if (this.CheckNeedAutoRotate())
            {
                float num = 0.016f * this.autoRotateSpeedLerpRatio;
                this._currentAutoRotateSpeed = Mathf.Lerp(this._currentAutoRotateSpeed, this.minAutoRotateSpeed, Mathf.Clamp01(num));
                Vector3 vector = Vector3.RotateTowards(this._avatarModel.forward, this._autoRotateTargetDirection, this._currentAutoRotateSpeed, 0f);
                this._avatarModel.forward = vector;
                this.TryToStopAutoRotate();
            }
        }

        private void DoStopGradually()
        {
            if (this.CheckNeedStopGradually())
            {
                float num = 0.016f * this.speedLerpRatio;
                this._currentRotateSpeed = Mathf.Lerp(this._currentRotateSpeed, 0f, Mathf.Clamp01(num));
                this.TryToStopSlowDown();
                if (this._inSlowDown)
                {
                    if (this._rotateRight)
                    {
                        this._avatarModel.Rotate(Vector3.up, -this._currentRotateSpeed);
                    }
                    else
                    {
                        this._avatarModel.Rotate(Vector3.up, this._currentRotateSpeed);
                    }
                }
            }
        }

        private void DrawAvatarRotatePanelGizmos()
        {
            RectTransform component = base.GetComponent<RectTransform>();
            Vector3[] fourCornersArray = new Vector3[4];
            component.GetWorldCorners(fourCornersArray);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(fourCornersArray[0], fourCornersArray[1]);
            Gizmos.DrawLine(fourCornersArray[1], fourCornersArray[2]);
            Gizmos.DrawLine(fourCornersArray[2], fourCornersArray[3]);
            Gizmos.DrawLine(fourCornersArray[3], fourCornersArray[0]);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(fourCornersArray[0], 0.05f);
            Gizmos.DrawSphere(fourCornersArray[1], 0.05f);
            Gizmos.DrawSphere(fourCornersArray[2], 0.05f);
            Gizmos.DrawSphere(fourCornersArray[3], 0.05f);
        }

        public void OnDrag(PointerEventData data)
        {
            if (((!this._inAutoRotate && this._isPointerDown) && this.enableManualRotate) && (this._isRotateWhenDragOutPanel || this._isPointerInPanel))
            {
                Vector2 delta = data.delta;
                float num = Vector2.Dot(Vector2.right, delta);
                float dpi = Screen.dpi;
                float f = num / dpi;
                this._rotateRight = num > 0f;
                this._currentRotateSpeed = Mathf.Abs(f) * this._rotateSpeedRatio;
                if (this._currentRotateSpeed > this.maxRotateSpeed)
                {
                    this._currentRotateSpeed = this.maxRotateSpeed;
                }
                this.AddRotateSpeedToList(this._currentRotateSpeed);
                if (this._rotateRight)
                {
                    this._avatarModel.Rotate(Vector3.up, -this._currentRotateSpeed);
                }
                else
                {
                    this._avatarModel.Rotate(Vector3.up, this._currentRotateSpeed);
                }
            }
        }

        private void OnDrawGizmos()
        {
        }

        public void OnPointerDown(PointerEventData data)
        {
            this._isPointerDown = true;
            this.ResetStatus();
        }

        public void OnPointerEnter(PointerEventData data)
        {
            this._isPointerInPanel = true;
        }

        public void OnPointerExit(PointerEventData data)
        {
            this._isPointerInPanel = false;
            if (!this._isRotateWhenDragOutPanel)
            {
                this.ResetStatus();
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            this._isPointerDown = false;
            this._currentRotateSpeed = this.SampleLastSeveralFrameRotateSpeed();
            if (this.CheckBeginStopGradually(data.position))
            {
                this._inSlowDown = true;
            }
        }

        private void ResetStatus()
        {
            this._currentRotateSpeed = 0f;
            this._inSlowDown = false;
            this._lastSeveralFramesSpeed.Clear();
        }

        private float SampleLastSeveralFrameRotateSpeed()
        {
            if (this._lastSeveralFramesSpeed.Count == 0)
            {
                return 0f;
            }
            float num = 0f;
            foreach (float num2 in this._lastSeveralFramesSpeed)
            {
                num += num2;
            }
            return (num / ((float) this._lastSeveralFramesSpeed.Count));
        }

        public void SetupView(AvatarDataItem avatarDataItem)
        {
            this._avatarDataItem = avatarDataItem;
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            if (sceneCanvas is MonoMainCanvas)
            {
                this._avatarModel = ((MonoMainCanvas) sceneCanvas).avatar3dModelContext.GetAvatarById(this._avatarDataItem.avatarID);
            }
            else
            {
                this._avatarModel = ((MonoTestUI) sceneCanvas).avatar3dModelContext.GetAvatarById(this._avatarDataItem.avatarID);
            }
        }

        public void StartAutoRotateModel(AvatarModelAutoRotateType rotateType, MiscData.PageInfoKey pageKey = 3, string tabName = "Default")
        {
            if (rotateType == AvatarModelAutoRotateType.RotateToFront)
            {
                Vector3 forward = GameObject.Find("MainCamera").transform.forward;
                this._autoRotateTargetDirection = new Vector3(-forward.x, 0f, -forward.z);
            }
            else if (rotateType == AvatarModelAutoRotateType.RotateToBack)
            {
                Vector3 vector2 = GameObject.Find("MainCamera").transform.forward;
                this._autoRotateTargetDirection = new Vector3(vector2.x, 0f, vector2.z);
            }
            else if (rotateType == AvatarModelAutoRotateType.RotateToOrigin)
            {
                ConfigAvatarShowInfo info = UIUtil.GetAvatarShowInfo(this._avatarDataItem, pageKey, tabName);
                Quaternion identity = Quaternion.identity;
                identity.eulerAngles = info.Avatar.EulerAngle;
                this._autoRotateTargetDirection = (Vector3) (identity * Vector3.forward);
            }
            this._currentAutoRotateSpeed = this.maxAutoRotateSpeed;
            this._inAutoRotate = true;
        }

        private void TryToStopAutoRotate()
        {
            if (this._inAutoRotate && (Mathf.Abs((float) (Vector3.Dot(this._avatarModel.forward, this._autoRotateTargetDirection) - 1f)) < 0.001f))
            {
                this._inAutoRotate = false;
            }
        }

        private void TryToStopSlowDown()
        {
            if (this._inSlowDown && (Mathf.Abs(this._currentRotateSpeed) < 0.001f))
            {
                this._inSlowDown = false;
            }
        }

        private void Update()
        {
            this.DoStopGradually();
            this.DoAutoRotate();
        }

        public enum AvatarModelAutoRotateType
        {
            RotateToBack,
            RotateToFront,
            RotateToOrigin
        }
    }
}

