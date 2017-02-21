namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class LimitTimeWithMaskTriggerMixin : ConfigAbilityMixin
    {
        public float CountTime;
        public DynamicInt EvadeLimitCount;
        public float MaskDuration;
        public string MaskTriggerID;
        public string SkillID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityLimitWithMaskTriggerMixin(instancedAbility, instancedModifier, this);
        }
    }
}

