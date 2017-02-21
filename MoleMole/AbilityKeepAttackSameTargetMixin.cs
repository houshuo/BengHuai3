namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityKeepAttackSameTargetMixin : BaseAbilityMixin
    {
        private EntityTimer _fadeTimer;
        private uint _lastTargetID;
        private KeepAttackSameTargetMixin config;

        public AbilityKeepAttackSameTargetMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (KeepAttackSameTargetMixin) config;
            this._fadeTimer = new EntityTimer(instancedAbility.Evaluate(this.config.TargetFadeWindow));
        }

        public override void Core()
        {
            this._fadeTimer.Core(1f);
            if (this._fadeTimer.isTimeUp)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnTargetFadeOrChanged, base.instancedAbility, base.instancedModifier, null, null);
                this._fadeTimer.Reset(false);
            }
        }

        public override void OnAdded()
        {
            this._fadeTimer.Reset(false);
            this._lastTargetID = 0;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOther((EvtHittingOther) evt));
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            if (!string.IsNullOrEmpty(evt.animEventID))
            {
                if (evt.toID != this._lastTargetID)
                {
                    if (this._lastTargetID != 0)
                    {
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnTargetFadeOrChanged, base.instancedAbility, base.instancedModifier, null, null);
                    }
                    this._lastTargetID = evt.toID;
                }
                else
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.OnAttackSameTarget, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.toID), evt);
                    this._fadeTimer.Reset(true);
                }
            }
            return true;
        }
    }
}

