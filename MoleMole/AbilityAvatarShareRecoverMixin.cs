namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class AbilityAvatarShareRecoverMixin : BaseAbilityMixin
    {
        private AvatarShareRecoverMixin config;

        public AbilityAvatarShareRecoverMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarShareRecoverMixin) config;
        }

        public override void OnAdded()
        {
            if (this.config.ShareHP)
            {
                base.actor.onHPChanged = (Action<float, float, float>) Delegate.Combine(base.actor.onHPChanged, new Action<float, float, float>(this.ShareHPCallback));
            }
            if (this.config.ShareSP)
            {
                base.actor.onSPChanged = (Action<float, float, float>) Delegate.Combine(base.actor.onSPChanged, new Action<float, float, float>(this.ShareSPCallback));
            }
        }

        public override void OnRemoved()
        {
            if (this.config.ShareHP)
            {
                base.actor.onHPChanged = (Action<float, float, float>) Delegate.Remove(base.actor.onHPChanged, new Action<float, float, float>(this.ShareHPCallback));
            }
            if (this.config.ShareSP)
            {
                base.actor.onSPChanged = (Action<float, float, float>) Delegate.Remove(base.actor.onSPChanged, new Action<float, float, float>(this.ShareSPCallback));
            }
        }

        private void ShareHPCallback(float from, float to, float amount)
        {
            if ((amount > 0f) && base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, null, null))
            {
                float num = base.instancedAbility.Evaluate(this.config.ShareHPRatio) * amount;
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                for (int i = 0; i < allPlayerAvatars.Count; i++)
                {
                    BaseMonoAvatar avatar = allPlayerAvatars[i];
                    if (((avatar != null) && avatar.IsAlive()) && (avatar.GetRuntimeID() != base.actor.runtimeID))
                    {
                        Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).HealHP(num);
                    }
                }
            }
        }

        private void ShareSPCallback(float from, float to, float amount)
        {
            if ((amount > 0f) && base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, null, null))
            {
                float num = base.instancedAbility.Evaluate(this.config.ShareSPRatio) * amount;
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                for (int i = 0; i < allPlayerAvatars.Count; i++)
                {
                    BaseMonoAvatar avatar = allPlayerAvatars[i];
                    if (((avatar != null) && avatar.IsAlive()) && (avatar.GetRuntimeID() != base.actor.runtimeID))
                    {
                        Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).HealSP(num);
                    }
                }
            }
        }
    }
}

