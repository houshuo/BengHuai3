namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AbilityTriggerMultiBulletMixin : BaseAbilityMixin
    {
        private TriggerMultiBulletMixin config;

        public AbilityTriggerMultiBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (TriggerMultiBulletMixin) config;
        }

        public override void Core()
        {
            base.Core();
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar != null)
            {
                int count = localAvatar.SubAttackTargetList.Count;
                for (int i = 0; i < count; i++)
                {
                    BaseMonoEntity entity = localAvatar.SubAttackTargetList[i];
                    EvtAbilityStart start = new EvtAbilityStart(base.entity.GetRuntimeID(), null) {
                        abilityName = this.config.AbilityName
                    };
                    if (entity != null)
                    {
                        start.otherID = entity.GetRuntimeID();
                    }
                    Singleton<EventManager>.Instance.FireEvent(start, MPEventDispatchMode.Normal);
                }
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
        }
    }
}

