namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine.Events;

    public class ResetLevelDialogContext : BaseDialogContext
    {
        private readonly LevelDataItem _levelData;
        private LevelDetailDialogContextV2 _parentDialog;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache2;

        public ResetLevelDialogContext(LevelDataItem levelData, LevelDetailDialogContextV2 parentDialog)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ResetLevelDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ResetLevelDialog"
            };
            base.config = pattern;
            this._levelData = levelData;
            this._parentDialog = parentDialog;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), new UnityAction(this.OnGiveUpButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Title/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnCloseButtonCallBack));
        }

        public void OnCloseButtonCallBack()
        {
            this.Destroy();
        }

        public void OnGiveUpButtonCallBack()
        {
            this.Destroy();
        }

        public void OnOKButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestStageEnterTimes((uint) this._levelData.levelId);
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x6d) && this.OnStageEnterTimesRsp(pkt.getData<ResetStageEnterTimesRsp>()));
        }

        private bool OnStageEnterTimesRsp(ResetStageEnterTimesRsp rsp)
        {
            GeneralDialogContext context;
            if (rsp.get_retcode() == 2)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge", new object[0])
                };
                if (<>f__am$cache2 == null)
                {
                    <>f__am$cache2 = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
                        }
                    };
                }
                context.buttonCallBack = <>f__am$cache2;
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (rsp.get_retcode() == 3)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ResetStageEnterFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (rsp.get_retcode() == null)
            {
                this._parentDialog.RefreshChallengeNumber(null);
            }
            this.Destroy();
            return false;
        }

        protected override bool SetupView()
        {
            bool flag = LevelResetCostMetaDataReader.TryGetLevelResetCostMetaDataByKey(this._levelData.resetTimes + 1) == null;
            int num = !flag ? this._levelData.GetHCoinSpentToResetLevel(this._levelData.resetTimes + 1) : 0;
            string stageName = this._levelData.StageName;
            int maxResetTimes = this._levelData.MaxResetTimes;
            int num3 = this._levelData.MaxResetTimes - this._levelData.resetTimes;
            int hcoin = Singleton<PlayerModule>.Instance.playerData.hcoin;
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/InfoAvatar/HCoinNumText").GetComponent<Text>().text = !flag ? num.ToString() : "??";
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/InfoAvatar/LevelName").GetComponent<Text>().text = stageName;
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/ReviveTimes/AvailableTimes").GetComponent<Text>().text = num3.ToString();
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/ReviveTimes/AvailableTimes/MaxTimes").GetComponent<Text>().text = maxResetTimes.ToString();
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/Consume/Hcoin/Num").GetComponent<Text>().text = hcoin.ToString();
            return false;
        }
    }
}

