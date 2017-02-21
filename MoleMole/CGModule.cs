namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class CGModule : BaseModule
    {
        private List<int> _finishCGList;
        public static int BASE_CG_ID = 0x9c40;

        public CGModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._finishCGList = new List<int>();
        }

        public List<int> GetAllCGIDList()
        {
            List<int> list = new List<int>();
            foreach (CgMetaData data in CgMetaDataReader.GetItemList())
            {
                list.Add(data.CgID);
            }
            return list;
        }

        public CgDataItem GetCgDataItem(int cgId)
        {
            CgMetaData cgMetaData = CgMetaDataReader.TryGetCgMetaDataByKey(cgId);
            if (cgMetaData != null)
            {
                return new CgDataItem(cgMetaData);
            }
            return null;
        }

        public List<CgDataItem> GetCgDataItemList()
        {
            List<CgDataItem> list = new List<CgDataItem>();
            foreach (CgMetaData data in CgMetaDataReader.GetItemList())
            {
                list.Add(new CgDataItem(data));
            }
            return list;
        }

        public List<CgDataItem> GetFinishedCgDataItemList()
        {
            List<CgDataItem> list = new List<CgDataItem>();
            foreach (CgMetaData data in CgMetaDataReader.GetItemList())
            {
                if ((data != null) && this._finishCGList.Contains(data.CgID))
                {
                    list.Add(new CgDataItem(data));
                }
            }
            return list;
        }

        public List<int> GetFinishedCGIDList()
        {
            List<int> list = new List<int>();
            List<CgMetaData> list3 = CgMetaDataReader.GetItemList().FindAll(x => this._finishCGList.Contains(x.CgID));
            if (list3 != null)
            {
                foreach (CgMetaData data in list3)
                {
                    list.Add(data.CgID);
                }
            }
            return list;
        }

        public List<int> GetUnFinishedCGIDList()
        {
            List<int> list = new List<int>();
            foreach (CgMetaData data in CgMetaDataReader.GetItemList().FindAll(x => !this._finishCGList.Contains(x.CgID)))
            {
                list.Add(data.CgID);
            }
            return list;
        }

        public List<int> GetUnFinishedCGIDList(int levelID)
        {
            List<int> list = new List<int>();
            if (levelID != 0)
            {
                foreach (CgMetaData data in CgMetaDataReader.GetItemList().FindAll(x => !this._finishCGList.Contains(x.CgID)))
                {
                    if (!list.Contains(data.CgID) && (data.levelID == levelID))
                    {
                        list.Add(data.CgID);
                    }
                }
            }
            return list;
        }

        public bool IsCGFinished(int plotID)
        {
            return this._finishCGList.Contains(plotID);
        }

        private bool IsLevelPlotID(int cgID)
        {
            return (cgID > BASE_CG_ID);
        }

        public void MarkCGIDFinish(int cgID)
        {
            if (!this._finishCGList.Contains(cgID))
            {
                Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) cgID, true);
                this.UpdateFinishCGID(cgID);
            }
        }

        private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishCGID((int) num);
                }
            }
            return false;
        }

        private bool OnGetFinishGuideDataRsp(GetFinishGuideDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishCGID((int) num);
                }
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x80:
                    return this.OnGetFinishGuideDataRsp(pkt.getData<GetFinishGuideDataRsp>());

                case 130:
                    return this.OnFinishGuideReportRsp(pkt.getData<FinishGuideReportRsp>());
            }
            return false;
        }

        private void UpdateFinishCGID(int cgID)
        {
            if (!this._finishCGList.Contains(cgID) && this.IsLevelPlotID(cgID))
            {
                this._finishCGList.Add(cgID);
            }
        }
    }
}

