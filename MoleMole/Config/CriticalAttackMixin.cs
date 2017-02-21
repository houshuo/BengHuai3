namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class CriticalAttackMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] OnCriticalAttackActions = ConfigAbilityAction.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityCriticalAttackMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.OnCriticalAttackActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.OnCriticalAttackActions != null)
            {
                foreach (ConfigAbilityAction action in this.OnCriticalAttackActions)
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

