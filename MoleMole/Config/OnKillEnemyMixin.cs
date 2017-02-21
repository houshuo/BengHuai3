namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class OnKillEnemyMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityOnKillEnemyMixin(instancedAbility, instancedModifier, this);
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
        }
    }
}

