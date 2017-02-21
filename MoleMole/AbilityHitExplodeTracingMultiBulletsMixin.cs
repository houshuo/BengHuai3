namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityHitExplodeTracingMultiBulletsMixin : AbilityHitExplodeBulletMixin
    {
        private HitExplodeTracingMultiBulletsMixin config;

        public AbilityHitExplodeTracingMultiBulletsMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeTracingMultiBulletsMixin) config;
        }

        public override void Core()
        {
            base.Core();
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
            Vector3 forward;
            BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
            if ((attackTarget == null) || !this.config.FaceTarget)
            {
                forward = base.entity.transform.forward;
            }
            else
            {
                forward = attackTarget.GetAttachPoint("RootNode").position - bullet.triggerBullet.transform.position;
                Quaternion from = Quaternion.LookRotation(base.entity.transform.forward);
                Quaternion to = Quaternion.LookRotation(forward);
                forward = (Vector3) (Quaternion.RotateTowards(from, to, 15f) * Vector3.forward);
            }
            bullet.triggerBullet.transform.forward = forward;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar != null)
            {
                int count = localAvatar.SubAttackTargetList.Count;
                for (int i = 0; i < count; i++)
                {
                    string bulletTypeName = this.config.BulletTypeName;
                    HitExplodeTracingBulletMixinArgument abilityArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
                    if (abilityArgument != null)
                    {
                        if (abilityArgument.BulletName != null)
                        {
                            bulletTypeName = abilityArgument.BulletName;
                        }
                        if (abilityArgument.RandomBulletNames != null)
                        {
                            bulletTypeName = abilityArgument.RandomBulletNames[UnityEngine.Random.Range(0, abilityArgument.RandomBulletNames.Length)];
                        }
                    }
                    AbilityTriggerBullet bullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(bulletTypeName, base.actor, base.instancedAbility.Evaluate(this.config.BulletPostionLinearSpeed), this.config.Targetting, this.config.IgnoreTimeScale, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), -1f);
                    if ((this.config.BulletEffect != null) && (this.config.BulletEffect.EffectPattern != null))
                    {
                        Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(this.config.BulletEffect.EffectPattern, bullet.triggerBullet, this.config.BulletEffectGround);
                    }
                    base._bulletAttackDatas.Add(bullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(base.actor, this.config.HitAnimEventID));
                    float angle = (180 / (count + 1)) * (i + 1);
                    float num4 = base.instancedAbility.Evaluate(this.config.BulletPositionRadius);
                    float duration = base.instancedAbility.Evaluate(this.config.BulletPositionDuration);
                    Vector3 vector = Vector3.Cross(Vector3.up, base.entity.transform.forward);
                    vector = (Vector3) (Quaternion.AngleAxis(angle, base.entity.transform.forward) * vector);
                    Vector3 vector2 = (Vector3) (vector.normalized * num4);
                    Vector3 position = localAvatar.SubAttackTargetList[i].transform.position;
                    Vector3 up = Vector3.up;
                    position += up;
                    bullet.triggerBullet.SetupPositioning(bullet.triggerBullet.transform.position, bullet.triggerBullet.transform.position + vector2, duration, base.instancedAbility.Evaluate(this.config.BulletSpeed), this.config.TracingLerpCoef, this.config.TracingLerpCoefAcc, position, this.config.PassBy);
                    this.InitBulletForward(bullet);
                }
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
        }
    }
}

