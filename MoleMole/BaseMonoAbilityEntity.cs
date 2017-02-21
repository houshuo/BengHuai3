namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public abstract class BaseMonoAbilityEntity : BaseMonoEntity
    {
        protected Dictionary<string, bool> _animatorBoolParams;
        protected Dictionary<string, int> _animatorIntParams;
        private int _denySelectCount;
        public Dictionary<string, FixedSafeFloatStack> _entityProperties;
        private int _isGhostCount;
        private Dictionary<string, Action> _propertyChangedCallbacks;
        public ConfigCommonEntity commonConfig;
        public Action<bool> onActiveChanged;
        public Action onAnimatorBoolChanged;
        public Action onAnimatorIntChanged;
        public Action<string> onBeHitCanceled;
        public Action<string, string> onCurrentSkillIDChanged;
        public Action<bool> onHasAdditiveVelocityChanged;
        public Action<bool> onIsGhostChanged;

        protected BaseMonoAbilityEntity()
        {
        }

        public abstract int AddAdditiveVelocity(Vector3 velocity);
        public abstract void AddAnimEventPredicate(string predicate);
        public void AddEffectOverride(string effectOverrideKey, string effectPattern)
        {
            MonoEffectOverride component = base.GetComponent<MonoEffectOverride>();
            if (component == null)
            {
                component = base.gameObject.AddComponent<MonoEffectOverride>();
            }
            component.effectOverrides.Add(effectOverrideKey, effectPattern);
        }

        public virtual int AttachEffect(string effectPattern)
        {
            return Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(effectPattern, this);
        }

        public abstract bool ContainAnimEventPredicate(string predicate);
        public virtual void DetachEffect(int patternIx)
        {
            Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(patternIx);
        }

        public virtual void DetachEffectImmediately(int patternIx)
        {
            Singleton<EffectManager>.Instance.SetDestroyImmediatelyIndexedEffectPattern(patternIx);
        }

        public float Evaluate(DynamicFloat target)
        {
            if (target.isDynamic)
            {
                return this.GetProperty(target.dynamicKey);
            }
            return target.fixedValue;
        }

        public int Evaluate(DynamicInt target)
        {
            if (target.isDynamic)
            {
                return (int) this.GetProperty(target.dynamicKey);
            }
            return target.fixedValue;
        }

        public abstract void FireEffect(string patternName);
        public abstract void FireEffect(string patternName, Vector3 initPos, Vector3 initDir);
        public abstract void FireEffectTo(string patternName, BaseMonoEntity to);
        public abstract BaseMonoEntity GetAttackTarget();
        public abstract float GetCurrentNormalizedTime();
        public virtual float GetProperty(string propertyKey)
        {
            if (this._entityProperties.ContainsKey(propertyKey))
            {
                return (float) this._entityProperties[propertyKey].value;
            }
            if (this.commonConfig.EntityProperties.ContainsKey(propertyKey))
            {
                return this.commonConfig.EntityProperties[propertyKey].Default;
            }
            return AbilityData.PROPERTIES[propertyKey].Default;
        }

        public virtual float GetPropertyByStackIndex(string propertyKey, int stackIx)
        {
            return this._entityProperties[propertyKey].Get(stackIx);
        }

        public abstract bool HasAdditiveVelocityOfIndex(int index);
        public virtual void Init(uint runtimeID)
        {
            base._runtimeID = runtimeID;
            this._entityProperties = new Dictionary<string, FixedSafeFloatStack>();
            this._animatorBoolParams = new Dictionary<string, bool>();
            this._animatorIntParams = new Dictionary<string, int>();
            this._propertyChangedCallbacks = new Dictionary<string, Action>();
        }

        public abstract void MaskAnimEvent(string animEventName);
        public abstract void MaskTrigger(string triggerID);
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onCurrentSkillIDChanged = null;
            this.onActiveChanged = null;
            this.onAnimatorBoolChanged = null;
            this.onAnimatorIntChanged = null;
        }

        protected virtual void OnDisable()
        {
            if (this.onActiveChanged != null)
            {
                this.onActiveChanged(false);
            }
        }

        protected virtual void OnEnable()
        {
            if (this.onActiveChanged != null)
            {
                this.onActiveChanged(true);
            }
        }

        public void PlayAudio(string audioPattern)
        {
        }

        public void PlayAudio(string audioPattern, Transform target)
        {
        }

        public abstract void PopHighspeedMovement();
        public abstract void PopMaterialGroup();
        public virtual void PopNoCollision()
        {
        }

        public virtual void PopProperty(string propertyKey, int stackIx)
        {
            this._entityProperties[propertyKey].Pop(stackIx);
        }

        public abstract void PopTimeScale(int stackIx);
        public abstract void PushHighspeedMovement();
        public abstract void PushMaterialGroup(string targetGroupname);
        public virtual void PushNoCollision()
        {
        }

        public virtual int PushProperty(string propertyKey, float value)
        {
            if (!this._entityProperties.ContainsKey(propertyKey))
            {
                bool flag = false;
                if (AbilityData.PROPERTIES.ContainsKey(propertyKey))
                {
                    flag = true;
                    this._entityProperties.Add(propertyKey, AbilityData.PROPERTIES[propertyKey].CreatePropertySafeStack());
                }
                else
                {
                    flag = true;
                    this._entityProperties.Add(propertyKey, this.commonConfig.EntityProperties[propertyKey].CreatePropertySafeStack());
                }
                if (flag && this._propertyChangedCallbacks.ContainsKey(propertyKey))
                {
                    <PushProperty>c__AnonStoreyB8 yb = new <PushProperty>c__AnonStoreyB8 {
                        callback = this._propertyChangedCallbacks[propertyKey]
                    };
                    FixedSafeFloatStack local1 = this._entityProperties[propertyKey];
                    local1.onChanged = (Action<SafeFloat, int, SafeFloat, int>) Delegate.Combine(local1.onChanged, new Action<SafeFloat, int, SafeFloat, int>(yb.<>m__76));
                    this._propertyChangedCallbacks.Remove(propertyKey);
                }
            }
            return this._entityProperties[propertyKey].Push(value, false);
        }

        public abstract void PushTimeScale(float timescale, int stackIx);
        protected void RegisterPropertyChangedCallback(string propertyKey, Action callback)
        {
            this._propertyChangedCallbacks.Add(propertyKey, callback);
        }

        public abstract void RemoveAnimEventPredicate(string predicate);
        public void RemoveEffectOverride(string effectOverrideKey)
        {
            base.GetComponent<MonoEffectOverride>().effectOverrides.Remove(effectOverrideKey);
        }

        public virtual void RemovePersistentAnimatorBool(string key)
        {
            this._animatorBoolParams.Remove(key);
            if (this.onAnimatorBoolChanged != null)
            {
                this.onAnimatorBoolChanged();
            }
        }

        public virtual void RemovePersistentAnimatorInt(string key)
        {
            this._animatorIntParams.Remove(key);
            if (this.onAnimatorIntChanged != null)
            {
                this.onAnimatorIntChanged();
            }
        }

        public abstract void ResetTrigger(string name);
        public abstract void SetAdditiveVelocity(Vector3 velocity);
        public abstract void SetAdditiveVelocityOfIndex(Vector3 velocity, int index);
        public abstract void SetAttackTarget(BaseMonoEntity attackTarget);
        public void SetCountedDenySelect(bool value, bool pernament = false)
        {
            bool denySelect = this.denySelect;
            if (pernament)
            {
                this._denySelectCount += !value ? -1000 : 0x3e8;
            }
            else
            {
                this._denySelectCount += !value ? -1 : 1;
            }
            if (!denySelect && this.denySelect)
            {
                Singleton<LevelManager>.Instance.levelActor.UntargetEntity(this);
            }
        }

        public void SetCountedIsGhost(bool value)
        {
            bool isGhost = this.isGhost;
            this._isGhostCount += !value ? -1 : 1;
            if ((isGhost != value) && (this.onIsGhostChanged != null))
            {
                this.onIsGhostChanged(value);
            }
        }

        public abstract void SetDied(KillEffect killEffect);
        public abstract void SetHasAdditiveVelocity(bool hasAdditiveVelocity);
        public abstract void SetNeedOverrideVelocity(bool needOverrideVelocity);
        public abstract void SetOverrideVelocity(Vector3 velocity);
        public virtual void SetPersistentAnimatoInt(string key, int value)
        {
            if (!this._animatorIntParams.ContainsKey(key))
            {
                this._animatorIntParams.Add(key, value);
            }
            else
            {
                this._animatorIntParams[key] = value;
            }
            if (this.onAnimatorIntChanged != null)
            {
                this.onAnimatorIntChanged();
            }
        }

        public virtual void SetPersistentAnimatorBool(string key, bool value)
        {
            if (!this._animatorBoolParams.ContainsKey(key))
            {
                this._animatorBoolParams.Add(key, value);
            }
            else
            {
                this._animatorBoolParams[key] = value;
            }
            if (this.onAnimatorBoolChanged != null)
            {
                this.onAnimatorBoolChanged();
            }
        }

        public virtual void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
        {
            this._entityProperties[propertyKey].Set(stackIx, value, false);
        }

        public virtual void SetShaderData(E_ShaderData dataType, bool bEnable)
        {
        }

        public virtual void SetShaderDataLerp(E_ShaderData dataType, bool bEnable, float enableDuration = -1f, float disableDuration = -1f, bool bUseNewTexture = false)
        {
        }

        public abstract void SetTimeScale(float timescale, int stackIx);
        public abstract void SetTrigger(string name);
        public abstract void SteerFaceDirectionTo(Vector3 forward);
        public void StopAudio(string audioPattern)
        {
        }

        public abstract void TriggerAttackPattern(string animEventID, LayerMask layerMask);
        public abstract void UnmaskAnimEvent(string animEventName);
        public abstract void UnmaskTrigger(string triggerID);

        public abstract string CurrentSkillID { get; }

        public bool denySelect
        {
            get
            {
                return (this._denySelectCount > 0);
            }
        }

        public bool isGhost
        {
            get
            {
                return (this._isGhostCount > 0);
            }
        }

        [CompilerGenerated]
        private sealed class <PushProperty>c__AnonStoreyB8
        {
            internal Action callback;

            internal void <>m__76(SafeFloat oldV, int oldIx, SafeFloat newV, int newIx)
            {
                this.callback();
            }
        }
    }
}

