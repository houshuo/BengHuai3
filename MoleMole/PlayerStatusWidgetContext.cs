namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class PlayerStatusWidgetContext : BaseWidgetContext
    {
        private SequenceDialogManager _achieveDialogManager;
        private CanvasTimer _timer;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache3;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache4;
        private bool teamProfileActive = true;

        public PlayerStatusWidgetContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "PlayerStatusWidgetContext",
                viewPrefabPath = "UI/Menus/Widget/PlayerStatusPanel",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.findViewSavedInScene = true;
            base.uiType = UIType.SuspendBar;
            this._achieveDialogManager = new SequenceDialogManager(new Action(this.<PlayerStatusWidgetContext>m__1AD));
        }

        [CompilerGenerated]
        private void <PlayerStatusWidgetContext>m__1AD()
        {
            this._achieveDialogManager.ClearDialogs();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TeamBriefPanel/Button").GetComponent<Button>(), new UnityAction(this.OnTeamBriefPanelClick));
            base.BindViewCallback(base.view.transform.Find("RightPanel/SCoin").GetComponent<Button>(), new UnityAction(this.OnSCoinPanelClick));
            base.BindViewCallback(base.view.transform.Find("RightPanel/HCoin").GetComponent<Button>(), new UnityAction(this.OnHCoinPanelClick));
            base.BindViewCallback(base.view.transform.Find("RightPanel/Stamina").GetComponent<Button>(), new UnityAction(this.OnStaminaPanelPanelClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/BackBtn").GetComponent<Button>(), new UnityAction(this.OnBackBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionBtns/MainMenuBtn").GetComponent<Button>(), new UnityAction(this.OnMainMenuBtnClick));
            if (GlobalVars.LEVEL_MODE_DEBUG)
            {
                base.BindViewCallback(base.view.transform.Find("GMTalkButton").GetComponent<Button>(), new UnityAction(this.OnGMTalkBtnClick));
            }
        }

        private void CheckPlayerData()
        {
            if (!Singleton<PlayerModule>.Instance.playerData.initByGetMainDataRsp)
            {
                Singleton<NetworkManager>.Instance.RequestGetAllMainData();
            }
            else if (!Singleton<PlayerModule>.Instance.playerData.IsStaminaFull() && (Singleton<PlayerModule>.Instance.playerData.nextStaminaRecoverDatetime < TimeUtil.Now))
            {
                Singleton<NetworkManager>.Instance.RequestGetStaminaRecoverLeftTime();
            }
            else if (!Singleton<PlayerModule>.Instance.playerData.IsSkillPointFull() && (Singleton<PlayerModule>.Instance.playerData.nextSkillPtRecoverDatetime < TimeUtil.Now))
            {
                Singleton<NetworkManager>.Instance.RequestGetSkillPointRecoverLeftTime();
            }
        }

        [DebuggerHidden]
        private IEnumerator DelayShowAchieveUnlockCoroutine()
        {
            return new <DelayShowAchieveUnlockCoroutine>c__Iterator6B { <>f__this = this };
        }

        public override void Destroy()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            base.Destroy();
        }

        public void HideAllButBackBtn()
        {
            base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(false);
            base.view.transform.Find("RightPanel").gameObject.SetActive(false);
            base.view.transform.Find("ActionBtns/MainMenuBtn").gameObject.SetActive(false);
            base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
            base.view.transform.Find("ActionBtns/BackBtn").gameObject.SetActive(true);
        }

        public void OnBackBtnClick()
        {
            Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
        }

        public void OnGMTalkBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new GMTalkDialogContext(), UIType.Any);
        }

        public void OnHCoinPanelClick()
        {
            if (!(Singleton<MainUIManager>.Instance.CurrentPageContext is RechargePageContext))
            {
                Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
            }
        }

        public void OnMainMenuBtnClick()
        {
            Singleton<MainUIManager>.Instance.CurrentPageContext.BackToMainMenuPage();
        }

        private bool OnMissionUpdated(uint id)
        {
            base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice").gameObject.SetActive(this.PopupNoticeAcitve());
            MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem((int) id);
            if (missionDataItem != null)
            {
                LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey((int) id);
                if ((linearMissionDataByKey == null) || (linearMissionDataByKey.IsAchievement == 0))
                {
                    return false;
                }
                if (missionDataItem.status == 3)
                {
                    this._achieveDialogManager.AddDialog(new AchieveUnlockContext((int) id));
                    if (!this._achieveDialogManager.IsPlaying())
                    {
                        Singleton<ApplicationManager>.Instance.StartCoroutine(this.DelayShowAchieveUnlockCoroutine());
                    }
                }
            }
            return false;
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.ShowScoinExchangeInfo)
            {
                return this.ShowSCoinExchangeDialog();
            }
            if (ntf.type == NotifyTypes.ShowStaminaExchangeInfo)
            {
                return this.ShowStaminaExchangeDialog();
            }
            if (ntf.type == NotifyTypes.SetBackButtonActive)
            {
                return this.OnSetBackButtonActive((bool) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetPlayerStatusWidgetDisplay)
            {
                return this.OnSetPlayerStatusWidgetDisplay((bool) ntf.body);
            }
            return ((ntf.type == NotifyTypes.MissionUpdated) && this.OnMissionUpdated((uint) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 11:
                    return this.SetupView();

                case 15:
                    return this.OnScoinExchangeRsp(pkt.getData<ScoinExchangeRsp>());

                case 0x13:
                    return this.OnStaminaExchangeRsp(pkt.getData<StaminaExchangeRsp>());
            }
            return false;
        }

        public bool OnScoinExchangeRsp(ScoinExchangeRsp rsp)
        {
            GeneralDialogContext context;
            if (rsp.get_retcode() == null)
            {
                object[] objArray1 = new object[] { MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Hcoin", new object[0])), " - ", rsp.get_hcoin_cost(), " ， ", MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Scoin", new object[0])), " + ", rsp.get_scoin_get() };
                string str = string.Concat(objArray1);
                if (rsp.get_boost_rateSpecified() && (rsp.get_boost_rate() > 100))
                {
                    string str2 = str;
                    string[] textArray1 = new string[] { str2, " ， ", MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Desc_Critical", new object[0])), " \x00d7 ", string.Format("{0:0%}", ((float) rsp.get_boost_rate()) / 100f) };
                    str = string.Concat(textArray1);
                }
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc", new object[0]),
                    desc = str
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (rsp.get_retcode() == 2)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge", new object[0])
                };
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
                        }
                    };
                }
                context.buttonCallBack = <>f__am$cache3;
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        public void OnSCoinPanelClick()
        {
            if (Singleton<PlayerModule>.Instance.playerData.scoinExchangeCache.Value != null)
            {
                this.ShowSCoinExchangeDialog();
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestGetScoinExchangeInfo();
            }
        }

        public bool OnSetBackButtonActive(bool active)
        {
            this.teamProfileActive = !active;
            base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(this.teamProfileActive);
            base.view.transform.Find("ActionBtns").gameObject.SetActive(!this.teamProfileActive);
            return false;
        }

        public bool OnSetPlayerStatusWidgetDisplay(bool show)
        {
            base.view.gameObject.SetActive(show);
            return false;
        }

        public bool OnStaminaExchangeRsp(StaminaExchangeRsp rsp)
        {
            GeneralDialogContext context;
            if (rsp.get_retcode() == null)
            {
                object[] objArray1 = new object[] { MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Hcoin", new object[0])), "-", rsp.get_hcoin_cost(), " ， ", MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Stamina", new object[0])), "+", rsp.get_stamina_get() };
                string str = string.Concat(objArray1);
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeSucc", new object[0]),
                    desc = str
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else if (rsp.get_retcode() == 2)
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_GoToRechargeDesc", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_GoToRecharge", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_GiveUpRecharge", new object[0])
                };
                if (<>f__am$cache4 == null)
                {
                    <>f__am$cache4 = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<MainUIManager>.Instance.ShowPage(new RechargePageContext("RechargeTab"), UIType.Page);
                        }
                    };
                }
                context.buttonCallBack = <>f__am$cache4;
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            else
            {
                context = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_ExchangeFail", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
            }
            return false;
        }

        public void OnStaminaPanelPanelClick()
        {
            Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo);
        }

        public void OnTeamBriefPanelClick()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new PlayerProfilePageContext(PlayerProfilePageContext.TabType.PlayerTab), UIType.Page);
        }

        private bool PopupNoticeAcitve()
        {
            foreach (MissionDataItem item in Singleton<MissionModule>.Instance.GetMissionDict().Values)
            {
                LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(item.id);
                if (((linearMissionDataByKey != null) && (linearMissionDataByKey.IsAchievement == 1)) && (item.status == 3))
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetView()
        {
            this.SetupView();
        }

        public void SetPopupVisible(bool visible)
        {
            Transform transform = base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice");
            if (transform != null)
            {
                transform.gameObject.SetActive(visible);
            }
            base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Normal").gameObject.SetActive(false);
        }

        protected override bool SetupView()
        {
            PlayerDataItem playerData = Singleton<PlayerModule>.Instance.playerData;
            base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(true);
            base.view.transform.Find("RightPanel").gameObject.SetActive(true);
            base.view.transform.Find("ActionBtns/MainMenuBtn").gameObject.SetActive(true);
            base.view.transform.Find("TeamBriefPanel").gameObject.SetActive(this.teamProfileActive);
            base.view.transform.Find("TeamBriefPanel/NickName/NicknameText").GetComponent<Text>().text = playerData.NickNameText;
            base.view.transform.Find("TeamBriefPanel/LevelText").GetComponent<Text>().text = "LV." + playerData.teamLevel;
            base.view.transform.Find("TeamBriefPanel/NickName/PopUp/Notice").gameObject.SetActive(this.PopupNoticeAcitve());
            base.view.transform.Find("TeamBriefPanel/TeamExpSlider").GetComponent<MonoSliderGroup>().UpdateValue((float) playerData.teamExp, (float) playerData.TeamMaxExp, 0f);
            base.view.transform.Find("RightPanel/SCoin/NumText").GetComponent<Text>().text = string.Empty + playerData.scoin;
            base.view.transform.Find("RightPanel/HCoin/NumText").GetComponent<Text>().text = string.Empty + playerData.hcoin;
            base.view.transform.Find("RightPanel/Stamina/NumText").GetComponent<Text>().text = string.Empty + playerData.stamina;
            base.view.transform.Find("RightPanel/Stamina/MaxNumText").GetComponent<Text>().text = string.Empty + playerData.MaxStamina;
            base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.LEVEL_MODE_DEBUG);
            base.view.transform.Find("GMTalkButton").gameObject.SetActive(GlobalVars.DEBUG_FEATURE_ON);
            return false;
        }

        public bool ShowSCoinExchangeDialog()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new SCoinExchangeDialogContext(), UIType.Any);
            return false;
        }

        public bool ShowStaminaExchangeDialog()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange"), UIType.Any);
            return false;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateInfiniteTimer(1f);
            this._timer.timeTriggerCallback = new Action(this.CheckPlayerData);
        }

        [CompilerGenerated]
        private sealed class <DelayShowAchieveUnlockCoroutine>c__Iterator6B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal PlayerStatusWidgetContext <>f__this;
            internal BasePageContext <pageContext>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(0.8f);
                        this.$PC = 1;
                        goto Label_00D2;

                    case 1:
                    case 2:
                        this.<pageContext>__0 = Singleton<MainUIManager>.Instance.CurrentPageContext;
                        if (((this.<pageContext>__0 != null) && !(this.<pageContext>__0 is GachaMainPageContext)) && (!(this.<pageContext>__0 is GachaResultPageContext) && (this.<pageContext>__0.dialogContextList.Count <= 0)))
                        {
                            this.<>f__this._achieveDialogManager.StartShow(0f);
                            this.$PC = -1;
                            break;
                        }
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_00D2;
                }
                return false;
            Label_00D2:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

