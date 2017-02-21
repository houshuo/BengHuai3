namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class MailOverviewPageContext : BasePageContext
    {
        private SequenceDialogManager _dialogManager;
        private Dictionary<int, RectTransform> _dictBeforeFetch;
        private List<MailDataItem> _mailList;
        private MonoGridScroller _scroller;
        private MonoScrollerFadeManager _scrollerFadeInManager;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache6;

        public MailOverviewPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MailOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Mail/MailOverviewPage"
            };
            base.config = pattern;
            this._mailList = new List<MailDataItem>();
        }

        public override void BackPage()
        {
            Singleton<MailModule>.Instance.SetAllMailAsOld();
            base.BackPage();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("InfoPanel/GetAllBtn").GetComponent<Button>(), new UnityAction(this.OnGetAllBtnClick));
        }

        private void ClearUnlockAvatarDialogs()
        {
            this._dialogManager.ClearDialogs();
        }

        public override void Destroy()
        {
            Singleton<MailModule>.Instance.SetAllMailAsOld();
            base.Destroy();
        }

        private bool IsMailEqual(RectTransform mailNew, RectTransform mailOld)
        {
            if ((mailNew == null) || (mailOld == null))
            {
                return false;
            }
            MonoMailInfoRow component = mailOld.GetComponent<MonoMailInfoRow>();
            return (mailNew.GetComponent<MonoMailInfoRow>().GetMailCacheKey() == component.GetMailCacheKey());
        }

        private void OnGetAllBtnClick()
        {
            if (!Singleton<MailModule>.Instance.HasAttachmentMail())
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_NoAttachmentMailHint", new object[0]), 2f), UIType.Any);
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestGetAllMailAttachment();
            }
        }

        private void OnMailCheckBtnClick(MailDataItem mailData)
        {
            Singleton<MailModule>.Instance.SetMailAsOld(mailData);
            if (mailData.type == 3)
            {
                Singleton<MailModule>.Instance.SetMailRead(mailData);
            }
            this.SetupView();
            Singleton<MainUIManager>.Instance.ShowDialog(new MailDetailDialogContext(mailData), UIType.Any);
        }

        private void OnMailGetBtnClick(MailDataItem mailData)
        {
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = entry => entry.Key;
            }
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = entry => entry.Value;
            }
            this._dictBeforeFetch = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(this._scroller.GetItemDict(), <>f__am$cache5, <>f__am$cache6);
            Singleton<NetworkManager>.Instance.RequestGetOneMailAttachment(mailData);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.UnlockAvatar)
            {
                int body = (int) ntf.body;
                AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(body, false);
                this._dialogManager.AddDialog(dialogContext);
                if (!this._dialogManager.IsPlaying())
                {
                    this._dialogManager.StartShow(0f);
                }
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x57:
                case 0x55:
                    this.SetupView();
                    break;
            }
            return false;
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            if ((index >= 0) && (index < this._mailList.Count))
            {
                MailDataItem mailData = this._mailList[index];
                trans.GetComponent<MonoMailInfoRow>().SetupView(mailData, new Action<MailDataItem>(this.OnMailCheckBtnClick), new Action<MailDataItem>(this.OnMailGetBtnClick));
            }
        }

        protected override bool SetupView()
        {
            this._mailList = Singleton<MailModule>.Instance.GetAllMails();
            this._scroller = base.view.transform.Find("MailListPanel/ScrollView").GetComponent<MonoGridScroller>();
            this._scroller.Init(new MonoGridScroller.OnChange(this.OnScrollerChange), this._mailList.Count, null);
            this._scrollerFadeInManager = base.view.transform.Find("MailListPanel/ScrollView").GetComponent<MonoScrollerFadeManager>();
            this._scrollerFadeInManager.Init(this._scroller.GetItemDict(), this._dictBeforeFetch, new Func<RectTransform, RectTransform, bool>(this.IsMailEqual));
            this._dictBeforeFetch = null;
            this._scrollerFadeInManager.Play();
            base.view.transform.Find("InfoPanel/MailNum/Num").GetComponent<Text>().text = this._mailList.Count.ToString();
            this._dialogManager = new SequenceDialogManager(new Action(this.ClearUnlockAvatarDialogs));
            return false;
        }
    }
}

