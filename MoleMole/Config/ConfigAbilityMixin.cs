namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [CheckForHashable]
    public abstract class ConfigAbilityMixin : BaseActionContainer
    {
        public static ConfigAbilityMixin[] EMPTY = new ConfigAbilityMixin[0];
        [NonSerialized]
        public bool isUnique;

        protected ConfigAbilityMixin()
        {
        }

        public abstract BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier);
        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return ConfigAbilityAction.EMPTY_SUBS;
        }

        public virtual BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return null;
        }
    }
}

