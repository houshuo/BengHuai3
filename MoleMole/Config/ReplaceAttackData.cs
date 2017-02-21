namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ReplaceAttackData : ConfigAbilityAction, IHashable
    {
        public float AddAttackeeAniDefenceRatio;
        public float AttackerAniDamageRatio;
        public int FrameHalt;
        public bool ReplaceAttackerAniDamageRatio;
        public bool ReplaceFrameHalt;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ReplaceAttackDataHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.FrameHalt, ref lastHash);
            HashUtils.ContentHashOnto(this.ReplaceFrameHalt, ref lastHash);
            HashUtils.ContentHashOnto(this.AttackerAniDamageRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ReplaceAttackerAniDamageRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.AddAttackeeAniDefenceRatio, ref lastHash);
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

