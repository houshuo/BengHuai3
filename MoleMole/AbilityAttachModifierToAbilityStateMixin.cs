namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToAbilityStateMixin : BaseAbilityMixin
    {
        public AttachModifierToAbilityStateMixin config;

        public AbilityAttachModifierToAbilityStateMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToAbilityStateMixin) config;
        }

        private bool IsTargetState(AbilityState target)
        {
            for (int i = 0; i < this.config.AbilityStates.Length; i++)
            {
                if (target.ContainsState(this.config.AbilityStates[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
            bool flag = this.IsTargetState(state);
            if (flag && (this.config.OnModifierName != null))
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (flag && (this.config.OffModifierName != null))
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
            }
        }

        private void OnAbilityStateRemove(AbilityState state)
        {
            bool flag = this.IsTargetState(state);
            if (flag && (this.config.OnModifierName != null))
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (flag && (this.config.OffModifierName != null))
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
            }
        }

        public override void OnAdded()
        {
            base.actor.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Combine(base.actor.onAbilityStateAdd, new Action<AbilityState, bool>(this.OnAbilityStateAdd));
            base.actor.onAbilityStateRemove = (Action<AbilityState>) Delegate.Combine(base.actor.onAbilityStateRemove, new Action<AbilityState>(this.OnAbilityStateRemove));
            bool flag = this.IsTargetState(base.actor.abilityState);
            if (flag && (this.config.OnModifierName != null))
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (!flag && (this.config.OffModifierName != null))
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.OffModifierName);
            }
        }

        public override void OnRemoved()
        {
            base.actor.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Remove(base.actor.onAbilityStateAdd, new Action<AbilityState, bool>(this.OnAbilityStateAdd));
            base.actor.onAbilityStateRemove = (Action<AbilityState>) Delegate.Remove(base.actor.onAbilityStateRemove, new Action<AbilityState>(this.OnAbilityStateRemove));
            if (this.config.OnModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OnModifierName);
            }
            if (this.config.OffModifierName != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.OffModifierName);
            }
        }
    }
}

