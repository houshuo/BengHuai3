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

    public class RedeemDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private string _errorCode;
        private string _redeemCode;
        private GetRedeemCodeInfoRsp _redeemInfo;
        private List<RewardUIData> _redeemRewardList;
        private RedeemStatus _redeemStatus;
        private const string DROP_ITEM_ANIMATION_NAME = "DropItemScale10";

        public RedeemDialogContext(string redeemCode, RedeemStatus status, GetRedeemCodeInfoRsp redeemInfo = null, string errorCode = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "RedeemDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/RedeemDialog"
            };
            base.config = pattern;
            this._redeemCode = redeemCode;
            this._redeemStatus = status;
            this._redeemInfo = redeemInfo;
            this._errorCode = errorCode;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Error/BackBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Info/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Info/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Complete/BackBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        private void HideRewardTransSomePart(Transform rewardTrans)
        {
            rewardTrans.Find("BG/UnidentifyText").gameObject.SetActive(false);
            rewardTrans.Find("NewMark").gameObject.SetActive(false);
            rewardTrans.Find("AvatarStar").gameObject.SetActive(false);
            rewardTrans.Find("Star").gameObject.SetActive(false);
            rewardTrans.Find("StigmataType").gameObject.SetActive(false);
            rewardTrans.Find("FragmentIcon").gameObject.SetActive(false);
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnDropItemButtonClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        private bool OnExchangeRedeemCodeRsp(ExchangeRedeemCodeRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.SetupRedeemSuccess();
            }
            else
            {
                this._errorCode = LocalizationGeneralLogic.GetNetworkErrCodeOutput((ExchangeRedeemCodeRsp.Retcode) 1, new object[0]) + '\n';
                this._errorCode = this._errorCode + LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                this.SetupErrorContext();
            }
            return false;
        }

        public void OnOKButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestExchangeRedeemCode(this._redeemCode);
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            if (pkt.getCmdId() == 0xd6)
            {
                this.OnExchangeRedeemCodeRsp(pkt.getData<ExchangeRedeemCodeRsp>());
            }
            return false;
        }

        private void OnScrollChange(Transform trans, int index)
        {
            RewardUIData data = this._redeemRewardList[index];
            if (data.rewardType == ResourceType.Item)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, data.level);
                dummyStorageDataItem.number = data.value;
                trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnDropItemButtonClick), true, false, false, false);
            }
            else
            {
                this.HideRewardTransSomePart(trans);
                trans.GetComponent<MonoLevelDropIconButton>().Clear();
                trans.Find("ItemIcon/ItemIcon/Icon").GetComponent<Image>().sprite = data.GetIconSprite();
                trans.Find("BG/Desc").GetComponent<Text>().text = "x" + data.value.ToString();
            }
            trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale10";
        }

        private void SetupErrorContext()
        {
            base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(true);
            base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Error/DescText").GetComponent<Text>().text = this._errorCode;
        }

        private void SetupRedeemInfo()
        {
            base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(true);
            base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(false);
            Transform transform = base.view.transform.Find("Dialog/Content/Info");
            transform.Find("SubTitle").GetComponent<Text>().text = this._redeemInfo.get_desc();
            this.SetupRewardList();
            transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollChange), this._redeemRewardList.Count, new Vector2(0f, 0f));
            this._animationManager = new SequenceAnimationManager(null, null);
            this._animationManager.AddAllChildrenInTransform(transform.Find("ScrollView/Content"));
            this._animationManager.StartPlay(0.1f, true);
        }

        private void SetupRedeemSuccess()
        {
            base.view.transform.Find("Dialog/Content/Error").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Info").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Complete").gameObject.SetActive(true);
        }

        private void SetupRewardList()
        {
            this._redeemRewardList = new List<RewardUIData>();
            if (this._redeemInfo.get_reward_list().Count >= 1)
            {
                RewardData data = this._redeemInfo.get_reward_list()[0];
                if (data.get_exp() > 0)
                {
                    RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) data.get_exp());
                    this._redeemRewardList.Add(playerExpData);
                }
                if (data.get_scoin() > 0)
                {
                    RewardUIData scoinData = RewardUIData.GetScoinData((int) data.get_scoin());
                    this._redeemRewardList.Add(scoinData);
                }
                if (data.get_hcoin() > 0)
                {
                    RewardUIData hcoinData = RewardUIData.GetHcoinData((int) data.get_hcoin());
                    this._redeemRewardList.Add(hcoinData);
                }
                if (data.get_stamina() > 0)
                {
                    RewardUIData staminaData = RewardUIData.GetStaminaData((int) data.get_stamina());
                    this._redeemRewardList.Add(staminaData);
                }
                if (data.get_skill_point() > 0)
                {
                    RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) data.get_skill_point());
                    this._redeemRewardList.Add(skillPointData);
                }
                if (data.get_friends_point() > 0)
                {
                    RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) data.get_friends_point());
                    this._redeemRewardList.Add(friendPointData);
                }
                foreach (RewardItemData data8 in data.get_item_list())
                {
                    RewardUIData item = new RewardUIData(ResourceType.Item, (int) data8.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data8.get_id(), (int) data8.get_level());
                    this._redeemRewardList.Add(item);
                }
            }
        }

        protected override bool SetupView()
        {
            switch (this._redeemStatus)
            {
                case RedeemStatus.Error:
                    this.SetupErrorContext();
                    break;

                case RedeemStatus.ShowInfo:
                    this.SetupRedeemInfo();
                    break;

                case RedeemStatus.RedeemSuccess:
                    this.SetupRedeemSuccess();
                    break;
            }
            return false;
        }

        public enum RedeemStatus
        {
            Error,
            ShowInfo,
            RedeemSuccess
        }
    }
}

