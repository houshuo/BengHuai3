namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ByAttackHitFlag : ConfigAbilityPredicate
    {
        public AttackResult.ActorHitFlag HitFlag;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByAttackHitFlagHandler(this, instancedAbility, instancedModifier, target, evt);
        }
    }
}

