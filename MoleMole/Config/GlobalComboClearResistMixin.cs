namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class GlobalComboClearResistMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat ResumeTimeSpan;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityGlobalComboClearResistMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ResumeTimeSpan != null)
            {
                HashUtils.ContentHashOnto(this.ResumeTimeSpan.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ResumeTimeSpan.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ResumeTimeSpan.dynamicKey, ref lastHash);
            }
        }
    }
}

