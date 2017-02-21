namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityRangeAttackProtectShieldMixin : BaseAbilityMixin
    {
        private float _damageReduceRatio;
        private float _protectRange;
        private RangeAttackProtectShieldMixin config;

        public AbilityRangeAttackProtectShieldMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (RangeAttackProtectShieldMixin) config;
            this._damageReduceRatio = instancedAbility.Evaluate(this.config.DamageReduceRatio);
            this._protectRange = instancedAbility.Evaluate(this.config.ProtectRange);
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
            if ((actor != null) && (Vector3.Distance(actor.entity.transform.position, base.actor.entity.transform.position) > this._protectRange))
            {
                evt.attackData.damage *= 1f - this._damageReduceRatio;
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnRangeAttackProtectShieldSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }
    }
}

