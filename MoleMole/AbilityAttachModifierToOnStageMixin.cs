namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToOnStageMixin : BaseAbilityMixin
    {
        private ActorModifier _modifier;
        private AttachModifierToOnStageMixin config;

        public AbilityAttachModifierToOnStageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToOnStageMixin) config;
        }

        public override void OnAdded()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Combine(base.entity.onActiveChanged, new Action<bool>(this.OnAvatarIsOnStageChanged));
            if (base.entity.gameObject.activeSelf)
            {
                this.OnAvatarIsOnStageChanged(true);
            }
        }

        private void OnAvatarIsOnStageChanged(bool active)
        {
            if (!active)
            {
                base.actor.abilityPlugin.TryRemoveModifier(this._modifier);
            }
            else
            {
                this._modifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
        }

        public override void OnRemoved()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Remove(base.entity.onActiveChanged, new Action<bool>(this.OnAvatarIsOnStageChanged));
            if (this._modifier != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(this._modifier);
            }
        }
    }
}

