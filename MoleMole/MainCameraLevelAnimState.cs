namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MainCameraLevelAnimState : BaseMainCameraState
    {
        private Animation _animation;
        private CameraAnimationCullingType _cullType;
        private bool _exitTransitionLerp;
        private bool _ignoreTimeScale;
        private bool _isFirstFrame;
        private Action _levelAnimEndCallback;
        private Action _levelAnimStartCallback;
        private BaseMainCameraState _nextState;
        private Vector2 _origRadialBlurCenter;
        private float _origRadialBlurScatterScale;
        private float _origRadialBlurStrenth;
        private bool _pauseLevel;
        private MonoSimpleAnimation _simpleAnimationComponent;

        public MainCameraLevelAnimState(MonoMainCamera camera) : base(camera)
        {
            this._nextState = camera.staticState;
            this._exitTransitionLerp = false;
            base.muteCameraShake = false;
        }

        private void DoExit()
        {
            if (this._exitTransitionLerp)
            {
                base._owner.TransitWithLerp(this._nextState, 0.3f);
            }
            else
            {
                base._owner.Transit(this._nextState);
                Singleton<EventManager>.Instance.FireEvent(new EvtCamearaAnimState(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), EvtCamearaAnimState.State.Finish), MPEventDispatchMode.Normal);
            }
        }

        public override void Enter()
        {
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
            this._isFirstFrame = true;
            if (this._simpleAnimationComponent != null)
            {
                if (this._simpleAnimationComponent.initClipZNear > 0f)
                {
                    base._owner.cameraComponent.nearClipPlane = Mathf.Max(0.01f, this._simpleAnimationComponent.initClipZNear);
                }
                if (this._simpleAnimationComponent.initFOV > 0f)
                {
                    base._owner.cameraComponent.fieldOfView = this._simpleAnimationComponent.initFOV;
                }
                this._simpleAnimationComponent.selfUpdateKeyedRotation = false;
                if (this._simpleAnimationComponent.useKeyedDirectionalLightRotation)
                {
                    base._owner.SetNeedLerpDirectionalLight(false);
                }
            }
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(false, false, true);
            PostFX component = base._owner.GetComponent<PostFX>();
            if (component != null)
            {
                this._origRadialBlurCenter = component.RadialBlurCenter;
                this._origRadialBlurStrenth = component.RadialBlurStrenth;
                this._origRadialBlurScatterScale = component.RadialBlurScatterScale;
            }
            if (this._pauseLevel)
            {
                Singleton<LevelManager>.Instance.SetPause(true);
            }
            if (this._cullType == CameraAnimationCullingType.CullAvatars)
            {
                Singleton<AvatarManager>.Instance.SetAllAvatarVisibility(false);
            }
            if (this._levelAnimStartCallback != null)
            {
                this._levelAnimStartCallback();
            }
        }

        public override void Exit()
        {
            if (this._levelAnimEndCallback != null)
            {
                this._levelAnimEndCallback();
            }
            if (this._animation != null)
            {
                this._animation.Stop();
                UnityEngine.Object.Destroy(this._animation.gameObject);
            }
            base._owner.cameraComponent.nearClipPlane = base._owner.originalNearClip;
            base._owner.cameraComponent.fieldOfView = base._owner.originalFOV;
            if (this._simpleAnimationComponent.useKeyedRadialBlur)
            {
                this._simpleAnimationComponent.useKeyedRadialBlur = false;
                PostFX component = base._owner.GetComponent<PostFX>();
                if (component != null)
                {
                    component.RadialBlurCenter = this._origRadialBlurCenter;
                    component.RadialBlurStrenth = this._origRadialBlurStrenth;
                    component.RadialBlurScatterScale = this._origRadialBlurScatterScale;
                }
            }
            if (this._pauseLevel)
            {
                Singleton<LevelManager>.Instance.SetPause(false);
            }
            if (this._cullType == CameraAnimationCullingType.CullAvatars)
            {
                Singleton<AvatarManager>.Instance.SetAllAvatarVisibility(true);
            }
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.SetInLevelMainPageActive(true, false, true);
            base._owner.SetNeedLerpDirectionalLight(true);
        }

        public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
        {
            this._nextState = nextState;
            this._exitTransitionLerp = exitTransitionLerp;
        }

        public void SetupFollowAnim(Animation anim, bool ignoreTimeScale, bool pauseLevel, CameraAnimationCullingType cullType = 0, Action levelAnimStartCallback = null, Action levelAnimEndCallback = null)
        {
            this._animation = anim;
            this._pauseLevel = pauseLevel;
            this._ignoreTimeScale = ignoreTimeScale;
            this._simpleAnimationComponent = anim.GetComponent<MonoSimpleAnimation>();
            this._levelAnimStartCallback = levelAnimStartCallback;
            this._levelAnimEndCallback = levelAnimEndCallback;
            this._cullType = cullType;
        }

        private void SyncByAnimation()
        {
            base.cameraPosition = this._animation.transform.position;
            base.cameraForward = this._animation.transform.forward;
            base.cameraFOV = base._owner.originalFOV;
            if (this._simpleAnimationComponent != null)
            {
                if (this._simpleAnimationComponent.useKeyedFOV)
                {
                    base.cameraFOV = this._simpleAnimationComponent.keyedFOV;
                }
                if (this._simpleAnimationComponent.useKeyedDirectionalLightRotation)
                {
                    base._owner.directionalLight.transform.rotation = this._simpleAnimationComponent.GetLightRotation();
                }
            }
            if ((this._simpleAnimationComponent != null) && this._simpleAnimationComponent.useKeyedRadialBlur)
            {
                PostFX component = base._owner.GetComponent<PostFX>();
                if (component != null)
                {
                    component.RadialBlurCenter = this._simpleAnimationComponent.radialBlurCenter;
                    component.RadialBlurScatterScale = this._simpleAnimationComponent.radialBlurScatterScale;
                    component.RadialBlurStrenth = this._simpleAnimationComponent.radialBlurStrenth;
                }
            }
        }

        public override void Update()
        {
            if (this._simpleAnimationComponent != null)
            {
                this._simpleAnimationComponent.SyncRotation();
            }
            if (this._isFirstFrame)
            {
                this.SyncByAnimation();
                this._isFirstFrame = false;
            }
            else
            {
                if ((this._animation != null) && this._animation.isPlaying)
                {
                    this.SyncByAnimation();
                    if (this._ignoreTimeScale)
                    {
                        if (!this._pauseLevel)
                        {
                            if (Singleton<LevelManager>.Instance.IsPaused())
                            {
                                this._animation[this._animation.clip.name].speed = 0f;
                            }
                            else
                            {
                                this._animation[this._animation.clip.name].speed = 1f;
                            }
                        }
                    }
                    else
                    {
                        this._animation[this._animation.clip.name].speed = Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                    }
                }
                else
                {
                    this.DoExit();
                }
                Debug.DrawLine(base._owner.transform.position, base._owner.transform.position + ((Vector3) (base._owner.transform.forward * 10f)), Color.red, 3f);
            }
        }
    }
}

