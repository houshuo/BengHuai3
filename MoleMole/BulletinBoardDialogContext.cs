namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class BulletinBoardDialogContext : BaseSequenceDialogContext
    {
        private Dictionary<ShowType, uint> _selectIdDict;
        private List<Bulletin> _showList;
        private ShowType _showType;

        public BulletinBoardDialogContext(ShowType showType = 0)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "BulletinBoardDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/BulletinBoard/BulletinBoardDialog"
            };
            base.config = pattern;
            this._showType = showType;
            this._selectIdDict = new Dictionary<ShowType, uint>();
            this._selectIdDict.Add(ShowType.ShowEvent, 0);
            this._selectIdDict.Add(ShowType.ShowSystem, 0);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/EventBtn").GetComponent<Button>(), new UnityAction(this.ShowEventBulletinList));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/SysBtn").GetComponent<Button>(), new UnityAction(this.ShowSystemBulletinList));
        }

        public void Close()
        {
            SuperDebug.VeryImportantAssert(base.view != null, "view is null!");
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        private bool OnDownloadResAssetSucc()
        {
            this.ShowBulletinListByType(this._showType);
            return false;
        }

        private bool OnGetBulletinRsp(GetBulletinRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.ShowBulletinListByType(this._showType);
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.DownloadResAssetSucc) && this.OnDownloadResAssetSucc());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x8a) && this.OnGetBulletinRsp(pkt.getData<GetBulletinRsp>()));
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            bool isSelected = this._showList[index].get_id() == this._selectIdDict[this._showType];
            trans.GetComponent<MonoBulletinTitleButton>().SetupView(this._showList[index], isSelected, new MoleMole.ShowBulletinById(this.ShowBulletinById));
        }

        private void SetActiveTabBtn(bool active, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
        }

        private void SetupPopUp()
        {
            bool flag = Singleton<BulletinModule>.Instance.HasNewBulletinsByType(0);
            base.view.transform.Find("Dialog/TabBtns/EventBtn/PopUp").gameObject.SetActive(flag);
            bool flag2 = Singleton<BulletinModule>.Instance.HasNewBulletinsByType(1);
            base.view.transform.Find("Dialog/TabBtns/SysBtn/PopUp").gameObject.SetActive(flag2);
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(false);
            this.ShowBulletinListByType(this._showType);
            return false;
        }

        private void ShowBulletinById(uint id)
        {
            Bulletin bulletin = Singleton<BulletinModule>.Instance.TryGetBulletinByID(id);
            if ((bulletin != null) && (((ShowType) bulletin.get_type()) == this._showType))
            {
                this._selectIdDict[this._showType] = id;
                base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").GetComponent<MonoGridScroller>().RefreshCurrent();
                Transform transform = base.view.transform.Find("Dialog/Content/OneNotice/ScrollView/Content");
                string str = bulletin.get_banner_path();
                Image component = transform.Find("Image/Pics").GetComponent<Image>();
                bool flag = !string.IsNullOrEmpty(str) && UIUtil.TrySetupEventSprite(component, str);
                transform.Find("Image").gameObject.SetActive(flag);
                if (flag)
                {
                    LayoutElement element = transform.Find("Image").GetComponent<LayoutElement>();
                    Rect rect = transform.Find("Image/Pics").GetComponent<Image>().sprite.rect;
                    element.preferredHeight = rect.height;
                    element.preferredWidth = rect.width;
                }
                transform.Find("Title/Text").GetComponent<Text>().text = bulletin.get_title();
                if (bulletin.get_event_date_str() == string.Empty)
                {
                    transform.Find("Title/Time").gameObject.SetActive(false);
                }
                else
                {
                    transform.Find("Title/Time").gameObject.SetActive(true);
                    transform.Find("Title/Time").GetComponent<Text>().text = bulletin.get_event_date_str();
                }
                transform.Find("Body").GetComponent<MonoBulletinBody>().SetupView(UIUtil.ProcessStrWithNewLine(bulletin.get_content()));
                base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
            }
        }

        private void ShowBulletinListByType(ShowType type)
        {
            this._showType = type;
            this._showList = (type != ShowType.ShowEvent) ? Singleton<BulletinModule>.Instance.SystemBulletinList : Singleton<BulletinModule>.Instance.EventBulletinList;
            base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollerChange), this._showList.Count, null);
            if (this._showList.Count > 0)
            {
                base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(true);
                this.ShowBulletinById((this._selectIdDict[this._showType] != 0) ? this._selectIdDict[this._showType] : this._showList[0].get_id());
            }
            else
            {
                base.view.transform.Find("Dialog/Content/OneNotice/ScrollView").gameObject.SetActive(false);
                base.view.transform.Find("Dialog/Content/TitleBtnList/ScrollView").gameObject.SetActive(false);
            }
            this.SetActiveTabBtn(type == ShowType.ShowEvent, base.view.transform.Find("Dialog/TabBtns/EventBtn").GetComponent<Button>());
            this.SetActiveTabBtn(type == ShowType.ShowSystem, base.view.transform.Find("Dialog/TabBtns/SysBtn").GetComponent<Button>());
            if (Singleton<BulletinModule>.Instance.HasNewBulletinsByType((uint) this._showType))
            {
                Singleton<BulletinModule>.Instance.SetBulletinsOldByShowType((uint) this._showType);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BulletinPopUpUpdate, null));
            }
            this.SetupPopUp();
        }

        private void ShowEventBulletinList()
        {
            this.ShowBulletinListByType(ShowType.ShowEvent);
        }

        private void ShowSystemBulletinList()
        {
            this.ShowBulletinListByType(ShowType.ShowSystem);
        }

        public enum ShowType
        {
            ShowEvent,
            ShowSystem
        }
    }
}

