namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class BanAvatarSkillButtonMixin : ConfigAbilityMixin, IHashable
    {
        public string ReplaceButtonIconPath;
        public string SkillID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityBanAvatarSkillButtonMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            HashUtils.ContentHashOnto(this.ReplaceButtonIconPath, ref lastHash);
        }
    }
}

