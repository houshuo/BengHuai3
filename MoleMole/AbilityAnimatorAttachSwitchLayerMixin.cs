namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAnimatorAttachSwitchLayerMixin : BaseAbilityMixin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        public AnimatorAttachSwitchLayerMixin config;

        public AbilityAnimatorAttachSwitchLayerMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AnimatorAttachSwitchLayerMixin) config;
            this._animatorEntity = (BaseMonoAnimatorEntity) base.entity;
        }

        public override void OnAdded()
        {
            this._animatorEntity.StartFadeAnimatorLayerWeight(this.config.FromLayer, 0f, this.config.Duration);
            this._animatorEntity.StartFadeAnimatorLayerWeight(this.config.ToLayer, 1f, this.config.Duration);
        }

        public override void OnRemoved()
        {
            if (!this.config.NoEndResume)
            {
                this._animatorEntity.StartFadeAnimatorLayerWeight(this.config.FromLayer, 1f, this.config.Duration);
                this._animatorEntity.StartFadeAnimatorLayerWeight(this.config.ToLayer, 0f, this.config.Duration);
            }
        }
    }
}

