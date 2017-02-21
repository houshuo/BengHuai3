namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarLimitSkillByStaminaMixin : ConfigAbilityMixin, IHashable
    {
        public string MaskTriggerID;
        public float ResumeSpeed;
        public bool ShowStaminaBar;
        public float SkillHeatCost;
        public string SkillID;
        public float StaminaMax;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarLimitSkillByStaminaMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.StaminaMax, ref lastHash);
            HashUtils.ContentHashOnto(this.ResumeSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.MaskTriggerID, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillHeatCost, ref lastHash);
            HashUtils.ContentHashOnto(this.ShowStaminaBar, ref lastHash);
        }
    }
}

