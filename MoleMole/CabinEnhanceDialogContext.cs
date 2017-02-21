namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class CabinEnhanceDialogContext : BaseDialogContext
    {
        private CabinDataItemBase _cabinData;
        private CainEnhanceType _enhanceType;
        private int _extendGradBefore;
        private List<StorageDataItemBase> _itemNeedList;
        private bool _materialEnough;
        private List<EvoItem> _materialList;
        private Dictionary<int, EvoItem> _resourceDict;

        public CabinEnhanceDialogContext(CabinDataItemBase cabinData, CainEnhanceType enhanceType)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "CabinEnhanceDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/CabinEnhanceDialog"
            };
            base.config = pattern;
            this._cabinData = cabinData;
            this._extendGradBefore = this._cabinData.extendGrade;
            this._enhanceType = enhanceType;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Consume/Btn").GetComponent<Button>(), new UnityAction(this.OnActionButtonClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        private void GetEvoResourceList(out List<EvoItem> list, out Dictionary<int, EvoItem> dict)
        {
            list = new List<EvoItem>();
            dict = new Dictionary<int, EvoItem>();
            this._materialEnough = true;
            foreach (StorageDataItemBase base2 in this._itemNeedList)
            {
                int iD = base2.ID;
                int number = base2.number;
                EvoItem item = new EvoItem();
                List<StorageDataItemBase> list2 = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(iD, number);
                if (list2.Count > 0)
                {
                    list2.Sort(new Comparison<StorageDataItemBase>(StorageDataItemBase.CompareToLevelAsc));
                    item.item = list2[0].Clone();
                    item.enough = true;
                    item.item.number = number;
                }
                else
                {
                    item.item = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(iD, 1);
                    item.enough = false;
                    item.item.number = number;
                    this._materialEnough = false;
                }
                list.Add(item);
                if (!dict.ContainsKey(iD))
                {
                    dict.Add(iD, item);
                }
            }
        }

        public void OnActionButtonClick()
        {
            switch (this._enhanceType)
            {
                case CainEnhanceType.LevelUp:
                    Singleton<NetworkManager>.Instance.RequestCabinLevelUp(this._cabinData.cabinType);
                    break;

                case CainEnhanceType.Extend:
                    Singleton<NetworkManager>.Instance.RequestExtendCabin(this._cabinData.cabinType);
                    break;
            }
        }

        private void OnChange(Transform trans, int index)
        {
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.SetupView(this._materialList[index].item, MonoItemIconButton.SelectMode.ConsumeMaterial, false, false, false);
            component.transform.Find("NotEnough").gameObject.SetActive(!this._materialList[index].enough);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnResourceItemButtonClick));
        }

        private void OnDropLinkClick(LevelDataItem levelData)
        {
            <OnDropLinkClick>c__AnonStoreyDF ydf = new <OnDropLinkClick>c__AnonStoreyDF {
                levelData = levelData,
                <>f__this = this
            };
            if (ydf.levelData != null)
            {
                GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                    type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_LeaveIslandHint", new object[0]),
                    buttonCallBack = new Action<bool>(ydf.<>m__10A)
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        private bool OnExtendCabinRspp(ExtendCabinRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    desc = rsp.get_retcode().ToString()
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinBeginExtend, this._extendGradBefore + 1));
            }
            this.Destroy();
            return false;
        }

        private bool OnLevelUpCabinRsp(LevelUpCabinRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    desc = rsp.get_retcode().ToString()
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            else
            {
                Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[this._cabinData.cabinType] = true;
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            this.Destroy();
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x9f:
                    this.OnLevelUpCabinRsp(pkt.getData<LevelUpCabinRsp>());
                    break;

                case 0xa1:
                    return this.OnExtendCabinRspp(pkt.getData<ExtendCabinRsp>());
            }
            return false;
        }

        private void OnResourceItemButtonClick(StorageDataItemBase item, bool selelcted = false)
        {
            if (this._resourceDict[item.ID].enough)
            {
                UIUtil.ShowItemDetail(item, true, true);
            }
            else if (item is MaterialDataItem)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem, new Action<LevelDataItem>(this.OnDropLinkClick)), UIType.Any);
            }
        }

        private void SetupCabinLevelUpDiff(CabinType cabinType)
        {
            Transform techTran = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info1");
            Transform transform2 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info2");
            Transform transform3 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/Info3");
            Transform transform4 = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/CabinOtherInfo");
            techTran.gameObject.SetActive(false);
            transform2.gameObject.SetActive(false);
            transform3.gameObject.SetActive(false);
            transform4.gameObject.SetActive(false);
            switch (cabinType)
            {
                case 1:
                {
                    techTran.gameObject.SetActive(true);
                    techTran.Find("Current").gameObject.SetActive(true);
                    techTran.Find("UpIcon").gameObject.SetActive(true);
                    int maxPowerCost = Singleton<IslandModule>.Instance.GetMaxPowerCost();
                    int nextLevelMaxPowerCost = Singleton<IslandModule>.Instance.GetNextLevelMaxPowerCost();
                    techTran.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxPower", new object[0]);
                    techTran.Find("Current").GetComponent<Text>().text = maxPowerCost.ToString();
                    techTran.Find("Next").GetComponent<Text>().text = nextLevelMaxPowerCost.ToString();
                    break;
                }
                case 2:
                    this.SetupTechInfo(techTran);
                    break;

                case 3:
                {
                    this.SetupTechInfo(techTran);
                    int scoinGrowthBase = (int) CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(this._cabinData.level).scoinGrowthBase;
                    int scoinStorageBase = (int) CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(this._cabinData.level + 1).scoinGrowthBase;
                    if (scoinGrowthBase != scoinStorageBase)
                    {
                        transform2.gameObject.SetActive(true);
                        transform2.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_ScoinGrowth", new object[0]);
                        transform2.Find("Current").GetComponent<Text>().text = scoinGrowthBase.ToString();
                        transform2.Find("Next").GetComponent<Text>().text = scoinStorageBase.ToString();
                    }
                    scoinGrowthBase = (int) CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(this._cabinData.level).scoinStorageBase;
                    scoinStorageBase = (int) CabinCollectLevelMetaDataReader.GetCabinCollectLevelDataMetaDataByKey(this._cabinData.level + 1).scoinStorageBase;
                    if (scoinGrowthBase != scoinStorageBase)
                    {
                        transform3.gameObject.SetActive(true);
                        transform3.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_ScoinStorage", new object[0]);
                        transform3.Find("Current").GetComponent<Text>().text = scoinGrowthBase.ToString();
                        transform3.Find("Next").GetComponent<Text>().text = scoinStorageBase.ToString();
                    }
                    break;
                }
                case 4:
                    this.SetupTechInfo(techTran);
                    break;

                case 5:
                {
                    this.SetupTechInfo(techTran);
                    int maxVentureNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(this._cabinData.level).maxVentureNumBase;
                    int maxVentureInProgressNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(this._cabinData.level + 1).maxVentureNumBase;
                    if (maxVentureNumBase != maxVentureInProgressNumBase)
                    {
                        transform2.gameObject.SetActive(true);
                        transform2.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxVenture", new object[0]);
                        transform2.Find("Current").GetComponent<Text>().text = maxVentureNumBase.ToString();
                        transform2.Find("Next").GetComponent<Text>().text = maxVentureInProgressNumBase.ToString();
                    }
                    maxVentureNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(this._cabinData.level).maxVentureInProgressNumBase;
                    maxVentureInProgressNumBase = CabinVentureLevelMetaDataReader.GetCabinVentureLevelDataMetaDataByKey(this._cabinData.level + 1).maxVentureInProgressNumBase;
                    if (maxVentureNumBase != maxVentureInProgressNumBase)
                    {
                        transform3.gameObject.SetActive(true);
                        transform3.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_MaxInProgressVenture", new object[0]);
                        transform3.Find("Current").GetComponent<Text>().text = maxVentureNumBase.ToString();
                        transform3.Find("Next").GetComponent<Text>().text = maxVentureInProgressNumBase.ToString();
                    }
                    break;
                }
                case 6:
                    this.SetupTechInfo(techTran);
                    break;

                case 7:
                    this.SetupTechInfo(techTran);
                    break;
            }
        }

        private void SetupTechInfo(Transform techTran)
        {
            int availableNodesDiff = this._cabinData._techTree.GetAvailableNodesDiff(this._cabinData.level + 1);
            if (availableNodesDiff > 0)
            {
                techTran.gameObject.SetActive(true);
                techTran.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CabinLevelUp_NewAvailableTechNode", new object[0]);
                techTran.Find("Current").gameObject.SetActive(false);
                techTran.Find("UpIcon").gameObject.SetActive(false);
                techTran.Find("Next").GetComponent<Text>().text = availableNodesDiff.ToString();
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/CabinLevelUpInfo").gameObject.SetActive(this._enhanceType == CainEnhanceType.LevelUp);
            base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(this._enhanceType == CainEnhanceType.LevelUp);
            base.view.transform.Find("Dialog/Content/CabinExtendInfo").gameObject.SetActive(this._enhanceType == CainEnhanceType.Extend);
            base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
            int cabinLevelUpScoinCost = 0;
            switch (this._enhanceType)
            {
                case CainEnhanceType.LevelUp:
                {
                    base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_LevelUpCabin", new object[0]);
                    base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Action_BeginToLevelUp", new object[0]);
                    Transform transform = base.view.transform.Find("Dialog/Content/CabinLevelUpInfo/CabinLevelUpInfo");
                    transform.Find("CabinName").GetComponent<Text>().text = this._cabinData.GetCabinName();
                    transform.Find("CurrentLevel").GetComponent<Text>().text = "Lv." + this._cabinData.level;
                    transform.Find("NextLevel").GetComponent<Text>().text = "Lv." + (this._cabinData.level + 1);
                    this._itemNeedList = this._cabinData.GetLevelUpItemNeed();
                    base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(true);
                    base.view.transform.Find("Dialog/Content/Materials/RemainTime/Time").GetComponent<MonoRemainTimer>().SetTargetTime(this._cabinData.GetCabinLevelUpTimeCost());
                    cabinLevelUpScoinCost = this._cabinData.GetCabinLevelUpScoinCost();
                    this.SetupCabinLevelUpDiff(this._cabinData.cabinType);
                    break;
                }
                case CainEnhanceType.Extend:
                {
                    base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Title_ExtendCabin", new object[0]);
                    base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Action_BeginToExtend", new object[0]);
                    Transform transform2 = base.view.transform.Find("Dialog/Content/CabinExtendInfo/CabinExtendInfo");
                    transform2.Find("CabinName").GetComponent<Text>().text = this._cabinData.GetCabinName();
                    transform2.Find("CurrentLevel").GetComponent<MonoCabinExtendGrade>().SetupView(this._cabinData.extendGrade);
                    transform2.Find("NextLevel").GetComponent<MonoCabinExtendGrade>().SetupView(this._cabinData.extendGrade + 1);
                    Transform transform3 = base.view.transform.Find("Dialog/Content/CabinExtendInfo/CabinExtendLevelInfo");
                    transform3.Find("Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Label_LevelTopLimit", new object[0]);
                    transform3.Find("CurrentLevel").GetComponent<Text>().text = "Lv." + this._cabinData.GetCabinMaxLevel();
                    transform3.Find("NextLevel").GetComponent<Text>().text = "Lv." + this._cabinData.GetCabinMaxLevelNextExntendGrade();
                    this._itemNeedList = this._cabinData.GetExtendItemNeed();
                    cabinLevelUpScoinCost = this._cabinData.GetCabinExntendScoinCost();
                    break;
                }
            }
            this.GetEvoResourceList(out this._materialList, out this._resourceDict);
            base.view.transform.Find("Dialog/Content/Materials/Materials").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._materialList.Count, new Vector2(0f, 0f));
            base.view.transform.Find("Dialog/Content/Consume/SCoinNum").GetComponent<Text>().text = cabinLevelUpScoinCost.ToString();
            bool flag = cabinLevelUpScoinCost <= Singleton<PlayerModule>.Instance.playerData.scoin;
            bool flag2 = flag && this._materialEnough;
            bool flag3 = true;
            bool flag4 = true;
            bool flag5 = this._cabinData.level < this._cabinData.GetCabinMaxLevel();
            if (this._enhanceType == CainEnhanceType.LevelUp)
            {
                flag3 = this._cabinData.GetPlayerLevelNeedToUpLevel() <= Singleton<PlayerModule>.Instance.playerData.teamLevel;
                flag4 = !Singleton<IslandModule>.Instance.HasCabinLevelUpInProgress();
                flag2 &= flag3;
                flag2 &= flag4;
                flag2 &= flag5;
            }
            base.view.transform.Find("Dialog/Content/Consume/Btn").GetComponent<Button>().interactable = flag2;
            if (!flag)
            {
                base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Err_ScoinLack", new object[0]);
            }
            if (!this._materialEnough)
            {
                base.view.transform.Find("Dialog/Content/Consume/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Err_ItemLack", new object[0]);
            }
            if (this._enhanceType == CainEnhanceType.LevelUp)
            {
                if (!flag3)
                {
                    base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
                    base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
                    object[] replaceParams = new object[] { this._cabinData.GetPlayerLevelNeedToUpLevel() };
                    base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_CommandLevelLack", replaceParams);
                }
                if (!flag4)
                {
                    base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
                    base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
                    base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_OnlyOneCabinCanUpLevel", new object[0]);
                }
                if (!flag5)
                {
                    base.view.transform.Find("Dialog/Content/Materials/RedTips").gameObject.SetActive(true);
                    base.view.transform.Find("Dialog/Content/Materials/RemainTime").gameObject.SetActive(false);
                    if (this._cabinData.CanExtendCabin())
                    {
                        base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_NeedToExtend", new object[0]);
                    }
                    else
                    {
                        base.view.transform.Find("Dialog/Content/Materials/RedTips/Label").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AlreadyMax", new object[0]);
                    }
                }
            }
            return false;
        }

        private void ShowChapterSelectPage(bool confirmed, LevelDataItem levelData)
        {
            <ShowChapterSelectPage>c__AnonStoreyE0 ye = new <ShowChapterSelectPage>c__AnonStoreyE0 {
                levelData = levelData,
                <>f__this = this
            };
            if (confirmed)
            {
                Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, new Action(ye.<>m__10B), true);
            }
        }

        private void ShowChapterSelectPageWhenMainSceneLoaded(LevelDataItem levelData)
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(levelData), UIType.Page);
        }

        [CompilerGenerated]
        private sealed class <OnDropLinkClick>c__AnonStoreyDF
        {
            internal CabinEnhanceDialogContext <>f__this;
            internal LevelDataItem levelData;

            internal void <>m__10A(bool confirmed)
            {
                this.<>f__this.ShowChapterSelectPage(confirmed, this.levelData);
            }
        }

        [CompilerGenerated]
        private sealed class <ShowChapterSelectPage>c__AnonStoreyE0
        {
            internal CabinEnhanceDialogContext <>f__this;
            internal LevelDataItem levelData;

            internal void <>m__10B()
            {
                this.<>f__this.ShowChapterSelectPageWhenMainSceneLoaded(this.levelData);
            }
        }
    }
}

