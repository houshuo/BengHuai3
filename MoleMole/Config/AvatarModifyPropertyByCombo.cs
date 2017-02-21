namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarModifyPropertyByCombo : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Initial;
        public DynamicFloat MaxValue;
        public DynamicInt MaxValueCombo;
        public DynamicFloat MinValue = DynamicFloat.ZERO;
        public DynamicFloat PerComboDelta;
        public string Property;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarModifyPropertyByComboMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Initial != null)
            {
                HashUtils.ContentHashOnto(this.Initial.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Initial.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Initial.dynamicKey, ref lastHash);
            }
            if (this.MinValue != null)
            {
                HashUtils.ContentHashOnto(this.MinValue.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MinValue.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MinValue.dynamicKey, ref lastHash);
            }
            if (this.MaxValue != null)
            {
                HashUtils.ContentHashOnto(this.MaxValue.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxValue.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxValue.dynamicKey, ref lastHash);
            }
            if (this.MaxValueCombo != null)
            {
                HashUtils.ContentHashOnto(this.MaxValueCombo.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxValueCombo.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxValueCombo.dynamicKey, ref lastHash);
            }
            if (this.PerComboDelta != null)
            {
                HashUtils.ContentHashOnto(this.PerComboDelta.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PerComboDelta.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PerComboDelta.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.Property, ref lastHash);
        }
    }
}

