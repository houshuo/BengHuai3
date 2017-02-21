namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;

    public class CommonIDModule : BaseModule
    {
        private List<int> _finishCommonIDList;
        public static int APP_STORE_COMMENT_ID_1 = 0x1389;
        public static int APP_STORE_COMMENT_ID_2 = 0x138a;
        public static int BASE_COMMON_ID = 0xc350;

        public CommonIDModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._finishCommonIDList = new List<int>();
        }

        public bool IsCommonFinished(int commonID)
        {
            return this._finishCommonIDList.Contains(commonID);
        }

        public void MarkCommonIDFinish(int commonID)
        {
            if (!this._finishCommonIDList.Contains(commonID))
            {
                Singleton<NetworkManager>.Instance.RequestFinishGuideReport((uint) commonID, true);
                this.UpdateFinishCommonID(commonID);
            }
        }

        private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishCommonID((int) num);
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
                    this.UpdateFinishCommonID((int) num);
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

        private void UpdateFinishCommonID(int commonID)
        {
            if (!this._finishCommonIDList.Contains(commonID))
            {
                this._finishCommonIDList.Add(commonID);
            }
        }
    }
}

