namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarSkillButtonClickedMixin : BaseAbilityMixin
    {
        private MonoSkillButton _skillButton;
        private AvatarSkillButtonClickedMixin config;

        public AbilityAvatarSkillButtonClickedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSkillButtonClickedMixin) config;
        }

        public override void OnAdded()
        {
            this._skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID);
            this._skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>) Delegate.Combine(this._skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(this.OnSkillButtonClicked));
        }

        public override void OnRemoved()
        {
            this._skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>) Delegate.Remove(this._skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(this.OnSkillButtonClicked));
        }

        private bool OnSkillButtonClicked(MonoSkillButton.PointerState pointerState)
        {
            if (pointerState == MonoSkillButton.PointerState.PointerUp)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnClickedActions, base.instancedAbility, base.instancedModifier, null, null);
            }
            return !this.config.ConsumeClick;
        }
    }
}

