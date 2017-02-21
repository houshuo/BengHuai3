namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MainCameraAvatarAnimState : BaseMainCameraState
    {
        private Animation _animation;
        private BaseMonoAvatar _avatar;
        private bool _exitTransitionLerp;
        private bool _isFirstFrame;
        private bool _isInterrupted;
        private bool _isLastFrameTimeScaleZero;
        private BaseMainCameraState _nextState;
        private float _sampleTimer;
        private MonoSimpleAnimation _simpleAnimationComponent;

        public MainCameraAvatarAnimState(MonoMainCamera camera) : base(camera)
        {
            this._nextState = base._owner.followState;
            this._exitTransitionLerp = false;
        }

        private void DoExit()
        {
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
            if (this._isInterrupted)
            {
                this._animation.enabled = true;
                this._isInterrupted = false;
            }
            else
            {
                this._isFirstFrame = true;
                this._sampleTimer = 0f;
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
            }
        }

        public override void Exit()
        {
            if (this._isInterrupted)
            {
                this._animation.enabled = false;
            }
            else
            {
                base._owner.cameraComponent.nearClipPlane = base._owner.originalNearClip;
                base._owner.cameraComponent.fieldOfView = base._owner.originalFOV;
                if (this._animation != null)
                {
                    this._animation.Stop();
                    UnityEngine.Object.Destroy(this._animation.gameObject);
                }
                base._owner.SetNeedLerpDirectionalLight(true);
            }
        }

        public void SetInterrupt()
        {
            this._isInterrupted = true;
        }

        public void SetNextState(BaseMainCameraState nextState, bool exitTransitionLerp)
        {
            this._nextState = nextState;
            this._exitTransitionLerp = exitTransitionLerp;
        }

        public void SetupFollowAvatarAnim(Animation animation, BaseMonoAvatar avatar)
        {
            this._animation = animation;
            this._avatar = avatar;
            this._simpleAnimationComponent = animation.GetComponent<MonoSimpleAnimation>();
        }

        private void SyncByAnimation()
        {
            base.cameraPosition = this._animation.transform.position;
            if (this._simpleAnimationComponent.useAnimRotation)
            {
                if (this._simpleAnimationComponent.realAnimGameObject != null)
                {
                    base.cameraForward = this._simpleAnimationComponent.realAnimGameObject.transform.forward;
                }
                else
                {
                    base.cameraForward = this._animation.transform.forward;
                }
            }
            else
            {
                base.cameraForward = this._animation.transform.forward;
            }
            if (this._simpleAnimationComponent != null)
            {
                if (this._simpleAnimationComponent.useKeyedFOV)
                {
                    base.cameraFOV = this._simpleAnimationComponent.keyedFOV;
                }
                else
                {
                    base.cameraFOV = base._owner.originalFOV;
                }
                if (this._simpleAnimationComponent.useKeyedDirectionalLightRotation)
                {
                    base._owner.directionalLight.transform.rotation = this._simpleAnimationComponent.GetLightRotation();
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
                this._animation[this._animation.clip.name].time = 0f;
                this._isFirstFrame = false;
            }
            else if ((this._animation != null) && this._animation.isPlaying)
            {
                if (this._avatar != null)
                {
                    if ((this._simpleAnimationComponent != null) && this._simpleAnimationComponent.hasPushedLevelTimeScale)
                    {
                        this.SyncByAnimation();
                        if (Singleton<LevelManager>.Instance.IsPaused())
                        {
                            this._animation[this._animation.clip.name].speed = 0f;
                        }
                        else
                        {
                            this._animation[this._animation.clip.name].speed = 1f;
                            this._sampleTimer += Time.deltaTime;
                        }
                    }
                    else
                    {
                        this._sampleTimer += Time.deltaTime * this._avatar.TimeScale;
                        if (this._isLastFrameTimeScaleZero && (this._avatar.TimeScale != 0f))
                        {
                            this._sampleTimer += 0.01666667f;
                        }
                        this._animation[this._animation.clip.name].time = this._sampleTimer;
                        this._animation.Sample();
                        this.SyncByAnimation();
                        this._isLastFrameTimeScaleZero = this._avatar.TimeScale == 0f;
                    }
                }
                else
                {
                    this.DoExit();
                }
            }
            else
            {
                this.DoExit();
            }
        }
    }
}

