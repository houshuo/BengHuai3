namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class SearchFriendDialogContext : BaseDialogContext
    {
        public SearchFriendDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SearchFriendDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SearchFriendDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnOKButtonCallBack()
        {
            int num;
            if (int.TryParse(base.view.transform.Find("Dialog/Content/SearchID/InputField").GetComponent<InputField>().text, out num))
            {
                if (num == Singleton<PlayerModule>.Instance.playerData.userId)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_SearchSelf", new object[0]), 2f), UIType.Any);
                }
                else if (Singleton<FriendModule>.Instance.IsMyFriend(num))
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_IsFriend", new object[0]), 2f), UIType.Any);
                }
                else
                {
                    FriendDetailDataItem playerInfo = Singleton<FriendModule>.Instance.TryGetFriendDetailData(num);
                    if (playerInfo == null)
                    {
                        Singleton<NetworkManager>.Instance.RequestFriendDetailInfo(num);
                    }
                    else
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new SearchedFriendDetailDialogContext(playerInfo, false), UIType.Any);
                        this.Close();
                    }
                }
            }
            else
            {
                base.view.transform.Find("Dialog/Content/ErrHintText").gameObject.SetActive(true);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            if (pkt.getCmdId() == 0x49)
            {
                GetPlayerDetailDataRsp rsp = pkt.getData<GetPlayerDetailDataRsp>();
                this.OnSearchFriendDetailInfoRsp(rsp);
            }
            return false;
        }

        private bool OnSearchFriendDetailInfoRsp(GetPlayerDetailDataRsp rsp)
        {
            switch (rsp.get_retcode())
            {
                case 0:
                    Singleton<MainUIManager>.Instance.ShowDialog(new SearchedFriendDetailDialogContext(new FriendDetailDataItem(rsp.get_detail()), false), UIType.Any);
                    this.Close();
                    break;

                case 1:
                case 2:
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_TargetNotExist", new object[0]), 2f), UIType.Any);
                    break;
            }
            return false;
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/ErrHintText").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/SelfID/SelfIDNum").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
            return false;
        }
    }
}

