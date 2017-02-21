namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class IslandModule : BaseModule
    {
        private Dictionary<CabinType, CabinDataItemBase> _cabinDict;
        private HashSet<int> _dispatchAvatarIdSet;
        private DateTime _venture_endtime_background = DateTime.MaxValue;
        private bool _venture_inprogress_background;
        private Dictionary<int, VentureDataItem> _ventureDict;
        private int _ventureRefreshTimes;
        [CompilerGenerated]
        private static Func<CabinDataItemBase, bool> <>f__am$cache6;
        [CompilerGenerated]
        private static Comparison<VentureDataItem> <>f__am$cache7;

        public IslandModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.InitCabinDict();
            this._ventureDict = new Dictionary<int, VentureDataItem>();
            this._ventureRefreshTimes = 0;
            this._dispatchAvatarIdSet = new HashSet<int>();
        }

        public CabinAvatarEnhanceDataItem GetAvatarEnhanceCabinByClass(int classID)
        {
            CabinAvatarEnhanceDataItem item = this._cabinDict[2] as CabinAvatarEnhanceDataItem;
            if (item._classType == classID)
            {
                return item;
            }
            item = this._cabinDict[6] as CabinAvatarEnhanceDataItem;
            if (item._classType == classID)
            {
                return item;
            }
            item = this._cabinDict[7] as CabinAvatarEnhanceDataItem;
            if (item._classType == classID)
            {
                return item;
            }
            return null;
        }

        public CabinDataItemBase GetCabinDataByType(CabinType cabinType)
        {
            return this._cabinDict[cabinType];
        }

        public List<CabinDataItemBase> GetCabinList()
        {
            return Enumerable.ToList<CabinDataItemBase>(this._cabinDict.Values);
        }

        public int GetDropMaterialPackageNum()
        {
            if (this.IsDropMaterials())
            {
                return (this._cabinDict[3]._techTree.GetAbilitySum(13, 1) + 1);
            }
            return 0;
        }

        public int GetFinishLevelUpNowHcoinCost(int timeRemain)
        {
            float f = 0f;
            foreach (CabinLevelUpTimePriceMetaData data in CabinLevelUpTimePriceMetaDataReader.GetItemList())
            {
                if (timeRemain > data.timeMax)
                {
                    f += data.price;
                }
                else
                {
                    int num2 = (timeRemain - data.timeMin) + 1;
                    f += (num2 * data.price) / ((float) ((data.timeMax - data.timeMin) + 1));
                    break;
                }
            }
            return Mathf.CeilToInt(f);
        }

        public int GetLeftPowerCost()
        {
            return (this.GetMaxPowerCost() - this.GetUsedPowerCost());
        }

        public int GetMaxFriendAdd()
        {
            return this._cabinDict[4]._techTree.GetAbilitySum(5, 1);
        }

        public int GetMaxPowerCost()
        {
            return CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(this._cabinDict[1].level).MaxPowerCost;
        }

        public int GetNextLevelMaxPowerCost()
        {
            int level = this._cabinDict[1].level + 1;
            if (CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(level) != null)
            {
                return CabinPowerCostMetaDataReader.GetCabinPowerCostMetaDataByKey(level).MaxPowerCost;
            }
            return -1;
        }

        public int GetSkillPointAdd()
        {
            return this._cabinDict[4]._techTree.GetAbilitySum(11, 1);
        }

        public List<CabinDataItemBase> GetUnlockCabinDataList()
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = x => x.status == CabinStatus.UnLocked;
            }
            return Enumerable.ToList<CabinDataItemBase>(this._cabinDict.Values.Where<CabinDataItemBase>(<>f__am$cache6));
        }

        public int GetUsedPowerCost()
        {
            int num = 0;
            IEnumerator enumerator = Enum.GetValues(typeof(CabinType)).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    CabinType current = (CabinType) ((int) enumerator.Current);
                    num += this._cabinDict[current].GetUsedPower();
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return num;
        }

        public int GetVentureDoneNum()
        {
            int num = 0;
            foreach (KeyValuePair<int, VentureDataItem> pair in this._ventureDict)
            {
                if (pair.Value.status == VentureDataItem.VentureStatus.Done)
                {
                    num++;
                }
            }
            return num;
        }

        public int GetVentureInProgressNum()
        {
            int num = 0;
            foreach (KeyValuePair<int, VentureDataItem> pair in this._ventureDict)
            {
                if ((pair.Value.status == VentureDataItem.VentureStatus.InProgress) || (pair.Value.status == VentureDataItem.VentureStatus.Done))
                {
                    num++;
                }
            }
            return num;
        }

        public List<VentureDataItem> GetVentureList()
        {
            List<VentureDataItem> list = Enumerable.ToList<VentureDataItem>(this._ventureDict.Values);
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = (left, right) => left.StaminaCost - right.StaminaCost;
            }
            list.Sort(<>f__am$cache7);
            return list;
        }

        public bool HasCabinLevelUpInProgress()
        {
            foreach (KeyValuePair<CabinType, CabinDataItemBase> pair in this._cabinDict)
            {
                if (pair.Value.levelUpEndTime > TimeUtil.Now)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasCabinNeedToShowLevelUp()
        {
            foreach (KeyValuePair<CabinType, CabinDataItemBase> pair in this._cabinDict)
            {
                if (pair.Value.NeedToShowLevelUpComplete())
                {
                    return true;
                }
            }
            return false;
        }

        private void InitCabinDict()
        {
            this._cabinDict = new Dictionary<CabinType, CabinDataItemBase>();
            this._cabinDict[1] = CabinEngineDataItem.GetInstance();
            this._cabinDict[3] = CabinCollectDataItem.GetInstance();
            this._cabinDict[4] = CabinMiscDataItem.GetInstance();
            this._cabinDict[5] = CabinVentureDataItem.GetInstance();
            this._cabinDict[2] = CabinKianaEnhanceDataItem.GetInstance();
            this._cabinDict[6] = CabinMeiEnhanceDataItem.GetInstance();
            this._cabinDict[7] = CabinBronyaEnhanceDataItem.GetInstance();
            foreach (CabinDataItemBase base2 in this._cabinDict.Values)
            {
                base2.level = 0;
                base2.extendGrade = 1;
            }
        }

        public void InitTechTree()
        {
            this._cabinDict[3]._techTree.InitMetaData();
            this._cabinDict[4]._techTree.InitMetaData();
            this._cabinDict[5]._techTree.InitMetaData();
            this._cabinDict[2]._techTree.InitMetaData();
            this._cabinDict[6]._techTree.InitMetaData();
            this._cabinDict[7]._techTree.InitMetaData();
        }

        public bool IsAvatarDispatched(int avatarId)
        {
            return this._dispatchAvatarIdSet.Contains(avatarId);
        }

        public bool IsDropMaterials()
        {
            return this._cabinDict[3]._techTree.AbilityUnLock(12);
        }

        private bool OnAddCabinTechRsp(AddCabinTechRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
            }
            return false;
        }

        private bool OnDelIslandVentureNotify(DelIslandVentureNotify rsp)
        {
            <OnDelIslandVentureNotify>c__AnonStoreyCE yce = new <OnDelIslandVentureNotify>c__AnonStoreyCE {
                <>f__this = this
            };
            using (List<uint>.Enumerator enumerator = rsp.get_venture_id_list().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yce.id = enumerator.Current;
                    this._dispatchAvatarIdSet.RemoveWhere(new Predicate<int>(yce.<>m__D0));
                    this._ventureDict.Remove((int) yce.id);
                }
            }
            return false;
        }

        private bool OnDispatchIslandVentureRsp(DispatchIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
            }
            return false;
        }

        private bool OnGetCollectCabinRsp(GetCollectCabinRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                CabinCollectDataItem item = this._cabinDict[3] as CabinCollectDataItem;
                item.currentScoinAmount = (int) rsp.get_add_scoin();
                item.dropItems = rsp.get_drop_item_list();
                item.nextScoinUpdateTime = Miscs.GetDateTimeFromTimeStamp(rsp.get_next_add_time());
                item.canUpdateScoinLate = rsp.get_next_add_timeSpecified();
            }
            return false;
        }

        private bool OnGetIsLandRsp(GetIslandRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (Cabin cabin in rsp.get_cabin_list())
                {
                    CabinType key = (CabinType) cabin.get_type();
                    if (this._cabinDict.ContainsKey(key))
                    {
                        CabinDataItemBase base2 = this._cabinDict[key];
                        base2.level = (int) cabin.get_level();
                        base2.extendGrade = (int) cabin.get_extend_grade();
                        base2.levelUpEndTime = !cabin.get_level_up_end_timeSpecified() ? TimeUtil.Now.AddDays(-1.0) : Miscs.GetDateTimeFromTimeStamp(cabin.get_level_up_end_time());
                        base2.SetupMateData();
                        if (base2.HasTechTree())
                        {
                            base2._techTree.OnReceiveActiveNodes(cabin.get_tech_list());
                        }
                        if (base2 is CabinAvatarEnhanceDataItem)
                        {
                            (base2 as CabinAvatarEnhanceDataItem).SetAvatarClassType(key);
                        }
                    }
                }
                Singleton<MiHoYoGameData>.Instance.Save();
                Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
            }
            return false;
        }

        private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_is_all())
                {
                    this._ventureDict.Clear();
                    this._dispatchAvatarIdSet.Clear();
                }
                foreach (IslandVenture venture in rsp.get_venture_list())
                {
                    <OnGetIslandVentureRsp>c__AnonStoreyCD ycd = new <OnGetIslandVentureRsp>c__AnonStoreyCD {
                        <>f__this = this,
                        ventureId = (int) venture.get_id()
                    };
                    if (!this._ventureDict.ContainsKey(ycd.ventureId))
                    {
                        VentureDataItem item = new VentureDataItem(ycd.ventureId);
                        this._ventureDict[ycd.ventureId] = item;
                    }
                    this._ventureDict[ycd.ventureId].SetEndTime(venture.get_end_time());
                    this._dispatchAvatarIdSet.RemoveWhere(new Predicate<int>(ycd.<>m__CF));
                    this._ventureDict[ycd.ventureId].SetDispatchAvatarList(venture.get_avatar_id());
                    foreach (uint num in venture.get_avatar_id())
                    {
                        this._dispatchAvatarIdSet.Add((int) num);
                    }
                }
                this._ventureRefreshTimes = (int) rsp.get_refresh_times();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x9d:
                    return this.OnGetIsLandRsp(pkt.getData<GetIslandRsp>());

                case 0xa5:
                    return this.OnAddCabinTechRsp(pkt.getData<AddCabinTechRsp>());

                case 0xa9:
                    return this.OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());

                case 170:
                    return this.OnDelIslandVentureNotify(pkt.getData<DelIslandVentureNotify>());

                case 0xb8:
                    return this.OnGetCollectCabinRsp(pkt.getData<GetCollectCabinRsp>());
            }
            return false;
        }

        public void OnPlayerLevelChanged(int newLevel, int oldLevel)
        {
            Dictionary<CabinType, bool> cabinNeedToShowNewUnlockDict = Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict;
            foreach (KeyValuePair<CabinType, CabinDataItemBase> pair in this._cabinDict)
            {
                if (!cabinNeedToShowNewUnlockDict.ContainsKey(pair.Key) && ((pair.Value.GetUnlockPlayerLevel() > oldLevel) && (pair.Value.GetUnlockPlayerLevel() <= newLevel)))
                {
                    cabinNeedToShowNewUnlockDict.Add(pair.Key, true);
                }
            }
        }

        public bool RefreshVentureBackground()
        {
            return (this._venture_inprogress_background && (TimeUtil.Now > this._venture_endtime_background));
        }

        public void RegisterVentureInProgress()
        {
            this._venture_inprogress_background = false;
            this._venture_endtime_background = DateTime.MaxValue;
            foreach (KeyValuePair<int, VentureDataItem> pair in this._ventureDict)
            {
                if ((pair.Value.status == VentureDataItem.VentureStatus.InProgress) && (pair.Value.endTime < this._venture_endtime_background))
                {
                    this._venture_endtime_background = pair.Value.endTime;
                    this._venture_inprogress_background = true;
                }
            }
        }

        public void UnRegisterVentureInProgress()
        {
            this._venture_inprogress_background = false;
            this._venture_endtime_background = DateTime.MaxValue;
        }

        public int VentureRefreshTimes
        {
            get
            {
                return this._ventureRefreshTimes;
            }
        }

        [CompilerGenerated]
        private sealed class <OnDelIslandVentureNotify>c__AnonStoreyCE
        {
            internal IslandModule <>f__this;
            internal uint id;

            internal bool <>m__D0(int x)
            {
                return this.<>f__this._ventureDict[(int) this.id].dispatchAvatarIdList.Contains(x);
            }
        }

        [CompilerGenerated]
        private sealed class <OnGetIslandVentureRsp>c__AnonStoreyCD
        {
            internal IslandModule <>f__this;
            internal int ventureId;

            internal bool <>m__CF(int x)
            {
                return this.<>f__this._ventureDict[this.ventureId].dispatchAvatarIdList.Contains(x);
            }
        }
    }
}

