namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseAbilityAvatarSkillButtonHoldChargeMixin : BaseAbilityMixin
    {
        private BaseMonoAvatar _avatar;
        private string _chargeAudioLoopName;
        private int _chargeEffectPatternIx;
        protected float _chargeTimeRatio;
        private bool _checkPointerDownInBS;
        private string _lastFrom;
        protected int _loopCount;
        protected int _loopIx;
        private State _oldState;
        private MonoSkillButton _skillButton;
        private State _state;
        private List<BaseMonoEntity> _subSelectTargetList;
        private EntityTimer _switchTimer;
        private EntityTimer _triggeredChargeTimer;
        private bool _useTriggerTimeControl;
        private BaseAvatarSkillButtonHoldChargeAnimatorMixin config;

        public BaseAbilityAvatarSkillButtonHoldChargeMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._checkPointerDownInBS = true;
            this.config = (BaseAvatarSkillButtonHoldChargeAnimatorMixin) config;
            this._avatar = (BaseMonoAvatar) base.entity;
            this._triggeredChargeTimer = new EntityTimer();
            this._switchTimer = new EntityTimer(this.config.ChargeSwitchWindow, base.entity);
            this._loopCount = this.config.ChargeLoopSkillIDs.Length;
            this._subSelectTargetList = new List<BaseMonoEntity>();
            this._chargeTimeRatio = 1f;
        }

        private void AddSubAttackTarget(BaseMonoEntity target)
        {
            BaseMonoAvatar entity = base.actor.entity as BaseMonoAvatar;
            if (entity != null)
            {
                entity.AddTargetToSubAttackList(target);
            }
        }

        private void ClearAllSubAttackTargets()
        {
            BaseMonoAvatar entity = base.actor.entity as BaseMonoAvatar;
            if (entity != null)
            {
                entity.ClearSubAttackList();
            }
        }

        private void ClearSubTargets()
        {
            if (this._subSelectTargetList.Count > 0)
            {
                for (int i = 0; i < this._subSelectTargetList.Count; i++)
                {
                    BaseMonoEntity entity = this._subSelectTargetList[i];
                    if (entity != null)
                    {
                        MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(entity.GetRuntimeID());
                        if ((actor != null) && !string.IsNullOrEmpty(this.config.SubTargetModifierName))
                        {
                            actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.SubTargetModifierName);
                        }
                    }
                }
                this._subSelectTargetList.Clear();
            }
            this.ClearAllSubAttackTargets();
        }

        public override void Core()
        {
            if (this._oldState != this._state)
            {
                this._oldState = this._state;
            }
            if (this._triggeredChargeTimer.isActive)
            {
                this._triggeredChargeTimer.Core(1f);
                if (this._state == State.Before)
                {
                    if (this._triggeredChargeTimer.isTimeUp)
                    {
                        this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                        this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
                    }
                    else
                    {
                        this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                        this._avatar.SetTrigger(this.config.NextLoopTriggerID);
                    }
                }
                else if ((this._state == State.InLoop) && this._triggeredChargeTimer.isTimeUp)
                {
                    this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
                }
                if (this._triggeredChargeTimer.isTimeUp)
                {
                    this._triggeredChargeTimer.Reset(false);
                    this._useTriggerTimeControl = false;
                }
            }
            if (this._state == State.InLoop)
            {
                this.UpdateInLoop();
                if (this.ShouldMoveToNextLoop())
                {
                    this._loopIx++;
                    if (this._loopIx == this._loopCount)
                    {
                        this.OnMoveingToNextLoop(true);
                        this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
                        this._avatar.IsLockDirection = false;
                        this.ClearSubTargets();
                    }
                    else
                    {
                        this.OnMoveingToNextLoop(false);
                        this._avatar.SetTrigger(this.config.NextLoopTriggerID);
                        if (this.config.ChargeSubTargetAmount != null)
                        {
                            int targetAmount = this.config.ChargeSubTargetAmount[this._loopIx];
                            this.SelectSubTargets(targetAmount);
                        }
                    }
                }
                this._switchTimer.Core(1f);
                if (this._switchTimer.isTimeUp)
                {
                    this._switchTimer.Reset(false);
                }
            }
        }

        private bool IsControlHold()
        {
            if (this._useTriggerTimeControl)
            {
                return this.IsTriggerCharging();
            }
            return this._skillButton.IsPointerHold();
        }

        private bool IsTriggerCharging()
        {
            return (this._triggeredChargeTimer.isActive && !this._triggeredChargeTimer.isTimeUp);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtLocalAvatarChanged) && this.OnLocalAvatarChange((EvtLocalAvatarChanged) evt));
        }

        private int NearestTargetCompare(BaseMonoMonster monsterA, BaseMonoMonster monsterB)
        {
            float num = Vector3.Distance(base.entity.transform.position, monsterA.transform.position);
            float num2 = Vector3.Distance(base.entity.transform.position, monsterB.transform.position);
            return (int) (num - num2);
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            float abilityArgument = (float) evt.abilityArgument;
            if (abilityArgument != 0f)
            {
                this._triggeredChargeTimer.timespan = abilityArgument;
                this._triggeredChargeTimer.Reset(true);
                this._useTriggerTimeControl = true;
            }
            else
            {
                this._useTriggerTimeControl = false;
            }
        }

        public override void OnAdded()
        {
            this._avatar.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._avatar.onCurrentSkillIDChanged, new Action<string, string>(this.WithTransientSkillIDChangedCallback));
            this._state = State.Idle;
            this._switchTimer.Reset(false);
            this._loopIx = 0;
            this._chargeEffectPatternIx = -1;
            this._skillButton = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.GetSkillButtonBySkillID(this.config.SkillButtonID);
            this._skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>) Delegate.Combine(this._skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(this.SkillButtonStateChangedCallback));
            if (this._avatar.IsAIActive() && !string.IsNullOrEmpty(this.config.ChargeTimeRatioAIKey))
            {
                (this._avatar.GetActiveAIController() as BTreeAvatarAIController).SetBehaviorVariable(this.config.ChargeTimeRatioAIKey, this._chargeTimeRatio);
            }
            if ((this.config.ChargeLoopEffects != null) && (this.config.ChargeSwitchEffects != null))
            {
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLocalAvatarChanged>(base.actor.runtimeID);
        }

        protected abstract void OnBeforeToInLoop();
        protected abstract void OnInLoopToAfter();
        private bool OnLocalAvatarChange(EvtLocalAvatarChanged evt)
        {
            if (evt.localAvatarID == this._avatar.GetRuntimeID())
            {
                this._triggeredChargeTimer.isTimeUp = true;
            }
            return true;
        }

        protected abstract void OnMoveingToNextLoop(bool endLoop);
        public override void OnRemoved()
        {
            this._avatar.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._avatar.onCurrentSkillIDChanged, new Action<string, string>(this.WithTransientSkillIDChangedCallback));
            this._skillButton.onPointerStateChange = (Func<MonoSkillButton.PointerState, bool>) Delegate.Remove(this._skillButton.onPointerStateChange, new Func<MonoSkillButton.PointerState, bool>(this.SkillButtonStateChangedCallback));
            if (this._chargeEffectPatternIx != -1)
            {
                base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
            }
            if (this._chargeAudioLoopName != null)
            {
                base.entity.StopAudio(this._chargeAudioLoopName);
            }
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLocalAvatarChanged>(base.actor.runtimeID);
        }

        private void SelectSubTargets(int targetAmount)
        {
            BaseMonoEntity attackTarget = base.actor.entity.GetAttackTarget();
            if ((attackTarget != null) && !this._subSelectTargetList.Contains(attackTarget))
            {
                this._subSelectTargetList.Add(attackTarget);
                this.AddSubAttackTarget(attackTarget);
                if (attackTarget is BaseMonoMonster)
                {
                    MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(attackTarget.GetRuntimeID());
                    if ((actor != null) && !string.IsNullOrEmpty(this.config.SubTargetModifierName))
                    {
                        actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.SubTargetModifierName);
                    }
                }
            }
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            allMonsters.Sort(new Comparison<BaseMonoMonster>(this.NearestTargetCompare));
            for (int i = 0; i < allMonsters.Count; i++)
            {
                BaseMonoMonster item = allMonsters[i];
                MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(item.GetRuntimeID());
                if (((item != attackTarget) && (actor2 != null)) && ((this._subSelectTargetList.Count < targetAmount) && !this._subSelectTargetList.Contains(item)))
                {
                    if (!string.IsNullOrEmpty(this.config.SubTargetModifierName))
                    {
                        actor2.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.SubTargetModifierName);
                    }
                    this._subSelectTargetList.Add(item);
                    this.AddSubAttackTarget(item);
                }
            }
        }

        protected abstract bool ShouldMoveToNextLoop();
        private bool SkillButtonStateChangedCallback(MonoSkillButton.PointerState pointerState)
        {
            if (!this._useTriggerTimeControl)
            {
                if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(this._avatar.GetRuntimeID()))
                {
                    return true;
                }
                BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(base.actor.runtimeID);
                bool allowHoldLockDirection = this.config.AllowHoldLockDirection;
                if ((avatarByRuntimeID != null) && allowHoldLockDirection)
                {
                    if (pointerState == MonoSkillButton.PointerState.PointerUp)
                    {
                        avatarByRuntimeID.IsLockDirection = false;
                    }
                    else if (pointerState == MonoSkillButton.PointerState.PointerDown)
                    {
                        avatarByRuntimeID.IsLockDirection = true;
                    }
                }
                if (this._state == State.Before)
                {
                    if (pointerState == MonoSkillButton.PointerState.PointerUp)
                    {
                        this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                        this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
                    }
                    else if (pointerState == MonoSkillButton.PointerState.PointerDown)
                    {
                        if (!this.config.DisallowReleaseButtonInBS)
                        {
                            this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                            this._avatar.SetTrigger(this.config.NextLoopTriggerID);
                        }
                        else if (this._checkPointerDownInBS)
                        {
                            this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                            this._avatar.SetTrigger(this.config.NextLoopTriggerID);
                            this._checkPointerDownInBS = false;
                        }
                    }
                }
                else if ((this._state == State.InLoop) && (pointerState == MonoSkillButton.PointerState.PointerUp))
                {
                    this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
                }
            }
            return true;
        }

        private void SkillIDChangedCallback(string from, string to)
        {
            if (this._state == State.Idle)
            {
                if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to))
                {
                    this.SkillIDChangedToBefore();
                }
            }
            else if (this._state == State.Before)
            {
                if (to == this.config.ChargeLoopSkillIDs[this._loopIx])
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        MixinEffect effect = this.config.ChargeLoopEffects[this._loopIx];
                        if (effect.EffectPattern != null)
                        {
                            this._chargeEffectPatternIx = base.entity.AttachEffect(effect.EffectPattern);
                        }
                        if (effect.AudioPattern != null)
                        {
                            this._chargeAudioLoopName = effect.AudioPattern;
                            base.entity.PlayAudio(effect.AudioPattern);
                        }
                    }
                    this._state = State.InLoop;
                    this.OnBeforeToInLoop();
                    if (this.config.ChargeSubTargetAmount != null)
                    {
                        int targetAmount = this.config.ChargeSubTargetAmount[this._loopIx];
                        this.SelectSubTargets(targetAmount);
                    }
                }
                else if (Miscs.ArrayContains<string>(this.config.AfterSkillIDs, to))
                {
                    this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.After;
                }
                else if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to))
                {
                    if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, from))
                    {
                        this.SkillIDChangedToBefore();
                    }
                }
                else
                {
                    this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                }
            }
            else if (this._state == State.InLoop)
            {
                if (Miscs.ArrayContains<string>(this.config.ChargeLoopSkillIDs, to))
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        if (this.config.ImmediatelyDetachLoopEffect)
                        {
                            base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
                        }
                        else
                        {
                            base.entity.DetachEffect(this._chargeEffectPatternIx);
                        }
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                        MixinEffect effect2 = this.config.ChargeLoopEffects[this._loopIx];
                        if (effect2.EffectPattern != null)
                        {
                            this._chargeEffectPatternIx = base.entity.AttachEffect(effect2.EffectPattern);
                        }
                        if (effect2.AudioPattern != null)
                        {
                            this._chargeAudioLoopName = effect2.AudioPattern;
                            base.entity.PlayAudio(effect2.AudioPattern);
                        }
                        if (this.config.ChargeSwitchEffects != null)
                        {
                            base.FireMixinEffect(this.config.ChargeSwitchEffects[this._loopIx - 1], base.entity, false);
                        }
                    }
                    this._switchTimer.Reset(true);
                }
                else if (Miscs.ArrayContains<string>(this.config.AfterSkillIDs, to))
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        if (this.config.ImmediatelyDetachLoopEffect)
                        {
                            base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
                        }
                        else
                        {
                            base.entity.DetachEffect(this._chargeEffectPatternIx);
                        }
                        this._chargeEffectPatternIx = -1;
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                    }
                    EvtChargeRelease evt = new EvtChargeRelease(base.actor.runtimeID, to) {
                        isSwitchRelease = this._switchTimer.isActive && !this._switchTimer.isTimeUp
                    };
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                    this._switchTimer.Reset(false);
                    this._state = State.After;
                    this.OnInLoopToAfter();
                }
                else
                {
                    if (this.config.ChargeLoopEffects != null)
                    {
                        if (this.config.ImmediatelyDetachLoopEffect)
                        {
                            base.entity.DetachEffectImmediately(this._chargeEffectPatternIx);
                        }
                        else
                        {
                            base.entity.DetachEffect(this._chargeEffectPatternIx);
                        }
                        this._chargeEffectPatternIx = -1;
                        if (this._chargeAudioLoopName != null)
                        {
                            base.entity.StopAudio(this._chargeAudioLoopName);
                            this._chargeAudioLoopName = null;
                        }
                    }
                    this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                    this.ClearSubTargets();
                }
            }
            else if (this._state == State.After)
            {
                if (Miscs.ArrayContains<string>(this.config.BeforeSkillIDs, to))
                {
                    this.SkillIDChangedToBefore();
                }
                else
                {
                    this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                    this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                    this._state = State.Idle;
                }
                this.ClearSubTargets();
            }
        }

        private void SkillIDChangedToBefore()
        {
            this._state = State.Before;
            this._loopIx = 0;
            if (this.IsControlHold())
            {
                if (this.config.DisallowReleaseButtonInBS)
                {
                    this._checkPointerDownInBS = false;
                }
                else
                {
                    this._checkPointerDownInBS = true;
                }
                this._avatar.ResetTrigger(this.config.AfterSkillTriggerID);
                this._avatar.SetTrigger(this.config.NextLoopTriggerID);
            }
            else
            {
                this._checkPointerDownInBS = true;
                this._avatar.ResetTrigger(this.config.NextLoopTriggerID);
                this._avatar.SetTrigger(this.config.AfterSkillTriggerID);
            }
        }

        protected abstract void UpdateInLoop();
        private void WithTransientSkillIDChangedCallback(string from, string to)
        {
            if (Miscs.ArrayContains<string>(this.config.TransientSkillIDs, to))
            {
                this._lastFrom = from;
            }
            else if (Miscs.ArrayContains<string>(this.config.TransientSkillIDs, from))
            {
                this.SkillIDChangedCallback(this._lastFrom, to);
            }
            else
            {
                this.SkillIDChangedCallback(from, to);
            }
        }

        private enum State
        {
            Idle,
            Before,
            InLoop,
            After
        }
    }
}

