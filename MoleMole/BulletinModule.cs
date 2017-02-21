namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class BulletinModule : BaseModule
    {
        private Dictionary<uint, Bulletin> _allBulletinDict;
        public DateTime LastCheckBulletinTime;
        private const uint SHOW_TYPE_EVENT = 0;
        private const uint SHOW_TYPE_SYSTEM = 1;

        public BulletinModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this.EventBulletinList = new List<Bulletin>();
            this.SystemBulletinList = new List<Bulletin>();
            this._allBulletinDict = new Dictionary<uint, Bulletin>();
        }

        public bool HasNewBulletins()
        {
            return (this.HasNewBulletinsByType(0) || this.HasNewBulletinsByType(1));
        }

        public bool HasNewBulletinsByType(uint type)
        {
            <HasNewBulletinsByType>c__AnonStoreyC5 yc = new <HasNewBulletinsByType>c__AnonStoreyC5 {
                cacheSet = Singleton<MiHoYoGameData>.Instance.LocalData.OldBulletinIDSet
            };
            if (type == 0)
            {
                return this.EventBulletinList.Exists(new Predicate<Bulletin>(yc.<>m__BB));
            }
            return ((type == 1) && this.SystemBulletinList.Exists(new Predicate<Bulletin>(yc.<>m__BC)));
        }

        private bool OnGetBulletinRsp(GetBulletinRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.EventBulletinList.Clear();
                this.SystemBulletinList.Clear();
                this._allBulletinDict.Clear();
                foreach (Bulletin bulletin in rsp.get_bulletin_list())
                {
                    if (bulletin.get_type() == 0)
                    {
                        this.EventBulletinList.Add(bulletin);
                    }
                    else if (bulletin.get_type() == 1)
                    {
                        this.SystemBulletinList.Add(bulletin);
                    }
                    this._allBulletinDict[bulletin.get_id()] = bulletin;
                }
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x8a:
                    return this.OnGetBulletinRsp(pkt.getData<GetBulletinRsp>());

                case 0xbb:
                    return this.OnUrgencyMsgNotify(pkt.getData<UrgencyMsgNotify>());
            }
            return false;
        }

        private bool OnUrgencyMsgNotify(UrgencyMsgNotify rsp)
        {
            if (Singleton<MainUIManager>.Instance != null)
            {
                Singleton<MainUIManager>.Instance.ShowWidget(new AnnouncementDialogContext(rsp.get_msg()), UIType.Any);
            }
            return false;
        }

        public void SetBulletinsOldByShowType(uint type)
        {
            HashSet<uint> oldBulletinIDSet = Singleton<MiHoYoGameData>.Instance.LocalData.OldBulletinIDSet;
            if (type == 0)
            {
                foreach (Bulletin bulletin in this.EventBulletinList)
                {
                    if (!oldBulletinIDSet.Contains(bulletin.get_id()))
                    {
                        oldBulletinIDSet.Add(bulletin.get_id());
                    }
                }
            }
            else if (type == 1)
            {
                foreach (Bulletin bulletin2 in this.SystemBulletinList)
                {
                    if (!oldBulletinIDSet.Contains(bulletin2.get_id()))
                    {
                        oldBulletinIDSet.Add(bulletin2.get_id());
                    }
                }
            }
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public Bulletin TryGetBulletinByID(uint id)
        {
            Bulletin bulletin;
            this._allBulletinDict.TryGetValue(id, out bulletin);
            return bulletin;
        }

        public List<Bulletin> EventBulletinList { get; private set; }

        public List<Bulletin> SystemBulletinList { get; private set; }

        [CompilerGenerated]
        private sealed class <HasNewBulletinsByType>c__AnonStoreyC5
        {
            internal HashSet<uint> cacheSet;

            internal bool <>m__BB(Bulletin x)
            {
                return !this.cacheSet.Contains(x.get_id());
            }

            internal bool <>m__BC(Bulletin x)
            {
                return !this.cacheSet.Contains(x.get_id());
            }
        }
    }
}

