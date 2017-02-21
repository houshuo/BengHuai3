namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityDamageByAttackerDamageMixin : BaseAbilityMixin
    {
        private ConfigEntityAttackProperty _attackProperty;
        public DamageByAttackerDamageMixin config;

        public AbilityDamageByAttackerDamageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._attackProperty = new ConfigEntityAttackProperty();
            this.config = (DamageByAttackerDamageMixin) config;
        }

        public override void OnAdded()
        {
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.PostBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
        }

        private bool PostBeingHit(EvtBeingHit evt)
        {
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
            {
                return false;
            }
            if (evt.attackData.rejected)
            {
                return false;
            }
            float num = evt.attackData.damage * base.instancedAbility.Evaluate(this.config.DamagePercentage);
            this._attackProperty.AddedDamageValue = num;
            this._attackProperty.DamagePercentage = 0f;
            this._attackProperty.AniDamageRatio = 0f;
            this._attackProperty.FrameHalt = 0;
            this._attackProperty.HitType = AttackResult.ActorHitType.Ailment;
            this._attackProperty.HitEffect = AttackResult.AnimatorHitEffect.Normal;
            this._attackProperty.RetreatVelocity = 0f;
            this._attackProperty.IsAnimEventAttack = true;
            this._attackProperty.IsInComboCount = false;
            bool forceSkipAttackerResolve = !base.actor.IsActive();
            AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(base.actor, this._attackProperty, null, null);
            AttackPattern.SendHitEvent(base.actor.runtimeID, evt.sourceID, null, null, attackData, forceSkipAttackerResolve, MPEventDispatchMode.Normal);
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            return true;
        }
    }
}

