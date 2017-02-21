namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class LevelPlotModule : BaseModule
    {
        private List<int> _finishPlotList;
        public static int BASE_LEVEL_PLOT_ID = 0x4e20;

        public LevelPlotModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._finishPlotList = new List<int>();
        }

        public List<int> GetUnFinishedPlotIDList(int levelID)
        {
            List<int> list = new List<int>();
            if (levelID != 0)
            {
                foreach (PlotMetaData data in PlotMetaDataReader.GetItemList().FindAll(x => !this._finishPlotList.Contains(x.plotID)))
                {
                    if (!list.Contains(data.plotID) && (data.levelID == levelID))
                    {
                        list.Add(data.plotID);
                    }
                }
            }
            return list;
        }

        private bool IsLevelPlotID(int tutorialID)
        {
            return (tutorialID > BASE_LEVEL_PLOT_ID);
        }

        public bool IsPlotFinished(int plotID)
        {
            return this._finishPlotList.Contains(plotID);
        }

        public void MarkPlotIDFinish(int tutorialID)
        {
            Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) tutorialID, true);
            this.UpdateFinishPlotID(tutorialID);
        }

        private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishPlotID((int) num);
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
                    this.UpdateFinishPlotID((int) num);
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

        private void UpdateFinishPlotID(int tutorialID)
        {
            if (!this._finishPlotList.Contains(tutorialID) && this.IsLevelPlotID(tutorialID))
            {
                this._finishPlotList.Add(tutorialID);
            }
        }
    }
}

