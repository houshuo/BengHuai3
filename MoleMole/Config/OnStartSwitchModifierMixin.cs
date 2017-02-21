namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class OnStartSwitchModifierMixin : ConfigAbilityMixin, IHashable
    {
        public bool AlwaysSwitchOn;
        public float MaxDuration = -1f;
        public string OffModifierName;
        public string OnModifierInstantTriggerEvent;
        public string OnModifierName;
        public int OnModifierReplaceCostSP;
        public string OnModifierReplaceIconPath;
        public bool OnModifierSwitchToInstantTrigger;
        public string SkillButtonID;
        public bool UseLowHPForceOff = true;
        public bool UseLowSPForceOff;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityStartSwitchModifierMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.OnModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.OffModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.UseLowSPForceOff, ref lastHash);
            HashUtils.ContentHashOnto(this.UseLowHPForceOff, ref lastHash);
            HashUtils.ContentHashOnto(this.AlwaysSwitchOn, ref lastHash);
            HashUtils.ContentHashOnto(this.MaxDuration, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillButtonID, ref lastHash);
            HashUtils.ContentHashOnto(this.OnModifierReplaceIconPath, ref lastHash);
            HashUtils.ContentHashOnto(this.OnModifierReplaceCostSP, ref lastHash);
            HashUtils.ContentHashOnto(this.OnModifierSwitchToInstantTrigger, ref lastHash);
            HashUtils.ContentHashOnto(this.OnModifierInstantTriggerEvent, ref lastHash);
        }
    }
}

