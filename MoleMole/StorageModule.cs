namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class StorageModule : BaseModule
    {
        private HashSet<int> _everOwnItemIDs;
        private Dictionary<KeyValuePair<System.Type, int>, StorageDataItemBase> _userStorageDict;
        [CompilerGenerated]
        private static Predicate<StorageDataItemBase> <>f__am$cache5;
        [CompilerGenerated]
        private static Predicate<StorageDataItemBase> <>f__am$cache6;
        [CompilerGenerated]
        private static Predicate<StorageDataItemBase> <>f__am$cache7;
        [CompilerGenerated]
        private static Predicate<StorageDataItemBase> <>f__am$cache8;
        public Dictionary<StorageSortType, Comparison<StorageDataItemBase>> sortComparisionMap;
        public Dictionary<string, StorageSortType> sortTypeMap;

        public StorageModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.UserStorageItemList = new List<StorageDataItemBase>();
            this._userStorageDict = new Dictionary<KeyValuePair<System.Type, int>, StorageDataItemBase>();
            this._everOwnItemIDs = new HashSet<int>();
            this.InitForSort();
        }

        public List<StorageDataItemBase> GetAllAvatarExpAddMaterial()
        {
            List<StorageDataItemBase> list = new List<StorageDataItemBase>();
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = dataItem => (dataItem is MaterialDataItem) && ((dataItem as MaterialDataItem).GetAvatarExpProvideNum() > 0f);
            }
            list.AddRange(this.UserStorageItemList.FindAll(<>f__am$cache8));
            return list;
        }

        public List<StorageDataItemBase> GetAllUserMaterial()
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = dataItem => dataItem is MaterialDataItem;
            }
            return this.UserStorageItemList.FindAll(<>f__am$cache7);
        }

        public List<StorageDataItemBase> GetAllUserStigmata()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = dataItem => dataItem is StigmataDataItem;
            }
            return this.UserStorageItemList.FindAll(<>f__am$cache6);
        }

        public List<StorageDataItemBase> GetAllUserWeapons()
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = dataItem => dataItem is WeaponDataItem;
            }
            return this.UserStorageItemList.FindAll(<>f__am$cache5);
        }

        public List<StorageDataItemBase> GetAllVentureSpeedUpMaterial()
        {
            return this.UserStorageItemList.FindAll(dataItem => this.IsVentureSpeedUpMaterial(dataItem.ID));
        }

        public int GetCurrentCapacity()
        {
            return this.UserStorageItemList.Count;
        }

        public WeaponDataItem GetDummyFirstWeaponDataByRole(EntityRoleName role, int level)
        {
            foreach (ConfigWeapon weapon in WeaponData.GetAllWeaponConfigs().Values)
            {
                if (weapon.OwnerRole == role)
                {
                    return new WeaponDataItem(1, new WeaponMetaData(weapon.WeaponID, "DUMMY WEAPON", 1, 1, 1, 1, 1, 1, 1, 1, 1f, 1f, 1f, 1f, "TYPE_FOON", 1, 1, "BODY_MOD", "DUMMY WEAPON", "DUMMY WEAPON", 1, string.Empty, string.Empty, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 1f, 1f, 1f, 1f, 1, new List<string>(), 1, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f));
                }
            }
            return null;
        }

        public StigmataDataItem GetDummyStigmataDataItem(int stigmataID, int level)
        {
            StigmataMetaData stigmataMetaDataByKey = StigmataMetaDataReader.GetStigmataMetaDataByKey(stigmataID);
            if (stigmataMetaDataByKey == null)
            {
                return null;
            }
            return new StigmataDataItem(0x4e20, stigmataMetaDataByKey) { level = level };
        }

        public StorageDataItemBase GetDummyStorageDataItem(int metaId, int level = 1)
        {
            StorageDataItemBase base2 = null;
            ItemMetaData itemMetaData = ItemMetaDataReader.TryGetItemMetaDataByKey(metaId);
            if (itemMetaData != null)
            {
                return new MaterialDataItem(itemMetaData) { level = level };
            }
            WeaponMetaData weaponMetaData = WeaponMetaDataReader.TryGetWeaponMetaDataByKey(metaId);
            if (weaponMetaData != null)
            {
                return new WeaponDataItem(0, weaponMetaData) { level = level };
            }
            StigmataMetaData stigmataMetaData = StigmataMetaDataReader.TryGetStigmataMetaDataByKey(metaId);
            if (stigmataMetaData != null)
            {
                base2 = new StigmataDataItem(0, stigmataMetaData) {
                    level = level
                };
                (base2 as StigmataDataItem).SetDummyAffixSkill();
                return base2;
            }
            AvatarCardMetaData avatarCardMetaData = AvatarCardMetaDataReader.TryGetAvatarCardMetaDataByKey(metaId);
            if (avatarCardMetaData != null)
            {
                return new AvatarCardDataItem(avatarCardMetaData) { level = level };
            }
            AvatarFragmentMetaData data5 = AvatarFragmentMetaDataReader.TryGetAvatarFragmentMetaDataByKey(metaId);
            if (data5 != null)
            {
                return new AvatarFragmentDataItem(data5) { level = level };
            }
            if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaId) != null)
            {
                return new EndlessToolDataItem(metaId, 1) { level = level };
            }
            return null;
        }

        public WeaponDataItem GetDummyWeaponDataItem(int weaponID, int level)
        {
            return new WeaponDataItem(0, WeaponMetaDataReader.GetWeaponMetaDataByKey(weaponID)) { level = level };
        }

        public List<StorageDataItemBase> GetFragmentList()
        {
            List<StorageDataItemBase> list = new List<StorageDataItemBase>();
            foreach (AvatarDataItem item in Singleton<AvatarModule>.Instance.UserAvatarList)
            {
                if (item.fragment > 0)
                {
                    list.Add(item.GetAvatarFragmentDataItem());
                }
            }
            return list;
        }

        public List<StigmataDataItem> GetStigmatasCanUseForNewAffix(StigmataDataItem stigmata)
        {
            List<StorageDataItemBase> allUserStigmata = this.GetAllUserStigmata();
            List<StigmataDataItem> list2 = new List<StigmataDataItem>();
            foreach (StorageDataItemBase base2 in allUserStigmata)
            {
                StigmataDataItem item = base2 as StigmataDataItem;
                if ((((item != null) && (item.uid != stigmata.uid)) && ((item.ID == stigmata.ID) || StigmataMetaDataReaderExtend.IsEvoRelation(item.ID, stigmata.ID))) && item.CanRefine)
                {
                    list2.Add(item);
                }
            }
            return list2;
        }

        public StorageDataItemBase GetStorageItemByTypeAndID(System.Type type, int id)
        {
            StorageDataItemBase base2;
            KeyValuePair<System.Type, int> key = new KeyValuePair<System.Type, int>(type, id);
            this._userStorageDict.TryGetValue(key, out base2);
            return base2;
        }

        public bool HasEnoughItem(int itemMetaId, int itemNum)
        {
            StorageDataItemBase base2;
            if (this.GetDummyStorageDataItem(itemMetaId, 1) == null)
            {
                return true;
            }
            this._userStorageDict.TryGetValue(new KeyValuePair<System.Type, int>(typeof(MaterialDataItem), itemMetaId), out base2);
            return ((base2 != null) && (base2.number >= itemNum));
        }

        private void InitForSort()
        {
            this.sortTypeMap = new Dictionary<string, StorageSortType>();
            this.sortComparisionMap = new Dictionary<StorageSortType, Comparison<StorageDataItemBase>>();
            foreach (string str in StorageShowPageContext.TAB_KEY)
            {
                this.sortTypeMap.Add(str, StorageSortType.Rarity_DESC);
            }
            this.sortComparisionMap.Add(StorageSortType.Rarity_DESC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToRarityDesc));
            this.sortComparisionMap.Add(StorageSortType.Rarity_ASC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToRarityAsc));
            this.sortComparisionMap.Add(StorageSortType.Level_DESC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToLevelDesc));
            this.sortComparisionMap.Add(StorageSortType.Level_ASC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToLevelAsc));
            this.sortComparisionMap.Add(StorageSortType.Cost_DESC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToCostDesc));
            this.sortComparisionMap.Add(StorageSortType.Cost_ASC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToCostAsc));
            this.sortComparisionMap.Add(StorageSortType.BaseType_DESC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToBaseTypeDesc));
            this.sortComparisionMap.Add(StorageSortType.BaseType_ASC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToBaseTypeAsc));
            this.sortComparisionMap.Add(StorageSortType.Time_DESC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToUidDesc));
            this.sortComparisionMap.Add(StorageSortType.Time_ASC, new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToUidAsc));
        }

        public bool IsFull()
        {
            return (this.UserStorageItemList.Count >= Singleton<PlayerModule>.Instance.playerData.equipmentSizeLimit);
        }

        public bool IsItemNew(int id)
        {
            return !this._everOwnItemIDs.Contains(id);
        }

        public bool IsVentureSpeedUpMaterial(int metaID)
        {
            return (MaterialVentureSpeedUpDataReader.TryGetMaterialVentureSpeedUpDataByKey(metaID) != null);
        }

        private bool OnDelEquipmentNotify(DelEquipmentNotify rsp)
        {
            foreach (uint num in rsp.get_weapon_unique_id_list())
            {
                int num2 = (int) num;
                WeaponDataItem item = null;
                if (this._userStorageDict.ContainsKey(new KeyValuePair<System.Type, int>(typeof(WeaponDataItem), num2)))
                {
                    item = this._userStorageDict[new KeyValuePair<System.Type, int>(typeof(WeaponDataItem), num2)] as WeaponDataItem;
                    this.UserStorageItemList.Remove(item);
                    this._userStorageDict.Remove(new KeyValuePair<System.Type, int>(typeof(WeaponDataItem), num2));
                }
            }
            foreach (uint num3 in rsp.get_stigmata_unique_id_list())
            {
                int num4 = (int) num3;
                StigmataDataItem item2 = null;
                if (this._userStorageDict.ContainsKey(new KeyValuePair<System.Type, int>(typeof(StigmataDataItem), num4)))
                {
                    item2 = this._userStorageDict[new KeyValuePair<System.Type, int>(typeof(StigmataDataItem), num4)] as StigmataDataItem;
                    this.UserStorageItemList.Remove(item2);
                    this._userStorageDict.Remove(new KeyValuePair<System.Type, int>(typeof(StigmataDataItem), num4));
                }
            }
            return false;
        }

        private bool OnDressEquipmentRsp(DressEquipmentRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        private bool OnEquipmentEvoRsp(EquipmentEvoRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                EquipmentItem item = rsp.get_new_item();
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            }
            return false;
        }

        private bool OnEquipmentPowerUpRsp(EquipmentPowerUpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        private bool OnEquipmentSellRsp(EquipmentSellRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        private bool OnGetEquipmentDataRsp(GetEquipmentDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (Weapon weapon in rsp.get_weapon_list())
                {
                    WeaponDataItem item = null;
                    if (this._userStorageDict.ContainsKey(new KeyValuePair<System.Type, int>(typeof(WeaponDataItem), (int) weapon.get_unique_id())))
                    {
                        item = this._userStorageDict[new KeyValuePair<System.Type, int>(typeof(WeaponDataItem), (int) weapon.get_unique_id())] as WeaponDataItem;
                    }
                    else
                    {
                        WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey((int) weapon.get_id());
                        item = new WeaponDataItem((int) weapon.get_unique_id(), weaponMetaDataByKey);
                        this.UserStorageItemList.Add(item);
                        this._userStorageDict.Add(new KeyValuePair<System.Type, int>(item.GetType(), item.uid), item);
                        Singleton<ItempediaModule>.Instance.UnlockItem((int) weapon.get_id());
                    }
                    item.level = (int) weapon.get_level();
                    item.exp = (int) weapon.get_exp();
                    item.isProtected = weapon.get_is_protected();
                }
                foreach (Stigmata stigmata in rsp.get_stigmata_list())
                {
                    StigmataDataItem item2 = null;
                    if (this._userStorageDict.ContainsKey(new KeyValuePair<System.Type, int>(typeof(StigmataDataItem), (int) stigmata.get_unique_id())))
                    {
                        item2 = this._userStorageDict[new KeyValuePair<System.Type, int>(typeof(StigmataDataItem), (int) stigmata.get_unique_id())] as StigmataDataItem;
                    }
                    else
                    {
                        StigmataMetaData stigmataMetaDataByKey = StigmataMetaDataReader.GetStigmataMetaDataByKey((int) stigmata.get_id());
                        item2 = new StigmataDataItem((int) stigmata.get_unique_id(), stigmataMetaDataByKey);
                        this.UserStorageItemList.Add(item2);
                        this._userStorageDict.Add(new KeyValuePair<System.Type, int>(item2.GetType(), item2.uid), item2);
                        Singleton<ItempediaModule>.Instance.UnlockItem((int) stigmata.get_id());
                    }
                    item2.level = (int) stigmata.get_level();
                    item2.exp = (int) stigmata.get_exp();
                    item2.isProtected = stigmata.get_is_protected();
                    bool flag = !stigmata.get_is_affix_identifySpecified() || stigmata.get_is_affix_identify();
                    int num = !stigmata.get_pre_affix_idSpecified() ? 0 : ((int) stigmata.get_pre_affix_id());
                    int num2 = !stigmata.get_suf_affix_idSpecified() ? 0 : ((int) stigmata.get_suf_affix_id());
                    item2.SetAffixSkill(flag, num, num2);
                }
                foreach (Material material in rsp.get_material_list())
                {
                    MaterialDataItem item3 = null;
                    if (this._userStorageDict.ContainsKey(new KeyValuePair<System.Type, int>(typeof(MaterialDataItem), (int) material.get_id())))
                    {
                        item3 = this._userStorageDict[new KeyValuePair<System.Type, int>(typeof(MaterialDataItem), (int) material.get_id())] as MaterialDataItem;
                        if (material.get_num() > 0)
                        {
                            goto Label_0384;
                        }
                        this._userStorageDict.Remove(new KeyValuePair<System.Type, int>(typeof(MaterialDataItem), (int) material.get_id()));
                        this.UserStorageItemList.Remove(item3);
                        continue;
                    }
                    item3 = new MaterialDataItem(ItemMetaDataReader.GetItemMetaDataByKey((int) material.get_id()));
                    this.UserStorageItemList.Add(item3);
                    this._userStorageDict.Add(new KeyValuePair<System.Type, int>(item3.GetType(), item3.ID), item3);
                    Singleton<ItempediaModule>.Instance.UnlockItem((int) material.get_id());
                Label_0384:
                    item3.number = (int) material.get_num();
                }
            }
            return false;
        }

        private bool OnGetHasGotItemIdListRsp(GetHasGotItemIdListRsp rsp)
        {
            this._everOwnItemIDs.Clear();
            foreach (uint num in rsp.get_item_id_list())
            {
                this._everOwnItemIDs.Add((int) num);
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x1b:
                    return this.OnGetEquipmentDataRsp(pkt.getData<GetEquipmentDataRsp>());

                case 0x1c:
                    return this.OnDelEquipmentNotify(pkt.getData<DelEquipmentNotify>());

                case 0x20:
                    return this.OnEquipmentPowerUpRsp(pkt.getData<EquipmentPowerUpRsp>());

                case 0x22:
                    return this.OnEquipmentSellRsp(pkt.getData<EquipmentSellRsp>());

                case 0x67:
                    return this.OnSellAvatarFragmentRsp(pkt.getData<SellAvatarFragmentRsp>());

                case 0x26:
                    return this.OnEquipmentEvoRsp(pkt.getData<EquipmentEvoRsp>());

                case 40:
                    return this.OnDressEquipmentRsp(pkt.getData<DressEquipmentRsp>());

                case 0x69:
                    return this.OnGetHasGotItemIdListRsp(pkt.getData<GetHasGotItemIdListRsp>());
            }
            return false;
        }

        private bool OnSellAvatarFragmentRsp(SellAvatarFragmentRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        public void RecordNewItem(int id)
        {
            this._everOwnItemIDs.Add(id);
        }

        public MaterialDataItem TryGetMaterialDataByID(int id)
        {
            StorageDataItemBase base2;
            this._userStorageDict.TryGetValue(new KeyValuePair<System.Type, int>(typeof(MaterialDataItem), id), out base2);
            return (base2 as MaterialDataItem);
        }

        public List<StorageDataItemBase> TryGetStorageDataItemByMetaId(int metaId, int number = 1)
        {
            <TryGetStorageDataItemByMetaId>c__AnonStoreyD4 yd = new <TryGetStorageDataItemByMetaId>c__AnonStoreyD4 {
                metaId = metaId,
                number = number
            };
            return this.UserStorageItemList.FindAll(new Predicate<StorageDataItemBase>(yd.<>m__EC));
        }

        public List<StorageDataItemBase> UserStorageItemList { get; private set; }

        [CompilerGenerated]
        private sealed class <TryGetStorageDataItemByMetaId>c__AnonStoreyD4
        {
            internal int metaId;
            internal int number;

            internal bool <>m__EC(StorageDataItemBase x)
            {
                return ((x.ID == this.metaId) && (x.number >= this.number));
            }
        }

        public enum StorageSortType
        {
            Rarity_DESC,
            Rarity_ASC,
            Level_DESC,
            Level_ASC,
            BaseType_DESC,
            BaseType_ASC,
            Cost_DESC,
            Cost_ASC,
            Suite_DESC,
            Suite_ASC,
            Time_DESC,
            Time_ASC
        }
    }
}

