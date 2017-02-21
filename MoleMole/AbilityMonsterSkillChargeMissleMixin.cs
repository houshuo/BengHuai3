namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterSkillChargeMissleMixin : AbilityMonsterSkillIDChargeAnimatorMixin
    {
        public MonsterSkillChargeMissleMixin config;

        public AbilityMonsterSkillChargeMissleMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterSkillChargeMissleMixin) config;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (evt.abilityArgument != null)
            {
                base.OnAbilityTriggered(evt);
            }
            else
            {
                int index = (base._loopIx >= base._loopCount) ? (base._loopCount - 1) : base._loopIx;
                BaseAbilityActor[] enemyActorsOf = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(base.actor);
                int num2 = this.config.ChargeMissleAmount[index];
                int length = enemyActorsOf.Length;
                for (int i = 0; i < length; i++)
                {
                    BaseMonoEntity target = enemyActorsOf[i].entity;
                    this.TriggerAbility(target, this.config.AbilityName);
                }
                if (length > 0)
                {
                    for (int j = 0; j < (num2 - length); j++)
                    {
                        BaseMonoEntity entity = enemyActorsOf[UnityEngine.Random.Range(0, length)].entity;
                        this.TriggerAbility(entity, this.config.AbilityNameSub);
                    }
                }
                else
                {
                    for (int k = 0; k < (num2 - length); k++)
                    {
                        this.TriggerAbility(null, this.config.AbilityNameSub);
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

