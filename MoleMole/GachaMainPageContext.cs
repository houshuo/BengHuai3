namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class GachaMainPageContext : BasePageContext
    {
        private int[] _avatarCardIDs = new int[] { 0xeac5, 0xeac6, 0xeac7, 0xeac8, 0xeb29, 0xeb2a, 0xeb2b, 0xeb2c, 0xeb8d, 0xeb8e, 0xeb8f, 0xeb90 };
        private int _cost;
        private GachaType _currentGachaType;
        private string _currentTabKey = "HcoinTab";
        private GachaDisplayInfo _displayInfo;
        private SequenceDialogManager _dropItemShowDialogManager;
        private List<StorageDataItemBase> _gachaGotList;
        private List<GachaItem> _gachaItemList;
        private TabManager _tabManager;
        private SequenceDialogManager _unLockAvatarDialogManager;
        private WaitGachaRsp _waitGachaRsp;
        public const string FRIEND_TAB = "FriendTab";
        public const string HCOIN_TAB = "HcoinTab";
        public const string SPECIAL_TAB = "SpecialTab";

        public GachaMainPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GachaMainPageContext",
                viewPrefabPath = "UI/Menus/Page/Gacha/GachaMainPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._gachaGotList = new List<StorageDataItemBase>();
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
            this._waitGachaRsp = new WaitGachaRsp();
        }

        private void BeginDropAnimationAfterBoxAnimation()
        {
            Singleton<MainUIManager>.Instance.SceneCanvas.gameObject.SetActive(true);
            this._dropItemShowDialogManager.StartShow(0f);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnHcoinTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnSpecialTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), new UnityAction(this.OnFriendTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("HCoinTab/ActBtns/One/Btn").GetComponent<Button>(), new UnityAction(this.OnHCoinOneGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn").GetComponent<Button>(), new UnityAction(this.OnHcoinTenGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("SpecialTab/ActBtns/One/Btn").GetComponent<Button>(), new UnityAction(this.OnSpecialOneGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn").GetComponent<Button>(), new UnityAction(this.OnSpecialTenGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("FriendPointTab/ActBtns/One/Btn").GetComponent<Button>(), new UnityAction(this.OnFriendOneGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn").GetComponent<Button>(), new UnityAction(this.OnFriendTenGachaBtnClick));
            base.BindViewCallback(base.view.transform.Find("HCoinTab/SupplyImg/Pic").GetComponent<Button>(), new UnityAction(this.OnHcoinDetailBtnClick));
            base.BindViewCallback(base.view.transform.Find("SpecialTab/SupplyImg/Pic").GetComponent<Button>(), new UnityAction(this.OnSpecialDetailBtnClick));
        }

        private void ClearUnlockAvatarDialogManagerContent()
        {
            if (this._unLockAvatarDialogManager != null)
            {
                this._unLockAvatarDialogManager.ClearDialogs();
            }
        }

        private bool FriendConditionCheck(int needFriendPoint)
        {
            if (this.IsStorageFull())
            {
                return false;
            }
            if (Singleton<PlayerModule>.Instance.playerData.friendsPoint < needFriendPoint)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    desc = LocalizationGeneralLogic.GetText("Err_FriendPointShortage", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                return false;
            }
            return true;
        }

        private int GetMaxFriendPointGachaTime(out int cost)
        {
            int friendsPoint = Singleton<PlayerModule>.Instance.playerData.friendsPoint;
            int num2 = (int) this._displayInfo.friendPointGachaData.get_friends_point_cost();
            int num3 = friendsPoint / num2;
            if (num3 > 10)
            {
                num3 = 10;
            }
            else if (num3 < 2)
            {
                num3 = 2;
            }
            cost = num2 * num3;
            return num3;
        }

        private void HackGachaItems(List<GachaItem> itemList)
        {
            itemList.Clear();
            for (int i = 0; i < this._avatarCardIDs.Length; i++)
            {
                GachaItem item = new GachaItem();
                item.set_item_id((uint) this._avatarCardIDs[i]);
                itemList.Add(item);
            }
        }

        private bool IsStorageFull()
        {
            if (Singleton<StorageModule>.Instance.IsFull())
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new ClearStorageHintDialog(1.5f), UIType.Any);
                return true;
            }
            return false;
        }

        private void OnFriendOneGachaBtnClick()
        {
            this._currentGachaType = 1;
            int needFriendPoint = (int) this._displayInfo.friendPointGachaData.get_friends_point_cost();
            if (this.FriendConditionCheck(needFriendPoint))
            {
                this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaOne, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, 1);
                this._cost = needFriendPoint;
            }
        }

        private void OnFriendTabBtnClick()
        {
            this._currentTabKey = "FriendTab";
            this._tabManager.ShowTab(this._currentTabKey);
        }

        private void OnFriendTenGachaBtnClick()
        {
            int num;
            this._currentGachaType = 1;
            int maxFriendPointGachaTime = this.GetMaxFriendPointGachaTime(out num);
            if (this.FriendConditionCheck(num))
            {
                this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaTen, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, maxFriendPointGachaTime);
                this._cost = num;
            }
        }

        private void OnGachaDisplayDataExpired()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint", new object[0]), 2f), UIType.Any);
            Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
        }

        public bool OnGachaDisplayInfoGot(GetGachaDisplayRsp rsp)
        {
            this.SetupView();
            base.TryToDoTutorial();
            return false;
        }

        private bool OnGachaRsp(GachaRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._gachaGotList.Clear();
                this._gachaItemList = rsp.get_item_list();
                this._dropItemShowDialogManager = new SequenceDialogManager(new Action(this.ShowGachaResultPage));
                this._unLockAvatarDialogManager = new SequenceDialogManager(new Action(this.ClearUnlockAvatarDialogManagerContent));
                foreach (GachaItem item in rsp.get_item_list())
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), (int) item.get_level());
                    if (dummyStorageDataItem != null)
                    {
                        dummyStorageDataItem.number = (int) item.get_num();
                        if (dummyStorageDataItem is AvatarCardDataItem)
                        {
                            if (item.get_split_fragment_numSpecified())
                            {
                                (dummyStorageDataItem as AvatarCardDataItem).SpliteToFragment((int) item.get_split_fragment_num());
                            }
                            else
                            {
                                AvatarCardDataItem item2 = dummyStorageDataItem as AvatarCardDataItem;
                                AvatarUnlockDialogContext dialogContext = new AvatarUnlockDialogContext(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(item2.ID).avatarID, true);
                                this._unLockAvatarDialogManager.AddDialog(dialogContext);
                            }
                        }
                        this._gachaGotList.Add(dummyStorageDataItem);
                        List<Tuple<StorageDataItemBase, bool>> itemDataList = new List<Tuple<StorageDataItemBase, bool>> {
                            new Tuple<StorageDataItemBase, bool>(dummyStorageDataItem, item.get_is_rare_drop())
                        };
                        if (item.get_gift_item_idSpecified())
                        {
                            StorageDataItemBase base3 = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_gift_item_id(), (int) item.get_gift_level());
                            if (base3 != null)
                            {
                                base3.number = (int) item.get_gift_num();
                                itemDataList.Add(new Tuple<StorageDataItemBase, bool>(base3, false));
                                this._gachaGotList.Add(base3);
                            }
                        }
                        this._dropItemShowDialogManager.AddDialog(new DropNewItemDialogContextV2(itemDataList));
                    }
                }
                this._waitGachaRsp.End();
            }
            else if (rsp.get_retcode() == 3)
            {
                GeneralDialogContext context2 = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Return_GachaTitcketLack", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Return_GachaTitcketLack", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context2, UIType.Any);
            }
            this.UpdateView();
            return false;
        }

        private void OnHcoinDetailBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new GachaDetailPageContext(this._displayInfo.hcoinGachaData.get_common_data(), 2), UIType.Page);
        }

        private void OnHCoinOneGachaBtnClick()
        {
            this._currentGachaType = 2;
            int ticketID = (int) this._displayInfo.hcoinGachaData.get_ticket_material_id();
            if (this.TicketConditionCheck(ticketID, 1))
            {
                this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaOne, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, 1);
                this._cost = 1;
            }
        }

        private void OnHcoinTabBtnClick()
        {
            this._currentTabKey = "HcoinTab";
            this._tabManager.ShowTab(this._currentTabKey);
        }

        private void OnHcoinTenGachaBtnClick()
        {
            this._currentGachaType = 2;
            int ticketID = (int) this._displayInfo.hcoinGachaData.get_ticket_material_id();
            if (this.TicketConditionCheck(ticketID, 10))
            {
                this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaTen, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, 10);
                this._cost = 10;
            }
        }

        public override void OnLandedFromBackPage()
        {
            base.OnLandedFromBackPage();
            bool flag = (this._displayInfo.specialGachaData != null) && (TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(this._displayInfo.specialGachaData.get_data_expire_time()));
            base.view.transform.Find("SpecialTab").gameObject.SetActive(flag && (this._currentTabKey == "SpecialTab"));
            base.view.transform.Find("TabBtns/TabBtn_2").gameObject.SetActive(flag);
            base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
            if ((this._unLockAvatarDialogManager != null) && (this._unLockAvatarDialogManager.GetDialogNum() > 0))
            {
                this._unLockAvatarDialogManager.StartShow(0f);
            }
            foreach (LetterSpacing spacing in base.view.transform.GetComponentsInChildren<LetterSpacing>())
            {
                if (spacing.autoFixLine)
                {
                    spacing.AccommodateText();
                }
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.DownloadResAssetSucc) && this.SetupView());
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x3f:
                    return this.OnGachaDisplayInfoGot(pkt.getData<GetGachaDisplayRsp>());

                case 0x3b:
                    return this.OnGachaRsp(pkt.getData<GachaRsp>());

                case 0xd8:
                    return this.SetupView();
            }
            return false;
        }

        private void OnSpecialDetailBtnClick()
        {
            if ((this._displayInfo.specialGachaData != null) && (TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(this._displayInfo.specialGachaData.get_data_expire_time())))
            {
                Singleton<MainUIManager>.Instance.ShowPage(new GachaDetailPageContext(this._displayInfo.specialGachaData.get_common_data(), 3), UIType.Page);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint", new object[0]), 2f), UIType.Any);
                Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
            }
        }

        private void OnSpecialOneGachaBtnClick()
        {
            this._currentGachaType = 3;
            if ((this._displayInfo.specialGachaData != null) && (TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(this._displayInfo.specialGachaData.get_data_expire_time())))
            {
                int ticketID = (int) this._displayInfo.specialGachaData.get_ticket_material_id();
                if (this.TicketConditionCheck(ticketID, 1))
                {
                    this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaOne, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                    Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, 1);
                    this._cost = 1;
                }
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint", new object[0]), 2f), UIType.Any);
                Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
            }
        }

        private void OnSpecialTabBtnClick()
        {
            this._currentTabKey = "SpecialTab";
            this._tabManager.ShowTab(this._currentTabKey);
        }

        private void OnSpecialTenGachaBtnClick()
        {
            this._currentGachaType = 3;
            if ((this._displayInfo.specialGachaData != null) && (TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(this._displayInfo.specialGachaData.get_data_expire_time())))
            {
                int ticketID = (int) this._displayInfo.specialGachaData.get_ticket_material_id();
                if (this.TicketConditionCheck(ticketID, 10))
                {
                    this._waitGachaRsp.Start(this._currentGachaType, GachaAmountType.GachaTen, new Action<GachaType, GachaAmountType>(this.TriggerStageGachaBox));
                    Singleton<NetworkManager>.Instance.RequestGacha(this._currentGachaType, 10);
                    this._cost = 10;
                }
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_SpecialGachaTimeoutHint", new object[0]), 2f), UIType.Any);
                Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
            }
        }

        private void OnTabSetActive(bool selected, GameObject contentGo, Button tabButton)
        {
            tabButton.interactable = !selected;
            tabButton.transform.Find("Unselect").gameObject.SetActive(!selected);
            tabButton.transform.Find("Select").gameObject.SetActive(selected);
            contentGo.SetActive(selected);
            if (selected)
            {
                foreach (LetterSpacing spacing in contentGo.GetComponentsInChildren<LetterSpacing>())
                {
                    if (spacing.autoFixLine)
                    {
                        spacing.AccommodateText();
                    }
                }
            }
        }

        private void SetupFriendTab()
        {
            int num;
            FriendsPointGachaData friendPointGachaData = this._displayInfo.friendPointGachaData;
            base.view.transform.Find("FriendPointTab/InfoPanel/Title/Time").GetComponent<Text>().text = friendPointGachaData.get_common_data().get_title();
            base.view.transform.Find("FriendPointTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(friendPointGachaData.get_common_data().get_content());
            base.view.transform.Find("FriendPointTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + friendPointGachaData.get_friends_point_cost();
            int maxFriendPointGachaTime = this.GetMaxFriendPointGachaTime(out num);
            base.view.transform.Find("FriendPointTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1], new object[0]);
            base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[maxFriendPointGachaTime], new object[0]);
            base.view.transform.Find("FriendPointTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + num;
            base.view.transform.Find("FriendPointTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = maxFriendPointGachaTime.ToString();
            base.view.transform.Find("FriendPointTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FriendGachaNote", new object[0]);
            base.view.transform.Find("FriendPointTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_FriendGachaNote", new object[0]);
            base.view.transform.Find("FriendPointTab/FriendPointRemain/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.friendsPoint.ToString();
            UIUtil.TrySetupEventSprite(base.view.transform.Find("FriendPointTab/SupplyImg/Pic").GetComponent<Image>(), friendPointGachaData.get_common_data().get_supply_image());
            UIUtil.TrySetupEventSprite(base.view.transform.Find("FriendPointTab/InfoPanel/Title/Image").GetComponent<Image>(), friendPointGachaData.get_common_data().get_title_image());
            this._tabManager.SetTab("FriendTab", base.view.transform.Find("TabBtns/TabBtn_3").GetComponent<Button>(), base.view.transform.Find("FriendPointTab").gameObject);
        }

        private void SetupHcoinTab()
        {
            HcoinGachaData hcoinGachaData = this._displayInfo.hcoinGachaData;
            StorageDataItemBase base2 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID((int) hcoinGachaData.get_ticket_material_id());
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) hcoinGachaData.get_ticket_material_id(), 1);
            string gachaTicketIconPath = MiscData.GetGachaTicketIconPath((int) hcoinGachaData.get_ticket_material_id());
            Sprite spriteByPrefab = !string.IsNullOrEmpty(gachaTicketIconPath) ? Miscs.GetSpriteByPrefab(gachaTicketIconPath) : null;
            if (spriteByPrefab == null)
            {
                spriteByPrefab = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
            }
            base.view.transform.Find("HCoinTab/InfoPanel/Title/Time").GetComponent<Text>().text = hcoinGachaData.get_common_data().get_title();
            base.view.transform.Find("HCoinTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(hcoinGachaData.get_common_data().get_content());
            base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1], new object[0]);
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[10], new object[0]);
            base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + 1;
            base.view.transform.Find("HCoinTab/ActBtns/One/Btn/Cost/Icon").GetComponent<Image>().sprite = spriteByPrefab;
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + 10;
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Btn/Cost/Icon").GetComponent<Image>().sprite = spriteByPrefab;
            base.view.transform.Find("HCoinTab/ActBtns/One/Scoin").gameObject.SetActive(false);
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Scoin").gameObject.SetActive(false);
            base.view.transform.Find("HCoinTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_HCoinGachaNote", new object[0]);
            base.view.transform.Find("HCoinTab/ActBtns/One/Added/Num").GetComponent<Text>().text = "2";
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_HCoinGachaNote", new object[0]);
            base.view.transform.Find("HCoinTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = "20";
            base.view.transform.Find("HCoinTab/TicketRemain/Num").GetComponent<Text>().text = (base2 != null) ? base2.number.ToString() : "0";
            base.view.transform.Find("HCoinTab/TicketRemain/Icon").GetComponent<Image>().sprite = spriteByPrefab;
            UIUtil.TrySetupEventSprite(base.view.transform.Find("HCoinTab/SupplyImg/Pic").GetComponent<Image>(), hcoinGachaData.get_common_data().get_supply_image());
            UIUtil.TrySetupEventSprite(base.view.transform.Find("HCoinTab/InfoPanel/Title/Image").GetComponent<Image>(), hcoinGachaData.get_common_data().get_title_image());
            base.view.transform.Find("HCoinTab/RemainTime").gameObject.SetActive(hcoinGachaData.get_data_expire_timeSpecified());
            if (hcoinGachaData.get_data_expire_timeSpecified())
            {
                base.view.transform.Find("HCoinTab/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(Miscs.GetDateTimeFromTimeStamp(hcoinGachaData.get_data_expire_time()), null, new Action(this.OnGachaDisplayDataExpired), false);
            }
            this._tabManager.SetTab("HcoinTab", base.view.transform.Find("TabBtns/TabBtn_1").GetComponent<Button>(), base.view.transform.Find("HCoinTab").gameObject);
        }

        private void SetupSpeicalTab()
        {
            HcoinGachaData specialGachaData = this._displayInfo.specialGachaData;
            bool flag = (specialGachaData != null) && (TimeUtil.Now < Miscs.GetDateTimeFromTimeStamp(specialGachaData.get_data_expire_time()));
            base.view.transform.Find("SpecialTab").gameObject.SetActive(flag);
            base.view.transform.Find("TabBtns/TabBtn_2").gameObject.SetActive(flag);
            if (!flag)
            {
                if (this._currentTabKey == "SpecialTab")
                {
                    this._currentTabKey = "HcoinTab";
                }
            }
            else
            {
                StorageDataItemBase base2 = Singleton<StorageModule>.Instance.TryGetMaterialDataByID((int) specialGachaData.get_ticket_material_id());
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) specialGachaData.get_ticket_material_id(), 1);
                string gachaTicketIconPath = MiscData.GetGachaTicketIconPath((int) specialGachaData.get_ticket_material_id());
                Sprite spriteByPrefab = !string.IsNullOrEmpty(gachaTicketIconPath) ? Miscs.GetSpriteByPrefab(gachaTicketIconPath) : null;
                if (spriteByPrefab == null)
                {
                    spriteByPrefab = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
                }
                base.view.transform.Find("SpecialTab/InfoPanel/Title/Time").GetComponent<Text>().text = specialGachaData.get_common_data().get_title();
                base.view.transform.Find("SpecialTab/InfoPanel/Desc/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(specialGachaData.get_common_data().get_content());
                base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[1], new object[0]);
                base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.GachaTimeTextID[10], new object[0]);
                base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Cost/Num").GetComponent<Text>().text = "x" + 1;
                base.view.transform.Find("SpecialTab/ActBtns/One/Btn/Cost/Icon").GetComponent<Image>().sprite = spriteByPrefab;
                base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Cost/Num").GetComponent<Text>().text = "x" + 10;
                base.view.transform.Find("SpecialTab/ActBtns/Ten/Btn/Cost/Icon").GetComponent<Image>().sprite = spriteByPrefab;
                base.view.transform.Find("SpecialTab/ActBtns/One/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_SpecialGachaNote", new object[0]);
                base.view.transform.Find("SpecialTab/ActBtns/One/Added/Num").GetComponent<Text>().text = "2";
                base.view.transform.Find("SpecialTab/ActBtns/Ten/Added/Note").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_SpecialGachaNote", new object[0]);
                base.view.transform.Find("SpecialTab/ActBtns/Ten/Added/Num").GetComponent<Text>().text = "20";
                base.view.transform.Find("SpecialTab/TicketRemain/Num").GetComponent<Text>().text = (base2 != null) ? base2.number.ToString() : "0";
                base.view.transform.Find("SpecialTab/TicketRemain/Icon").GetComponent<Image>().sprite = spriteByPrefab;
                UIUtil.TrySetupEventSprite(base.view.transform.Find("SpecialTab/SupplyImg/Pic").GetComponent<Image>(), specialGachaData.get_common_data().get_supply_image());
                UIUtil.TrySetupEventSprite(base.view.transform.Find("SpecialTab/InfoPanel/Title/Image").GetComponent<Image>(), specialGachaData.get_common_data().get_title_image());
                base.view.transform.Find("SpecialTab/RemainTime").gameObject.SetActive(specialGachaData.get_data_expire_timeSpecified());
                if (specialGachaData.get_data_expire_timeSpecified())
                {
                    base.view.transform.Find("SpecialTab/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(Miscs.GetDateTimeFromTimeStamp(specialGachaData.get_data_expire_time()), null, new Action(this.OnGachaDisplayDataExpired), false);
                }
                this._tabManager.SetTab("SpecialTab", base.view.transform.Find("TabBtns/TabBtn_2").GetComponent<Button>(), base.view.transform.Find("SpecialTab").gameObject);
            }
        }

        protected override bool SetupView()
        {
            this._displayInfo = Singleton<GachaModule>.Instance.GachaDisplay;
            if (this._displayInfo == null)
            {
                base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
                base.view.transform.Find("HCoinTab").gameObject.SetActive(false);
                base.view.transform.Find("SpecialTab").gameObject.SetActive(false);
                base.view.transform.Find("FriendPointTab").gameObject.SetActive(false);
                base.view.transform.Find("TabBtns").gameObject.SetActive(false);
                Singleton<NetworkManager>.Instance.RequestGachaDisplayInfo();
                return false;
            }
            base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
            this.UpdateView();
            return false;
        }

        private void ShowAppStoreCommentPage()
        {
            if ((this._gachaItemList != null) && (this._gachaItemList.Count > 0))
            {
                bool flag = false;
                foreach (GachaItem item in this._gachaItemList)
                {
                    if (((item != null) && item.get_is_rare_dropSpecified()) && item.get_is_rare_drop())
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    UIUtil.ShowAppStoreComment(CommonIDModule.APP_STORE_COMMENT_ID_1);
                }
            }
        }

        private void ShowGachaResultPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new GachaResultPageContext(this._displayInfo, this._currentGachaType, this._gachaGotList, this._gachaItemList, this._cost), UIType.Page);
        }

        private string SpaceshipNullAssertFailedCallback()
        {
            string[] textArray1 = new string[] { "spaceship is null when TriggerStageGachaBox currentCanvas: ", Singleton<MainUIManager>.Instance.GetMainCanvas().ToString(), " currentPage: ", Singleton<MainUIManager>.Instance.CurrentPageContext.ToString(), " pageStack: ", Singleton<MainUIManager>.Instance.GetAllPageNamesInStack() };
            return string.Concat(textArray1);
        }

        private bool TicketConditionCheck(int ticketID, int num)
        {
            if (!this.IsStorageFull())
            {
                MaterialDataItem item = Singleton<StorageModule>.Instance.TryGetMaterialDataByID(ticketID);
                if ((item != null) && (item.number >= num))
                {
                    return true;
                }
                Singleton<MainUIManager>.Instance.ShowDialog(new GachaTicketLackDialogContext(ticketID, num), UIType.Any);
            }
            return false;
        }

        private void TriggerStageGachaBox(GachaType type, GachaAmountType gachaAmountType)
        {
            base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
            Singleton<MainUIManager>.Instance.SceneCanvas.gameObject.SetActive(false);
            MonoStageGacha component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("Entities/DynamicObject/StageGacha3D", BundleType.RESOURCE_FILE)).GetComponent<MonoStageGacha>();
            GameObject uiCamera = GameObject.Find("UICamera");
            GameObject spaceShipObj = Singleton<MainUIManager>.Instance.GetMainCanvas().GetSpaceShipObj();
            SuperDebug.VeryImportantAssert(spaceShipObj != null, new Func<string>(this.SpaceshipNullAssertFailedCallback));
            component.Init(spaceShipObj, uiCamera, type, gachaAmountType, new Action(this.BeginDropAnimationAfterBoxAnimation));
        }

        private bool UpdateView()
        {
            base.view.transform.Find("TabBtns").gameObject.SetActive(true);
            this.SetupHcoinTab();
            this.SetupSpeicalTab();
            this.SetupFriendTab();
            this._tabManager.ShowTab(this._currentTabKey);
            return false;
        }

        public enum GachaAmountType
        {
            GachaOne,
            GachaTen
        }

        public class WaitGachaRsp
        {
            public GachaMainPageContext.GachaAmountType _amountType;
            public Action<GachaType, GachaMainPageContext.GachaAmountType> _callback;
            public GachaType _gachaType;
            public bool _waiting;

            public void End()
            {
                if (this._waiting)
                {
                    this._waiting = false;
                    this._callback(this._gachaType, this._amountType);
                }
            }

            private void ShowWheel()
            {
                LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x3b, null) {
                    ignoreMaxWaitTime = true
                };
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            }

            public void Start(GachaType gacha, GachaMainPageContext.GachaAmountType amount, Action<GachaType, GachaMainPageContext.GachaAmountType> callback)
            {
                if (!this._waiting)
                {
                    this._waiting = true;
                    this._gachaType = gacha;
                    this._amountType = amount;
                    this._callback = callback;
                    this.ShowWheel();
                }
            }
        }
    }
}

