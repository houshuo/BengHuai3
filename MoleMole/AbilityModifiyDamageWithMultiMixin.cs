namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityModifiyDamageWithMultiMixin : AbilityModifiyDamageMixin
    {
        private ModifyDamageWithMultiMixin config;

        public AbilityModifiyDamageWithMultiMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageWithMultiMixin) config;
        }

        private int GetTargetCountWithAbilityState(BaseAbilityActor[] targets)
        {
            int num = 0;
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].abilityState.ContainsState(this.config.TargetAbilityState))
                {
                    num++;
                }
            }
            return num;
        }

        protected override void ModifyDamage(EvtHittingOther evt, float multiple = 1)
        {
            multiple = 0f;
            switch (this.config.MultipleType)
            {
                case ModifyDamageWithMultiMixin.DamageMultipleType.BySelfCurrentSPAmount:
                    multiple = (float) base.actor.SP;
                    if (this.config.ClearAllSP)
                    {
                        base.actor.HealSP((float) -base.actor.SP);
                    }
                    break;

                case ModifyDamageWithMultiMixin.DamageMultipleType.BySelfMaxSPAmount:
                    multiple = (float) base.actor.maxSP;
                    break;

                case ModifyDamageWithMultiMixin.DamageMultipleType.ByTargetAbilityState:
                    BaseAbilityActor[] alliedActorsOf;
                    switch (this.config.Targetting)
                    {
                        case MixinTargetting.Allied:
                            alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(base.actor);
                            multiple = this.GetTargetCountWithAbilityState(alliedActorsOf);
                            goto Label_0192;

                        case MixinTargetting.Enemy:
                            alliedActorsOf = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(base.actor);
                            multiple = this.GetTargetCountWithAbilityState(alliedActorsOf);
                            goto Label_0192;

                        case MixinTargetting.All:
                            alliedActorsOf = Singleton<EventManager>.Instance.GetActorByCategory<BaseAbilityActor>(3);
                            multiple = this.GetTargetCountWithAbilityState(alliedActorsOf);
                            alliedActorsOf = Singleton<EventManager>.Instance.GetActorByCategory<BaseAbilityActor>(4);
                            multiple += this.GetTargetCountWithAbilityState(alliedActorsOf);
                            goto Label_0192;
                    }
                    break;

                case ModifyDamageWithMultiMixin.DamageMultipleType.ByTargetDistance:
                {
                    BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
                    if (entity != null)
                    {
                        Vector3 vector = entity.XZPosition - base.instancedAbility.caster.entity.XZPosition;
                        multiple = vector.magnitude;
                    }
                    break;
                }
                case ModifyDamageWithMultiMixin.DamageMultipleType.ByLevelCurrentCombo:
                    multiple = (float) Singleton<LevelManager>.Instance.levelActor.levelCombo;
                    break;
            }
        Label_0192:
            multiple -= this.config.BaseMultiple;
            if (multiple < 0f)
            {
                multiple = 0f;
            }
            if ((base.instancedAbility.Evaluate(this.config.MaxMultiple) > 0f) && (multiple > base.instancedAbility.Evaluate(this.config.MaxMultiple)))
            {
                multiple = base.instancedAbility.Evaluate(this.config.MaxMultiple);
            }
            base.ModifyDamage(evt, multiple);
        }
    }
}

