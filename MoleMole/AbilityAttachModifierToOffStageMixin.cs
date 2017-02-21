namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachModifierToOffStageMixin : BaseAbilityMixin
    {
        private ActorModifier _modifier;
        private AttachModifierToOffStageMixin config;

        public AbilityAttachModifierToOffStageMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachModifierToOffStageMixin) config;
        }

        public override void OnAdded()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Combine(base.entity.onActiveChanged, new Action<bool>(this.OnAvatarIsOnStageChanged));
            if (!base.entity.gameObject.activeSelf)
            {
                this.OnAvatarIsOnStageChanged(false);
            }
        }

        private void OnAvatarIsOnStageChanged(bool active)
        {
            if (active)
            {
                if ((this._modifier != null) && base.actor.entity.gameObject.activeInHierarchy)
                {
                    base.actor.abilityPlugin.TryRemoveModifier(this._modifier);
                }
            }
            else
            {
                this._modifier = base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.ModifierName);
            }
        }

        public override void OnRemoved()
        {
            base.entity.onActiveChanged = (Action<bool>) Delegate.Combine(base.entity.onActiveChanged, new Action<bool>(this.OnAvatarIsOnStageChanged));
            if (this._modifier != null)
            {
                base.actor.abilityPlugin.TryRemoveModifier(this._modifier);
            }
        }
    }
}

