namespace MoleMole.Config
{
    using MoleMole;

    public class DebugMixin : ConfigAbilityMixin
    {
        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDebugMixin(instancedAbility, instancedModifier, this);
        }
    }
}

