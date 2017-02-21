namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class SignInDialogContext : BaseSequenceDialogContext
    {
        private int _dayNum;
        private int _daysOfMonth;
        private int _monthNum;
        private List<RewardData> _signInRewardItemList;
        private GetSignInRewardStatusRsp _signInRewardStatus;

        public SignInDialogContext(GetSignInRewardStatusRsp rsp)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SignInDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SignInDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this._signInRewardStatus = rsp;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/GetRewardBtn").GetComponent<Button>(), new UnityAction(this.OnGetRewardBtnClick));
        }

        private void OnGetRewardBtnClick()
        {
            if ((this._signInRewardStatus != null) && this._signInRewardStatus.get_is_need_sign_in())
            {
                Singleton<NetworkManager>.Instance.RequestGetSignInReward();
            }
        }

        private bool OnGetSignInRewardRsp(GetSignInRewardRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                SignInRewardGotDialogContext dialogContext = new SignInRewardGotDialogContext(rsp, (int) this._signInRewardStatus.get_next_sign_in_day());
                dialogContext.RegisterCallBack(new SignInRewardGotDialogContext.OnDialogDestroy(this.OnRewardGotConfirm));
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                Singleton<NetworkManager>.Instance.RequestGetSignInRewardStatus();
            }
            else
            {
                this.Destroy();
            }
            return false;
        }

        private bool OnGetSignInRewardStatusRsp(GetSignInRewardStatusRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._signInRewardStatus = rsp;
                if (rsp.get_is_need_sign_in())
                {
                    this.SetupTheRewardPanel();
                }
            }
            return false;
        }

        private void OnItemClick(RewardData rewardData)
        {
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x7a:
                    return this.OnGetSignInRewardStatusRsp(pkt.getData<GetSignInRewardStatusRsp>());

                case 0x7c:
                    return this.OnGetSignInRewardRsp(pkt.getData<GetSignInRewardRsp>());
            }
            return false;
        }

        private void OnRewardGotConfirm()
        {
            this.Destroy();
        }

        private void OnScrollerChanged(Transform trans, int index)
        {
            RewardData rewardData = this._signInRewardItemList[index];
            MonoSignInRewardItemIconButton component = trans.GetComponent<MonoSignInRewardItemIconButton>();
            component.SetupView(rewardData, (index + 1) < this._signInRewardStatus.get_next_sign_in_day(), (index + 1) == this._signInRewardStatus.get_next_sign_in_day());
            component.SetClickCallback(new MonoSignInRewardItemIconButton.ClickCallBack(this.OnItemClick));
        }

        private void SetupTheRewardPanel()
        {
            if (this._signInRewardStatus == null)
            {
                LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x7a, null);
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            }
            else
            {
                base.view.transform.Find("Dialog/Content/MonthPanel").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/GetRewardBtn").gameObject.SetActive(true);
                MonoGridScroller component = base.view.transform.Find("Dialog/Content/MonthPanel/ScrollView").GetComponent<MonoGridScroller>();
                int num = ((this._daysOfMonth % 6) != 0) ? ((this._daysOfMonth / 6) + 1) : (this._daysOfMonth / 6);
                int num2 = (int) ((this._signInRewardStatus.get_next_sign_in_day() - 1) / 6);
                component.Init(new MonoGridScroller.OnChange(this.OnScrollerChanged), this._signInRewardItemList.Count, new Vector2(0f, 1f - (((float) num2) / ((float) num))));
            }
        }

        protected override bool SetupView()
        {
            DateTime now = TimeUtil.Now;
            this._dayNum = (int) this._signInRewardStatus.get_next_sign_in_day();
            this._monthNum = now.Month;
            this._daysOfMonth = DateTime.DaysInMonth(now.Year, now.Month);
            this._signInRewardItemList = new List<RewardData>();
            if ((this._monthNum % 2) == 0)
            {
                List<EvenSignInRewardMetaData> itemList = EvenSignInRewardMetaDataReader.GetItemList();
                for (int i = 0; i < itemList.Count; i++)
                {
                    if (i >= this._daysOfMonth)
                    {
                        break;
                    }
                    EvenSignInRewardMetaData data = itemList[i];
                    this._signInRewardItemList.Add(RewardDataReader.GetRewardDataByKey(data.rewardItemID));
                }
            }
            else
            {
                List<OddSignInRewardMetaData> list2 = OddSignInRewardMetaDataReader.GetItemList();
                for (int j = 0; j < list2.Count; j++)
                {
                    if (j >= this._daysOfMonth)
                    {
                        break;
                    }
                    OddSignInRewardMetaData data2 = list2[j];
                    this._signInRewardItemList.Add(RewardDataReader.GetRewardDataByKey(data2.rewardItemID));
                }
            }
            base.view.transform.Find("Dialog/Content/Title/Month").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.MonthTextIDList[this._monthNum], new object[0]);
            base.view.transform.Find("Dialog/Content/Title/DayNum").GetComponent<Text>().text = this._dayNum.ToString();
            base.view.transform.Find("Dialog/Content/MonthPanel").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/GetRewardBtn").gameObject.SetActive(false);
            if ((this._signInRewardStatus == null) || (this._signInRewardStatus.get_retcode() != null))
            {
                LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x7a, null);
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            }
            else
            {
                this.SetupTheRewardPanel();
            }
            return false;
        }
    }
}

