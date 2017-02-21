namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterListenEvadeMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] BeEvadeSuccessActions = ConfigAbilityAction.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterListenEvadeMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.BeEvadeSuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.BeEvadeSuccessActions)
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

