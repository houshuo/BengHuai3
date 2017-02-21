namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MainCameraStoryState : BaseMainCameraState
    {
        private bool _backFollowState;
        private Quaternion _baseAttitude;
        private Vector3 _baseGravity;
        private int _currentPlotID;
        private bool _exitTransitionLerp;
        private Gyroscope _gyro;
        private BaseMainCameraState _nextState;
        private bool _pauseLevel;
        private bool _quitWithFadeIn;
        private uint _storyScreenID;
        [CompilerGenerated]
        private static Predicate<BaseMonoAvatar> <>f__am$cache1C;
        public float anchorRadius;
        public BaseMonoAvatar avatar;
        private bool blurDataLoaded;
        public AnimationCurve blurEnterCurve;
        public float blurEnterDuration;
        public AnimationCurve blurExitCurve;
        public float blurExitDuration;
        public const float DEFAULT_FADE_DURATION = 0.5f;
        public const string DEFAULT_RENDER_TEXTURE_PATH = "Rendering/Texture/TestRenderTexture";
        public float fov;
        private int originalCullingMask;
        public float ParallexBoundHardness;
        public float ParallexRange;
        public float ParallexSensitivity;
        public float pitch;
        public float pitchOffset;
        public float xOffset;
        public float yaw;
        public float yawOffset;
        public float yOffset;

        public MainCameraStoryState(MonoMainCamera camera) : base(camera)
        {
            this._backFollowState = true;
            this._quitWithFadeIn = true;
            this.anchorRadius = 6f;
            this.yOffset = 1f;
            this.fov = 60f;
            this.ParallexRange = 5f;
            this.ParallexSensitivity = 0.1f;
            this.ParallexBoundHardness = 0.5f;
            this.LoadBlurData();
        }

        private void ClearStoryScreen()
        {
            this.avatar.SetLocomotionBool("IsStoryMode", false, false);
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true, false, false);
        }

        private void DoExit(float exitDuration = 0.5f)
        {
            this.SetNextState(base._owner.followState, true);
            if (this._exitTransitionLerp)
            {
                base._owner.TransitWithLerp(this._nextState, 1f);
            }
            else
            {
                base._owner.Transit(this._nextState);
            }
        }

        public override void Enter()
        {
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false, false, false);
            this.avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            this.SetOtherAvatarsVisibility(false);
            this.avatar.PlayState("StandBy");
            this.avatar.SetLocomotionBool("IsStoryMode", true, false);
            this.anchorRadius = this.avatar.config.StoryCameraSetting.anchorRadius;
            this.yaw = this.avatar.config.StoryCameraSetting.yaw;
            this.pitch = this.avatar.config.StoryCameraSetting.pitch;
            this.yOffset = this.avatar.config.StoryCameraSetting.yOffset;
            this.xOffset = this.avatar.config.StoryCameraSetting.xOffset;
            this.fov = this.avatar.config.StoryCameraSetting.fov;
            Vector3 axis = Vector3.Cross(this.avatar.FaceDirection, Vector3.up);
            base.cameraForward = (Vector3) (Quaternion.AngleAxis(this.yaw, Vector3.up) * this.avatar.FaceDirection);
            base.cameraForward = (Vector3) (Quaternion.AngleAxis(this.pitch, axis) * base.cameraForward);
            base.cameraPosition = (Vector3) (((this.avatar.XZPosition - (base.cameraForward * this.anchorRadius)) + (Vector3.up * this.yOffset)) + (axis * this.xOffset));
            base.cameraFOV = this.fov;
            this._storyScreenID = Singleton<DynamicObjectManager>.Instance.CreateStoryScreen(this.avatar.GetRuntimeID(), "StoryScreen", this.avatar.XZPosition + this.avatar.FaceDirection, this.avatar.FaceDirection, this._currentPlotID);
            this.avatar.SetLocomotionBool("IsStoryMode", true, false);
            Singleton<LevelDesignManager>.Instance.SetMuteAvatarVoice(true);
            MonoStoryScreen dynamicObjectByRuntimeID = (MonoStoryScreen) Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(this._storyScreenID);
            if (dynamicObjectByRuntimeID != null)
            {
                dynamicObjectByRuntimeID.onOpenAnimationChange = (Action<bool>) Delegate.Combine(dynamicObjectByRuntimeID.onOpenAnimationChange, new Action<bool>(this.OnOpenAnimationChange));
            }
            if (this.blurDataLoaded)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetCameraFakeDOFCustommed(this.blurEnterCurve, this.blurEnterDuration);
            }
            this._gyro = Input.gyro;
            this._gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
            this.SetAllOtherDynamicObjectsAndEfffectsVisibility(false);
            this._baseAttitude = this._gyro.attitude;
            this._baseGravity = this._gyro.gravity;
            if (this._pauseLevel)
            {
                Singleton<LevelManager>.Instance.SetPause(true);
            }
        }

        public override void Exit()
        {
            this.avatar.SetLocomotionBool("IsStoryMode", false, false);
            this.SetOtherAvatarsVisibility(true);
            this.SetAllOtherDynamicObjectsAndEfffectsVisibility(true);
            if (this.blurDataLoaded)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetCameraFakeDOFCustommed(this.blurExitCurve, this.blurExitDuration);
            }
            if (Singleton<LevelManager>.Instance.IsPaused())
            {
                Singleton<LevelManager>.Instance.SetPause(false);
            }
        }

        private float GetFixedEulerAngle(float angle)
        {
            float num = angle;
            if (num > 360f)
            {
                num -= 360f;
            }
            if (num < 0f)
            {
                num += 360f;
            }
            return ((num <= 180f) ? num : (num - 360f));
        }

        private void LoadBlurData()
        {
            string filePath = "FakeDOF/DOFEnterPlot";
            ConfigCameraFakeDOF edof = ConfigUtil.LoadConfig<ConfigCameraFakeDOF>(filePath);
            this.blurEnterCurve = edof.Curve;
            this.blurEnterDuration = edof.Duration;
            string str2 = "FakeDOF/DOFExitPlot";
            ConfigCameraFakeDOF edof2 = ConfigUtil.LoadConfig<ConfigCameraFakeDOF>(str2);
            this.blurExitCurve = edof2.Curve;
            this.blurExitDuration = edof2.Duration;
            this.blurDataLoaded = true;
        }

        private void OnOpenAnimationChange(bool openState)
        {
            if (!openState)
            {
                MonoStoryScreen dynamicObjectByRuntimeID = (MonoStoryScreen) Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(this._storyScreenID);
                if (dynamicObjectByRuntimeID != null)
                {
                    dynamicObjectByRuntimeID.onOpenAnimationChange = (Action<bool>) Delegate.Remove(dynamicObjectByRuntimeID.onOpenAnimationChange, new Action<bool>(this.OnOpenAnimationChange));
                }
                this.QuitStoryStateAsDefault();
            }
        }

        public void QuitStoryStateAsDefault()
        {
            if (this._exitTransitionLerp)
            {
                bool backFollow = this._backFollowState;
                this.QuitStoryStateWithLerp(false, 1f, backFollow);
            }
            else
            {
                this.QuitStoryStateWithFade(0.5f, this._backFollowState, this._quitWithFadeIn);
            }
        }

        public void QuitStoryStateWithFade(float duration, bool backFollow = true, bool needFadeIn = false)
        {
            <QuitStoryStateWithFade>c__AnonStoreyB9 yb = new <QuitStoryStateWithFade>c__AnonStoreyB9 {
                backFollow = backFollow,
                needFadeIn = needFadeIn,
                duration = duration,
                <>f__this = this
            };
            Action fadeEndCallback = new Action(yb.<>m__79);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(yb.duration, false, null, fadeEndCallback);
        }

        public void QuitStoryStateWithLerp(bool instant = false, float lerpRatio = 1f, bool backFollow = true)
        {
            this.ClearStoryScreen();
            if (backFollow)
            {
                this.TransitToFollow(instant, lerpRatio);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish), MPEventDispatchMode.Normal);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished, null));
        }

        private void SetAllOtherDynamicObjectsAndEfffectsVisibility(bool visible)
        {
            Singleton<EffectManager>.Instance.SetAllAliveEffectPause(!visible);
            Singleton<DynamicObjectManager>.Instance.SetDynamicObjectsVisibilityExept<MonoStoryScreen>(visible);
        }

        public void SetCurrentPlotSetting(int plotID, bool lerpOut = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
        {
            this._currentPlotID = plotID;
            this._exitTransitionLerp = lerpOut;
            this._backFollowState = backFollow;
            this._quitWithFadeIn = needFadeIn;
            this._pauseLevel = pauseLevel;
        }

        public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
        {
            this._nextState = nextState;
            this._exitTransitionLerp = exitTransitionLerp;
        }

        private void SetOtherAvatarsVisibility(bool visible)
        {
            if (<>f__am$cache1C == null)
            {
                <>f__am$cache1C = x => !Singleton<AvatarManager>.Instance.IsLocalAvatar(x.GetRuntimeID());
            }
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().FindAll(<>f__am$cache1C))
            {
                Singleton<AvatarManager>.Instance.SetAvatarVisibility(visible, avatar);
                if (!visible)
                {
                    avatar.PushTimeScale(0f, 7);
                }
                else
                {
                    avatar.PopTimeScale(7);
                }
            }
        }

        private void SetStoryModeCullingMaskEnable(bool enable)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (mainCamera != null)
            {
                PostFXWithResScale component = mainCamera.GetComponent<PostFXWithResScale>();
                int[] layers = new int[] { 0, 4, 8, 9, 12, 0x17, 0x18, 0x1a };
                int cullingMaskByLayers = mainCamera.GetCullingMaskByLayers(layers);
                if ((component != null) && component.isActiveAndEnabled)
                {
                    if (enable)
                    {
                        this.originalCullingMask = component.cullingMask;
                        component.cullingMask = cullingMaskByLayers;
                    }
                    else
                    {
                        component.cullingMask = this.originalCullingMask;
                    }
                }
                else if (enable)
                {
                    this.originalCullingMask = mainCamera.cameraComponent.cullingMask;
                    mainCamera.SetMainCameraCullingMask(cullingMaskByLayers);
                }
                else
                {
                    mainCamera.SetMainCameraCullingMask(this.originalCullingMask);
                }
            }
        }

        public void StartQuit()
        {
            MonoStoryScreen dynamicObjectByRuntimeID = (MonoStoryScreen) Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(this._storyScreenID);
            if (dynamicObjectByRuntimeID != null)
            {
                dynamicObjectByRuntimeID.StartDie();
            }
        }

        public void TransitToFollow(bool instant, float lerpRatio = 1f)
        {
            this.SetNextState(base._owner.followState, !instant);
            if (this._exitTransitionLerp)
            {
                base._owner.TransitWithLerp(this._nextState, lerpRatio);
            }
            else
            {
                base._owner.Transit(this._nextState);
            }
        }

        public override void Update()
        {
            if (this._gyro != null)
            {
                this.yawOffset = Mathf.Lerp(this.yawOffset, (this._baseGravity.x - this._gyro.gravity.x) * this.ParallexRange, this.ParallexSensitivity);
                this.pitchOffset = Mathf.Lerp(this.pitchOffset, (this._baseGravity.y - this._gyro.gravity.y) * this.ParallexRange, this.ParallexSensitivity);
            }
            Vector3 axis = Vector3.Cross(this.avatar.FaceDirection, Vector3.up);
            base.cameraForward = (Vector3) (Quaternion.AngleAxis(this.yaw + this.yawOffset, Vector3.up) * this.avatar.FaceDirection);
            base.cameraForward = (Vector3) (Quaternion.AngleAxis(this.pitch + this.pitchOffset, axis) * base.cameraForward);
            base.cameraPosition = (Vector3) (((this.avatar.XZPosition - (base.cameraForward * this.anchorRadius)) + (Vector3.up * this.yOffset)) + (axis * this.xOffset));
            base.cameraFOV = this.fov;
        }

        [CompilerGenerated]
        private sealed class <QuitStoryStateWithFade>c__AnonStoreyB9
        {
            private static Action <>f__am$cache4;
            internal MainCameraStoryState <>f__this;
            internal bool backFollow;
            internal float duration;
            internal bool needFadeIn;

            internal void <>m__79()
            {
                this.<>f__this.ClearStoryScreen();
                if (this.backFollow)
                {
                    this.<>f__this.TransitToFollow(true, 1f);
                }
                if (this.needFadeIn)
                {
                    if (<>f__am$cache4 == null)
                    {
                        <>f__am$cache4 = delegate {
                            Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish), MPEventDispatchMode.Normal);
                            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished, null));
                        };
                    }
                    Action fadeEndCallback = <>f__am$cache4;
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(this.duration, false, null, fadeEndCallback);
                }
                else
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish), MPEventDispatchMode.Normal);
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished, null));
                }
            }

            private static void <>m__7B()
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtStoryState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtStoryState.State.Finish), MPEventDispatchMode.Normal);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnPlotFinished, null));
            }
        }
    }
}

