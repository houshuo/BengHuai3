namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public static class GlobalDataManager
    {
        private static int _calculatedContentHash;
        private static List<string> _dataListMTNecessary = new List<string>();
        private static List<string> _dataListMTNonNecessary = new List<string>();
        private static string _loadDataDesc = string.Empty;
        private static float _loadDataReaderLastTime = 0f;
        private static float _loadDataReaderTimeDelta = 0f;
        private static float _refreshProgress;
        public const string META_CONFIG_PATH = "Common/MetaConfig";
        public static ConfigMetaConfig metaConfig;

        public static void CalculateAndStoreLoadedDataContentHash()
        {
            _calculatedContentHash = HashDataContent();
        }

        private static bool CheckMoveToNextFrame()
        {
            RefreshProgress += 0.002f;
            float num = (Application.targetFrameRate <= 0) ? 0.03f : (1f / ((float) Application.targetFrameRate));
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            if ((realtimeSinceStartup - _loadDataReaderLastTime) > num)
            {
                _loadDataReaderLastTime = realtimeSinceStartup;
                _loadDataReaderTimeDelta = 0f;
                return true;
            }
            _loadDataReaderLastTime = realtimeSinceStartup;
            _loadDataReaderTimeDelta += realtimeSinceStartup - _loadDataReaderLastTime;
            return false;
        }

        public static int HashDataContent()
        {
            int num = 0;
            num ^= ActMetaDataReader.CalculateContentHash();
            num ^= AvatarAttackPunishMetaDataReader.CalculateContentHash();
            num ^= AvatarCardMetaDataReader.CalculateContentHash();
            num ^= AvatarDefencePunishMetaDataReader.CalculateContentHash();
            num ^= AvatarFragmentMetaDataReader.CalculateContentHash();
            num ^= AvatarLevelMetaDataReader.CalculateContentHash();
            num ^= AvatarMetaDataReader.CalculateContentHash();
            num ^= AvatarSkillMetaDataReader.CalculateContentHash();
            num ^= AvatarStarMetaDataReader.CalculateContentHash();
            num ^= AvatarSubSkillLevelMetaDataReader.CalculateContentHash();
            num ^= AvatarSubSkillMetaDataReader.CalculateContentHash();
            num ^= BattleTypeMetaDataReader.CalculateContentHash();
            num ^= ChapterMetaDataReader.CalculateContentHash();
            num ^= ClassMetaDataReader.CalculateContentHash();
            num ^= EndlessToolMetaDataReader.CalculateContentHash();
            num ^= EquipmentLevelMetaDataReader.CalculateContentHash();
            num ^= EquipmentSetMetaDataReader.CalculateContentHash();
            num ^= EquipSkillMetaDataReader.CalculateContentHash();
            num ^= ItemMetaDataReader.CalculateContentHash();
            num ^= LevelChallengeMetaDataReader.CalculateContentHash();
            num ^= LevelMetaDataReader.CalculateContentHash();
            num ^= LinearMissionDataReader.CalculateContentHash();
            num ^= MissionDataReader.CalculateContentHash();
            num ^= MonsterConfigMetaDataReader.CalculateContentHash();
            num ^= MonsterSkillMetaDataReader.CalculateContentHash();
            num ^= NPCLevelMetaDataReader.CalculateContentHash();
            num ^= PlayerLevelMetaDataReader.CalculateContentHash();
            num ^= ReviveCostTypeMetaDataReader.CalculateContentHash();
            num ^= RewardDataReader.CalculateContentHash();
            num ^= SeriesMetaDataReader.CalculateContentHash();
            num ^= StigmataAffixMetaDataReader.CalculateContentHash();
            num ^= StigmataMetaDataReader.CalculateContentHash();
            num ^= UniqueMonsterMetaDataReader.CalculateContentHash();
            num ^= VentureMetaDataReader.CalculateContentHash();
            num ^= WeaponMetaDataReader.CalculateContentHash();
            num ^= AvatarData.CalculateContentHash();
            num ^= MonsterData.CalculateContentHash();
            num ^= AbilityData.CalculateContentHash();
            return (num ^ PropObjectData.CalculateContentHash());
        }

        private static void InlevelDataMoveOneStep(float step)
        {
            RefreshProgress += step;
        }

        private static void LoadDataReader()
        {
            TextMapMetaDataReader.LoadFromFile();
            LocalDataVersion.LoadFromFile();
            MiscData.LoadFromFile();
            NetworkErrCodeMetaDataReader.LoadFromFile();
            PlayerLevelMetaDataReader.LoadFromFile();
            EquipmentLevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
            WeaponMetaDataReaderExtend.LoadFromFileAndBuildMap();
            EndlessToolMetaDataReader.LoadFromFile();
            StigmataMetaDataReaderExtend.LoadFromFileAndBuildMap();
            StigmataAffixMetaDataReader.LoadFromFile();
            ItemMetaDataReader.LoadFromFile();
            AvatarFragmentMetaDataReader.LoadFromFile();
            AvatarCardMetaDataReader.LoadFromFile();
            EquipSkillMetaDataReader.LoadFromFile();
            EquipmentSetMetaDataReader.LoadFromFile();
            MaterialExpBonusMetaDataReader.LoadFromFile();
            MaterialAvatarExpBonusMetaDataReader.LoadFromFile();
            PowerTypeMetaDataReader.LoadFromFile();
            AvatarAttackPunishMetaDataReader.LoadFromFile();
            AvatarDefencePunishMetaDataReader.LoadFromFile();
            AvatarSkillMetaDataReader.LoadFromFile();
            AvatarSubSkillMetaDataReaderExtend.LoadFromFileAndBuildMap();
            AvatarSubSkillLevelMetaDataReader.LoadFromFile();
            ClassMetaDataReader.LoadFromFile();
            AvatarLevelMetaDataReader.LoadFromFile();
            AvatarStarMetaDataReader.LoadFromFile();
            AvatarMetaDataReaderExtend.LoadFromFileAndBuildMap();
            MissionDataReader.LoadFromFile();
            LinearMissionDataReader.LoadFromFile();
            TutorialDataReader.LoadFromFile();
            TutorialStepDataReader.LoadFromFile();
            ChapterMetaDataReader.LoadFromFile();
            LevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
            LevelChallengeMetaDataReader.LoadFromFile();
            LevelTutorialMetaDataReader.LoadFromFile();
            ActMetaDataReader.LoadFromFile();
            BattleTypeMetaDataReader.LoadFromFile();
            LevelResetCostMetaDataReader.LoadFromFile();
            WeekDayActivityMetaDataReader.LoadFromFile();
            SeriesMetaDataReader.LoadFromFile();
            ReviveCostTypeMetaDataReader.LoadFromFile();
            EndlessDropMetaDataReader.LoadFromFile();
            MonsterUIMetaDataReaderExtend.LoadFromFileAndBuildMap();
            MonsterSkillMetaDataReader.LoadFromFile();
            NPCLevelMetaDataReader.LoadFromFile();
            EvenSignInRewardMetaDataReader.LoadFromFile();
            OddSignInRewardMetaDataReader.LoadFromFile();
            GalTouchData.LoadFromFile();
            RewardDataReader.LoadFromFile();
            UnlockUIDataReaderExtend.LoadFromFileAndBuildMap();
            PlotMetaDataReader.LoadFromFile();
            CgMetaDataReader.LoadFromFile();
            DialogMetaDataReader.LoadFromFile();
            CabinLevelMetaDataReader.LoadFromFile();
            CabinExtendGradeMetaDataReader.LoadFromFile();
            CabinTechTreeMetaDataReader.LoadFromFile();
            CabinLevelUpTimePriceMetaDataReader.LoadFromFile();
            CabinPowerCostMetaDataReader.LoadFromFile();
            CabinDisjointEquipmentMetaDataReader.LoadFromFile();
            CabinVentureLevelMetaDataReader.LoadFromFile();
            VentureMetaDataReader.LoadFromFile();
            CabinVentureRefreshMetaDataReader.LoadFromFile();
            ItempediaData.LoadFromFile();
            CabinCollectLevelMetaDataReader.LoadFromFile();
            MaterialVentureSpeedUpDataReader.LoadFromFile();
            ShopGoodsMetaDataReader.LoadFromFile();
            ShopGoodsPriceRateMetaDataReader.LoadFromFile();
            if (GlobalVars.DISABLE_NETWORK_DEBUG)
            {
                FakePacketHelper.LoadFromFile();
            }
        }

        [DebuggerHidden]
        private static IEnumerator LoadDataReaderAsync()
        {
            return new <LoadDataReaderAsync>c__Iterator18();
        }

        [DebuggerHidden]
        private static IEnumerator LoadDataUseMultiThread()
        {
            return new <LoadDataUseMultiThread>c__Iterator1A();
        }

        private static void LoadInLevelData()
        {
            metaConfig = ConfigUtil.LoadConfig<ConfigMetaConfig>("Common/MetaConfig");
            AvatarData.ReloadFromFile();
            MonsterData.ReloadFromFile();
            AbilityData.ReloadFromFile();
            EffectData.ReloadFromFile();
            StageData.ReloadFromFile();
            AuxObjectData.ReloadFromFile();
            DynamicObjectData.ReloadFromFile();
            WeaponData.ReloadFromFile();
            EquipmentSkillData.ReloadFromFile();
            GalTouchBuffData.ReloadFromFile();
            PropObjectData.ReloadFromFile();
            RenderingData.ReloadFromFile();
            WeatherData.ReloadFromFile();
            AtmosphereSeriesData.ReloadFromFile();
            AnimatorEventData.ReloadFromFile();
            GraphicsSettingData.ReloadFromFile();
            CameraData.ReloadFromFile();
            TouchPatternData.ReloadFromFile();
            FaceAnimationData.ReloadFromFile();
            SharedAnimEventData.ReloadFromData();
            AIData.ReloadFromFile();
            InLevelData.ReloadFromFile();
        }

        [DebuggerHidden]
        private static IEnumerator LoadInLevelDataAsync()
        {
            return new <LoadInLevelDataAsync>c__Iterator19();
        }

        private static void OnLoadOneDataFinish(string dataName)
        {
            if (_dataListMTNecessary.Contains(dataName))
            {
                _dataListMTNecessary.Remove(dataName);
            }
            if (_dataListMTNonNecessary.Contains(dataName))
            {
                _dataListMTNonNecessary.Remove(dataName);
            }
            if ((_dataListMTNecessary.Count == 0) && (_dataListMTNonNecessary.Count == 0))
            {
                IsInRefreshDataAsync = false;
                CalculateAndStoreLoadedDataContentHash();
            }
        }

        public static void Refresh()
        {
            LoadDataReader();
            LoadInLevelData();
            CalculateAndStoreLoadedDataContentHash();
        }

        public static void RefreshAsync(Action refreshNecessaryfinishCallback = null)
        {
            if (!IsInRefreshDataAsync)
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(RefreshAsyncImp(refreshNecessaryfinishCallback));
            }
        }

        [DebuggerHidden]
        private static IEnumerator RefreshAsyncImp(Action refreshNecessaryfinishCallback = null)
        {
            return new <RefreshAsyncImp>c__Iterator17 { refreshNecessaryfinishCallback = refreshNecessaryfinishCallback, <$>refreshNecessaryfinishCallback = refreshNecessaryfinishCallback };
        }

        public static int contentHash
        {
            get
            {
                return _calculatedContentHash;
            }
        }

        public static bool IsInRefreshDataAsync
        {
            [CompilerGenerated]
            get
            {
                return <IsInRefreshDataAsync>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <IsInRefreshDataAsync>k__BackingField = value;
            }
        }

        private static float RefreshProgress
        {
            get
            {
                return _refreshProgress;
            }
            set
            {
                _refreshProgress = value;
                if (_refreshProgress > 0.03f)
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetLoadAssetText, new Tuple<bool, string, bool, float>(true, _loadDataDesc, true, _refreshProgress)));
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadDataReaderAsync>c__Iterator18 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        GlobalDataManager._loadDataReaderLastTime = Time.realtimeSinceStartup;
                        GlobalDataManager._loadDataReaderTimeDelta = 0f;
                        TextMapMetaDataReader.LoadFromFile();
                        GlobalDataManager._loadDataDesc = LocalizationGeneralLogic.GetText("Menu_LoadData", new object[0]);
                        if (!GlobalDataManager.CheckMoveToNextFrame())
                        {
                            break;
                        }
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_0AD4;

                    case 1:
                        break;

                    case 2:
                        goto Label_019E;

                    case 3:
                        goto Label_01C0;

                    case 4:
                        goto Label_01E2;

                    case 5:
                        goto Label_0204;

                    case 6:
                        goto Label_0226;

                    case 7:
                        goto Label_0248;

                    case 8:
                        goto Label_026A;

                    case 9:
                        goto Label_028D;

                    case 10:
                        goto Label_02B0;

                    case 11:
                        goto Label_02D3;

                    case 12:
                        goto Label_0300;

                    case 13:
                        goto Label_0323;

                    case 14:
                        goto Label_0346;

                    case 15:
                        goto Label_0369;

                    case 0x10:
                        goto Label_038C;

                    case 0x11:
                        goto Label_03AF;

                    case 0x12:
                        goto Label_03D2;

                    case 0x13:
                        goto Label_03F5;

                    case 20:
                        goto Label_0418;

                    case 0x15:
                        goto Label_043B;

                    case 0x16:
                        goto Label_045E;

                    case 0x17:
                        goto Label_0481;

                    case 0x18:
                        goto Label_04A4;

                    case 0x19:
                        goto Label_04C7;

                    case 0x1a:
                        goto Label_04EA;

                    case 0x1b:
                        goto Label_050D;

                    case 0x1c:
                        goto Label_0530;

                    case 0x1d:
                        goto Label_0553;

                    case 30:
                        goto Label_0576;

                    case 0x1f:
                        goto Label_0599;

                    case 0x20:
                        goto Label_05BC;

                    case 0x21:
                        goto Label_05DF;

                    case 0x22:
                        goto Label_0602;

                    case 0x23:
                        goto Label_0625;

                    case 0x24:
                        goto Label_0648;

                    case 0x25:
                        goto Label_066B;

                    case 0x26:
                        goto Label_068E;

                    case 0x27:
                        goto Label_06B1;

                    case 40:
                        goto Label_06D4;

                    case 0x29:
                        goto Label_06F7;

                    case 0x2a:
                        goto Label_071A;

                    case 0x2b:
                        goto Label_073D;

                    case 0x2c:
                        goto Label_0760;

                    case 0x2d:
                        goto Label_0783;

                    case 0x2e:
                        goto Label_07A6;

                    case 0x2f:
                        goto Label_07C9;

                    case 0x30:
                        goto Label_07EC;

                    case 0x31:
                        goto Label_080F;

                    case 50:
                        goto Label_0832;

                    case 0x33:
                        goto Label_0855;

                    case 0x34:
                        goto Label_0878;

                    case 0x35:
                        goto Label_089B;

                    case 0x36:
                        goto Label_08BE;

                    case 0x37:
                        goto Label_08E1;

                    case 0x38:
                        goto Label_0904;

                    case 0x39:
                        goto Label_0927;

                    case 0x3a:
                        goto Label_094A;

                    case 0x3b:
                        goto Label_096D;

                    case 60:
                        goto Label_0990;

                    case 0x3d:
                        goto Label_09B3;

                    case 0x3e:
                        goto Label_09D6;

                    case 0x3f:
                        goto Label_09F9;

                    case 0x40:
                        goto Label_0A1C;

                    case 0x41:
                        goto Label_0A3F;

                    case 0x42:
                        goto Label_0A62;

                    case 0x43:
                        goto Label_0A85;

                    case 0x44:
                        goto Label_0AA8;

                    case 0x45:
                        goto Label_0ACB;

                    default:
                        goto Label_0AD2;
                }
                PlayerLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_0AD4;
                }
            Label_019E:
                AvatarSkillMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_0AD4;
                }
            Label_01C0:
                AvatarSubSkillMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 4;
                    goto Label_0AD4;
                }
            Label_01E2:
                AvatarSubSkillLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 5;
                    goto Label_0AD4;
                }
            Label_0204:
                AvatarAttackPunishMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 6;
                    goto Label_0AD4;
                }
            Label_0226:
                AvatarDefencePunishMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 7;
                    goto Label_0AD4;
                }
            Label_0248:
                ClassMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 8;
                    goto Label_0AD4;
                }
            Label_026A:
                AvatarLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 9;
                    goto Label_0AD4;
                }
            Label_028D:
                AvatarStarMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 10;
                    goto Label_0AD4;
                }
            Label_02B0:
                AvatarMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 11;
                    goto Label_0AD4;
                }
            Label_02D3:
                if (GlobalVars.DISABLE_NETWORK_DEBUG)
                {
                    FakePacketHelper.LoadFromFile();
                    if (GlobalDataManager.CheckMoveToNextFrame())
                    {
                        this.$current = null;
                        this.$PC = 12;
                        goto Label_0AD4;
                    }
                }
            Label_0300:
                LocalDataVersion.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 13;
                    goto Label_0AD4;
                }
            Label_0323:
                MiscData.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 14;
                    goto Label_0AD4;
                }
            Label_0346:
                LevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 15;
                    goto Label_0AD4;
                }
            Label_0369:
                NetworkErrCodeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x10;
                    goto Label_0AD4;
                }
            Label_038C:
                EquipmentLevelMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x11;
                    goto Label_0AD4;
                }
            Label_03AF:
                WeaponMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x12;
                    goto Label_0AD4;
                }
            Label_03D2:
                EndlessToolMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x13;
                    goto Label_0AD4;
                }
            Label_03F5:
                StigmataMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 20;
                    goto Label_0AD4;
                }
            Label_0418:
                StigmataAffixMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x15;
                    goto Label_0AD4;
                }
            Label_043B:
                ItemMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x16;
                    goto Label_0AD4;
                }
            Label_045E:
                AvatarFragmentMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x17;
                    goto Label_0AD4;
                }
            Label_0481:
                AvatarCardMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x18;
                    goto Label_0AD4;
                }
            Label_04A4:
                EquipSkillMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x19;
                    goto Label_0AD4;
                }
            Label_04C7:
                EquipmentSetMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x1a;
                    goto Label_0AD4;
                }
            Label_04EA:
                MaterialExpBonusMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x1b;
                    goto Label_0AD4;
                }
            Label_050D:
                MaterialAvatarExpBonusMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x1c;
                    goto Label_0AD4;
                }
            Label_0530:
                PowerTypeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x1d;
                    goto Label_0AD4;
                }
            Label_0553:
                MissionDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 30;
                    goto Label_0AD4;
                }
            Label_0576:
                LinearMissionDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x1f;
                    goto Label_0AD4;
                }
            Label_0599:
                TutorialDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x20;
                    goto Label_0AD4;
                }
            Label_05BC:
                TutorialStepDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x21;
                    goto Label_0AD4;
                }
            Label_05DF:
                ChapterMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x22;
                    goto Label_0AD4;
                }
            Label_0602:
                LevelChallengeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x23;
                    goto Label_0AD4;
                }
            Label_0625:
                LevelTutorialMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x24;
                    goto Label_0AD4;
                }
            Label_0648:
                ActMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x25;
                    goto Label_0AD4;
                }
            Label_066B:
                BattleTypeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x26;
                    goto Label_0AD4;
                }
            Label_068E:
                LevelResetCostMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x27;
                    goto Label_0AD4;
                }
            Label_06B1:
                WeekDayActivityMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 40;
                    goto Label_0AD4;
                }
            Label_06D4:
                SeriesMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x29;
                    goto Label_0AD4;
                }
            Label_06F7:
                ReviveCostTypeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2a;
                    goto Label_0AD4;
                }
            Label_071A:
                EndlessGroupMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2b;
                    goto Label_0AD4;
                }
            Label_073D:
                EndlessDropMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2c;
                    goto Label_0AD4;
                }
            Label_0760:
                MonsterUIMetaDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2d;
                    goto Label_0AD4;
                }
            Label_0783:
                MonsterSkillMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2e;
                    goto Label_0AD4;
                }
            Label_07A6:
                NPCLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x2f;
                    goto Label_0AD4;
                }
            Label_07C9:
                EvenSignInRewardMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x30;
                    goto Label_0AD4;
                }
            Label_07EC:
                OddSignInRewardMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x31;
                    goto Label_0AD4;
                }
            Label_080F:
                RewardDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 50;
                    goto Label_0AD4;
                }
            Label_0832:
                UnlockUIDataReaderExtend.LoadFromFileAndBuildMap();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x33;
                    goto Label_0AD4;
                }
            Label_0855:
                GalTouchData.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x34;
                    goto Label_0AD4;
                }
            Label_0878:
                PlotMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x35;
                    goto Label_0AD4;
                }
            Label_089B:
                CgMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x36;
                    goto Label_0AD4;
                }
            Label_08BE:
                DialogMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x37;
                    goto Label_0AD4;
                }
            Label_08E1:
                CabinLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x38;
                    goto Label_0AD4;
                }
            Label_0904:
                CabinExtendGradeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x39;
                    goto Label_0AD4;
                }
            Label_0927:
                CabinTechTreeMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x3a;
                    goto Label_0AD4;
                }
            Label_094A:
                CabinLevelUpTimePriceMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x3b;
                    goto Label_0AD4;
                }
            Label_096D:
                CabinPowerCostMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 60;
                    goto Label_0AD4;
                }
            Label_0990:
                CabinDisjointEquipmentMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x3d;
                    goto Label_0AD4;
                }
            Label_09B3:
                CabinVentureLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x3e;
                    goto Label_0AD4;
                }
            Label_09D6:
                VentureMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x3f;
                    goto Label_0AD4;
                }
            Label_09F9:
                CabinVentureRefreshMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x40;
                    goto Label_0AD4;
                }
            Label_0A1C:
                ItempediaData.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x41;
                    goto Label_0AD4;
                }
            Label_0A3F:
                CabinCollectLevelMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x42;
                    goto Label_0AD4;
                }
            Label_0A62:
                MaterialVentureSpeedUpDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x43;
                    goto Label_0AD4;
                }
            Label_0A85:
                ShopGoodsMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x44;
                    goto Label_0AD4;
                }
            Label_0AA8:
                ShopGoodsPriceRateMetaDataReader.LoadFromFile();
                if (GlobalDataManager.CheckMoveToNextFrame())
                {
                    this.$current = null;
                    this.$PC = 0x45;
                    goto Label_0AD4;
                }
            Label_0ACB:
                this.$PC = -1;
            Label_0AD2:
                return false;
            Label_0AD4:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadDataUseMultiThread>c__Iterator1A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        GlobalDataManager._dataListMTNecessary.Clear();
                        GlobalDataManager._dataListMTNonNecessary.Clear();
                        GlobalDataManager._dataListMTNecessary.Add("GraphicsSettingData");
                        GlobalDataManager._dataListMTNecessary.Add("WeaponData");
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(GraphicsSettingData.ReloadFromFileAsync(0.01f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 1;
                        goto Label_0314;

                    case 1:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(WeaponData.ReloadFromFileAsync(0.06f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 2;
                        goto Label_0314;

                    case 2:
                        GlobalDataManager._dataListMTNonNecessary.Add("EquipmentSkillData");
                        GlobalDataManager._dataListMTNonNecessary.Add("PropObjectData");
                        GlobalDataManager._dataListMTNonNecessary.Add("SharedAnimEventData");
                        GlobalDataManager._dataListMTNonNecessary.Add("AIData");
                        GlobalDataManager._dataListMTNonNecessary.Add("AvatarData");
                        GlobalDataManager._dataListMTNonNecessary.Add("MonsterData");
                        GlobalDataManager._dataListMTNonNecessary.Add("AbilityData");
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(EquipmentSkillData.ReloadFromFileAsync(0.02f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 3;
                        goto Label_0314;

                    case 3:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(PropObjectData.ReloadFromFileAsync(0.02f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 4;
                        goto Label_0314;

                    case 4:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(SharedAnimEventData.ReloadFromFileAsync(0.02f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 5;
                        goto Label_0314;

                    case 5:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(AIData.ReloadFromFileAsync(0.01f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 6;
                        goto Label_0314;

                    case 6:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(AvatarData.ReloadFromFileAsync(0.06f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 7;
                        goto Label_0314;

                    case 7:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(MonsterData.ReloadFromFileAsync(0.05f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 8;
                        goto Label_0314;

                    case 8:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(AbilityData.ReloadFromFileAsync(0.06f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep), new Action<string>(GlobalDataManager.OnLoadOneDataFinish)));
                        this.$PC = 9;
                        goto Label_0314;

                    case 9:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0314:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadInLevelDataAsync>c__Iterator19 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AsyncAssetRequst <asyncRequest>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<asyncRequest>__0 = ConfigUtil.LoadConfigAsync("Common/MetaConfig", BundleType.RESOURCE_FILE);
                        this.$current = this.<asyncRequest>__0.operation;
                        this.$PC = 1;
                        goto Label_02E5;

                    case 1:
                        GlobalDataManager.metaConfig = (ConfigMetaConfig) this.<asyncRequest>__0.asset;
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(EffectData.ReloadFromFileAsync(0.1f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 2;
                        goto Label_02E5;

                    case 2:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(StageData.ReloadFromFileAsync(0.04f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 3;
                        goto Label_02E5;

                    case 3:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(AuxObjectData.ReloadFromFileAsync(0.04f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 4;
                        goto Label_02E5;

                    case 4:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(DynamicObjectData.ReloadFromFileAsync(0.03f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 5;
                        goto Label_02E5;

                    case 5:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(GalTouchBuffData.ReloadFromFileAsync(0.01f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 6;
                        goto Label_02E5;

                    case 6:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(RenderingData.ReloadFromFileAsync(0.04f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 7;
                        goto Label_02E5;

                    case 7:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(WeatherData.ReloadFromFileAsync(0.08f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 8;
                        goto Label_02E5;

                    case 8:
                        AtmosphereSeriesData.ReloadFromFile();
                        this.$current = null;
                        this.$PC = 9;
                        goto Label_02E5;

                    case 9:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(CameraData.ReloadFromFileAsync(0.02f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 10;
                        goto Label_02E5;

                    case 10:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(AnimatorEventData.ReloadFromFileAsync(0.08f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 11;
                        goto Label_02E5;

                    case 11:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(TouchPatternData.ReloadFromFileAsync(0.03f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 12;
                        goto Label_02E5;

                    case 12:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(FaceAnimationData.ReloadFromFileAsync(0.084f, new Action<float>(GlobalDataManager.InlevelDataMoveOneStep)));
                        this.$PC = 13;
                        goto Label_02E5;

                    case 13:
                        InLevelData.ReloadFromFile();
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_02E5:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <RefreshAsyncImp>c__Iterator17 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>refreshNecessaryfinishCallback;
            internal Action refreshNecessaryfinishCallback;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        GlobalDataManager.IsInRefreshDataAsync = true;
                        GlobalDataManager.RefreshProgress = 0f;
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(GlobalDataManager.LoadDataReaderAsync());
                        this.$PC = 1;
                        goto Label_00F1;

                    case 1:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(GlobalDataManager.LoadInLevelDataAsync());
                        this.$PC = 2;
                        goto Label_00F1;

                    case 2:
                        this.$current = Singleton<ApplicationManager>.Instance.StartCoroutine(GlobalDataManager.LoadDataUseMultiThread());
                        this.$PC = 3;
                        goto Label_00F1;

                    case 3:
                    case 4:
                        if (GlobalDataManager._dataListMTNecessary.Count > 0)
                        {
                            this.$current = null;
                            this.$PC = 4;
                            goto Label_00F1;
                        }
                        GlobalDataManager.RefreshProgress = 1f;
                        if (this.refreshNecessaryfinishCallback != null)
                        {
                            this.refreshNecessaryfinishCallback();
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00F1:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

