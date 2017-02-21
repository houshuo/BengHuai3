namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class TriggerAnimEvent : ConfigAbilityAction
    {
        public string AnimEventID;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.TriggerAnimEventHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }
    }
}

