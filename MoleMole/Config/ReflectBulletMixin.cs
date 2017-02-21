namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ReflectBulletMixin : ConfigAbilityMixin
    {
        public float Angle = 30f;
        public DynamicFloat DamageRatio;
        public bool IsReflectToLauncher;
        public float NewAliveDuration;
        public ConfigAbilityAction[] ReflectSuccessActions = ConfigAbilityAction.EMPTY;
        public bool ResetAliveDuration;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityReflectBulletMixin(instancedAbility, instancedModifier, this);
        }
    }
}

