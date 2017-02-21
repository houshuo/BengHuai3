namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityReflectBulletMixin : BaseAbilityMixin
    {
        public ReflectBulletMixin config;

        public AbilityReflectBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ReflectBulletMixin) config;
        }

        public override void OnAdded()
        {
            base.actor.AddAbilityState(AbilityState.ReflectBullet, false);
        }

        private bool OnBulletRefected(EvtAfterBulletReflected evt)
        {
            AbilityTriggerBullet bullet = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(evt.bulletID);
            MonoTriggerBullet triggerBullet = bullet.triggerBullet;
            bullet.Setup(base.actor, triggerBullet.speed, MixinTargetting.All, triggerBullet.IgnoreTimeScale, triggerBullet.AliveDuration);
            evt.attackData.attackerAttackValue *= base.instancedAbility.Evaluate(this.config.DamageRatio);
            Vector3 position = Singleton<EventManager>.Instance.GetEntity(evt.launcherID).GetAttachPoint("RootNode").position;
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.launcherID);
            if (Vector3.Angle(base.instancedAbility.caster.entity.transform.forward, actor.entity.transform.position - base.instancedAbility.caster.entity.transform.position) < this.config.Angle)
            {
                if (this.config.IsReflectToLauncher)
                {
                    triggerBullet.SetupTracing(position, triggerBullet._traceLerpCoef, triggerBullet._traceLerpCoefAcc, false);
                    triggerBullet.transform.forward = -triggerBullet.transform.forward;
                }
                else
                {
                    position.y += UnityEngine.Random.Range((float) 1f, (float) 3f);
                    Vector3 rhs = position - base.entity.XZPosition;
                    float sqrMagnitude = rhs.sqrMagnitude;
                    rhs.y = 0f;
                    Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                    if (Vector3.Dot(onUnitSphere, rhs) < 0f)
                    {
                        onUnitSphere = -onUnitSphere;
                    }
                    onUnitSphere.y = Mathf.Abs(onUnitSphere.y);
                    triggerBullet.transform.forward = onUnitSphere;
                    triggerBullet.SetupTracing((Vector3) ((onUnitSphere.normalized * sqrMagnitude) * 0.8f), triggerBullet._traceLerpCoef, triggerBullet._traceLerpCoefAcc, false);
                }
                if (this.config.ResetAliveDuration)
                {
                    triggerBullet.AliveDuration = this.config.NewAliveDuration;
                }
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.ReflectSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.launcherID), evt);
            }
            else
            {
                EvtBulletHit hit = new EvtBulletHit(triggerBullet.GetRuntimeID(), base.actor.runtimeID) {
                    ownerID = base.actor.runtimeID,
                    cannotBeReflected = true
                };
                Vector3 vector4 = triggerBullet.transform.position - ((Vector3) ((Time.deltaTime * triggerBullet.BulletTimeScale) * triggerBullet.transform.GetComponent<Rigidbody>().velocity));
                AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                    hitPoint = vector4,
                    hitDir = triggerBullet.transform.forward
                };
                hit.hitCollision = collsion;
                Singleton<EventManager>.Instance.FireEvent(hit, MPEventDispatchMode.Normal);
            }
            return false;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtAfterBulletReflected) && this.OnBulletRefected((EvtAfterBulletReflected) evt));
        }

        public override void OnRemoved()
        {
            base.actor.RemoveAbilityState(AbilityState.ReflectBullet);
        }
    }
}

