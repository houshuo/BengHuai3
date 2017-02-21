namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DefendModeMonsterMixin : ConfigAbilityMixin, IHashable
    {
        public int DefaultAIValue;
        public DynamicFloat HatredAddRateByDamage;
        public DynamicFloat HatredAddThreholdRatioByDamage;
        public float[] hatredAIAreaSections;
        public int[] hatredAIValues;
        public DynamicFloat HatredDecreaseInterval;
        public DynamicFloat HatredDecreateRateByInterval;
        public DynamicFloat MaxHatred = DynamicFloat.ONE;
        public DynamicFloat MinAISwitchDuration;
        public DynamicFloat TestNumber = DynamicFloat.ZERO;

        public DefendModeMonsterMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 0.2f
            };
            this.HatredAddRateByDamage = num;
            num = new DynamicFloat {
                fixedValue = 0.05f
            };
            this.HatredAddThreholdRatioByDamage = num;
            this.HatredDecreaseInterval = DynamicFloat.ONE;
            num = new DynamicFloat {
                fixedValue = 0.05f
            };
            this.HatredDecreateRateByInterval = num;
            this.hatredAIAreaSections = new float[] { 0.5f };
            this.hatredAIValues = new int[] { 2, 3 };
            this.DefaultAIValue = 2;
            num = new DynamicFloat {
                fixedValue = 0.5f
            };
            this.MinAISwitchDuration = num;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDefendModeMonsterMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.TestNumber != null)
            {
                HashUtils.ContentHashOnto(this.TestNumber.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.TestNumber.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.TestNumber.dynamicKey, ref lastHash);
            }
            if (this.MaxHatred != null)
            {
                HashUtils.ContentHashOnto(this.MaxHatred.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxHatred.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MaxHatred.dynamicKey, ref lastHash);
            }
            if (this.HatredAddRateByDamage != null)
            {
                HashUtils.ContentHashOnto(this.HatredAddRateByDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredAddRateByDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredAddRateByDamage.dynamicKey, ref lastHash);
            }
            if (this.HatredAddThreholdRatioByDamage != null)
            {
                HashUtils.ContentHashOnto(this.HatredAddThreholdRatioByDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredAddThreholdRatioByDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredAddThreholdRatioByDamage.dynamicKey, ref lastHash);
            }
            if (this.HatredDecreaseInterval != null)
            {
                HashUtils.ContentHashOnto(this.HatredDecreaseInterval.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredDecreaseInterval.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredDecreaseInterval.dynamicKey, ref lastHash);
            }
            if (this.HatredDecreateRateByInterval != null)
            {
                HashUtils.ContentHashOnto(this.HatredDecreateRateByInterval.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredDecreateRateByInterval.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HatredDecreateRateByInterval.dynamicKey, ref lastHash);
            }
            if (this.hatredAIAreaSections != null)
            {
                foreach (float num in this.hatredAIAreaSections)
                {
                    HashUtils.ContentHashOnto(num, ref lastHash);
                }
            }
            if (this.hatredAIValues != null)
            {
                foreach (int num3 in this.hatredAIValues)
                {
                    HashUtils.ContentHashOnto(num3, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.DefaultAIValue, ref lastHash);
            if (this.MinAISwitchDuration != null)
            {
                HashUtils.ContentHashOnto(this.MinAISwitchDuration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MinAISwitchDuration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MinAISwitchDuration.dynamicKey, ref lastHash);
            }
        }
    }
}

