namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterSwordDodgeMixin : BaseAbilityMixin
    {
        private MonsterSwordDodgeMixin config;

        public AbilityMonsterSwordDodgeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterSwordDodgeMixin) config;
        }

        private AttackData CreateNoDamageAttack()
        {
            return new AttackData { damage = 0f, isInComboCount = false, attackerAniDamageRatio = 2f, aniDamageRatio = 2f, hitLevel = AttackResult.ActorHitLevel.Normal, hitEffect = AttackResult.AnimatorHitEffect.Normal, resolveStep = AttackData.AttackDataStep.AttackerResolved };
        }

        private void OnPostBeingHit(EvtBeingHit evt)
        {
            if ((((evt.attackData.attackerClass == EntityClass.ShortSworder) && (evt.attackData.attackerAniDamageRatio < this.config.NoDodgeAttackRatio)) && (evt.attackData.attackerNature != EntityNature.Psycho)) && ((this.config.DodgeRatio > UnityEngine.Random.value) || (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp)))
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DodgeActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
                Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.sourceID, base.actor.runtimeID, null, this.CreateNoDamageAttack()), MPEventDispatchMode.Normal);
            }
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                this.OnPostBeingHit((EvtBeingHit) evt);
            }
            return true;
        }
    }
}

