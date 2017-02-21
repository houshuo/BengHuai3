namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class DropLinkDialogContext : BaseDialogContext
    {
        private Action<LevelDataItem> _customDropLinkCallBack;
        private List<LevelDataItem> _dropLevelDataList;
        [CompilerGenerated]
        private static Comparison<LevelDataItem> <>f__am$cache3;
        public readonly MaterialDataItem dropItem;

        public DropLinkDialogContext(MaterialDataItem dropItem, Action<LevelDataItem> onDropLinkClick = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "DropLinkDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ItemDropLinkDialog"
            };
            base.config = pattern;
            this.dropItem = dropItem;
            this.SetupLevelList(onDropLinkClick);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        private void SetupDropLinks()
        {
            Transform transform = base.view.transform.Find("Dialog/Content/DropLinks/Content");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                LevelDataItem levelData = (i < this._dropLevelDataList.Count) ? this._dropLevelDataList[i] : null;
                child.GetComponent<MonoDropLink>().SetupView(levelData, this._customDropLinkCallBack);
            }
        }

        private void SetupLevelList(Action<LevelDataItem> onDropLinkClick)
        {
            this._dropLevelDataList = new List<LevelDataItem>();
            this._customDropLinkCallBack = onDropLinkClick;
            List<int> dropList = this.dropItem.GetDropList();
            List<uint> levelIDList = new List<uint>();
            foreach (int num in dropList)
            {
                LevelDataItem item = Singleton<LevelModule>.Instance.TryGetLevelById(num);
                if (item == null)
                {
                    item = new LevelDataItem(num);
                }
                this._dropLevelDataList.Add(item);
                if (!item.dropDisplayInfoReceived)
                {
                    levelIDList.Add((uint) item.levelId);
                }
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (left, right) => left.levelId - right.levelId;
            }
            this._dropLevelDataList.Sort(<>f__am$cache3);
            if (levelIDList.Count > 0)
            {
                Singleton<NetworkManager>.Instance.RequestLevelDropList(levelIDList);
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.dropItem.GetImagePath());
            base.view.transform.Find("Dialog/Content/Star/EquipStar").GetComponent<MonoEquipStar>().SetupView(this.dropItem.rarity);
            base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = this.dropItem.GetDisplayTitle();
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.dropItem.GetDescription();
            this.SetupDropLinks();
            return false;
        }
    }
}

