namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarSkillButtonChargeMissleMixin : AbilityAvatarSkillButtonHoldChargeMixin
    {
        public AvatarSkillButtonHoldChargeMissleMixin config;

        public AbilityAvatarSkillButtonChargeMissleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSkillButtonHoldChargeMissleMixin) config;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (evt.abilityArgument != null)
            {
                base.OnAbilityTriggered(evt);
            }
            else
            {
                BaseMonoAvatar avatar = base.entity as BaseMonoAvatar;
                if (avatar != null)
                {
                    int index = (base._loopIx >= base._loopCount) ? (base._loopCount - 1) : base._loopIx;
                    int num2 = this.config.ChargeMissleAmount[index];
                    int count = avatar.SubAttackTargetList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        BaseMonoEntity target = avatar.SubAttackTargetList[i];
                        this.TriggerAbility(target, this.config.AbilityName);
                    }
                    if (count > 0)
                    {
                        for (int j = 0; j < (num2 - count); j++)
                        {
                            BaseMonoEntity entity2 = avatar.SubAttackTargetList[UnityEngine.Random.Range(0, count)];
                            this.TriggerAbility(entity2, this.config.AbilityNameSub);
                        }
                    }
                    else
                    {
                        for (int k = 0; k < (num2 - count); k++)
                        {
                            this.TriggerAbility(null, this.config.AbilityNameSub);
                        }
                    }
                }
            }
        }

        private void TriggerAbility(BaseMonoEntity target, string ability)
        {
            EvtAbilityStart evt = new EvtAbilityStart(base.entity.GetRuntimeID(), null) {
                abilityName = ability
            };
            if (target != null)
            {
                evt.otherID = target.GetRuntimeID();
            }
            Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
        }
    }
}

