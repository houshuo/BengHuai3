namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarSkillButtonClickedMixin : ConfigAbilityMixin, IHashable
    {
        public bool ConsumeClick;
        public ConfigAbilityAction[] OnClickedActions = ConfigAbilityAction.EMPTY;
        public string SkillButtonID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarSkillButtonClickedMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillButtonID, ref lastHash);
            if (this.OnClickedActions != null)
            {
                foreach (ConfigAbilityAction action in this.OnClickedActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            HashUtils.ContentHashOnto(this.ConsumeClick, ref lastHash);
        }
    }
}

