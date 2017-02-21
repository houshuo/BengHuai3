namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityOnKillEnemyMixin : BaseAbilityMixin
    {
        private OnKillEnemyMixin config;

        public AbilityOnKillEnemyMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (OnKillEnemyMixin) config;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.OnKilled((EvtKilled) evt));
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base.actor.runtimeID);
        }

        private bool OnKilled(EvtKilled evt)
        {
            if (evt.killerID == base.actor.runtimeID)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.Actions, base.instancedAbility, base.instancedModifier, null, null);
            }
            return true;
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base.actor.runtimeID);
        }
    }
}

