namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToLevelBuffMixin : BaseAbilityMixin
    {
        private AttachModifierToLevelBuffMixin config;

        public AbilityAttachModifierToLevelBuffMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToLevelBuffMixin) config;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtLevelBuffState) && this.ListenLevelBuffState((EvtLevelBuffState) evt));
        }

        private bool ListenLevelBuffState(EvtLevelBuffState evt)
        {
            if (evt.levelBuff != this.config.LevelBuff)
            {
                return false;
            }
            if (evt.state == LevelBuffState.Start)
            {
                if (this.config.OnModifierName != null)
                {
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OnModifierName);
                }
                if (this.config.OffModifierName != null)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
                }
            }
            else if (evt.state == LevelBuffState.Stop)
            {
                if (this.config.OnModifierName != null)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
                }
                if (this.config.OffModifierName != null)
                {
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
                }
            }
            return true;
        }

        public override void OnAdded()
        {
            if (Singleton<LevelManager>.Instance.levelActor.levelBuffs[(int) this.config.LevelBuff].isActive)
            {
                if (this.config.OnModifierName != null)
                {
                    base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OnModifierName);
                }
            }
            else if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(base.actor.runtimeID);
        }

        public override void OnRemoved()
        {
            if (this.config.OnModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
            }
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(base.actor.runtimeID);
        }
    }
}

