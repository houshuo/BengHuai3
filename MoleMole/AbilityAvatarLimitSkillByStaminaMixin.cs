namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarLimitSkillByStaminaMixin : BaseAbilityMixin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private AvatarActor _avatarActor;
        protected string _currentSkillID;
        private bool _isSkilling;
        private bool _maskSkill;
        private float _stamina;
        private DisplayValue<float> _staminaDisplay;
        private AvatarLimitSkillByStaminaMixin config;

        public AbilityAvatarLimitSkillByStaminaMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarLimitSkillByStaminaMixin) config;
            this._animatorEntity = (BaseMonoAnimatorEntity) base.entity;
            this._avatarActor = (AvatarActor) base.actor;
        }

        public override void Core()
        {
            base.Core();
            if ((this._stamina < this.config.StaminaMax) && !this._isSkilling)
            {
                DelegateUtils.UpdateField<float>(ref this._stamina, Mathf.Min(this._stamina + ((base.entity.TimeScale * Time.deltaTime) * this.config.ResumeSpeed), this.config.StaminaMax), new Action<float, float>(this.UpdateStaminaDisplayValue));
            }
            if ((this._stamina < this.config.SkillHeatCost) && !this._maskSkill)
            {
                this._avatarActor.entity.MaskTrigger(this.config.MaskTriggerID);
                this._maskSkill = true;
            }
            if ((this._stamina >= this.config.SkillHeatCost) && this._maskSkill)
            {
                this._avatarActor.entity.UnmaskTrigger(this.config.MaskTriggerID);
                this._maskSkill = false;
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._stamina = this.config.StaminaMax;
        }

        public override void OnAdded()
        {
            this._animatorEntity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._animatorEntity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            if (this.config.ShowStaminaBar)
            {
                this._staminaDisplay = base.actor.abilityPlugin.CreateOrGetDisplayFloat("Stamina", 0f, 1f, 1f);
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this.config.SkillID == to)
            {
                DelegateUtils.UpdateField<float>(ref this._stamina, this._stamina - this.config.SkillHeatCost, new Action<float, float>(this.UpdateStaminaDisplayValue));
                this._isSkilling = true;
            }
            if (this.config.SkillID == from)
            {
                this._isSkilling = false;
            }
        }

        private void UpdateStaminaDisplayValue(float fromStamina, float toStamina)
        {
            if (this._staminaDisplay != null)
            {
                this._staminaDisplay.Pub(Mathf.Clamp01(toStamina / this.config.StaminaMax));
            }
        }
    }
}

