namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarTiedMixin : BaseAbilityMixin
    {
        private float _accumulatedSteerAmount;
        private int _animatorMoveStackIx;
        private BaseMonoAvatar _avatar;
        private Vector3 _lastSteer;
        private AvatarTiedMixin config;

        public AbilityAvatarTiedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarTiedMixin) config;
            this._avatar = (BaseMonoAvatar) base.actor.entity;
        }

        public override void Core()
        {
            AvatarControlData activeControlData = this._avatar.GetActiveControlData();
            if (activeControlData.hasAnyControl)
            {
                Vector3 steerDirection = activeControlData.steerDirection;
                Vector3 vector2 = steerDirection - this._lastSteer;
                this._accumulatedSteerAmount += vector2.sqrMagnitude;
                this._lastSteer = steerDirection;
                if (this._accumulatedSteerAmount > base.instancedAbility.Evaluate(this.config.UntieSteerAmount))
                {
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedModifier);
                }
            }
        }

        public override void OnAdded()
        {
            this._animatorMoveStackIx = this._avatar.PushProperty("Animator_RigidBodyVelocityRatio", -1000f);
            this._lastSteer = Vector3.zero;
            this._accumulatedSteerAmount = 0f;
        }

        public override void OnRemoved()
        {
            this._avatar.PopProperty("Animator_RigidBodyVelocityRatio", this._animatorMoveStackIx);
        }
    }
}

