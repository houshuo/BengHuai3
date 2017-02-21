namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityAvatarQTEMixin : BaseAbilityMixin
    {
        private AvatarActor _avatarActor;
        private EntityTimer _delayQteTimer;
        private EntityTimer _qteMaxTimer;
        private List<QTETarget> _qteTargetList;
        private AvatarQTEMixin config;

        public AbilityAvatarQTEMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._qteTargetList = new List<QTETarget>();
            this.config = (AvatarQTEMixin) config;
            this._avatarActor = base.actor as AvatarActor;
            this._qteMaxTimer = new EntityTimer(instancedAbility.Evaluate(this.config.QTEMaxTimeSpan));
            this._qteMaxTimer.Reset(false);
            this._delayQteTimer = new EntityTimer(instancedAbility.Evaluate(this.config.DelayQTETimeSpan));
            this._delayQteTimer.Reset(false);
        }

        private void AddModifier(ActorAbility instancedAbility, string modifierName)
        {
            if (this._avatarActor.CanSwitchInQTE())
            {
                base.actor.abilityPlugin.ApplyModifier(instancedAbility, modifierName);
            }
        }

        private void AddQTETarget(MonsterActor actor)
        {
            bool flag = true;
            foreach (QTETarget target in this._qteTargetList)
            {
                if (target.monsterActor == actor)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                this._qteTargetList.Add(new QTETarget(actor));
            }
        }

        private bool CheckMaintainCondition(QTECondition qteCondition)
        {
            if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(this._avatarActor.runtimeID))
            {
                if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()).MuteOtherQTE)
                {
                    return false;
                }
                for (int i = 0; i < this._qteTargetList.Count; i++)
                {
                    QTETarget target = this._qteTargetList[i];
                    target.Core();
                    if (!target.CanCheck())
                    {
                        return true;
                    }
                    MonsterActor monsterActor = target.monsterActor;
                    if (((monsterActor != null) && (monsterActor.isAlive != 0)) && this.QTERangeCheck(monsterActor, qteCondition.QTERange))
                    {
                        if (qteCondition.QTEType == QTEConditionType.QTEAnimationTag)
                        {
                            for (int j = 0; j < qteCondition.QTEValues.Length; j++)
                            {
                                MonsterData.MonsterTagGroup tagGroup = (MonsterData.MonsterTagGroup) ((int) Enum.Parse(typeof(MonsterData.MonsterTagGroup), qteCondition.QTEValues[j]));
                                if (monsterActor.monster.IsAnimatorInTag(tagGroup))
                                {
                                    return true;
                                }
                            }
                        }
                        else if (qteCondition.QTEType == QTEConditionType.QTEBuffTag)
                        {
                            for (int k = 0; k < qteCondition.QTEValues.Length; k++)
                            {
                                AbilityState state = (AbilityState) ((int) Enum.Parse(typeof(AbilityState), qteCondition.QTEValues[k]));
                                if ((monsterActor.abilityState & state) != AbilityState.None)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override void Core()
        {
            bool flag = true;
            for (int i = 0; i < this.config.Conditions.Length; i++)
            {
                QTECondition qteCondition = this.config.Conditions[i];
                if (!this.CheckMaintainCondition(qteCondition))
                {
                    flag = false;
                    break;
                }
            }
            if (this._qteMaxTimer.isActive)
            {
                if (!this._qteMaxTimer.isTimeUp)
                {
                    this._qteMaxTimer.Core(1f);
                }
                else
                {
                    flag = false;
                    this._qteMaxTimer.Reset(false);
                    this._delayQteTimer.Reset(true);
                }
            }
            if (!flag)
            {
                if (!this._avatarActor.IsSwitchInCD() && (this._avatarActor.isAlive != 0))
                {
                    if (this._delayQteTimer.isActive)
                    {
                        if (!this._delayQteTimer.isTimeUp)
                        {
                            this._delayQteTimer.Core(1f);
                        }
                        else
                        {
                            this._qteTargetList.Clear();
                            this._avatarActor.DisableQTEAttack();
                        }
                    }
                }
                else
                {
                    this._qteTargetList.Clear();
                    this._avatarActor.DisableQTEAttack();
                }
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtAttackLanded)
            {
                return this.OnAttackLanded((EvtAttackLanded) evt);
            }
            return ((evt is EvtBuffAdd) && this.OnBuffAdd((EvtBuffAdd) evt));
        }

        public override void OnAdded()
        {
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
                Singleton<EventManager>.Instance.RegisterEventListener<EvtBuffAdd>(base.actor.runtimeID);
                Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(base.actor.runtimeID);
            }
        }

        private bool OnAttackLanded(EvtAttackLanded evt)
        {
            bool flag = true;
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID);
            if (((actor != null) && (actor is MonsterActor)) && !Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.runtimeID))
            {
                if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()).MuteOtherQTE)
                {
                    return false;
                }
                MonsterActor qteTarget = actor as MonsterActor;
                foreach (QTECondition condition in this.config.TriggerConditions)
                {
                    if (condition.QTEType == QTEConditionType.QTEAnimationTag)
                    {
                        flag &= this.QTERangeCheck(qteTarget, condition.QTERange);
                        bool flag2 = false;
                        for (int i = 0; i < condition.QTEValues.Length; i++)
                        {
                            flag2 |= evt.attackResult.hitEffect == ((int) Enum.Parse(typeof(AttackResult.AnimatorHitEffect), condition.QTEValues[i]));
                        }
                        flag &= flag2;
                    }
                    else if (condition.QTEType == QTEConditionType.QTEBuffTag)
                    {
                        flag = false;
                        break;
                    }
                }
                if ((flag && !this._avatarActor.IsSwitchInCD()) && !this._avatarActor.AllowOtherSwitchIn)
                {
                    this.AddQTETarget(qteTarget);
                    this._avatarActor.EnableQTEAttack(this.config.QTEName);
                    this._qteMaxTimer.Reset(true);
                }
            }
            return false;
        }

        private bool OnBuffAdd(EvtBuffAdd evt)
        {
            bool flag = true;
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID);
            if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(base.actor.runtimeID) && (actor != null))
            {
                if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()).MuteOtherQTE)
                {
                    return false;
                }
                MonsterActor qteTarget = actor as MonsterActor;
                foreach (QTECondition condition in this.config.TriggerConditions)
                {
                    if (condition.QTEType == QTEConditionType.QTEBuffTag)
                    {
                        flag &= this.QTERangeCheck(qteTarget, condition.QTERange);
                        bool flag2 = false;
                        for (int i = 0; i < condition.QTEValues.Length; i++)
                        {
                            flag2 |= evt.abilityState == ((int) Enum.Parse(typeof(AbilityState), condition.QTEValues[i]));
                        }
                        flag &= flag2;
                    }
                    else if (condition.QTEType == QTEConditionType.QTEAnimationTag)
                    {
                        flag = false;
                        break;
                    }
                }
                if ((flag && !this._avatarActor.IsSwitchInCD()) && !this._avatarActor.AllowOtherSwitchIn)
                {
                    this.AddQTETarget(qteTarget);
                    this._avatarActor.EnableQTEAttack(this.config.QTEName);
                    this._qteMaxTimer.Reset(true);
                }
            }
            return false;
        }

        public override void OnRemoved()
        {
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                Singleton<EventManager>.Instance.RemoveEventListener<EvtBuffAdd>(base.actor.runtimeID);
                Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackLanded>(base.actor.runtimeID);
                base.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(base.entity.onCurrentSkillIDChanged, new Action<string, string>(this.SkillIDChangedCallback));
            }
            base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.ModifierName);
        }

        private bool QTEAbilityStateCheck(MonsterActor qteTarget, AbilityState targetAbilityState)
        {
            return ((qteTarget != null) && (qteTarget.abilityState == targetAbilityState));
        }

        private bool QTEAnimationTagCheck(MonsterActor qteTarget, MonsterData.MonsterTagGroup animationTag)
        {
            return ((qteTarget != null) && qteTarget.monster.IsAnimatorInTag(animationTag));
        }

        private bool QTEHitEffectCheck(EvtAttackLanded evt, AttackResult.AnimatorHitEffect targetHitEffect)
        {
            return ((evt != null) && (evt.attackResult.hitEffect == targetHitEffect));
        }

        private bool QTERangeCheck(MonsterActor qteTarget, float targetRange)
        {
            return ((qteTarget != null) && (Vector3.Distance(Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition, qteTarget.entity.XZPosition) < targetRange));
        }

        private void RemoveModifier(ActorAbility instancedAbility, string modifierName)
        {
            base.actor.abilityPlugin.TryRemoveModifier(instancedAbility, modifierName);
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < this.config.SkillIDs.Length; i++)
            {
                if (this.config.SkillIDs[i] == from)
                {
                    flag = true;
                }
                if (this.config.SkillIDs[i] == to)
                {
                    flag2 = true;
                }
            }
            if (!base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, base.actor, null))
            {
                this.RemoveModifier(base.instancedAbility, this.config.ModifierName);
            }
            else if (!flag && flag2)
            {
                this.AddModifier(base.instancedAbility, this.config.ModifierName);
                Singleton<EventManager>.Instance.FireEvent(new EvtQTEFire(base.actor.runtimeID, this.config.QTEName), MPEventDispatchMode.Normal);
                this._qteTargetList.Clear();
                this._avatarActor.DisableQTEAttack();
            }
        }

        public class QTETarget
        {
            public float checkDelayTime;
            public MonsterActor monsterActor;
            private const float QTE_CHECK_DELAY = 0.3f;

            public QTETarget(MonsterActor actor)
            {
                this.monsterActor = actor;
                this.checkDelayTime = 0.3f;
            }

            public bool CanCheck()
            {
                return (this.checkDelayTime <= 0f);
            }

            public void Core()
            {
                if (this.checkDelayTime > 0f)
                {
                    this.checkDelayTime -= Time.deltaTime;
                }
            }
        }
    }
}

