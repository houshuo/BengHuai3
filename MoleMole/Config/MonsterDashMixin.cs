namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class MonsterDashMixin : ConfigAbilityMixin
    {
        public float DashTime = 0.1f;
        public float MaxDistance = 100f;
        public float MinDistance;
        public string[] SkillIDs;
        public float TargetDistance = 2.5f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterDashMixin(instancedAbility, instancedModifier, this);
        }
    }
}

