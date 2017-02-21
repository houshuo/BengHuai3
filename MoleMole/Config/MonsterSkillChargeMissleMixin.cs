namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class MonsterSkillChargeMissleMixin : MonsterSkillIDChargeAnimatorMixin
    {
        public string AbilityName;
        public string AbilityNameSub;
        public int[] ChargeMissleAmount;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterSkillChargeMissleMixin(instancedAbility, instancedModifier, this);
        }
    }
}

