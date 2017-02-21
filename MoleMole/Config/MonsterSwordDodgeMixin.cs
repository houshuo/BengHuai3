namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterSwordDodgeMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] DodgeActions = ConfigAbilityAction.EMPTY;
        public float DodgeRatio;
        public float NoDodgeAttackRatio;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterSwordDodgeMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.DodgeRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.NoDodgeAttackRatio, ref lastHash);
            if (this.DodgeActions != null)
            {
                foreach (ConfigAbilityAction action in this.DodgeActions)
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

