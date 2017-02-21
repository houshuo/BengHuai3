namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityAttachRebindAttachPointMixin : BaseAbilityMixin
    {
        private AttachRebindAttachPointMixin config;

        public AbilityAttachRebindAttachPointMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AttachRebindAttachPointMixin) config;
        }

        public override void OnAdded()
        {
            (base.entity as BaseMonoAnimatorEntity).RebindAttachPoint(this.config.PointName, this.config.OtherName);
        }

        public override void OnRemoved()
        {
            (base.entity as BaseMonoAnimatorEntity).RebindAttachPoint(this.config.PointName, this.config.OriginName);
        }
    }
}

