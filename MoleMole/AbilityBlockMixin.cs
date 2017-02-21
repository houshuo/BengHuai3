namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityBlockMixin : BaseAbilityMixin
    {
        private bool _allowBlock;
        private float _blockChance;
        private EntityTimer _blockTimer;
        private BlockMixin config;

        public AbilityBlockMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._allowBlock = true;
            this.config = (BlockMixin) config;
            this._blockChance = instancedAbility.Evaluate(this.config.BlockChance);
            this._blockChance = Mathf.Clamp(this._blockChance, 0f, 1f);
            this._blockTimer = new EntityTimer(instancedAbility.Evaluate(this.config.BlockTimer));
        }

        public override void Core()
        {
            if (!this._allowBlock && (this._blockTimer.isActive && this.IsBlockHasCD()))
            {
                this._blockTimer.Core(1f);
                if (this._blockTimer.isTimeUp)
                {
                    this._allowBlock = true;
                    this._blockTimer.Reset(false);
                }
            }
        }

        private bool IsBlockHasCD()
        {
            return (this._blockTimer.timespan > 0f);
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (!this._allowBlock)
            {
                return false;
            }
            if (this.config.BlockSkillIDs != null)
            {
                string currentSkillID = base.actor.entity.CurrentSkillID;
                if (string.IsNullOrEmpty(currentSkillID))
                {
                    return false;
                }
                bool flag = false;
                for (int i = 0; i < this.config.BlockSkillIDs.Length; i++)
                {
                    if (currentSkillID == this.config.BlockSkillIDs[i])
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    return false;
                }
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.AttackerPredicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt))
            {
                return false;
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.TargetPredicates, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID), evt))
            {
                return false;
            }
            if (UnityEngine.Random.value >= this._blockChance)
            {
                return false;
            }
            evt.attackData.damage -= base.instancedAbility.Evaluate(this.config.DamageReduce);
            evt.attackData.damage *= 1f - base.instancedAbility.Evaluate(this.config.DamageReduceRatio);
            evt.attackData.damage = (evt.attackData.damage >= 0f) ? evt.attackData.damage : 0f;
            evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
            if (evt.attackData.damage == 0f)
            {
                evt.attackData.hitLevel = AttackResult.ActorHitLevel.Mute;
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.BlockActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID), evt);
            if (this.IsBlockHasCD())
            {
                this._blockTimer.Reset(true);
                this._allowBlock = false;
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }
    }
}

