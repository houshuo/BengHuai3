namespace MoleMole
{
    using FullInspector;
    using LuaInterface;
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class MonsterActor : BaseAbilityActor
    {
        private int _frozenAnmatorSpeedIx;
        private FrozenState _frozenState;
        private int _paralyzeAnimatorSpeedIx;
        private ParalyzeState _paralyzeState;
        private EntityTimer _paralyzeTimer;
        public float avatarExpReward;
        [InspectorCollapsedFoldout]
        public ConfigMonster config;
        public List<LDDropDataItem> dropDataItems;
        public const float GOODS_DROP_MAX_DISTANCE = 2f;
        public bool isElite;
        public MonsterConfigMetaData metaConfig;
        public BaseMonoMonster monster;
        public bool needDropReward = true;
        public float scoinReward;
        public bool showSubHpBarWhenAttackLanded;
        public uint uniqueMonsterID;

        public void AmendHitCollision(AttackResult attackResult)
        {
            if (attackResult.hitCollision == null)
            {
                AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                    hitPoint = this.monster.RootNodePosition,
                    hitDir = -this.monster.FaceDirection
                };
                attackResult.hitCollision = collsion;
            }
            else if (attackResult.hitCollision.hitDir == Vector3.zero)
            {
                attackResult.hitCollision.hitPoint = this.monster.RootNodePosition;
                attackResult.hitCollision.hitDir = -this.monster.FaceDirection;
            }
        }

        protected virtual void AttackLanded(EvtAttackLanded evt)
        {
            if (!evt.attackResult.isFromBullet)
            {
                this.monster.FrameHalt(evt.attackResult.frameHalt);
            }
        }

        public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect)
        {
            if ((this._paralyzeState == ParalyzeState.ParalyzeFreezed) && (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light))
            {
                this.monster.PopProperty("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx);
                this._paralyzeAnimatorSpeedIx = this.monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
                this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 1f);
                if ((attackResult.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp) || (attackResult.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow))
                {
                    this._paralyzeTimer.timespan = 0.5f;
                }
                else
                {
                    this._paralyzeTimer.timespan = 0.35f;
                }
                this._paralyzeTimer.Reset(true);
                this._paralyzeState = ParalyzeState.ParalyzeHitResuming;
            }
            if (base.abilityState.ContainsState(AbilityState.Frozen) && (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light))
            {
                attackResult.hitEffect = AttackResult.AnimatorHitEffect.Light;
            }
            this.monster.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity);
        }

        public override void Core()
        {
            base.Core();
            this.UpdateAbilityState();
        }

        protected virtual void DamageLanded(EvtDamageLanded evt)
        {
            this.monster.FrameHalt(evt.attackResult.frameHalt);
        }

        public void EnableWarningFieldActor(float warningRadius, float escapeRadius)
        {
            MonsterAIPlugin plugin = base.GetPlugin<MonsterAIPlugin>();
            if (plugin != null)
            {
                plugin.InitWarningField(warningRadius, escapeRadius);
            }
        }

        public void FireAttackDataEffects(AttackResult attackResult)
        {
            if ((attackResult.attackEffectPattern != null) && ((attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal) || (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack)))
            {
                AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, this.monster, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
            }
            if ((attackResult.beHitEffectPattern != null) && ((attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal) || (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyBeHit)))
            {
                AttackPattern.ActAttackEffects(attackResult.beHitEffectPattern, this.monster, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
            }
            if (attackResult.attackCameraShake != null)
            {
                AttackPattern.ActCameraShake(attackResult.attackCameraShake);
            }
        }

        public virtual void ForceKill()
        {
            this.ForceKill(0x21800001, KillEffect.KillNow);
        }

        public override void ForceKill(uint killerID, KillEffect killEffect)
        {
            this.Kill(killerID, null, killEffect);
        }

        public void ForceRemoveImmediatelly()
        {
            base.isAlive = 0;
            this.needDropReward = false;
            this.monster.SetDied(KillEffect.KillImmediately);
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.monster = (BaseMonoMonster) entity;
            base.runtimeID = this.monster.GetRuntimeID();
            this.uniqueMonsterID = this.monster.uniqueMonsterID;
            string configType = string.Empty;
            if (this.uniqueMonsterID != 0)
            {
                configType = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID).configType;
            }
            this.config = MonsterData.GetMonsterConfig(this.monster.MonsterName, this.monster.TypeName, configType);
            base.commonConfig = this.config.CommonConfig;
            this.metaConfig = MonsterData.GetMonsterConfigMetaData(this.monster.MonsterName, this.monster.TypeName);
            base.Init(entity);
            Singleton<EventManager>.Instance.FireEvent(new EvtMonsterCreated(base.runtimeID), MPEventDispatchMode.Normal);
            this._paralyzeTimer = new EntityTimer();
            this._paralyzeTimer.SetActive(false);
            this._paralyzeState = ParalyzeState.Idle;
            base.AddPlugin(new MonsterAIPlugin(this));
            this.InitAbilityStateImmune();
            this.InitDebuffDurationRatio();
        }

        private void InitAbilityStateImmune()
        {
            foreach (AbilityState state in this.config.DebuffResistance.ImmuneStates)
            {
                base.SetAbilityStateImmune(state, true);
            }
        }

        private void InitDebuffDurationRatio()
        {
            float durationRatio = this.config.DebuffResistance.DurationRatio;
            if (durationRatio > 0f)
            {
                base.PushProperty("Actor_DebuffDurationRatioDelta", -durationRatio);
            }
        }

        public void InitLevelData(int level, bool isElite)
        {
            base.level = level;
            NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(level);
            base.baseMaxHP = base.maxHP = base.HP = this.metaConfig.HP * nPCLevelMetaDataByKey.HPRatio;
            base.defense = this.metaConfig.defense * nPCLevelMetaDataByKey.DEFRatio;
            base.attack = this.metaConfig.attack * nPCLevelMetaDataByKey.ATKRatio;
            base.PushProperty("Actor_ResistAllElementAttackRatio", nPCLevelMetaDataByKey.ElementalResistRatio);
            this.isElite = isElite;
            if (isElite)
            {
                base.baseMaxHP = base.maxHP = base.HP = base.maxHP * this.config.EliteArguments.HPRatio;
                base.defense *= this.config.EliteArguments.DefenseRatio;
                base.attack *= this.config.EliteArguments.AttackRatio;
            }
            foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair in this.config.Abilities)
            {
                base.abilityIDMap.Add(pair.Key, pair.Value.AbilityName);
                if (pair.Value.AbilityName != "Noop")
                {
                    base.appliedAbilities.Add(Tuple.Create<ConfigAbility, Dictionary<string, object>>(AbilityData.GetAbilityConfig(pair.Value.AbilityName, pair.Value.AbilityOverride), AbilityData.EMPTY_OVERRIDE_MAP));
                }
            }
            this.InitUniqueMonsterConfig(nPCLevelMetaDataByKey);
        }

        private void InitUniqueMonsterConfig(NPCLevelMetaData npcLevelMetaData)
        {
            if (this.uniqueMonsterID != 0)
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID);
                base.baseMaxHP = base.maxHP = base.HP = (this.metaConfig.HP * uniqueMonsterMetaData.HPRatio) * npcLevelMetaData.HPRatio;
                base.defense = (this.metaConfig.defense * uniqueMonsterMetaData.defenseRatio) * npcLevelMetaData.DEFRatio;
                base.attack = (this.metaConfig.attack * uniqueMonsterMetaData.attackRatio) * npcLevelMetaData.ATKRatio;
                if (uniqueMonsterMetaData.abilities.Length > 0)
                {
                    LuaState state = new LuaState();
                    IDictionaryEnumerator enumerator = ((LuaTable) state.DoString(uniqueMonsterMetaData.abilities)[0]).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            ConfigAbility abilityConfig;
                            DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                            string key = (string) current.Key;
                            LuaTable table2 = (LuaTable) current.Value;
                            string monsterName = uniqueMonsterMetaData.monsterName;
                            if (monsterName == null)
                            {
                                abilityConfig = AbilityData.GetAbilityConfig(key);
                            }
                            else
                            {
                                abilityConfig = AbilityData.GetAbilityConfig(key, monsterName);
                            }
                            Dictionary<string, object> dictionary = new Dictionary<string, object>();
                            IDictionaryEnumerator enumerator2 = table2.GetEnumerator();
                            try
                            {
                                while (enumerator2.MoveNext())
                                {
                                    DictionaryEntry entry2 = (DictionaryEntry) enumerator2.Current;
                                    string str3 = (string) entry2.Key;
                                    if (entry2.Value is double)
                                    {
                                        dictionary.Add(str3, (float) ((double) entry2.Value));
                                    }
                                    else if (entry2.Value is string)
                                    {
                                        dictionary.Add(str3, (string) entry2.Value);
                                    }
                                }
                            }
                            finally
                            {
                                IDisposable disposable = enumerator2 as IDisposable;
                                if (disposable == null)
                                {
                                }
                                disposable.Dispose();
                            }
                            base.appliedAbilities.Add(Tuple.Create<ConfigAbility, Dictionary<string, object>>(abilityConfig, dictionary));
                        }
                    }
                    finally
                    {
                        IDisposable disposable2 = enumerator as IDisposable;
                        if (disposable2 == null)
                        {
                        }
                        disposable2.Dispose();
                    }
                }
            }
        }

        public virtual void Kill(uint killerID, string animEventID, KillEffect killEffect)
        {
            if (base.onJustKilled != null)
            {
                base.onJustKilled(killerID, null, killEffect);
            }
            if (base.isAlive == 0)
            {
                if (killEffect == KillEffect.KillImmediately)
                {
                    this.monster.SetDied(KillEffect.KillImmediately);
                }
            }
            else
            {
                base.isAlive = 0;
                Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID, killerID, animEventID), MPEventDispatchMode.Normal);
                this.monster.SetDied(killEffect);
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return base.ListenEvent(evt);
        }

        protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
            base.OnAbilityStateAdd(state, muteDisplayEffect);
            if (state == AbilityState.Stun)
            {
                this.monster.SetLocomotionBool("BuffStun", true, false);
                this.monster.SetCountedMuteControl(true);
            }
            else if (state == AbilityState.Paralyze)
            {
                this.monster.SetLocomotionBool("BuffParalyze", true, false);
                this._paralyzeAnimatorSpeedIx = this.monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
                this._paralyzeTimer.Reset(true);
                this._paralyzeTimer.timespan = 0.35f;
                this._paralyzeState = ParalyzeState.ParalyzeHitResuming;
                this.monster.SetCountedMuteControl(true);
            }
            else if (state == AbilityState.Frozen)
            {
                if (this.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    this._frozenAnmatorSpeedIx = this.monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
                    this._frozenState = FrozenState.WaitingForGrounded;
                }
                else
                {
                    this._frozenAnmatorSpeedIx = this.monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 0f);
                    this._frozenState = FrozenState.FrozenFreezed;
                }
                this.monster.SetCountedMuteControl(true);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtBuffAdd(base.runtimeID, state), MPEventDispatchMode.Normal);
        }

        protected override void OnAbilityStateRemove(AbilityState state)
        {
            base.OnAbilityStateRemove(state);
            if (state == AbilityState.Stun)
            {
                this.monster.SetLocomotionBool("BuffStun", false, false);
                this.monster.SetCountedMuteControl(false);
            }
            else if (state == AbilityState.Paralyze)
            {
                this.monster.SetLocomotionBool("BuffParalyze", false, false);
                this.monster.PopProperty("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx);
                this.monster.SetCountedMuteControl(false);
                this._paralyzeState = ParalyzeState.Idle;
            }
            else if (state == AbilityState.Frozen)
            {
                this.monster.PopProperty("Animator_OverallSpeedRatioMultiplied", this._frozenAnmatorSpeedIx);
                this._frozenState = FrozenState.Idle;
                this.monster.SetCountedMuteControl(false);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtBuffRemove(base.runtimeID, state), MPEventDispatchMode.Normal);
        }

        private bool OnAttackLanded(EvtAttackLanded evt)
        {
            this.AttackLanded(evt);
            base.MarkImportantEventIsHandled(evt);
            return true;
        }

        protected virtual bool OnBeingHit(EvtBeingHit evt)
        {
            DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
            return true;
        }

        protected virtual bool OnBeingHitResolve(EvtBeingHit evt)
        {
            evt.Resolve();
            if (evt.attackData.rejected)
            {
                if (evt.attackData.rejectState == AttackResult.RejectType.RejectButShowAttackEffect)
                {
                    this.AmendHitCollision(evt.attackData);
                    this.FireAttackDataEffects(evt.attackData);
                }
                return false;
            }
            if ((base.isAlive == 0) || (evt.attackData.GetTotalDamage() > base.HP))
            {
                evt.attackData.attackeeAniDefenceRatio = 0f;
            }
            AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
            this.AmendHitCollision(attackResult);
            if (base.isAlive != 0)
            {
                if (base.abilityState.ContainsState(AbilityState.Invincible))
                {
                    attackResult.damage = 0f;
                    attackResult.plainDamage = 0f;
                    attackResult.fireDamage = 0f;
                    attackResult.thunderDamage = 0f;
                    attackResult.iceDamage = 0f;
                    attackResult.alienDamage = 0f;
                    attackResult.hitLevel = AttackResult.ActorHitLevel.Mute;
                    attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                    attackResult.frameHalt += 5;
                }
                else if (base.abilityState.ContainsState(AbilityState.Endure))
                {
                    attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                    attackResult.frameHalt += 5;
                }
                if (!attackResult.isAnimEventAttack)
                {
                    attackResult.hitEffect = AttackResult.AnimatorHitEffect.Mute;
                    attackResult.hitLevel = AttackResult.ActorHitLevel.Normal;
                    attackResult.hitEffectPattern = AttackResult.HitEffectPattern.OnlyBeHit;
                    attackResult.attackCameraShake = null;
                    attackResult.killEffect = KillEffect.KillNow;
                }
                float totalDamage = attackResult.GetTotalDamage();
                float newValue = base.HP - totalDamage;
                if (newValue <= 0f)
                {
                    newValue = 0f;
                }
                if (base.abilityState.ContainsState(AbilityState.Undamagable))
                {
                    DelegateUtils.UpdateField(ref this.HP, (float) base.HP, newValue - base.HP, base.onHPChanged);
                }
                else
                {
                    DelegateUtils.UpdateField(ref this.HP, newValue, newValue - base.HP, base.onHPChanged);
                    evt.resolvedDamage = totalDamage;
                }
                if (base.HP == 0f)
                {
                    if ((base.abilityState & AbilityState.Limbo) != AbilityState.None)
                    {
                        evt.beHitEffect = BeHitEffect.NormalBeHit;
                        this.BeingHit(attackResult, BeHitEffect.NormalBeHit);
                    }
                    else
                    {
                        if (attackResult.killEffect != KillEffect.KillTillHitAnimationEnd)
                        {
                            if ((this.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) || (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUp)) || (evt.attackData.hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow))
                            {
                                attackResult.killEffect = KillEffect.KillFastWithNormalAnim;
                            }
                            else if (((base.abilityState & AbilityState.WitchTimeSlowed) != AbilityState.None) || (attackResult.aniDamageRatio >= 0.9f))
                            {
                                attackResult.killEffect = KillEffect.KillFastWithDieAnim;
                            }
                        }
                        this.Kill(evt.sourceID, evt.animEventID, attackResult.killEffect);
                        evt.beHitEffect = BeHitEffect.KillingBeHit;
                        this.BeingHit(attackResult, BeHitEffect.KillingBeHit);
                    }
                }
                else
                {
                    evt.beHitEffect = BeHitEffect.NormalBeHit;
                    this.BeingHit(attackResult, BeHitEffect.NormalBeHit);
                }
            }
            else
            {
                evt.beHitEffect = BeHitEffect.OverkillBeHit;
                this.BeingHit(attackResult, BeHitEffect.OverkillBeHit);
            }
            this.FireAttackDataEffects(attackResult);
            if (evt.attackData.isAnimEventAttack)
            {
                EvtAttackLanded landed = new EvtAttackLanded(evt.sourceID, base.runtimeID, evt.animEventID, attackResult);
                Singleton<EventManager>.Instance.FireEvent(landed, MPEventDispatchMode.Normal);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackLanded, landed));
            }
            else
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtDamageLanded(evt.sourceID, base.runtimeID, attackResult), MPEventDispatchMode.Normal);
            }
            base.MarkImportantEventIsHandled(evt);
            return true;
        }

        private bool OnDamageLanded(EvtDamageLanded evt)
        {
            this.DamageLanded(evt);
            return true;
        }

        public override bool OnEventResolves(BaseEvent evt)
        {
            bool flag = false;
            if (evt is EvtHittingOther)
            {
                flag |= this.OnHittingOtherResolve((EvtHittingOther) evt);
            }
            else if (evt is EvtBeingHit)
            {
                flag |= this.OnBeingHitResolve((EvtBeingHit) evt);
            }
            return (flag | base.OnEventResolves(evt));
        }

        public override bool OnEventWithPlugins(BaseEvent evt)
        {
            bool flag = base.OnEventWithPlugins(evt);
            if (evt is EvtHittingOther)
            {
                return this.OnHittingOther((EvtHittingOther) evt);
            }
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHit((EvtBeingHit) evt);
            }
            if (evt is EvtAttackLanded)
            {
                return this.OnAttackLanded((EvtAttackLanded) evt);
            }
            if (evt is EvtDamageLanded)
            {
                return this.OnDamageLanded((EvtDamageLanded) evt);
            }
            if (evt is EvtKilled)
            {
                return this.OnKill((EvtKilled) evt);
            }
            return flag;
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            if (evt.attackData == null)
            {
                evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(this, evt.animEventID);
            }
            if ((evt.attackData.hitCollision == null) && (evt.hitCollision != null))
            {
                evt.attackData.hitCollision = evt.hitCollision;
            }
            else if (((evt.hitCollision == null) && (evt.attackData.hitCollision == null)) && (Singleton<EventManager>.Instance.GetActor(evt.toID) != null))
            {
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
                AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                    hitPoint = entity.GetAttachPoint("RootNode").position,
                    hitDir = entity.XZPosition - this.monster.XZPosition
                };
                evt.hitCollision = collsion;
            }
            return true;
        }

        private bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            evt.Resolve();
            Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, base.runtimeID, evt.animEventID, evt.attackData), MPEventDispatchMode.Normal);
            base.MarkImportantEventIsHandled(evt);
            return true;
        }

        private bool OnKill(EvtKilled evt)
        {
            base.RemovePlugin<MonsterAIPlugin>();
            return true;
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            if ((Singleton<LevelManager>.Instance != null) && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning))
            {
                if ((this.dropDataItems != null) && this.needDropReward)
                {
                    foreach (LDDropDataItem item in this.dropDataItems)
                    {
                        item.CreateDropGoods(this.monster.GetDropPosition(), Vector3.forward, true);
                    }
                }
                LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
                instance.avatarExpInside += this.avatarExpReward;
            }
        }

        public void RefillAttackDataDamagePercentage(string animEventID, ref AttackData attackData)
        {
            if ((this.uniqueMonsterID != 0) && !SharedAnimEventData.IsSharedAnimEventID(animEventID))
            {
                List<float> aTKRatios = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID).ATKRatios;
                List<List<string>> aTKRatioNames = this.config.ATKRatioNames;
                int num = -1;
                for (int i = 0; i < aTKRatioNames.Count; i++)
                {
                    List<string> list3 = aTKRatioNames[i];
                    foreach (string str in list3)
                    {
                        if (str == animEventID)
                        {
                            num = i;
                            break;
                        }
                    }
                    if (num != -1)
                    {
                        break;
                    }
                }
                if ((num != -1) && (num < aTKRatios.Count))
                {
                    attackData.attackerAttackPercentage *= aTKRatios[num];
                }
            }
        }

        public void SetMonsterAttackRatio(float ratio)
        {
            base.attack *= ratio;
        }

        public void SetMonsterHPRatio(float ratio)
        {
            base.baseMaxHP *= ratio;
            base.maxHP *= ratio;
            base.HP *= ratio;
        }

        private void UpdateAbilityState()
        {
            if (this._paralyzeState == ParalyzeState.WaitingForGrounded)
            {
                if (this.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
                {
                    if (base.abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
                    {
                        this.monster.PopProperty("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx);
                        this._paralyzeAnimatorSpeedIx = this.monster.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0.1f);
                    }
                    else
                    {
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0f);
                    }
                    this._paralyzeState = ParalyzeState.ParalyzeFreezed;
                }
            }
            else if ((this._paralyzeState != ParalyzeState.ParalyzeFreezed) && (this._paralyzeState == ParalyzeState.ParalyzeHitResuming))
            {
                this._paralyzeTimer.Core(1f);
                if (!this.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
                {
                    this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 1f);
                    this._paralyzeTimer.Reset(false);
                    this._paralyzeState = ParalyzeState.WaitingForGrounded;
                }
                else if (this._paralyzeTimer.isTimeUp)
                {
                    if (base.abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
                    {
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0.1f);
                    }
                    else
                    {
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0f);
                    }
                    this._paralyzeTimer.Reset(false);
                    this._paralyzeState = ParalyzeState.ParalyzeFreezed;
                }
            }
            if (this._frozenState == FrozenState.WaitingForGrounded)
            {
                if (this.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Grounded))
                {
                    if (base.abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
                    {
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._frozenAnmatorSpeedIx, 0.1f);
                    }
                    else
                    {
                        this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._frozenAnmatorSpeedIx, 0f);
                    }
                    this._frozenState = FrozenState.FrozenFreezed;
                }
            }
            else if (this._frozenState == FrozenState.FrozenFreezed)
            {
                if (base.abilityState.ContainsState(AbilityState.SlowWhenFrozenOrParalyze))
                {
                    this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._frozenAnmatorSpeedIx, 0.1f);
                }
                else
                {
                    this.monster.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._frozenAnmatorSpeedIx, 0f);
                }
            }
        }

        private enum FrozenState
        {
            Idle,
            WaitingForGrounded,
            FrozenFreezed
        }

        private enum ParalyzeState
        {
            Idle,
            WaitingForGrounded,
            ParalyzeFreezed,
            ParalyzeHitResuming
        }
    }
}

