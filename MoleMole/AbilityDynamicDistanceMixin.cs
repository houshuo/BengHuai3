namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityDynamicDistanceMixin : BaseAbilityMixin
    {
        private int _animatorMoveStackIx;
        private BaseMonoAbilityEntity _entity;
        private State _state;
        private DynamicDistanceMixin config;

        public AbilityDynamicDistanceMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DynamicDistanceMixin) config;
            this._entity = base.entity;
        }

        private bool CheckNormalizedTime()
        {
            float currentNormalizedTime = this.GetCurrentNormalizedTime();
            return ((currentNormalizedTime >= this.config.NormalizedTimeStart) && (currentNormalizedTime <= this.config.NormalizedTimeStop));
        }

        public override void Core()
        {
            if (this._state == State.Wait)
            {
                if (this.GetCurrentNormalizedTime() >= this.config.NormalizedTimeStart)
                {
                    this.SetDynamicSpeed();
                }
            }
            else if (((this._state == State.Runing) && (this.config.NormalizedTimeStop < 1f)) && (this.GetCurrentNormalizedTime() > this.config.NormalizedTimeStop))
            {
                this.ResetDynamicSpeed();
            }
        }

        private float GetCurrentNormalizedTime()
        {
            return ((base.entity.GetCurrentNormalizedTime() <= 1f) ? base.entity.GetCurrentNormalizedTime() : 1f);
        }

        public override void OnAdded()
        {
            this._state = State.Idle;
            if (this.config.SkillID != null)
            {
                this._entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            }
            else
            {
                this.SetDynamicSpeed();
            }
        }

        public override void OnRemoved()
        {
            if (this.config.SkillID != null)
            {
                this._entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            }
            this.ResetDynamicSpeed();
        }

        private void ResetDynamicSpeed()
        {
            this._state = State.End;
            if (this._animatorMoveStackIx != 0)
            {
                base.entity.PopHighspeedMovement();
                this._entity.PopProperty("Animator_RigidBodyVelocityRatio", this._animatorMoveStackIx);
                this._animatorMoveStackIx = 0;
            }
        }

        private void SetDynamicSpeed()
        {
            if (this.CheckNormalizedTime())
            {
                if (this._entity.GetAttackTarget() != null)
                {
                    float maxDynamicDistanceDistance = Vector3.Distance(this._entity.GetAttackTarget().XZPosition, this._entity.XZPosition);
                    if (maxDynamicDistanceDistance > this.config.MaxDynamicDistanceDistance)
                    {
                        maxDynamicDistanceDistance = this.config.MaxDynamicDistanceDistance;
                    }
                    if (maxDynamicDistanceDistance < this.config.MinDynamicDistanceDistance)
                    {
                        maxDynamicDistanceDistance = this.config.MinDynamicDistanceDistance;
                    }
                    this._animatorMoveStackIx = this._entity.PushProperty("Animator_RigidBodyVelocityRatio", (maxDynamicDistanceDistance / this.config.DefaultDistance) - 1f);
                    base.entity.PushHighspeedMovement();
                }
                else if (this.config.NoTargetDistance != 0f)
                {
                    this._animatorMoveStackIx = this._entity.PushProperty("Animator_RigidBodyVelocityRatio", (this.config.NoTargetDistance / this.config.DefaultDistance) - 1f);
                    base.entity.PushHighspeedMovement();
                }
                else
                {
                    this._animatorMoveStackIx = 0;
                }
                this._state = State.Runing;
            }
            else if (this.GetCurrentNormalizedTime() < this.config.NormalizedTimeStart)
            {
                this._state = State.Wait;
            }
            else
            {
                this._state = State.End;
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if ((from != this.config.SkillID) && (to == this.config.SkillID))
            {
                this._state = State.Idle;
                this.SetDynamicSpeed();
            }
            else if ((from == this.config.SkillID) && (to != this.config.SkillID))
            {
                this.ResetDynamicSpeed();
            }
        }

        private enum State
        {
            Idle,
            Wait,
            Runing,
            End
        }
    }
}

