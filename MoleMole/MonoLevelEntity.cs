namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoLevelEntity : BaseMonoAbilityEntity
    {
        private FixedStack<float> _auxTimeScaleStack;
        private FixedStack<float> _timeScaleStack;
        private static ConfigCommonEntity LEVEL_ENTITY_COMMON_CONFIG;

        static MonoLevelEntity()
        {
            ConfigCommonEntity entity = new ConfigCommonEntity {
                CommonArguments = new ConfigEntityCommonArguments(),
                EntityProperties = new Dictionary<string, ConfigAbilityPropertyEntry>()
            };
            LEVEL_ENTITY_COMMON_CONFIG = entity;
        }

        public override int AddAdditiveVelocity(Vector3 velocity)
        {
            return -1;
        }

        public override void AddAnimEventPredicate(string predicate)
        {
        }

        protected void Awake()
        {
            base._runtimeID = 0x21800001;
            this._timeScaleStack = new FixedStack<float>(8, null);
            this._timeScaleStack.Push(1f, true);
            this._auxTimeScaleStack = new FixedStack<float>(8, new Action<float, int, float, int>(this.OnAuxTimeScaleChanged));
            this._auxTimeScaleStack.Push(1f, true);
        }

        public override bool ContainAnimEventPredicate(string predicate)
        {
            return false;
        }

        public override void FireEffect(string patternName)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, this.XZPosition, base.transform.forward, Vector3.one, this);
        }

        public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, Vector3.one, this);
        }

        public override void FireEffectTo(string patternName, BaseMonoEntity to)
        {
        }

        public override Transform GetAttachPoint(string name)
        {
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

        public override void Init(uint runtimeID)
        {
            base.commonConfig = LEVEL_ENTITY_COMMON_CONFIG;
            base.Init(runtimeID);
        }

        public override bool IsActive()
        {
            return true;
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public override void MaskAnimEvent(string animEventName)
        {
        }

        public override void MaskTrigger(string triggerID)
        {
        }

        private void OnAuxTimeScaleChanged(float oldValue, int oldIx, float newValue, int newIx)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam("TimeScale", newValue);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.auxTimeScaleStack.onChanged = null;
        }

        public override void PopHighspeedMovement()
        {
        }

        public override void PopMaterialGroup()
        {
        }

        public override void PopTimeScale(int stackIx)
        {
            this._timeScaleStack.Pop(stackIx);
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

        public override void RemovePersistentAnimatorBool(string key)
        {
        }

        public override void ResetTrigger(string name)
        {
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

        public override void SetPersistentAnimatorBool(string key, bool value)
        {
        }

        public override void SetTimeScale(float timescale, int stackIx)
        {
            this._timeScaleStack.Set(stackIx, timescale, false);
        }

        public override void SetTrigger(string name)
        {
        }

        public override void SteerFaceDirectionTo(Vector3 forward)
        {
        }

        public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
        {
        }

        public override void UnmaskAnimEvent(string animEventName)
        {
        }

        public override void UnmaskTrigger(string triggerID)
        {
        }

        public float AuxTimeScale
        {
            get
            {
                return this._auxTimeScaleStack.value;
            }
        }

        public FixedStack<float> auxTimeScaleStack
        {
            get
            {
                return this._auxTimeScaleStack;
            }
        }

        public override string CurrentSkillID
        {
            get
            {
                return null;
            }
        }

        public override float TimeScale
        {
            get
            {
                return this._timeScaleStack.value;
            }
        }

        public FixedStack<float> timeScaleStack
        {
            get
            {
                return this._timeScaleStack;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return Vector3.zero;
            }
        }
    }
}

