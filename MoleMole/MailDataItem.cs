namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class MailDataItem
    {
        public MailAttachment attachment;
        protected bool isAttachmentGot;
        public DateTime time;

        public MailDataItem(Mail mail)
        {
            MailAttachment attachment;
            if ((mail == null) || (mail.get_attachment() == null))
            {
                attachment = null;
            }
            else if (((mail.get_attachment().get_item_list() == null) || (mail.get_attachment().get_item_list().Count == 0)) && ((mail.get_attachment().get_hcoin() == 0) && (mail.get_attachment().get_scoin() == 0)))
            {
                attachment = null;
            }
            else
            {
                attachment = new MailAttachment();
                if (mail.get_attachment().get_hcoin() > 0)
                {
                    RewardUIData hcoinData = RewardUIData.GetHcoinData((int) mail.get_attachment().get_hcoin());
                    attachment.itemList.Add(hcoinData);
                }
                if (mail.get_attachment().get_scoin() > 0)
                {
                    RewardUIData scoinData = RewardUIData.GetScoinData((int) mail.get_attachment().get_scoin());
                    attachment.itemList.Add(scoinData);
                }
                foreach (MailItem item in mail.get_attachment().get_item_list())
                {
                    RewardUIData data3 = new RewardUIData(ResourceType.Item, (int) item.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) item.get_item_id(), (int) item.get_level());
                    attachment.itemList.Add(data3);
                }
            }
            bool isAttachmentGot = mail.get_is_attachment_gotSpecified() && mail.get_is_attachment_got();
            this.Init((int) mail.get_id(), mail.get_type(), mail.get_title(), mail.get_content(), mail.get_sender(), mail.get_time(), attachment, isAttachmentGot);
        }

        public static int CompareToTimeDesc(MailDataItem lobj, MailDataItem robj)
        {
            if (lobj.hasAttachment && !robj.hasAttachment)
            {
                return -1;
            }
            if (!lobj.hasAttachment && robj.hasAttachment)
            {
                return 1;
            }
            if (!Singleton<MailModule>.Instance.IsMailRead(lobj) && Singleton<MailModule>.Instance.IsMailRead(robj))
            {
                return -1;
            }
            if (Singleton<MailModule>.Instance.IsMailRead(lobj) && !Singleton<MailModule>.Instance.IsMailRead(robj))
            {
                return 1;
            }
            return robj.time.CompareTo(lobj.time);
        }

        public KeyValuePair<MailType, int> GetKeyForMail()
        {
            return new KeyValuePair<MailType, int>(this.type, this.ID);
        }

        public MailCacheKey GetKeyForMailCache()
        {
            return new MailCacheKey(this.type, this.ID, this.time);
        }

        private void Init(int id, MailType type, string title, string content, string sender, uint time, MailAttachment attachment = null, bool isAttachmentGot = false)
        {
            this.ID = id;
            this.type = type;
            this.title = title;
            this.content = content;
            this.sender = sender;
            this.time = Miscs.GetDateTimeFromTimeStamp(time);
            this.attachment = attachment;
            this.isAttachmentGot = isAttachmentGot;
        }

        public string content { get; private set; }

        public bool hasAttachment
        {
            get
            {
                return ((this.attachment != null) && (this.attachment.itemList.Count > 0));
            }
        }

        public int ID { get; private set; }

        public string sender { get; private set; }

        public string title { get; private set; }

        public MailType type { get; private set; }
    }
}

