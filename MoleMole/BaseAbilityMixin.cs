namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class BaseAbilityMixin
    {
        public BaseAbilityActor actor;
        public BaseMonoAbilityEntity entity;
        public ActorAbility instancedAbility;
        public int instancedMixinID;
        public ActorModifier instancedModifier;
        public int mixinLocalID;
        public BaseAbilityEntityIdentiy selfIdentity;

        public BaseAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
        {
            this.instancedAbility = instancedAbility;
            this.instancedModifier = instancedModifier;
            this.actor = (instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner;
            this.entity = this.actor.entity;
            this.mixinLocalID = config.localID;
        }

        public int AttachMixinEffect(MixinEffect effectConfig)
        {
            if (effectConfig.AudioPattern != null)
            {
                this.entity.PlayAudio(effectConfig.AudioPattern, this.entity.transform);
            }
            return this.entity.AttachEffect(effectConfig.EffectPattern);
        }

        public virtual void Core()
        {
        }

        public bool EvaluatePredicate(float lhs, float rhs, MixinPredicate predicate)
        {
            switch (predicate)
            {
                case MixinPredicate.Greater:
                    return (lhs > rhs);

                case MixinPredicate.GreaterOrEqual:
                    return (lhs >= rhs);

                case MixinPredicate.Equal:
                    return (lhs == rhs);

                case MixinPredicate.Lesser:
                    return (lhs < rhs);
            }
            return false;
        }

        public void FireMixinEffect(MixinEffect effectConfig, BaseMonoEntity target, bool allowInactiveFire = false)
        {
            if ((effectConfig != null) && (target != null))
            {
                bool flag = false;
                if (allowInactiveFire)
                {
                    flag = true;
                }
                else if (this.entity.gameObject.activeSelf)
                {
                    flag = true;
                }
                if (flag)
                {
                    if (effectConfig.EffectPattern != null)
                    {
                        this.entity.FireEffect(effectConfig.EffectPattern, target.transform.position, target.transform.forward);
                    }
                    if (effectConfig.AudioPattern != null)
                    {
                        this.entity.PlayAudio(effectConfig.AudioPattern, target.transform);
                    }
                }
            }
        }

        public void FireMixinEffect(MixinEffect effectConfig, BaseMonoEntity target, Vector3 pos, Vector3 forward, bool allowInactiveFire = false)
        {
            if ((effectConfig != null) && (target != null))
            {
                bool flag = false;
                if (allowInactiveFire)
                {
                    flag = true;
                }
                else if (this.entity.gameObject.activeSelf)
                {
                    flag = true;
                }
                if (flag)
                {
                    if (effectConfig.EffectPattern != null)
                    {
                        this.entity.FireEffect(effectConfig.EffectPattern, pos, forward);
                    }
                    if (effectConfig.AudioPattern != null)
                    {
                        this.entity.PlayAudio(effectConfig.AudioPattern, target.transform);
                    }
                }
            }
        }

        public virtual void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
        {
        }

        public virtual bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnAbilityTriggered(EvtAbilityStart evt)
        {
        }

        public virtual void OnAdded()
        {
        }

        public virtual bool OnEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual bool OnPostEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnRemoved()
        {
        }

        protected void StartRecordMixinInvokeEntry(out RecordInvokeEntryContext context, uint targetID = 0)
        {
            this.actor.mpAbilityPlugin.StartRecordInvokeEntry(this.instancedAbility.instancedAbilityID, (this.instancedModifier == null) ? 0 : this.instancedModifier.instancedModifierID, (targetID != 0) ? targetID : this.actor.runtimeID, this.mixinLocalID, out context);
        }
    }
}

