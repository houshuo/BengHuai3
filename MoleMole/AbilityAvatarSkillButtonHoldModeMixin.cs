namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAvatarSkillButtonHoldModeMixin : BaseAbilityMixin
    {
        private AvatarActor _avatarActor;
        public AvatarSkillButtonHoldModeMixin config;

        public AbilityAvatarSkillButtonHoldModeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSkillButtonHoldModeMixin) config;
            this._avatarActor = (AvatarActor) base.actor;
        }

        public override void OnAdded()
        {
            this._avatarActor.SetAttackButtonHoldMode(true);
        }

        public override void OnRemoved()
        {
            this._avatarActor.SetAttackButtonHoldMode(false);
        }
    }
}

