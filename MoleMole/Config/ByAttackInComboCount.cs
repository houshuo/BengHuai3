namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ByAttackInComboCount : ConfigAbilityPredicate
    {
        public bool InComboCount;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByAttackInComboCountHandler(this, instancedAbility, instancedModifier, target, evt);
        }
    }
}

