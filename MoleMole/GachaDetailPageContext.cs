namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class GachaDetailPageContext : BasePageContext
    {
        private GachaDisplayCommonData _displayData;
        private int _gachaType;
        private const string ITEM_ICON_BUTTON_PREFAB_PATH = "UI/Menus/Widget/Map/DropItemButton";
        private List<StorageDataItemBase> upAvatarDataList;
        private List<StorageDataItemBase> upStigmataDataList;
        private List<StorageDataItemBase> upWeaponDataList;

        public GachaDetailPageContext(GachaDisplayCommonData displayData, int gachaType)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GachaDetailPageContext",
                viewPrefabPath = "UI/Menus/Page/Gacha/GachaDetailInfoPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            this._displayData = displayData;
            this._gachaType = gachaType;
        }

        private void OnItemClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        private void SetupUpAvatarPanel()
        {
            Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpAvatarPanel");
            transform.gameObject.SetActive(this._displayData.get_up_avatar_list().Count > 0);
            if (this._displayData.get_up_avatar_list().Count > 0)
            {
                this.upAvatarDataList = new List<StorageDataItemBase>();
                foreach (uint num in this._displayData.get_up_avatar_list())
                {
                    AvatarCardDataItem item = new AvatarCardDataItem(AvatarCardMetaDataReader.GetAvatarCardMetaDataByKey(AvatarMetaDataReaderExtend.GetAvatarIDsByKey((int) num).avatarCardID));
                    this.upAvatarDataList.Add(item);
                }
                this.SetupUpContent(transform.Find("AvatarPanel"), transform.Find("AvatarNamePanel/Text").GetComponent<Text>(), this.upAvatarDataList);
            }
        }

        private void SetupUpContent(Transform gridTrans, Text nameText, List<StorageDataItemBase> itemList)
        {
            string str = string.Empty;
            gridTrans.DestroyChildren();
            foreach (StorageDataItemBase base2 in itemList)
            {
                if (base2 is AvatarCardDataItem)
                {
                    str = str + Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(AvatarMetaDataReaderExtend.GetAvatarIDsByKey(base2.ID).avatarID).FullName;
                    str = str + Environment.NewLine;
                }
                else if ((base2 is WeaponDataItem) || (base2 is StigmataDataItem))
                {
                    str = str + base2.GetDisplayTitle();
                    str = str + Environment.NewLine;
                }
                GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/Map/DropItemButton"));
                if (obj2 != null)
                {
                    obj2.transform.SetParent(gridTrans, false);
                    obj2.GetComponent<MonoLevelDropIconButton>().SetupView(base2, new DropItemButtonClickCallBack(this.OnItemClick), true, false, false, false);
                    obj2.GetComponent<CanvasGroup>().alpha = 1f;
                }
            }
            nameText.text = str;
        }

        private void SetupUpStigmataPanel()
        {
            Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpStigmataPanel");
            transform.gameObject.SetActive(this._displayData.get_up_stigmata_list().Count > 0);
            if (this._displayData.get_up_stigmata_list().Count > 0)
            {
                this.upStigmataDataList = new List<StorageDataItemBase>();
                foreach (StigmataDetailData data in this._displayData.get_up_stigmata_list())
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) data.get_id(), (int) data.get_level());
                    this.upStigmataDataList.Add(dummyStorageDataItem);
                }
                this.SetupUpContent(transform.Find("StigmataPanel"), transform.Find("StigmataNamePanel/Text").GetComponent<Text>(), this.upStigmataDataList);
            }
        }

        private void SetupUpWeaponPanel()
        {
            Transform transform = base.view.transform.Find("Content/ScrollView/Content/UpWeaponPanel");
            transform.gameObject.SetActive(this._displayData.get_up_weapon_list().Count > 0);
            if (this._displayData.get_up_weapon_list().Count > 0)
            {
                this.upWeaponDataList = new List<StorageDataItemBase>();
                foreach (WeaponDetailData data in this._displayData.get_up_weapon_list())
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) data.get_id(), (int) data.get_level());
                    this.upWeaponDataList.Add(dummyStorageDataItem);
                }
                this.SetupUpContent(transform.Find("WeaponPanel"), transform.Find("WeaponNamePanel/Text").GetComponent<Text>(), this.upWeaponDataList);
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Content/ScrollView/Content/Title/Intro").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(this._displayData.get_content());
            if (string.IsNullOrEmpty(this._displayData.get_title_image()))
            {
                base.view.transform.Find("Content/ScrollView/Content/Title/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.GachaTypeTitleFigures[this._gachaType]);
            }
            else
            {
                UIUtil.TrySetupEventSprite(base.view.transform.Find("Content/ScrollView/Content/Title/Image").GetComponent<Image>(), this._displayData.get_title_image());
            }
            base.view.transform.Find("Content/ScrollView/Content/Title/Time").GetComponent<Text>().text = this._displayData.get_title();
            this.SetupUpAvatarPanel();
            this.SetupUpWeaponPanel();
            this.SetupUpStigmataPanel();
            base.view.transform.Find("Content/ScrollView/Content/RulePanel").gameObject.SetActive(this._displayData.get_ruleSpecified());
            if (this._displayData.get_ruleSpecified())
            {
                base.view.transform.Find("Content/ScrollView/Content/RulePanel/TextContent/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(this._displayData.get_rule());
            }
            base.view.transform.Find("Content/ScrollView/Content/ContentDetailPanel").gameObject.SetActive(this._displayData.get_content_detailSpecified());
            if (this._displayData.get_content_detailSpecified())
            {
                base.view.transform.Find("Content/ScrollView/Content/ContentDetailPanel/TextContent/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(this._displayData.get_content_detail());
            }
            base.view.transform.GetComponent<MonoFadeInAnimManager>().Play("PageFadeIn", false, null);
            return false;
        }
    }
}

