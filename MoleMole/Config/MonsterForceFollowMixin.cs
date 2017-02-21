namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterForceFollowMixin : ConfigAbilityMixin, IHashable
    {
        public float FollowSpeed = 10f;
        public float MaxDistance = 100f;
        public float MinDistance;
        public float NormalizeTimeEnd = 1f;
        public string[] SkillIDs;
        public float TargetDistance = 1.5f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterForceFollowMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SkillIDs != null)
            {
                foreach (string str in this.SkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.NormalizeTimeEnd, ref lastHash);
            HashUtils.ContentHashOnto(this.TargetDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.MinDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.MaxDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.FollowSpeed, ref lastHash);
        }
    }
}

