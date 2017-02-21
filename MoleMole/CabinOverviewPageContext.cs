namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class CabinOverviewPageContext : BasePageContext
    {
        private Dictionary<MonoIslandBuilding, CabinDataItemBase> _buildingDataDict;
        private CabinDataItemBase _cabinData;
        private bool _cameraFocusEnd;
        private Dictionary<CabinType, MonoIslandBuilding> _data2BuildingDict;
        private Camera _mainCamera;
        private CanvasTimer _pageFadeOutTimer;
        private Camera _uiCamera;
        private bool _waitingForVentureList;
        private int PAGE_FADE_OUT_TRIGGER_ID = Animator.StringToHash("PageFadeOut");

        public CabinOverviewPageContext(CabinDataItemBase cabinData, Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "CabinOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Island/IslandCabinOverviewPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._cabinData = cabinData;
            this._buildingDataDict = buildingDataDict;
        }

        public override void BackToMainMenuPage()
        {
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, null, true);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn").GetComponent<Button>(), new UnityAction(this.OnLevelUpBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Btn").GetComponent<Button>(), new UnityAction(this.OnFinishLevelUpNowBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/Btn").GetComponent<Button>(), new UnityAction(this.OnExtendBtnClick));
            base.BindViewCallback(base.view.transform.Find("ActionPanel/EnterCabinBtn/EnterCabinBtn").GetComponent<Button>(), new UnityAction(this.OnEnterCabinBtnClick));
        }

        private bool DoEnterCabin()
        {
            if (this._pageFadeOutTimer != null)
            {
                this._pageFadeOutTimer.Destroy();
            }
            this._pageFadeOutTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
            this._pageFadeOutTimer.timeUpCallback = new Action(this.EnterCabin);
            base.view.transform.GetComponent<Animator>().SetTrigger(this.PAGE_FADE_OUT_TRIGGER_ID);
            return false;
        }

        private bool DoSetupView()
        {
            base.view.transform.Find("InfoPanel/Info/CabinName").GetComponent<Text>().text = this._cabinData.GetCabinName();
            bool flag = this._cabinData.status == CabinStatus.Locked;
            base.view.transform.Find("ActionPanel/EnhanceBtns").gameObject.SetActive(!flag);
            base.view.transform.Find("ActionPanel/EnterCabinBtn/EnterCabinBtn").GetComponent<Button>().interactable = !flag;
            base.view.transform.Find("InfoPanel/Info/Lv").gameObject.SetActive(!flag);
            if (!flag)
            {
                base.view.transform.Find("InfoPanel/Info/Lv/Lv").GetComponent<Text>().text = "Lv." + this._cabinData.level;
                base.view.transform.Find("InfoPanel/Info/Lv/Max").GetComponent<Text>().text = "/" + this._cabinData.GetCabinMaxLevel();
                bool flag2 = this._cabinData.levelUpEndTime > TimeUtil.Now;
                base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus").gameObject.SetActive(flag2);
                base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/ExtendLevel/Level").GetComponent<MonoCabinExtendGrade>().SetupView(this._cabinData.extendGrade);
                base.view.transform.Find("ActionPanel/EnhanceBtns/ExtendBtn/Btn").GetComponent<Button>().interactable = this._cabinData.CanExtendCabin();
                base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn").gameObject.SetActive(!flag2);
                base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn").GetComponent<Button>().interactable = this._cabinData.CanUpLevel();
                if (this._cabinData.CanUpLevel())
                {
                    base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Time/Time").GetComponent<MonoRemainTimer>().SetTargetTime(this._cabinData.GetCabinLevelUpTimeCost());
                }
                else
                {
                    base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Time").gameObject.SetActive(false);
                }
                base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpBtn/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_LevelUp", new object[0]);
                if (flag2)
                {
                    base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Time/RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(this._cabinData.levelUpEndTime, new Action(this.UpdateProgressBarForLevelUp), new Action(this.OnLevelUpTimeOut), false);
                }
            }
            base.view.transform.Find("ActionPanel/UnlockCondition").gameObject.SetActive(flag);
            if (flag)
            {
                object[] replaceParams = new object[] { this._cabinData.GetUnlockPlayerLevel() };
                base.view.transform.Find("ActionPanel/UnlockCondition/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinUnlockNeedLevel", replaceParams);
            }
            return false;
        }

        private void EnterCabin()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new CabinDetailPageContext(this._cabinData, false), UIType.Page);
        }

        private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
        {
            Vector3 position = this._mainCamera.WorldToScreenPoint(worldPosition);
            position.z = Mathf.Clamp(position.z, this._uiCamera.nearClipPlane, this._uiCamera.farClipPlane);
            return this._uiCamera.ScreenToWorldPoint(position);
        }

        private bool OnBeginExtend(int extendGrade)
        {
            MonoIslandBuilding building = this._data2BuildingDict[this._cabinData.cabinType];
            CabinExtendGradeMetaData cabinExtendGradeMetaDataByKey = CabinExtendGradeMetaDataReader.GetCabinExtendGradeMetaDataByKey(this._cabinData.cabinType, extendGrade);
            building.UpdateBuildingWhenExtend(cabinExtendGradeMetaDataByKey.buildingPath);
            building.AddHighLightMat(null);
            building.SetHighLightAlpha(1f);
            building.SetRenderQueue(E_IslandRenderQueue.Front);
            building.SetPolygonOffset(building.highlight_polygon_offset);
            Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_Extension", null, null, null);
            this.PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinExtend"), true);
            return false;
        }

        private void OnConfirmToFinishLevelUpBtnClick(bool confirmed)
        {
            if (confirmed)
            {
                if (this._cabinData.levelUpEndTime <= TimeUtil.Now)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_AlreadyFinishLevelUp", new object[0]), 2f), UIType.Any);
                }
                else
                {
                    TimeSpan span = (TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now);
                    if (Singleton<IslandModule>.Instance.GetFinishLevelUpNowHcoinCost((int) span.TotalSeconds) > Singleton<PlayerModule>.Instance.playerData.hcoin)
                    {
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("10029", new object[0]), 2f), UIType.Any);
                    }
                    else
                    {
                        Singleton<NetworkManager>.Instance.RequestFinishCabinLevelUp(this._cabinData.cabinType);
                    }
                }
            }
        }

        private void OnEnterCabinBtnClick()
        {
            this._cameraFocusEnd = false;
            GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().ToFocusing();
            if (this._cabinData.cabinType == 5)
            {
                this._waitingForVentureList = true;
                Singleton<NetworkManager>.Instance.RequestIslandVenture();
            }
        }

        private void OnExtendBtnClick()
        {
            int level = this._cabinData.level;
            int cabinMaxLevel = this._cabinData.GetCabinMaxLevel();
            if (level < cabinMaxLevel)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_CabinExtendFailLevelLow", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new CabinEnhanceDialogContext(this._cabinData, CainEnhanceType.Extend), UIType.Any);
            }
        }

        private bool OnFinishCabinLevelUpRsp(FinishCabinLevelUpRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_Upgrade", null, null, null);
                this.PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"), false);
                Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[this._cabinData.cabinType] = false;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        private void OnFinishLevelUpNowBtnClick()
        {
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
            };
            object[] replaceParams = new object[1];
            TimeSpan span = (TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now);
            replaceParams[0] = Singleton<IslandModule>.Instance.GetFinishLevelUpNowHcoinCost((int) span.TotalSeconds);
            dialogContext.desc = LocalizationGeneralLogic.GetText("Menu_Desc_ConfirmToFinishCabinLevelUpNow", replaceParams);
            dialogContext.buttonCallBack = new Action<bool>(this.OnConfirmToFinishLevelUpBtnClick);
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        private bool OnIslandCameraFocus()
        {
            this._cameraFocusEnd = true;
            if ((this._cabinData.cabinType == 5) && this._waitingForVentureList)
            {
                Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(0xa9, null), UIType.Any);
            }
            else
            {
                this.DoEnterCabin();
            }
            return false;
        }

        private bool OnIslandCameraLanded(MonoIslandBuilding buidling)
        {
            CabinDataItemBase base2 = this._buildingDataDict[buidling];
            if (base2.status == CabinStatus.UnLocked)
            {
                this._cabinData = base2;
                this.SetupView();
            }
            else
            {
                this.BackPage();
            }
            return false;
        }

        private void OnLevelUpBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new CabinEnhanceDialogContext(this._cabinData, CainEnhanceType.LevelUp), UIType.Any);
        }

        private void OnLevelUpTimeOut()
        {
            this.PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"), false);
            Singleton<NetworkManager>.Instance.RequestGetIsland();
            Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[this._cabinData.cabinType] = false;
            Singleton<MiHoYoGameData>.Instance.Save();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.OnIslandCameraPreLanded)
            {
                return this.OnIslandCameraLanded((MonoIslandBuilding) ntf.body);
            }
            if (ntf.type == NotifyTypes.OnIslandCameraPreFocus)
            {
                return this.OnIslandCameraFocus();
            }
            return ((ntf.type == NotifyTypes.OnCabinBeginExtend) && this.OnBeginExtend((int) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            ushort num = pkt.getCmdId();
            if (num == 0x9d)
            {
                return this.DoSetupView();
            }
            if (((num == 0xa9) && (this._cabinData.cabinType == 5)) && this._waitingForVentureList)
            {
                this._waitingForVentureList = false;
                if (this._cameraFocusEnd)
                {
                    return this.DoEnterCabin();
                }
            }
            if (num == 0xa3)
            {
                this.OnFinishCabinLevelUpRsp(pkt.getData<FinishCabinLevelUpRsp>());
            }
            return false;
        }

        private void PlayEffect(Transform effectTrans, bool bBuildingPos = false)
        {
            if (bBuildingPos)
            {
                MonoIslandBuilding building = this._data2BuildingDict[this._cabinData.cabinType];
                effectTrans.position = building.transform.position;
            }
            foreach (ParticleSystem system in effectTrans.GetComponentsInChildren<ParticleSystem>())
            {
                system.Play();
            }
        }

        protected override bool SetupView()
        {
            this._mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
            this._uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            this._cameraFocusEnd = false;
            IEnumerator enumerator = base.view.transform.Find("EffectContainer").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    foreach (ParticleSystem system in current.GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[this._cabinData.cabinType] = false;
            Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict[this._cabinData.cabinType] = false;
            Singleton<MiHoYoGameData>.Instance.Save();
            if (this._data2BuildingDict == null)
            {
                this._data2BuildingDict = new Dictionary<CabinType, MonoIslandBuilding>();
                foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair in this._buildingDataDict)
                {
                    this._data2BuildingDict[pair.Value.cabinType] = pair.Key;
                }
            }
            this.DoSetupView();
            return false;
        }

        private void UpdateProgressBarForLevelUp()
        {
            TimeSpan span = (TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now);
            float num = ((float) span.TotalSeconds) / ((float) this._cabinData.GetCabinLevelUpTimeCost());
            base.view.transform.Find("ActionPanel/EnhanceBtns/LevelUpStatus/Time/Slider").GetComponent<Slider>().value = num;
        }
    }
}

