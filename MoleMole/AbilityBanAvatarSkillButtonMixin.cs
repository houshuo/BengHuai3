namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityBanAvatarSkillButtonMixin : BaseAbilityMixin
    {
        private BanAvatarSkillButtonMixin config;

        public AbilityBanAvatarSkillButtonMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (BanAvatarSkillButtonMixin) config;
        }

        public override void OnAdded()
        {
            AvatarActor actor = base.actor as AvatarActor;
            if (actor.GetSkillInfo(this.config.SkillID) != null)
            {
                actor.GetSkillInfo(this.config.SkillID).muted = true;
                actor.GetSkillInfo(this.config.SkillID).maskIconPath = this.config.ReplaceButtonIconPath;
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.entity.GetRuntimeID()))
                {
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillID).RefreshSkillInfo();
                }
            }
        }

        public override void OnRemoved()
        {
            AvatarActor actor = base.actor as AvatarActor;
            if (actor.GetSkillInfo(this.config.SkillID) != null)
            {
                actor.GetSkillInfo(this.config.SkillID).muted = false;
                actor.GetSkillInfo(this.config.SkillID).maskIconPath = null;
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.entity.GetRuntimeID()))
                {
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillID).RefreshSkillInfo();
                }
            }
        }
    }
}

