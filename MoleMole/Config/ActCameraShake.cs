namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ActCameraShake : ConfigAbilityAction
    {
        public ConfigEntityCameraShake CameraShake;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.CameraShakeHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }
    }
}

