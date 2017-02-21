namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ForceInterruptMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] InterruptActions = ConfigAbilityAction.EMPTY;
        public string[] SkillIDs;
        public float TimeThreshold;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityForceInterruptMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.InterruptActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SkillIDs != null)
            {
                foreach (string str in this.SkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.TimeThreshold, ref lastHash);
            if (this.InterruptActions != null)
            {
                foreach (ConfigAbilityAction action in this.InterruptActions)
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

