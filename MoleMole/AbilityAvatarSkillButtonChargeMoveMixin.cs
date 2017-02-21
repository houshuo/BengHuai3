namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityAvatarSkillButtonChargeMoveMixin : BaseAbilityMixin
    {
        private BaseMonoAvatar _avatar;
        private HashSet<string> _chargeLoopSkillIDs;
        private bool _isSteer;
        private Vector3 _moveDirection;
        private float _moveSpeed;
        private float _smoothTimer;
        private State _state;
        private float _steerSpeed;
        public AvatarSkillButtonHoldChargeMoveMixin config;
        private bool hasSteerBefore;
        private bool inStoping;
        private const float SMOOTH_LENGTH = 1f;

        public AbilityAvatarSkillButtonChargeMoveMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._moveDirection = Vector3.zero;
            this.config = (AvatarSkillButtonHoldChargeMoveMixin) config;
            this._avatar = (BaseMonoAvatar) base.entity;
            this._chargeLoopSkillIDs = new HashSet<string>(this.config.ChargeLoopSkillIDs);
            this._moveSpeed = instancedAbility.Evaluate(this.config.MoveSpeed);
            this._isSteer = this.config.IsSteer;
            this._steerSpeed = this.config.SteerSpeed;
        }

        public override void Core()
        {
            if (this._state == State.InLoop)
            {
                AvatarControlData activeControlData = this._avatar.GetActiveControlData();
                if (activeControlData.hasSteer)
                {
                    if (this._isSteer)
                    {
                        Vector3 steerDirection = activeControlData.steerDirection;
                        this._avatar.SteerFaceDirectionTo(Vector3.Lerp(this._avatar.FaceDirection, steerDirection, this._steerSpeed * Time.deltaTime));
                    }
                    else
                    {
                        this._avatar.SetHasAdditiveVelocity(true);
                        this._avatar.SetAdditiveVelocity((Vector3) (activeControlData.steerDirection * this._moveSpeed));
                    }
                    this._moveDirection = activeControlData.steerDirection;
                    this.hasSteerBefore = true;
                    this.inStoping = false;
                }
                else if (!this._isSteer)
                {
                    if (this.hasSteerBefore)
                    {
                        this.hasSteerBefore = false;
                        this.inStoping = true;
                        this._smoothTimer = 0f;
                    }
                    if (this.inStoping)
                    {
                        this._smoothTimer += Time.deltaTime * base.entity.TimeScale;
                        this._avatar.SetHasAdditiveVelocity(true);
                        this._avatar.SetAdditiveVelocity((Vector3) (this._moveDirection * Mathf.Lerp(0f, this._moveSpeed, 1f - (this._smoothTimer / 1f))));
                        if (this._smoothTimer > 1f)
                        {
                            this.inStoping = false;
                            this._smoothTimer = 0f;
                        }
                    }
                }
            }
        }

        public override void OnAdded()
        {
            this._state = State.Idle;
            this._avatar.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._avatar.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        public override void OnRemoved()
        {
            this._avatar.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._avatar.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._avatar.SetHasAdditiveVelocity(false);
            this._avatar.SetAdditiveVelocity(Vector3.zero);
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this._chargeLoopSkillIDs.Contains(to))
            {
                this._state = State.InLoop;
            }
            if ((this._state == State.InLoop) && !this._chargeLoopSkillIDs.Contains(to))
            {
                this._state = State.After;
                this._avatar.SetHasAdditiveVelocity(false);
                this._avatar.SetAdditiveVelocity(Vector3.zero);
                this._moveDirection = Vector3.zero;
            }
        }

        private enum State
        {
            Idle,
            InLoop,
            After
        }
    }
}

