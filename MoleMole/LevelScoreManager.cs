namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LevelScoreManager
    {
        private LevelDataItem _level;
        [CompilerGenerated]
        private static Predicate<WeekDayActivityDataItem> <>f__am$cache26;
        public List<string> appliedToolList;
        public int avaiableReviveNum;
        public float avatarExpInside;
        public bool collectAntiCheatData;
        public float configAvatarExpInside;
        public List<int> configChallengeIds;
        public List<DropItem> configLevelDrops;
        public float configScoinInside;
        public int difficulty;
        public List<DropItem> dropItemList;
        public List<DropItem> dropItemListToShow;
        public List<DropItem> endlessConfigLevelDrops;
        public StageEndStatus endStatus;
        public List<int> finishedChallengeIndexList;
        public FriendDetailDataItem friendDetailItem;
        public bool hasNuclearActivityBefore;
        public bool isDebugDynamicLevel;
        public bool isDebugLevel;
        public bool isLevelEnd;
        public bool isLevelSuccess;
        public bool isTryLevel;
        public LevelActor.Mode levelMode;
        public float levelTimer;
        public StageType LevelType = 1;
        public string luaFile;
        public SafeInt32 maxComboNum = 0;
        public int maxReviveNum;
        public List<AvatarDataItem> memberList;
        public int NPCHardLevel;
        public int playerExpBefore;
        public int playerLevelBefore;
        public int progress;
        public float scoinInside;
        public string signKey;
        public StageEndRsp stageEndRsp;
        public List<int> trackChallengeIds;
        public bool useDebugFunction;

        public void AddDropItem(int metaId, int level, int num)
        {
            if (this.CheckDropItem(metaId, level, num))
            {
                DropItem item = new DropItem();
                item.set_item_id((uint) metaId);
                item.set_level((uint) level);
                item.set_num((uint) num);
                this.dropItemList.Add(item);
            }
        }

        public void AddDropItemToShow(int metaId, int level, int num)
        {
            if (this.CheckDropItem(metaId, level, num))
            {
                DropItem item = new DropItem();
                item.set_item_id((uint) metaId);
                item.set_level((uint) level);
                item.set_num((uint) num);
                this.dropItemListToShow.Add(item);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DropItemConutChanged, this.dropItemListToShow.Count));
            }
        }

        private bool CheckDropItem(int metaId, int level, int num)
        {
            <CheckDropItem>c__AnonStoreyD0 yd = new <CheckDropItem>c__AnonStoreyD0 {
                metaId = metaId
            };
            if (this.LevelType == 4)
            {
                if ((this.configLevelDrops.Find(new Predicate<DropItem>(yd.<>m__DA)) == null) && (this.endlessConfigLevelDrops.Find(new Predicate<DropItem>(yd.<>m__DB)) == null))
                {
                    return false;
                }
            }
            else if (this.configLevelDrops.Find(new Predicate<DropItem>(yd.<>m__DC)) == null)
            {
                return false;
            }
            return true;
        }

        private LevelDataItem CreateDummyLevelItem(string luaFile)
        {
            return new LevelDataItem(0, new LevelMetaData(0, "DEV_LEVEL", 0, 0, 0, 1, 1, 0, 1, 10, 0, 0, 0, 0, 0, 0f, 0, 0f, 0, 0, 0, new List<int>(), string.Empty, string.Empty, string.Empty, 1, 1, 1, new List<int>(), "DEV_LEVEL", "DEV_LEVEL", string.Empty, string.Empty, "Lua/Levels/Common/" + luaFile, new List<LevelMetaData.LevelChallengeMetaNode>(), 0, 0, 1, 0x2710, 1, 10, new List<string>(), 100));
        }

        private LevelDataItem CreateEndlessLevelItem()
        {
            return new LevelDataItem(0xdbc05, new LevelMetaData(0, "ENDLESS_LEVEL", 0, 0, 0, 1, 1, 0, 1, 10, 0, 0, 0, 0, 0, 0f, 0, 0f, 0, 0, 0, new List<int>(), string.Empty, string.Empty, string.Empty, 1, 1, 1, new List<int>(), "ENDLESS_LEVEL", "ENDLESS_LEVEL", string.Empty, string.Empty, MiscData.Config.BasicConfig.EndlessLevelLuaFilePath, new List<LevelMetaData.LevelChallengeMetaNode>(), 0, 0, 1, 0x2710, 1, 10, new List<string>(), 100));
        }

        private List<DropItem> GetDropList(List<DropItem> list)
        {
            List<DropItem> list2 = new List<DropItem>();
            Dictionary<int, DropItem> dictionary = new Dictionary<int, DropItem>();
            foreach (DropItem item in list)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), 1);
                dummyStorageDataItem.number = (int) item.get_num();
                if (((dummyStorageDataItem is WeaponDataItem) || (dummyStorageDataItem is StigmataDataItem)) || (dummyStorageDataItem is AvatarCardDataItem))
                {
                    list2.Add(item);
                }
                else if (dictionary.ContainsKey(dummyStorageDataItem.ID))
                {
                    DropItem local1 = dictionary[dummyStorageDataItem.ID];
                    local1.set_num(local1.get_num() + ((uint) dummyStorageDataItem.number));
                }
                else
                {
                    DropItem item2 = new DropItem();
                    item2.set_item_id(item.get_item_id());
                    item2.set_level(item.get_level());
                    item2.set_num(item.get_num());
                    item2.set_rarity(item.get_rarity());
                    dictionary[dummyStorageDataItem.ID] = item2;
                }
            }
            foreach (KeyValuePair<int, DropItem> pair in dictionary)
            {
                list2.Add(pair.Value);
            }
            list2.Sort(new Comparison<DropItem>(this.SortDropItem));
            return list2;
        }

        public List<DropItem> GetDropListToShow()
        {
            return this.GetDropList(this.dropItemListToShow);
        }

        private int GetItemTypeIndex(StorageDataItemBase itemData)
        {
            if (itemData is WeaponDataItem)
            {
                return 1;
            }
            if (itemData is StigmataDataItem)
            {
                return 2;
            }
            if (itemData is MaterialDataItem)
            {
                switch (itemData.GetBaseType())
                {
                    case 1:
                        return 3;

                    case 2:
                        return 4;
                }
                return 20;
            }
            if (itemData is AvatarFragmentDataItem)
            {
                return 0x15;
            }
            return 100;
        }

        public int GetReviveCost()
        {
            int reviveTime = (this.maxReviveNum - this.avaiableReviveNum) + 1;
            return this._level.GetReviveCost(reviveTime);
        }

        public List<DropItem> GetTotalDropList()
        {
            return this.GetDropList(this.dropItemList);
        }

        public void HandleLevelEnd(EvtLevelState.LevelEndReason endReason)
        {
            this.isLevelEnd = true;
            if (endReason == EvtLevelState.LevelEndReason.EndWin)
            {
                this.endStatus = 1;
            }
            else if (endReason == EvtLevelState.LevelEndReason.EndLoseAllDead)
            {
                this.endStatus = 3;
            }
            else if (endReason == EvtLevelState.LevelEndReason.EndLoseNotMeetCondition)
            {
                this.endStatus = 2;
            }
            else if (endReason == EvtLevelState.LevelEndReason.EndLoseQuit)
            {
                this.endStatus = 4;
            }
            else
            {
                this.endStatus = 4;
            }
            if (endReason == EvtLevelState.LevelEndReason.EndWin)
            {
                LevelChallengeHelperPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelChallengeHelperPlugin>();
                if (plugin != null)
                {
                    HashSet<int> set = new HashSet<int>();
                    for (int i = 0; i < plugin.challengeList.Count; i++)
                    {
                        BaseLevelChallenge challenge = plugin.challengeList[i];
                        if (challenge.IsFinished())
                        {
                            set.Add(challenge.challengeId);
                        }
                    }
                    int count = this.configChallengeIds.Count;
                    for (int j = 0; j < count; j++)
                    {
                        if (set.Contains(this.configChallengeIds[j]))
                        {
                            this.finishedChallengeIndexList.Add(j);
                        }
                    }
                }
                if (this.LevelType == 4)
                {
                    foreach (DropItem item in this.endlessConfigLevelDrops)
                    {
                        this.AddDropItem((int) item.get_item_id(), (int) item.get_level(), (int) item.get_num());
                    }
                }
            }
            else
            {
                this.dropItemList.Clear();
                this.dropItemListToShow.Clear();
            }
        }

        public bool HasNuclearActivity(int levelID)
        {
            WeekDayActivityDataItem item = Singleton<LevelModule>.Instance.TryGetWeekDayActivityByLevelID(levelID);
            if (item == null)
            {
                return false;
            }
            SeriesDataItem weekDaySeriesByActivityID = Singleton<LevelModule>.Instance.GetWeekDaySeriesByActivityID(item.GetActivityID());
            if (weekDaySeriesByActivityID == null)
            {
                return false;
            }
            if (<>f__am$cache26 == null)
            {
                <>f__am$cache26 = x => (x.GetActivityType() == 3) && (x.GetStatus() == ActivityDataItemBase.Status.InProgress);
            }
            return weekDaySeriesByActivityID.weekActivityList.Exists(<>f__am$cache26);
        }

        private void InitLevelScore()
        {
            this.isLevelEnd = false;
            this.finishedChallengeIndexList = new List<int>();
            this.dropItemList = new List<DropItem>();
            this.dropItemListToShow = new List<DropItem>();
            this.avatarExpInside = 0f;
            this.scoinInside = 0f;
            this.maxComboNum = 0;
        }

        public bool IsAllowLevelPunish()
        {
            bool flag = Singleton<PlayerModule>.Instance.playerData.teamLevel >= MiscData.Config.BasicConfig.MinPlayerPunishLevel;
            return ((this.LevelType != 4) && flag);
        }

        public bool IsLevelDone()
        {
            return ((this._level == null) || (this._level.status == 3));
        }

        public bool RequestLevelEnd()
        {
            bool hashChanged = false;
            if (MiscData.Config.EnableHashCheck)
            {
                hashChanged = GlobalDataManager.HashDataContent() != GlobalDataManager.contentHash;
            }
            if (this.LevelType == 4)
            {
                bool flag2 = false;
                List<EndlessAvatarHp> avatarHPList = new List<EndlessAvatarHp>();
                List<int> newMemberList = new List<int>();
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
                {
                    AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID());
                    EndlessAvatarHp endlessAvatarHPData = Singleton<EndlessModule>.Instance.GetEndlessAvatarHPData(actor.avatarDataItem.avatarID);
                    int num2 = Mathf.Clamp(!avatar.IsAlive() ? 0 : Mathf.FloorToInt((actor.HP / actor.maxHP) * 100f), 0, 100);
                    int num3 = Mathf.Clamp(!avatar.IsAlive() ? 0 : Mathf.FloorToInt((actor.SP / actor.maxSP) * 100f), 0, 100);
                    if (this.endStatus != 1)
                    {
                        num2 = Mathf.Clamp(num2 - 50, 0, 100);
                    }
                    endlessAvatarHPData.set_hp_percent((uint) num2);
                    endlessAvatarHPData.set_sp_percent((uint) num3);
                    avatarHPList.Add(endlessAvatarHPData);
                    Singleton<EndlessModule>.Instance.SetAvatarHP((int) endlessAvatarHPData.get_hp_percent(), (int) endlessAvatarHPData.get_avatar_id());
                    if (endlessAvatarHPData.get_hp_percent() > 0)
                    {
                        newMemberList.Add(actor.avatarDataItem.avatarID);
                    }
                    else
                    {
                        flag2 = true;
                    }
                }
                Singleton<PlayerModule>.Instance.playerData.SetTeamMember(4, newMemberList);
                if (flag2)
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AvatarDie, null));
                }
                Singleton<NetworkManager>.Instance.RequestEndlessFloorEndReq(this.endStatus, this.dropItemList, avatarHPList, hashChanged);
            }
            else
            {
                PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
                this.playerLevelBefore = playerData.teamLevel;
                this.playerExpBefore = playerData.teamExp;
                Singleton<LevelManager>.Instance.levelActor.ControlLevelDamageStastics(DamageStastcisControlType.DamageStasticsResult);
                List<StageCheatData> cheatDataList = null;
                if (this.collectAntiCheatData)
                {
                    LevelAntiCheatPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAntiCheatPlugin>();
                    if (plugin != null)
                    {
                        plugin.CollectAntiCheatData();
                        cheatDataList = plugin.cheatDataList;
                    }
                }
                Singleton<NetworkManager>.Instance.RequestLevelEndReq(this._level.levelId, this.endStatus, Mathf.FloorToInt(this._level.ScoinFixed + this.scoinInside), Mathf.FloorToInt(this._level.AvatarExpFixed + this.avatarExpInside), this.finishedChallengeIndexList, this.dropItemList, cheatDataList, hashChanged, this.signKey);
            }
            if (MiscData.Config.EnableHashCheck && hashChanged)
            {
                Singleton<ApplicationManager>.Instance.AntiCheatQuit("Menu_Title_HashCheatQuit", "Menu_Desc_HashCheatQuit");
                return false;
            }
            return true;
        }

        public void SetDebugLevelBeginIntent(string luaName)
        {
            if (string.IsNullOrEmpty(luaName))
            {
                luaName = "Level Analyse Auto.lua";
            }
            this.isDebugLevel = true;
            this.NPCHardLevel = 1;
            this.luaFile = "Lua/Levels/Common/" + luaName;
            this.difficulty = 1;
            this.levelMode = LevelActor.Mode.Single;
            this.configLevelDrops = new List<DropItem>();
            this.configChallengeIds = new List<int>();
            this.configAvatarExpInside = 0f;
            this.configScoinInside = 0f;
            this.maxReviveNum = 3;
            this.avaiableReviveNum = this.maxReviveNum;
            this.memberList = new List<AvatarDataItem>();
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(1);
            for (int i = 0; i < memberList.Count; i++)
            {
                AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i]).Clone();
                this.memberList.Add(item);
            }
            this.InitLevelScore();
            this._level = this.CreateDummyLevelItem(luaName);
        }

        public void SetDefendModeLevelBeginIntent(int progress, int difficulty)
        {
            this.luaFile = MiscData.Config.BasicConfig.DefendModeLevelLuaFilePath;
            this.levelMode = LevelActor.Mode.Single;
            this.difficulty = difficulty;
            this.progress = progress;
            this.LevelType = 2;
        }

        public void SetDevLevelBeginIntent(string levelPath, LevelActor.Mode mode, int hardLevel, int difficulty, FriendDetailDataItem friend)
        {
            this.isDebugLevel = true;
            this.luaFile = levelPath;
            this.levelMode = mode;
            this.difficulty = difficulty;
            this.configLevelDrops = new List<DropItem>();
            this.configChallengeIds = new List<int>();
            this.trackChallengeIds = new List<int>();
            this.configAvatarExpInside = 0f;
            this.configScoinInside = 0f;
            this.NPCHardLevel = hardLevel;
            this.maxReviveNum = 3;
            this.avaiableReviveNum = this.maxReviveNum;
            this.friendDetailItem = friend;
            this.InitLevelScore();
            this._level = this.CreateDummyLevelItem(levelPath);
        }

        private void SetEndlessDropItem(EndlessStageBeginRsp rsp)
        {
            this.configLevelDrops = new List<DropItem>();
            this.endlessConfigLevelDrops = new List<DropItem>();
            foreach (DropItem item in rsp.get_drop_item_list())
            {
                if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey((int) item.get_item_id()) != null)
                {
                    this.endlessConfigLevelDrops.Add(item);
                }
                else
                {
                    this.configLevelDrops.Add(item);
                }
            }
        }

        public void SetEndlessLevelBeginIntent(int progress, int hardLevel, List<string> appliedToolList, EndlessStageBeginRsp endlessStageBeginRsp, float levelTimer = -1f, int difficulty = 1)
        {
            this.luaFile = MiscData.Config.BasicConfig.EndlessLevelLuaFilePath;
            this.levelMode = LevelActor.Mode.Single;
            this.difficulty = difficulty;
            this.progress = progress;
            this.LevelType = 4;
            this.appliedToolList = appliedToolList;
            this.SetEndlessDropItem(endlessStageBeginRsp);
            this.configChallengeIds = new List<int>();
            this.trackChallengeIds = new List<int>();
            this.configAvatarExpInside = 0f;
            this.configScoinInside = 0f;
            this.NPCHardLevel = hardLevel;
            this.maxReviveNum = 0;
            this.avaiableReviveNum = this.maxReviveNum;
            this.levelTimer = levelTimer;
            this.memberList = new List<AvatarDataItem>();
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(4);
            for (int i = 0; i < memberList.Count; i++)
            {
                AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i]).Clone();
                this.memberList.Add(item);
            }
            this.InitLevelScore();
            this._level = this.CreateEndlessLevelItem();
        }

        public void SetLevelBeginIntent(LevelDataItem level, int progress, List<DropItem> drops, LevelActor.Mode levelMode = 0, FriendDetailDataItem friendDetailItem = null)
        {
            this._level = level;
            this.progress = progress;
            this.configLevelDrops = drops;
            this.friendDetailItem = friendDetailItem;
            this.NPCHardLevel = this._level.NPCHardLevel;
            this.luaFile = this._level.LuaFile;
            if ((this._level.levelId == 0x2775) && (Singleton<LevelTutorialModule>.Instance.GetUnFinishedTutorialIDList(this._level.levelId).Count > 0))
            {
                this.luaFile = "Lua/Levels/MainLine_Stage01/Level Tutorial.lua";
            }
            this.difficulty = (int) this._level.Diffculty;
            this.levelMode = levelMode;
            this.LevelType = this._level.LevelType;
            this.configChallengeIds = level.GetAllChallengeIdList();
            this.trackChallengeIds = level.GetTrackChallengeIdList();
            this.configAvatarExpInside = level.AvatarExpInside;
            this.configScoinInside = level.ScoinInside;
            this.maxReviveNum = level.maxReviveTimes;
            this.avaiableReviveNum = this.maxReviveNum;
            this.memberList = new List<AvatarDataItem>();
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(level.LevelType);
            for (int i = 0; i < memberList.Count; i++)
            {
                AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(memberList[i]).Clone();
                this.memberList.Add(item);
            }
            this.InitLevelScore();
            this.hasNuclearActivityBefore = this.HasNuclearActivity(level.levelId);
        }

        public void SetTryLevelBeginIntent(int avatarId, string luaPath, int trySkillId = 0, int trySubSkillId = 0)
        {
            this.isTryLevel = true;
            this.NPCHardLevel = 1;
            this.luaFile = luaPath;
            this.difficulty = 1;
            this.levelMode = LevelActor.Mode.Single;
            this.configLevelDrops = new List<DropItem>();
            this.configChallengeIds = new List<int>();
            this.configAvatarExpInside = 0f;
            this.configScoinInside = 0f;
            AvatarDataItem item = Singleton<AvatarModule>.Instance.GetAvatarByID(avatarId).Clone();
            if (trySkillId != 0)
            {
                AvatarSkillDataItem avatarSkillBySkillID = item.GetAvatarSkillBySkillID(trySkillId);
                avatarSkillBySkillID.UnLocked = true;
                if (trySubSkillId != 0)
                {
                    avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId(trySubSkillId).level = 1;
                }
            }
            List<AvatarDataItem> list = new List<AvatarDataItem> {
                item
            };
            this.memberList = list;
            this.InitLevelScore();
        }

        private int SortDropItem(DropItem one, DropItem two)
        {
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) one.get_item_id(), 1);
            StorageDataItemBase itemData = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) one.get_item_id(), 1);
            bool flag = Singleton<StorageModule>.Instance.IsItemNew((int) one.get_item_id());
            bool flag2 = Singleton<StorageModule>.Instance.IsItemNew((int) two.get_item_id());
            if (flag)
            {
                return -1;
            }
            if (flag2)
            {
                return 1;
            }
            int itemTypeIndex = this.GetItemTypeIndex(dummyStorageDataItem);
            int num2 = this.GetItemTypeIndex(itemData);
            if (itemTypeIndex != num2)
            {
                return (itemTypeIndex - num2);
            }
            if (one.get_rarity() != two.get_rarity())
            {
                return (int) (one.get_rarity() - two.get_rarity());
            }
            if (one.get_level() != two.get_level())
            {
                return (int) (one.get_level() - two.get_level());
            }
            return 0;
        }

        public string actTitle
        {
            get
            {
                return ((this._level != null) ? new ActDataItem(this._level.ActID).actTitle : string.Empty);
            }
        }

        public string chapterTitle
        {
            get
            {
                return ((this._level != null) ? Singleton<LevelModule>.Instance.GetChapterById(this._level.ChapterID).Title : string.Empty);
            }
        }

        public int LevelId
        {
            get
            {
                return ((this._level != null) ? this._level.levelId : 0);
            }
        }

        public string LevelTitle
        {
            get
            {
                return ((this._level != null) ? this._level.Title : string.Empty);
            }
        }

        public string stageName
        {
            get
            {
                return ((this._level != null) ? this._level.StageName : string.Empty);
            }
        }

        [CompilerGenerated]
        private sealed class <CheckDropItem>c__AnonStoreyD0
        {
            internal int metaId;

            internal bool <>m__DA(DropItem x)
            {
                return (x.get_item_id() == this.metaId);
            }

            internal bool <>m__DB(DropItem x)
            {
                return (x.get_item_id() == this.metaId);
            }

            internal bool <>m__DC(DropItem x)
            {
                return (x.get_item_id() == this.metaId);
            }
        }
    }
}

