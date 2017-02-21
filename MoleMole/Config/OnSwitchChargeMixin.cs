namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class OnSwitchChargeMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public string[] AfterSkillIDs;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityOnSwitchChargeMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.Actions != null)
            {
                foreach (ConfigAbilityAction action in this.Actions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.AfterSkillIDs != null)
            {
                foreach (string str in this.AfterSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
        }
    }
}

