namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterDefendMixin : ConfigAbilityMixin, IHashable
    {
        public bool AllowLayerLightControl;
        public DynamicFloat BreakDefendAniDamageRatio;
        public DynamicFloat DefendActionCD;
        public DynamicFloat DefendActionChance = DynamicFloat.ZERO;
        public DynamicFloat DefendActionRange;
        public ConfigAbilityAction[] DefendActionReadyActions;
        public DynamicFloat DefendAngle;
        public MixinEffect DefendEffect;
        public bool DefendMelee = true;
        public bool DefendRange = true;
        public string[] DefendSkillIDs;
        public ConfigAbilityAction[] DefendStartActions;
        public ConfigAbilityAction[] DefendSuccessActions;
        public MixinEffect DefendSuccessEffect;
        public string DefendTriggerID;
        public DynamicFloat InDefendMaxTime;
        public ConfigAbilityPredicate[] Predicates;
        public string[] ResetCDSkillIDs;
        public int ShieldLightLayer;
        public float ShieldLightMax;
        public float ShieldLightMin;

        public MonsterDefendMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 2f
            };
            this.BreakDefendAniDamageRatio = num;
            num = new DynamicFloat {
                fixedValue = 180f
            };
            this.DefendAngle = num;
            this.DefendActionRange = DynamicFloat.ONE;
            this.DefendActionCD = DynamicFloat.ZERO;
            this.InDefendMaxTime = DynamicFloat.ZERO;
            this.ShieldLightLayer = 3;
            this.ShieldLightMax = 1f;
            this.DefendStartActions = ConfigAbilityAction.EMPTY;
            this.DefendActionReadyActions = ConfigAbilityAction.EMPTY;
            this.DefendSuccessActions = ConfigAbilityAction.EMPTY;
            this.Predicates = ConfigAbilityPredicate.EMPTY;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterDefendMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DefendSkillIDs != null)
            {
                foreach (string str in this.DefendSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.DefendTriggerID, ref lastHash);
            if (this.DefendActionChance != null)
            {
                HashUtils.ContentHashOnto(this.DefendActionChance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionChance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionChance.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.DefendMelee, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendRange, ref lastHash);
            if (this.BreakDefendAniDamageRatio != null)
            {
                HashUtils.ContentHashOnto(this.BreakDefendAniDamageRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BreakDefendAniDamageRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BreakDefendAniDamageRatio.dynamicKey, ref lastHash);
            }
            if (this.DefendAngle != null)
            {
                HashUtils.ContentHashOnto(this.DefendAngle.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendAngle.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendAngle.dynamicKey, ref lastHash);
            }
            if (this.DefendActionRange != null)
            {
                HashUtils.ContentHashOnto(this.DefendActionRange.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionRange.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionRange.dynamicKey, ref lastHash);
            }
            if (this.DefendEffect != null)
            {
                HashUtils.ContentHashOnto(this.DefendEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendEffect.AudioPattern, ref lastHash);
            }
            if (this.DefendSuccessEffect != null)
            {
                HashUtils.ContentHashOnto(this.DefendSuccessEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendSuccessEffect.AudioPattern, ref lastHash);
            }
            if (this.DefendActionCD != null)
            {
                HashUtils.ContentHashOnto(this.DefendActionCD.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionCD.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendActionCD.dynamicKey, ref lastHash);
            }
            if (this.InDefendMaxTime != null)
            {
                HashUtils.ContentHashOnto(this.InDefendMaxTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.InDefendMaxTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.InDefendMaxTime.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.AllowLayerLightControl, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldLightLayer, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldLightMax, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldLightMin, ref lastHash);
            if (this.ResetCDSkillIDs != null)
            {
                foreach (string str2 in this.ResetCDSkillIDs)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
            if (this.DefendStartActions != null)
            {
                foreach (ConfigAbilityAction action in this.DefendStartActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.DefendActionReadyActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.DefendActionReadyActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.DefendSuccessActions != null)
            {
                foreach (ConfigAbilityAction action3 in this.DefendSuccessActions)
                {
                    if (action3 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
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

