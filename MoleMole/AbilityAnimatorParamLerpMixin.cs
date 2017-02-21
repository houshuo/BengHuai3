namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAnimatorParamLerpMixin : BaseAbilityMixin
    {
        private bool _isInLerping;
        private float _lerpEndValue;
        private float _lerpStartValue;
        private AnimatorParamLerpMixin config;

        public AbilityAnimatorParamLerpMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AnimatorParamLerpMixin) config;
            this._isInLerping = false;
        }

        public override void Core()
        {
            if (this._isInLerping)
            {
                float t = (Mathf.Clamp((base.actor.entity as BaseMonoAnimatorEntity).GetCurrentNormalizedTime(), this.config.LerpStartNormalizedTime, this.config.LerpEndNormalizedTime) - this.config.LerpStartNormalizedTime) / (this.config.LerpEndNormalizedTime - this.config.LerpStartNormalizedTime);
                float num4 = Mathf.Lerp(this._lerpStartValue, this._lerpEndValue, t);
                this.SetAnimatorParam(num4);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (this.config.LerpStartValue > 0f)
            {
                this._lerpStartValue = this.config.LerpStartValue;
            }
            else
            {
                this._lerpStartValue = (base.actor.entity as BaseMonoAnimatorEntity).GetLocomotionFloat(this.config.AnimatorParamName);
            }
            this._lerpEndValue = this.config.LerpEndValue;
            this._isInLerping = true;
        }

        public override void OnAdded()
        {
            BaseMonoAnimatorEntity entity = base.actor.entity as BaseMonoAnimatorEntity;
            entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(entity.onCurrentSkillIDChanged, new Action<string, string>(this.OnSkillIDChanged));
        }

        public override void OnRemoved()
        {
            BaseMonoAnimatorEntity entity = base.actor.entity as BaseMonoAnimatorEntity;
            entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(entity.onCurrentSkillIDChanged, new Action<string, string>(this.OnSkillIDChanged));
        }

        private void OnSkillIDChanged(string skillOld, string skillNew)
        {
            if (this._isInLerping)
            {
                this._isInLerping = false;
            }
        }

        private void SetAnimatorParam(float value)
        {
            (base.actor.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(this.config.AnimatorParamName, value, false);
        }
    }
}

