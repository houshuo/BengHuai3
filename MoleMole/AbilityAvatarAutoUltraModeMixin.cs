namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityAvatarAutoUltraModeMixin : BaseAbilityMixin
    {
        private bool _isUltraMode;
        private AvatarAutoUltraModeMixin config;

        public AbilityAvatarAutoUltraModeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarAutoUltraModeMixin) config;
        }

        public override void Core()
        {
            base.Core();
            bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(base.entity.GetRuntimeID());
            if (!this._isUltraMode && (((base.actor.SP / base.actor.maxSP) >= this.config.AutoUltraSPRatio) & flag))
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.BeginActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                this._isUltraMode = true;
            }
            if (this._isUltraMode & flag)
            {
                float num = this.config.CostSPSpeed * Time.deltaTime;
                DelegateUtils.UpdateField(ref base.actor.SP, base.actor.SP - num, -num, base.actor.onSPChanged);
                if ((base.actor.SP / base.actor.maxSP) < this.config.EndUltarSPRatio)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.EndActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                    this._isUltraMode = false;
                }
            }
        }
    }
}

