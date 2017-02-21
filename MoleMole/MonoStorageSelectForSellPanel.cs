namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class MonoStorageSelectForSellPanel : MonoBehaviour
    {
        private bool _isMultiSell;
        private Dictionary<int, StorageDataItemBase> _sellItemMap;
        private int _sellNum;
        public const int MAX_SELL_NUM = 20;

        private int CalculateTotalSCoinSell()
        {
            float f = 0f;
            int num2 = this._isMultiSell ? this._sellNum : 1;
            foreach (StorageDataItemBase base2 in this._sellItemMap.Values)
            {
                f += base2.GetPriceForSell() * num2;
            }
            return Mathf.FloorToInt(f);
        }

        private void Close()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSellViewActive, false));
        }

        public bool IsItemInSelectedMap(StorageDataItemBase item)
        {
            int idForKey = item.GetIdForKey();
            return this._sellItemMap.ContainsKey(idForKey);
        }

        public void OnSellDecreaseBtnClick()
        {
            if ((this._sellItemMap.Count != 0) && (this._sellNum > 1))
            {
                this._sellNum--;
                this.UpdateDataView();
            }
        }

        public void OnSellIncreaseBtnClick()
        {
            if (this._sellItemMap.Count != 0)
            {
                StorageDataItemBase base2 = Enumerable.First<StorageDataItemBase>(this._sellItemMap.Values);
                if (this._sellNum < base2.number)
                {
                    this._sellNum++;
                    this.UpdateDataView();
                }
            }
        }

        public void OnSellMaxBtnClick()
        {
            if (this._sellItemMap.Count != 0)
            {
                StorageDataItemBase base2 = Enumerable.First<StorageDataItemBase>(this._sellItemMap.Values);
                this._sellNum = base2.number;
                this.UpdateDataView();
            }
        }

        public void OnSellPanelCloseBtnClick()
        {
            this.Close();
        }

        public void OnSellPanelOKBtnClick()
        {
            if (this._sellItemMap.Values.Count != 0)
            {
                List<StorageDataItemBase> sellList = new List<StorageDataItemBase>();
                int num = !this._isMultiSell ? 1 : this._sellNum;
                foreach (StorageDataItemBase base2 in this._sellItemMap.Values)
                {
                    StorageDataItemBase item = base2.Clone();
                    item.number = num;
                    sellList.Add(item);
                }
                Singleton<MainUIManager>.Instance.ShowDialog(new SellConfirmDialogContext(sellList), UIType.Any);
            }
        }

        public void RefreshOnItemButonClick(StorageDataItemBase item)
        {
            int idForKey = item.GetIdForKey();
            if (this._sellItemMap.ContainsKey(idForKey))
            {
                this._sellItemMap.Remove(idForKey);
                if (this._isMultiSell)
                {
                    this._sellNum = 0;
                }
            }
            else if (this._isMultiSell)
            {
                this._sellItemMap.Clear();
                this._sellItemMap.Add(idForKey, item);
                this._sellNum = 1;
            }
            else if (this._sellItemMap.Count < 20)
            {
                this._sellItemMap.Add(idForKey, item);
            }
            this.UpdateDataView();
        }

        public void SetupView(bool isMultiSell)
        {
            this._isMultiSell = isMultiSell;
            this._sellItemMap = new Dictionary<int, StorageDataItemBase>();
            this._sellNum = 0;
            base.transform.Find("SingleNum").gameObject.SetActive(!isMultiSell);
            base.transform.Find("MultiNum").gameObject.SetActive(isMultiSell);
            this.UpdateDataView();
        }

        private void UpdateDataView()
        {
            int num = this.CalculateTotalSCoinSell();
            if (!this._isMultiSell)
            {
                Transform transform = base.transform.Find("SingleNum");
                transform.Find("SCoin/Num").GetComponent<Text>().text = num.ToString();
                int count = this._sellItemMap.Keys.Count;
                transform.Find("Select/Num").GetComponent<Text>().text = count + "/" + 20;
                int equipmentSizeLimit = Singleton<PlayerModule>.Instance.playerData.equipmentSizeLimit;
                int currentCapacity = Singleton<StorageModule>.Instance.GetCurrentCapacity();
                transform.Find("Capacity/Num").GetComponent<Text>().text = ((currentCapacity - count)).ToString() + "/" + equipmentSizeLimit;
            }
            else
            {
                Transform transform2 = base.transform.Find("MultiNum");
                transform2.Find("SCoin/Num").GetComponent<Text>().text = num.ToString();
                transform2.Find("SellNum/Text").GetComponent<Text>().text = this._sellNum.ToString();
            }
        }
    }
}

