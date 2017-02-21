namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterDefendMixin : BaseAbilityMixin
    {
        private bool _allowDefendActions;
        private bool _allowLayerControl;
        private BaseMonoAnimatorEntity _animatorEntity;
        private float _breakDefendAniDamageRatio;
        private float _defendActionChance;
        private float _defendActionRange;
        private EntityTimer _defendActionTimer;
        private float _defendAngle;
        private bool _defendCDAllowed;
        private EntityTimer _inDefendStateTimer;
        private bool _isInDefendState;
        private float _layerWeightTransitionDuration;
        private int _shieldLightLayer;
        private float _shieldLightMax;
        private float _shieldLightMin;
        private MonsterDefendMixin config;

        public AbilityMonsterDefendMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._breakDefendAniDamageRatio = 1f;
            this._defendActionRange = 1f;
            this._defendActionChance = 1f;
            this._shieldLightLayer = 3;
            this._shieldLightMax = 1f;
            this._layerWeightTransitionDuration = 0.3f;
            this.config = (MonsterDefendMixin) config;
            this._defendAngle = instancedAbility.Evaluate(this.config.DefendAngle);
            this._defendActionTimer = new EntityTimer(instancedAbility.Evaluate(this.config.DefendActionCD));
            this._inDefendStateTimer = new EntityTimer(instancedAbility.Evaluate(this.config.InDefendMaxTime));
            this._breakDefendAniDamageRatio = instancedAbility.Evaluate(this.config.BreakDefendAniDamageRatio);
            this._defendActionRange = instancedAbility.Evaluate(this.config.DefendActionRange);
            this._defendActionChance = instancedAbility.Evaluate(this.config.DefendActionChance);
            this._defendActionChance = Mathf.Clamp(this._defendActionChance, 0f, 1f);
            this._defendActionTimer.SetActive(this.config.DefendSuccessActions.Length > 0);
            this._shieldLightLayer = this.config.ShieldLightLayer;
            this._shieldLightMax = Mathf.Clamp(this.config.ShieldLightMax, 0f, 1f);
            this._shieldLightMin = Mathf.Clamp(this.config.ShieldLightMin, 0f, 1f);
            this._allowLayerControl = this.config.AllowLayerLightControl;
            this._animatorEntity = (BaseMonoAnimatorEntity) base.entity;
        }

        public override void Core()
        {
            this._defendActionTimer.Core(1f);
            this._inDefendStateTimer.Core(1f);
            if (this._defendActionTimer.isTimeUp || this._inDefendStateTimer.isTimeUp)
            {
                this._defendCDAllowed = true;
                this._defendActionTimer.Reset(false);
                this._inDefendStateTimer.Reset(false);
                this.DefendActionReady();
            }
            base.actor.entity.ResetTrigger(this.config.DefendTriggerID);
        }

        private void DefendActionReady()
        {
            if (this.config.DefendActionReadyActions.Length > 0)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendActionReadyActions, base.instancedAbility, base.instancedModifier, base.actor, null);
            }
            if (this._allowLayerControl)
            {
                this._animatorEntity.StartFadeAnimatorLayerWeight(this._shieldLightLayer, this._shieldLightMax, this._layerWeightTransitionDuration);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (this._defendActionTimer != null)
            {
                this._defendActionTimer.Reset(true);
            }
            this.StartDefend();
        }

        public override void OnAdded()
        {
            if (this.config.ResetCDSkillIDs != null)
            {
                base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            }
            this.StartDefend();
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.sourceID);
            MonsterActor actor2 = base.actor as MonsterActor;
            BaseMonoMonster entity = base.entity as BaseMonoMonster;
            if ((actor != null) && (entity != null))
            {
                entity.hasArmor = false;
            }
            if ((actor2 != null) && (actor != null))
            {
                if (evt.attackData.attackerAniDamageRatio > this._breakDefendAniDamageRatio)
                {
                    return true;
                }
                string currentSkillID = actor2.monster.CurrentSkillID;
                if (!string.IsNullOrEmpty(currentSkillID))
                {
                    bool flag = false;
                    for (int i = 0; i < this.config.DefendSkillIDs.Length; i++)
                    {
                        if (this.config.DefendSkillIDs[i] == currentSkillID)
                        {
                            flag = true;
                            break;
                        }
                    }
                    bool flag2 = false;
                    bool flag3 = Vector3.Angle(base.actor.entity.transform.forward, actor.entity.transform.position - base.actor.entity.transform.position) < this._defendAngle;
                    if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, base.actor, null))
                    {
                        flag2 = false;
                    }
                    else if (flag3)
                    {
                        flag2 = true;
                    }
                    bool flag4 = false;
                    if ((evt.attackData.hitType == AttackResult.ActorHitType.Melee) && this.config.DefendMelee)
                    {
                        flag4 = true;
                    }
                    if ((evt.attackData.hitType == AttackResult.ActorHitType.Ranged) && this.config.DefendRange)
                    {
                        flag4 = true;
                    }
                    if ((flag && flag2) && flag4)
                    {
                        if (!this._isInDefendState)
                        {
                            this._isInDefendState = true;
                            this._inDefendStateTimer.Reset(base.instancedAbility.Evaluate(this.config.InDefendMaxTime) > 0f);
                        }
                        if (entity != null)
                        {
                            entity.hasArmor = true;
                        }
                        base.actor.entity.SetTrigger(this.config.DefendTriggerID);
                        evt.attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
                        evt.attackData.hitEffectPattern = AttackResult.HitEffectPattern.OnlyAttack;
                        bool flag5 = Vector3.Distance(actor.entity.transform.position, base.actor.entity.transform.position) < this._defendActionRange;
                        this._allowDefendActions = (UnityEngine.Random.value < this._defendActionChance) && this._defendCDAllowed;
                        if (flag3)
                        {
                            evt.attackData.damage = 0f;
                            if (evt.attackData.GetElementalDamage() > 0f)
                            {
                                evt.attackData.hitLevel = AttackResult.ActorHitLevel.Normal;
                            }
                            else
                            {
                                evt.attackData.hitLevel = AttackResult.ActorHitLevel.Mute;
                            }
                            if (this._allowDefendActions && flag5)
                            {
                                if (this.config.DefendSuccessEffect != null)
                                {
                                    base.FireMixinEffect(this.config.DefendSuccessEffect, base.entity, false);
                                }
                            }
                            else if (this.config.DefendEffect != null)
                            {
                                base.FireMixinEffect(this.config.DefendEffect, base.entity, false);
                            }
                        }
                        if (this._allowDefendActions && flag5)
                        {
                            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendSuccessActions, base.instancedAbility, base.instancedModifier, base.actor, evt);
                            this._defendCDAllowed = false;
                            this._defendActionTimer.Reset(true);
                            this._isInDefendState = false;
                            this._inDefendStateTimer.Reset(false);
                            this.StartDefend();
                        }
                    }
                }
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }

        public override void OnRemoved()
        {
            if (this.config.ResetCDSkillIDs != null)
            {
                base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            }
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            for (int i = 0; i < this.config.ResetCDSkillIDs.Length; i++)
            {
                if (to == this.config.ResetCDSkillIDs[i])
                {
                    this._defendCDAllowed = false;
                    this._defendActionTimer.Reset(true);
                    this._isInDefendState = false;
                    this._inDefendStateTimer.Reset(false);
                    this.StartDefend();
                }
            }
        }

        private void StartDefend()
        {
            if ((this.config != null) && (this.config.DefendStartActions != null))
            {
                if (this.config.DefendStartActions.Length > 0)
                {
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.DefendStartActions, base.instancedAbility, base.instancedModifier, base.actor, null);
                }
                if (this._allowLayerControl && (this._animatorEntity != null))
                {
                    this._animatorEntity.StartFadeAnimatorLayerWeight(this._shieldLightLayer, this._shieldLightMin, this._layerWeightTransitionDuration);
                }
            }
        }
    }
}

