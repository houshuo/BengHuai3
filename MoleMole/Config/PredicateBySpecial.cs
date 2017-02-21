namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class PredicateBySpecial : BaseUtilityAction, IHashable
    {
        public ConfigAbilityAction[] Actions = ConfigAbilityAction.EMPTY;
        public EntityNature SameNature;
        public SpecialType Special;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.PredicateBySpecialHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.Actions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.Special, ref lastHash);
            HashUtils.ContentHashOnto((int) this.SameNature, ref lastHash);
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
            HashUtils.ContentHashOnto((int) base.Target, ref lastHash);
            if ((base.TargetOption != null) && (base.TargetOption.Range != null))
            {
                HashUtils.ContentHashOnto(base.TargetOption.Range.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.dynamicKey, ref lastHash);
            }
            if (base.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }

        public enum SpecialType
        {
            IsEveryAvatarHasDifferenctClass,
            IsEveryAvatarHasDifferenctRoleName,
            IsEveryAvatarHasDifferenctNature,
            IsEveryAvatarHasSameNature
        }
    }
}

