namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterAreaLastingBuffMixin : ConfigAbilityMixin, IHashable
    {
        public string BuffDurationEndTrigger;
        public string BuffTimeRatioAnimatorParam;
        public DynamicFloat Duration = DynamicFloat.ZERO;
        public AreaLastingHitBreakType HitBreakType;
        public bool IncludeSelf = true;
        public string[] ModifierNames;
        public DynamicFloat Radius = DynamicFloat.ONE;
        public string SelfLastingModifierName;
        public MixinTargetting Target = MixinTargetting.Allied;
        public bool TriggerOnAdded;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterAreaLastingBuffMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Radius != null)
            {
                HashUtils.ContentHashOnto(this.Radius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.dynamicKey, ref lastHash);
            }
            if (this.Duration != null)
            {
                HashUtils.ContentHashOnto(this.Duration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Duration.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto((int) this.Target, ref lastHash);
            HashUtils.ContentHashOnto(this.BuffDurationEndTrigger, ref lastHash);
            HashUtils.ContentHashOnto(this.TriggerOnAdded, ref lastHash);
            HashUtils.ContentHashOnto((int) this.HitBreakType, ref lastHash);
            HashUtils.ContentHashOnto(this.IncludeSelf, ref lastHash);
            HashUtils.ContentHashOnto(this.SelfLastingModifierName, ref lastHash);
            if (this.ModifierNames != null)
            {
                foreach (string str in this.ModifierNames)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.BuffTimeRatioAnimatorParam, ref lastHash);
        }

        public enum AreaLastingHitBreakType
        {
            Normal,
            ConvertAllHitsToLightHit,
            BreakingHitCancels
        }
    }
}

