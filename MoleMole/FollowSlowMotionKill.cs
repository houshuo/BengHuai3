namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEngine;

    public class FollowSlowMotionKill : BaseFollowShortState
    {
        private SlowMotionEffect[] _effects;
        private ParamsBase _followingShortState;
        private bool _hasUserControled;
        private float _lastElevationOffset;
        private float _lastRadiusOffset;
        protected float _origElevation;
        protected float _origRadius;
        private State _state;
        private static readonly int MAX_EFFECT_NUM = 2;

        public FollowSlowMotionKill(MainCameraFollowState followState) : base(followState)
        {
            base.isSkippingBaseState = false;
            this._effects = new SlowMotionEffect[MAX_EFFECT_NUM];
            for (int i = 0; i < this._effects.Length; i++)
            {
                this._effects[i] = new SlowMotionEffect();
            }
        }

        [DebuggerHidden]
        private IEnumerable ActiveEffectEnum()
        {
            return new <ActiveEffectEnum>c__Iterator20 { <>f__this = this, $PC = -2 };
        }

        private void ClearEffects()
        {
            foreach (SlowMotionEffect effect in this._effects)
            {
                effect.active = false;
            }
        }

        public override void Enter()
        {
            if (base._owner.followAvatarControlledRotate.active && base._owner.followAvatarControlledRotate.IsExiting())
            {
                base._owner.TransitBaseState(base._owner.followAvatarState, false);
            }
            if (base._owner.recoverState.active)
            {
                base._owner.recoverState.CancelAndStopAtCurrentState();
            }
            this._origElevation = base._owner.recoverState.GetOriginalElevation();
            this._origRadius = base._owner.recoverState.GetOriginalRadius();
            this._lastRadiusOffset = 0f;
            this._lastElevationOffset = 0f;
            this._state = State.Entering;
            this._hasUserControled = false;
            IEnumerator enumerator = this.ActiveEffectEnum().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ((SlowMotionEffect) enumerator.Current).Enter(base._owner);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_Start", null, null, null);
        }

        public override void Exit()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_End", null, null, null);
            if (!base._owner.followAvatarControlledRotate.active)
            {
                base._owner.recoverState.TryRecover();
            }
        }

        private bool HasActiveEffect()
        {
            foreach (SlowMotionEffect effect in this._effects)
            {
                if (effect.active)
                {
                    return true;
                }
            }
            return false;
        }

        public override void PostUpdate()
        {
            if (this._state == State.Entering)
            {
                this._state = State.During;
            }
            else if (this._state == State.During)
            {
                SlowMotionEffect.OutParams @params = this.UpdateAndBlendEffectes();
                if (@params != null)
                {
                    Time.timeScale = @params.timeScale;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    if (!base._owner.followAvatarControlledRotate.active && !this._hasUserControled)
                    {
                        base._owner.anchorPolar += @params.anchorDeltaPolar;
                        base._owner.anchorRadius += @params.anchorRadiusOffset - this._lastRadiusOffset;
                        this._lastRadiusOffset = @params.anchorRadiusOffset;
                        base._owner.anchorElevation += @params.anchorElevationOffset - this._lastElevationOffset;
                        float b = -Mathf.Asin(Mathf.Max((float) (base._owner.followCenterY - 0.3f), (float) 0f) / Mathf.Max((float) (base._owner.anchorRadius * base._owner.cameraLocateRatio), (float) 0.1f)) * 57.29578f;
                        base._owner.anchorElevation = Mathf.Max(base._owner.anchorElevation, b);
                        this._lastElevationOffset = @params.anchorElevationOffset;
                        base._owner.forwardDeltaAngle = @params.forwardDeltaAngle;
                    }
                    else
                    {
                        this._hasUserControled = true;
                    }
                }
                if (!this.HasActiveEffect())
                {
                    this._state = State.Exiting;
                }
                base._owner.needLerpPositionThisFrame = false;
                base._owner.needLerpForwardThisFrame = false;
            }
            else if (this._state == State.Exiting)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                if (this._followingShortState != null)
                {
                    this.SetFollowingShortState();
                }
                else
                {
                    base.End();
                }
            }
        }

        public void SetFollowingLookAtPosition(Vector3 position, bool mute)
        {
            LookAtPositionParams @params = new LookAtPositionParams {
                position = position,
                mute = mute
            };
            this._followingShortState = @params;
        }

        private void SetFollowingShortState()
        {
            if ((this._followingShortState != null) && (this._followingShortState.GetType() == typeof(LookAtPositionParams)))
            {
                LookAtPositionParams @params = this._followingShortState as LookAtPositionParams;
                base._owner.mainCamera.FollowLookAtPosition(@params.position, @params.mute, true);
            }
        }

        public void SetSlowMotionKill(ConfigCameraSlowMotionKill config, float distTarget, float distCamera)
        {
            IEnumerator enumerator = this.ActiveEffectEnum().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SlowMotionEffect current = (SlowMotionEffect) enumerator.Current;
                    if (!current.OverDuration(0.5f))
                    {
                        return;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            foreach (SlowMotionEffect effect2 in this._effects)
            {
                if (!effect2.active)
                {
                    effect2.Set(config, distTarget, distCamera);
                    effect2.active = true;
                    if (base.active)
                    {
                        effect2.Enter(base._owner);
                    }
                    break;
                }
            }
        }

        public override void Update()
        {
            base.isSkippingBaseState = !base._owner.followAvatarControlledRotate.active;
            if (base._owner.followAvatarState.active)
            {
                base._owner.followAvatarState.CancelEnteringLerp();
            }
        }

        private SlowMotionEffect.OutParams UpdateAndBlendEffectes()
        {
            if (!this.HasActiveEffect())
            {
                return null;
            }
            int num = 0;
            SlowMotionEffect.OutParams @params = new SlowMotionEffect.OutParams {
                timeScale = 1f,
                anchorDeltaPolar = 0f,
                anchorElevationOffset = 0f,
                anchorRadiusOffset = 0f,
                forwardDeltaAngle = 0f
            };
            IEnumerator enumerator = this.ActiveEffectEnum().GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    SlowMotionEffect current = (SlowMotionEffect) enumerator.Current;
                    num++;
                    current.Update();
                    SlowMotionEffect.OutParams outParams = current.outParams;
                    @params.timeScale *= outParams.timeScale;
                    @params.anchorDeltaPolar += outParams.anchorDeltaPolar;
                    @params.anchorElevationOffset += outParams.anchorElevationOffset;
                    @params.anchorRadiusOffset += outParams.anchorRadiusOffset;
                    @params.forwardDeltaAngle += outParams.forwardDeltaAngle;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            @params.anchorDeltaPolar /= (float) num;
            @params.anchorElevationOffset /= (float) num;
            @params.anchorRadiusOffset /= (float) num;
            @params.forwardDeltaAngle /= (float) num;
            return @params;
        }

        [CompilerGenerated]
        private sealed class <ActiveEffectEnum>c__Iterator20 : IEnumerator, IDisposable, IEnumerable, IEnumerator<object>, IEnumerable<object>
        {
            internal object $current;
            internal int $PC;
            internal SlowMotionEffect[] <$s_1030>__0;
            internal int <$s_1031>__1;
            internal FollowSlowMotionKill <>f__this;
            internal SlowMotionEffect <effect>__2;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<$s_1030>__0 = this.<>f__this._effects;
                        this.<$s_1031>__1 = 0;
                        goto Label_0087;

                    case 1:
                        break;

                    default:
                        goto Label_00A1;
                }
            Label_0079:
                this.<$s_1031>__1++;
            Label_0087:
                if (this.<$s_1031>__1 < this.<$s_1030>__0.Length)
                {
                    this.<effect>__2 = this.<$s_1030>__0[this.<$s_1031>__1];
                    if (this.<effect>__2.active)
                    {
                        this.$current = this.<effect>__2;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_0079;
                }
                this.$PC = -1;
            Label_00A1:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            IEnumerator<object> IEnumerable<object>.GetEnumerator()
            {
                if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
                {
                    return this;
                }
                return new FollowSlowMotionKill.<ActiveEffectEnum>c__Iterator20 { <>f__this = this.<>f__this };
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<object>.GetEnumerator();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private class LookAtPositionParams : FollowSlowMotionKill.ParamsBase
        {
            public bool mute;
            public Vector3 position;
        }

        private class ParamsBase
        {
        }

        private enum State
        {
            Entering,
            During,
            Exiting
        }
    }
}

