namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class BlockMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityPredicate[] AttackerPredicates = ConfigAbilityPredicate.EMPTY;
        public ConfigAbilityAction[] BlockActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat BlockChance = DynamicFloat.ZERO;
        public string[] BlockSkillIDs;
        public DynamicFloat BlockTimer = DynamicFloat.ZERO;
        public DynamicFloat DamageReduce = DynamicFloat.ZERO;
        public DynamicFloat DamageReduceRatio = DynamicFloat.ZERO;
        public ConfigAbilityPredicate[] TargetPredicates = ConfigAbilityPredicate.EMPTY;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityBlockMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.BlockActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DamageReduce != null)
            {
                HashUtils.ContentHashOnto(this.DamageReduce.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduce.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduce.dynamicKey, ref lastHash);
            }
            if (this.BlockChance != null)
            {
                HashUtils.ContentHashOnto(this.BlockChance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BlockChance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BlockChance.dynamicKey, ref lastHash);
            }
            if (this.DamageReduceRatio != null)
            {
                HashUtils.ContentHashOnto(this.DamageReduceRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduceRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamageReduceRatio.dynamicKey, ref lastHash);
            }
            if (this.BlockTimer != null)
            {
                HashUtils.ContentHashOnto(this.BlockTimer.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BlockTimer.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BlockTimer.dynamicKey, ref lastHash);
            }
            if (this.BlockActions != null)
            {
                foreach (ConfigAbilityAction action in this.BlockActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.TargetPredicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in this.TargetPredicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
            if (this.AttackerPredicates != null)
            {
                foreach (ConfigAbilityPredicate predicate2 in this.AttackerPredicates)
                {
                    if (predicate2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate2, ref lastHash);
                    }
                }
            }
            if (this.BlockSkillIDs != null)
            {
                foreach (string str in this.BlockSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
        }
    }
}

