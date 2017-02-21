namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityLimitLoopTransitionMixin : BaseAbilityMixin
    {
        private int _countLeft;
        private int _maxCount;
        private LimitLoopTransitionMixin config;

        public AbilityLimitLoopTransitionMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (LimitLoopTransitionMixin) config;
            this._maxCount = instancedAbility.Evaluate(this.config.LoopLimitCount);
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._countLeft = this._maxCount = Mathf.FloorToInt((float) evt.abilityArgument);
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            this._countLeft = base.instancedAbility.Evaluate(this.config.LoopLimitCount);
            base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            base.entity.RemovePersistentAnimatorBool(this.config.AllowLoopBoolID);
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (to == this.config.SkillID)
            {
                this._countLeft--;
                if (this._countLeft <= 0)
                {
                    base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, false);
                }
            }
            else if ((from == this.config.SkillID) && (to != this.config.SkillID))
            {
                base.entity.SetPersistentAnimatorBool(this.config.AllowLoopBoolID, true);
                this._countLeft = this._maxCount;
            }
        }
    }
}

