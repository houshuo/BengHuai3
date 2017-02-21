namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public abstract class BaseAbilityActor : BasePluggedActor
    {
        private List<Tuple<AbilityState, int>> _abilityStateEffectIxLs = new List<Tuple<AbilityState, int>>();
        private List<AbilityStateEntry> _abilityStateImmuneCount = new List<AbilityStateEntry>();
        private List<AbilityStateEntry> _abilityStatePushCount = new List<AbilityStateEntry>();
        [ShowInInspector]
        protected Dictionary<string, FixedSafeFloatStack> _actorProperties;
        private int _immuneCount;
        private int _maxSpeedPropertyIx;
        private int _paralyzeAniDefenceStackIx;
        private Dictionary<string, Action> _propertyChangedCallbacks;
        [ShowInInspector]
        private List<ConfigBuffDebuffResistance> _resistanceBuffDebuffs;
        private EntityTimer _witchTimeResumeTimer;
        private static Dictionary<AbilityState, string> ABILITY_EFFECT_MAP;
        public Dictionary<string, string> abilityIDMap;
        [HideInInspector]
        public ActorAbilityPlugin abilityPlugin;
        public AbilityState abilityState;
        public List<Tuple<ConfigAbility, Dictionary<string, object>>> appliedAbilities;
        public SafeFloat attack = 0f;
        public SafeFloat baseMaxHP = 0f;
        public SafeFloat baseMaxSP = 0f;
        [HideInInspector]
        public ConfigCommonEntity commonConfig;
        public SafeFloat defense = 0f;
        public BaseMonoAbilityEntity entity;
        public SafeFloat HP = 0f;
        public SafeBool isAlive = 0;
        public bool isInLevelAnim;
        public SafeInt32 level = 0;
        public SafeFloat maxHP = 0f;
        public SafeFloat maxSP = 0f;
        [HideInInspector]
        public MPActorAbilityPlugin mpAbilityPlugin;
        public Action<AbilityState, bool> onAbilityStateAdd;
        public Action<AbilityState> onAbilityStateRemove;
        public Action<float, float, float> onHPChanged;
        public Action<uint, string, KillEffect> onJustKilled;
        public Action<float, float> onMaxHPChanged;
        public Action<float, float> onMaxSPChanged;
        public Action onPostInitialized;
        public Action<float, float, float> onSPChanged;
        private const float PARALYZE_ANI_DEFENCE_DEFENCE_DOWN = -0.2f;
        public SafeFloat SP = 0f;

        static BaseAbilityActor()
        {
            Dictionary<AbilityState, string> dictionary = new Dictionary<AbilityState, string>();
            dictionary.Add(AbilityState.MoveSpeedUp, "Ability_MoveSpeedUp");
            dictionary.Add(AbilityState.AttackSpeedUp, "Ability_AttackSpeedUp");
            dictionary.Add(AbilityState.PowerUp, "Ability_PowerUp");
            dictionary.Add(AbilityState.Shielded, "Ability_Shielded");
            dictionary.Add(AbilityState.CritUp, "Ability_CritUp");
            dictionary.Add(AbilityState.Bleed, "Ability_Bleed");
            dictionary.Add(AbilityState.Burn, "Ability_Burn");
            dictionary.Add(AbilityState.Poisoned, "Ability_Poisoned");
            dictionary.Add(AbilityState.Stun, "Ability_Stun");
            dictionary.Add(AbilityState.Paralyze, "Ability_Paralyze");
            dictionary.Add(AbilityState.MoveSpeedDown, "Ability_MoveSpeedDown");
            dictionary.Add(AbilityState.AttackSpeedDown, "Ability_AttackSpeedDown");
            dictionary.Add(AbilityState.Fragile, "Ability_Fragile");
            dictionary.Add(AbilityState.Weak, "Ability_Weak");
            dictionary.Add(AbilityState.TargetLocked, "Ability_TargetLocked");
            ABILITY_EFFECT_MAP = dictionary;
        }

        protected BaseAbilityActor()
        {
        }

        public void AbilityBeingHit(EvtBeingHit evt)
        {
            if ((this.abilityState & AbilityState.WitchTimeSlowed) != AbilityState.None)
            {
                float resumeTime = 0.5f;
                if ((evt.animEventID != null) && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) == 3))
                {
                    AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.sourceID);
                    if ((actor != null) && (actor.config != null))
                    {
                        ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor.config, evt.animEventID);
                        if ((event2 != null) && (event2.WitchTimeResume != null))
                        {
                            resumeTime = event2.WitchTimeResume.ResumeTime;
                        }
                    }
                }
                if (evt.attackData.isAnimEventAttack && (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light))
                {
                    this.entity.SetTimeScale(1f, 1);
                    this._witchTimeResumeTimer.timespan = resumeTime;
                    this._witchTimeResumeTimer.Reset(true);
                }
            }
        }

        public void AddAbilityState(AbilityState state, bool muteDisplayEffect)
        {
            AbilityStateEntry entry = this.GetStateEntry(this._abilityStatePushCount, state, true);
            if (entry.count == 0)
            {
                this.abilityState |= state;
                if (this.onAbilityStateAdd != null)
                {
                    this.onAbilityStateAdd(state, muteDisplayEffect);
                }
            }
            entry.count++;
        }

        public void AddBuffDebuffResistance(ConfigBuffDebuffResistance resistance)
        {
            if ((resistance != null) && (this._resistanceBuffDebuffs != null))
            {
                this._resistanceBuffDebuffs.Add(resistance);
            }
        }

        public override void Core()
        {
            base.Core();
            this._witchTimeResumeTimer.Core(1f);
            if (this._witchTimeResumeTimer.isTimeUp)
            {
                this.entity.SetTimeScale(0.1f, 1);
                this._witchTimeResumeTimer.Reset(false);
                this._witchTimeResumeTimer.timespan = 0.5f;
            }
        }

        public float Evaluate(DynamicFloat target)
        {
            if (target.isDynamic)
            {
                return this.GetProperty(target.dynamicKey);
            }
            return target.fixedValue;
        }

        public int Evaluate(DynamicInt target)
        {
            if (target.isDynamic)
            {
                return (int) this.GetProperty(target.dynamicKey);
            }
            return target.fixedValue;
        }

        public abstract void ForceKill(uint killerID, KillEffect killEffect);
        public string GetAbilityNameByID(string abilityID)
        {
            return this.abilityIDMap[abilityID];
        }

        public float GetAbilityStateDurationRatio(AbilityState abilityState)
        {
            float num = 1f;
            if (this._resistanceBuffDebuffs != null)
            {
                for (int i = 0; i < this._resistanceBuffDebuffs.Count; i++)
                {
                    if (this._resistanceBuffDebuffs[i].ResistanceBuffDebuffs.Contains(abilityState))
                    {
                        num *= 1f - this._resistanceBuffDebuffs[i].DurationRatio;
                    }
                }
            }
            return num;
        }

        public float GetProperty(string propertyKey)
        {
            if (this._actorProperties.ContainsKey(propertyKey))
            {
                return (float) this._actorProperties[propertyKey].value;
            }
            if (this.commonConfig.EntityProperties.ContainsKey(propertyKey) && (this.commonConfig.EntityProperties[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Actor))
            {
                return this.commonConfig.EntityProperties[propertyKey].Default;
            }
            return AbilityData.PROPERTIES[propertyKey].Default;
        }

        public float GetPropertyByStackIndex(string propertyKey, int stackIx)
        {
            if (this._actorProperties.ContainsKey(propertyKey))
            {
                return this._actorProperties[propertyKey].Get(stackIx);
            }
            return this.entity.GetPropertyByStackIndex(propertyKey, stackIx);
        }

        public float GetResistanceRatio(AbilityState abilityState)
        {
            float num = 1f;
            if (this._resistanceBuffDebuffs != null)
            {
                for (int i = 0; i < this._resistanceBuffDebuffs.Count; i++)
                {
                    if (this._resistanceBuffDebuffs[i].ResistanceBuffDebuffs.Contains(abilityState))
                    {
                        num *= 1f - this._resistanceBuffDebuffs[i].ResistanceRatio;
                    }
                }
            }
            return num;
        }

        private AbilityStateEntry GetStateEntry(List<AbilityStateEntry> ls, AbilityState state, bool createIfNotFound = false)
        {
            for (int i = 0; i < ls.Count; i++)
            {
                if (ls[i].state == state)
                {
                    return ls[i];
                }
            }
            if (createIfNotFound)
            {
                AbilityStateEntry item = new AbilityStateEntry {
                    state = state,
                    count = 0
                };
                ls.Add(item);
                return item;
            }
            return null;
        }

        public bool HasAppliedAbilityName(string abilityName)
        {
            foreach (Tuple<ConfigAbility, Dictionary<string, object>> tuple in this.appliedAbilities)
            {
                if (tuple.Item1.AbilityName == abilityName)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void HealHP(float amount)
        {
            if (this.isAlive != 0)
            {
                DelegateUtils.UpdateField(ref this.HP, Mathf.Clamp(this.HP + amount, 0f, (float) this.maxHP), amount, this.onHPChanged);
            }
        }

        public virtual void HealSP(float amount)
        {
            if (this.isAlive != 0)
            {
                DelegateUtils.UpdateField(ref this.SP, Mathf.Clamp(this.SP + amount, 0f, (float) this.maxSP), amount, this.onSPChanged);
            }
        }

        protected void HPPropertyChangedCallback()
        {
            float num = (float) (this.HP / this.maxHP);
            float newValue = (this.baseMaxHP + this.GetProperty("Actor_MaxHPDelta")) * (1f + this.GetProperty("Actor_MaxHPRatio"));
            bool flag = newValue > this.maxHP;
            DelegateUtils.UpdateField(ref this.maxHP, newValue, this.onMaxHPChanged);
            if (flag)
            {
                float num3 = num * this.maxHP;
                DelegateUtils.UpdateField(ref this.HP, num3, num3 - this.HP, this.onHPChanged);
            }
            else
            {
                float num4 = Mathf.Min((float) this.maxHP, (float) this.HP);
                DelegateUtils.UpdateField(ref this.HP, num4, num4 - this.HP, this.onHPChanged);
            }
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.entity = (BaseMonoAbilityEntity) entity;
            this._actorProperties = new Dictionary<string, FixedSafeFloatStack>();
            this._propertyChangedCallbacks = new Dictionary<string, Action>();
            this.abilityIDMap = new Dictionary<string, string>();
            this.appliedAbilities = new List<Tuple<ConfigAbility, Dictionary<string, object>>>();
            this._resistanceBuffDebuffs = new List<ConfigBuffDebuffResistance>();
            this.isAlive = 1;
            this.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Combine(this.onAbilityStateAdd, new Action<AbilityState, bool>(this.OnAbilityStateAdd));
            this.onAbilityStateRemove = (Action<AbilityState>) Delegate.Combine(this.onAbilityStateRemove, new Action<AbilityState>(this.OnAbilityStateRemove));
            this.RegisterPropertyChangedCallback("Actor_MaxHPRatio", new Action(this.HPPropertyChangedCallback));
            this.RegisterPropertyChangedCallback("Actor_MaxHPDelta", new Action(this.HPPropertyChangedCallback));
            this.RegisterPropertyChangedCallback("Actor_MaxSPRatio", new Action(this.SPPropertyChangedCallback));
            this.RegisterPropertyChangedCallback("Actor_MaxSPDelta", new Action(this.SPPropertyChangedCallback));
            this._witchTimeResumeTimer = new EntityTimer(0.5f, Singleton<LevelManager>.Instance.levelEntity);
            this._witchTimeResumeTimer.SetActive(false);
        }

        public bool IsImmuneAbilityState(AbilityState state)
        {
            AbilityStateEntry entry = this.GetStateEntry(this._abilityStateImmuneCount, state, false);
            return ((entry != null) ? (entry.count > 0) : false);
        }

        protected virtual void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
            bool flag = false;
            if (state == AbilityState.Invincible)
            {
                this.abilityPlugin.RemoveAllDebuffModifiers();
                this.SetImmuneDebuff(true);
                if (!muteDisplayEffect)
                {
                    flag = true;
                    this.entity.SetShaderDataLerp(E_ShaderData.Invincible, true, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.Immune)
            {
                this.abilityPlugin.RemoveAllDebuffModifiers();
                this.SetImmuneDebuff(true);
            }
            else if (state == AbilityState.Endure)
            {
                for (int i = 0; i < AbilityData.ABILITY_STATE_CONTROL_DEBUFFS.Length; i++)
                {
                    AbilityState state2 = AbilityData.ABILITY_STATE_CONTROL_DEBUFFS[i];
                    this.abilityPlugin.RemoveModifierByState(state2);
                    this.SetAbilityStateImmune(state2, true);
                }
                if (!muteDisplayEffect)
                {
                    flag = true;
                    this.entity.SetShaderDataLerp(E_ShaderData.Endure, true, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.Paralyze)
            {
                this._paralyzeAniDefenceStackIx = this.PushProperty("Actor_AniDefenceDelta", -0.2f);
            }
            else if (state == AbilityState.WitchTimeSlowed)
            {
                this.entity.PushTimeScale(0.1f, 1);
                if (!muteDisplayEffect)
                {
                    flag = true;
                    this.entity.SetShaderDataLerp(E_ShaderData.ColorBias, true, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.MaxMoveSpeed)
            {
                this._maxSpeedPropertyIx = this.entity.PushProperty("Animator_MoveSpeedRatio", 999999f);
            }
            else if ((state == AbilityState.Frozen) && !muteDisplayEffect)
            {
                flag = true;
                this.entity.SetShaderData(E_ShaderData.Frozon, true);
            }
            if (!muteDisplayEffect && (flag || ABILITY_EFFECT_MAP.ContainsKey(state)))
            {
                int num2 = this._abilityStateEffectIxLs.SeekAddPosition<Tuple<AbilityState, int>>();
                this._abilityStateEffectIxLs[num2] = Tuple.Create<AbilityState, int>(state, !flag ? this.entity.AttachEffect(ABILITY_EFFECT_MAP[state]) : -1);
            }
        }

        protected virtual void OnAbilityStateRemove(AbilityState state)
        {
            bool flag = false;
            for (int i = 0; i < this._abilityStateEffectIxLs.Count; i++)
            {
                if ((this._abilityStateEffectIxLs[i] != null) && (((AbilityState) this._abilityStateEffectIxLs[i].Item1) == state))
                {
                    if (this._abilityStateEffectIxLs[i].Item2 == -1)
                    {
                        flag = true;
                    }
                    else
                    {
                        this.entity.DetachEffect(this._abilityStateEffectIxLs[i].Item2);
                    }
                    this._abilityStateEffectIxLs[i] = null;
                }
            }
            if (state == AbilityState.Invincible)
            {
                this.SetImmuneDebuff(false);
                if (flag)
                {
                    this.entity.SetShaderDataLerp(E_ShaderData.Invincible, false, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.Immune)
            {
                this.SetImmuneDebuff(false);
            }
            else if (state == AbilityState.Endure)
            {
                for (int j = 0; j < AbilityData.ABILITY_STATE_CONTROL_DEBUFFS.Length; j++)
                {
                    AbilityState state2 = AbilityData.ABILITY_STATE_CONTROL_DEBUFFS[j];
                    this.SetAbilityStateImmune(state2, false);
                }
                if (flag)
                {
                    this.entity.SetShaderDataLerp(E_ShaderData.Endure, false, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.Paralyze)
            {
                this.PopProperty("Actor_AniDefenceDelta", this._paralyzeAniDefenceStackIx);
            }
            else if (state == AbilityState.WitchTimeSlowed)
            {
                this.entity.PopTimeScale(1);
                this._witchTimeResumeTimer.Reset(false);
                if (flag)
                {
                    this.entity.SetShaderDataLerp(E_ShaderData.ColorBias, false, -1f, -1f, false);
                }
            }
            else if (state == AbilityState.MaxMoveSpeed)
            {
                this.entity.PopProperty("Animator_MoveSpeedRatio", this._maxSpeedPropertyIx);
            }
            else if ((state == AbilityState.Frozen) && flag)
            {
                this.entity.SetShaderData(E_ShaderData.Frozon, false);
            }
        }

        private bool OnBeingHitResolve(EvtBeingHit evt)
        {
            this.AbilityBeingHit(evt);
            return false;
        }

        public override bool OnEventResolves(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnBeingHitResolve((EvtBeingHit) evt));
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            this.onHPChanged = null;
            this.onSPChanged = null;
            this.onMaxHPChanged = null;
            this.onMaxSPChanged = null;
            this.onAbilityStateAdd = null;
            this.onAbilityStateRemove = null;
        }

        public void PopProperty(string propertyKey, int stackIx)
        {
            if (this.commonConfig.EntityProperties.ContainsKey(propertyKey) || AbilityData.PROPERTIES.ContainsKey(propertyKey))
            {
                if ((this.commonConfig.EntityProperties.ContainsKey(propertyKey) && (this.commonConfig.EntityProperties[propertyKey].Type == ConfigAbilityPropertyEntry.PropertyType.Entity)) || (AbilityData.PROPERTIES.ContainsKey(propertyKey) && (AbilityData.PROPERTIES[propertyKey].Type != ConfigAbilityPropertyEntry.PropertyType.Actor)))
                {
                    this.entity.PopProperty(propertyKey, stackIx);
                }
                else
                {
                    this._actorProperties[propertyKey].Pop(stackIx);
                }
            }
        }

        public virtual void PostInit()
        {
            ActorAbilityPlugin.PostInitAbilityActorPlugin(this);
            if (this.onPostInitialized != null)
            {
                this.onPostInitialized();
            }
        }

        public int PushProperty(string propertyKey, float value)
        {
            if (!this.commonConfig.EntityProperties.ContainsKey(propertyKey) && !AbilityData.PROPERTIES.ContainsKey(propertyKey))
            {
                return -1;
            }
            if ((!this.commonConfig.EntityProperties.ContainsKey(propertyKey) || (this.commonConfig.EntityProperties[propertyKey].Type != ConfigAbilityPropertyEntry.PropertyType.Actor)) && (!AbilityData.PROPERTIES.ContainsKey(propertyKey) || (AbilityData.PROPERTIES[propertyKey].Type != ConfigAbilityPropertyEntry.PropertyType.Actor)))
            {
                return this.entity.PushProperty(propertyKey, value);
            }
            bool flag = false;
            if (!this._actorProperties.ContainsKey(propertyKey))
            {
                if (AbilityData.PROPERTIES.ContainsKey(propertyKey))
                {
                    flag = true;
                    this._actorProperties.Add(propertyKey, AbilityData.PROPERTIES[propertyKey].CreatePropertySafeStack());
                }
                else
                {
                    flag = true;
                    this._actorProperties.Add(propertyKey, this.commonConfig.EntityProperties[propertyKey].CreatePropertySafeStack());
                }
            }
            if (flag && this._propertyChangedCallbacks.ContainsKey(propertyKey))
            {
                <PushProperty>c__AnonStoreyAD yad = new <PushProperty>c__AnonStoreyAD {
                    callback = this._propertyChangedCallbacks[propertyKey]
                };
                FixedSafeFloatStack local1 = this._actorProperties[propertyKey];
                local1.onChanged = (Action<SafeFloat, int, SafeFloat, int>) Delegate.Combine(local1.onChanged, new Action<SafeFloat, int, SafeFloat, int>(yad.<>m__4B));
                this._propertyChangedCallbacks.Remove(propertyKey);
            }
            return this._actorProperties[propertyKey].Push(value, false);
        }

        protected void RegisterPropertyChangedCallback(string propertyKey, Action callback)
        {
            this._propertyChangedCallbacks.Add(propertyKey, callback);
        }

        public void RemoveAbilityState(AbilityState state)
        {
            AbilityStateEntry entry = this.GetStateEntry(this._abilityStatePushCount, state, false);
            if (entry != null)
            {
                entry.count--;
                if (entry.count == 0)
                {
                    this.abilityState &= ~state;
                    if (this.onAbilityStateRemove != null)
                    {
                        this.onAbilityStateRemove(state);
                    }
                }
            }
        }

        public void RemoveBuffDebuffResistance(ConfigBuffDebuffResistance resistance)
        {
            if (((resistance != null) && (this._resistanceBuffDebuffs != null)) && this._resistanceBuffDebuffs.Contains(resistance))
            {
                this._resistanceBuffDebuffs.Remove(resistance);
            }
        }

        public void SetAbilityStateImmune(AbilityState state, bool immune)
        {
            AbilityStateEntry entry = this.GetStateEntry(this._abilityStateImmuneCount, state, true);
            entry.count += !immune ? -1 : 1;
            if ((entry.count > 0) && ((this.abilityState & state) != AbilityState.None))
            {
                this.abilityPlugin.RemoveModifierByState(state);
            }
        }

        public void SetImmuneDebuff(bool immune)
        {
            this._immuneCount += !immune ? -1 : 1;
            this.abilityPlugin.IsImmuneDebuff = this._immuneCount > 0;
        }

        public void SetPropertyByStackIndex(string propertyKey, int stackIx, float value)
        {
            if (this._actorProperties.ContainsKey(propertyKey))
            {
                this._actorProperties[propertyKey].Set(stackIx, value, false);
            }
            else
            {
                this.entity.SetPropertyByStackIndex(propertyKey, stackIx, value);
            }
        }

        protected void SPPropertyChangedCallback()
        {
            float num = (float) (this.SP / this.maxSP);
            float newValue = (this.baseMaxSP + this.GetProperty("Actor_MaxSPDelta")) * (1f + this.GetProperty("Actor_MaxSPRatio"));
            bool flag = newValue > this.maxSP;
            DelegateUtils.UpdateField(ref this.maxSP, newValue, this.onMaxSPChanged);
            if (flag)
            {
                float num3 = num * this.maxSP;
                DelegateUtils.UpdateField(ref this.SP, num3, num3 - this.SP, this.onSPChanged);
            }
            else
            {
                float num4 = Mathf.Min((float) this.maxSP, (float) this.SP);
                DelegateUtils.UpdateField(ref this.SP, num4, num4 - this.SP, this.onSPChanged);
            }
        }

        [CompilerGenerated]
        private sealed class <PushProperty>c__AnonStoreyAD
        {
            internal Action callback;

            internal void <>m__4B(SafeFloat oldV, int oldIx, SafeFloat newV, int newIx)
            {
                this.callback();
            }
        }

        private class AbilityStateEntry
        {
            public int count;
            public AbilityState state;
        }
    }
}

