namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToAnimatorBooleanMixin : BaseAbilityMixin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private bool _lastAnimatorBoolValue;
        public AttachModifierToAnimatorBooleanMixin config;

        public AbilityAttachModifierToAnimatorBooleanMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToAnimatorBooleanMixin) config;
            this._animatorEntity = (BaseMonoAnimatorEntity) base.entity;
        }

        public override void Core()
        {
            bool locomotionBool = this._animatorEntity.GetLocomotionBool(this.config.AnimatorBoolean);
            if (locomotionBool != this._lastAnimatorBoolValue)
            {
                if (!this.config.IsInvert ? locomotionBool : !locomotionBool)
                {
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
                }
                else
                {
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
                }
                this._lastAnimatorBoolValue = locomotionBool;
            }
        }

        public override void OnAdded()
        {
            this._lastAnimatorBoolValue = this._animatorEntity.GetLocomotionBool(this.config.AnimatorBoolean);
            if (this.config.IsInvert && !this._lastAnimatorBoolValue)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
        }

        public override void OnRemoved()
        {
        }
    }
}

