namespace MoleMole.Config
{
    using MoleMole;

    public class ModifyDamageByAttackeeMixin : ConfigAbilityMixin
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public DynamicFloat AddedDamageTakeRatio = DynamicFloat.ZERO;
        public ConfigAbilityPredicate[] Predicates = ConfigAbilityPredicate.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityModifyDamageByAttackeeMixin(instancedAbility, instancedModifier, this);
        }
    }
}

