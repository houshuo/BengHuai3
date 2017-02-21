namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using UnityEngine.Events;

    public class InLevelReviveConfirmDialogContext : BaseDialogContext
    {
        private string _avatarFullname;
        private int _hcoinCost;
        private LevelActor _levelActor;
        private LevelScoreManager _levelScoreManager;
        private InLevelReviveDialogContext _reviveContext;

        public InLevelReviveConfirmDialogContext(InLevelReviveDialogContext reviveContext, int hcoinCost, string avatarFullName)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelReviveConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/InLevelReviveConfirmDialog"
            };
            base.config = pattern;
            this._reviveContext = reviveContext;
            this._hcoinCost = hcoinCost;
            this._avatarFullname = avatarFullName;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), new UnityAction(this.OnOkBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
        }

        public bool OnAvatarReviveRsp(AvatarReviveRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                if (rsp.get_revive_timesSpecified())
                {
                    int num = this._levelScoreManager.maxReviveNum - ((int) rsp.get_revive_times());
                    if (this._levelScoreManager.avaiableReviveNum > num)
                    {
                        this._levelScoreManager.avaiableReviveNum = num;
                        this._levelActor.ReviveAvatarByID(this._reviveContext.avatarRuntimeID, this._reviveContext.revivePosition);
                        this.Destroy();
                        Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off", null, null, null);
                        this._reviveContext.OnReviveConfirm();
                    }
                }
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void OnBGBtnClick()
        {
            this._reviveContext.view.SetActive(true);
            this.Destroy();
        }

        private void OnOkBtnClick()
        {
            LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x6b, null) {
                ignoreMaxWaitTime = true
            };
            Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            Singleton<NetworkManager>.Instance.RequestAvatarRevive(false);
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x6b) && this.OnAvatarReviveRsp(pkt.getData<AvatarReviveRsp>()));
        }

        protected override bool SetupView()
        {
            this._levelScoreManager = Singleton<LevelScoreManager>.Instance;
            this._levelActor = Singleton<LevelManager>.Instance.levelActor;
            this._reviveContext.view.SetActive(false);
            base.view.transform.Find("Dialog/Content/InfoPanel/InfoAvatar/HcoinNum").GetComponent<Text>().text = this._hcoinCost.ToString();
            base.view.transform.Find("Dialog/Content/InfoPanel/InfoAvatar/AvatarFullName").GetComponent<Text>().text = this._avatarFullname;
            return false;
        }
    }
}

