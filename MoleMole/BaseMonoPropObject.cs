namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public abstract class BaseMonoPropObject : BaseMonoAbilityEntity, IAttacker
    {
        protected bool _isToBeRemove;
        private float _lastTimeScale;
        private float _timeScale;
        protected FixedStack<float> _timeScaleStack;
        protected Transform _transform;
        [NonSerialized]
        public Animator animator;
        public ConfigPropObject config;
        [NonSerialized]
        public BaseMonoAbilityEntity owner;
        [Header("Prop Center")]
        public Transform RootNode;
        protected const string TRIGGER_APPEAR = "AppearTrigger";
        protected const string TRIGGER_HIT_PARAM = "HitTrigger";
        protected const string TRIGGER_LIGHT_HIT_PARAM = "LightHitTrigger";

        public event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

        protected BaseMonoPropObject()
        {
        }

        public override int AddAdditiveVelocity(Vector3 velocity)
        {
            return -1;
        }

        public override void AddAnimEventPredicate(string predicate)
        {
        }

        [AnimationCallback]
        public void AnimEventHandler(string animEventID)
        {
        }

        public virtual void Appear()
        {
            this.animator.SetTrigger("AppearTrigger");
        }

        [AnimationCallback]
        public void AppearEndTriggerEffect(string effectName)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(effectName, this.XZPosition, this.FaceDirection, Vector3.one, this);
        }

        [AnimationCallback]
        public void AppearStartTriggerEffect(string effectName)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(effectName, this.XZPosition, this.FaceDirection, Vector3.one, this);
        }

        protected virtual void Awake()
        {
            this._transform = base.transform;
            this.animator = base.GetComponent<Animator>();
        }

        public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID)
        {
        }

        public override bool ContainAnimEventPredicate(string predicate)
        {
            return false;
        }

        public override void FireEffect(string patternName)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, this.XZPosition, base.transform.forward, base.transform.localScale, this);
        }

        public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, base.transform.localScale, this);
        }

        public override void FireEffectTo(string patternName, BaseMonoEntity to)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, this.XZPosition, base.transform.forward, base.transform.localScale, this, to);
        }

        public virtual void FrameHalt(int frameNum)
        {
        }

        public override Transform GetAttachPoint(string name)
        {
            if (name == "RootNode")
            {
                return this.RootNode;
            }
            return base.transform;
        }

        public override BaseMonoEntity GetAttackTarget()
        {
            return null;
        }

        public override float GetCurrentNormalizedTime()
        {
            return 0f;
        }

        public override bool HasAdditiveVelocityOfIndex(int index)
        {
            return false;
        }

        public void Init(uint ownerID, uint runtimeID, string propName, bool appearAnim = false)
        {
            this.config = PropObjectData.GetPropObjectConfig(propName);
            base.commonConfig = this.config.CommonConfig;
            this.Init(runtimeID);
            this._timeScaleStack = new FixedStack<float>(8, null);
            this._timeScaleStack.Push(1f, true);
            if ((Singleton<LevelManager>.Instance != null) && (Singleton<LevelManager>.Instance.levelEntity != null))
            {
                this._timeScale = this._lastTimeScale = this._timeScaleStack.value * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            }
            else
            {
                this._timeScale = this._lastTimeScale = this._timeScaleStack.value;
            }
            if (Singleton<EventManager>.Instance != null)
            {
                this.owner = (BaseMonoAbilityEntity) Singleton<EventManager>.Instance.GetEntity(ownerID);
            }
            if ((this.config.PropArguments != null) && !this.config.PropArguments.IsTargetable)
            {
                base.SetCountedDenySelect(true, true);
            }
            if ((this.config.PropArguments != null) && (this.config.PropArguments.Duration > 0f))
            {
                base.StartCoroutine(this.WaitDestroyByDuration(this.config.PropArguments.Duration));
            }
            if (appearAnim)
            {
                this.Appear();
            }
        }

        public override bool IsActive()
        {
            return !this._isToBeRemove;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemove;
        }

        public override void MaskAnimEvent(string animEventName)
        {
        }

        public override void MaskTrigger(string triggerID)
        {
        }

        float IAttacker.Evaluate(DynamicFloat target)
        {
            return base.Evaluate(target);
        }

        int IAttacker.Evaluate(DynamicInt target)
        {
            return base.Evaluate(target);
        }

        Transform IAttacker.get_transform()
        {
            return base.transform;
        }

        uint IAttacker.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        public void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
        {
            if (this.onAnimatedHitBoxCreatedCallBack != null)
            {
                this.onAnimatedHitBoxCreatedCallBack(hitBox, attackPattern);
            }
        }

        protected virtual void OnDurationTimeOut()
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectForceKilled(base._runtimeID), MPEventDispatchMode.Normal);
        }

        protected virtual void OnTimeScaleChanged(float newTimeScale)
        {
        }

        public override void PopHighspeedMovement()
        {
        }

        public override void PopMaterialGroup()
        {
        }

        public override void PopTimeScale(int stackIx)
        {
            if (this._timeScaleStack.IsOccupied(stackIx))
            {
                this._timeScaleStack.Pop(stackIx);
            }
        }

        public override void PushHighspeedMovement()
        {
        }

        public override void PushMaterialGroup(string targetGroupname)
        {
        }

        public override void PushTimeScale(float timescale, int stackIx)
        {
            this._timeScaleStack.Push(stackIx, timescale, false);
        }

        public override void RemoveAnimEventPredicate(string predicate)
        {
        }

        public override void ResetTrigger(string name)
        {
            if (this.animator != null)
            {
                this.animator.ResetTrigger(name);
            }
        }

        public override void SetAdditiveVelocity(Vector3 velocity)
        {
        }

        public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
        {
        }

        public override void SetAttackTarget(BaseMonoEntity attackTarget)
        {
        }

        public override void SetDied(KillEffect killEffect)
        {
            this._isToBeRemove = true;
        }

        public override void SetHasAdditiveVelocity(bool hasAdditiveVelocity)
        {
        }

        public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
        {
        }

        public override void SetOverrideVelocity(Vector3 velocity)
        {
        }

        public override void SetTimeScale(float timescale, int stackIx)
        {
            this._timeScaleStack.Set(stackIx, timescale, false);
        }

        public override void SetTrigger(string name)
        {
            if (this.animator != null)
            {
                this.animator.SetTrigger(name);
            }
        }

        private void Start()
        {
            if ((Singleton<EventManager>.Instance != null) && (this.owner != null))
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectCreated(this.owner.GetRuntimeID(), base._runtimeID), MPEventDispatchMode.Normal);
            }
        }

        public override void SteerFaceDirectionTo(Vector3 forward)
        {
            base.transform.forward = forward;
        }

        public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
        {
            ConfigPropAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
            event2.AttackPattern.patternMethod(animEventID, event2.AttackPattern, this, layerMask);
        }

        public override void UnmaskAnimEvent(string animEventName)
        {
        }

        public override void UnmaskTrigger(string triggerID)
        {
        }

        protected virtual void Update()
        {
            this._timeScale = (((this.owner != null) ? this.owner.TimeScale : Singleton<LevelManager>.Instance.levelEntity.TimeScale) * this._timeScaleStack.value) * (1f + this.GetProperty("Entity_TimeScaleDelta"));
            if (this._lastTimeScale != this.TimeScale)
            {
                this.OnTimeScaleChanged(this.TimeScale);
            }
            this._lastTimeScale = this.TimeScale;
        }

        [DebuggerHidden]
        private IEnumerator WaitDestroyByDuration(float duration)
        {
            return new <WaitDestroyByDuration>c__Iterator28 { duration = duration, <$>duration = duration, <>f__this = this };
        }

        public BaseMonoEntity AttackTarget
        {
            get
            {
                return null;
            }
        }

        public override string CurrentSkillID
        {
            get
            {
                return null;
            }
        }

        public Vector3 FaceDirection
        {
            get
            {
                return base.transform.forward;
            }
        }

        public override float TimeScale
        {
            get
            {
                return this._timeScale;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return new Vector3(this._transform.position.x, 0f, this._transform.position.z);
            }
        }

        [CompilerGenerated]
        private sealed class <WaitDestroyByDuration>c__Iterator28 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>duration;
            internal BaseMonoPropObject <>f__this;
            internal float duration;

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
                    case 1:
                        if (this.duration > 0f)
                        {
                            this.duration -= Time.deltaTime * this.<>f__this._timeScale;
                            this.$current = null;
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this.OnDurationTimeOut();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
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
    }
}

