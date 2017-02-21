namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarQTEMixin : ConfigAbilityMixin, IHashable
    {
        public QTECondition[] Conditions;
        public DynamicFloat DelayQTETimeSpan;
        public string ModifierName;
        public ConfigAbilityPredicate[] Predicates;
        public DynamicFloat QTEMaxTimeSpan = DynamicFloat.ONE;
        public string QTEName;
        public string[] SkillIDs;
        public QTECondition[] TriggerConditions;

        public AvatarQTEMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 0.2f
            };
            this.DelayQTETimeSpan = num;
            this.TriggerConditions = QTECondition.EMPTY;
            this.Conditions = QTECondition.EMPTY;
            this.Predicates = ConfigAbilityPredicate.EMPTY;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarQTEMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SkillIDs != null)
            {
                foreach (string str in this.SkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.QTEName, ref lastHash);
            if (this.QTEMaxTimeSpan != null)
            {
                HashUtils.ContentHashOnto(this.QTEMaxTimeSpan.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.QTEMaxTimeSpan.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.QTEMaxTimeSpan.dynamicKey, ref lastHash);
            }
            if (this.DelayQTETimeSpan != null)
            {
                HashUtils.ContentHashOnto(this.DelayQTETimeSpan.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DelayQTETimeSpan.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DelayQTETimeSpan.dynamicKey, ref lastHash);
            }
            if (this.TriggerConditions != null)
            {
                foreach (QTECondition condition in this.TriggerConditions)
                {
                    HashUtils.ContentHashOnto(condition.QTERange, ref lastHash);
                    HashUtils.ContentHashOnto((int) condition.QTEType, ref lastHash);
                    if (condition.QTEValues != null)
                    {
                        foreach (string str2 in condition.QTEValues)
                        {
                            HashUtils.ContentHashOnto(str2, ref lastHash);
                        }
                    }
                }
            }
            if (this.Conditions != null)
            {
                foreach (QTECondition condition2 in this.Conditions)
                {
                    HashUtils.ContentHashOnto(condition2.QTERange, ref lastHash);
                    HashUtils.ContentHashOnto((int) condition2.QTEType, ref lastHash);
                    if (condition2.QTEValues != null)
                    {
                        foreach (string str3 in condition2.QTEValues)
                        {
                            HashUtils.ContentHashOnto(str3, ref lastHash);
                        }
                    }
                }
            }
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

