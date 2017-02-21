namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToPartialSkillIDMixin : BaseAbilityMixin
    {
        private ActorModifier _attachedModifier;
        private State _oldState;
        private State _state;
        private AttachModifierToPartialSkillIDMixin config;

        public AbilityAttachModifierToPartialSkillIDMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToPartialSkillIDMixin) config;
        }

        public override void Core()
        {
            if (this._oldState != this._state)
            {
                this._oldState = this._state;
            }
            if (this._state == State.InSkill)
            {
                if (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStart)
                {
                    this.Transit(State.InPartial);
                }
            }
            else if (this._state == State.InPartial)
            {
                if (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStop)
                {
                    this.Transit(State.OutPartial);
                }
            }
            else if (this._state == State.OutPartial)
            {
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
            if (this._state == State.Idle)
            {
                if (to == this.config.SkillID)
                {
                    if (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStart)
                    {
                        this.Transit(State.InPartial);
                    }
                    else
                    {
                        this.Transit(State.InSkill);
                    }
                }
            }
            else if (this._state == State.InSkill)
            {
                if (to == this.config.SkillID)
                {
                    if (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStart)
                    {
                        this.Transit(State.InPartial);
                    }
                }
                else
                {
                    this.Transit(State.Idle);
                }
            }
            else if (this._state == State.InPartial)
            {
                if (to != this.config.SkillID)
                {
                    this.Transit(State.Idle);
                }
            }
            else if (this._state == State.OutPartial)
            {
                if (to != this.config.SkillID)
                {
                    this.Transit(State.Idle);
                }
                else if (to == this.config.SkillID)
                {
                    if (base.entity.GetCurrentNormalizedTime() > this.config.NormalizedTimeStart)
                    {
                        this.Transit(State.InPartial);
                    }
                    else
                    {
                        this.Transit(State.InSkill);
                    }
                }
            }
        }

        private void Transit(State nextState)
        {
            if (nextState == State.InPartial)
            {
                this._attachedModifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
            else if (this._state == State.InPartial)
            {
                base.actor.abilityPlugin.TryRemoveModifier(this._attachedModifier);
                this._attachedModifier = null;
            }
            this._state = nextState;
        }

        private enum State
        {
            Idle,
            InSkill,
            InPartial,
            OutPartial
        }
    }
}

