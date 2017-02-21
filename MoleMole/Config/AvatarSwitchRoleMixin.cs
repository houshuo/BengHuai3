namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarSwitchRoleMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] SwitchInActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] SwitchOutActions = ConfigAbilityAction.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarSwitchRoleMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SwitchInActions != null)
            {
                foreach (ConfigAbilityAction action in this.SwitchInActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.SwitchOutActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.SwitchOutActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
        }
    }
}

