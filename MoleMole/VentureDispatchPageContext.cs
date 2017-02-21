namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class VentureDispatchPageContext : BasePageContext
    {
        private bool _isConditionMatch;
        private VentureDataItem _ventureData;
        private const int MAX_TEAM_MEMBER_NUM = 3;

        public VentureDispatchPageContext(VentureDataItem ventureData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "VentureDispatchPageContext",
                viewPrefabPath = "UI/Menus/Page/Island/VentureDispatchPage"
            };
            base.config = pattern;
            this._ventureData = ventureData;
        }

        public override void BackToMainMenuPage()
        {
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, false, null, true);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), new UnityAction(this.OnOkButtonCallBack));
        }

        public bool OnDispatchIslandVentureRsp(DispatchIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this.BackPage();
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void OnDropItemButtonClick(StorageDataItemBase dropItemData)
        {
            UIUtil.ShowItemDetail(dropItemData, true, true);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.DispatchAvatarChanged)
            {
                this.SetupMyTeam();
                this.SetupVentureConditaions();
                return false;
            }
            return ((ntf.type == NotifyTypes.ShowStaminaExchangeInfo2) && this.ShowStaminaExchangeDialog());
        }

        public void OnOkButtonCallBack()
        {
            bool flag = this._ventureData.StaminaCost <= Singleton<PlayerModule>.Instance.playerData.stamina;
            bool flag2 = Singleton<IslandModule>.Instance.GetVentureInProgressNum() < (Singleton<IslandModule>.Instance.GetCabinDataByType(5) as CabinVentureDataItem).GetMaxVentureNumInProgress();
            if ((this._isConditionMatch && flag) && flag2)
            {
                Singleton<NetworkManager>.Instance.RequestDispatchIslandVenture(this._ventureData.VentureID, this._ventureData.selectedAvatarList);
            }
            else
            {
                GeneralDialogContext context;
                if (!this._isConditionMatch)
                {
                    context = new GeneralDialogContext {
                        title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConditionNotMatchHint", new object[0])
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (!flag2)
                {
                    context = new GeneralDialogContext {
                        title = LocalizationGeneralLogic.GetText("Menu_Tips", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_Desc_VentureInProgressExceedLimit", new object[0]),
                        type = GeneralDialogContext.ButtonType.SingleButton
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(context, UIType.Any);
                }
                else if (!flag)
                {
                    Singleton<PlayerModule>.Instance.playerData._cacheDataUtil.CheckCacheValidAndGo<PlayerStaminaExchangeInfo>(ECacheData.Stamina, NotifyTypes.ShowStaminaExchangeInfo2);
                }
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0xae) && this.OnDispatchIslandVentureRsp(pkt.getData<DispatchIslandVentureRsp>()));
        }

        private void SetupMyTeam()
        {
            Transform transform = base.view.transform.Find("TeamPanel/Team");
            this._ventureData.CleanUpSelectAvatarList();
            for (int i = 1; i <= 3; i++)
            {
                transform.Find(i.ToString()).GetComponent<MonoVentureDispatchAvatar>().SetupView(i, this._ventureData);
            }
        }

        private void SetupVentureConditaions()
        {
            Transform transform = base.view.transform.Find("InfoPanel/RequestPanel/RequestScrollView/Content");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<MonoVentureConditionRow>().SetupView(i, this._ventureData);
            }
            this._isConditionMatch = this._ventureData.IsConditionAllMatch();
        }

        private void SetupVentureInfo()
        {
            base.view.transform.Find("TeamPanel/Info/Title/Desc").GetComponent<Text>().text = this._ventureData.VentureName;
            base.view.transform.Find("TeamPanel/Info/Desc/Text").GetComponent<Text>().text = this._ventureData.Desc;
            base.view.transform.Find("TeamPanel/Info/Desc/Text").GetComponent<TypewriterEffect>().RestartRead();
            base.view.transform.Find("InfoPanel/Level/Num").GetComponent<Text>().text = this._ventureData.Level.ToString();
            Transform transform = base.view.transform.Find("InfoPanel/Difficulty/Icon");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive((i + 1) == this._ventureData.Difficulty);
            }
            base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = this._ventureData.StaminaCost.ToString();
        }

        private void SetupVentureRewards()
        {
            List<int> rewardItemIDListToShow = this._ventureData.RewardItemIDListToShow;
            Transform transform = base.view.transform.Find("InfoPanel/DropPanel/Drops/ScollerContent");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (i >= rewardItemIDListToShow.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardItemIDListToShow[i], 1);
                    child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnDropItemButtonClick), false, false, false, true);
                }
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Cost/Stamina").GetComponent<Text>().text = this._ventureData.StaminaCost.ToString();
            this.SetupMyTeam();
            this.SetupVentureInfo();
            this.SetupVentureConditaions();
            this.SetupVentureRewards();
            return false;
        }

        public bool ShowStaminaExchangeDialog()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new StaminaExchangeDialogContext("Menu_Desc_StaminaExchange2"), UIType.Any);
            return false;
        }
    }
}

