namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarSwitchRoleMixin : BaseAbilityMixin
    {
        private AvatarSwitchRoleMixin config;

        public AbilityAvatarSwitchRoleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSwitchRoleMixin) config;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtAvatarSwapInEnd)
            {
                return this.OnSwitchInEnd((EvtAvatarSwapInEnd) evt);
            }
            return ((evt is EvtAvatarSwapOutStart) && this.OnSwitchOutStart((EvtAvatarSwapOutStart) evt));
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(base.actor.runtimeID);
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(base.actor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(base.actor.runtimeID);
        }

        private bool OnSwitchInEnd(EvtAvatarSwapInEnd evt)
        {
            if (base.actor.runtimeID == evt.targetID)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.SwitchInActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
            }
            return true;
        }

        private bool OnSwitchOutStart(EvtAvatarSwapOutStart evt)
        {
            if (base.actor.runtimeID == evt.targetID)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.SwitchOutActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt);
            }
            return true;
        }
    }
}

