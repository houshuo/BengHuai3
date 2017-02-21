namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ModifyAttackData : ConfigAbilityMixin, IHashable
    {
        public bool NoTriggerEvadeAndDefend;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyAttackData(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.NoTriggerEvadeAndDefend, ref lastHash);
            if (this.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }
    }
}

