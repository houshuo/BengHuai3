namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarDodgeTeleportMixin : AbilityDodgeTeleportMixin
    {
        private AvatarDodgeTeleportMixin config;

        public AbilityAvatarDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarDodgeTeleportMixin) config;
        }

        protected override Vector3 GetDirection(TeleportDirectionMode mode, float angle)
        {
            if (mode != TeleportDirectionMode.UseSteerAngle)
            {
                return base.GetDirection(mode, angle);
            }
            AvatarControlData controlData = (base.actor.entity as BaseMonoAvatar).GetInputController().controlData;
            if (controlData.orderMove)
            {
                return controlData.steerDirection;
            }
            return -(base.entity as BaseMonoAvatar).FaceDirection;
        }

        protected override void OnPostBeingHit(EvtBeingHit evtHit)
        {
            base.OnPostBeingHit(evtHit);
            if ((this.config.DodgeMeleeAttack && this.config.CanHitTrigger) && ((evtHit.attackData.hitType == AttackResult.ActorHitType.Melee) && base.CheckAllowTeleport()))
            {
                base.IgnoreHitDamage(evtHit);
                this.TeleportBack(evtHit.sourceID);
            }
        }

        private void TeleportBack(uint sourceID)
        {
            Vector3 position;
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.TeleportActions, base.instancedAbility, base.instancedModifier, null, null);
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(sourceID);
            if (entity == null)
            {
                position = base.entity.transform.position;
            }
            Vector3 vector3 = base.entity.transform.position - entity.transform.position;
            Vector3 normalized = vector3.normalized;
            position = entity.transform.position + ((Vector3) (normalized * this.config.MeleeDistance));
            base.TeleportTo(position, this.config.TeleportTime);
            if (base.instancedAbility.Evaluate(this.config.CDTime) > 0f)
            {
                base._cdTimer = base.instancedAbility.Evaluate(this.config.CDTime);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(base.actor.runtimeID), MPEventDispatchMode.Normal);
        }
    }
}

