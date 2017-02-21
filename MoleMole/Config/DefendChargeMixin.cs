namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DefendChargeMixin : ConfigAbilityMixin, IHashable
    {
        public float DefendASNormalizedEndTime;
        public float DefendBSNormalizedStartTime;
        public string DefendBSSkillID;
        public string DefendDurationModifierName;
        public string DefendLoopSkillID;
        public string DefendPerfectDurationModifierName;
        public DynamicFloat DefendPerfectEndTime = DynamicFloat.ZERO;
        public DynamicFloat DefendPerfectStartTime = DynamicFloat.ZERO;
        public ConfigEntityAttackEffect DefendReplaceAttackEffect;
        public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] DefendSuccessPerfectActions = ConfigAbilityAction.EMPTY;
        public string DefnedASSkillID;

        public DefendChargeMixin()
        {
            base.isUnique = true;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDefendChargeMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.DefendSuccessActions, this.DefendSuccessPerfectActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.DefendBSSkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendLoopSkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.DefnedASSkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendBSNormalizedStartTime, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendASNormalizedEndTime, ref lastHash);
            if (this.DefendPerfectStartTime != null)
            {
                HashUtils.ContentHashOnto(this.DefendPerfectStartTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendPerfectStartTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendPerfectStartTime.dynamicKey, ref lastHash);
            }
            if (this.DefendPerfectEndTime != null)
            {
                HashUtils.ContentHashOnto(this.DefendPerfectEndTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendPerfectEndTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendPerfectEndTime.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.DefendDurationModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.DefendPerfectDurationModifierName, ref lastHash);
            if (this.DefendReplaceAttackEffect != null)
            {
                HashUtils.ContentHashOnto(this.DefendReplaceAttackEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendReplaceAttackEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendReplaceAttackEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.DefendReplaceAttackEffect.AttackEffectTriggerPos, ref lastHash);
            }
            if (this.DefendSuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.DefendSuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.DefendSuccessPerfectActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.DefendSuccessPerfectActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
        }
    }
}

