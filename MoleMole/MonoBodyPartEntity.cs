namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoBodyPartEntity : BaseMonoAbilityEntity
    {
        public Collider hitbox;
        public bool IsCameraTargetable = true;

        public override int AddAdditiveVelocity(Vector3 velocity)
        {
            return -1;
        }

        public override void AddAnimEventPredicate(string predicate)
        {
            this.owner.AddAnimEventPredicate(predicate);
        }

        public override bool ContainAnimEventPredicate(string predicate)
        {
            return this.owner.ContainAnimEventPredicate(predicate);
        }

        public override void FireEffect(string patternName)
        {
            this.owner.FireEffect(patternName);
        }

        public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
        {
            this.owner.FireEffect(patternName, initPos, initDir);
        }

        public override void FireEffectTo(string patternName, BaseMonoEntity to)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, this.XZPosition, base.transform.forward, Vector3.one, this, to);
        }

        public override Transform GetAttachPoint(string name)
        {
            return base.transform;
        }

        public override BaseMonoEntity GetAttackTarget()
        {
            return this.owner.GetAttackTarget();
        }

        public override float GetCurrentNormalizedTime()
        {
            return this.owner.GetCurrentNormalizedTime();
        }

        public override float GetProperty(string propertyKey)
        {
            return 0f;
        }

        public override float GetPropertyByStackIndex(string propertyKey, int stackIx)
        {
            return 0f;
        }

        public override bool HasAdditiveVelocityOfIndex(int index)
        {
            return false;
        }

        public void Init(uint runtimeID, BaseMonoAnimatorEntity owner)
        {
            base.commonConfig = owner.commonConfig;
            base.Init(runtimeID);
            this.owner = owner;
        }

        public override bool IsActive()
        {
            return this.owner.IsActive();
        }

        public override bool IsToBeRemove()
        {
            return this.owner.IsToBeRemove();
        }

        public override void MaskAnimEvent(string animEventName)
        {
            this.owner.MaskAnimEvent(animEventName);
        }

        public override void MaskTrigger(string triggerID)
        {
            this.owner.MaskTrigger(triggerID);
        }

        public override void PopHighspeedMovement()
        {
            this.owner.PopHighspeedMovement();
        }

        public override void PopMaterialGroup()
        {
            this.owner.PopMaterialGroup();
        }

        public override void PopProperty(string propertyKey, int stackIx)
        {
        }

        public override void PopTimeScale(int stackIx)
        {
            this.owner.PopTimeScale(stackIx);
        }

        public override void PushHighspeedMovement()
        {
            this.owner.PushHighspeedMovement();
        }

        public override void PushMaterialGroup(string targetGroupname)
        {
            this.owner.PushMaterialGroup(targetGroupname);
        }

        public override int PushProperty(string propertyKey, float value)
        {
            return 0;
        }

        public override void PushTimeScale(float timescale, int stackIx)
        {
            this.owner.PushTimeScale(timescale, stackIx);
        }

        public override void RemoveAnimEventPredicate(string predicate)
        {
            this.owner.RemoveAnimEventPredicate(predicate);
        }

        public override void ResetTrigger(string name)
        {
            this.owner.ResetTrigger(name);
        }

        public override void SetAdditiveVelocity(Vector3 velocity)
        {
            this.owner.SetAdditiveVelocity(velocity);
        }

        public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
        {
        }

        public override void SetAttackTarget(BaseMonoEntity attackTarget)
        {
            this.owner.SetAttackTarget(attackTarget);
        }

        public override void SetDied(KillEffect killEffect)
        {
            this.hitbox.enabled = false;
        }

        public override void SetHasAdditiveVelocity(bool hasAdditiveVelocity)
        {
            this.owner.SetHasAdditiveVelocity(hasAdditiveVelocity);
        }

        public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
        {
            this.owner.SetNeedOverrideVelocity(needOverrideVelocity);
        }

        public override void SetOverrideVelocity(Vector3 velocity)
        {
            this.owner.SetOverrideVelocity(velocity);
        }

        public override void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
        {
        }

        public override void SetTimeScale(float timescale, int stackIx)
        {
            this.owner.SetTimeScale(timescale, stackIx);
        }

        public override void SetTrigger(string name)
        {
            this.owner.SetTrigger(name);
        }

        public override void SteerFaceDirectionTo(Vector3 forward)
        {
            this.owner.SteerFaceDirectionTo(forward);
        }

        public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
        {
            this.owner.TriggerAttackPattern(animEventID, layerMask);
        }

        public override void UnmaskAnimEvent(string animEventName)
        {
            this.owner.UnmaskAnimEvent(animEventName);
        }

        public override void UnmaskTrigger(string triggerID)
        {
            this.owner.UnmaskTrigger(triggerID);
        }

        public override string CurrentSkillID
        {
            get
            {
                return this.owner.CurrentSkillID;
            }
        }

        public BaseMonoAnimatorEntity owner { get; set; }

        public override float TimeScale
        {
            get
            {
                return this.owner.TimeScale;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
            }
        }
    }
}

