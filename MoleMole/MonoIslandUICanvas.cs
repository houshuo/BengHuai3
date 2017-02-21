namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoIslandUICanvas : BaseMonoCanvas
    {
        private Transform _blockPanel;
        public MonoIslandBuilding bronyaEnhance;
        private Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict;
        public MonoIslandBuilding collect;
        public GameObject islandMainPage;
        public MonoIslandBuilding kianaEnhance;
        public IslandMainPageContext mainPageContext;
        public MonoIslandBuilding meiEnhance;
        public MonoIslandBuilding misc;
        public MonoIslandBuilding mission;
        public PlayerStatusWidgetContext playerBar;
        public MonoIslandBuilding power;

        private void Awake()
        {
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
            this._blockPanel = base.transform.Find("BlockPanel");
        }

        public CabinDataItemBase GetCabinDataByBuilding(MonoIslandBuilding building)
        {
            return this.buildingDataDict[building];
        }

        public void SetBuildingEffect(MonoIslandBuilding excludeBuilding, bool enable)
        {
            foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair in this.buildingDataDict)
            {
                if (pair.Key != excludeBuilding)
                {
                    if (enable)
                    {
                        pair.Key.GetModel().ToUnMaskedGraphic();
                    }
                    else
                    {
                        pair.Key.GetModel().ToMaskedGraphic();
                    }
                }
            }
        }

        private void SetupBuilding(MonoIslandBuilding building)
        {
            CabinDataItemBase base2 = this.buildingDataDict[building];
            int extendGrade = (base2.status != CabinStatus.UnLocked) ? 1 : base2.extendGrade;
            CabinExtendGradeMetaData cabinExtendGradeMetaDataByKey = CabinExtendGradeMetaDataReader.GetCabinExtendGradeMetaDataByKey(base2.cabinType, extendGrade);
            building.UpdateBuildingWhenExtend(cabinExtendGradeMetaDataByKey.buildingPath);
        }

        private void SetupBuildings()
        {
            this.SetupBuilding(this.power);
            this.SetupBuilding(this.mission);
            this.SetupBuilding(this.misc);
            this.SetupBuilding(this.collect);
            this.SetupBuilding(this.kianaEnhance);
            this.SetupBuilding(this.meiEnhance);
            this.SetupBuilding(this.bronyaEnhance);
        }

        public override void Start()
        {
            this.buildingDataDict = new Dictionary<MonoIslandBuilding, CabinDataItemBase>();
            this.buildingDataDict[this.power] = Singleton<IslandModule>.Instance.GetCabinDataByType(1);
            this.buildingDataDict[this.collect] = Singleton<IslandModule>.Instance.GetCabinDataByType(3);
            this.buildingDataDict[this.misc] = Singleton<IslandModule>.Instance.GetCabinDataByType(4);
            this.buildingDataDict[this.mission] = Singleton<IslandModule>.Instance.GetCabinDataByType(5);
            this.buildingDataDict[this.kianaEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType(2);
            this.buildingDataDict[this.meiEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType(6);
            this.buildingDataDict[this.bronyaEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType(7);
            this.SetupBuildings();
            this.playerBar = new PlayerStatusWidgetContext();
            Singleton<MainUIManager>.Instance.ShowWidget(this.playerBar, UIType.Any);
            CabinDetailPageContext context = new CabinDetailPageContext(this.buildingDataDict[this.power], true) {
                EnableTutorial = false
            };
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            context.Destroy();
            CabinOverviewPageContext context2 = new CabinOverviewPageContext(this.buildingDataDict[this.power], this.buildingDataDict) {
                EnableTutorial = false
            };
            Singleton<MainUIManager>.Instance.ShowPage(context2, UIType.Page);
            context2.Destroy();
            this.islandMainPage.SetActive(true);
            this.mainPageContext = new IslandMainPageContext(this.islandMainPage, this.buildingDataDict);
            Singleton<MainUIManager>.Instance.ShowPage(this.mainPageContext, UIType.Page);
            this.mainPageContext.view.name = "IslandMainPageContext";
            GraphicsSettingData.ApplySettingConfig();
            AudioSettingData.ApplySettingConfig();
            this.TriggerFullScreenBlock(false);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene, null));
            base.Start();
        }

        public void TriggerFullScreenBlock(bool enable)
        {
            if ((this._blockPanel != null) && (this._blockPanel.gameObject != null))
            {
                this._blockPanel.gameObject.SetActive(enable);
            }
        }

        public override void Update()
        {
            base.Update();
        }
    }
}

