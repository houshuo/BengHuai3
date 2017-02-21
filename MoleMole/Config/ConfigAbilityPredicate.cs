namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [CheckForHashable]
    public abstract class ConfigAbilityPredicate
    {
        public static ConfigAbilityPredicate[] EMPTY = new ConfigAbilityPredicate[0];

        protected ConfigAbilityPredicate()
        {
        }

        public abstract bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt);
    }
}

