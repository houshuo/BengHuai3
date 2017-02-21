namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class ItemDetailDialogContext : BaseDialogContext
    {
        public readonly bool hideActionBtns;
        public readonly StorageDataItemBase storageItem;

        public ItemDetailDialogContext(StorageDataItemBase storageItem, bool hideActionBtns = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ItemDetailDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ItemDetailDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this.storageItem = storageItem;
            this.hideActionBtns = hideActionBtns;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/RarityUpBtn").GetComponent<Button>(), new UnityAction(this.OnRarityUpBtnCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        private string GetAllDesc()
        {
            string str = UIUtil.ProcessStrWithNewLine(this.storageItem.GetDescription());
            MaterialDataItem storageItem = this.storageItem as MaterialDataItem;
            if (storageItem != null)
            {
                string bGDescription = storageItem.GetBGDescription();
                return string.Format("{0}{1}<color=#a8a8a8ff>{2}</color>", str, Environment.NewLine, bGDescription);
            }
            return str;
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnRarityUpBtnCallBack()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new StorageEvoPageContext(this.storageItem), UIType.Page);
        }

        private void SetupRarityView()
        {
            string hexString = MiscData.Config.ItemRarityColorList[this.storageItem.rarity];
            base.view.transform.Find("Dialog/Content/Icon").GetComponent<Image>().color = Miscs.ParseColor(hexString);
        }

        protected override bool SetupView()
        {
            this.SetupRarityView();
            base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this.storageItem.GetImagePath());
            Transform transform = base.view.transform.Find("Dialog/Content/Star/EquipStar");
            if (this.storageItem is AvatarFragmentDataItem)
            {
                transform.gameObject.SetActive(false);
            }
            else
            {
                transform.gameObject.SetActive(true);
                transform.GetComponent<MonoEquipStar>().SetupView(this.storageItem.rarity);
            }
            base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = this.storageItem.GetDisplayTitle();
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.GetAllDesc();
            base.view.transform.Find("Dialog/Content/Num/Text").GetComponent<Text>().text = this.storageItem.number.ToString();
            base.view.transform.Find("Dialog/Content/Num").gameObject.SetActive(!this.hideActionBtns);
            base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(!this.hideActionBtns && (this.storageItem.GetEvoStorageItem() != null));
            return false;
        }
    }
}

