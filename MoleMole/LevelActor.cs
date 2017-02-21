namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LevelActor : BaseAbilityActor
    {
        private AvatarSwapState _avatarSwapState;
        private ComboTimerState _comboTimerState;
        private bool _countAnimEventComboOnceInOneFrame = true;
        private LevelDamageStasticsPlugin _damagePlugin;
        private HashSet<KeyValuePair<int, string>> _frameHitSet;
        private int _frameIndex;
        private SafeFloat _levelComboTimer = 0f;
        private LevelDefendModePlugin _levelDefendModePlugin;
        private float _levelMinSwapTimer;
        private LevelMissionStatisticsPlugin _levelMissionStatisticsPlugin;
        private LevelAIPlugin _levelMonsterAIPlugin;
        private uint _swapInID;
        private uint _swapOutID;
        public LevelMinSwapTimerState _swapTimerState;
        private Action _timeSlowDoneCallback;
        private float _timeSlowTimer;
        [CompilerGenerated]
        private static Predicate<BaseMonoAvatar> <>f__am$cache1C;
        public LevelAntiCheatPlugin antiCheatPlugin;
        public Action comboTimeUPCallback;
        public float downLevelNatureBonusFactor = 1f;
        private const float LEVEL_COMBO_WINDOW_TIME = 2.8f;
        private const float LEVEL_MIN_SWAP_TIME = 0.5f;
        public BaseLevelBuff[] levelBuffs;
        public SafeInt32 levelCombo = 0;
        public MonoLevelEntity levelEntity;
        public Mode levelMode;
        private const int MAX_FRAME_INDEX = 20;
        public Action<int, int> onLevelComboChanged;
        public LevelBuffStopWorld stopWorldLevelBuff;
        public float upLevelNatureBonusFactor = 1f;
        public LevelBuffWitchTime witchTimeLevelBuff;

        public void AddTriggerFieldInDefendMode(TriggerFieldActor triggerFieldActor)
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                this._levelDefendModePlugin.AddTriggerFieldActor(triggerFieldActor);
            }
        }

        public void ControlLevelDamageStastics(DamageStastcisControlType type)
        {
            if (base.HasPlugin<LevelDamageStasticsPlugin>())
            {
                this._damagePlugin.ControlDamageStastics(type);
            }
        }

        public override void Core()
        {
            this._frameIndex++;
            if (this._frameIndex > 20)
            {
                this._frameHitSet.Clear();
                this._frameIndex = 0;
            }
            base.Core();
            if (this._timeSlowTimer > 0f)
            {
                this._timeSlowTimer -= Time.unscaledDeltaTime;
                if (this._timeSlowTimer <= 0f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                    Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_End", null, null, null);
                    if (this._timeSlowDoneCallback != null)
                    {
                        this._timeSlowDoneCallback();
                        this._timeSlowDoneCallback = null;
                    }
                }
            }
            if (this._swapTimerState == LevelMinSwapTimerState.Running)
            {
                this._levelMinSwapTimer -= this.levelEntity.TimeScale * Time.deltaTime;
                if (this._levelMinSwapTimer < 0f)
                {
                    this._swapTimerState = LevelMinSwapTimerState.Idle;
                    this._levelMinSwapTimer = 0f;
                }
            }
            if (this._avatarSwapState == AvatarSwapState.Started)
            {
                this._avatarSwapState = AvatarSwapState.Idle;
            }
            if (this._levelComboTimer > 0f)
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (localAvatar != null)
                {
                    string currentSkillID = localAvatar.CurrentSkillID;
                    if (!string.IsNullOrEmpty(currentSkillID) && localAvatar.config.Skills.ContainsKey(currentSkillID))
                    {
                        float comboTimerPauseNormalizedTimeStart = localAvatar.config.Skills[currentSkillID].ComboTimerPauseNormalizedTimeStart;
                        float comboTimerPauseNormalizedTimeStop = localAvatar.config.Skills[currentSkillID].ComboTimerPauseNormalizedTimeStop;
                        float currentNormalizedTime = localAvatar.GetCurrentNormalizedTime();
                        if (comboTimerPauseNormalizedTimeStart < comboTimerPauseNormalizedTimeStop)
                        {
                            if ((currentNormalizedTime > comboTimerPauseNormalizedTimeStart) && (currentNormalizedTime < comboTimerPauseNormalizedTimeStop))
                            {
                                this._comboTimerState = ComboTimerState.Pause;
                            }
                            else if (currentNormalizedTime > comboTimerPauseNormalizedTimeStop)
                            {
                                this._comboTimerState = ComboTimerState.Running;
                            }
                        }
                    }
                }
                if (this._comboTimerState == ComboTimerState.Running)
                {
                    this._levelComboTimer -= this.levelEntity.TimeScale * Time.deltaTime;
                }
                if (this._levelComboTimer < 0f)
                {
                    if (this.comboTimeUPCallback != null)
                    {
                        this.comboTimeUPCallback();
                    }
                    else
                    {
                        DelegateUtils.UpdateField(ref this.levelCombo, 0, this.onLevelComboChanged);
                    }
                }
            }
        }

        private void DragAllAvatarsNearLocalAvatar()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            List<BaseMonoAvatar> list2 = new List<BaseMonoAvatar>();
            if (helperAvatar != null)
            {
                list2.Add(helperAvatar);
            }
            foreach (BaseMonoAvatar avatar3 in allPlayerAvatars)
            {
                if (avatar3 != localAvatar)
                {
                    list2.Add(avatar3);
                }
            }
            foreach (BaseMonoAvatar avatar4 in list2)
            {
                if (Physics.Linecast(localAvatar.transform.position, avatar4.transform.position, (int) (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER)))
                {
                    avatar4.transform.position = localAvatar.transform.position;
                }
            }
        }

        public override void ForceKill(uint killerID, KillEffect killEffect)
        {
        }

        private string GetFootStepNameFromStageTypeName(string stageTypeName)
        {
            string str = "Tile";
            if (stageTypeName.IndexOf("Spaceship") != -1)
            {
                return "Metal";
            }
            if (stageTypeName.IndexOf("ME") != -1)
            {
                return "Concrete";
            }
            if (stageTypeName.IndexOf("NZ_Town") != -1)
            {
                str = "Grass";
            }
            return str;
        }

        public int GetLevelDefendModeMonsterEnterAmount()
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                return this._levelDefendModePlugin.MonsterEnterAmount;
            }
            return 0;
        }

        public int GetLevelDefendModeMonsterKillAmount()
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                return this._levelDefendModePlugin.MonsterKillAmount;
            }
            return 0;
        }

        protected virtual void HandleAvatarCreationForStageCreation(EvtStageCreated evt, out bool sendStageReady)
        {
            List<MonoSpawnPoint> avatarSpawnPointList = new List<MonoSpawnPoint>();
            foreach (string str in evt.avatarSpawnNameList)
            {
                int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(str);
                avatarSpawnPointList.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
            }
            if (evt.isBorn)
            {
                Singleton<AvatarManager>.Instance.CreateTeamAvatars();
                Singleton<AvatarManager>.Instance.InitAvatarsPos(avatarSpawnPointList);
                Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
            }
            else
            {
                Singleton<AvatarManager>.Instance.InitAvatarsPos(avatarSpawnPointList);
                Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
            }
            sendStageReady = true;
        }

        public override void HealHP(float amount)
        {
        }

        public override void HealSP(float amount)
        {
        }

        private void HideDeadBody()
        {
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                if (!avatar.IsAlive())
                {
                    avatar.gameObject.SetActive(false);
                }
            }
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.levelEntity = (MonoLevelEntity) entity;
            base.commonConfig = this.levelEntity.commonConfig;
            base.Init(entity);
            base.runtimeID = 0x21800001;
            this.levelState = LevelState.LevelLoaded;
            this._damagePlugin = new LevelDamageStasticsPlugin(this);
            if (Singleton<LevelScoreManager>.Instance.collectAntiCheatData)
            {
                this.antiCheatPlugin = new LevelAntiCheatPlugin(this._damagePlugin);
                base.AddPlugin(this.antiCheatPlugin);
            }
            this._frameHitSet = new HashSet<KeyValuePair<int, string>>();
            this._frameIndex = 0;
            LevelChallengeHelperPlugin plugin = new LevelChallengeHelperPlugin(this);
            base.AddPlugin(plugin);
            int levelId = Singleton<LevelScoreManager>.Instance.LevelId;
            if ((levelId != 0) && (Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(levelId).Count > 0))
            {
                LevelTutorialHelperPlugin plugin2 = new LevelTutorialHelperPlugin(this);
                base.AddPlugin(plugin2);
            }
            this._levelMissionStatisticsPlugin = new LevelMissionStatisticsPlugin(this);
            base.AddPlugin(this._levelMissionStatisticsPlugin);
            this._levelMonsterAIPlugin = new LevelAIPlugin(this);
            base.AddPlugin(this._levelMonsterAIPlugin);
            this.InitAdditionalLevelActorPlugins();
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtReviveAvatar>(base.runtimeID);
            AvatarManager instance = Singleton<AvatarManager>.Instance;
            instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>) Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(this.OnLocalAvatarChanged));
        }

        protected virtual void InitAdditionalLevelActorPlugins()
        {
            this.witchTimeLevelBuff = new LevelBuffWitchTime(this);
            this.stopWorldLevelBuff = new LevelBuffStopWorld(this);
            this.levelBuffs = new BaseLevelBuff[] { this.witchTimeLevelBuff, this.stopWorldLevelBuff };
            base.AddPlugin(new LevelAbilityHelperPlugin(this));
        }

        public bool IsLevelBuffActive(LevelBuffType levelBuffType)
        {
            for (int i = 0; i < this.levelBuffs.Length; i++)
            {
                if (this.levelBuffs[i].levelBuffType == levelBuffType)
                {
                    return this.levelBuffs[i].isActive;
                }
            }
            return false;
        }

        private bool ListenAttackLanded(EvtAttackLanded evt)
        {
            KeyValuePair<int, string> item = new KeyValuePair<int, string>(this._frameIndex, evt.animEventID);
            if ((!this._countAnimEventComboOnceInOneFrame || !this._frameHitSet.Contains(item)) && (((evt.attackResult.isAnimEventAttack && evt.attackResult.isInComboCount) && (!evt.attackResult.rejected && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3))) && Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID)))
            {
                this._frameHitSet.Add(item);
                this.ResetComboTimer();
                DelegateUtils.UpdateField(ref this.levelCombo, ((int) this.levelCombo) + 1, this.onLevelComboChanged);
                if (Singleton<LevelScoreManager>.Instance.maxComboNum < this.levelCombo)
                {
                    Singleton<LevelScoreManager>.Instance.maxComboNum = this.levelCombo;
                }
            }
            return true;
        }

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            return false;
        }

        public bool ListenBeingHit(EvtBeingHit evt)
        {
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID))
            {
                case 4:
                case 3:
                case 7:
                    if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Mute)
                    {
                        return true;
                    }
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDamegeText, evt));
                    break;
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = base.ListenEvent(evt);
            if (evt is EvtKilled)
            {
                return (flag | this.ListenKill((EvtKilled) evt));
            }
            if (evt is EvtBeingHit)
            {
                return (flag | this.ListenBeingHit((EvtBeingHit) evt));
            }
            if (evt is EvtAttackStart)
            {
                return (flag | this.ListenAttackStart((EvtAttackStart) evt));
            }
            if (evt is EvtAttackLanded)
            {
                return (flag | this.ListenAttackLanded((EvtAttackLanded) evt));
            }
            if (evt is EvtReviveAvatar)
            {
                return this.ListenReviveAvatar((EvtReviveAvatar) evt);
            }
            if (evt is EvtAvatarSwapOutStart)
            {
                return this.ListenSwapOutAvatarStart((EvtAvatarSwapOutStart) evt);
            }
            return flag;
        }

        private bool ListenKill(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3)
            {
                if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.targetID))
                {
                    return false;
                }
                if (<>f__am$cache1C == null)
                {
                    <>f__am$cache1C = avatar => avatar.IsAlive();
                }
                if (Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().Find(<>f__am$cache1C) == null)
                {
                    Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base.runtimeID);
                }
            }
            Singleton<DetourManager>.Instance.RemoveDetourElement(evt.targetID);
            return true;
        }

        public bool ListenReviveAvatar(EvtReviveAvatar evt)
        {
            Vector3 revivePosition;
            AvatarActor actor = (AvatarActor) Singleton<EventManager>.Instance.GetActor(evt.avatarID);
            if (evt.isRevivePosAssigned)
            {
                revivePosition = evt.revivePosition;
            }
            else
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                revivePosition = ((localAvatar == null) || !localAvatar.IsActive()) ? evt.revivePosition : localAvatar.XZPosition;
            }
            actor.Revive(revivePosition);
            Singleton<CameraManager>.Instance.GetMainCamera().SetFailPostFX(false);
            base.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig("Level_AvatarReviveInvincible"), actor.runtimeID, 2f);
            return true;
        }

        private bool ListenSwapOutAvatarStart(EvtAvatarSwapOutStart evt)
        {
            if (((this.levelMode == Mode.Single) && (evt.targetID == this._swapOutID)) && (this._avatarSwapState == AvatarSwapState.WaitingForEvent))
            {
                this.SwapLocalAvatar(this._swapOutID, this._swapInID);
            }
            return true;
        }

        private void MultiModeSwap(BaseMonoAvatar swapOutAvatar, BaseMonoAvatar swapInAvatar)
        {
            this._swapTimerState = LevelMinSwapTimerState.Running;
            this._levelMinSwapTimer = 0.5f;
            Singleton<AvatarManager>.Instance.SetLocalAvatar(swapInAvatar.GetRuntimeID());
            Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(swapInAvatar.GetRuntimeID(), true);
            if (swapOutAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OnStage)
            {
                swapOutAvatar.RefreshController();
                swapOutAvatar.OrderMove = false;
            }
        }

        protected override void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
        }

        protected override void OnAbilityStateRemove(AbilityState state)
        {
        }

        public bool OnCreateAvatar(EvtAvatarCreated evt)
        {
            BaseMonoAvatar entity = (BaseMonoAvatar) Singleton<EventManager>.Instance.GetEntity(evt.avatarID);
            if (!Singleton<AvatarManager>.Instance.IsPlayerAvatar(entity))
            {
                if (Singleton<AvatarManager>.Instance.IsHelperAvatar(evt.avatarID))
                {
                    entity.gameObject.SetActive(false);
                }
                return false;
            }
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.avatarID))
            {
                if (false)
                {
                    entity.PlayState("Story");
                }
                else
                {
                    entity.TriggerAppear();
                    entity.RefreshController();
                }
            }
            else if (this.levelMode == Mode.Single)
            {
                entity.gameObject.SetActive(false);
            }
            else
            {
                entity.TriggerAppear();
                entity.RefreshController();
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnAvatarCreate, evt.avatarID));
            return true;
        }

        public bool OnCreateMonster(EvtMonsterCreated evt)
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrow(evt.monsterID);
            return true;
        }

        public bool OnCreateStage(EvtStageCreated evt)
        {
            bool flag;
            this.HandleAvatarCreationForStageCreation(evt, out flag);
            if (evt.isBorn)
            {
                string[] soundBankNames = new string[] { "BK_InLevel_Common" };
                Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
            }
            string footStepNameFromStageTypeName = this.GetFootStepNameFromStageTypeName(Singleton<StageManager>.Instance.GetStageTypeName());
            if (footStepNameFromStageTypeName != null)
            {
                List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
                int num = 0;
                int count = allAvatars.Count;
                while (num < count)
                {
                    Singleton<WwiseAudioManager>.Instance.SetSwitch("Terrain_Type", footStepNameFromStageTypeName, allAvatars[num].gameObject);
                    num++;
                }
            }
            Singleton<DetourManager>.Instance.LoadNavMeshRelatedLevel(Singleton<StageManager>.Instance.GetStageTypeName());
            if (flag)
            {
                EvtStageReady ready = new EvtStageReady {
                    isBorn = evt.isBorn
                };
                Singleton<EventManager>.Instance.FireEvent(ready, MPEventDispatchMode.Normal);
            }
            return true;
        }

        private bool OnDynamicObjectCreated(EvtDynamicObjectCreated evt)
        {
            if ((evt.dynamicType == BaseMonoDynamicObject.DynamicType.Barrier) && (this.levelMode != Mode.MultiRemote))
            {
                this.DragAllAvatarsNearLocalAvatar();
            }
            return true;
        }

        public override bool OnEventResolves(BaseEvent evt)
        {
            return ((evt is EvtHittingOther) && this.OnHittingOtherResolve((EvtHittingOther) evt));
        }

        public override bool OnEventWithPlugins(BaseEvent evt)
        {
            if (evt is EvtLevelState)
            {
                return this.OnLevelState((EvtLevelState) evt);
            }
            if (evt is EvtAvatarCreated)
            {
                return this.OnCreateAvatar((EvtAvatarCreated) evt);
            }
            if (evt is EvtMonsterCreated)
            {
                return this.OnCreateMonster((EvtMonsterCreated) evt);
            }
            if (evt is EvtStageCreated)
            {
                return this.OnCreateStage((EvtStageCreated) evt);
            }
            if (evt is EvtStageReady)
            {
                return this.OnStageReady((EvtStageReady) evt);
            }
            if (evt is EvtStageTriggerCreated)
            {
                return this.OnStageTriggerCreated((EvtStageTriggerCreated) evt);
            }
            if (evt is EvtDynamicObjectCreated)
            {
                return this.OnDynamicObjectCreated((EvtDynamicObjectCreated) evt);
            }
            return ((evt is EvtHittingOther) && this.OnHittingOther((EvtHittingOther) evt));
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            return true;
        }

        private bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            evt.Resolve();
            Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, base.runtimeID, evt.animEventID, evt.attackData), MPEventDispatchMode.Normal);
            return true;
        }

        public bool OnLevelState(EvtLevelState evt)
        {
            if (evt.state == EvtLevelState.State.Start)
            {
                if (this.levelState == LevelState.LevelLoaded)
                {
                    this.levelState = LevelState.LevelStarted;
                }
                Singleton<LevelDesignManager>.Instance.LevelDesignStart();
            }
            else if ((evt.state == EvtLevelState.State.EndWin) || (evt.state == EvtLevelState.State.EndLose))
            {
                this.levelState = LevelState.LevelEnded;
                Singleton<LevelDesignManager>.Instance.LevelDesignEndWithResult(evt.levelEndReason, evt.cgId);
                Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
            }
            else if (evt.state == EvtLevelState.State.EnterTransition)
            {
                this.levelState = LevelState.LevelTransiting;
            }
            else if (evt.state == EvtLevelState.State.ExitTransition)
            {
                this.levelState = LevelState.LevelRunning;
            }
            else if (evt.state == EvtLevelState.State.PostStageReady)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PostStageReady, null));
            }
            return true;
        }

        private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
        {
            Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(to.transform, new Vector3(0f, 2f, 0f));
            Singleton<CameraManager>.Instance.GetMainCamera().SetupFollowAvatar(to.GetRuntimeID());
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatar(to.GetRuntimeID(), from.GetRuntimeID());
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().OnUpdateLocalAvatarAbilityDisplay(to.GetRuntimeID(), from.GetRuntimeID());
            to.RefreshController();
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            this.onLevelComboChanged = null;
        }

        private bool OnStageReady(EvtStageReady evt)
        {
            if (this.levelState == LevelState.LevelStarted)
            {
                this.levelState = LevelState.LevelRunning;
            }
            if (evt.isBorn)
            {
                Singleton<StageManager>.Instance.ApplyActiveStageEffectSettingAndStartCheckingForChange();
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.PostStageReady, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
            return true;
        }

        private bool OnStageTriggerCreated(EvtStageTriggerCreated evt)
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrow(evt.triggerRuntimeID);
            return true;
        }

        public void RemoveLevelDamageStastics()
        {
            base.RemovePlugin(this._damagePlugin);
        }

        public void ResetCombo()
        {
            this.ResetComboTimer();
            DelegateUtils.UpdateField(ref this.levelCombo, 0, this.onLevelComboChanged);
        }

        public void ResetComboTimer()
        {
            this._levelComboTimer = (2.8f + base.GetProperty("Actor_ComboTimerDelta")) * base.GetProperty("Actor_ComboTimerRatio");
        }

        public void ReviveAvatarByID(uint runtimeID, Vector3 revivePosition)
        {
            this.HideDeadBody();
            BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
            Singleton<EventManager>.Instance.FireEvent(new EvtReviveAvatar(avatarByRuntimeID.GetRuntimeID(), true, revivePosition), MPEventDispatchMode.Normal);
        }

        public void SetAvatarBeAttackMaxNum(int maxNum)
        {
            this._levelMonsterAIPlugin.SetAvatarBeAttackMaxNum(maxNum);
        }

        public void SetLevelComboTimerState(ComboTimerState state)
        {
            this._comboTimerState = state;
        }

        public void SetLevelDefendModePluginReset(int targetValue)
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                this._levelDefendModePlugin.Reset(targetValue);
            }
        }

        public void SetLevelDefendModePluginStart()
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                this._levelDefendModePlugin.Reset(0);
            }
            else
            {
                this._levelDefendModePlugin = new LevelDefendModePlugin(this);
                base.AddPlugin(this._levelDefendModePlugin);
            }
            this._levelDefendModePlugin.SetActive(true);
        }

        public void SetLevelDefendModePluginStart(int targetValue)
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                this._levelDefendModePlugin.Reset(targetValue);
            }
            else
            {
                this._levelDefendModePlugin = new LevelDefendModePlugin(this, targetValue);
                base.AddPlugin(this._levelDefendModePlugin);
            }
            this._levelDefendModePlugin.SetActive(true);
        }

        public void SetLevelDefendModePluginStop()
        {
            if ((this._levelDefendModePlugin != null) && base.HasPlugin<LevelDefendModePlugin>())
            {
                this._levelDefendModePlugin.Stop();
                base.RemovePlugin(this._levelDefendModePlugin);
                this._levelDefendModePlugin = null;
            }
        }

        public void SetupLevelDamageStastics()
        {
            base.AddPlugin(this._damagePlugin);
        }

        public void SingleModeSwapTo(Vector3 xzPosition, Vector3 forward, BaseMonoAvatar swapInAvatar)
        {
            this._swapTimerState = LevelMinSwapTimerState.Running;
            this._levelMinSwapTimer = 0.5f;
            if (swapInAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.OffStage)
            {
                swapInAvatar.transform.position = xzPosition;
                swapInAvatar.SteerFaceDirectionTo(forward);
                swapInAvatar.gameObject.SetActive(true);
                swapInAvatar.TriggerSwitchIn();
            }
            else if (swapInAvatar.switchState == BaseMonoAvatar.AvatarSwitchState.SwitchingOut)
            {
                swapInAvatar.TriggerSwitchIn();
            }
            else if (swapInAvatar.IsSwitchOutTriggerSet())
            {
                swapInAvatar.ResetTriggerSwitchOut();
            }
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_SwitchRole", swapInAvatar.XZPosition, swapInAvatar.FaceDirection, Vector3.one, this.levelEntity);
            if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(swapInAvatar.GetRuntimeID()))
            {
                Singleton<AvatarManager>.Instance.SetLocalAvatar(swapInAvatar.GetRuntimeID());
            }
        }

        public void StartLevelBuff(BaseLevelBuff buff, float duration, bool allowRefresh, bool enteringTimeSlow, LevelBuffSide side, uint ownerRuntimeID, bool notStartEffect)
        {
            if (buff.isActive)
            {
                if (buff == this.witchTimeLevelBuff)
                {
                    this.witchTimeLevelBuff.Refresh(duration, side, base.ownerID, enteringTimeSlow, allowRefresh, notStartEffect);
                }
                else if (buff != this.stopWorldLevelBuff)
                {
                }
            }
            else if (buff == this.witchTimeLevelBuff)
            {
                this.witchTimeLevelBuff.Setup(enteringTimeSlow, duration, side, notStartEffect);
                base.AddPlugin(buff);
            }
            else if (buff == this.stopWorldLevelBuff)
            {
                this.stopWorldLevelBuff.Setup(enteringTimeSlow, duration, ownerRuntimeID);
                base.AddPlugin(this.stopWorldLevelBuff);
            }
            buff.isActive = true;
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(buff.levelBuffType, LevelBuffState.Start, side, ownerRuntimeID), MPEventDispatchMode.Normal);
        }

        public virtual void StopLevelBuff(BaseLevelBuff buff)
        {
            base.RemovePlugin(buff);
            buff.isActive = false;
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(buff.levelBuffType, LevelBuffState.Stop, buff.levelBuffSide, base.runtimeID), MPEventDispatchMode.Normal);
        }

        public void SuddenLevelEnd()
        {
            this.levelState = LevelState.LevelEnded;
            Singleton<WwiseAudioManager>.Instance.ClearManualPrepareBank();
        }

        public void SuddenLevelStart()
        {
            this.levelState = LevelState.LevelStarted;
            string[] soundBankNames = new string[] { "BK_InLevel_Common" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
        }

        public void SwapLocalAvatar(uint swapOutID, uint swapInID)
        {
            this._avatarSwapState = AvatarSwapState.Started;
            BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(swapOutID);
            BaseMonoAvatar swapInAvatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(swapInID);
            if (this.levelMode == Mode.Single)
            {
                this.SingleModeSwapTo(avatarByRuntimeID.XZPosition, avatarByRuntimeID.FaceDirection, swapInAvatar);
            }
            else if ((this.levelMode == Mode.Multi) || (this.levelMode == Mode.MultiRemote))
            {
                this.MultiModeSwap(avatarByRuntimeID, swapInAvatar);
            }
        }

        public void TimeSlow(float duration)
        {
            this.TimeSlow(duration, 0.05f, null);
        }

        public void TimeSlow(float duration, float slowRatio, Action doneCallback)
        {
            if ((this._timeSlowTimer > 0f) && (this._timeSlowDoneCallback != null))
            {
                this._timeSlowDoneCallback();
                this._timeSlowDoneCallback = null;
            }
            this._timeSlowTimer = duration;
            this._timeSlowDoneCallback = doneCallback;
            Time.timeScale = slowRatio;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSlow_Start", null, null, null);
        }

        public void TriggerSwapLocalAvatar(uint swapOutID, uint swapInID, bool force)
        {
            if ((force || (this._swapTimerState != LevelMinSwapTimerState.Running)) && (this._avatarSwapState != AvatarSwapState.Started))
            {
                if (this.levelMode == Mode.Single)
                {
                    AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(swapOutID);
                    if (actor.AllowOtherSwitchIn)
                    {
                        actor.avatar.TriggerSwitchOut(BaseMonoAvatar.AvatarSwapOutType.Delayed);
                        this.SwapLocalAvatar(swapOutID, swapInID);
                    }
                    else
                    {
                        this._swapOutID = swapOutID;
                        this._swapInID = swapInID;
                        this._avatarSwapState = AvatarSwapState.WaitingForEvent;
                        actor.avatar.TriggerSwitchOut(!force ? BaseMonoAvatar.AvatarSwapOutType.Normal : BaseMonoAvatar.AvatarSwapOutType.Force);
                    }
                }
                else if ((this.levelMode == Mode.Multi) || (this.levelMode == Mode.MultiRemote))
                {
                    this.SwapLocalAvatar(swapOutID, swapInID);
                }
            }
        }

        public void UntargetEntity(BaseMonoEntity target)
        {
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                BaseMonoAvatar avatar = allAvatars[i];
                BaseMonoEntity attackTarget = avatar.AttackTarget;
                if (attackTarget is MonoBodyPartEntity)
                {
                    attackTarget = ((MonoBodyPartEntity) attackTarget).owner;
                }
                if (((avatar != null) && avatar.IsActive()) && (attackTarget == target))
                {
                    avatar.SetAttackTarget(null);
                }
            }
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int j = 0; j < allMonsters.Count; j++)
            {
                BaseMonoMonster monster = allMonsters[j];
                if (((monster != null) && monster.IsActive()) && (monster.AttackTarget == target))
                {
                    monster.SetAttackTarget(null);
                }
            }
        }

        public LevelState levelState { get; private set; }

        public enum AvatarSwapState
        {
            Idle,
            WaitingForEvent,
            Started
        }

        public enum ComboTimerState
        {
            Running,
            Pause
        }

        public enum LevelMinSwapTimerState
        {
            Idle,
            Running
        }

        public enum LevelState
        {
            LevelLoaded,
            LevelStarted,
            LevelRunning,
            LevelEnded,
            LevelTransiting
        }

        public enum Mode
        {
            Single,
            Multi,
            MultiRemote,
            NetworkedMP
        }
    }
}

