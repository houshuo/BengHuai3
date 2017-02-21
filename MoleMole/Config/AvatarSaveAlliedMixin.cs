namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarSaveAlliedMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] AdditionalActions = ConfigAbilityAction.EMPTY;
        public DynamicInt SaveCountLimit = DynamicInt.ONE;

        public AvatarSaveAlliedMixin()
        {
            base.isUnique = true;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarSaveAlliedMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SaveCountLimit != null)
            {
                HashUtils.ContentHashOnto(this.SaveCountLimit.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.SaveCountLimit.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.SaveCountLimit.dynamicKey, ref lastHash);
            }
            if (this.AdditionalActions != null)
            {
                foreach (ConfigAbilityAction action in this.AdditionalActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
        }
    }
}

