namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DefendMixin : ConfigAbilityMixin, IHashable
    {
        public string DefendDurationModifierName;
        public string DefendPerfectDurationModifierName;
        public DynamicFloat DefendPerfectEndTime = DynamicFloat.ONE;
        public DynamicFloat DefendPerfectStartTime = DynamicFloat.ZERO;
        public ConfigEntityAttackEffect DefendReplaceAttackEffect;
        public ConfigAbilityAction[] DefendStartActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] DefendSuccessActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] DefendSuccessPerfectActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat DefendWindow = DynamicFloat.ONE;

        public DefendMixin()
        {
            base.isUnique = true;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDefendMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.DefendStartActions, this.DefendSuccessActions, this.DefendSuccessPerfectActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DefendWindow != null)
            {
                HashUtils.ContentHashOnto(this.DefendWindow.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendWindow.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DefendWindow.dynamicKey, ref lastHash);
            }
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
            if (this.DefendSuccessActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.DefendSuccessActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.DefendSuccessPerfectActions != null)
            {
                foreach (ConfigAbilityAction action3 in this.DefendSuccessPerfectActions)
                {
                    if (action3 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
                    }
                }
            }
        }
    }
}

