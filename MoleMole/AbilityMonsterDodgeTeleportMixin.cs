namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityMonsterDodgeTeleportMixin : AbilityDodgeTeleportMixin
    {
        protected MonsterDodgeTeleportMixin config;

        public AbilityMonsterDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterDodgeTeleportMixin) config;
        }

        protected override void ClearTargetAttackTarget(uint sourceID)
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(sourceID);
            if (actor != null)
            {
                (actor.entity as BaseMonoAvatar).SetAttackTarget(null);
            }
        }
    }
}

