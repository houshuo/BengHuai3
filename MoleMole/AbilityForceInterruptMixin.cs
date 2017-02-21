namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityForceInterruptMixin : BaseAbilityMixin
    {
        private State _state;
        private float _timer;
        private ForceInterruptMixin config;

        public AbilityForceInterruptMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ForceInterruptMixin) config;
        }

        public override void Core()
        {
            if (this._state == State.Running)
            {
                this._timer -= Time.deltaTime * base.entity.TimeScale;
                if (this._timer <= 0f)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.InterruptActions, base.instancedAbility, base.instancedModifier, null, null);
                    this._state = State.Idle;
                }
            }
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._state = State.Idle;
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < this.config.SkillIDs.Length; i++)
            {
                if (this.config.SkillIDs[i] == from)
                {
                    flag = true;
                }
                if (this.config.SkillIDs[i] == to)
                {
                    flag2 = true;
                }
            }
            if (!flag && flag2)
            {
                if (this._state == State.Idle)
                {
                    this._state = State.Running;
                    this._timer = this.config.TimeThreshold;
                }
            }
            else if (flag && !flag2)
            {
                this._state = State.Idle;
            }
        }

        private enum State
        {
            Idle,
            Running
        }
    }
}

