namespace MoleMole
{
    using proto;
    using System;

    public class GachaModule : BaseModule
    {
        private GachaDisplayInfo _gachaDisplayInfo;
        private DateTime _lastCheckGachaInfoTime;

        private GachaModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._gachaDisplayInfo = null;
            this._lastCheckGachaInfoTime = DateTime.MinValue;
        }

        public bool IsGachaInfoValid()
        {
            return (TimeUtil.Now < this._lastCheckGachaInfoTime.AddSeconds((double) MiscData.Config.BasicConfig.CheckGachaInfoIntervalSecond));
        }

        public bool OnGachaRsp(GachaRsp rsp)
        {
            if ((rsp.get_retcode() == null) && !this.IsGachaInfoValid())
            {
                Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
            }
            return false;
        }

        private bool OnGetGachaDisplayRsp(GetGachaDisplayRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (this._gachaDisplayInfo == null)
                {
                    this._gachaDisplayInfo = new GachaDisplayInfo();
                }
                this._gachaDisplayInfo.hcoinGachaData = rsp.get_hcoin_gacha_data();
                this._gachaDisplayInfo.specialGachaData = (rsp.get_special_hcoin_gacha_data_list().Count <= 0) ? null : rsp.get_special_hcoin_gacha_data_list()[0];
                this._gachaDisplayInfo.friendPointGachaData = rsp.get_friends_point_gacha_data();
                this._lastCheckGachaInfoTime = TimeUtil.Now;
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x3f:
                    return this.OnGetGachaDisplayRsp(pkt.getData<GetGachaDisplayRsp>());

                case 0x3b:
                    return this.OnGachaRsp(pkt.getData<GachaRsp>());
            }
            return false;
        }

        public GachaDisplayInfo GachaDisplay
        {
            get
            {
                if (this.IsGachaInfoValid())
                {
                    return this._gachaDisplayInfo;
                }
                return null;
            }
        }
    }
}

