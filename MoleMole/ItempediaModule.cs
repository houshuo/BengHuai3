namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ItempediaModule : BaseModule
    {
        private List<int> _itemIds = new List<int>();
        private List<int> _stigmataIds = new List<int>();
        private List<int> _unlockedItemIds;
        private List<int> _weaponIds = new List<int>();
        [CompilerGenerated]
        private static Predicate<int> <>f__am$cache4;
        [CompilerGenerated]
        private static Predicate<int> <>f__am$cache5;
        [CompilerGenerated]
        private static Predicate<int> <>f__am$cache6;

        public ItempediaModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
        }

        public int GetAllItemCount()
        {
            return this._itemIds.Count;
        }

        private List<ItemMetaData> GetAllItemData()
        {
            List<ItemMetaData> itemList = ItemMetaDataReader.GetItemList();
            List<ItemMetaData> list2 = new List<ItemMetaData>();
            int num = 0;
            int count = itemList.Count;
            while (num < count)
            {
                ItemMetaData item = itemList[num];
                if (!ItempediaData.IsInBlacklist(item.ID))
                {
                    list2.Add(item);
                }
                num++;
            }
            return list2;
        }

        public int GetAllStigmataCount()
        {
            return this._stigmataIds.Count;
        }

        private List<StigmataMetaData> GetAllStigmataData()
        {
            List<StigmataMetaData> itemList = StigmataMetaDataReader.GetItemList();
            List<StigmataMetaData> list2 = new List<StigmataMetaData>();
            int num = 0;
            int count = itemList.Count;
            while (num < count)
            {
                StigmataMetaData item = itemList[num];
                if (!ItempediaData.IsInBlacklist(item.ID) && (item.subRarity == 0))
                {
                    list2.Add(item);
                }
                num++;
            }
            return list2;
        }

        public int[] GetAllUnlockItems()
        {
            return this._unlockedItemIds.ToArray();
        }

        public int GetAllWeaponCount()
        {
            return this._weaponIds.Count;
        }

        private List<WeaponMetaData> GetAllWeaponData()
        {
            List<WeaponMetaData> itemList = WeaponMetaDataReader.GetItemList();
            List<WeaponMetaData> list2 = new List<WeaponMetaData>();
            int num = 0;
            int count = itemList.Count;
            while (num < count)
            {
                WeaponMetaData item = itemList[num];
                if (!ItempediaData.IsInBlacklist(item.ID) && (item.subRarity == 0))
                {
                    list2.Add(item);
                }
                num++;
            }
            return list2;
        }

        public int GetItempediaTotalCount()
        {
            return (this.GetAllWeaponCount() + this.GetAllStigmataCount());
        }

        private int GetUnlockCountByPredicate(Predicate<int> pred)
        {
            if (this._unlockedItemIds == null)
            {
                return 0;
            }
            int num = 0;
            int num2 = 0;
            int count = this._unlockedItemIds.Count;
            while (num2 < count)
            {
                if (pred(this._unlockedItemIds[num2]))
                {
                    num++;
                }
                num2++;
            }
            return num;
        }

        public int GetUnlockCountItem()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = x => x < 0x2710;
            }
            return this.GetUnlockCountByPredicate(<>f__am$cache6);
        }

        public int GetUnlockCountStigmata()
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = x => (x / 0x2710) == 3;
            }
            return this.GetUnlockCountByPredicate(<>f__am$cache5);
        }

        public int GetUnlockCountTotal()
        {
            return (this.GetUnlockCountWeapon() + this.GetUnlockCountStigmata());
        }

        public int GetUnlockCountWeapon()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => (x / 0x2710) == 2;
            }
            return this.GetUnlockCountByPredicate(<>f__am$cache4);
        }

        private bool IsContainedInMultiList(int id, params List<int>[] lists)
        {
            int num = 0;
            while (true)
            {
                int index = 0;
                int length = lists.Length;
                while (index < length)
                {
                    List<int> list = lists[index];
                    if (list.Count > index)
                    {
                        int num4 = 0;
                        int count = list.Count;
                        while (num4 < count)
                        {
                            if (list[num4] == id)
                            {
                                return true;
                            }
                            num4++;
                        }
                    }
                    index++;
                }
                bool flag = true;
                int num6 = 0;
                int num7 = lists.Length;
                while (num6 < num7)
                {
                    if (lists[num6].Count > num)
                    {
                        flag = false;
                        break;
                    }
                    num6++;
                }
                if (flag)
                {
                    return false;
                }
                num++;
            }
        }

        public bool IsInItempedia(int id)
        {
            return ((this._weaponIds.Contains(id) || this._stigmataIds.Contains(id)) || this._itemIds.Contains(id));
        }

        private bool OnGetHasGotItemIdListRsp(GetHasGotItemIdListRsp rsp)
        {
            this.UpdateIdList();
            this._unlockedItemIds = new List<int>(rsp.get_item_id_list().Count);
            int num = 0;
            int count = rsp.get_item_id_list().Count;
            while (num < count)
            {
                if (this.IsInItempedia(rsp.get_item_id_list()[num]))
                {
                    this._unlockedItemIds.Add(rsp.get_item_id_list()[num]);
                }
                num++;
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 packet)
        {
            return ((packet.getCmdId() == 0x69) && this.OnGetHasGotItemIdListRsp(packet.getData<GetHasGotItemIdListRsp>()));
        }

        public void UnlockItem(int id)
        {
            List<int>[] lists = new List<int>[] { this._weaponIds, this._stigmataIds, this._itemIds };
            if (this.IsContainedInMultiList(id, lists) && !this._unlockedItemIds.Contains(id))
            {
                this._unlockedItemIds.Add(id);
            }
        }

        private void UpdateIdList()
        {
            this._weaponIds.Clear();
            this._stigmataIds.Clear();
            this._itemIds.Clear();
            List<WeaponMetaData> allWeaponData = this.GetAllWeaponData();
            int num = 0;
            int count = allWeaponData.Count;
            while (num < count)
            {
                this._weaponIds.Add(allWeaponData[num].ID);
                num++;
            }
            List<StigmataMetaData> allStigmataData = this.GetAllStigmataData();
            int num3 = 0;
            int num4 = allStigmataData.Count;
            while (num3 < num4)
            {
                this._stigmataIds.Add(allStigmataData[num3].ID);
                num3++;
            }
            List<ItemMetaData> allItemData = this.GetAllItemData();
            int num5 = 0;
            int num6 = allItemData.Count;
            while (num5 < num6)
            {
                this._itemIds.Add(allItemData[num5].ID);
                num5++;
            }
        }
    }
}

