namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [fiInspectorOnly]
    public sealed class MonoMainCamera : BaseMonoCamera
    {
        private Vector3 _calculatedShakeOffset;
        private List<CameraShakeEntry> _cameraShakeLs = new List<CameraShakeEntry>();
        private CameraExposureArgument _camExposureArg;
        private CameraExposureState _camExposureState;
        private LinkedList<CameraGlareArgument> _camGlareArgs = new LinkedList<CameraGlareArgument>();
        private bool _debugIsNear = true;
        private Vector3 _directedShakeOffset;
        private Quaternion _directionalLightFollowRotation;
        private AnimationCurve _dofAnimationCurve;
        private float _dofCustomDuration;
        private CameraDOFCustomState _dofCustomState = CameraDOFCustomState.Inactive;
        private float _dofCustomTimer;
        private bool _doingTransitionLerp;
        private Plane[] _frustumPlanes = new Plane[6];
        private bool _isAlongDirected;
        private CameraShakeEntry _largestShakeEntryThisFrame;
        private bool _muteManualControl;
        private bool _needUpdateDirectionalLight = true;
        private BaseMainCameraState _nextState;
        private float _shakeDirectedRatio;
        private int _shakeFrameCounter;
        private float _shakeRange;
        private int _shakeStepFrame;
        private float _shakeTimer;
        private float _shakeTotalTime;
        private BaseMainCameraState _state;
        private Vector3 _transitionFromForward;
        private float _transitionFromFov;
        private Vector3 _transitionFromPos;
        private float _transitionLerpSpeedRatio = 1f;
        private int _transitionLerpStep;
        private float _transitionLerpTimer;
        public Light directionalLight;
        private Color failColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private Color normalColor = new Color(0.5f, 0.5f, 0.5f, 0f);
        private const float TRANSITION_LERP_SPAN = 0.5f;

        public void ActExposureEffect(float exposureTime, float keepTime, float recoverTime, float maxRate)
        {
            this._camExposureArg.exposureTime = exposureTime;
            this._camExposureArg.keepTime = keepTime;
            this._camExposureArg.recoverTime = recoverTime;
            this._camExposureArg.maxExposureRate = Mathf.Max(this._camExposureArg.maxExposureRate, maxRate);
            this._camExposureArg.deltaExposureRate = (maxRate - this._camExposureArg.currentExposureRate) / this._camExposureArg.exposureTime;
            this._camExposureArg.deltaGlareThresRate = this._camExposureArg.currentGlareThresRate / this._camExposureArg.exposureTime;
            this._camExposureArg.timer = exposureTime;
            this._camExposureState = CameraExposureState.Exposure;
        }

        public void ActGlareEffect(float glareTime, float keepTime, float recoverTime, float targetRate)
        {
            CameraGlareArgument argument;
            argument = new CameraGlareArgument {
                glareTime = glareTime,
                keepTime = keepTime,
                recoverTime = recoverTime,
                targetRate = targetRate,
                deltaRate = 0f,
                timer = argument.glareTime,
                state = CameraGlareState.Glare
            };
            this._camGlareArgs.AddLast(argument);
        }

        public void ActShakeEffect(float time, float range, float angle, int stepFrame, bool isAngleDirected, bool clearPreviousShake)
        {
            if ((this._largestShakeEntryThisFrame == null) || (range > this._largestShakeEntryThisFrame.range))
            {
                CameraShakeEntry shakeEntry = new CameraShakeEntry {
                    timer = time,
                    duration = time,
                    range = range,
                    angle = angle,
                    stepFrame = stepFrame,
                    stepFrameCounter = 1,
                    isAngleDirected = isAngleDirected
                };
                int num = this._cameraShakeLs.SeekAddPosition<CameraShakeEntry>();
                this._cameraShakeLs[num] = shakeEntry;
                if ((this.SeekLargestShakeEntryIndex() == num) || clearPreviousShake)
                {
                    if (clearPreviousShake)
                    {
                        this._cameraShakeLs.Clear();
                    }
                    this.SetupShake(shakeEntry);
                }
                this._largestShakeEntryThisFrame = shakeEntry;
            }
        }

        public override void Awake()
        {
            base.Awake();
            this.cameraComponent = base.GetComponent<Camera>();
            this.directionalLight = base.transform.Find("DirLight").GetComponent<Light>();
            this._directionalLightFollowRotation = this.directionalLight.transform.rotation;
        }

        public static Vector3 CameraForwardLerp(Vector3 a, Vector3 b, float t)
        {
            a.Normalize();
            b.Normalize();
            Vector3 forward = a;
            a.y = 0f;
            Vector3 vector2 = b;
            b.y = 0f;
            Quaternion quaternion = Quaternion.LookRotation(forward);
            Quaternion quaternion2 = Quaternion.LookRotation(vector2);
            Quaternion quaternion3 = Quaternion.Slerp(quaternion, quaternion2, t);
            float num = Mathf.Asin(a.y);
            float num2 = Mathf.Asin(b.y);
            float y = Mathf.Lerp(num, num2, t);
            return (Vector3) ((Quaternion.Euler(0f, y, 0f) * quaternion3) * Vector3.forward);
        }

        public void ClearNextState()
        {
            this._doingTransitionLerp = false;
            this._nextState = null;
        }

        [Conditional("NG_HSOD_DEBUG"), Conditional("UNITY_EDITOR")]
        private void DebugUpdate()
        {
            if (Input.GetKeyUp(KeyCode.U))
            {
                if (this.followState.active)
                {
                    this.FollowLookAtPosition(Vector3.zero, false, false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Y))
            {
                if (this.followState.active)
                {
                    if (this._debugIsNear)
                    {
                        this.SetFollowRange(MainCameraFollowState.RangeState.Far, false);
                        this._debugIsNear = false;
                    }
                    else
                    {
                        this.SetFollowRange(MainCameraFollowState.RangeState.Near, false);
                        this._debugIsNear = true;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.I))
            {
                if (this.followState.active)
                {
                    this.SetTimedPullZ(0.8f, 0f, 0f, -10f, 1.5f, 1f, "BinlaoTest", false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.O))
            {
                if (this.followState.active)
                {
                    this.SuddenSwitchFollowAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.P))
            {
                Singleton<LevelDesignManager>.Instance.PlayCameraAnimationOnEnv("Ver1_12_1", false, true, false, CameraAnimationCullingType.CullNothing);
            }
            else if (Input.GetKeyUp(KeyCode.F2))
            {
                this.ActShakeEffect(0.1f, 0.5f, 90f, 3, true, false);
            }
            else if (Input.GetKeyUp(KeyCode.F3))
            {
                this.ActShakeEffect(10f, 0.2f, 0f, 3, true, false);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                this.SuddenRecover();
            }
            else if (Input.GetKeyUp(KeyCode.Z))
            {
                BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
                if ((currentPageContext != null) && (currentPageContext is InLevelMainPageContext))
                {
                    (currentPageContext as InLevelMainPageContext).SetInLevelMainPageActive(true, false, false);
                }
            }
            else if (Input.GetKeyUp(KeyCode.X))
            {
                BasePageContext context3 = Singleton<MainUIManager>.Instance.CurrentPageContext;
                if ((context3 != null) && (context3 is InLevelMainPageContext))
                {
                    (context3 as InLevelMainPageContext).SetInLevelMainPageActive(false, false, false);
                }
            }
        }

        public void FollowControlledRotateEnableExitTimer()
        {
            if ((this.followState.active && this.followState.followAvatarControlledRotate.active) && !this._muteManualControl)
            {
                this.followState.followAvatarControlledRotate.SetExitingControl(true);
            }
        }

        public void FollowControlledRotateStart()
        {
            if (this.followState.active && !this._muteManualControl)
            {
                if (this.followState.followAvatarControlledRotate.active)
                {
                    this.followState.followAvatarControlledRotate.SetExitingControl(false);
                }
                else
                {
                    this.followState.TransitBaseState(this.followState.followAvatarControlledRotate, false);
                }
                if (this.followState.rotateToAvatarFacingState.active)
                {
                    this.followState.TryRemoveShortState();
                }
            }
        }

        public void FollowControlledRotateStop()
        {
            if ((this.followState.active && this.followState.followAvatarControlledRotate.active) && !this._muteManualControl)
            {
                this.followState.TryToTransitToOtherBaseState(false, null);
            }
        }

        public void FollowLookAtPosition(Vector3 position, bool mute = false, bool force = false)
        {
            if (!force && this.followState.slowMotionKillState.active)
            {
                this.followState.slowMotionKillState.SetFollowingLookAtPosition(position, mute);
            }
            else
            {
                this.followState.TryRemoveShortState();
                this.followState.lookAtPositionState.SetLookAtTarget(position, mute);
                this.followState.AddShortState(this.followState.lookAtPositionState);
            }
        }

        public int GetCullingMaskByLayers(int[] layers)
        {
            int num = 0;
            foreach (int num2 in layers)
            {
                num |= ((int) 1) << num2;
            }
            return num;
        }

        public int GetVisibleMonstersCount()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            int num = 0;
            for (int i = 0; i < allMonsters.Count; i++)
            {
                if (this.IsEntityVisible(allMonsters[i]))
                {
                    num++;
                }
            }
            return num;
        }

        public int GetVisibleMonstersCountWithOffset(float fovOffset, float nearOffset, float farOffset)
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            int num = 0;
            for (int i = 0; i < allMonsters.Count; i++)
            {
                if (this.IsEntityVisibleInCustomOffset(allMonsters[i], fovOffset, nearOffset, farOffset))
                {
                    num++;
                }
            }
            return num;
        }

        public void Init(uint runtimeID)
        {
            base.Init(1, runtimeID);
            this.originalNearClip = this.cameraComponent.nearClipPlane;
            this.originalFOV = this.cameraComponent.fieldOfView;
            this.staticState = new MainCameraStaticState(this);
            this.followState = new MainCameraFollowState(this);
            this.levelAnimState = new MainCameraLevelAnimState(this);
            this.avatarAnimState = new MainCameraAvatarAnimState(this);
            this.cinemaState = new MainCameraCinemaState(this);
            this.storyState = new MainCameraStoryState(this);
            this._state = this.staticState;
            this._state.Enter();
            this._camExposureState = CameraExposureState.Idle;
            CameraExposureArgument argument = new CameraExposureArgument {
                timer = 0f,
                exposureTime = 0f,
                keepTime = 0f,
                recoverTime = 0f,
                maxExposureRate = 1f,
                currentExposureRate = 1f,
                originalExposure = base.GetComponent<PostFXBase>().Exposure,
                deltaExposureRate = 0f,
                currentGlareThresRate = 1f,
                originalGlareThres = base.GetComponent<PostFXBase>().glareThreshold,
                deltaGlareThresRate = 0f
            };
            this._camExposureArg = argument;
            CameraGlareArgument.originalValue = base.GetComponent<PostFXBase>().glareIntensity;
            Singleton<EventManager>.Instance.CreateActor<MainCameraActor>(this);
        }

        public void InterruptTransitionLerp(BaseMainCameraState nextState)
        {
            this._doingTransitionLerp = false;
            this._nextState = null;
            this.Transit(nextState);
        }

        public bool IsEntityVisible(BaseMonoEntity entity)
        {
            Collider component = entity.GetComponent<Collider>();
            if ((component != null) && component.enabled)
            {
                return GeometryUtility.TestPlanesAABB(this._frustumPlanes, component.bounds);
            }
            Vector3 vector = this.cameraComponent.WorldToViewportPoint(entity.XZPosition);
            return (((vector.z > 0f) && Miscs.IsFloatInRange(vector.x, 0f, 1f)) && Miscs.IsFloatInRange(vector.y, 0f, 1f));
        }

        public bool IsEntityVisibleInCustomOffset(BaseMonoEntity entity, float fovOffset, float nearOffset, float farOffset)
        {
            bool flag = false;
            float fieldOfView = this.cameraComponent.fieldOfView;
            float nearClipPlane = this.cameraComponent.nearClipPlane;
            float farClipPlane = this.cameraComponent.farClipPlane;
            Camera cameraComponent = this.cameraComponent;
            cameraComponent.fieldOfView += fovOffset;
            Camera camera2 = this.cameraComponent;
            camera2.nearClipPlane += nearOffset;
            Camera camera3 = this.cameraComponent;
            camera3.farClipPlane += farOffset;
            Matrix4x4 projectionMatrix = this.cameraComponent.projectionMatrix;
            Collider component = entity.GetComponent<Collider>();
            if ((component != null) && component.enabled)
            {
                flag = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(projectionMatrix), component.bounds);
            }
            Vector3 vector = this.cameraComponent.WorldToViewportPoint(entity.XZPosition);
            flag = ((vector.z > 0f) && Miscs.IsFloatInRange(vector.x, 0f, 1f)) && Miscs.IsFloatInRange(vector.y, 0f, 1f);
            this.cameraComponent.fieldOfView = fieldOfView;
            this.cameraComponent.nearClipPlane = nearClipPlane;
            this.cameraComponent.farClipPlane = farClipPlane;
            return flag;
        }

        public bool IsInTransitionLerp()
        {
            return this._doingTransitionLerp;
        }

        private void LateUpdate()
        {
            if (this._nextState != null)
            {
                if (this._doingTransitionLerp)
                {
                    if (this._transitionLerpStep == 0)
                    {
                        this._transitionFromPos = this._state.cameraPosition;
                        this._transitionFromForward = this._state.cameraForward;
                        this._transitionFromFov = this._state.cameraFOV;
                        this._state.Exit();
                        this._state.SetActive(false);
                        this._state = this._nextState;
                        this._state.Enter();
                        this._state.SetActive(true);
                        this._state.Update();
                        this._transitionLerpTimer += Time.unscaledDeltaTime * this._transitionLerpSpeedRatio;
                        float t = this._transitionLerpTimer / 0.5f;
                        t *= 2f - t;
                        base.transform.position = Vector3.Lerp(this._transitionFromPos, this._state.cameraPosition, t);
                        base.transform.forward = CameraForwardLerp(this._transitionFromForward, this._state.cameraForward, (Time.deltaTime * 5f) * this._transitionLerpSpeedRatio);
                        this.cameraComponent.fieldOfView = Mathf.Lerp(this._transitionFromFov, this._state.cameraFOV, t);
                        this._transitionLerpStep = 1;
                    }
                    else if (this._transitionLerpStep == 1)
                    {
                        this._state.Update();
                        this._transitionLerpTimer += Time.unscaledDeltaTime * this._transitionLerpSpeedRatio;
                        float num2 = this._transitionLerpTimer / 0.5f;
                        if (num2 > 1f)
                        {
                            base.transform.position = this._state.cameraPosition;
                            base.transform.forward = this._state.cameraForward;
                            this.cameraComponent.fieldOfView = this._state.cameraFOV;
                            this._nextState = null;
                            this._doingTransitionLerp = false;
                        }
                        else
                        {
                            num2 *= 2f - num2;
                            base.transform.position = Vector3.Lerp(this._transitionFromPos, this._state.cameraPosition, num2);
                            base.transform.forward = CameraForwardLerp(this._transitionFromForward, this._state.cameraForward, num2);
                            this.cameraComponent.fieldOfView = Mathf.Lerp(this._transitionFromFov, this._state.cameraFOV, num2);
                        }
                    }
                }
                else
                {
                    this._state.Exit();
                    this._state.SetActive(false);
                    this._state = this._nextState;
                    this._state.Enter();
                    this._state.SetActive(true);
                    this._nextState = null;
                    this._state.Update();
                    base._cameraTrans.position = this._state.cameraPosition;
                    base._cameraTrans.forward = this._state.cameraForward;
                    this.cameraComponent.fieldOfView = this._state.cameraFOV;
                }
            }
            else
            {
                this._state.Update();
                base._cameraTrans.position = this._state.cameraPosition;
                base._cameraTrans.forward = this._state.cameraForward;
                this.cameraComponent.fieldOfView = this._state.cameraFOV;
            }
            float num3 = !this._state.lerpDirectionalLight ? 8f : 1f;
            if (this._needUpdateDirectionalLight)
            {
                this._directionalLightFollowRotation = Quaternion.Slerp(this._directionalLightFollowRotation, base._cameraTrans.rotation, Time.deltaTime * num3);
                this.directionalLight.transform.rotation = Quaternion.Euler(45f, this._directionalLightFollowRotation.eulerAngles.y, this._directionalLightFollowRotation.eulerAngles.z);
            }
            this.UpdateCameraShake();
            this.UpdateCameraExposure();
            this.UpdateCameraGlare();
            this.UpdateCameraDofCustom();
            GeometryUtilityUser.CalculateFrustumPlanes(this.cameraComponent, ref this._frustumPlanes);
        }

        private static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float num = (2f * near) / (right - left);
            float num2 = (2f * near) / (top - bottom);
            float num3 = (right + left) / (right - left);
            float num4 = (top + bottom) / (top - bottom);
            float num5 = -(far + near) / (far - near);
            float num6 = -((2f * far) * near) / (far - near);
            float num7 = -1f;
            Matrix4x4 matrixx = new Matrix4x4();
            matrixx[0, 0] = num;
            matrixx[0, 1] = 0f;
            matrixx[0, 2] = num3;
            matrixx[0, 3] = 0f;
            matrixx[1, 0] = 0f;
            matrixx[1, 1] = num2;
            matrixx[1, 2] = num4;
            matrixx[1, 3] = 0f;
            matrixx[2, 0] = 0f;
            matrixx[2, 1] = 0f;
            matrixx[2, 2] = num5;
            matrixx[2, 3] = num6;
            matrixx[3, 0] = 0f;
            matrixx[3, 1] = 0f;
            matrixx[3, 2] = num7;
            matrixx[3, 3] = 0f;
            return matrixx;
        }

        public void PlayAvatarCameraAnimationThenStay(Animation anim, BaseMonoAvatar avatar)
        {
            this.avatarAnimState.SetupFollowAvatarAnim(anim, avatar);
            if (this.IsInTransitionLerp())
            {
                this.InterruptTransitionLerp(this.avatarAnimState);
            }
            else
            {
                this.Transit(this.avatarAnimState);
            }
        }

        public void PlayAvatarCameraAnimationThenTransitToFollow(Animation anim, BaseMonoAvatar avatar, MainCameraFollowState.EnterPolarMode enterPolarMode, bool exitTransitionLerp)
        {
            float polar = 0f;
            if (enterPolarMode == MainCameraFollowState.EnterPolarMode.AlongTargetPolar)
            {
                polar = this.followState.anchorPolar;
            }
            this.followState.SetEnterPolarMode(enterPolarMode, polar);
            this.avatarAnimState.SetupFollowAvatarAnim(anim, avatar);
            this.avatarAnimState.SetNextState(this.followState, exitTransitionLerp);
            if (this.IsInTransitionLerp())
            {
                this.InterruptTransitionLerp(this.avatarAnimState);
            }
            else
            {
                this.Transit(this.avatarAnimState);
            }
        }

        public void PlayLevelAnimationThenTransitBack(Animation anim, bool ignoreTimeScale, bool enterLerp, bool exitLerp, bool pauseLevel, CameraAnimationCullingType cullType = 0, Action startCallback = null, Action endCallback = null)
        {
            this.levelAnimState.SetupFollowAnim(anim, ignoreTimeScale, pauseLevel, cullType, startCallback, endCallback);
            this.levelAnimState.SetNextState((this._state != this.staticState) ? this._state : this.followState, exitLerp);
            if (this._state == this.avatarAnimState)
            {
                this.avatarAnimState.SetInterrupt();
            }
            if (enterLerp)
            {
                this.TransitWithLerp(this.levelAnimState, 0.1f);
            }
            else
            {
                this.Transit(this.levelAnimState);
            }
        }

        public void PlayStoryCameraState(int plotID, bool enterLerp = true, bool exitLerp = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
        {
            if (this._state != this.storyState)
            {
                bool flag = backFollow;
                this.storyState.SetCurrentPlotSetting(plotID, exitLerp, needFadeIn, flag, pauseLevel);
                if (enterLerp)
                {
                    this.TransitWithLerp(this.storyState, 0.5f);
                }
                else
                {
                    this.Transit(this.storyState);
                }
            }
        }

        private int SeekLargestShakeEntryIndex()
        {
            int num = -1;
            float num2 = 0f;
            for (int i = 0; i < this._cameraShakeLs.Count; i++)
            {
                if (this._cameraShakeLs[i] != null)
                {
                    CameraShakeEntry entry = this._cameraShakeLs[i];
                    if ((entry.range * (entry.timer / entry.duration)) > num2)
                    {
                        num = i;
                    }
                }
            }
            return num;
        }

        public void SetCameraFakeDOFCustommed(AnimationCurve cureve, float duration)
        {
            this._dofAnimationCurve = cureve;
            this._dofCustomTimer = 0f;
            this._dofCustomDuration = duration;
            this._dofCustomState = CameraDOFCustomState.Active;
        }

        public void SetCameraLocateRatio(float cameraLocateRatio)
        {
            this.followState.cameraLocateRatio = cameraLocateRatio;
        }

        public void SetFailPostFX(bool enabled)
        {
            PostFXBase component = base.GetComponent<PostFXBase>();
            if (component != null)
            {
                if (enabled)
                {
                    component.enabled = true;
                }
                else
                {
                    component.enabled = component.originalEnabled;
                }
                component.SepiaColor = !enabled ? this.normalColor : this.failColor;
            }
        }

        public void SetFollowAnchorRadius(float anchorRadius)
        {
            float num = Mathf.Clamp(anchorRadius, 6f, 8.5f);
            this.followState.anchorRadius = num;
        }

        public void SetFollowControledRotationData(Vector2 delta)
        {
            if ((this.followState.active && this.followState.followAvatarControlledRotate.active) && !this._muteManualControl)
            {
                this.followState.followAvatarControlledRotate.SetDragDelta(delta);
            }
        }

        public void SetFollowControledZoomingData(float zoomDelta)
        {
            if ((this.followState.active && this.followState.followAvatarControlledRotate.active) && !this._muteManualControl)
            {
                this.followState.followAvatarControlledRotate.SetZoomDelta(zoomDelta);
            }
        }

        public void SetFollowRange(MainCameraFollowState.RangeState rangeState, bool force = false)
        {
            if (force || !this.followState.slowMotionKillState.active)
            {
                this.followState.TryRemoveShortState();
                this.followState.rangeTransitState.SetRange(rangeState);
                this.followState.AddShortState(this.followState.rangeTransitState);
            }
        }

        public void SetMainCameraCullingMask(int mask)
        {
            this.cameraComponent.cullingMask = mask;
        }

        public void SetMuteManualCameraControl(bool mute)
        {
            this._muteManualControl = mute;
            if (mute)
            {
                if (this.followState.rotateToAvatarFacingState.active)
                {
                    this.followState.TryRemoveShortState();
                }
                if (this.followState.followAvatarControlledRotate.active)
                {
                    this.followState.TransitBaseState(this.followState.followAvatarState, false);
                }
            }
        }

        public void SetNeedLerpDirectionalLight(bool needLerp)
        {
            this._needUpdateDirectionalLight = needLerp;
        }

        public void SetRotateToFaceDirection()
        {
            if ((this.followState.active && this.followState.rotateToAvatarFacingState.CanRotate()) && !this._muteManualControl)
            {
                this.followState.AddOrReplaceShortState(this.followState.rotateToAvatarFacingState);
            }
        }

        public void SetSlowMotionKill(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
        {
            if (this.followState.slowMotionKillState.active)
            {
                this.followState.slowMotionKillState.SetSlowMotionKill(config, distTarget, distCamera);
            }
            else
            {
                this.followState.TryRemoveShortState();
                this.followState.slowMotionKillState.SetSlowMotionKill(config, distTarget, distCamera);
                this.followState.AddShortState(this.followState.slowMotionKillState);
            }
        }

        public void SetTimedPullZ(float radiusRatio, float elevationAngles, float centerYOffset, float fovOffset, float time, float lerpTimer = 0, string lerpCurveName = "", bool force = false)
        {
            if (force || !this.followState.slowMotionKillState.active)
            {
                this.followState.TryRemoveShortState();
                this.followState.timedPullZState.SetTimedPullZ(radiusRatio, elevationAngles, centerYOffset, fovOffset, time, lerpTimer, lerpCurveName);
                this.followState.AddShortState(this.followState.timedPullZState);
            }
        }

        public void SetupFollowAvatar(uint avatarID)
        {
            this.followState.SetupFollowAvatar(avatarID);
        }

        private void SetupShake(CameraShakeEntry shakeEntry)
        {
            this._shakeTimer = shakeEntry.timer;
            this._shakeTotalTime = shakeEntry.duration;
            this._shakeStepFrame = shakeEntry.stepFrame;
            this._shakeFrameCounter = shakeEntry.stepFrameCounter;
            this._shakeRange = shakeEntry.range * this._state.cameraShakeRatio;
            this._shakeDirectedRatio = !shakeEntry.isAngleDirected ? 0f : 0.8f;
            if (shakeEntry.isAngleDirected)
            {
                this._directedShakeOffset = (Vector3) (Quaternion.AngleAxis(shakeEntry.angle, Vector3.forward) * Vector3.right);
                this._directedShakeOffset = (Vector3) (this._directedShakeOffset * (shakeEntry.range * this._shakeDirectedRatio));
                this._isAlongDirected = true;
            }
            else
            {
                this._directedShakeOffset = Vector3.zero;
                this._isAlongDirected = false;
            }
        }

        public void SetUserDefinedCameraLocateRatio(float cameraLocateRatio)
        {
            this.followState.isCameraLocateRatioUserDefined = true;
            this.followState.cameraLocateRatio = cameraLocateRatio;
        }

        public void SuddenRecover()
        {
            if (this.followState.active)
            {
                this.followState.TryRemoveShortState();
                this.followState.AddShortState(this.followState.suddenRecoverState);
            }
        }

        public void SuddenSwitchFollowAvatar(uint avatarID, bool force = false)
        {
            if (force || !this.followState.slowMotionKillState.active)
            {
                this.followState.TryRemoveShortState();
                this.followState.suddenChangeState.SetSuddenChangeTarget(Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(avatarID));
                this.followState.AddShortState(this.followState.suddenChangeState);
            }
        }

        public void Transit(BaseMainCameraState to)
        {
            if (this._nextState != null)
            {
                this.ClearNextState();
            }
            if (this._nextState != null)
            {
            }
            if (this.IsInTransitionLerp())
            {
                this.InterruptTransitionLerp(this._nextState);
            }
            else
            {
                this._nextState = to;
            }
        }

        public void TransitToCinema(ICinema cinema)
        {
            this.cinemaState.SetCinema(cinema);
            this.Transit(this.cinemaState);
        }

        public void TransitToFollow()
        {
            this.Transit(this.followState);
        }

        public void TransitToStatic()
        {
            this.Transit(this.staticState);
        }

        public void TransitToStory()
        {
            this.Transit(this.storyState);
        }

        public void TransitWithLerp(BaseMainCameraState to, float transitionLerpSpeedRatio = 1)
        {
            if (this.IsInTransitionLerp())
            {
                this.InterruptTransitionLerp(this._nextState);
            }
            else
            {
                this._nextState = to;
                this._transitionLerpStep = 0;
                this._transitionLerpTimer = 0f;
                this._transitionLerpSpeedRatio = transitionLerpSpeedRatio;
                this._doingTransitionLerp = true;
            }
        }

        private void UpdateCameraDofCustom()
        {
            FakeDOF component = base.GetComponent<FakeDOF>();
            if (component != null)
            {
                if (this._dofCustomState != CameraDOFCustomState.Active)
                {
                    if (component.backgroundBlurFactor < 0.01f)
                    {
                        component.enabled = false;
                    }
                }
                else
                {
                    component.enabled = true;
                    if (((this._dofAnimationCurve != null) && (this._dofCustomDuration > 0f)) && ((this._dofCustomDuration > 0f) && (this._dofAnimationCurve != null)))
                    {
                        this._dofCustomTimer += Time.deltaTime;
                        float time = this._dofCustomTimer / this._dofCustomDuration;
                        float num2 = this._dofAnimationCurve.Evaluate(time);
                        component.backgroundBlurFactor = num2 * 5f;
                        if (time > 1f)
                        {
                            this._dofCustomState = CameraDOFCustomState.Done;
                        }
                    }
                }
            }
        }

        private void UpdateCameraExposure()
        {
            float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            if ((timeScale != 0f) && (this._camExposureState != CameraExposureState.Idle))
            {
                if (this._camExposureState == CameraExposureState.Exposure)
                {
                    this._camExposureArg.timer -= Time.deltaTime * timeScale;
                    this._camExposureArg.currentExposureRate += (this._camExposureArg.deltaExposureRate * Time.deltaTime) * timeScale;
                    this._camExposureArg.currentGlareThresRate -= (this._camExposureArg.deltaGlareThresRate * Time.deltaTime) * timeScale;
                    if (this._camExposureArg.timer <= 0f)
                    {
                        this._camExposureArg.timer = this._camExposureArg.keepTime;
                        this._camExposureState = CameraExposureState.Keep;
                    }
                }
                else if (this._camExposureState == CameraExposureState.Keep)
                {
                    this._camExposureArg.timer -= Time.deltaTime * timeScale;
                    if (this._camExposureArg.timer <= 0f)
                    {
                        this._camExposureArg.timer = this._camExposureArg.recoverTime;
                        this._camExposureArg.deltaExposureRate = (this._camExposureArg.maxExposureRate - 1f) / this._camExposureArg.recoverTime;
                        this._camExposureArg.deltaGlareThresRate = 1f / this._camExposureArg.recoverTime;
                        this._camExposureState = CameraExposureState.Recover;
                    }
                }
                else
                {
                    this._camExposureArg.timer -= Time.deltaTime * timeScale;
                    this._camExposureArg.currentExposureRate -= (this._camExposureArg.deltaExposureRate * Time.deltaTime) * timeScale;
                    this._camExposureArg.currentGlareThresRate += (this._camExposureArg.deltaGlareThresRate * Time.deltaTime) * timeScale;
                    if (this._camExposureArg.timer <= 0f)
                    {
                        this._camExposureArg.currentExposureRate = 1f;
                        this._camExposureState = CameraExposureState.Idle;
                    }
                }
                this._camExposureArg.currentExposureRate = Mathf.Clamp(this._camExposureArg.currentExposureRate, 1f, this._camExposureArg.maxExposureRate);
                this._camExposureArg.currentGlareThresRate = Mathf.Clamp(this._camExposureArg.currentGlareThresRate, 0f, 1f);
                base.GetComponent<PostFXBase>().Exposure = this._camExposureArg.currentExposureRate * this._camExposureArg.originalExposure;
                base.GetComponent<PostFXBase>().glareThreshold = this._camExposureArg.currentGlareThresRate * this._camExposureArg.originalGlareThres;
            }
        }

        private void UpdateCameraGlare()
        {
            float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            if (timeScale != 0f)
            {
                float num2 = 1f;
                LinkedListNode<CameraGlareArgument> node = null;
                LinkedListNode<CameraGlareArgument> first = this._camGlareArgs.First;
                while (first != null)
                {
                    node = first;
                    first = node.Next;
                    CameraGlareArgument argument = node.Value;
                    if (argument.state == CameraGlareState.Idle)
                    {
                        this._camGlareArgs.Remove(argument);
                    }
                    else
                    {
                        if (argument.state == CameraGlareState.Glare)
                        {
                            argument.timer -= Time.deltaTime * timeScale;
                            argument.deltaRate = (1f - (argument.timer / argument.glareTime)) * (argument.targetRate - 1f);
                            if (argument.timer <= 0f)
                            {
                                argument.deltaRate = argument.targetRate - 1f;
                                argument.timer = argument.keepTime;
                                argument.state = CameraGlareState.Keep;
                            }
                        }
                        else if (argument.state == CameraGlareState.Keep)
                        {
                            argument.timer -= Time.deltaTime * timeScale;
                            if (argument.timer <= 0f)
                            {
                                argument.timer = argument.recoverTime;
                                argument.state = CameraGlareState.Recover;
                            }
                        }
                        else
                        {
                            argument.timer -= Time.deltaTime * timeScale;
                            argument.deltaRate = (argument.timer / argument.recoverTime) * (argument.targetRate - 1f);
                            if (argument.timer <= 0f)
                            {
                                argument.deltaRate = 0f;
                                argument.state = CameraGlareState.Idle;
                            }
                        }
                        num2 += argument.deltaRate;
                    }
                }
                base.GetComponent<PostFXBase>().glareIntensity = num2 * CameraGlareArgument.originalValue;
            }
        }

        private void UpdateCameraShake()
        {
            float timeScale = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            if (timeScale != 0f)
            {
                if (this._shakeTimer > 0f)
                {
                    this._shakeFrameCounter--;
                    if (this._shakeFrameCounter == 0)
                    {
                        this._shakeFrameCounter = this._shakeStepFrame;
                        Vector3 vector = (Vector3) ((UnityEngine.Random.insideUnitCircle.normalized * this._shakeRange) * (1f - this._shakeDirectedRatio));
                        Vector3 vector2 = this._directedShakeOffset;
                        if (!this._isAlongDirected)
                        {
                            vector2 = (Vector3) (vector2 * 0.7f);
                        }
                        Vector3 direction = this._directedShakeOffset + vector;
                        float num2 = Mathf.Clamp((float) (this._shakeTimer / this._shakeTotalTime), (float) 0.2f, (float) 0.9f);
                        direction = (Vector3) (direction * (num2 * num2));
                        Vector3 vector4 = base.transform.TransformDirection(direction);
                        this._calculatedShakeOffset = vector4;
                        this._directedShakeOffset = (Vector3) (this._directedShakeOffset * -1f);
                        this._isAlongDirected = !this._isAlongDirected;
                    }
                    if (!this._state.muteCameraShake)
                    {
                        base.transform.position = this._calculatedShakeOffset + base.transform.position;
                    }
                }
                this._shakeTimer -= Time.unscaledDeltaTime;
                bool flag = false;
                for (int i = 0; i < this._cameraShakeLs.Count; i++)
                {
                    if (this._cameraShakeLs[i] != null)
                    {
                        CameraShakeEntry entry = this._cameraShakeLs[i];
                        entry.timer -= Time.deltaTime * timeScale;
                        if (entry.timer <= 0f)
                        {
                            this._cameraShakeLs[i] = null;
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    int num4 = this.SeekLargestShakeEntryIndex();
                    if (num4 >= 0)
                    {
                        this.SetupShake(this._cameraShakeLs[num4]);
                    }
                }
                this._largestShakeEntryThisFrame = null;
            }
        }

        public Vector3 WorldToUIPoint(Vector3 pos)
        {
            Vector3 position = this.cameraComponent.WorldToScreenPoint(pos);
            Camera component = Singleton<CameraManager>.Instance.GetInLevelUICamera().GetComponent<Camera>();
            position.z = Mathf.Clamp(position.z, component.nearClipPlane, component.farClipPlane);
            return component.ScreenToWorldPoint(position);
        }

        public MainCameraAvatarAnimState avatarAnimState { get; private set; }

        public Camera cameraComponent { get; private set; }

        public MainCameraCinemaState cinemaState { get; private set; }

        public MainCameraFollowState followState { get; private set; }

        public MainCameraLevelAnimState levelAnimState { get; private set; }

        public float originalFOV { get; private set; }

        public float originalNearClip { get; private set; }

        public MainCameraStaticState staticState { get; private set; }

        public MainCameraStoryState storyState { get; private set; }

        private enum CameraDOFCustomState
        {
            Active,
            Inactive,
            Done
        }

        private class CameraExposureArgument
        {
            public float currentExposureRate;
            public float currentGlareThresRate;
            public float deltaExposureRate;
            public float deltaGlareThresRate;
            public float exposureTime;
            public float keepTime;
            public float maxExposureRate;
            public float originalExposure;
            public float originalGlareThres;
            public float recoverTime;
            public float timer;
        }

        private enum CameraExposureState
        {
            Idle,
            Exposure,
            Keep,
            Recover
        }

        private class CameraGlareArgument
        {
            public float deltaRate;
            public float glareTime;
            public float keepTime;
            public static float originalValue;
            public float recoverTime;
            public MonoMainCamera.CameraGlareState state;
            public float targetRate;
            public float timer;
        }

        private enum CameraGlareState
        {
            Idle,
            Glare,
            Keep,
            Recover
        }

        private class CameraShakeEntry
        {
            public float angle;
            public float duration;
            public bool isAngleDirected;
            public float range;
            public int stepFrame;
            public int stepFrameCounter;
            public float timer;

            public override string ToString()
            {
                object[] args = new object[] { this.duration, this.range, Time.frameCount, this.timer };
                return string.Format("duration:{0} range:{1} frame:{2}, timer:{3}", args);
            }
        }
    }
}

