namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterBlurDistanceBeyondMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Distance = DynamicFloat.ZERO;
        public string[] ModifierNames;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterBlurDistanceBeyondMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Distance != null)
            {
                HashUtils.ContentHashOnto(this.Distance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Distance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Distance.dynamicKey, ref lastHash);
            }
            if (this.ModifierNames != null)
            {
                foreach (string str in this.ModifierNames)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
        }
    }
}

