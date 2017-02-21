namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ActCameraExposure : ConfigAbilityAction
    {
        public float ExposureTime;
        public float KeepTime;
        public float MaxExposure;
        public float RecoverTime;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.CameraExposureHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }
    }
}

