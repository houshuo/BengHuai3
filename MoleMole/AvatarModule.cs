namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class AvatarModule : BaseModule
    {
        private Dictionary<int, AvatarDataItem> _userAvatarDict;

        public AvatarModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.UserAvatarList = new List<AvatarDataItem>();
            this._userAvatarDict = new Dictionary<int, AvatarDataItem>();
            this.AddAllAvatarsFromMetaData();
        }

        private void AddAllAvatarsFromMetaData()
        {
            foreach (AvatarMetaData data in AvatarMetaDataReader.GetItemList())
            {
                if (!this._userAvatarDict.ContainsKey(data.avatarID))
                {
                    this.AddAvatar(this.GetDummyAvatarDataItem(data.avatarID));
                }
            }
        }

        private void AddAvatar(AvatarDataItem avatar)
        {
            this.UserAvatarList.Add(avatar);
            this._userAvatarDict.Add(avatar.avatarID, avatar);
        }

        public AvatarDataItem GetAvatarByID(int avatarID)
        {
            return this._userAvatarDict[avatarID];
        }

        public AvatarMetaData GetAvatarMetaDataByRegistryKey(string registryKey)
        {
            <GetAvatarMetaDataByRegistryKey>c__AnonStoreyC4 yc = new <GetAvatarMetaDataByRegistryKey>c__AnonStoreyC4 {
                registryKey = registryKey
            };
            return AvatarMetaDataReader.GetItemList().Find(new Predicate<AvatarMetaData>(yc.<>m__BA));
        }

        public List<KeyValuePair<string, bool>> GetCanUnlockSkillNameList(int avatarID, int levelBefore, int starBefore, int levelAfter, int starAfter)
        {
            List<KeyValuePair<string, bool>> list = new List<KeyValuePair<string, bool>>();
            AvatarDataItem item = new AvatarDataItem(avatarID, levelBefore, starBefore);
            AvatarDataItem item2 = new AvatarDataItem(avatarID, levelAfter, starAfter);
            for (int i = 0; i < item.skillDataList.Count; i++)
            {
                AvatarSkillDataItem item3 = item.skillDataList[i];
                AvatarSkillDataItem item4 = item2.skillDataList[i];
                if (item4.UnLocked && !item3.UnLocked)
                {
                    list.Add(new KeyValuePair<string, bool>(item4.SkillName, false));
                }
                for (int j = 0; j < item3.avatarSubSkillList.Count; j++)
                {
                    AvatarSubSkillDataItem item5 = item3.avatarSubSkillList[j];
                    AvatarSubSkillDataItem item6 = item4.avatarSubSkillList[j];
                    bool flag = (levelBefore >= item5.UnlockLv) && (starBefore >= item5.UnlockStar);
                    if (((levelAfter >= item6.UnlockLv) && (starAfter >= item6.UnlockStar)) && !flag)
                    {
                        list.Add(new KeyValuePair<string, bool>(item4.SkillName + "." + item6.Name, true));
                    }
                }
            }
            return list;
        }

        public AvatarDataItem GetDummyAvatarDataItem(int id)
        {
            return new AvatarDataItem(id, 1, 0);
        }

        public AvatarDataItem GetDummyAvatarDataItem(string avatarRegistryKey, int level = 1, int star = 0)
        {
            AvatarMetaData avatarMetaDataByRegistryKey = this.GetAvatarMetaDataByRegistryKey(avatarRegistryKey);
            if (avatarMetaDataByRegistryKey == null)
            {
                return this.GetUnusedAvatarDataItem(avatarRegistryKey);
            }
            if (star == 0)
            {
                AvatarStarMetaData starMetaData = new AvatarStarMetaData(avatarMetaDataByRegistryKey.avatarID, 0, 1, 1, "SpriteOutput/AvatarIcon/101", "SpriteOutput/AvatarIconSide/101", "SpriteOutput/AvatarTachie/KianaC2", 1000f, 0f, 1000f, 0f, 100f, 0f, 0f, 0f, 0f, 0f);
                AvatarDataItem item = new AvatarDataItem(avatarMetaDataByRegistryKey.avatarID, avatarMetaDataByRegistryKey, ClassMetaDataReader.GetClassMetaDataByKey(avatarMetaDataByRegistryKey.classID), starMetaData, AvatarLevelMetaDataReader.GetAvatarLevelMetaDataByKey(level), level, star);
                item.UpdateSkillInfo();
                return item;
            }
            return new AvatarDataItem(avatarMetaDataByRegistryKey.avatarID, level, star);
        }

        private AvatarDataItem GetUnusedAvatarDataItem(string avatarRegistryKey)
        {
            int avatarID = 0x186a1;
            AvatarMetaData metaData = new AvatarMetaData(avatarID, 1, "DUMMY_" + avatarRegistryKey, avatarRegistryKey, "DUMMY AVATAR FOR DEVLOPEMENT :" + avatarRegistryKey, avatarRegistryKey, new List<int>(), 0, new List<int> { 11, 12, 13, 14, 15, 0x10 }, 1, 0, 0, 0, 0, 14, 15, 3f, 0f, 0f, 0, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0, 0f, 0);
            ClassMetaData classMetaData = new ClassMetaData(0x2710, "DUMMY CLASS", "DUMMY CLASS", "FirstName", "LastName");
            AvatarLevelMetaData levelMetaData = new AvatarLevelMetaData(1, 1, 0, 1f);
            return new AvatarDataItem(avatarID, metaData, classMetaData, new AvatarStarMetaData(avatarID, 1, 1, 1, "SpriteOutput/AvatarIcon/101", "SpriteOutput/AvatarIconSide/101", "SpriteOutput/AvatarTachie/KianaC2", 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f), levelMetaData, 1, 1);
        }

        private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (Avatar avatar in rsp.get_avatar_list())
                {
                    int body = (int) avatar.get_avatar_id();
                    this._userAvatarDict[body].Initialized = true;
                    if (!this._userAvatarDict[body].UnLocked && (avatar.get_star() > 0))
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UnlockAvatar, body));
                    }
                    this._userAvatarDict[body].UnLocked = avatar.get_star() > 0;
                    if (this._userAvatarDict[body].UnLocked)
                    {
                        base.UpdateField<int>(ref this._userAvatarDict[body].star, (int) avatar.get_star(), new Action<int, int>(this._userAvatarDict[body].OnStarUpdate));
                        base.UpdateField<int>(ref this._userAvatarDict[body].star, (int) avatar.get_star(), new Action<int, int>(this._userAvatarDict[body].OnStarUpdate));
                        this.UpdateEquipment(this._userAvatarDict[body], avatar);
                    }
                    base.UpdateField<int>(ref this._userAvatarDict[body].level, (int) avatar.get_level(), new Action<int, int>(this._userAvatarDict[body].OnLevelUpdate));
                    this._userAvatarDict[body].exp = (int) avatar.get_exp();
                    this._userAvatarDict[body].fragment = (int) avatar.get_fragment();
                    foreach (AvatarSkill skill in avatar.get_skill_list())
                    {
                        AvatarSkillDataItem avatarSkillBySkillID = this._userAvatarDict[body].GetAvatarSkillBySkillID((int) skill.get_skill_id());
                        foreach (AvatarSubSkill skill2 in skill.get_sub_skill_list())
                        {
                            avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int) skill2.get_sub_skill_id()).level = (int) skill2.get_level();
                        }
                    }
                }
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x19) && this.OnGetAvatarDataRsp(pkt.getData<GetAvatarDataRsp>()));
        }

        public AvatarDataItem TryGetAvatarByID(int avatarID)
        {
            AvatarDataItem item;
            this._userAvatarDict.TryGetValue(avatarID, out item);
            return item;
        }

        private void UpdateEquipment(AvatarDataItem dataItem, Avatar packetItem)
        {
            int[] numArray = new int[] { packetItem.get_weapon_unique_id(), packetItem.get_stigmata_unique_id_1(), packetItem.get_stigmata_unique_id_2(), packetItem.get_stigmata_unique_id_3() };
            for (int i = 0; i < numArray.Length; i++)
            {
                EquipmentSlot slot = AvatarDataItem.EQUIP_SLOTS[i];
                this.UpdateEquipmentBySlot(dataItem, slot, numArray[i]);
            }
        }

        private void UpdateEquipmentBySlot(AvatarDataItem avatar, EquipmentSlot slot, int newUid)
        {
            System.Type type = (slot != 1) ? typeof(StigmataDataItem) : typeof(WeaponDataItem);
            StorageDataItemBase base2 = avatar.equipsMap[slot];
            avatar.equipsMap[slot] = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(type, newUid);
            StorageDataItemBase base3 = avatar.equipsMap[slot];
            if ((base2 != null) && (base2.avatarID == avatar.avatarID))
            {
                base2.avatarID = -1;
            }
            if (base3 != null)
            {
                base3.avatarID = avatar.avatarID;
            }
        }

        public bool anyAvatarCanUnlock
        {
            get
            {
                foreach (AvatarDataItem item in this.UserAvatarList)
                {
                    if (item.CanUnlock)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public List<AvatarDataItem> UserAvatarList { get; private set; }

        [CompilerGenerated]
        private sealed class <GetAvatarMetaDataByRegistryKey>c__AnonStoreyC4
        {
            internal string registryKey;

            internal bool <>m__BA(AvatarMetaData avatarMetaData)
            {
                return (avatarMetaData.avatarRegistryKey == this.registryKey);
            }
        }
    }
}

