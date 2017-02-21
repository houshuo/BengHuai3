namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAttachAdditiveVelocityMixin : BaseAbilityMixin
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private AttachAdditiveVelocityMixin config;

        public AbilityAttachAdditiveVelocityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachAdditiveVelocityMixin) config;
            this._animatorEntity = base.entity as BaseMonoAnimatorEntity;
        }

        public override void OnAdded()
        {
            this.SetVelocity();
        }

        public override void OnRemoved()
        {
            this.ResetVelocity();
        }

        private void ResetVelocity()
        {
            this._animatorEntity.SetHasAdditiveVelocity(false);
            this._animatorEntity.SetAdditiveVelocity(Vector3.zero);
            this._animatorEntity.PopHighspeedMovement();
        }

        private void SetVelocity()
        {
            Vector3 velocity = (Vector3) (this.config.MoveSpeed * this._animatorEntity.FaceDirection);
            this._animatorEntity.SetHasAdditiveVelocity(true);
            this._animatorEntity.SetAdditiveVelocity(velocity);
            this._animatorEntity.PushHighspeedMovement();
        }
    }
}

