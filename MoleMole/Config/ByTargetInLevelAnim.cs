namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ByTargetInLevelAnim : ConfigAbilityPredicate
    {
        public bool InLevelAnim;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByTargetInLevelAnimHandler(this, instancedAbility, instancedModifier, target, evt);
        }
    }
}

