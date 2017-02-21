namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MirrorMixin : ConfigAbilityMixin, IHashable
    {
        public float AheadTime;
        public bool ApplyAttackerWitchTimeRatio;
        public float DelayTime;
        public DynamicFloat HPRatioOfParent = DynamicFloat.ONE;
        public string[] MirrorAbilities;
        public string MirrorAbilitiesOverrideName;
        public ConfigAbilityAction[] MirrorAheadDestroyActions = ConfigAbilityAction.EMPTY;
        public string MirrorAIName;
        public DynamicInt MirrorAmount;
        public ConfigAbilityAction[] MirrorCreateActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] MirrorDestroyActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat MirrorLastingTime;
        public float PerMirrorDelayTime;
        public int RemoveMirrorAfterSkillCount;
        public string[] SelfModifiers = Miscs.EMPTY_STRINGS;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMirrorMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.MirrorCreateActions, this.MirrorDestroyActions, this.MirrorAheadDestroyActions };
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.MirrorLastingTime != null)
            {
                HashUtils.ContentHashOnto(this.MirrorLastingTime.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MirrorLastingTime.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MirrorLastingTime.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ApplyAttackerWitchTimeRatio, ref lastHash);
            if (this.MirrorAmount != null)
            {
                HashUtils.ContentHashOnto(this.MirrorAmount.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MirrorAmount.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MirrorAmount.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.AheadTime, ref lastHash);
            HashUtils.ContentHashOnto(this.DelayTime, ref lastHash);
            HashUtils.ContentHashOnto(this.PerMirrorDelayTime, ref lastHash);
            if (this.HPRatioOfParent != null)
            {
                HashUtils.ContentHashOnto(this.HPRatioOfParent.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HPRatioOfParent.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HPRatioOfParent.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.MirrorAIName, ref lastHash);
            if (this.MirrorAbilities != null)
            {
                foreach (string str in this.MirrorAbilities)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.MirrorAbilitiesOverrideName, ref lastHash);
            HashUtils.ContentHashOnto(this.RemoveMirrorAfterSkillCount, ref lastHash);
            if (this.SelfModifiers != null)
            {
                foreach (string str2 in this.SelfModifiers)
                {
                    HashUtils.ContentHashOnto(str2, ref lastHash);
                }
            }
            if (this.MirrorCreateActions != null)
            {
                foreach (ConfigAbilityAction action in this.MirrorCreateActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.MirrorDestroyActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.MirrorDestroyActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.MirrorAheadDestroyActions != null)
            {
                foreach (ConfigAbilityAction action3 in this.MirrorAheadDestroyActions)
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

