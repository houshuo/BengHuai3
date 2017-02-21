namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DefendWithShieldMixin : DefendWithDirectionMixin, IHashable
    {
        public float ShieldAniDamageRatioPow = 1f;
        public ConfigAbilityAction[] ShieldBrokenActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat ShieldHPRatio;
        public string ShieldRatioAnimatorParam;
        public float ShieldResumeSpeedByRatio;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDefendWithShieldMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.ShieldBrokenActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ShieldHPRatio != null)
            {
                HashUtils.ContentHashOnto(this.ShieldHPRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ShieldHPRatio.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ShieldResumeSpeedByRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldAniDamageRatioPow, ref lastHash);
            HashUtils.ContentHashOnto(this.ShieldRatioAnimatorParam, ref lastHash);
            if (this.ShieldBrokenActions != null)
            {
                foreach (ConfigAbilityAction action in this.ShieldBrokenActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (base.DefendSkillIDs != null)
            {
                foreach (string str in base.DefendSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(base.AlwaysDefend, ref lastHash);
            HashUtils.ContentHashOnto(base.DefendNormalizedTimeStart, ref lastHash);
            HashUtils.ContentHashOnto(base.DefendNormalizedTimeStop, ref lastHash);
            HashUtils.ContentHashOnto(base.DefendAngle, ref lastHash);
            HashUtils.ContentHashOnto(base.ReverseAngle, ref lastHash);
            HashUtils.ContentHashOnto(base.BreakDefendAniDamageRatio, ref lastHash);
            HashUtils.ContentHashOnto(base.DefendDamageReduce, ref lastHash);
            HashUtils.ContentHashOnto(base.DefendElemental, ref lastHash);
            HashUtils.ContentHashOnto((int) base.DefendSuccessHitEffect, ref lastHash);
            if (base.DefendSuccessActions != null)
            {
                foreach (ConfigAbilityAction action2 in base.DefendSuccessActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (base.DefendFailActions != null)
            {
                foreach (ConfigAbilityAction action3 in base.DefendFailActions)
                {
                    if (action3 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
                    }
                }
            }
            if (base.DefendPredicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.DefendPredicates)
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

