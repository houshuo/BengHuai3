namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DynamicDistanceMixin : ConfigAbilityMixin, IHashable
    {
        public float DefaultDistance;
        public float MaxDynamicDistanceDistance = 20f;
        public float MinDynamicDistanceDistance;
        public float NormalizedTimeStart;
        public float NormalizedTimeStop = 1f;
        public float NoTargetDistance;
        public string SkillID;
        public string TypeName;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDynamicDistanceMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.TypeName, ref lastHash);
            HashUtils.ContentHashOnto(this.DefaultDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.NoTargetDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.MinDynamicDistanceDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.MaxDynamicDistanceDistance, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStart, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStop, ref lastHash);
        }
    }
}

