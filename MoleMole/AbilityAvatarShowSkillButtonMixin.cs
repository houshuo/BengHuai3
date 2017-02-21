namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarShowSkillButtonMixin : BaseAbilityMixin
    {
        private AvatarActor _avatarActor;
        private bool _removedFromMask;
        private AvatarShowSkillButtonMixin config;

        public AbilityAvatarShowSkillButtonMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarShowSkillButtonMixin) config;
            this._avatarActor = (AvatarActor) base.actor;
        }

        public override void OnAdded()
        {
            if (this._avatarActor.maskedSkillButtons.Contains(this.config.SkillButtonID))
            {
                this._avatarActor.maskedSkillButtons.Remove(this.config.SkillButtonID);
                this._removedFromMask = true;
            }
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.runtimeID))
            {
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID).gameObject.SetActive(true);
                if (this.config.SkillButtonID == "SKL02")
                {
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSPBar().gameObject.SetActive(true);
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.RefreshSPView((float) this._avatarActor.SP, (float) this._avatarActor.SP, 0f);
                }
            }
        }

        public override void OnRemoved()
        {
            if (this._removedFromMask && Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.runtimeID))
            {
                this._avatarActor.maskedSkillButtons.Add(this.config.SkillButtonID);
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID).gameObject.SetActive(false);
                if (this.config.SkillButtonID == "SKL02")
                {
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSPBar().gameObject.SetActive(false);
                }
            }
        }
    }
}

