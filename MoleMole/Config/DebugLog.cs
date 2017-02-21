namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class DebugLog : ConfigAbilityAction
    {
        public string Content;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.DebugLogHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }
    }
}

