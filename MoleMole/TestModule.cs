namespace MoleMole
{
    using proto;
    using System;

    public class TestModule : BaseModule
    {
        private TestModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
        }

        private bool OnGmTalkRsp(GmTalkRsp rsp)
        {
            if (((rsp.get_retcode() == null) && rsp.get_msgSpecified()) && rsp.get_msg().Equals("CLEAR ALL", StringComparison.OrdinalIgnoreCase))
            {
                MiHoYoGameData.DeleteAllData();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x17) && this.OnGmTalkRsp(pkt.getData<GmTalkRsp>()));
        }

        public void RequestGMTalk(string msg_param)
        {
            GmTalkReq data = new GmTalkReq();
            data.set_msg(msg_param);
            Singleton<NetworkManager>.Instance.SendPacket<GmTalkReq>(data);
        }
    }
}

