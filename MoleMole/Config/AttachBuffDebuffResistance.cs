namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachBuffDebuffResistance : ConfigAbilityAction, IHashable
    {
        public AbilityState[] ResistanceBuffDebuffs;
        public float ResistanceDurationRatio;
        public float ResistanceRatio;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.AttachBuffDebufResistanceHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ResistanceBuffDebuffs != null)
            {
                foreach (AbilityState state in this.ResistanceBuffDebuffs)
                {
                    HashUtils.ContentHashOnto((int) state, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ResistanceRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ResistanceDurationRatio, ref lastHash);
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
    }
}

