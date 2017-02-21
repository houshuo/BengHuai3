namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class StorageItemSellDialogContext : BaseDialogContext
    {
        private int _maxItemNumber;
        private GameObject _multipItemSellInfoPanel;
        private GameObject _oneItemSellInfoPanel;
        private int _sellItemNumber;
        public StorageDataItemBase storageDataItem;

        public StorageItemSellDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "StorageItemSellDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SellItemDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/ActionPanel/ConfirmButton").GetComponent<Button>(), new UnityAction(this.OnConfirmButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/ActionPanel/CancelButton").GetComponent<Button>(), new UnityAction(this.OnCancelButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/IncreaseButton").GetComponent<Button>(), new UnityAction(this.OnIncreaseButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/DecreaseButton").GetComponent<Button>(), new UnityAction(this.OnDecreaseButtonCallBack));
        }

        private void Init()
        {
            this._oneItemSellInfoPanel = base.view.transform.Find("Dialog/OneItemSellInfoPanel").gameObject;
            this._multipItemSellInfoPanel = base.view.transform.Find("Dialog/MultipleItemSellInfoPanel").gameObject;
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public void OnCancelButtonCallBack()
        {
            this.Destroy();
        }

        public void OnConfirmButtonCallBack()
        {
            List<StorageDataItemBase> storageItemList = new List<StorageDataItemBase>();
            if (this.storageDataItem is MaterialDataItem)
            {
                StorageDataItemBase item = new MaterialDataItem(ItemMetaDataReader.GetItemMetaDataByKey(this.storageDataItem.ID)) {
                    number = this._sellItemNumber
                };
                storageItemList.Add(item);
            }
            else
            {
                storageItemList.Add(this.storageDataItem);
            }
            Singleton<NetworkManager>.Instance.RequestEquipmentSell(storageItemList);
            if (!(this.storageDataItem is MaterialDataItem) || (this.storageDataItem.number <= this._sellItemNumber))
            {
                Singleton<MainUIManager>.Instance.BackPageTo("StorageShowPageContext");
                this.Destroy();
            }
        }

        public void OnDecreaseButtonCallBack()
        {
            if (this._sellItemNumber > 1)
            {
                this._sellItemNumber--;
                this.OnSellNumChange();
            }
        }

        public void OnIncreaseButtonCallBack()
        {
            if (this._sellItemNumber < this._maxItemNumber)
            {
                this._sellItemNumber++;
                this.OnSellNumChange();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x22) && this.SetupView());
        }

        private void OnSellNumChange()
        {
            base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/SellNum/Text").GetComponent<Text>().text = this._sellItemNumber.ToString();
            base.view.transform.Find("Dialog/MultipleItemSellInfoPanel/CoinNum/CoinGotNumber").GetComponent<Text>().text = (this.storageDataItem.GetPriceForSell() * this._sellItemNumber).ToString();
        }

        protected override bool SetupView()
        {
            this.Init();
            base.view.transform.Find("Dialog/ItemButton").GetComponent<MonoStorageItemIcon>().SetupView(this.storageDataItem, base.view.transform.Find("Dialog"), null, -1, this.storageDataItem.GetType(), true);
            bool flag = this.storageDataItem is MaterialDataItem;
            this._oneItemSellInfoPanel.SetActive(!flag);
            this._multipItemSellInfoPanel.SetActive(flag);
            if (flag)
            {
                MaterialDataItem storageDataItem = this.storageDataItem as MaterialDataItem;
                this._sellItemNumber = 1;
                this._maxItemNumber = storageDataItem.number;
                this.OnSellNumChange();
            }
            else
            {
                base.view.transform.Find("Dialog/OneItemSellInfoPanel/CoinGotNumber").GetComponent<Text>().text = Mathf.FloorToInt(this.storageDataItem.GetPriceForSell()).ToString();
            }
            return false;
        }
    }
}

