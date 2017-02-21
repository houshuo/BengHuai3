namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ShootPalsyBomb : ConfigAbilityAction, IHashable
    {
        public string AttachPoint;
        public string BombAttackID;
        public float BombSpeed;
        public DynamicString PropName;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.ShootPalsyBombHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.PropName != null)
            {
                HashUtils.ContentHashOnto(this.PropName.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PropName.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PropName.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.BombSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.BombAttackID, ref lastHash);
            HashUtils.ContentHashOnto(this.AttachPoint, ref lastHash);
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

