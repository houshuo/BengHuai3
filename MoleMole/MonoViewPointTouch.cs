namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoViewPointTouch : MonoBehaviour
    {
        private Dictionary<int, CameraControllPointer> _cameraControlPointers = new Dictionary<int, CameraControllPointer>();
        private CameraControlType _currentControlType;
        private Vector2 _dragOffset = Vector2.zero;
        private CameraControlType _preControlType;
        private float _screenHeightInInch;
        private float _screenWidthInInch;
        private const float VIEWPOINT_DRAG_MOVEMENT_THRESHOLD = 0.06f;
        private const float ZOOMING_DRAG_MOVEMENT_THRESHOLD = 20f;
        private const float ZOOMING_SCALE_FACTOR = 4f;

        private CameraControlType GetCurrentTouchType()
        {
            switch (this._cameraControlPointers.Count)
            {
                case 1:
                    return CameraControlType.OnePoint;

                case 2:
                    return CameraControlType.TwoPoint;
            }
            return CameraControlType.None;
        }

        public void OnViewPointPanelDrag(BaseEventData baseData)
        {
            PointerEventData data = (PointerEventData) baseData;
            if (this._cameraControlPointers.ContainsKey(data.pointerId))
            {
                CameraControllPointer pointer = this._cameraControlPointers[data.pointerId];
                pointer.offsetPoint = data.delta;
                CameraControllPointer pointer2 = pointer;
                CameraControlType currentTouchType = this.GetCurrentTouchType();
                if (currentTouchType == CameraControlType.TwoPoint)
                {
                    foreach (KeyValuePair<int, CameraControllPointer> pair in this._cameraControlPointers)
                    {
                        if (pair.Key != data.pointerId)
                        {
                            pointer2 = pair.Value;
                        }
                    }
                    float num = Vector2.Distance(pointer.enterPoint, pointer2.enterPoint);
                    float num2 = Vector2.Distance(pointer.offsetPoint + pointer.enterPoint, pointer2.offsetPoint + pointer2.enterPoint);
                    float f = num - num2;
                    float zoomDelta = (f * Time.deltaTime) * 4f;
                    if (!pointer.isDrag && (Mathf.Abs(f) > 20f))
                    {
                        pointer.isDrag = true;
                        Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                    }
                    if (pointer.isDrag)
                    {
                        this._dragOffset = data.delta;
                        Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledZoomingData(zoomDelta);
                    }
                }
                else if ((currentTouchType == CameraControlType.OnePoint) && (this._preControlType == CameraControlType.None))
                {
                    if (!pointer.isDrag)
                    {
                        float num5 = ((data.position.x - pointer.enterPoint.x) / ((float) Screen.width)) * this._screenWidthInInch;
                        float num6 = ((data.position.y - pointer.enterPoint.y) / ((float) Screen.height)) * this._screenHeightInInch;
                        float num7 = (num5 * num5) + (num6 * num6);
                        if (num7 > 0.0036f)
                        {
                            pointer.isDrag = true;
                            Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                        }
                    }
                    if (pointer.isDrag)
                    {
                        Vector2 delta = data.delta;
                        delta.x = (delta.x / ((float) Screen.width)) * this._screenWidthInInch;
                        delta.y = (delta.y / ((float) Screen.height)) * this._screenHeightInInch;
                        Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(delta);
                    }
                }
                else if ((currentTouchType == CameraControlType.OnePoint) && (this._preControlType == CameraControlType.TwoPoint))
                {
                    Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(data.delta - this._dragOffset);
                }
            }
        }

        public void OnViewPointPanelInitializePotentialDrag(BaseEventData baseData)
        {
            PointerEventData data = (PointerEventData) baseData;
            data.useDragThreshold = false;
        }

        public void OnViewPointPanelPointerDown(BaseEventData baseData)
        {
            PointerEventData data = (PointerEventData) baseData;
            if (!this._cameraControlPointers.ContainsKey(data.pointerId))
            {
                CameraControllPointer pointer = new CameraControllPointer {
                    pointerId = data.pointerId,
                    isDrag = false,
                    enterPoint = data.position,
                    offsetPoint = data.delta
                };
                this._cameraControlPointers.Add(pointer.pointerId, pointer);
                this.RefreshControlType();
            }
        }

        public void OnViewPointPanelPointerUp(BaseEventData baseData)
        {
            PointerEventData data = (PointerEventData) baseData;
            if (this._cameraControlPointers.ContainsKey(data.pointerId))
            {
                CameraControllPointer pointer = this._cameraControlPointers[data.pointerId];
                if (this._currentControlType == CameraControlType.OnePoint)
                {
                    if (pointer.isDrag)
                    {
                        Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateEnableExitTimer();
                        if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDamageStasticsPlugin>() && Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDamageStasticsPlugin>().isUpdating)
                        {
                            Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDamageStasticsPlugin>().AddScreenRotateTimes();
                        }
                    }
                    else
                    {
                        if (Singleton<CameraManager>.Instance.controlledRotateKeepManual)
                        {
                            Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStop();
                        }
                        if (((Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning) && (Singleton<MonsterManager>.Instance.LivingMonsterCount() > 0)) && Singleton<AvatarManager>.Instance.GetLocalAvatar().IsAlive())
                        {
                            Singleton<AvatarManager>.Instance.GetLocalAvatar().SelectTarget();
                            Singleton<AvatarManager>.Instance.GetLocalAvatar().ClearAttackTargetTimed(1.2f);
                        }
                        else
                        {
                            Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
                        }
                    }
                }
                this._cameraControlPointers.Remove(data.pointerId);
                this.RefreshControlType();
                if (this._currentControlType == CameraControlType.None)
                {
                    this._dragOffset = Vector2.zero;
                }
            }
        }

        private void RefreshControlType()
        {
            this._preControlType = this._currentControlType;
            this._currentControlType = this.GetCurrentTouchType();
        }

        private void Start()
        {
            if (GraphicsSettingUtil._originScreenResolution.width > 0)
            {
                this._screenWidthInInch = ((float) GraphicsSettingUtil._originScreenResolution.width) / Screen.dpi;
                this._screenHeightInInch = ((float) GraphicsSettingUtil._originScreenResolution.height) / Screen.dpi;
            }
            else
            {
                this._screenWidthInInch = ((float) Screen.width) / Screen.dpi;
                this._screenHeightInInch = ((float) Screen.height) / Screen.dpi;
            }
        }

        private class CameraControllPointer
        {
            public Vector2 enterPoint;
            public bool isDrag;
            public Vector2 offsetPoint;
            public int pointerId;
        }

        private enum CameraControlType
        {
            None,
            OnePoint,
            TwoPoint
        }
    }
}

