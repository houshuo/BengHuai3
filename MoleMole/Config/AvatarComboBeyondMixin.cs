namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarComboBeyondMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicInt[] ComboSteps;
        public string[] ModifierNames;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarComboBeyondMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ModifierNames != null)
            {
                foreach (string str in this.ModifierNames)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.ComboSteps != null)
            {
                foreach (DynamicInt num2 in this.ComboSteps)
                {
                    HashUtils.ContentHashOnto(num2.isDynamic, ref lastHash);
                    HashUtils.ContentHashOnto(num2.fixedValue, ref lastHash);
                    HashUtils.ContentHashOnto(num2.dynamicKey, ref lastHash);
                }
            }
        }
    }
}

