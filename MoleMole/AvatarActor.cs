namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UniRx;
    using UnityEngine;

    public class AvatarActor : BaseAbilityActor
    {
        private bool _allowOtherCanSwithInWhenSelfOnStage;
        private int _freezeAnimatorSpeedIx;
        private int _freezeMassRatioIx;
        private bool _isInQTEWarning;
        [ShowInInspector]
        protected bool _isOnStage;
        private bool _muteOtherQTE;
        private int _paralyzeAnimatorSpeedIx;
        private int _paralyzeMassRatioIx;
        private ParalyzeState _paralyzeState;
        private EntityTimer _paralyzeTimer;
        private Dictionary<string, SKillInfo> _skillInfoMap;
        private int _stunMassRatioIx;
        private EntityTimer _switchInTimer;
        private TiedState _tiedState;
        private bool _useATKButtonHoldMode;
        public BaseMonoAvatar avatar;
        [InspectorCollapsedFoldout]
        public AvatarDataItem avatarDataItem;
        public string avatarIconPath;
        [InspectorCollapsedFoldout]
        public ConfigAvatar config;
        private const float COOLED_DOWN_TIME = -253f;
        public SafeFloat critical = 0f;
        private static string[] DEFAULT_SKILL_BUTTON_NAMES = new string[] { "ATK", "SKL01", "SKL02" };
        public List<string> maskedSkillButtons;
        public Action<bool> onQTEChange;
        public Action<string, int, int> onSkillChargeChanged;
        public Action<string, float, float> onSkillSPNeedChanged;
        public List<SKillInfo> skillInfoList;

        public void AmendHitCollision(AttackResult attackResult)
        {
            if (attackResult.hitCollision == null)
            {
                AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                    hitPoint = this.avatar.RootNodePosition,
                    hitDir = -this.avatar.FaceDirection
                };
                attackResult.hitCollision = collsion;
            }
            else if (attackResult.hitCollision.hitDir == Vector3.zero)
            {
                attackResult.hitCollision.hitPoint = this.avatar.RootNodePosition;
                attackResult.hitCollision.hitDir = -this.avatar.FaceDirection;
            }
        }

        public virtual void AttackLanded(EvtAttackLanded evt)
        {
            this.avatar.FrameHalt(evt.attackResult.frameHalt);
        }

        public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect, uint sourceID)
        {
            bool doSteerToHitForward = false;
            bool targetLockSource = false;
            if ((attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light) && (attackResult.hitCollision.hitDir != Vector3.zero))
            {
                doSteerToHitForward = true;
            }
            if ((Singleton<AvatarManager>.Instance.IsLocalAvatar(base.runtimeID) && (this.avatar.AttackTarget == null)) && (this.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput) && (Singleton<RuntimeIDManager>.Instance.ParseCategory(sourceID) == 4)))
            {
                BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(sourceID);
                if ((monster != null) && !monster.denySelect)
                {
                    targetLockSource = true;
                }
            }
            if ((this._paralyzeState == ParalyzeState.ParalyzeFreezed) && (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light))
            {
                this.avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 1f);
                this._paralyzeTimer.timespan = 0.35f;
                this._paralyzeTimer.Reset(true);
                this._paralyzeState = ParalyzeState.ParalyzeHitResuming;
            }
            if (base.abilityState.ContainsState(AbilityState.Frozen))
            {
                doSteerToHitForward = false;
                if (attackResult.hitEffect > AttackResult.AnimatorHitEffect.Light)
                {
                    attackResult.hitEffect = AttackResult.AnimatorHitEffect.Light;
                }
            }
            this.avatar.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity, sourceID, targetLockSource, doSteerToHitForward);
        }

        public bool CanSwitchInQTE()
        {
            return !string.IsNullOrEmpty(this.CurrentQTEName);
        }

        public bool CanUseSkill(string skillName)
        {
            if (this._skillInfoMap[skillName].muted)
            {
                return false;
            }
            if (this.IsSkillLocked(skillName))
            {
                return false;
            }
            if (this._skillInfoMap[skillName].MaxChargesCount <= 0)
            {
                return (this.IsSPEnough(skillName) && !this.IsSkillInCD(skillName));
            }
            if (skillName == "SKL01")
            {
                return this.IsSPEnough(skillName);
            }
            return (this.IsSPEnough(skillName) && (this._skillInfoMap[skillName].chargesCounter > 0));
        }

        public void ChangeSwitchInCDTime(float CDTime)
        {
            this._switchInTimer = new EntityTimer(CDTime, base.entity);
            this._switchInTimer.timeupAction = (Action) Delegate.Combine(this._switchInTimer.timeupAction, new Action(this.OnSwitchInReady));
            this._switchInTimer.Reset(false);
            this._switchInTimer.isTimeUp = true;
        }

        public override void Core()
        {
            base.Core();
            if (!this._allowOtherCanSwithInWhenSelfOnStage)
            {
                this._switchInTimer.Core(1f);
            }
            if (base.isAlive != 0)
            {
                MonoLevelEntity levelEntity = Singleton<LevelManager>.Instance.levelEntity;
                for (int i = 0; i < this.skillInfoList.Count; i++)
                {
                    SKillInfo info = this.skillInfoList[i];
                    if (info.cdTimer >= 0f)
                    {
                        info.cdTimer -= levelEntity.TimeScale * Time.deltaTime;
                        if (info.cdTimer < 0f)
                        {
                            info.cdTimer = -253f;
                        }
                        if ((info.cdTimer == -253f) && (info.MaxChargesCount > 0))
                        {
                            int chargesCounter = (int) info.chargesCounter;
                            int num3 = Mathf.Clamp(chargesCounter + 1, 0, (int) info.MaxChargesCount);
                            info.chargesCounter = num3;
                            if (this.onSkillChargeChanged != null)
                            {
                                this.onSkillChargeChanged(info.skillName, chargesCounter, num3);
                            }
                            if (info.chargesCounter < info.MaxChargesCount)
                            {
                                info.cdTimer = this.GetSkillCD(info.skillName);
                            }
                        }
                    }
                }
                this.UpdateAbilityState();
            }
        }

        public Dictionary<string, object> CreateAppliedAbility(ConfigAbility abilityConfig)
        {
            Dictionary<string, object> dictionary;
            return new Dictionary<string, object> { Tuple.Create<ConfigAbility, Dictionary<string, object>>(abilityConfig, dictionary) };
        }

        protected virtual void DamageLanded(EvtDamageLanded evt)
        {
            this.avatar.FrameHalt(evt.attackResult.frameHalt);
        }

        public void DisableQTEAttack()
        {
            this.CurrentQTEName = string.Empty;
            this._isInQTEWarning = false;
            if (this.onQTEChange != null)
            {
                this.onQTEChange(false);
            }
        }

        public void EnableQTEAttack(string QTEName)
        {
            this.CurrentQTEName = QTEName;
            this._isInQTEWarning = true;
            if (this.onQTEChange != null)
            {
                this.onQTEChange(true);
            }
        }

        public void FireAttackDataEffects(AttackResult attackResult)
        {
            if ((attackResult.attackEffectPattern != null) && ((attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal) || (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack)))
            {
                AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, this.avatar, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
            }
            if (attackResult.attackCameraShake != null)
            {
                AttackPattern.ActCameraShake(attackResult.attackCameraShake);
            }
        }

        public void ForceKill()
        {
            this.Kill(0x21800001, null, KillEffect.KillNow);
        }

        public override void ForceKill(uint killerID, KillEffect killEffect)
        {
            this.Kill(killerID, null, killEffect);
        }

        public float GetSkillCD(string skillName)
        {
            float cD = (float) this._skillInfoMap[skillName].CD;
            if (skillName == "SKL01")
            {
                return (cD * (1f + base.GetProperty("Actor_SKL01CDRatio")));
            }
            if (skillName == "SKL02")
            {
                cD *= 1f + base.GetProperty("Actor_SKL02CDRatio");
            }
            return cD;
        }

        public SKillInfo GetSkillInfo(string skillName)
        {
            return (!this._skillInfoMap.ContainsKey(skillName) ? null : this._skillInfoMap[skillName]);
        }

        public string GetSkillNameByAnimEventID(string animEventID)
        {
            return (!animEventID.StartsWith("ATK") ? animEventID : "ATK");
        }

        public float GetSkillSPCost(string skillName)
        {
            float num = 0f;
            if ((!(skillName == "SKL01") && !(skillName == "SKL02")) && !(skillName == "SKL_WEAPON"))
            {
                return num;
            }
            float property = base.GetProperty("Actor_SkillSPCostDelta");
            float num3 = base.GetProperty("Actor_SkillSPCostRatio");
            return ((this._skillInfoMap[skillName].costSP + property) * (1f + num3));
        }

        public float GetSkillSPNeed(string skillName)
        {
            float num = 0f;
            if ((!(skillName == "SKL01") && !(skillName == "SKL02")) && !(skillName == "SKL_WEAPON"))
            {
                return num;
            }
            SKillInfo info = this._skillInfoMap[skillName];
            return Mathf.Max((float) info.costSP, (float) info.needSP);
        }

        public float GetSwtichCDRatio()
        {
            return (this._switchInTimer.timer / this._switchInTimer.timespan);
        }

        public bool HasChargesLeft(string skillName)
        {
            return ((this._skillInfoMap[skillName].MaxChargesCount <= 0) || (this._skillInfoMap[skillName].chargesCounter > 0));
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.avatar = (BaseMonoAvatar) entity;
            base.runtimeID = this.avatar.GetRuntimeID();
            this.config = AvatarData.GetAvatarConfig(this.avatar.AvatarTypeName);
            base.commonConfig = this.config.CommonConfig;
            base.Init(entity);
            this.skillInfoList = new List<SKillInfo>();
            this._skillInfoMap = new Dictionary<string, SKillInfo>();
            this.maskedSkillButtons = new List<string>();
            foreach (string str in this.config.Skills.Keys)
            {
                <Init>c__AnonStoreyAE yae = new <Init>c__AnonStoreyAE {
                    <>f__this = this,
                    skillConfig = this.config.Skills[str],
                    skillName = this.GetSkillNameByAnimEventID(str)
                };
                if (Miscs.ArrayContains<string>(DEFAULT_SKILL_BUTTON_NAMES, yae.skillName) && !this._skillInfoMap.ContainsKey(yae.skillName))
                {
                    SKillInfo item = new SKillInfo {
                        skillName = yae.skillName,
                        cdTimer = -253f,
                        CD = Mathf.Max((float) 0f, (float) (yae.skillConfig.SkillCD + base.Evaluate(yae.skillConfig.SkillCDDelta))),
                        costSP = Mathf.Max((float) 0f, (float) (yae.skillConfig.SPCost + base.Evaluate(yae.skillConfig.SPCostDelta))),
                        needSP = Mathf.Max((float) 0f, (float) (yae.skillConfig.SPNeed + base.Evaluate(yae.skillConfig.SPCostDelta))),
                        MaxChargesCount = yae.skillConfig.ChargesCount + base.Evaluate(yae.skillConfig.ChargesCountDelta),
                        canHold = yae.skillConfig.CanHold,
                        reviveCDAction = yae.skillConfig.ReviveCDAction,
                        IsInstantTrigger = yae.skillConfig.IsInstantTrigger,
                        muted = false,
                        muteHighlighted = yae.skillConfig.MuteHighlighted,
                        maskIconPath = null
                    };
                    item.chargesCounter = item.MaxChargesCount;
                    if (yae.skillName == "ATK")
                    {
                        item.muteHighlighted = true;
                    }
                    this.skillInfoList.Add(item);
                    this._skillInfoMap.Add(yae.skillName, item);
                    if (yae.skillConfig.SkillCDDelta.isDynamic)
                    {
                        base.RegisterPropertyChangedCallback(yae.skillConfig.SkillCDDelta.dynamicKey, new Action(yae.<>m__4C));
                    }
                    if (yae.skillConfig.ChargesCountDelta.isDynamic)
                    {
                        base.RegisterPropertyChangedCallback(yae.skillConfig.ChargesCountDelta.dynamicKey, new Action(yae.<>m__4D));
                    }
                    if (yae.skillConfig.SPCostDelta.isDynamic)
                    {
                        base.RegisterPropertyChangedCallback(yae.skillConfig.SPCostDelta.dynamicKey, new Action(yae.<>m__4E));
                    }
                }
            }
            this.avatar.onActiveChanged = (Action<bool>) Delegate.Combine(this.avatar.onActiveChanged, new Action<bool>(this.OnActiveChanged));
            this._switchInTimer = new EntityTimer(this.config.CommonArguments.SwitchInCD, entity);
            this._switchInTimer.timeupAction = (Action) Delegate.Combine(this._switchInTimer.timeupAction, new Action(this.OnSwitchInReady));
            this._switchInTimer.Reset(false);
            this._switchInTimer.isTimeUp = true;
            base.AddPlugin(new AvatarAIPlugin(this));
            Singleton<EventManager>.Instance.FireEvent(new EvtAvatarCreated(base.runtimeID), MPEventDispatchMode.Normal);
            this._paralyzeTimer = new EntityTimer();
            this._paralyzeTimer.SetActive(false);
            this._paralyzeState = ParalyzeState.Idle;
            this.InitAbilityStateImmune();
            this.InitDebuffDurationRatio();
            this.CurrentQTEName = string.Empty;
        }

        private void InitAbilityStateImmune()
        {
            foreach (AbilityState state in this.config.DebuffResistance.ImmuneStates)
            {
                base.SetAbilityStateImmune(state, true);
            }
        }

        public void InitAvatarDataItem(AvatarDataItem avatarDataItem, bool isLocal, bool isHelper, bool isLeader, bool leaderSkillOn)
        {
            this.avatarDataItem = avatarDataItem;
            this.isLeader = isLeader;
            this.InitHPAndSP();
            base.level = avatarDataItem.level;
            base.attack = avatarDataItem.FinalAttack;
            this.critical = avatarDataItem.FinalCritical;
            base.defense = avatarDataItem.FinalDefense;
            this.avatarIconPath = avatarDataItem.IconPathInLevel;
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                this._isOnStage = isLocal || isHelper;
            }
            else
            {
                this._isOnStage = true;
            }
            this.maskedSkillButtons.AddRange(this.config.CommonArguments.MaskedSkillButtons);
            AvatarData.UnlockAvatarAbilities(avatarDataItem, this, isLeader | leaderSkillOn);
            this.SetupSkillInfo(avatarDataItem);
            List<ConfigEquipmentSkillEntry> skillEntryList = new List<ConfigEquipmentSkillEntry>();
            EquipmentSkillData.AddAvatarWeaponEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
            EquipmentSkillData.AddAvatarStigmataEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
            EquipmentSkillData.AddAvatarSetEquipSkillAbilities(avatarDataItem, this, ref skillEntryList);
            this.SetupWeaponActiveSkillInfo(skillEntryList);
            if (isHelper)
            {
                base.AddPlugin(new AvatarHelperStatePlugin(this));
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

        public void InitGalTouchBuff(AvatarDataItem avatarDataItem)
        {
            int buffId = Singleton<GalTouchModule>.Instance.UseBuff(avatarDataItem.avatarID);
            if (buffId != 0)
            {
                TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(buffId);
                if (touchBuffItem != null)
                {
                    int characterHeartLevel = Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel(avatarDataItem.avatarID);
                    float num3 = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param1, touchBuffItem.param1Add, characterHeartLevel);
                    float num4 = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param2, touchBuffItem.param2Add, characterHeartLevel);
                    float num5 = GalTouchBuffData.GetCalculatedParam(touchBuffItem.param3, touchBuffItem.param3Add, characterHeartLevel);
                    GalTouchBuffData.ApplyGalTouchBuffEntry(this, buffId, num3, num4, num5);
                }
            }
        }

        private void InitHPAndSP()
        {
            base.baseMaxHP = base.maxHP = base.HP = this.avatarDataItem.FinalHP;
            base.baseMaxSP = base.maxSP = this.avatarDataItem.FinalSP;
            base.SP = 0f;
        }

        public bool IsAttackButtonHoldMode()
        {
            return (this._useATKButtonHoldMode || this.avatar.IsAttackHoldMode());
        }

        public bool IsOnStage()
        {
            return this._isOnStage;
        }

        public bool IsSkillHasCD(string skillName)
        {
            return (this._skillInfoMap[skillName].CD > 0f);
        }

        public bool IsSkillInCD(string skillName)
        {
            return (this._skillInfoMap[skillName].cdTimer >= 0f);
        }

        public bool IsSkillLocked(string skillName)
        {
            return this.maskedSkillButtons.Contains(skillName);
        }

        public bool IsSPEnough(string skillName)
        {
            return ((base.SP >= this.GetSkillSPCost(skillName)) && (base.SP >= this.GetSkillSPNeed(skillName)));
        }

        public bool IsSwitchInCD()
        {
            return !this._switchInTimer.isTimeUp;
        }

        public virtual void Kill(uint killerID, string killerAnimEventID, KillEffect killEffect)
        {
            if (base.onJustKilled != null)
            {
                base.onJustKilled(killerID, killerAnimEventID, killEffect);
            }
            if (base.isAlive == 0)
            {
                if (killEffect == KillEffect.KillImmediately)
                {
                    this.avatar.SetDied(KillEffect.KillImmediately);
                }
            }
            else
            {
                base.isAlive = 0;
                Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID, killerID, killerAnimEventID), MPEventDispatchMode.Normal);
                this.avatar.SetDied(KillEffect.KillNow);
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = base.ListenEvent(evt);
            if (evt is EvtLevelBuffState)
            {
                flag |= this.OnLevelBuffState((EvtLevelBuffState) evt);
            }
            return flag;
        }

        protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
            base.OnAbilityStateAdd(state, muteDisplayEffect);
            if (state == AbilityState.Stun)
            {
                this.avatar.SetTrigger("TriggerHit");
                this.avatar.SetLocomotionBool("BuffStun", true, false);
                this._stunMassRatioIx = base.PushProperty("Entity_MassRatio", 1000f);
            }
            else if (state == AbilityState.Paralyze)
            {
                this.avatar.SetTrigger("TriggerHit");
                this.avatar.SetLocomotionBool("BuffParalyze", true, false);
                this._paralyzeAnimatorSpeedIx = this.avatar.PushProperty("Animator_OverallSpeedRatioMultiplied", 1f);
                this.avatar.SetCountedMuteControl(true);
                this._paralyzeMassRatioIx = base.PushProperty("Entity_MassRatio", 1000f);
                this._paralyzeTimer.Reset(true);
                this._paralyzeTimer.timespan = 0.35f;
                this._paralyzeState = ParalyzeState.ParalyzeHitResuming;
                this.avatar.OrderMove = false;
                this.avatar.ClearAttackTriggers();
            }
            else if (state == AbilityState.Frozen)
            {
                this.avatar.SetLocomotionBool("BuffParalyze", true, false);
                this._freezeAnimatorSpeedIx = this.avatar.PushProperty("Animator_OverallSpeedRatioMultiplied", 0f);
                this.avatar.SetCountedMuteControl(true);
                this._freezeMassRatioIx = base.PushProperty("Entity_MassRatio", 1000f);
            }
            else if (state == AbilityState.Tied)
            {
                this._tiedState = TiedState.Tieing;
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtBuffAdd(base.runtimeID, state), MPEventDispatchMode.Normal);
        }

        protected override void OnAbilityStateRemove(AbilityState state)
        {
            base.OnAbilityStateRemove(state);
            if (state == AbilityState.Stun)
            {
                this.avatar.SetLocomotionBool("BuffStun", false, false);
                base.PopProperty("Entity_MassRatio", this._stunMassRatioIx);
                this.avatar.ClearAttackTriggers();
            }
            else if (state == AbilityState.Paralyze)
            {
                this.avatar.SetLocomotionBool("BuffParalyze", false, false);
                this.avatar.PopProperty("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx);
                this.avatar.SetCountedMuteControl(false);
                base.PopProperty("Entity_MassRatio", this._paralyzeMassRatioIx);
                this._paralyzeState = ParalyzeState.Idle;
                this.avatar.OrderMove = false;
                this.avatar.ClearAttackTriggers();
            }
            else if (state == AbilityState.Frozen)
            {
                this.avatar.SetLocomotionBool("BuffParalyze", false, false);
                this.avatar.PopProperty("Animator_OverallSpeedRatioMultiplied", this._freezeAnimatorSpeedIx);
                this.avatar.SetCountedMuteControl(false);
                this.avatar.PopProperty("Entity_MassRatio", this._freezeMassRatioIx);
                this.avatar.ClearAttackTriggers();
            }
            else if (state == AbilityState.Tied)
            {
                this._tiedState = TiedState.Idle;
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtBuffRemove(base.runtimeID, state), MPEventDispatchMode.Normal);
        }

        private void OnActiveChanged(bool active)
        {
            this._isOnStage = active;
        }

        private bool OnAttackLanded(EvtAttackLanded evt)
        {
            this.AttackLanded(evt);
            if (evt.animEventID != null)
            {
                ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, evt.animEventID);
                if (event2 == null)
                {
                    return true;
                }
                if (base.isAlive != 0)
                {
                    float delta = (event2.AttackProperty.SPRecover * this.config.CommonArguments.AttackSPRecoverRatio) * (1f + base.GetProperty("Actor_SPRecoverRatio"));
                    DelegateUtils.UpdateField(ref this.SP, Mathf.Clamp(base.SP + delta, 0f, (float) base.maxSP), delta, base.onSPChanged);
                    float num2 = evt.attackResult.damage * base.GetProperty("Actor_AttackStealHPRatio");
                    DelegateUtils.UpdateField(ref this.HP, Mathf.Clamp(base.HP + num2, 0f, (float) base.maxHP), num2, base.onHPChanged);
                }
                base.MarkImportantEventIsHandled(evt);
            }
            return true;
        }

        public bool OnAvatarSwappedOutStart(EvtAvatarSwapOutStart evt)
        {
            bool flag = false;
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                if ((avatar.GetRuntimeID() != base.runtimeID) && Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).CanSwitchInQTE())
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                this._switchInTimer.timespan *= this.config.CommonArguments.QTESwitchInCDRatio;
            }
            this._switchInTimer.Reset(true);
            return true;
        }

        protected virtual bool OnBeingHit(EvtBeingHit evt)
        {
            if (!this._isOnStage)
            {
                evt.attackData.Reject(AttackResult.RejectType.RejectAll);
                return true;
            }
            if (base.isAlive == 0)
            {
                evt.attackData.Reject(AttackResult.RejectType.RejectAll);
                return true;
            }
            DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
            return true;
        }

        protected virtual bool OnBeingHitResolve(EvtBeingHit evt)
        {
            evt.Resolve();
            if (evt.attackData.isAnimEventAttack && base.abilityState.ContainsState(AbilityState.BlockAnimEventAttack))
            {
                evt.attackData.Reject(AttackResult.RejectType.RejectAll);
            }
            if (evt.attackData.rejected)
            {
                if (evt.attackData.rejectState == AttackResult.RejectType.RejectButShowAttackEffect)
                {
                    this.AmendHitCollision(evt.attackData);
                    this.FireAttackDataEffects(evt.attackData);
                }
                return false;
            }
            AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
            this.AmendHitCollision(attackResult);
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
                attackResult.hitEffectPattern = AttackResult.HitEffectPattern.OnlyBeHit;
                attackResult.attackCameraShake = null;
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
                    this.BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
                }
                else
                {
                    evt.beHitEffect = BeHitEffect.KillingBeHit;
                    this.Kill(evt.sourceID, evt.animEventID, KillEffect.KillNow);
                    this.BeingHit(attackResult, BeHitEffect.KillingBeHit, evt.sourceID);
                }
            }
            else
            {
                evt.beHitEffect = BeHitEffect.NormalBeHit;
                this.BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
            }
            this.FireAttackDataEffects(attackResult);
            if (((this._tiedState == TiedState.Tieing) && evt.attackData.isAnimEventAttack) && (evt.attackData.hitEffect >= AttackResult.AnimatorHitEffect.Normal))
            {
                base.abilityPlugin.RemoveModifierByState(AbilityState.Tied);
            }
            if (evt.attackData.isAnimEventAttack)
            {
                EvtAttackLanded landed = new EvtAttackLanded(evt.sourceID, base.runtimeID, evt.animEventID, attackResult);
                Singleton<EventManager>.Instance.FireEvent(landed, MPEventDispatchMode.Normal);
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
            flag |= base.OnEventResolves(evt);
            return false;
        }

        public override bool OnEventWithPlugins(BaseEvent evt)
        {
            bool flag = base.OnEventWithPlugins(evt);
            if (evt is EvtSkillStart)
            {
                return this.OnSkillStart((EvtSkillStart) evt);
            }
            if (evt is EvtHittingOther)
            {
                return this.OnHittingOther((EvtHittingOther) evt);
            }
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHit((EvtBeingHit) evt);
            }
            if (evt is EvtKilled)
            {
                return this.OnKill((EvtKilled) evt);
            }
            if (evt is EvtAttackLanded)
            {
                return this.OnAttackLanded((EvtAttackLanded) evt);
            }
            if (evt is EvtDamageLanded)
            {
                return this.OnDamageLanded((EvtDamageLanded) evt);
            }
            if (evt is EvtAvatarSwapOutStart)
            {
                return this.OnAvatarSwappedOutStart((EvtAvatarSwapOutStart) evt);
            }
            return flag;
        }

        protected virtual bool OnHittingOther(EvtHittingOther evt)
        {
            if (evt.attackData == null)
            {
                evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(this, evt.animEventID);
            }
            if ((evt.attackData.hitCollision == null) && (evt.hitCollision != null))
            {
                evt.attackData.hitCollision = evt.hitCollision;
            }
            else if ((evt.hitCollision == null) && (evt.attackData.hitCollision == null))
            {
                BaseMonoAnimatorEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.toID) as BaseMonoAnimatorEntity;
                if (entity != null)
                {
                    AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                        hitPoint = entity.RootNodePosition,
                        hitDir = entity.XZPosition - this.avatar.XZPosition
                    };
                    evt.hitCollision = collsion;
                }
            }
            return true;
        }

        protected virtual bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            evt.Resolve();
            Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, base.runtimeID, evt.animEventID, evt.attackData), MPEventDispatchMode.Normal);
            base.MarkImportantEventIsHandled(evt);
            return true;
        }

        private bool OnKill(EvtKilled evt)
        {
            base.abilityPlugin.RemoveAllDebuffModifiers();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(base.runtimeID);
            return true;
        }

        public bool OnLevelBuffState(EvtLevelBuffState evt)
        {
            if (((evt.levelBuff == LevelBuffType.WitchTime) && (evt.state == LevelBuffState.Start)) && (Singleton<AvatarManager>.Instance.IsLocalAvatar(base.runtimeID) && (evt.sourceId == base.runtimeID)))
            {
                MonoEntityAudio component = base.entity.GetComponent<MonoEntityAudio>();
                if (component != null)
                {
                    component.PostWitchTime();
                }
            }
            return true;
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            this.DisableQTEAttack();
            this.onSkillChargeChanged = null;
            this.onSkillSPNeedChanged = null;
            this.onQTEChange = null;
        }

        private bool OnSkillStart(EvtSkillStart evt)
        {
            string skillID = evt.skillID;
            string skillNameByAnimEventID = this.GetSkillNameByAnimEventID(skillID);
            float skillSPCost = this.GetSkillSPCost(skillNameByAnimEventID);
            DelegateUtils.UpdateField(ref this.SP, Mathf.Clamp(base.SP - skillSPCost, 0f, (float) base.maxSP), -skillSPCost, base.onSPChanged);
            if (this._skillInfoMap[skillNameByAnimEventID].MaxChargesCount > 0)
            {
                int chargesCounter = (int) this._skillInfoMap[skillNameByAnimEventID].chargesCounter;
                int num3 = Mathf.Clamp(chargesCounter - 1, 0, (int) this._skillInfoMap[skillNameByAnimEventID].MaxChargesCount);
                this._skillInfoMap[skillNameByAnimEventID].chargesCounter = num3;
                if (this.onSkillChargeChanged != null)
                {
                    this.onSkillChargeChanged(evt.skillID, chargesCounter, num3);
                }
                if (!this.IsSkillInCD(skillNameByAnimEventID))
                {
                    this._skillInfoMap[skillNameByAnimEventID].cdTimer = this.GetSkillCD(skillNameByAnimEventID);
                }
            }
            else
            {
                this._skillInfoMap[skillNameByAnimEventID].cdTimer = this.GetSkillCD(skillNameByAnimEventID);
            }
            this.avatar.ClearAttackTriggers();
            return true;
        }

        public void OnSwitchInReady()
        {
            this._switchInTimer.timespan = this.config.CommonArguments.SwitchInCD;
        }

        public override void PostInit()
        {
            base.PostInit();
            base.abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.RemoveAllDebuffsAndDurationed;
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(base.runtimeID);
        }

        private void ResetHPAndSPWhenRevive()
        {
            DelegateUtils.UpdateField(ref this.maxHP, this.avatarDataItem.FinalHP, base.onMaxHPChanged);
            DelegateUtils.UpdateField(ref this.maxSP, this.avatarDataItem.FinalSP, base.onMaxSPChanged);
            DelegateUtils.UpdateField(ref this.HP, (float) base.maxHP, 0f, base.onHPChanged);
            DelegateUtils.UpdateField(ref this.SP, (float) base.maxSP, 0f, base.onSPChanged);
            base.HPPropertyChangedCallback();
            base.SPPropertyChangedCallback();
        }

        public void ResetSwitchInTimer()
        {
            this._switchInTimer.Reset(false);
            this._switchInTimer.isTimeUp = false;
        }

        public void Revive(Vector3 revivePosition)
        {
            base.isAlive = 1;
            this.ResetHPAndSPWhenRevive();
            this.SetSkillCDWhenRevive();
            base.abilityPlugin.ResetKilled();
            this.avatar.Revive(revivePosition);
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                if ((localAvatar == this.avatar) || (localAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OffStage))
                {
                    Singleton<LevelManager>.Instance.levelActor.SingleModeSwapTo(revivePosition, localAvatar.FaceDirection, this.avatar);
                }
                else
                {
                    Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(localAvatar.GetRuntimeID(), base.runtimeID, true);
                }
            }
            else if ((Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi) || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.MultiRemote))
            {
                Singleton<LevelManager>.Instance.levelActor.SingleModeSwapTo(revivePosition, localAvatar.FaceDirection, this.avatar);
                if ((localAvatar != this.avatar) && (localAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OnStage))
                {
                    localAvatar.RefreshController();
                }
            }
        }

        public void SetAllowOtherCanSwitchIn(bool canSwitchIn)
        {
            this._allowOtherCanSwithInWhenSelfOnStage = canSwitchIn;
        }

        public void SetAttackButtonHoldMode(bool useHoldMod)
        {
            this._useATKButtonHoldMode = useHoldMod;
        }

        public void SetAvatarAttackRatio(float ratio)
        {
            base.attack *= ratio;
        }

        public void SetMuteSkill(string skillName, bool muted)
        {
            this._skillInfoMap[skillName].muted = muted;
        }

        private void SetSkillCDWhenRevive()
        {
            foreach (SKillInfo info in this._skillInfoMap.Values)
            {
                switch (info.reviveCDAction)
                {
                    case ReviveSkillCDAction.Cleer:
                        info.cdTimer = -253f;
                        info.chargesCounter = info.MaxChargesCount;
                        break;
                }
            }
        }

        private bool SetupSkillInfo(AvatarDataItem avatarDataItem)
        {
            foreach (AvatarSkillDataItem item in avatarDataItem.skillDataList)
            {
                if (this._skillInfoMap.ContainsKey(item.ButtonName))
                {
                    this._skillInfoMap[item.ButtonName].avatarSkillID = item.skillID;
                    this._skillInfoMap[item.ButtonName].iconPath = item.IconPathInLevel;
                }
            }
            return false;
        }

        private void SetupWeaponActiveSkillInfo(List<ConfigEquipmentSkillEntry> skillEntryList)
        {
            foreach (ConfigEquipmentSkillEntry entry in skillEntryList)
            {
                if (entry.IsActiveSkill)
                {
                    EquipSkillMetaData equipSkillMetaDataByKey = EquipSkillMetaDataReader.GetEquipSkillMetaDataByKey(entry.EquipmentSkillID);
                    SKillInfo item = new SKillInfo {
                        skillName = "SKL_WEAPON",
                        CD = entry.SkillCD,
                        costSP = entry.SPCost,
                        needSP = entry.SPNeed,
                        MaxChargesCount = entry.MaxChargesCount,
                        chargesCounter = entry.MaxChargesCount,
                        cdTimer = -253f,
                        canHold = false,
                        avatarSkillID = entry.EquipmentSkillID,
                        iconPath = equipSkillMetaDataByKey.skillIconPath
                    };
                    this.skillInfoList.Add(item);
                    this._skillInfoMap.Add("SKL_WEAPON", item);
                    break;
                }
            }
        }

        private void UpdateAbilityState()
        {
            if (this._paralyzeState == ParalyzeState.WaitForGrounded)
            {
                if (!this.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Throw))
                {
                    this.avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0f);
                    this._paralyzeState = ParalyzeState.ParalyzeFreezed;
                }
            }
            else if ((this._paralyzeState != ParalyzeState.ParalyzeFreezed) && (this._paralyzeState == ParalyzeState.ParalyzeHitResuming))
            {
                this._paralyzeTimer.Core(1f);
                if (this.avatar.IsAnimatorInTag(AvatarData.AvatarTagGroup.Throw))
                {
                    this.avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 1f);
                    this._paralyzeTimer.Reset(false);
                    this._paralyzeState = ParalyzeState.WaitForGrounded;
                }
                else if (this._paralyzeTimer.isTimeUp)
                {
                    this.avatar.SetPropertyByStackIndex("Animator_OverallSpeedRatioMultiplied", this._paralyzeAnimatorSpeedIx, 0f);
                    this._paralyzeTimer.Reset(false);
                    this._paralyzeState = ParalyzeState.ParalyzeFreezed;
                }
            }
        }

        public bool AllowOtherSwitchIn
        {
            get
            {
                return this._allowOtherCanSwithInWhenSelfOnStage;
            }
            set
            {
                this._allowOtherCanSwithInWhenSelfOnStage = value;
            }
        }

        public string CurrentQTEName { get; private set; }

        public bool IsInQTE
        {
            get
            {
                return this._isInQTEWarning;
            }
        }

        public bool isLeader { get; set; }

        public bool MuteOtherQTE
        {
            get
            {
                return this._muteOtherQTE;
            }
            set
            {
                this._muteOtherQTE = value;
            }
        }

        public SwitchEffectLevel switchButtonEffect { get; private set; }

        [CompilerGenerated]
        private sealed class <Init>c__AnonStoreyAE
        {
            internal AvatarActor <>f__this;
            internal ConfigAvatarSkill skillConfig;
            internal string skillName;

            internal void <>m__4C()
            {
                this.<>f__this._skillInfoMap[this.skillName].CD = Mathf.Max((float) 0f, (float) (this.skillConfig.SkillCD + this.<>f__this.Evaluate(this.skillConfig.SkillCDDelta)));
            }

            internal void <>m__4D()
            {
                int chargesCounter = (int) this.<>f__this._skillInfoMap[this.skillName].chargesCounter;
                this.<>f__this._skillInfoMap[this.skillName].MaxChargesCount = this.skillConfig.ChargesCount + this.<>f__this.Evaluate(this.skillConfig.ChargesCountDelta);
                this.<>f__this._skillInfoMap[this.skillName].chargesCounter = this.<>f__this._skillInfoMap[this.skillName].MaxChargesCount;
                if (this.<>f__this.onSkillChargeChanged != null)
                {
                    this.<>f__this.onSkillChargeChanged(this.skillName, chargesCounter, (int) this.<>f__this._skillInfoMap[this.skillName].chargesCounter);
                }
            }

            internal void <>m__4E()
            {
                float needSP = (float) this.<>f__this._skillInfoMap[this.skillName].needSP;
                this.<>f__this._skillInfoMap[this.skillName].costSP = Mathf.Max((float) 0f, (float) (this.skillConfig.SPCost + this.<>f__this.Evaluate(this.skillConfig.SPCostDelta)));
                this.<>f__this._skillInfoMap[this.skillName].needSP = Mathf.Max((float) 0f, (float) (this.skillConfig.SPNeed + this.<>f__this.Evaluate(this.skillConfig.SPCostDelta)));
                if (this.<>f__this.onSkillSPNeedChanged != null)
                {
                    this.<>f__this.onSkillSPNeedChanged(this.skillName, needSP, (float) this.<>f__this._skillInfoMap[this.skillName].needSP);
                }
            }
        }

        private enum ParalyzeState
        {
            Idle,
            WaitForGrounded,
            ParalyzeFreezed,
            ParalyzeHitResuming
        }

        [Serializable]
        public class SKillInfo
        {
            public int avatarSkillID;
            public bool canHold;
            public SafeFloat CD = 0f;
            public SafeFloat cdTimer = 0f;
            public SafeInt32 chargesCounter = 0;
            public SafeFloat costSP = 0f;
            public string iconPath;
            public bool IsInstantTrigger;
            public string maskIconPath;
            public SafeInt32 MaxChargesCount = 0;
            public bool muted;
            public bool muteHighlighted;
            public SafeFloat needSP = 0f;
            public ReviveSkillCDAction reviveCDAction;
            public string skillName;
        }

        public enum SwitchEffectLevel
        {
            Idle,
            SwitchEffect,
            QTESwitchEffect
        }

        private enum TiedState
        {
            Idle,
            Tieing
        }
    }
}

