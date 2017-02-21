namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityModifyDamageTakenWithMultiMixin : AbilityModifyDamageTakenMixin
    {
        private ModifyDamageTakenWithMultiMixin config;

        public AbilityModifyDamageTakenWithMultiMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (ModifyDamageTakenWithMultiMixin) config;
        }

        protected override void ModifyDamage(EvtBeingHit evt, float multiple = 1)
        {
            multiple = 0f;
            if (this.config.MultipleType == ModifyDamageTakenWithMultiMixin.DamageMultipleType.ByTargetDistance)
            {
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.sourceID);
                if (entity != null)
                {
                    Vector3 vector = entity.XZPosition - base.instancedAbility.caster.entity.XZPosition;
                    multiple = vector.magnitude;
                }
            }
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

