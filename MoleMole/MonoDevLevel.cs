namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoDevLevel : MonoTheLevelV1
    {
        [HideInInspector]
        public LevelManager _levelManager;
        [HideInInspector]
        public int avatarCount;
        [HideInInspector]
        public List<DevAvatarData> avatarDevDatas;
        [HideInInspector]
        public List<string> avatarTypeNames;
        [HideInInspector]
        public bool isBenchmark;
        private bool isRotating;
        private const float KeyRotationDelta = 300f;
        [HideInInspector]
        public int LEVEL_DIFFICULTY;
        [HideInInspector]
        public int LEVEL_HARDLEVEL;
        [HideInInspector]
        public LevelActor.Mode LEVEL_MODE;
        [HideInInspector]
        public string LEVEL_PATH;
        [HideInInspector]
        public List<DevMonsterData> monsterDevDatas;
        [HideInInspector]
        public List<int> monsterInstanceIds;
        [HideInInspector]
        public bool pariticleMode;
        [HideInInspector]
        public DevStageData stageDevData;
        [HideInInspector]
        public int TEAM_LEVEL = 1;
        [HideInInspector]
        public bool useFakeHelper;

        public void Awake()
        {
            this.AwakeTryLoadFromFile();
            GlobalVars.DISABLE_NETWORK_DEBUG = true;
            QualitySettings.vSyncCount = 1;
            Singleton<LevelManager>.Create();
            Singleton<EffectManager>.Create();
            Singleton<RuntimeIDManager>.Instance.InitAtAwake();
            Singleton<StageManager>.Instance.InitAtAwake();
            Singleton<AvatarManager>.Instance.InitAtAwake();
            Singleton<CameraManager>.Instance.InitAtAwake();
            Singleton<MonsterManager>.Instance.InitAtAwake();
            Singleton<PropObjectManager>.Instance.InitAtAwake();
            Singleton<DynamicObjectManager>.Instance.InitAtAwake();
            Singleton<EffectManager>.Instance.InitAtAwake();
            Singleton<EventManager>.Instance.InitAtAwake();
            this.HandleBeforeLevelDesignAwake();
            Singleton<LevelDesignManager>.Instance.InitAtAwake();
            Singleton<AuxObjectManager>.Instance.InitAtAwake();
            Singleton<DetourManager>.Instance.InitAtAwake();
            Singleton<ShaderDataManager>.Instance.InitAtAwake();
            Singleton<CinemaDataManager>.Instance.InitAtAwake();
            this._levelManager = Singleton<LevelManager>.Instance;
            this._levelManager.CreateBehaviorManager();
            MonoLevelEntity entity = base.gameObject.AddComponent<MonoLevelEntity>();
            this._levelManager.levelEntity = entity;
            entity.Init(0x21800001);
            LevelActor actor = Singleton<EventManager>.Instance.CreateActor<LevelActor>(entity);
            this._levelManager.levelActor = actor;
            actor.PostInit();
            actor.AddPlugin(new DevLevelActorPlugin(this));
            actor.RemovePlugin<LevelMissionStatisticsPlugin>();
            this.PostAwakeTryLoadFromFile();
        }

        private void AwakeTryLoadFromFile()
        {
            if (DevLevelConfigData.configFromScene)
            {
                this.LEVEL_PATH = DevLevelConfigData.LEVEL_PATH;
                this.LEVEL_MODE = DevLevelConfigData.LEVEL_MODE;
                this.stageDevData = DevLevelConfigData.stageDevData;
                this.avatarDevDatas = DevLevelConfigData.avatarDevDatas;
                this.monsterDevDatas = DevLevelConfigData.monsterDevDatas;
                this.isBenchmark = DevLevelConfigData.isBenchmark;
                GlobalVars.IS_BENCHMARK = this.isBenchmark;
                this.avatarCount = DevLevelConfigData.avatarDevDatas.Count;
                if (this.isBenchmark)
                {
                    this.pariticleMode = true;
                }
                this.avatarTypeNames = new List<string>();
                foreach (DevAvatarData data in this.avatarDevDatas)
                {
                    this.avatarTypeNames.Add(data.avatarType);
                }
            }
            else
            {
                MainUIData.USE_VIEW_CACHING = false;
                GeneralLogicManager.InitAll();
                GlobalDataManager.Refresh();
            }
        }

        private void CheatForPariticleMode(AvatarActor actor)
        {
            if (this.pariticleMode)
            {
                foreach (AvatarActor.SKillInfo info in actor.skillInfoList)
                {
                    info.CD = 0f;
                }
                actor.baseMaxHP = actor.HP = actor.maxHP = 999999f;
                actor.baseMaxSP = actor.SP = actor.maxSP = 999999f;
                actor.ChangeSwitchInCDTime(0.1f);
            }
        }

        private void CreateFakeFriendAvatar()
        {
            bool leaderSkillOn = false;
            AvatarDataItem leaderAvatar = Singleton<LevelScoreManager>.Instance.friendDetailItem.leaderAvatar;
            Singleton<AvatarManager>.Instance.CreateAvatar(leaderAvatar, false, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(3), false, leaderSkillOn, true, false);
        }

        private void HandleAlreadyLoadedCameras()
        {
            RuntimeIDManager instance = Singleton<RuntimeIDManager>.Instance;
            MonoMainCamera camera = UnityEngine.Object.FindObjectOfType<MonoMainCamera>();
            uint nextRuntimeID = instance.GetNextRuntimeID(2);
            camera.Init(nextRuntimeID);
            Singleton<CameraManager>.Instance.RegisterCameraData(1, camera, nextRuntimeID);
            MonoInLevelUICamera camera2 = UnityEngine.Object.FindObjectOfType<MonoInLevelUICamera>();
            nextRuntimeID = instance.GetNextRuntimeID(2);
            camera2.Init(nextRuntimeID);
            Singleton<CameraManager>.Instance.RegisterCameraData(2, camera2, nextRuntimeID);
        }

        private void HandleAlreadyLoadedPrefabs()
        {
            RuntimeIDManager instance = Singleton<RuntimeIDManager>.Instance;
            StageEntry stageEntryByName = StageData.GetStageEntryByName(this.stageDevData.stageName);
            MonoBasePerpStage perpStage = UnityEngine.Object.FindObjectOfType<MonoBasePerpStage>();
            if (perpStage != null)
            {
                perpStage.Init(stageEntryByName, (string) null);
            }
            MonoStageEnv stageEnv = UnityEngine.Object.FindObjectOfType<MonoStageEnv>();
            Singleton<StageManager>.Instance.RegisterStage(stageEntryByName, perpStage, stageEnv);
            LevelActor actor = (LevelActor) Singleton<EventManager>.Instance.GetActor(0x21800001);
            actor.levelMode = this.LEVEL_MODE;
            BaseMonoAvatar avatar = UnityEngine.Object.FindObjectOfType<BaseMonoAvatar>();
            if (avatar != null)
            {
                DevAvatarData devAvatarData = this.avatarDevDatas[0];
                AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(this.avatarTypeNames[0], devAvatarData.avatarLevel, devAvatarData.avatarStar);
                this.SetUpAvatarDataItem(avatarDataItem, devAvatarData);
                uint nextRuntimeID = instance.GetNextRuntimeID(3);
                avatar.Init(true, nextRuntimeID, avatarDataItem.AvatarRegistryKey, avatarDataItem.GetWeapon().ID, avatar.transform.position, avatar.transform.forward, true);
                this.LoadAvatarWwiseSoundBank(avatar);
                AvatarActor actor2 = Singleton<EventManager>.Instance.CreateActor<AvatarActor>(avatar);
                actor2.InitAvatarDataItem(avatarDataItem, true, false, true, true);
                actor2.PostInit();
                Singleton<AvatarManager>.Instance.RegisterAvatar(avatar, true, true, false);
                this.SetUpAvatarSkill(actor2, devAvatarData);
                this.CheatForPariticleMode(actor2);
                for (int i = 1; i < this.avatarCount; i++)
                {
                    DevAvatarData data2 = this.avatarDevDatas[i];
                    AvatarDataItem item2 = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(this.avatarTypeNames[i], data2.avatarLevel, data2.avatarStar);
                    this.SetUpAvatarDataItem(item2, data2);
                    uint runtimeID = instance.GetNextRuntimeID(3);
                    Singleton<AvatarManager>.Instance.CreateAvatar(item2, false, avatar.XZPosition, avatar.FaceDirection, runtimeID, false, false, false, false);
                    AvatarActor actor3 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
                    this.SetUpAvatarSkill(actor3, data2);
                    this.CheatForPariticleMode(actor3);
                }
                if (this.useFakeHelper)
                {
                    this.CreateFakeFriendAvatar();
                }
                Singleton<CinemaDataManager>.Instance.Preload(avatar);
            }
            foreach (BaseMonoMonster monster in UnityEngine.Object.FindObjectsOfType<BaseMonoMonster>())
            {
                DevMonsterData data3 = null;
                int instanceID = monster.gameObject.GetInstanceID();
                for (int j = 0; j < this.monsterInstanceIds.Count; j++)
                {
                    if (this.monsterInstanceIds[j] == instanceID)
                    {
                        data3 = this.monsterDevDatas[j];
                        break;
                    }
                }
                if (data3 == null)
                {
                    UnityEngine.Object.Destroy(monster.gameObject);
                }
                else
                {
                    string monsterName;
                    string typeName;
                    uint num7 = instance.GetNextRuntimeID(4);
                    if (data3.uniqueMonsterID != 0)
                    {
                        UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(data3.uniqueMonsterID);
                        monsterName = uniqueMonsterMetaData.monsterName;
                        typeName = uniqueMonsterMetaData.typeName;
                    }
                    else
                    {
                        monsterName = data3.monsterName;
                        typeName = data3.typeName;
                    }
                    bool isElite = data3.isElite;
                    monster.Init(monsterName, typeName, num7, monster.transform.position, data3.uniqueMonsterID, null, true, isElite, true, 0);
                    for (int k = 0; k < monster.config.CommonArguments.RequestSoundBankNames.Length; k++)
                    {
                        Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(monster.config.CommonArguments.RequestSoundBankNames[k]);
                    }
                    MonsterActor actor4 = Singleton<EventManager>.Instance.CreateActor<MonsterActor>(monster);
                    actor4.InitLevelData(data3.level, data3.isElite);
                    actor4.PostInit();
                    Singleton<MonsterManager>.Instance.RegisterMonster(monster);
                    if (data3.abilities.Length > 0)
                    {
                        for (int m = 0; m < data3.abilities.Length; m++)
                        {
                            string str3 = data3.abilities[m];
                            if (!string.IsNullOrEmpty(str3))
                            {
                                actor4.abilityPlugin.InsertPreInitAbility(AbilityData.GetAbilityConfig(data3.abilities[m]));
                            }
                        }
                    }
                    if (data3.isStationary)
                    {
                        MonsterActor actor5 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monster.GetRuntimeID());
                        actor5.baseMaxHP = actor5.HP = actor5.maxHP = 999999f;
                        monster.GetActiveAIController().SetActive(false);
                    }
                }
            }
            Singleton<LevelManager>.Instance.levelActor.SuddenLevelStart();
            EvtStageReady evt = new EvtStageReady {
                isBorn = true
            };
            Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            Singleton<DetourManager>.Instance.LoadNavMeshRelatedLevel(this.stageDevData.stageName);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, null);
        }

        private void HandleBeforeLevelDesignAwake()
        {
            Singleton<LevelScoreManager>.Create();
            FriendDetailDataItem friend = null;
            if (true)
            {
                DevAvatarData devAvatarData = this.avatarDevDatas[0];
                AvatarDataItem avatarDataItem = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(this.avatarTypeNames[0], devAvatarData.avatarLevel, devAvatarData.avatarStar);
                this.SetUpAvatarDataItem(avatarDataItem, devAvatarData);
                friend = new FriendDetailDataItem(0, "FakeHelper", 1, avatarDataItem, null);
            }
            Singleton<LevelScoreManager>.Instance.SetDevLevelBeginIntent(this.LEVEL_PATH, this.LEVEL_MODE, this.LEVEL_HARDLEVEL, this.LEVEL_DIFFICULTY + 1, friend);
            Singleton<PlayerModule>.Instance.playerData.teamLevel = this.TEAM_LEVEL;
        }

        private void LoadAvatarWwiseSoundBank(BaseMonoAvatar avatar)
        {
            int index = 0;
            int length = avatar.config.CommonArguments.RequestSoundBankNames.Length;
            while (index < length)
            {
                Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(avatar.config.CommonArguments.RequestSoundBankNames[index]);
                index++;
            }
        }

        public void OnDestroy()
        {
            Singleton<LevelManager>.Instance.Destroy();
            Singleton<LevelManager>.Destroy();
        }

        private void PostAwakeTryLoadFromFile()
        {
            if (DevLevelConfigData.configFromScene)
            {
                this.TryDestroyTypeAll<BaseMonoAvatar>();
                this.TryDestroyTypeAll<BaseMonoMonster>();
                this.TryDestroyTypeAll<MonoBasePerpStage>();
                this.TryDestroyTypeAll<MonoStageEnv>();
                Resources.UnloadUnusedAssets();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                StageEntry stageEntryByName = StageData.GetStageEntryByName(this.stageDevData.stageName);
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(stageEntryByName.GetPerpStagePrefabPath(), BundleType.RESOURCE_FILE));
                obj2.transform.position = Vector3.zero;
                Transform transform = obj2.transform;
                transform.position -= obj2.transform.Find(stageEntryByName.LocationPointName).position;
                StageManager.SetPerpstageNodeVisibility(obj2.GetComponent<MonoBasePerpStage>(), stageEntryByName, false, false);
                StageManager.SetPerpstageNodeVisibility(obj2.GetComponent<MonoBasePerpStage>(), stageEntryByName, true, true);
                UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(stageEntryByName.GetEnvPrefabPath(), BundleType.RESOURCE_FILE));
                DevAvatarData data = this.avatarDevDatas[0];
                UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(AvatarData.GetPrefabResPath(data.avatarType, false), BundleType.RESOURCE_FILE));
                this.monsterInstanceIds = new List<int>();
                foreach (DevMonsterData data2 in this.monsterDevDatas)
                {
                    this.monsterInstanceIds.Add(UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(data2.monsterName, data2.typeName, false), BundleType.RESOURCE_FILE)).GetInstanceID());
                }
            }
        }

        private void PostStartHandleBenchmark()
        {
            if (this.isBenchmark)
            {
                Singleton<AvatarManager>.Instance.SetAutoBattle(true);
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                for (int i = 0; i < allPlayerAvatars.Count; i++)
                {
                    if (!string.IsNullOrEmpty(this.avatarDevDatas[i].avatarAI))
                    {
                        ExternalBehaviorTree tree = Miscs.LoadResource<ExternalBehaviorTree>(this.avatarDevDatas[i].avatarAI, BundleType.RESOURCE_FILE);
                        ((BTreeAvatarAIController) allPlayerAvatars[i].GetActiveAIController()).autoBattleBehavior = tree;
                        ((BTreeAvatarAIController) allPlayerAvatars[i].GetActiveAIController()).autoMoveBehvior = tree;
                        ((BTreeAvatarAIController) allPlayerAvatars[i].GetActiveAIController()).supporterBehavior = tree;
                        allPlayerAvatars[i].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().ExternalBehavior = tree;
                        allPlayerAvatars[i].GetComponent<BehaviorDesigner.Runtime.BehaviorTree>().EnableBehavior();
                    }
                }
                Screen.sleepTimeout = -1;
                SuperDebug.CloseAllDebugs();
                new GameObject { name = "__Benchmark" }.AddComponent<MonoBenchmarkSwitches>();
            }
        }

        private void PreStartHandleBenchmark()
        {
            if (this.isBenchmark)
            {
                foreach (DevAvatarData data in this.avatarDevDatas)
                {
                    if (!Miscs.ArrayContains<string>(data.avatarTestSkills, "Test_UnlockAllAniSkill"))
                    {
                        Miscs.ArrayAppend<string>(ref data.avatarTestSkills, "Test_UnlockAllAniSkill");
                    }
                    if (!Miscs.ArrayContains<string>(data.avatarTestSkills, "Test_Undamagable"))
                    {
                        Miscs.ArrayAppend<string>(ref data.avatarTestSkills, "Test_Undamagable");
                    }
                }
                foreach (DevMonsterData data2 in this.monsterDevDatas)
                {
                    if (!Miscs.ArrayContains<string>(data2.abilities, "Test_Undamagable"))
                    {
                        Miscs.ArrayAppend<string>(ref data2.abilities, "Test_Undamagable");
                    }
                }
            }
        }

        public void ReplaceInstanceID(int oldID, int newID)
        {
            if (this.monsterInstanceIds.Contains(oldID))
            {
                for (int i = 0; i < this.monsterInstanceIds.Count; i++)
                {
                    if (this.monsterInstanceIds[i] == oldID)
                    {
                        this.monsterInstanceIds[i] = newID;
                    }
                }
            }
        }

        private void SetUpAvatarDataItem(AvatarDataItem avatarDataItem, DevAvatarData devAvatarData)
        {
            avatarDataItem.equipsMap[1] = Singleton<StorageModule>.Instance.GetDummyWeaponDataItem(devAvatarData.avatarWeapon, devAvatarData.avatarWeaponLevel);
            if (devAvatarData.avatarStigmata[0] != -1)
            {
                avatarDataItem.equipsMap[2] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[0], devAvatarData.avatarStigmataLevels[0]);
            }
            else
            {
                avatarDataItem.equipsMap[2] = null;
            }
            if (devAvatarData.avatarStigmata[1] != -1)
            {
                avatarDataItem.equipsMap[3] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[1], devAvatarData.avatarStigmataLevels[1]);
            }
            else
            {
                avatarDataItem.equipsMap[3] = null;
            }
            if (devAvatarData.avatarStigmata[2] != -1)
            {
                avatarDataItem.equipsMap[4] = Singleton<StorageModule>.Instance.GetDummyStigmataDataItem(devAvatarData.avatarStigmata[2], devAvatarData.avatarStigmataLevels[2]);
            }
            else
            {
                avatarDataItem.equipsMap[4] = null;
            }
        }

        private void SetUpAvatarSkill(AvatarActor actor, DevAvatarData devAvatarData)
        {
            if (devAvatarData.avatarTestSkills.Length > 0)
            {
                for (int i = 0; i < devAvatarData.avatarTestSkills.Length; i++)
                {
                    string str = devAvatarData.avatarTestSkills[i];
                    if (!string.IsNullOrEmpty(str))
                    {
                        actor.abilityPlugin.InsertPreInitAbility(AbilityData.GetAbilityConfig(devAvatarData.avatarTestSkills[i]));
                    }
                }
            }
        }

        public void Start()
        {
            UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/UIToolkits/FPSIndicator", BundleType.RESOURCE_FILE));
            this.PreStartHandleBenchmark();
            Singleton<StageManager>.Instance.InitAtStart();
            Singleton<AvatarManager>.Instance.InitAtStart();
            this.HandleAlreadyLoadedCameras();
            Singleton<MonsterManager>.Instance.InitAtStart();
            Singleton<PropObjectManager>.Instance.InitAtStart();
            Singleton<DynamicObjectManager>.Instance.InitAtStart();
            Singleton<EffectManager>.Instance.InitAtStart();
            Singleton<EventManager>.Instance.InitAtStart();
            Singleton<AuxObjectManager>.Instance.InitAtStart();
            Singleton<DetourManager>.Instance.InitAtStart();
            this.HandleAlreadyLoadedPrefabs();
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            if (avatar != null)
            {
                Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(avatar.transform, new Vector3(0f, 2f, 0f));
            }
            string[] soundBankNames = new string[] { "All_In_One_Bank", "BK_Global", "BK_Events", "Test_3D_Att" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
            PostFXBase base2 = UnityEngine.Object.FindObjectOfType<PostFXBase>();
            if (base2 != null)
            {
                base2.originalEnabled = true;
            }
            this.PostStartHandleBenchmark();
        }

        private void TryDestroyType<T>() where T: Component
        {
            foreach (T local in UnityEngine.Object.FindObjectsOfType<T>())
            {
                if (local != null)
                {
                    UnityEngine.Object.DestroyImmediate(local.gameObject);
                }
            }
        }

        private void TryDestroyTypeAll<T>() where T: Component
        {
            T[] localArray = UnityEngine.Object.FindObjectsOfType<T>();
            for (int i = 0; i < localArray.Length; i++)
            {
                UnityEngine.Object.DestroyImmediate(localArray[i].gameObject);
            }
        }

        public void Update()
        {
            Singleton<StageManager>.Instance.Core();
            Singleton<AvatarManager>.Instance.Core();
            Singleton<CameraManager>.Instance.Core();
            Singleton<MonsterManager>.Instance.Core();
            Singleton<PropObjectManager>.Instance.Core();
            Singleton<DynamicObjectManager>.Instance.Core();
            Singleton<EffectManager>.Instance.Core();
            Singleton<EventManager>.Instance.Core();
            Singleton<LevelDesignManager>.Instance.Core();
            Singleton<AuxObjectManager>.Instance.Core();
            Singleton<DetourManager>.Instance.Core();
            if (Singleton<WwiseAudioManager>.Instance != null)
            {
                Singleton<WwiseAudioManager>.Instance.Core();
            }
        }

        private void UpdateForKeyboradInput()
        {
            Vector2 zero = Vector2.zero;
            if (Input.GetKeyUp(KeyCode.Semicolon))
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                Singleton<AvatarManager>.Instance.GetLocalAvatar().SetTrigger("TriggerHit");
            }
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                localAvatar.SetTrigger("TriggerHit");
                localAvatar.SetTrigger("TriggerKnockDown");
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Singleton<AvatarManager>.Instance.GetLocalAvatar().SetDied(KillEffect.KillNow);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                if (allMonsters.Count > 0)
                {
                    Singleton<CameraManager>.Instance.EnableBossCamera(allMonsters[0].GetRuntimeID());
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Singleton<CameraManager>.Instance.DisableBossCamera();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Singleton<CameraManager>.Instance.controlledRotateKeepManual = !Singleton<CameraManager>.Instance.controlledRotateKeepManual;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Singleton<CameraManager>.Instance.EnableCrowdCamera();
            }
            if (Input.GetKey(KeyCode.Keypad4))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, 0f);
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, 0f);
            }
            if (Input.GetKey(KeyCode.Keypad8))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2(0f, (300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad2))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2(0f, (-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad7))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, (300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad9))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, (300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad1))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, (-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Keypad3))
            {
                if (!this.isRotating)
                {
                    this.isRotating = true;
                    Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStart();
                }
                zero += new Vector2((300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime, (-300f * Singleton<LevelManager>.Instance.levelEntity.TimeScale) * Time.deltaTime);
            }
            if (((Input.GetKeyUp(KeyCode.Keypad4) || Input.GetKeyUp(KeyCode.Keypad6)) || (Input.GetKeyUp(KeyCode.Keypad8) || Input.GetKeyUp(KeyCode.Keypad5))) || ((Input.GetKeyUp(KeyCode.Keypad7) || Input.GetKeyUp(KeyCode.Keypad9)) || ((Input.GetKeyUp(KeyCode.Keypad1) || Input.GetKeyUp(KeyCode.Keypad2)) || Input.GetKeyUp(KeyCode.Keypad3))))
            {
                this.isRotating = false;
                Singleton<CameraManager>.Instance.GetMainCamera().FollowControlledRotateStop();
            }
            if (this.isRotating)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetFollowControledRotationData(zero);
            }
            if (Input.GetKey(KeyCode.KeypadPlus) && !Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(0x4e21, true, false, true, true, false);
            }
            if (Input.GetKey(KeyCode.KeypadMinus) && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().storyState.QuitStoryStateWithFade(0.5f, true, true);
            }
            if (Input.GetKey(KeyCode.KeypadMultiply) && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().storyState.QuitStoryStateWithLerp(false, 1f, true);
            }
            if (Input.GetKey(KeyCode.X))
            {
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(3f, false, null, null);
            }
            if (Input.GetKey(KeyCode.C))
            {
                Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(3f, false, null, null);
            }
        }
    }
}

