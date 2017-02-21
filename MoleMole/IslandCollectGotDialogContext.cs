namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class IslandCollectGotDialogContext : BaseDialogContext
    {
        private float _burstRate;
        private List<DropItem> _materials;
        private int _max_materials_num = 5;
        private int _scoinNum;

        public IslandCollectGotDialogContext(int scoinNum, float burstRate, List<DropItem> dropItems)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "IslandCollectGotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/IslandCollectGotDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            this._scoinNum = scoinNum;
            this._burstRate = burstRate;
            this._materials = dropItems;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn/Btn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        private void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        protected override bool SetupView()
        {
            string str = MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Scoin", new object[0])) + " + " + this._scoinNum;
            if (this._burstRate > 1f)
            {
                string str2 = str;
                string[] textArray1 = new string[] { str2, " ， ", MiscData.AddColor("Blue", LocalizationGeneralLogic.GetText("Menu_Desc_Critical", new object[0])), " \x00d7 ", string.Format("{0:0%}", this._burstRate) };
                str = string.Concat(textArray1);
            }
            base.view.transform.Find("Dialog/Content/TextScoin/line/Desc").GetComponent<Text>().text = str;
            base.view.transform.Find("Dialog/Content/MaterialList").gameObject.SetActive(this._materials.Count > 0);
            if (this._materials.Count >= 1)
            {
                if (this._materials.Count > this._max_materials_num)
                {
                }
                if (!Singleton<IslandModule>.Instance.IsDropMaterials() && (this._materials.Count > 0))
                {
                }
                for (int i = 0; i < this._max_materials_num; i++)
                {
                    int num4 = i + 1;
                    Transform transform = base.view.transform.Find(string.Format("Dialog/Content/MaterialList/{0}", num4.ToString()));
                    if (i < this._materials.Count)
                    {
                        transform.gameObject.SetActive(true);
                        int metaId = (int) this._materials[i].get_item_id();
                        int level = (int) this._materials[i].get_level();
                        StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, level);
                        if (dummyStorageDataItem != null)
                        {
                            transform.Find("ItemIcon/Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
                            transform.Find("Star").GetComponent<MonoItemIconStar>().SetupView(dummyStorageDataItem.rarity, dummyStorageDataItem.rarity);
                            transform.Find("Text").GetComponent<Text>().text = string.Format("x{0}", this._materials[i].get_num());
                        }
                    }
                    else
                    {
                        transform.gameObject.SetActive(false);
                    }
                }
            }
            return false;
        }
    }
}

