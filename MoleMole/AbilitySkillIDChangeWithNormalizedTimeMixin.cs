namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilitySkillIDChangeWithNormalizedTimeMixin : BaseAbilityMixin
    {
        private float _fromSkillNormalizedTime;
        private bool _startActionFinish;
        private SkillIDChangeWithNormalizedTimeMixin config;

        public AbilitySkillIDChangeWithNormalizedTimeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (SkillIDChangeWithNormalizedTimeMixin) config;
        }

        public override void Core()
        {
            if (base.entity.CurrentSkillID == this.config.SkillIDFrom)
            {
                this._fromSkillNormalizedTime = base.entity.GetCurrentNormalizedTime();
                if ((this._fromSkillNormalizedTime >= this.config.NormalizedTimeStart) && !this._startActionFinish)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.NormalizedTimeStartActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                    this._startActionFinish = true;
                }
            }
        }

        public override void OnAdded()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        public override void OnRemoved()
        {
            base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (to == this.config.SkillIDFrom)
            {
                this._startActionFinish = false;
            }
            if (((from == this.config.SkillIDFrom) && (to == this.config.SkillIDTo)) && ((this._fromSkillNormalizedTime < this.config.NormalizedTimeStop) && (this._fromSkillNormalizedTime > this.config.NormalizedTimeStart)))
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.SkillIDChangeActions, base.instancedAbility, base.instancedModifier, base.actor, null);
            }
        }
    }
}

