namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public class ActorAbilityPlugin : BaseActorPlugin
    {
        private List<ConfigAbility> _additionalAbilities;
        [ShowInInspector, InspectorCollapsedFoldout]
        protected List<ActorAbility> _appliedAbilities;
        [InspectorCollapsedFoldout, ShowInInspector]
        protected List<ActorModifier> _appliedModifiers;
        private static ConfigEntityAttackProperty _attackProperty = new ConfigEntityAttackProperty();
        private List<ActorModifier> _deadModifiers;
        private Dictionary<string, DisplayValue<float>> _displayValueMap;
        private Dictionary<string, DynamicActorValue<float>> _dynamicValueMap;
        private bool _isKilled;
        private bool _isMuted;
        protected LevelActor _levelActor;
        private List<Tuple<ActorModifier, EntityTimer>> _modifierDurationTimers;
        private List<Tuple<ActorModifier, EntityTimer>> _modifierThinkTimers;
        protected BaseAbilityActor _owner;
        private bool _waitForStageReady;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache16;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache17;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache18;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache19;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache1A;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache1B;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache1C;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache1D;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache1E;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache1F;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache20;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache21;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache22;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache23;
        [CompilerGenerated]
        private static Func<ConfigAbilityModifier, ConfigAbilityAction[]> <>f__am$cache24;
        [CompilerGenerated]
        private static Func<ConfigAbility, ConfigAbilityAction[]> <>f__am$cache25;
        [CompilerGenerated]
        private static Func<BaseAbilityActor, bool> <>f__am$cache26;
        private static Dictionary<string, object> EMPTY_OVERRIDE_MAP = new Dictionary<string, object>();
        public bool IsImmuneDebuff;
        public bool muteEvents;
        public OnKillBehavior onKillBehavior;

        protected ActorAbilityPlugin(BaseAbilityActor abilityActor)
        {
            this._owner = abilityActor;
            this._appliedAbilities = new List<ActorAbility>();
            this._appliedModifiers = new List<ActorModifier>();
            this._deadModifiers = new List<ActorModifier>();
            this._displayValueMap = new Dictionary<string, DisplayValue<float>>();
            this._dynamicValueMap = new Dictionary<string, DynamicActorValue<float>>();
            this._modifierDurationTimers = new List<Tuple<ActorModifier, EntityTimer>>();
            this._modifierThinkTimers = new List<Tuple<ActorModifier, EntityTimer>>();
            this._additionalAbilities = new List<ConfigAbility>();
            this._levelActor = Singleton<LevelManager>.Instance.levelActor;
        }

        protected float _Internal_CalculateApplyLevelBuffDuration(ApplyLevelBuff config, ActorAbility instancedAbility, BaseEvent evt)
        {
            float num = instancedAbility.Evaluate(config.Duration);
            switch (config.LevelBuffSpecial)
            {
                case LevelBuffSpecial.None:
                    return num;

                case LevelBuffSpecial.WitchTimeDurationScaledByEvadedAttack:
                {
                    if (!(evt is EvtEvadeSuccess))
                    {
                        if (!(evt is EvtDefendSuccess))
                        {
                            return num;
                        }
                        EvtDefendSuccess success2 = (EvtDefendSuccess) evt;
                        MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(success2.attackerID);
                        if (actor2 == null)
                        {
                            return num;
                        }
                        ConfigMonsterAnimEvent event4 = SharedAnimEventData.ResolveAnimEvent(actor2.config, success2.skillID);
                        if (event4 == null)
                        {
                            return num;
                        }
                        return (num * event4.AttackProperty.WitchTimeRatio);
                    }
                    EvtEvadeSuccess success = (EvtEvadeSuccess) evt;
                    BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(success.attackerID);
                    if (actor == null)
                    {
                        return num;
                    }
                    if (actor is MonsterActor)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent((actor as MonsterActor).config, success.skillID);
                        if (event2 == null)
                        {
                            return num;
                        }
                        return (num * event2.AttackProperty.WitchTimeRatio);
                    }
                    if (!(actor is PropObjectActor))
                    {
                        return num;
                    }
                    ConfigEntityAnimEvent event3 = SharedAnimEventData.ResolveAnimEvent((actor as PropObjectActor).config, success.skillID);
                    if (event3 == null)
                    {
                        return num;
                    }
                    return (num * event3.AttackProperty.WitchTimeRatio);
                }
                case LevelBuffSpecial.InfiniteDuration:
                    return 999999f;
            }
            return num;
        }

        protected virtual void AbilityStartInstancedMixin(BaseAbilityMixin mixin, EvtAbilityStart evt)
        {
            mixin.OnAbilityTriggered(evt);
        }

        public ActorAbility AddAbility(ConfigAbility abilityConfig)
        {
            return this.AddAbility(abilityConfig, EMPTY_OVERRIDE_MAP);
        }

        public virtual ActorAbility AddAbility(ConfigAbility abilityConfig, Dictionary<string, object> overrideMap)
        {
            if (this.IsMuted())
            {
                return null;
            }
            ActorAbility item = new ActorAbility(this._owner, abilityConfig, overrideMap);
            this._appliedAbilities.Add(item);
            int num = this._appliedAbilities.Count - 1;
            item.instancedAbilityID = num + 1;
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = config => config.OnAdded;
            }
            this.HandleAbilityActions(item, null, null, <>f__am$cache13);
            item.Attach();
            this.AddInstancedMixins(item.instancedMixins);
            return this._appliedAbilities[num];
        }

        protected virtual void AddAppliedAbilities()
        {
            List<Tuple<ConfigAbility, Dictionary<string, object>>> appliedAbilities = this._owner.appliedAbilities;
            for (int i = 0; i < appliedAbilities.Count; i++)
            {
                this.AddAbility(appliedAbilities[i].Item1, appliedAbilities[i].Item2);
            }
            for (int j = 0; j < this._additionalAbilities.Count; j++)
            {
                this.AddAbility(this._additionalAbilities[j]);
            }
            this._additionalAbilities = null;
        }

        protected virtual void AddInstancedMixins(BaseAbilityMixin[] mixins)
        {
            for (int i = 0; i < mixins.Length; i++)
            {
                mixins[i].OnAdded();
            }
        }

        protected virtual ActorModifier AddModifierOnIndex(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig, int index)
        {
            string modifierName = modifierConfig.ModifierName;
            ActorModifier modifier = this.TryRecycleDeadModifier(instancedAbility, modifierConfig);
            if (modifier == null)
            {
                modifier = new ActorModifier(instancedAbility, this._owner, instancedAbility.config.Modifiers[modifierName]);
            }
            this._appliedModifiers.ExpandToInclude<ActorModifier>(index);
            this._appliedModifiers[index] = modifier;
            modifier.instancedModifierID = index + 1;
            modifier.Attach();
            this.AddInstancedMixins(modifier.instancedMixins);
            return modifier;
        }

        public void AddOrGetAbilityAndTriggerOnTarget(ConfigAbility abilityConfig, uint targetID, object abilityArgument)
        {
            if (abilityConfig == null)
            {
                SuperDebug.VeryImportantError("AbilityConfig is Empty in AddOrGetAbilityAndTriggerOnTarget");
            }
            else if (string.IsNullOrEmpty(abilityConfig.AbilityName))
            {
                SuperDebug.VeryImportantError("AbilityConfig should have a abilityName");
            }
            else
            {
                if (!string.IsNullOrEmpty(abilityConfig.AbilityName) && !this.HasAbility(abilityConfig.AbilityName))
                {
                    this.AddAbility(abilityConfig);
                }
                EvtAbilityStart evt = new EvtAbilityStart(this._owner.runtimeID, targetID, null) {
                    abilityName = abilityConfig.AbilityName,
                    abilityArgument = abilityArgument
                };
                Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            }
        }

        public void ApplyLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            BaseLevelBuff witchTimeLevelBuff;
            LevelBuffSide overrideCurSide;
            ApplyLevelBuff config = (ApplyLevelBuff) actionConfig;
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            bool flag = true;
            if (this._levelActor.IsLevelBuffActive(config.LevelBuff))
            {
                flag = false;
            }
            float duration = this._Internal_CalculateApplyLevelBuffDuration(config, instancedAbility, evt);
            uint runtimeID = instancedAbility.caster.runtimeID;
            if (config.UseOverrideCurSide)
            {
                overrideCurSide = config.OverrideCurSide;
            }
            else
            {
                overrideCurSide = this.CalculateLevelBuffSide(runtimeID);
            }
            LevelBuffType levelBuff = config.LevelBuff;
            if (levelBuff == LevelBuffType.WitchTime)
            {
                Singleton<LevelManager>.Instance.levelActor.StartLevelBuff(levelActor.witchTimeLevelBuff, duration, config.LevelBuffAllowRefresh, config.EnteringTimeSlow, overrideCurSide, runtimeID, config.NotStartEffect);
                witchTimeLevelBuff = levelActor.witchTimeLevelBuff;
            }
            else if (levelBuff == LevelBuffType.StopWorld)
            {
                Singleton<LevelManager>.Instance.levelActor.StartLevelBuff(levelActor.stopWorldLevelBuff, duration, config.LevelBuffAllowRefresh, config.EnteringTimeSlow, overrideCurSide, runtimeID, config.NotStartEffect);
                witchTimeLevelBuff = levelActor.stopWorldLevelBuff;
            }
            else
            {
                witchTimeLevelBuff = null;
            }
            if (flag)
            {
                List<ActorModifier> list = new List<ActorModifier>();
                for (int i = 0; i < config.AttachModifiers.Length; i++)
                {
                    BaseAbilityActor actor2;
                    BaseAbilityActor[] actorArray;
                    bool flag2;
                    AttachModifier modifier = config.AttachModifiers[i];
                    this.ResolveTarget(modifier.Target, modifier.TargetOption, instancedAbility, target, out actor2, out actorArray, out flag2);
                    if ((actor2 != null) || flag2)
                    {
                        ActorModifier item = actor2.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                    else if (actorArray != null)
                    {
                        for (int k = 0; k < actorArray.Length; k++)
                        {
                            if (actorArray[k] != null)
                            {
                                ActorModifier modifier3 = actorArray[k].abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
                                if (modifier3 != null)
                                {
                                    list.Add(modifier3);
                                }
                            }
                        }
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    this._levelActor.GetPlugin<LevelAbilityHelperPlugin>().AddLevelBuffModifier(config.LevelBuff, list[j]);
                }
                if (config.AttachLevelEffectPattern != null)
                {
                    this._levelActor.GetPlugin<LevelAbilityHelperPlugin>().AttachLevelBuffEffect(config.LevelBuff, config.AttachLevelEffectPattern);
                }
            }
        }

        protected virtual ActorModifier ApplyModifier(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig)
        {
            if (this.IsMuted())
            {
                return null;
            }
            string modifierName = modifierConfig.ModifierName;
            if ((modifierConfig.State != AbilityState.None) && this._owner.IsImmuneAbilityState(modifierConfig.State))
            {
                return null;
            }
            if (AbilityData.IsModifierDebuff(modifierConfig))
            {
                if (AbilityData.IsModifierDebuff(modifierConfig) && this.IsImmuneDebuff)
                {
                    return null;
                }
                if (this._owner is MonsterActor)
                {
                    MonsterActor actor = (MonsterActor) this._owner;
                    float resistanceRatio = actor.config.DebuffResistance.ResistanceRatio;
                    float debuffResistanceRatio = actor.config.EliteArguments.DebuffResistanceRatio;
                    float num3 = actor.GetResistanceRatio(modifierConfig.State);
                    float num4 = UnityEngine.Random.value;
                    float num5 = ((1f - resistanceRatio) * (1f - debuffResistanceRatio)) * num3;
                    if (num4 >= num5)
                    {
                        return null;
                    }
                }
                else if (this._owner is AvatarActor)
                {
                    AvatarActor actor2 = (AvatarActor) this._owner;
                    float num6 = actor2.config.DebuffResistance.ResistanceRatio;
                    float num7 = actor2.GetResistanceRatio(modifierConfig.State);
                    float num8 = UnityEngine.Random.value;
                    float num9 = (1f - num6) * num7;
                    if (num8 >= num9)
                    {
                        return null;
                    }
                }
            }
            ActorModifier appliedModifier = this.GetAppliedModifier(modifierConfig, instancedAbility);
            if (appliedModifier != null)
            {
                if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Unique)
                {
                    return null;
                }
                if (modifierConfig.Stacking != ConfigAbilityModifier.ModifierStacking.Multiple)
                {
                    for (int i = 0; i < this._modifierDurationTimers.Count; i++)
                    {
                        if ((this._modifierDurationTimers[i] != null) && (this._modifierDurationTimers[i].Item1 == appliedModifier))
                        {
                            EntityTimer timer = this._modifierDurationTimers[i].Item2;
                            float num11 = instancedAbility.Evaluate(modifierConfig.Duration);
                            if (AbilityData.IsModifierDebuff(modifierConfig))
                            {
                                num11 *= 1f + this._owner.GetProperty("Actor_DebuffDurationRatioDelta");
                            }
                            if (AbilityData.IsModifierBuff(modifierConfig) || AbilityData.IsModifierDebuff(modifierConfig))
                            {
                                num11 *= this._owner.GetAbilityStateDurationRatio(modifierConfig.State);
                            }
                            if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Prolong)
                            {
                                timer.timespan = (timer.timespan - timer.timer) + num11;
                                timer.Reset();
                            }
                            else if (modifierConfig.Stacking == ConfigAbilityModifier.ModifierStacking.Refresh)
                            {
                                timer.Reset();
                            }
                        }
                    }
                    return null;
                }
            }
            if (modifierConfig.State != AbilityState.None)
            {
                AbilityState[] precedenceTrack = null;
                int stateIx = -1;
                AbilityData.GetStateIndiceInPrecedenceMap(modifierConfig.State, out precedenceTrack, out stateIx);
                for (int j = 0; j < AbilityData.ABILITY_STATE_PRECEDENCE_MAP.Length; j++)
                {
                    AbilityState[] stateArray2 = AbilityData.ABILITY_STATE_PRECEDENCE_MAP[j];
                    for (int k = 0; k < stateArray2.Length; k++)
                    {
                        if (stateArray2[k] == modifierConfig.State)
                        {
                            precedenceTrack = stateArray2;
                            stateIx = k;
                            for (int m = 0; m < stateIx; m++)
                            {
                                int modifierIndexByState = this.GetModifierIndexByState(precedenceTrack[m]);
                                if (((modifierIndexByState >= 0) ? this._appliedModifiers[modifierIndexByState] : null) != null)
                                {
                                    return null;
                                }
                            }
                            for (int n = stateIx + 1; n < precedenceTrack.Length; n++)
                            {
                                this.RemoveModifierByState(precedenceTrack[n]);
                            }
                        }
                    }
                }
            }
            bool flag = false;
            if ((modifierConfig.State != AbilityState.None) && modifierConfig.IsDebuff)
            {
                int num18 = this.GetModifierIndexByState(modifierConfig.State);
                ActorModifier modifier = (num18 >= 0) ? this._appliedModifiers[num18] : null;
                if (modifier != null)
                {
                    if (modifier.config.IsDebuff)
                    {
                        this._owner.AddAbilityState(modifierConfig.State, modifier.config.MuteStateDisplayEffect);
                        flag = true;
                        this.RemoveModifier(modifier, num18);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            int index = this._appliedModifiers.SeekAddPosition<ActorModifier>();
            appliedModifier = this.AddModifierOnIndex(instancedAbility, modifierConfig, index);
            if (flag)
            {
                this._owner.RemoveAbilityState(modifierConfig.State);
            }
            BaseMonoEntity timeScaleEntity = null;
            if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Owner)
            {
                timeScaleEntity = this._owner.entity;
            }
            else if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Caster)
            {
                timeScaleEntity = instancedAbility.caster.entity;
            }
            else if (modifierConfig.TimeScale == ConfigAbilityModifier.ModifierTimeScale.Level)
            {
                timeScaleEntity = this._levelActor.levelEntity;
            }
            float timespan = instancedAbility.Evaluate(appliedModifier.config.Duration);
            if (modifierConfig.ApplyAttackerWitchTimeRatio && (instancedAbility.CurrentTriggerEvent is EvtEvadeSuccess))
            {
                EvtEvadeSuccess currentTriggerEvent = instancedAbility.CurrentTriggerEvent as EvtEvadeSuccess;
                if (currentTriggerEvent != null)
                {
                    MonsterActor actor3 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(currentTriggerEvent.attackerID);
                    if (actor3 != null)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor3.config, currentTriggerEvent.skillID);
                        if (event2 != null)
                        {
                            timespan *= event2.AttackProperty.WitchTimeRatio;
                        }
                    }
                }
            }
            if (timespan != 0f)
            {
                if (AbilityData.IsModifierDebuff(modifierConfig))
                {
                    timespan *= 1f + this._owner.GetProperty("Actor_DebuffDurationRatioDelta");
                }
                if (AbilityData.IsModifierBuff(modifierConfig) || AbilityData.IsModifierDebuff(modifierConfig))
                {
                    timespan *= this._owner.GetAbilityStateDurationRatio(modifierConfig.State);
                }
                int num21 = this._modifierDurationTimers.SeekAddPosition<Tuple<ActorModifier, EntityTimer>>();
                this._modifierDurationTimers[num21] = Tuple.Create<ActorModifier, EntityTimer>(appliedModifier, new EntityTimer(timespan, timeScaleEntity));
                this._modifierDurationTimers[num21].Item2.SetActive(true);
            }
            float num22 = instancedAbility.Evaluate(appliedModifier.config.ThinkInterval);
            if (num22 != 0f)
            {
                int num23 = this._modifierThinkTimers.SeekAddPosition<Tuple<ActorModifier, EntityTimer>>();
                this._modifierThinkTimers[num23] = Tuple.Create<ActorModifier, EntityTimer>(appliedModifier, new EntityTimer(num22, timeScaleEntity));
                this._modifierThinkTimers[num23].Item2.SetActive(true);
            }
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = config => config.OnAdded;
            }
            this.HandleModifierActions(appliedModifier, null, null, <>f__am$cache15);
            if (appliedModifier.config.OnMonsterCreated.Length > 0)
            {
                Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(this._owner.runtimeID);
            }
            if (appliedModifier.config.OnAvatarCreated.Length > 0)
            {
                Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(this._owner.runtimeID);
            }
            return appliedModifier;
        }

        public ActorModifier ApplyModifier(ActorAbility instancedAbility, string modifierName)
        {
            ConfigAbilityModifier modifierConfig = instancedAbility.config.Modifiers[modifierName];
            return this.ApplyModifier(instancedAbility, modifierConfig);
        }

        public void ApplyModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            MoleMole.Config.ApplyModifier modifier = (MoleMole.Config.ApplyModifier) actionConfig;
            if (evt is EvtEvadeSuccess)
            {
                instancedAbility.CurrentTriggerEvent = evt;
            }
            target.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
        }

        public void AttachAllowSelectionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachAllowSelection selection = (AttachAllowSelection) actionConfig;
                context.AttachAllowSelected(target.entity, selection.AllowSelection);
            }
        }

        public void AttachAnimEventPredicateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachAnimEventPredicate predicate = (AttachAnimEventPredicate) actionConfig;
            target.entity.AddAnimEventPredicate(predicate.AnimEventPredicate);
            if (instancedModifier != null)
            {
                instancedModifier.AttachAnimEventPredicate(target.entity, predicate.AnimEventPredicate);
            }
            else
            {
                instancedAbility.AttachAnimEventPredicate(target.entity, predicate.AnimEventPredicate);
            }
        }

        public void AttachBuffDebufResistanceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachBuffDebuffResistance resistance = (AttachBuffDebuffResistance) actionConfig;
                context.AttachBuffDebuffResistance(target, resistance);
            }
        }

        public void AttachDisableHitboxHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachIsGhost ghost = (AttachIsGhost) actionConfig;
                context.AttachIsGhost(target.entity, ghost.IsGhost);
            }
        }

        public void AttachEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachEffect effect = (AttachEffect) actionConfig;
            int patternIx = target.entity.AttachEffect(effect.EffectPattern);
            if (instancedModifier != null)
            {
                instancedModifier.AttachEffectPatternIndex(patternIx);
            }
            else
            {
                instancedAbility.AttachEffectPatternIndex(patternIx);
            }
        }

        public void AttachEffectOverrideHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachEffectOverride @override = (AttachEffectOverride) actionConfig;
            target.entity.AddEffectOverride(@override.EffectOverrideKey, @override.EffectPattern);
            ((instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier)).AttachEffectOverride(target.entity, @override.EffectOverrideKey);
        }

        public void AttachImmuneAbilityStateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachImmuneAbilityState state = (AttachImmuneAbilityState) actionConfig;
            if (state.ClearAppliedState && ((target.abilityState & state.ImmuneState) != AbilityState.None))
            {
                target.RemoveAbilityState(state.ImmuneState);
            }
            target.SetAbilityStateImmune(state.ImmuneState, true);
            if (instancedModifier != null)
            {
                instancedModifier.AttachImmuneAbilityState(target, state.ImmuneState);
            }
            else
            {
                instancedAbility.AttachImmuneAbilityState(target, state.ImmuneState);
            }
        }

        public void AttachImmuneDebuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachImmuneDebuff debuff = (AttachImmuneDebuff) actionConfig;
            if (debuff.ClearAppliedDebuff)
            {
                target.abilityPlugin.RemoveAllDebuffModifiers();
            }
            target.SetImmuneDebuff(true);
            ((instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier)).AttachImmuneDebuff(target);
        }

        public void AttachMaskAnimEventIDsHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachMaskAnimEventIDs ds = (AttachMaskAnimEventIDs) actionConfig;
            BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
            for (int i = 0; i < ds.AnimEventIDs.Length; i++)
            {
                string animEventName = ds.AnimEventIDs[i];
                target.entity.MaskAnimEvent(animEventName);
                context.AttachMaskedAnimEventID(target.entity, animEventName);
            }
        }

        public void AttachMaterialGroupHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachMaterialGroup group = (AttachMaterialGroup) actionConfig;
            target.entity.PushMaterialGroup(group.MaterialGroupName);
            if (instancedModifier != null)
            {
                instancedModifier.AttachPushMaterialGroup(target.entity);
            }
            else
            {
                instancedAbility.AttachPushMaterialGroup(target.entity);
            }
        }

        public void AttachModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachModifier modifier = (AttachModifier) actionConfig;
            ActorModifier modifier2 = target.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
            if (modifier2 != null)
            {
                if (instancedModifier != null)
                {
                    instancedModifier.AttachModifier(modifier2);
                }
                else
                {
                    instancedAbility.AttachModifier(modifier2);
                }
            }
        }

        public void AttachNoCollisionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                ((instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier)).AttachNoCollision(target.entity);
            }
        }

        public void AttachOpacityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachOpacity opacity = (AttachOpacity) actionConfig;
                context.AttachOpacity(target.entity, instancedAbility.Evaluate(opacity.Opacity));
            }
        }

        public void AttachShaderHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachShader shader = (AttachShader) actionConfig;
            float enableDuration = !shader.UsePrefabEnableDurtion ? instancedAbility.Evaluate(shader.Duration) : -1f;
            target.entity.SetShaderDataLerp(shader.ShaderType, true, enableDuration, -1f, shader.UseNewTexture);
        }

        public void AttachTintHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttachStageTint tintConfig = (AttachStageTint) actionConfig;
            ((instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier)).AttachStageTint(tintConfig);
        }

        public void AvatarSkillStartHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AvatarSkillStart start = (AvatarSkillStart) actionConfig;
            if (instancedAbility.caster is AvatarActor)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(target.runtimeID, start.CDSkillID), MPEventDispatchMode.Normal);
            }
        }

        public bool ByAnimatorBoolTrueHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAnimatorBoolTrue @true = predConfig as ByAnimatorBoolTrue;
            if (target == null)
            {
                return false;
            }
            return (target.entity as BaseMonoAnimatorEntity).GetLocomotionBool(@true.Param);
        }

        public bool ByAnyHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAny any = predConfig as ByAny;
            for (int i = 0; i < any.Predicates.Length; i++)
            {
                if (this.EvaluateAbilityPredicate(any.Predicates[i], instancedAbility, instancedModifier, target, evt))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ByAttackAniDamageRatioHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAttackAniDamageRatio ratio = (ByAttackAniDamageRatio) predConfig;
            AttackData attackResult = this.GetAttackResult(evt) as AttackData;
            if (attackResult != null)
            {
                switch (ratio.CompareType)
                {
                    case ByAttackAniDamageRatio.LogicType.MoreThan:
                        return (attackResult.attackerAniDamageRatio > ratio.AniDamageRatio);

                    case ByAttackAniDamageRatio.LogicType.LessThan:
                        return (attackResult.attackerAniDamageRatio < ratio.AniDamageRatio);
                }
            }
            return false;
        }

        public bool ByAttackAnimEventIDHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            string animEventID = this.GetAnimEventID(evt);
            if (animEventID == null)
            {
                return false;
            }
            return (((ByAttackAnimEventID) predConfig).ByAnyEventID || Miscs.ArrayContains<string>(((ByAttackAnimEventID) predConfig).AnimEventIDs, animEventID));
        }

        public bool ByAttackCategoryTagHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAttackCategoryTag tag = (ByAttackCategoryTag) predConfig;
            if (target == null)
            {
                return false;
            }
            IEvtWithAttackResult result = evt as IEvtWithAttackResult;
            if (result == null)
            {
                return false;
            }
            AttackResult attackResult = result.GetAttackResult();
            if (attackResult == null)
            {
                return false;
            }
            return attackResult.attackCategoryTag.ContainsTag(tag.CategoryTag);
        }

        public bool ByAttackDataTypeHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAttackDataType type = predConfig as ByAttackDataType;
            AttackResult attackResult = this.GetAttackResult(evt);
            if (attackResult == null)
            {
                return false;
            }
            if (type.Type == ByAttackDataType.AttackDataType.Breakable)
            {
                return (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light);
            }
            return ((type.Type == ByAttackDataType.AttackDataType.EvadeDefendable) && !attackResult.noTriggerEvadeAndDefend);
        }

        public bool ByAttackerCategoryHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAttackerCategory category = predConfig as ByAttackerCategory;
            EvtBeingHit hit = evt as EvtBeingHit;
            if (hit != null)
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(hit.sourceID);
                if (actor != null)
                {
                    ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID);
                    if (category.Category == num)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ByAttackHitFlagHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttackResult attackResult = this.GetAttackResult(evt);
            return ((attackResult != null) && attackResult.ContainHitFlag(((ByAttackHitFlag) predConfig).HitFlag));
        }

        public bool ByAttackHitTypeHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttackResult attackResult = this.GetAttackResult(evt);
            return ((attackResult != null) && (attackResult.hitType == ((ByAttackHitType) predConfig).HitType));
        }

        public bool ByAttackInComboCountHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttackResult attackResult = this.GetAttackResult(evt);
            return ((attackResult != null) && (attackResult.isInComboCount == ((ByAttackInComboCount) predConfig).InComboCount));
        }

        public bool ByAttackIsAnimEventHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            AttackResult attackResult = this.GetAttackResult(evt);
            return ((attackResult != null) && (attackResult.isAnimEventAttack == ((ByAttackFromAnimEvent) predConfig).IsAnimEventAttack));
        }

        public bool ByAttackTargetAnimStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAttackTargetAnimState state = (ByAttackTargetAnimState) predConfig;
            if (target == null)
            {
                return false;
            }
            if (state.State == ByAttackTargetAnimState.AnimState.Throw)
            {
                AvatarActor actor = target as AvatarActor;
                if (((actor != null) && (actor.avatar != null)) && (actor.avatar.AttackTarget is BaseMonoMonster))
                {
                    return ((BaseMonoMonster) actor.avatar.AttackTarget).IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw);
                }
            }
            return false;
        }

        public bool ByAvatarHasChargesLeftHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByAvatarHasChargesLeft left = (ByAvatarHasChargesLeft) predConfig;
            AvatarActor actor = this._owner as AvatarActor;
            return actor.HasChargesLeft(left.CDSkillID);
        }

        public bool ByControlDataHasSteerHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByControlDataHasSteer steer = predConfig as ByControlDataHasSteer;
            BaseMonoAvatar entity = this._owner.entity as BaseMonoAvatar;
            return (!steer.HasSteer ? !entity.GetActiveControlData().hasSteer : entity.GetActiveControlData().hasSteer);
        }

        public bool ByHitDirectionHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            EvtBeingHit hit = evt as EvtBeingHit;
            if (hit.attackData != null)
            {
                if (!hit.attackData.isAnimEventAttack)
                {
                    return false;
                }
                if (hit.attackData.rejected)
                {
                    return false;
                }
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(hit.sourceID);
                bool flag = Vector3.Angle(instancedAbility.caster.entity.transform.forward, actor.entity.transform.position - instancedAbility.caster.entity.transform.position) < ((ByHitDirection) predConfig).Angle;
                if (((ByHitDirection) predConfig).ReverseAngle)
                {
                    flag = !flag;
                }
                if (flag)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ByIsLocalAvatarHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return Singleton<AvatarManager>.Instance.IsLocalAvatar(this._owner.runtimeID);
        }

        public bool ByIsPlayerAvatarHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return Singleton<AvatarManager>.Instance.IsPlayerAvatar(this._owner.runtimeID);
        }

        public bool ByNotHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByNot not = predConfig as ByNot;
            for (int i = 0; i < not.Predicates.Length; i++)
            {
                if (this.EvaluateAbilityPredicate(not.Predicates[i], instancedAbility, instancedModifier, target, evt))
                {
                    return false;
                }
            }
            return true;
        }

        public bool ByTargetAnimStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetAnimState state = (ByTargetAnimState) predConfig;
            if (target == null)
            {
                return false;
            }
            if ((state.State == ByTargetAnimState.AnimState.Throw) && (target is MonsterActor))
            {
                return ((MonsterActor) target).monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw);
            }
            return false;
        }

        public bool ByTargetAppliedUniqueModifierHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetAppliedUniqueModifier modifier = (ByTargetAppliedUniqueModifier) predConfig;
            if (target == null)
            {
                return false;
            }
            return (target.abilityPlugin.GetFirstUniqueModifier(modifier.UniquModifierName) != null);
        }

        public bool ByTargetClassHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return ((target != null) && (((ByTargetEntityClass) predConfig).TargetClass == target.commonConfig.CommonArguments.Class));
        }

        public bool ByTargetContainAnimEventPredicateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetContainAnimEventPredicate predicate = (ByTargetContainAnimEventPredicate) predConfig;
            if (predicate.ForceByCaster)
            {
                target = instancedAbility.caster;
            }
            if (target == null)
            {
                return false;
            }
            return target.entity.ContainAnimEventPredicate(predicate.AnimEventPredicate);
        }

        public bool ByTargetDistanceHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetDistance distance = (ByTargetDistance) predConfig;
            if (target != null)
            {
                Vector3 vector = target.entity.XZPosition - instancedAbility.caster.entity.XZPosition;
                float magnitude = vector.magnitude;
                switch (distance.Logic)
                {
                    case MixinPredicate.Greater:
                    case MixinPredicate.GreaterOrEqual:
                        return (magnitude > instancedAbility.Evaluate(distance.Distance));

                    case MixinPredicate.Lesser:
                        return (magnitude < instancedAbility.Evaluate(distance.Distance));
                }
            }
            return false;
        }

        public bool ByTargetInLevelAnimHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetInLevelAnim anim = (ByTargetInLevelAnim) predConfig;
            if (target == null)
            {
                return false;
            }
            return (target.isInLevelAnim == anim.InLevelAnim);
        }

        public bool ByTargetIsSelfHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetIsSelf self = predConfig as ByTargetIsSelf;
            if (target == null)
            {
                return false;
            }
            return !((target.runtimeID == this._owner.runtimeID) ^ self.IsSelf);
        }

        public bool ByTargetNatureHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return ((target != null) && (((ByTargetEntityNature) predConfig).TargetNature == target.commonConfig.CommonArguments.Nature));
        }

        public bool ByTargetQTENameHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetQTEName name = predConfig as ByTargetQTEName;
            AvatarActor actor = target as AvatarActor;
            return (((actor != null) && (name != null)) && (actor.CurrentQTEName == name.targetQTEName));
        }

        public bool ByTargetWithinAbilityStateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ByTargetWithinAbilityState state = (ByTargetWithinAbilityState) predConfig;
            if (state.TargetStates == null)
            {
                return ((target != null) && ((state.TargetState & target.abilityState) != AbilityState.None));
            }
            AbilityState none = AbilityState.None;
            for (int i = 0; i < state.TargetStates.Length; i++)
            {
                none |= state.TargetStates[i];
            }
            return ((target != null) && ((none & target.abilityState) != AbilityState.None));
        }

        protected LevelBuffSide CalculateLevelBuffSide(uint ownerID)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 3)
            {
                return LevelBuffSide.FromAvatar;
            }
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(ownerID) == 4)
            {
                return LevelBuffSide.FromMonster;
            }
            return LevelBuffSide.FromLevel;
        }

        public void CameraActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DoCameraAction action = (DoCameraAction) actionConfig;
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(target.runtimeID))
            {
                Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(target.runtimeID).DoCameraAction(action.CameraAction);
            }
        }

        public void CameraExposureHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ActCameraExposure exposure = (ActCameraExposure) actionConfig;
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (mainCamera != null)
            {
                mainCamera.ActExposureEffect(exposure.ExposureTime, exposure.KeepTime, exposure.RecoverTime, exposure.MaxExposure);
            }
        }

        public void CameraGlareHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ActCameraGlare glare = (ActCameraGlare) actionConfig;
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (mainCamera != null)
            {
                mainCamera.ActGlareEffect(glare.GlareTime, glare.KeepTime, glare.RecoverTime, glare.TargetRate);
            }
        }

        public void CameraShakeHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ActCameraShake shake = (ActCameraShake) actionConfig;
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(target.runtimeID))
            {
                AttackPattern.ActCameraShake(shake.CameraShake);
            }
        }

        private BaseAbilityActor CheckTargetAvailable(BaseAbilityActor target, bool includeGhost = false)
        {
            if ((target == null) || (target.entity == null))
            {
                return null;
            }
            if (!includeGhost && target.entity.isGhost)
            {
                return null;
            }
            return target;
        }

        public void ClearComboHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            Singleton<LevelManager>.Instance.levelActor.ResetCombo();
        }

        public override void Core()
        {
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                ActorAbility ability = this._appliedAbilities[i];
                if (ability != null)
                {
                    this.CoreMixins(ability.instancedMixins);
                }
            }
            for (int j = 0; j < this._appliedModifiers.Count; j++)
            {
                ActorModifier modifier = this._appliedModifiers[j];
                if (modifier != null)
                {
                    this.CoreMixins(modifier.instancedMixins);
                }
            }
            for (int k = 0; k < this._modifierDurationTimers.Count; k++)
            {
                if (this._modifierDurationTimers[k] != null)
                {
                    this._modifierDurationTimers[k].Item2.Core(1f);
                    if (this._modifierDurationTimers[k].Item2.isTimeUp)
                    {
                        this.TryRemoveModifier(this._modifierDurationTimers[k].Item1);
                        this._modifierDurationTimers[k] = null;
                    }
                }
            }
            for (int m = 0; m < this._modifierThinkTimers.Count; m++)
            {
                if (this._modifierThinkTimers[m] != null)
                {
                    this._modifierThinkTimers[m].Item2.Core(1f);
                    if (this._modifierThinkTimers[m].Item2.isTimeUp)
                    {
                        if (<>f__am$cache12 == null)
                        {
                            <>f__am$cache12 = config => config.OnThinkInterval;
                        }
                        this.HandleModifierActions(this._modifierThinkTimers[m].Item1, null, null, <>f__am$cache12);
                        this._modifierThinkTimers[m].Item2.Reset(true);
                    }
                }
            }
        }

        protected virtual void CoreMixins(BaseAbilityMixin[] mixins)
        {
            for (int i = 0; i < mixins.Length; i++)
            {
                mixins[i].Core();
            }
        }

        public void CreateGoodsHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            CreateGoods goods = (CreateGoods) actionConfig;
            Singleton<DynamicObjectManager>.Instance.CreateGood(0x21800001, goods.GoodType, goods.GoodAbility, instancedAbility.Evaluate(goods.GoodArgument), target.entity.XZPosition, target.entity.transform.forward, true, false);
        }

        public virtual BaseAbilityMixin CreateInstancedAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
        {
            return config.CreateInstancedMixin(instancedAbility, instancedModifier);
        }

        public DisplayValue<float> CreateOrGetDisplayFloat(string key, float floor, float ceiling, float value)
        {
            if (this._displayValueMap.ContainsKey(key))
            {
                return this._displayValueMap[key];
            }
            DisplayValue<float> value2 = new DisplayValue<float>(floor, ceiling, value);
            this._displayValueMap.Add(key, value2);
            return value2;
        }

        public DynamicActorValue<float> CreateOrGetDynamicFloat(string key, float value)
        {
            if (this._dynamicValueMap.ContainsKey(key))
            {
                return this._dynamicValueMap[key];
            }
            DynamicActorValue<float> value2 = new DynamicActorValue<float>(value);
            this._dynamicValueMap.Add(key, value2);
            return value2;
        }

        public void CreateUnitFieldHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            CreateUnitField field = (CreateUnitField) actionConfig;
            string propName = "Trap_Slow";
            Vector3 xZPosition = instancedAbility.caster.entity.XZPosition;
            uint runtimeID = Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, propName, 1f, 1f, xZPosition, instancedAbility.caster.entity.transform.forward, false);
            MonoTriggerUnitFieldProp propObjectByRuntimeID = (MonoTriggerUnitFieldProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
            propObjectByRuntimeID.InitUnitFieldPropRange(field.numberX, field.numberZ);
            propObjectByRuntimeID.EnableProp();
        }

        public void DamageByAnimEventIDHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DamageByAnimEventID tid = (DamageByAnimEventID) actionConfig;
            BaseActor caster = instancedAbility.caster;
            AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(caster, tid.AnimEventID);
            bool forceSkipAttackerResolve = !caster.IsEntityExists();
            AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, tid.AnimEventID, null, attackData, forceSkipAttackerResolve, MPEventDispatchMode.Normal);
        }

        public void DamageByAttackPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DamageByAttackProperty property = (DamageByAttackProperty) actionConfig;
            BaseActor caster = instancedAbility.caster;
            AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, property.AttackProperty, property.AttackEffect, property.CameraShake);
            attackData.hitType = AttackResult.ActorHitType.Ailment;
            bool forceSkipAttackerResolve = !caster.IsEntityExists();
            AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve, MPEventDispatchMode.Normal);
        }

        public void DamageByAttackValueHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DamageByAttackValue value2 = (DamageByAttackValue) actionConfig;
            BaseActor caster = instancedAbility.caster;
            _attackProperty.DamagePercentage = instancedAbility.Evaluate(value2.DamagePercentage);
            _attackProperty.AddedDamageValue = instancedAbility.Evaluate(value2.AddedDamageValue);
            _attackProperty.NormalDamage = instancedAbility.Evaluate(value2.PlainDamage);
            _attackProperty.NormalDamagePercentage = instancedAbility.Evaluate(value2.PlainDamagePercentage);
            _attackProperty.FireDamage = instancedAbility.Evaluate(value2.FireDamage);
            _attackProperty.FireDamagePercentage = instancedAbility.Evaluate(value2.FireDamagePercentage);
            _attackProperty.ThunderDamage = instancedAbility.Evaluate(value2.ThunderDamage);
            _attackProperty.ThunderDamagePercentage = instancedAbility.Evaluate(value2.ThunderDamagePercentage);
            _attackProperty.IceDamage = instancedAbility.Evaluate(value2.IceDamage);
            _attackProperty.IceDamagePercentage = instancedAbility.Evaluate(value2.IceDamagePercentage);
            _attackProperty.AlienDamage = instancedAbility.Evaluate(value2.AlienDamage);
            _attackProperty.AlienDamagePercentage = instancedAbility.Evaluate(value2.AlienDamagePercentage);
            _attackProperty.AniDamageRatio = instancedAbility.Evaluate(value2.AniDamageRatio);
            _attackProperty.FrameHalt = instancedAbility.Evaluate(value2.FrameHalt);
            _attackProperty.HitType = AttackResult.ActorHitType.Ailment;
            _attackProperty.HitEffect = value2.HitEffect;
            _attackProperty.RetreatVelocity = instancedAbility.Evaluate(value2.RetreatVelocity);
            _attackProperty.IsAnimEventAttack = value2.IsAnimEventAttack;
            _attackProperty.IsInComboCount = value2.IsInComboCount;
            AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, _attackProperty, value2.AttackEffect, value2.CameraShake);
            attackData.hitLevel = value2.HitLevel;
            bool forceSkipAttackerResolve = !caster.IsEntityExists();
            AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve, MPEventDispatchMode.Normal);
        }

        public EntityTimer DebugGetModifierTimer(ActorModifier instancedModifier)
        {
            foreach (Tuple<ActorModifier, EntityTimer> tuple in this._modifierDurationTimers)
            {
                if ((tuple != null) && (tuple.Item1 == instancedModifier))
                {
                    return tuple.Item2;
                }
            }
            return null;
        }

        [Conditional("NG_HSOD_DEBUG"), Conditional("UNITY_EDITOR")]
        protected void DebugLogAbility(ActorAbility instancedAbility, string format, params object[] arguments)
        {
        }

        public void DebugLogHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
        }

        public void DetachAnimEventPredicateHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DetachAnimEventPredicate predicate = (DetachAnimEventPredicate) actionConfig;
            target.entity.RemoveAnimEventPredicate(predicate.AnimEventPredicate);
            if (instancedModifier != null)
            {
                instancedModifier.DetachAnimEventPredicate(target.entity, predicate.AnimEventPredicate);
            }
            else
            {
                instancedAbility.DetachAnimEventPredicate(target.entity, predicate.AnimEventPredicate);
            }
        }

        public void DetachShaderHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            DetachShader shader = (DetachShader) actionConfig;
            float disableDuration = !shader.UsePrefabDisableDurtion ? shader.Duration : -1f;
            target.entity.SetShaderDataLerp(shader.ShaderType, false, -1f, disableDuration, false);
        }

        public bool EvaluateAbilityPredicate(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return predConfig.Call(this, instancedAbility, instancedModifier, target, evt);
        }

        public bool EvaluateAbilityPredicate(ConfigAbilityPredicate[] predConfigs, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            bool flag = true;
            for (int i = 0; i < predConfigs.Length; i++)
            {
                flag &= this.EvaluateAbilityPredicate(predConfigs[i], instancedAbility, instancedModifier, target, evt);
            }
            return flag;
        }

        protected virtual bool EventInstancedMixin(BaseAbilityMixin mixin, BaseEvent evt)
        {
            return mixin.OnEvent(evt);
        }

        public BaseAbilityActor[] FilterTargetArray(BaseAbilityActor[] targets, bool includeGhost = false)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i] = this.CheckTargetAvailable(targets[i], includeGhost);
            }
            return targets;
        }

        public void FireAudioHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
        }

        public void FireEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            FireEffect effect = (FireEffect) actionConfig;
            if (effect.OwnedByLevel)
            {
                Singleton<LevelManager>.Instance.levelActor.entity.FireEffect(effect.EffectPattern, target.entity.XZPosition, target.entity.transform.forward);
            }
            else if ((target.entity != null) && target.entity.gameObject.activeSelf)
            {
                target.entity.FireEffect(effect.EffectPattern);
            }
        }

        public void FireEffectToTargetHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            FireEffectToTarget target2 = (FireEffectToTarget) actionConfig;
            if ((instancedAbility.caster.entity != null) && (target.entity != null))
            {
                if (target2.Reverse)
                {
                    target.entity.FireEffectTo(target2.EffectPattern, instancedAbility.caster.entity);
                }
                else
                {
                    instancedAbility.caster.entity.FireEffectTo(target2.EffectPattern, target.entity);
                }
            }
        }

        public void FireEventHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            FireEvent event2 = (FireEvent) actionConfig;
            EventCategory category = (EventCategory) ((int) Enum.Parse(typeof(EventCategory), event2.EvtCategory));
            if (category == EventCategory.EvtDefendStart)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtDefendStart(this._owner.runtimeID), MPEventDispatchMode.Normal);
            }
            if (((category == EventCategory.EvtDefendSuccess) && (evt != null)) && (evt is EvtBeingHit))
            {
                EvtBeingHit hit = evt as EvtBeingHit;
                Singleton<EventManager>.Instance.FireEvent(new EvtDefendSuccess(this._owner.runtimeID, hit.sourceID, hit.animEventID), MPEventDispatchMode.Normal);
            }
        }

        public void ForceKillHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ForceKill kill = (ForceKill) actionConfig;
            if (target.isAlive != 0)
            {
                target.ForceKill(this._owner.runtimeID, kill.KillEffect);
            }
        }

        private string GetAnimEventID(BaseEvent evt)
        {
            IEvtWithAnimEventID tid = evt as IEvtWithAnimEventID;
            if (tid != null)
            {
                return tid.GetAnimEventID();
            }
            return null;
        }

        public List<ActorAbility> GetAppliedAbilities()
        {
            return ((this._appliedAbilities == null) ? new List<ActorAbility>() : this._appliedAbilities);
        }

        private ActorModifier GetAppliedModifier(ConfigAbilityModifier modifierConfig, ActorAbility ownerAbility)
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && ((this._appliedModifiers[i].config == modifierConfig) && (this._appliedModifiers[i].parentAbility == ownerAbility)))
                {
                    return this._appliedModifiers[i];
                }
            }
            return null;
        }

        private AttackResult GetAttackResult(BaseEvent evt)
        {
            IEvtWithAttackResult result = evt as IEvtWithAttackResult;
            if (result != null)
            {
                return result.GetAttackResult();
            }
            return null;
        }

        public ActorModifier GetFirstUniqueModifier(string uniqueModifierName)
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && (this._appliedModifiers[i].config.IsUnique && (this._appliedModifiers[i].config.ModifierName == uniqueModifierName)))
                {
                    return this._appliedModifiers[i];
                }
            }
            return null;
        }

        private AttackResult.HitCollsion GetHitCollision(BaseEvent evt)
        {
            IEvtWithHitCollision collision = evt as IEvtWithHitCollision;
            if (collision != null)
            {
                return collision.GetHitCollision();
            }
            return null;
        }

        private int GetModifierIndexByState(AbilityState state)
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && (this._appliedModifiers[i].config.State == state))
                {
                    return i;
                }
            }
            return -1;
        }

        private uint GetOtherID(BaseEvent evt)
        {
            IEvtWithOtherID rid = evt as IEvtWithOtherID;
            if (rid != null)
            {
                return rid.GetOtherID();
            }
            return 0;
        }

        protected virtual bool HandleAbilityActions(BaseAbilityActor other, BaseEvent evt, Func<ConfigAbility, ConfigAbilityAction[]> actionsGetter)
        {
            bool flag = false;
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                if (this._appliedAbilities[i] != null)
                {
                    ActorAbility instancedAbility = this._appliedAbilities[i];
                    ConfigAbilityAction[] actionArray = actionsGetter(instancedAbility.config);
                    flag |= actionArray.Length > 0;
                    for (int j = 0; j < actionArray.Length; j++)
                    {
                        this.HandleActionTargetDispatch(actionArray[j], instancedAbility, null, other, evt);
                    }
                }
            }
            return flag;
        }

        protected virtual bool HandleAbilityActions(ActorAbility instancedAbility, BaseAbilityActor other, BaseEvent evt, Func<ConfigAbility, ConfigAbilityAction[]> actionsGetter)
        {
            ConfigAbilityAction[] actionArray = actionsGetter(instancedAbility.config);
            for (int i = 0; i < actionArray.Length; i++)
            {
                this.HandleActionTargetDispatch(actionArray[i], instancedAbility, null, other, evt);
            }
            return (actionArray.Length > 0);
        }

        protected virtual void HandleAction(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (this.EvaluateAbilityPredicate(actionConfig.Predicates, instancedAbility, instancedModifier, target, evt))
            {
                actionConfig.Call(this, actionConfig, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void HandleActionTargetDispatch(ConfigAbilityAction[] actionConfigs, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt)
        {
            for (int i = 0; i < actionConfigs.Length; i++)
            {
                this.HandleActionTargetDispatch(actionConfigs[i], instancedAbility, instancedModifier, other, evt);
            }
        }

        private void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt)
        {
            if (<>f__am$cache26 == null)
            {
                <>f__am$cache26 = actionTarget => YesTargetPredicate(actionTarget);
            }
            this.HandleActionTargetDispatch(actionConfig, instancedAbility, instancedModifier, other, evt, <>f__am$cache26);
        }

        protected virtual void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<BaseAbilityActor, bool> targetPredicate)
        {
            BaseAbilityActor actor;
            BaseAbilityActor[] actorArray;
            bool flag;
            this.ResolveTarget(actionConfig.Target, actionConfig.TargetOption, instancedAbility, other, out actor, out actorArray, out flag);
            if (((actor != null) && targetPredicate(actor)) || flag)
            {
                this.HandleAction(actionConfig, instancedAbility, instancedModifier, actor, evt);
            }
            else if (actorArray != null)
            {
                for (int i = 0; i < actorArray.Length; i++)
                {
                    if ((actorArray[i] != null) && targetPredicate(actorArray[i]))
                    {
                        this.HandleAction(actionConfig, instancedAbility, instancedModifier, actorArray[i], evt);
                    }
                }
            }
        }

        protected virtual bool HandleModifierActions(BaseAbilityActor other, BaseEvent evt, Func<ConfigAbilityModifier, ConfigAbilityAction[]> actionsGetter)
        {
            bool flag = false;
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if (this._appliedModifiers[i] != null)
                {
                    ActorModifier instancedModifier = this._appliedModifiers[i];
                    ConfigAbilityAction[] actionArray = actionsGetter(instancedModifier.config);
                    flag |= actionArray.Length > 0;
                    for (int j = 0; j < actionArray.Length; j++)
                    {
                        this.HandleActionTargetDispatch(actionArray[j], instancedModifier.parentAbility, instancedModifier, other, evt);
                    }
                }
            }
            return flag;
        }

        protected virtual bool HandleModifierActions(ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<ConfigAbilityModifier, ConfigAbilityAction[]> actionsGetter)
        {
            ConfigAbilityAction[] actionArray = actionsGetter(instancedModifier.config);
            for (int i = 0; i < actionArray.Length; i++)
            {
                this.HandleActionTargetDispatch(actionArray[i], instancedModifier.parentAbility, instancedModifier, other, evt);
            }
            return (actionArray.Length > 0);
        }

        public bool HasAbility(string abilityName)
        {
            if (!string.IsNullOrEmpty(abilityName))
            {
                if (this._appliedAbilities == null)
                {
                    return false;
                }
                for (int i = 0; i < this._appliedAbilities.Count; i++)
                {
                    if (((this._appliedAbilities[i] != null) && (this._appliedAbilities[i].config != null)) && (this._appliedAbilities[i].config.AbilityName == abilityName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasDisplayFloat(string key)
        {
            return this._displayValueMap.ContainsKey(key);
        }

        public bool HasDynamicFloat(string key)
        {
            return this._dynamicValueMap.ContainsKey(key);
        }

        public void HealHPHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            HealHP lhp = (HealHP) actionConfig;
            float num = 0f;
            if (lhp.Amount != null)
            {
                num += instancedAbility.Evaluate(lhp.Amount);
            }
            if (lhp.AmountByCasterMaxHPRatio != null)
            {
                num += instancedAbility.caster.maxHP * instancedAbility.Evaluate(lhp.AmountByCasterMaxHPRatio);
            }
            if (lhp.AmountByTargetMaxHPRatio != null)
            {
                num += target.maxHP * instancedAbility.Evaluate(lhp.AmountByTargetMaxHPRatio);
            }
            if ((target.isAlive != 0) || ((target.abilityState & AbilityState.Limbo) != AbilityState.None))
            {
                target.HealHP(num * lhp.HealRatio);
                if ((!lhp.MuteHealEffect && (num > 0f)) && ((target != null) && target.entity.gameObject.activeSelf))
                {
                    target.entity.FireEffect("Ability_HealHP");
                }
            }
        }

        public void HealSPHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            HealSP lsp = (HealSP) actionConfig;
            if ((target.isAlive != 0) || ((target.abilityState & AbilityState.Limbo) != AbilityState.None))
            {
                float amount = instancedAbility.Evaluate(lsp.Amount);
                target.HealSP(amount);
                if ((!lsp.MuteHealEffect && (amount > 0f)) && ((target != null) && target.entity.gameObject.activeSelf))
                {
                    target.entity.FireEffect("Ability_HealSP");
                }
            }
        }

        public void InsertPreInitAbility(ConfigAbility abilityConfig)
        {
            this._additionalAbilities.Add(abilityConfig);
        }

        private bool IsModifierDurationed(ActorModifier modifier)
        {
            for (int i = 0; i < this._modifierDurationTimers.Count; i++)
            {
                if ((this._modifierDurationTimers[i] != null) && (this._modifierDurationTimers[i].Item1 == modifier))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsMuted()
        {
            return (this._isKilled || this._isMuted);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = false;
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                ActorAbility ability = this._appliedAbilities[i];
                if (ability != null)
                {
                    for (int k = 0; k < ability.instancedMixins.Length; k++)
                    {
                        flag |= ability.instancedMixins[k].ListenEvent(evt);
                    }
                }
            }
            for (int j = 0; j < this._appliedModifiers.Count; j++)
            {
                ActorModifier modifier = this._appliedModifiers[j];
                if (modifier != null)
                {
                    for (int m = 0; m < modifier.instancedMixins.Length; m++)
                    {
                        flag |= modifier.instancedMixins[m].ListenEvent(evt);
                    }
                }
            }
            if (evt is EvtMonsterCreated)
            {
                EvtMonsterCreated created = (EvtMonsterCreated) evt;
                if (<>f__am$cache22 == null)
                {
                    <>f__am$cache22 = config => config.OnMonsterCreated;
                }
                this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(created.monsterID), evt, <>f__am$cache22);
                return flag;
            }
            if (evt is EvtAvatarCreated)
            {
                EvtAvatarCreated created2 = (EvtAvatarCreated) evt;
                if (<>f__am$cache23 == null)
                {
                    <>f__am$cache23 = config => config.OnAvatarCreated;
                }
                this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(created2.avatarID), evt, <>f__am$cache23);
                return flag;
            }
            if (evt is EvtStageReady)
            {
                this.ListenStageReady((EvtStageReady) evt);
            }
            return flag;
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            if (this._waitForStageReady)
            {
                this.AddAppliedAbilities();
                Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(this._owner.runtimeID);
                this._waitForStageReady = false;
            }
            return true;
        }

        public void MissionTriggerAbilityActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            Singleton<MissionModule>.Instance.TryToUpdateTriggerAbilityAction((uint) ((MissionTriggerAbilityAction) actionConfig).FinishParaInt);
        }

        public void ModifyPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ModifyProperty property = (ModifyProperty) actionConfig;
            int index = instancedModifier.config.Properties.IndexOfKey(property.Property);
            int stackIx = instancedModifier.stackIndices[index];
            float num4 = target.GetPropertyByStackIndex(property.Property, stackIx) + instancedAbility.Evaluate(property.Delta);
            num4 = Mathf.Clamp(num4, instancedAbility.Evaluate(property.Min), instancedAbility.Evaluate(property.Max));
            target.SetPropertyByStackIndex(property.Property, stackIx, num4);
        }

        public void MuteAdditiveVelocityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            MuteAdditiveVelocity velocity = (MuteAdditiveVelocity) actionConfig;
            if ((target != null) && (target.isAlive != 0))
            {
                BaseMonoAnimatorEntity entity = target.entity as BaseMonoAnimatorEntity;
                if (entity != null)
                {
                    entity.MuteAdditiveVelocity = velocity.Mute;
                }
            }
        }

        public override void OnAdded()
        {
            if (this._levelActor.levelState == LevelActor.LevelState.LevelRunning)
            {
                this.AddAppliedAbilities();
            }
            else
            {
                Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(this._owner.runtimeID);
                this._waitForStageReady = true;
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (this.muteEvents)
            {
                return false;
            }
            bool flag = false;
            string abilityName = null;
            EvtAbilityStart start = null;
            if (evt is EvtAbilityStart)
            {
                start = (EvtAbilityStart) evt;
                if (start.abilityName == null)
                {
                    start.abilityName = this._owner.GetAbilityNameByID(start.abilityID);
                }
                abilityName = start.abilityName;
                if (abilityName == "Noop")
                {
                    return false;
                }
            }
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                ActorAbility ability = this._appliedAbilities[i];
                if (ability != null)
                {
                    bool flag2 = ability.config.AbilityName == abilityName;
                    for (int k = 0; k < ability.instancedMixins.Length; k++)
                    {
                        flag |= this.EventInstancedMixin(ability.instancedMixins[k], evt);
                        if (flag2)
                        {
                            flag = true;
                            if (ability.config.UseAbilityArgumentAsSpecialKey != null)
                            {
                                ability.argumentRecieved = true;
                                ability.argumentSpecialValue = (float) start.abilityArgument;
                            }
                            else if (ability.config.SetAbilityArgumentToOverrideMap != null)
                            {
                                ability.SetOverrideMapValue(ability.config.SetAbilityArgumentToOverrideMap, start.abilityArgument);
                            }
                            this.AbilityStartInstancedMixin(ability.instancedMixins[k], start);
                            ability.argumentRecieved = false;
                        }
                    }
                }
            }
            for (int j = 0; j < this._appliedModifiers.Count; j++)
            {
                ActorModifier modifier = this._appliedModifiers[j];
                if (modifier != null)
                {
                    bool flag3 = modifier.parentAbility.config.AbilityName == abilityName;
                    for (int m = 0; m < modifier.instancedMixins.Length; m++)
                    {
                        flag |= this.EventInstancedMixin(modifier.instancedMixins[m], evt);
                        if (flag3)
                        {
                            flag = true;
                            this.AbilityStartInstancedMixin(modifier.instancedMixins[m], start);
                        }
                    }
                }
            }
            if (evt is EvtAbilityStart)
            {
                for (int n = 0; n < this._appliedAbilities.Count; n++)
                {
                    ActorAbility instancedAbility = this._appliedAbilities[n];
                    if ((instancedAbility != null) && (instancedAbility.config.AbilityName == abilityName))
                    {
                        if (instancedAbility.config.UseAbilityArgumentAsSpecialKey != null)
                        {
                            instancedAbility.argumentRecieved = true;
                            instancedAbility.argumentSpecialValue = (float) start.abilityArgument;
                        }
                        else if (instancedAbility.config.SetAbilityArgumentToOverrideMap != null)
                        {
                            instancedAbility.SetOverrideMapValue(instancedAbility.config.SetAbilityArgumentToOverrideMap, start.abilityArgument);
                        }
                        if (<>f__am$cache17 == null)
                        {
                            <>f__am$cache17 = config => config.OnAbilityStart;
                        }
                        flag |= this.HandleAbilityActions(instancedAbility, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(start.otherID), evt, <>f__am$cache17);
                        instancedAbility.argumentRecieved = false;
                    }
                }
                return flag;
            }
            if (evt is EvtBeingHit)
            {
                EvtBeingHit hit = (EvtBeingHit) evt;
                if (<>f__am$cache18 == null)
                {
                    <>f__am$cache18 = config => config.OnBeingHit;
                }
                return (flag | this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(hit.sourceID), evt, <>f__am$cache18));
            }
            if (evt is EvtAttackLanded)
            {
                EvtAttackLanded landed = (EvtAttackLanded) evt;
                if (<>f__am$cache19 == null)
                {
                    <>f__am$cache19 = config => config.OnAttackLanded;
                }
                return (flag | this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(landed.attackeeID), evt, <>f__am$cache19));
            }
            if (evt is EvtEvadeStart)
            {
                if (<>f__am$cache1A == null)
                {
                    <>f__am$cache1A = config => config.OnEvadeStart;
                }
                return (flag | this.HandleModifierActions(null, evt, <>f__am$cache1A));
            }
            if (evt is EvtEvadeSuccess)
            {
                EvtEvadeSuccess success = (EvtEvadeSuccess) evt;
                if (<>f__am$cache1B == null)
                {
                    <>f__am$cache1B = config => config.OnEvadeSuccess;
                }
                return (flag | this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(success.attackerID), evt, <>f__am$cache1B));
            }
            if (evt is EvtDefendStart)
            {
                if (<>f__am$cache1C == null)
                {
                    <>f__am$cache1C = config => config.OnDefendStart;
                }
                return (flag | this.HandleModifierActions(null, evt, <>f__am$cache1C));
            }
            if (evt is EvtDefendSuccess)
            {
                EvtDefendSuccess success2 = (EvtDefendSuccess) evt;
                if (<>f__am$cache1D == null)
                {
                    <>f__am$cache1D = config => config.OnDefendSuccess;
                }
                return (flag | this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(success2.attackerID), evt, <>f__am$cache1D));
            }
            if (evt is EvtFieldEnter)
            {
                EvtFieldEnter enter = (EvtFieldEnter) evt;
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(enter.otherID);
                if (actor == null)
                {
                    return flag;
                }
                if (<>f__am$cache1E == null)
                {
                    <>f__am$cache1E = config => config.OnFieldEnter;
                }
                return (flag | this.HandleAbilityActions(actor, evt, <>f__am$cache1E));
            }
            if (evt is EvtFieldExit)
            {
                EvtFieldExit exit = (EvtFieldExit) evt;
                BaseAbilityActor actor2 = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(exit.otherID);
                if (actor2 == null)
                {
                    return flag;
                }
                if (<>f__am$cache1F == null)
                {
                    <>f__am$cache1F = config => config.OnFieldExit;
                }
                return (flag | this.HandleAbilityActions(actor2, evt, <>f__am$cache1F));
            }
            if (!(evt is EvtKilled))
            {
                return flag;
            }
            BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(((EvtKilled) evt).killerID);
            if (<>f__am$cache20 == null)
            {
                <>f__am$cache20 = config => config.OnKilled;
            }
            this.HandleModifierActions(other, evt, <>f__am$cache20);
            if (<>f__am$cache21 == null)
            {
                <>f__am$cache21 = config => config.OnKilled;
            }
            this.HandleAbilityActions(other, evt, <>f__am$cache21);
            return this.OnKilled((EvtKilled) evt);
        }

        private bool OnKilled(EvtKilled evt)
        {
            this._isKilled = true;
            if (this.onKillBehavior == OnKillBehavior.RemoveAll)
            {
                this.RemoveAllNonOnDestroyAbilities();
                this.RemoveAllModifies();
            }
            else if (this.onKillBehavior == OnKillBehavior.RemoveAllDebuffsAndDurationed)
            {
                this.RemoveAllDurationedOrDebuffs();
            }
            else if (this.onKillBehavior == OnKillBehavior.DoNotRemoveUntilDestroyed)
            {
            }
            return true;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (this.muteEvents)
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                ActorAbility ability = this._appliedAbilities[i];
                if (ability != null)
                {
                    for (int k = 0; k < ability.instancedMixins.Length; k++)
                    {
                        flag |= this.PostEventInstancedMixin(ability.instancedMixins[k], evt);
                    }
                }
            }
            for (int j = 0; j < this._appliedModifiers.Count; j++)
            {
                ActorModifier modifier = this._appliedModifiers[j];
                if (modifier != null)
                {
                    for (int m = 0; m < modifier.instancedMixins.Length; m++)
                    {
                        flag |= this.PostEventInstancedMixin(modifier.instancedMixins[m], evt);
                    }
                }
            }
            return flag;
        }

        public override void OnRemoved()
        {
            if (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)
            {
                if (<>f__am$cache25 == null)
                {
                    <>f__am$cache25 = config => config.OnDestroy;
                }
                this.HandleAbilityActions(null, null, <>f__am$cache25);
                this.RemoveAllModifies();
                this.RemoveAllAbilities();
                foreach (DisplayValue<float> value2 in this._displayValueMap.Values)
                {
                    value2.Dispose();
                }
            }
        }

        public override bool OnResolvedEvent(BaseEvent evt)
        {
            if (this.muteEvents)
            {
                return false;
            }
            bool flag = false;
            if (!(evt is EvtBeingHit))
            {
                return flag;
            }
            EvtBeingHit hit = (EvtBeingHit) evt;
            if (<>f__am$cache24 == null)
            {
                <>f__am$cache24 = config => config.OnBeingHitResolved;
            }
            return (flag | this.HandleModifierActions(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(hit.sourceID), evt, <>f__am$cache24));
        }

        protected virtual bool PostEventInstancedMixin(BaseAbilityMixin mixin, BaseEvent evt)
        {
            return mixin.OnPostEvent(evt);
        }

        public static void PostInitAbilityActorPlugin(BaseAbilityActor actor)
        {
            if (Singleton<LevelManager>.Instance.gameMode is NetworkedMP_Default_GameMode)
            {
                actor.mpAbilityPlugin = new MPActorAbilityPlugin(actor);
                actor.abilityPlugin = actor.mpAbilityPlugin;
                actor.AddPluginAs<ActorAbilityPlugin>(actor.abilityPlugin);
            }
            else
            {
                actor.abilityPlugin = new ActorAbilityPlugin(actor);
                actor.AddPlugin(actor.abilityPlugin);
            }
        }

        public void PredicateByActorPresenceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByActorPresence presence = (PredicateByActorPresence) actionConfig;
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            bool flag = true;
            for (int i = 0; i < presence.ActorNames.Length; i++)
            {
                bool flag2 = false;
                for (int j = 0; j < allPlayerAvatars.Count; j++)
                {
                    if (presence.ActorNames[i] == allPlayerAvatars[j].config.CommonArguments.RoleName)
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                this.HandleActionTargetDispatch(presence.Actions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void PredicateByAnimEventIDHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByAnimEventID tid = (PredicateByAnimEventID) actionConfig;
            string animEventID = null;
            if (evt is EvtBeingHit)
            {
                EvtBeingHit hit = (EvtBeingHit) evt;
                animEventID = hit.animEventID;
            }
            else if (evt is EvtHittingOther)
            {
                EvtHittingOther other = (EvtHittingOther) evt;
                animEventID = other.animEventID;
            }
            else if (evt is EvtAttackLanded)
            {
                EvtAttackLanded landed = (EvtAttackLanded) evt;
                animEventID = landed.animEventID;
            }
            if (animEventID != null)
            {
                for (int i = 0; i < tid.AnimEventIDs.Length; i++)
                {
                    if (animEventID == tid.AnimEventIDs[i])
                    {
                        this.HandleActionTargetDispatch(tid.Actions, instancedAbility, instancedModifier, target, evt);
                        return;
                    }
                }
            }
        }

        public void PredicateByHasEnemyAroundHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByHasEnemyAround around = (PredicateByHasEnemyAround) actionConfig;
            if (CollisionDetectPattern.CircleCollisionDetectBySphere(this._owner.entity.XZPosition + Vector3.up, 0f, this._owner.entity.transform.forward, instancedAbility.Evaluate(around.Range), Singleton<EventManager>.Instance.GetAbilityTargettingMask(this._owner.runtimeID, MixinTargetting.Enemy)).Count > 0)
            {
                this.HandleActionTargetDispatch(around.Actions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void PredicateByHitTypeHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByHitType type = (PredicateByHitType) actionConfig;
            EvtBeingHit hit = evt as EvtBeingHit;
            if (hit != null)
            {
                if (hit.attackData.hitType == AttackResult.ActorHitType.Melee)
                {
                    this.HandleActionTargetDispatch(type.MeleeActions, instancedAbility, instancedModifier, target, evt);
                }
                else if (hit.attackData.hitType == AttackResult.ActorHitType.Ranged)
                {
                    this.HandleActionTargetDispatch(type.RangeActions, instancedAbility, instancedModifier, target, evt);
                }
            }
        }

        public void PredicateByParamHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByParam param = (PredicateByParam) actionConfig;
            if (instancedAbility.HasParam(param.Param))
            {
                this.HandleActionTargetDispatch(param.Actions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void PredicateByParamNotZeroHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByParamNotZero zero = (PredicateByParamNotZero) actionConfig;
            if (instancedAbility.GetFloatParam(zero.Param) != 0f)
            {
                this.HandleActionTargetDispatch(zero.Actions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void PredicateBySpecialHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateBySpecial special = (PredicateBySpecial) actionConfig;
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            switch (special.Special)
            {
                case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctClass:
                {
                    bool flag = true;
                    for (int i = 0; i < allPlayerAvatars.Count; i++)
                    {
                        for (int j = 0; j < allPlayerAvatars.Count; j++)
                        {
                            if ((i != j) && (allPlayerAvatars[i].config.CommonArguments.Class == allPlayerAvatars[j].config.CommonArguments.Class))
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        this.HandleActionTargetDispatch(special.Actions, instancedAbility, instancedModifier, target, evt);
                    }
                    break;
                }
                case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctRoleName:
                {
                    bool flag2 = true;
                    for (int k = 0; k < allPlayerAvatars.Count; k++)
                    {
                        for (int m = 0; m < allPlayerAvatars.Count; m++)
                        {
                            if ((k != m) && (allPlayerAvatars[k].config.CommonArguments.RoleName == allPlayerAvatars[m].config.CommonArguments.RoleName))
                            {
                                flag2 = false;
                            }
                        }
                    }
                    if (flag2)
                    {
                        this.HandleActionTargetDispatch(special.Actions, instancedAbility, instancedModifier, target, evt);
                    }
                    break;
                }
                case PredicateBySpecial.SpecialType.IsEveryAvatarHasDifferenctNature:
                {
                    bool flag3 = true;
                    for (int n = 0; n < allPlayerAvatars.Count; n++)
                    {
                        for (int num6 = 0; num6 < allPlayerAvatars.Count; num6++)
                        {
                            if ((n != num6) && (allPlayerAvatars[n].config.CommonArguments.Nature == allPlayerAvatars[num6].config.CommonArguments.Nature))
                            {
                                flag3 = false;
                            }
                        }
                    }
                    if (flag3)
                    {
                        this.HandleActionTargetDispatch(special.Actions, instancedAbility, instancedModifier, target, evt);
                    }
                    break;
                }
                case PredicateBySpecial.SpecialType.IsEveryAvatarHasSameNature:
                {
                    bool flag4 = true;
                    for (int num7 = 0; num7 < allPlayerAvatars.Count; num7++)
                    {
                        for (int num8 = 0; num8 < allPlayerAvatars.Count; num8++)
                        {
                            if (allPlayerAvatars[num7].config.CommonArguments.Nature != special.SameNature)
                            {
                                flag4 = false;
                            }
                        }
                    }
                    if (flag4)
                    {
                        this.HandleActionTargetDispatch(special.Actions, instancedAbility, instancedModifier, target, evt);
                    }
                    break;
                }
            }
        }

        public void PredicateByTargetClassHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            <PredicateByTargetClassHandler>c__AnonStoreyAF yaf = new <PredicateByTargetClassHandler>c__AnonStoreyAF {
                config = (PredicateByTargetClass) actionConfig
            };
            for (int i = 0; i < yaf.config.Actions.Length; i++)
            {
                this.HandleActionTargetDispatch(yaf.config.Actions[i], instancedAbility, instancedModifier, target, evt, new Func<BaseAbilityActor, bool>(yaf.<>m__51));
            }
        }

        public void PredicateByTargetDistanceHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            PredicateByTargetDistance distance = (PredicateByTargetDistance) actionConfig;
            BaseActor caster = instancedAbility.caster;
            if (caster != null)
            {
                if ((target != null) && (distance.Distance >= 0f))
                {
                    Vector3 vector = target.gameObject.transform.position - caster.gameObject.transform.position;
                    if (vector.magnitude >= distance.Distance)
                    {
                        this.HandleActionTargetDispatch(distance.OutActions, instancedAbility, instancedModifier, target, evt);
                        return;
                    }
                }
                this.HandleActionTargetDispatch(distance.InActions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void PredicateByTargetNatureHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            <PredicateByTargetNatureHandler>c__AnonStoreyB0 yb = new <PredicateByTargetNatureHandler>c__AnonStoreyB0 {
                config = (PredicateByTargetNature) actionConfig
            };
            for (int i = 0; i < yb.config.Actions.Length; i++)
            {
                this.HandleActionTargetDispatch(yb.config.Actions[i], instancedAbility, instancedModifier, target, evt, new Func<BaseAbilityActor, bool>(yb.<>m__52));
            }
        }

        public void PredicatedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            Predicated predicated = (Predicated) actionConfig;
            if (this.EvaluateAbilityPredicate(predicated.TargetPredicates, instancedAbility, instancedModifier, target, evt))
            {
                this.HandleActionTargetDispatch(predicated.SuccessActions, instancedAbility, instancedModifier, target, evt);
            }
            else
            {
                this.HandleActionTargetDispatch(predicated.FailActions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void RandomedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            Randomed randomed = (Randomed) actionConfig;
            float num = UnityEngine.Random.value;
            float num2 = instancedAbility.Evaluate(randomed.Chance);
            bool flag = num < num2;
            if (num < num2)
            {
                this.HandleActionTargetDispatch(randomed.SuccessActions, instancedAbility, instancedModifier, target, evt);
            }
            else
            {
                this.HandleActionTargetDispatch(randomed.FailActions, instancedAbility, instancedModifier, target, evt);
            }
        }

        public void ReflectDamageByAttackPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ReflectDamageByAttackProperty property = (ReflectDamageByAttackProperty) actionConfig;
            BaseActor caster = instancedAbility.caster;
            EvtBeingHit hit = evt as EvtBeingHit;
            if (!hit.GetAttackResult().rejected)
            {
                property.AttackProperty.AddedDamageValue = hit.GetAttackResult().GetTotalDamage() * instancedAbility.Evaluate(property.ReflectRatio);
                AttackData attackData = DamageModelLogic.CreateAttackDataFromAttackProperty(caster, property.AttackProperty, property.AttackEffect, property.CameraShake);
                attackData.hitType = AttackResult.ActorHitType.Ailment;
                bool forceSkipAttackerResolve = !caster.IsEntityExists();
                AttackPattern.SendHitEvent(caster.runtimeID, target.runtimeID, null, null, attackData, forceSkipAttackerResolve, MPEventDispatchMode.Normal);
            }
        }

        public void RefreshTargetLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            RefreshTargetLevelBuff buff = (RefreshTargetLevelBuff) actionConfig;
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            if (this._levelActor.IsLevelBuffActive(buff.LevelBuff) && (buff.LevelBuff == LevelBuffType.WitchTime))
            {
                levelActor.witchTimeLevelBuff.ApplyWitchTimeSlowedBySideWithRuntimeID(target.runtimeID);
            }
        }

        protected virtual void RemoveAbility(ActorAbility instancedAbility)
        {
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                if (this._appliedAbilities[i] == instancedAbility)
                {
                    ActorAbility ability = this._appliedAbilities[i];
                    if (<>f__am$cache14 == null)
                    {
                        <>f__am$cache14 = config => config.OnRemoved;
                    }
                    this.HandleAbilityActions(ability, null, null, <>f__am$cache14);
                    this.RemoveInstancedMixins(ability.instancedMixins);
                    ability.Detach();
                    this._appliedAbilities[i] = null;
                }
            }
        }

        protected virtual void RemoveAllAbilities()
        {
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                if (this._appliedAbilities[i] != null)
                {
                    this.RemoveAbility(this._appliedAbilities[i]);
                }
            }
        }

        public void RemoveAllDebuffModifiers()
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && AbilityData.IsModifierDebuff(this._appliedModifiers[i].config))
                {
                    this.RemoveModifier(this._appliedModifiers[i], i);
                }
            }
        }

        private void RemoveAllDurationedOrDebuffs()
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && (this.IsModifierDurationed(this._appliedModifiers[i]) || AbilityData.IsModifierDebuff(this._appliedModifiers[i].config)))
                {
                    this.RemoveModifier(this._appliedModifiers[i], i);
                }
            }
        }

        protected virtual void RemoveAllModifies()
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if (this._appliedModifiers[i] != null)
                {
                    this.RemoveModifier(this._appliedModifiers[i], i);
                }
            }
        }

        protected virtual void RemoveAllNonOnDestroyAbilities()
        {
            for (int i = 0; i < this._appliedAbilities.Count; i++)
            {
                if ((this._appliedAbilities[i] != null) && (this._appliedAbilities[i].config.OnDestroy.Length <= 0))
                {
                    this.RemoveAbility(this._appliedAbilities[i]);
                }
            }
        }

        protected virtual void RemoveInstancedMixins(BaseAbilityMixin[] mixins)
        {
            for (int i = 0; i < mixins.Length; i++)
            {
                mixins[i].OnRemoved();
            }
        }

        protected virtual void RemoveModifier(ActorModifier modifier, int index)
        {
            if (<>f__am$cache16 == null)
            {
                <>f__am$cache16 = config => config.OnRemoved;
            }
            this.HandleModifierActions(modifier, null, null, <>f__am$cache16);
            this.RemoveModifierOnIndex(modifier, index);
            for (int i = 0; i < this._modifierThinkTimers.Count; i++)
            {
                if ((this._modifierThinkTimers[i] != null) && (this._modifierThinkTimers[i].Item1 == modifier))
                {
                    this._modifierThinkTimers[i] = null;
                }
            }
            for (int j = 0; j < this._modifierDurationTimers.Count; j++)
            {
                if ((this._modifierDurationTimers[j] != null) && (this._modifierDurationTimers[j].Item1 == modifier))
                {
                    this._modifierDurationTimers[j] = null;
                }
            }
            if (modifier.config.OnMonsterCreated.Length > 0)
            {
                Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(this._owner.runtimeID);
            }
            if (modifier.config.OnAvatarCreated.Length > 0)
            {
                Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(this._owner.runtimeID);
            }
        }

        public void RemoveModifierByState(AbilityState state)
        {
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && (this._appliedModifiers[i].config.State == state))
                {
                    this.RemoveModifier(this._appliedModifiers[i], i);
                }
            }
        }

        public void RemoveModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            MoleMole.Config.RemoveModifier modifier = (MoleMole.Config.RemoveModifier) actionConfig;
            target.abilityPlugin.TryRemoveModifier(instancedAbility, modifier.ModifierName);
        }

        protected virtual void RemoveModifierOnIndex(ActorModifier modifier, int index)
        {
            this.RemoveInstancedMixins(modifier.instancedMixins);
            modifier.Detach();
            this._appliedModifiers[index] = null;
            int num = this._deadModifiers.SeekAddPosition<ActorModifier>();
            this._deadModifiers[num] = modifier;
            modifier.instancedModifierID = 0;
        }

        public void RemoveUniqueModifierHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            RemoveUniqueModifier modifier = (RemoveUniqueModifier) actionConfig;
            ActorModifier firstUniqueModifier = target.abilityPlugin.GetFirstUniqueModifier(modifier.ModifierName);
            if (firstUniqueModifier != null)
            {
                target.abilityPlugin.TryRemoveModifier(firstUniqueModifier);
            }
        }

        public void ReplaceAttackDataHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ReplaceAttackData data = (ReplaceAttackData) actionConfig;
            EvtBeingHit hit = evt as EvtBeingHit;
            if (data.ReplaceFrameHalt)
            {
                hit.attackData.frameHalt = data.FrameHalt;
            }
            if (data.ReplaceAttackerAniDamageRatio)
            {
                hit.attackData.attackerAniDamageRatio = data.AttackerAniDamageRatio;
            }
            if (data.AddAttackeeAniDefenceRatio != 0f)
            {
                hit.attackData.attackeeAniDefenceRatio += data.AddAttackeeAniDefenceRatio;
            }
        }

        public void ReplaceAttackEffectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ReplaceAttackEffect effect = (ReplaceAttackEffect) actionConfig;
            EvtBeingHit hit = evt as EvtBeingHit;
            if (effect.AttackEffect != null)
            {
                hit.attackData.attackEffectPattern = effect.AttackEffect;
            }
            if (effect.BeHitEffect != null)
            {
                hit.attackData.beHitEffectPattern = effect.BeHitEffect;
            }
        }

        public void ResetAnimatorTriggerHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ResetAnimatorTrigger trigger = (ResetAnimatorTrigger) actionConfig;
            target.entity.ResetTrigger(trigger.TriggerID);
        }

        public void ResetKilled()
        {
            this._isKilled = false;
        }

        public void ResetPropertyHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ResetProperty property = (ResetProperty) actionConfig;
            int index = instancedModifier.config.Properties.IndexOfKey(property.Property);
            int stackIx = instancedModifier.stackIndices[index];
            target.SetPropertyByStackIndex(property.Property, stackIx, instancedAbility.Evaluate(instancedModifier.config.Properties[property.Property]));
        }

        protected void ResolveTarget(AbilityTargetting targetting, TargettingOption option, ActorAbility instancedAbility, BaseAbilityActor other, out BaseAbilityActor outTarget, out BaseAbilityActor[] outTargetLs, out bool needHandleTargetOnNull)
        {
            BaseAbilityActor actor3;
            BaseAbilityActor caster = instancedAbility.caster;
            BaseAbilityActor actor2 = this._owner;
            if (caster != actor2)
            {
                actor3 = actor2;
            }
            else
            {
                actor3 = other;
            }
            switch (targetting)
            {
                case AbilityTargetting.Self:
                    outTarget = actor2;
                    outTargetLs = null;
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.Caster:
                    outTarget = caster;
                    outTargetLs = null;
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.Target:
                    outTarget = this.CheckTargetAvailable(actor3, true);
                    outTargetLs = null;
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.Creator:
                    outTarget = this.CheckTargetAvailable(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(actor2.ownerID), true);
                    outTargetLs = null;
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.CasterAllied:
                    outTarget = null;
                    outTargetLs = this.FilterTargetArray(Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster), true);
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.TargetAllied:
                    outTarget = null;
                    outTargetLs = this.FilterTargetArray(Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(actor3), false);
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.EnemyAllied:
                    outTarget = null;
                    outTargetLs = this.FilterTargetArray(Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster), false);
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.CasterCenteredAllied:
                    outTarget = null;
                    if (caster.IsActive())
                    {
                        outTargetLs = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster);
                        for (int i = 0; i < outTargetLs.Length; i++)
                        {
                            if ((!outTargetLs[i].IsActive() || (this.CheckTargetAvailable(outTargetLs[i], true) == null)) || (Miscs.DistancForVec3IgnoreY(outTargetLs[i].entity.XZPosition, caster.entity.XZPosition) > instancedAbility.Evaluate(option.Range)))
                            {
                                outTargetLs[i] = null;
                            }
                        }
                        break;
                    }
                    outTargetLs = null;
                    break;

                case AbilityTargetting.CasterCenteredEnemies:
                    outTarget = null;
                    if (caster.IsActive())
                    {
                        outTargetLs = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster);
                        for (int j = 0; j < outTargetLs.Length; j++)
                        {
                            if ((!outTargetLs[j].IsActive() || (this.CheckTargetAvailable(outTargetLs[j], false) == null)) || (Miscs.DistancForVec3IgnoreY(outTargetLs[j].entity.XZPosition, caster.entity.XZPosition) > instancedAbility.Evaluate(option.Range)))
                            {
                                outTargetLs[j] = null;
                            }
                        }
                    }
                    else
                    {
                        outTargetLs = null;
                    }
                    needHandleTargetOnNull = false;
                    return;

                case AbilityTargetting.TargetCenteredAllied:
                    needHandleTargetOnNull = false;
                    outTarget = null;
                    if ((actor3 != null) && actor3.IsActive())
                    {
                        outTargetLs = Singleton<EventManager>.Instance.GetAlliedActorsOf<BaseAbilityActor>(caster);
                        for (int k = 0; k < outTargetLs.Length; k++)
                        {
                            if (!outTargetLs[k].IsActive() || ((this.CheckTargetAvailable(outTargetLs[k], true) == null) && (Miscs.DistancForVec3IgnoreY(outTargetLs[k].entity.XZPosition, actor3.entity.XZPosition) > instancedAbility.Evaluate(option.Range))))
                            {
                                outTargetLs[k] = null;
                            }
                        }
                        return;
                    }
                    outTargetLs = null;
                    return;

                case AbilityTargetting.TargetCenteredEnemies:
                    needHandleTargetOnNull = false;
                    outTarget = null;
                    if ((actor3 != null) && actor3.IsActive())
                    {
                        outTargetLs = Singleton<EventManager>.Instance.GetEnemyActorsOf<BaseAbilityActor>(caster);
                        for (int m = 0; m < outTargetLs.Length; m++)
                        {
                            if ((!outTargetLs[m].IsActive() || (this.CheckTargetAvailable(outTargetLs[m], false) == null)) || (Miscs.DistancForVec3IgnoreY(outTargetLs[m].entity.XZPosition, actor3.entity.XZPosition) > instancedAbility.Evaluate(option.Range)))
                            {
                                outTargetLs[m] = null;
                            }
                        }
                        return;
                    }
                    outTargetLs = null;
                    return;

                case AbilityTargetting.Other:
                    outTarget = other;
                    outTargetLs = null;
                    needHandleTargetOnNull = true;
                    return;

                default:
                    outTarget = null;
                    outTargetLs = null;
                    needHandleTargetOnNull = false;
                    return;
            }
            needHandleTargetOnNull = false;
        }

        public void RestartMainAIHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if ((target.isAlive != 0) && target.HasPlugin<MonsterAIPlugin>())
            {
                target.GetPlugin<MonsterAIPlugin>().RestartMainAI();
            }
        }

        public void RetargetedHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            BaseAbilityActor actor;
            BaseAbilityActor[] actorArray;
            bool flag;
            Retargeted retargeted = (Retargeted) actionConfig;
            this.ResolveTarget(retargeted.Retarget, retargeted.RetargetOption, instancedAbility, target, out actor, out actorArray, out flag);
            if ((actor != null) || flag)
            {
                this.HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, actor, evt);
            }
            else if ((actorArray != null) && (actorArray.Length > 0))
            {
                if (retargeted.RandomedTarget)
                {
                    int index = UnityEngine.Random.Range(0, actorArray.Length);
                    if (actorArray[index] != null)
                    {
                        this.HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, actorArray[index], evt);
                    }
                }
                else
                {
                    for (int i = 0; i < actorArray.Length; i++)
                    {
                        if ((actorArray[i] != null) && (!retargeted.IgnoreSelf || (actorArray[i] != instancedAbility.caster)))
                        {
                            this.HandleActionTargetDispatch(retargeted.RetargetedActions, instancedAbility, instancedModifier, actorArray[i], evt);
                        }
                    }
                }
            }
        }

        public void SetAIParamBoolHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAIParamBool @bool = (SetAIParamBool) actionConfig;
            if (target.entity is BaseMonoMonster)
            {
                BaseMonoMonster entity = target.entity as BaseMonoMonster;
                if (entity.IsAIControllerActive())
                {
                    ((BTreeMonsterAIController) (target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable<bool>(instancedAbility.Evaluate(@bool.Param), @bool.Value);
                }
            }
        }

        public void SetAIParamHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAIParam param = (SetAIParam) actionConfig;
            switch (param.LogicType)
            {
                case ParamLogicType.Replace:
                    if (target.entity is BaseMonoMonster)
                    {
                        BaseMonoMonster entity = target.entity as BaseMonoMonster;
                        if (entity.IsAIControllerActive())
                        {
                            ((BTreeMonsterAIController) (target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable<float>(instancedAbility.Evaluate(param.Param), param.Value);
                        }
                    }
                    break;

                case ParamLogicType.Add:
                    if (target.entity is BaseMonoMonster)
                    {
                        BaseMonoMonster monster2 = target.entity as BaseMonoMonster;
                        if (monster2.IsAIControllerActive())
                        {
                            ((BTreeMonsterAIController) (target.entity as BaseMonoMonster).GetActiveAIController()).AddBehaviorVariableFloat(instancedAbility.Evaluate(param.Param), param.Value);
                        }
                    }
                    break;
            }
        }

        public void SetAIParamStringHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAIParamString str = (SetAIParamString) actionConfig;
            if (target.entity is BaseMonoMonster)
            {
                BaseMonoMonster entity = target.entity as BaseMonoMonster;
                if (entity.IsAIControllerActive())
                {
                    ((BTreeMonsterAIController) (target.entity as BaseMonoMonster).GetActiveAIController()).SetBehaviorVariable<string>(instancedAbility.Evaluate(str.Param), instancedAbility.Evaluate(str.Value));
                }
            }
        }

        public void SetAllowSwitchOther(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachAllowSwitchOther other = (AttachAllowSwitchOther) actionConfig;
                context.AttachAllowSwitchOther(target.entity, other.AllowSwitchOther);
            }
        }

        public void SetAnimatorBoolHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAnimatorBool @bool = (SetAnimatorBool) actionConfig;
            target.entity.SetPersistentAnimatorBool(@bool.BoolID, @bool.Value);
        }

        public void SetAnimatorIntHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAnimatorInt num = (SetAnimatorInt) actionConfig;
            target.entity.SetPersistentAnimatoInt(num.IntID, num.Value);
        }

        public void SetAnimatorTriggerHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetAnimatorTrigger trigger = (SetAnimatorTrigger) actionConfig;
            target.entity.SetTrigger(trigger.TriggerID);
        }

        public void SetLocomotionFloatHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetLocomotionFloat num = (SetLocomotionFloat) actionConfig;
            (target.entity as BaseMonoAnimatorEntity).SetLocomotionFloat(num.Param, num.Value, false);
        }

        public void SetLocomotionRandomHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SetLocomotionRandom random = (SetLocomotionRandom) actionConfig;
            (target.entity as BaseMonoAnimatorEntity).SetLocomotionRandom(instancedAbility.Evaluate(random.Range));
        }

        public void SetMuteOtherQTE(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists())
            {
                BaseActorActionContext context = (instancedModifier == null) ? ((BaseActorActionContext) instancedAbility) : ((BaseActorActionContext) instancedModifier);
                AttachMuteOtherQTE rqte = (AttachMuteOtherQTE) actionConfig;
                context.AttachMuteOtherQTE(target.entity, rqte.MuteOtherQTE);
            }
        }

        public void SetSelfAttackTargetHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (target.IsEntityExists() && this._owner.IsEntityExists())
            {
                SetSelfAttackTarget target2 = (SetSelfAttackTarget) actionConfig;
                this._owner.entity.SetAttackTarget(target.entity);
                if (target2.SteerToTargetImmediately)
                {
                    Vector3 forward = target.entity.XZPosition - this._owner.entity.XZPosition;
                    this._owner.entity.SteerFaceDirectionTo(forward);
                }
            }
        }

        public void ShootPalsyBombHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ShootPalsyBomb bomb = (ShootPalsyBomb) actionConfig;
            DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(Singleton<EventManager>.Instance.GetActor(instancedAbility.caster.runtimeID), bomb.BombAttackID);
            uint runtimeID = Singleton<LevelDesignManager>.Instance.CreatePropObject("Spawn01", instancedAbility.Evaluate(bomb.PropName));
            MonoPalsyBombProp propObjectByRuntimeID = (MonoPalsyBombProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
            Transform attachPoint = instancedAbility.caster.entity.GetAttachPoint(bomb.AttachPoint);
            Vector3 vector = Vector3.RotateTowards(instancedAbility.caster.entity.transform.forward, Vector3.up, 1.047198f, 0f);
            Vector3 bornVelocity = new Vector3(vector.x * bomb.BombSpeed, vector.y * 10f, vector.z * bomb.BombSpeed);
            propObjectByRuntimeID.StartParabolaBorn(attachPoint.position, bornVelocity, new Vector3(0f, -20f, 0f));
        }

        public void ShowLevelDisplayTextHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ShowLevelDisplayText text = (ShowLevelDisplayText) actionConfig;
            string body = LocalizationGeneralLogic.GetText(text.Text, new object[0]);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, body));
        }

        public void SpawnPropObjectHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            SpawnPropObject obj2 = (SpawnPropObject) actionConfig;
            if (target.entity != null)
            {
                uint runtimeID = Singleton<PropObjectManager>.Instance.CreatePropObject(this._owner.runtimeID, obj2.PropObjectName, 0f, (float) target.attack, !obj2.FollowRootNode ? target.entity.XZPosition : target.entity.GetAttachPoint("RootNode").position, target.entity.transform.forward, false);
                PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(runtimeID);
                if (obj2.ModifierName != null)
                {
                    actor.abilityPlugin.ApplyModifier(instancedAbility, obj2.ModifierName);
                }
            }
        }

        public void StopAndDropAll()
        {
            this.RemoveAllModifies();
            this.RemoveAllAbilities();
            this._isMuted = true;
        }

        public void StopTargetLevelBuffHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            StopTargetLevelBuff buff = (StopTargetLevelBuff) actionConfig;
            if (this._levelActor.IsLevelBuffActive(buff.LevelBuff))
            {
                LevelBuffSide side = this.CalculateLevelBuffSide(instancedAbility.caster.runtimeID);
                if (buff.LevelBuff == LevelBuffType.WitchTime)
                {
                    LevelBuffWitchTime witchTimeLevelBuff = this._levelActor.witchTimeLevelBuff;
                    if (!buff.stopOtherSide ? (side == witchTimeLevelBuff.levelBuffSide) : (side != witchTimeLevelBuff.levelBuffSide))
                    {
                        this._levelActor.StopLevelBuff(witchTimeLevelBuff);
                    }
                }
                else if (buff.LevelBuff == LevelBuffType.StopWorld)
                {
                    LevelBuffStopWorld stopWorldLevelBuff = this._levelActor.stopWorldLevelBuff;
                    if (!buff.stopOtherSide ? (side == stopWorldLevelBuff.levelBuffSide) : (side != stopWorldLevelBuff.levelBuffSide))
                    {
                        this._levelActor.StopLevelBuff(stopWorldLevelBuff);
                    }
                }
            }
        }

        public void SubAttachDisplayFloat(string key, Action<float, float> cb, ref float curValue, ref float floor, ref float ceiling)
        {
            this._displayValueMap[key].SubAttach(cb, ref curValue, ref floor, ref ceiling);
        }

        public void SubAttachDynamicFloat(string key, Action<float, float> cb, ref float curValue)
        {
            this._dynamicValueMap[key].SubAttach(cb, ref curValue);
        }

        public void SubDetachDisplayFloat(string key, Action<float, float> cb)
        {
            this._displayValueMap[key].SubDetach(cb);
        }

        public void SubDetachDynamicFloat(string key, Action<float, float> cb)
        {
            this._dynamicValueMap[key].SubDetach(cb);
        }

        public void TimeSlowHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            ActTimeSlow slow = (ActTimeSlow) actionConfig;
            ConfigTimeSlow timeSlow = slow.TimeSlow;
            Singleton<LevelManager>.Instance.levelActor.TimeSlow(timeSlow.Duration, timeSlow.SlowRatio, null);
        }

        public void TriggerAbilityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            MoleMole.Config.TriggerAbility ability = (MoleMole.Config.TriggerAbility) actionConfig;
            EvtAbilityStart start = new EvtAbilityStart(target.runtimeID, instancedAbility.CurrentTriggerEvent) {
                abilityID = ability.AbilityID,
                abilityName = ability.AbilityName,
                otherID = this.GetOtherID(evt),
                hitCollision = this.GetHitCollision(evt),
                abilityArgument = ability.Argument
            };
            Singleton<EventManager>.Instance.FireEvent(start, MPEventDispatchMode.Normal);
        }

        public void TriggerAnimEventHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            TriggerAnimEvent event2 = (TriggerAnimEvent) actionConfig;
            if (target.IsActive())
            {
                (target.entity as BaseMonoAnimatorEntity).AnimEventHandler(event2.AnimEventID);
            }
        }

        public void TriggerAttackPatternHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            TriggerAttackPattern pattern = (TriggerAttackPattern) actionConfig;
            if (target.IsActive())
            {
                if (pattern.IgnoreEvade)
                {
                    target.entity.TriggerAttackPattern(pattern.AnimEventID, Singleton<EventManager>.Instance.GetAbilityTargettingMask(target.runtimeID, pattern.Targetting));
                }
                else
                {
                    target.entity.TriggerAttackPattern(pattern.AnimEventID, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(target.runtimeID, pattern.Targetting));
                }
            }
        }

        private ActorModifier TryRecycleDeadModifier(ActorAbility ownerAbility, ConfigAbilityModifier config)
        {
            for (int i = 0; i < this._deadModifiers.Count; i++)
            {
                if ((this._deadModifiers[i] != null) && ((this._deadModifiers[i].config == config) && (this._deadModifiers[i].parentAbility == ownerAbility)))
                {
                    ActorModifier modifier = this._deadModifiers[i];
                    this._deadModifiers[i] = null;
                    return modifier;
                }
            }
            return null;
        }

        public bool TryRemoveModifier(ActorModifier modifier)
        {
            if (modifier != null)
            {
                for (int i = 0; i < this._appliedModifiers.Count; i++)
                {
                    if (this._appliedModifiers[i] == modifier)
                    {
                        this.RemoveModifier(modifier, i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryRemoveModifier(ActorAbility instancedAbility, string modifierName)
        {
            ConfigAbilityModifier modifier = instancedAbility.config.Modifiers[modifierName];
            bool flag = false;
            for (int i = 0; i < this._appliedModifiers.Count; i++)
            {
                if ((this._appliedModifiers[i] != null) && (this._appliedModifiers[i].config == modifier))
                {
                    this.RemoveModifier(this._appliedModifiers[i], i);
                    if (modifier.Stacking != ConfigAbilityModifier.ModifierStacking.Multiple)
                    {
                        return true;
                    }
                    flag = true;
                }
            }
            return flag;
        }

        private static bool YesTargetPredicate(BaseAbilityActor target)
        {
            return true;
        }

        [CompilerGenerated]
        private sealed class <PredicateByTargetClassHandler>c__AnonStoreyAF
        {
            internal PredicateByTargetClass config;

            internal bool <>m__51(BaseAbilityActor actionTarget)
            {
                return ((actionTarget != null) && (actionTarget.commonConfig.CommonArguments.Class == this.config.EntityClass));
            }
        }

        [CompilerGenerated]
        private sealed class <PredicateByTargetNatureHandler>c__AnonStoreyB0
        {
            internal PredicateByTargetNature config;

            internal bool <>m__52(BaseAbilityActor actionTarget)
            {
                return ((actionTarget != null) && (actionTarget.commonConfig.CommonArguments.Nature == this.config.EntityNature));
            }
        }

        public delegate bool AbilityPredicateHandler(ConfigAbilityPredicate predConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt);

        public enum OnKillBehavior
        {
            RemoveAll,
            DoNotRemoveUntilDestroyed,
            RemoveAllDebuffsAndDurationed
        }
    }
}

