namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class LevelTutorialModule : BaseModule
    {
        private List<int> _finishTutorialList;
        public static int BASE_LEVEL_TUTORIAL_ID = 0x2710;

        public LevelTutorialModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._finishTutorialList = new List<int>();
        }

        public List<int> GetUnFinishedTutorialIDList(int levelID)
        {
            List<int> list = new List<int>();
            if (levelID != 0)
            {
                foreach (LevelTutorialMetaData data in LevelTutorialMetaDataReader.GetItemList().FindAll(x => !this._finishTutorialList.Contains(x.tutorialId)))
                {
                    if (!list.Contains(data.tutorialId) && (data.levelId == levelID))
                    {
                        list.Add(data.tutorialId);
                    }
                }
            }
            return list;
        }

        private bool IsLevelTutorialID(int tutorialID)
        {
            return (tutorialID > BASE_LEVEL_TUTORIAL_ID);
        }

        public bool IsTutorialIDFinish(int tutorialID)
        {
            return this._finishTutorialList.Contains(tutorialID);
        }

        public void MarkTutorialIDFinish(int tutorialID)
        {
            Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) tutorialID, true);
            this.UpdateFinishTutorialID(tutorialID);
        }

        private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishTutorialID((int) num);
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
                    this.UpdateFinishTutorialID((int) num);
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

        private void UpdateFinishTutorialID(int tutorialID)
        {
            if (!this._finishTutorialList.Contains(tutorialID) && this.IsLevelTutorialID(tutorialID))
            {
                this._finishTutorialList.Add(tutorialID);
            }
        }
    }
}

