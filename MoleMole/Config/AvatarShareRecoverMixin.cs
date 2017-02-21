namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarShareRecoverMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;
        public bool ShareHP;
        public DynamicFloat ShareHPRatio = DynamicFloat.ONE;
        public bool ShareSP;
        public DynamicFloat ShareSPRatio = DynamicFloat.ONE;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarShareRecoverMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.ShareSP, ref lastHash);
            if (this.ShareSPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShareSPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShareSPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShareSPRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ShareHP, ref lastHash);
            if (this.ShareHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShareHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShareHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShareHPRatio.dynamicKey, ref lastHash);
            }
            if (this.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }
    }
}

