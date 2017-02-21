namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SkillIDChangeWithNormalizedTimeMixin : ConfigAbilityMixin, IHashable
    {
        public float NormalizedTimeStart;
        public ConfigAbilityAction[] NormalizedTimeStartActions = ConfigAbilityAction.EMPTY;
        public float NormalizedTimeStop = 1f;
        public ConfigAbilityAction[] SkillIDChangeActions = ConfigAbilityAction.EMPTY;
        public string SkillIDFrom;
        public string SkillIDTo;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilitySkillIDChangeWithNormalizedTimeMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.NormalizedTimeStartActions, this.SkillIDChangeActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillIDFrom, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillIDTo, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStart, ref lastHash);
            HashUtils.ContentHashOnto(this.NormalizedTimeStop, ref lastHash);
            if (this.NormalizedTimeStartActions != null)
            {
                foreach (ConfigAbilityAction action in this.NormalizedTimeStartActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.SkillIDChangeActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.SkillIDChangeActions)
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

