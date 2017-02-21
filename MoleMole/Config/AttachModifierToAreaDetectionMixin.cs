namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToAreaDetectionMixin : ConfigAbilityMixin, IHashable
    {
        public float Delay = 1f;
        public bool IsInvert;
        public string[] ModifierNames;
        public DynamicFloat Radius = DynamicFloat.ZERO;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToAreaDetectionMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Radius != null)
            {
                HashUtils.ContentHashOnto(this.Radius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Radius.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.Delay, ref lastHash);
            HashUtils.ContentHashOnto(this.IsInvert, ref lastHash);
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

