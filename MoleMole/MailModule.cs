namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class MailModule : BaseModule
    {
        private Dictionary<KeyValuePair<MailType, int>, MailDataItem> _mailDict;

        public MailModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._mailDict = new Dictionary<KeyValuePair<MailType, int>, MailDataItem>();
        }

        public List<MailDataItem> GetAllMails()
        {
            List<MailDataItem> list = Enumerable.ToList<MailDataItem>(this._mailDict.Values);
            list.Sort(new Comparison<MailDataItem>(MailDataItem.CompareToTimeDesc));
            return list;
        }

        public bool HasAttachmentMail()
        {
            foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> pair in this._mailDict)
            {
                if (pair.Value.hasAttachment)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasNewMail()
        {
            List<MailCacheKey> oldMailCache = Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache;
            foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> pair in this._mailDict)
            {
                <HasNewMail>c__AnonStoreyD3 yd = new <HasNewMail>c__AnonStoreyD3 {
                    key = pair.Value.GetKeyForMailCache()
                };
                if (oldMailCache.Find(new Predicate<MailCacheKey>(yd.<>m__E1)) == null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsMailNew(MailDataItem mail)
        {
            <IsMailNew>c__AnonStoreyD2 yd = new <IsMailNew>c__AnonStoreyD2();
            List<MailCacheKey> oldMailCache = Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache;
            yd.key = mail.GetKeyForMailCache();
            return (oldMailCache.Find(new Predicate<MailCacheKey>(yd.<>m__E0)) == null);
        }

        public bool IsMailRead(MailDataItem mailData)
        {
            <IsMailRead>c__AnonStoreyD1 yd = new <IsMailRead>c__AnonStoreyD1 {
                key = mailData.GetKeyForMailCache()
            };
            return (Singleton<MiHoYoGameData>.Instance.LocalData.ReadMailIdList.Find(new Predicate<MailCacheKey>(yd.<>m__DF)) != null);
        }

        private bool OnGetMailAttachmentRsp(GetMailAttachmentRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (MailKey key in rsp.get_succ_mail_key_list())
                {
                    this._mailDict.Remove(new KeyValuePair<MailType, int>(key.get_type(), (int) key.get_id()));
                }
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            }
            return false;
        }

        private bool OnGetMailDataRsp(GetMailDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (Mail mail in rsp.get_mail_list())
                {
                    MailDataItem item = new MailDataItem(mail);
                    this._mailDict[item.GetKeyForMail()] = item;
                }
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x55:
                    return this.OnGetMailDataRsp(pkt.getData<GetMailDataRsp>());

                case 0x57:
                    return this.OnGetMailAttachmentRsp(pkt.getData<GetMailAttachmentRsp>());
            }
            return false;
        }

        public void SetAllMailAsOld()
        {
            foreach (KeyValuePair<KeyValuePair<MailType, int>, MailDataItem> pair in this._mailDict)
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache.Add(pair.Value.GetKeyForMailCache());
            }
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public void SetMailAsOld(MailDataItem mail)
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.OldMailCache.Add(mail.GetKeyForMailCache());
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public void SetMailRead(MailDataItem mailData)
        {
            if (!this.IsMailRead(mailData) && this._mailDict.ContainsKey(mailData.GetKeyForMail()))
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.ReadMailIdList.Add(mailData.GetKeyForMailCache());
                Singleton<MiHoYoGameData>.Instance.Save();
            }
        }

        public MailDataItem TryGetMailData(MailType mailType, int mailID)
        {
            MailDataItem item;
            this._mailDict.TryGetValue(new KeyValuePair<MailType, int>(mailType, mailID), out item);
            return item;
        }

        [CompilerGenerated]
        private sealed class <HasNewMail>c__AnonStoreyD3
        {
            internal MailCacheKey key;

            internal bool <>m__E1(MailCacheKey x)
            {
                return (((x.type == this.key.type) && (x.id == this.key.id)) && (x.time == this.key.time));
            }
        }

        [CompilerGenerated]
        private sealed class <IsMailNew>c__AnonStoreyD2
        {
            internal MailCacheKey key;

            internal bool <>m__E0(MailCacheKey x)
            {
                return (((x.type == this.key.type) && (x.id == this.key.id)) && (x.time == this.key.time));
            }
        }

        [CompilerGenerated]
        private sealed class <IsMailRead>c__AnonStoreyD1
        {
            internal MailCacheKey key;

            internal bool <>m__DF(MailCacheKey x)
            {
                return (((x.type == this.key.type) && (x.id == this.key.id)) && (x.time == this.key.time));
            }
        }
    }
}

