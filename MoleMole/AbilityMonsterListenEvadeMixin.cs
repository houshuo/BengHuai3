namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityMonsterListenEvadeMixin : BaseAbilityMixin
    {
        private MonsterListenEvadeMixin config;

        public AbilityMonsterListenEvadeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterListenEvadeMixin) config;
            Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeSuccess>(base.actor.runtimeID);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtEvadeSuccess) && this.OnEvadeSuccess((EvtEvadeSuccess) evt));
        }

        public override void OnAdded()
        {
        }

        private bool OnEvadeSuccess(EvtEvadeSuccess evt)
        {
            if (evt.attackerID == base.entity.GetRuntimeID())
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.BeEvadeSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
            }
            return true;
        }

        public override void OnRemoved()
        {
        }
    }
}

