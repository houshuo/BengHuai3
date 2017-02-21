namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoStorageSelectForPowerUp : MonoBehaviour
    {
        private bool _isMulti;
        private StorageDataItemBase _modifyingItem;
        private StorageDataItemBase _powerUpTarget;
        private List<StorageDataItemBase> _selectedItemList;
        private Dictionary<KeyValuePair<System.Type, int>, StorageDataItemBase> _selectedItemMap;
        public const int MAX_SELECT_NUM = 6;

        public void ClearModifyingItem()
        {
            this._modifyingItem = null;
        }

        private void FixListOnModifyNumber(StorageDataItemBase item)
        {
            KeyValuePair<System.Type, int> keyByItem = this.GetKeyByItem(item);
            if (this._selectedItemMap.ContainsKey(keyByItem))
            {
                if (item.number == 0)
                {
                    this._selectedItemList.Remove(this._selectedItemMap[keyByItem]);
                    this._selectedItemMap.Remove(keyByItem);
                }
            }
            else if (item.number > 0)
            {
                this._selectedItemList.Add(item);
                this._selectedItemMap.Add(keyByItem, item);
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshStorageShowing, null));
        }

        public int GetItemSelectNum(StorageDataItemBase item)
        {
            KeyValuePair<System.Type, int> keyByItem = this.GetKeyByItem(item);
            return (!this._selectedItemMap.ContainsKey(keyByItem) ? 0 : this._selectedItemMap[keyByItem].number);
        }

        private KeyValuePair<System.Type, int> GetKeyByItem(StorageDataItemBase item)
        {
            return new KeyValuePair<System.Type, int>(item.GetType(), item.GetIdForKey());
        }

        public bool IsItemInSelectedMap(StorageDataItemBase item)
        {
            KeyValuePair<System.Type, int> keyByItem = this.GetKeyByItem(item);
            return this._selectedItemMap.ContainsKey(keyByItem);
        }

        public void OnClearBtnClick()
        {
            if (this._modifyingItem != null)
            {
                this._modifyingItem.number = 0;
                this.FixListOnModifyNumber(this._modifyingItem);
                this.UpdateDataView();
            }
        }

        public void OnDecreaseBtnClick(StorageDataItemBase dataItem)
        {
            KeyValuePair<System.Type, int> keyByItem = this.GetKeyByItem(dataItem);
            if (this._selectedItemMap.ContainsKey(keyByItem))
            {
                this._modifyingItem = this._selectedItemMap[keyByItem];
            }
            else
            {
                this._modifyingItem = null;
            }
            if ((this._modifyingItem != null) && (this._modifyingItem.number > 0))
            {
                this._modifyingItem.number--;
                this.FixListOnModifyNumber(this._modifyingItem);
                this.UpdateDataView();
            }
        }

        public void OnIncreaseBtnClick()
        {
            if (this._modifyingItem != null)
            {
                int number = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(this._modifyingItem.GetType(), this._modifyingItem.GetIdForKey()).number;
                if (this._modifyingItem.number < number)
                {
                    this._modifyingItem.number++;
                    this.FixListOnModifyNumber(this._modifyingItem);
                    this.UpdateDataView();
                }
            }
        }

        public void OnMaxBtnClick()
        {
            if (this._modifyingItem != null)
            {
                this._modifyingItem.number = Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(this._modifyingItem.GetType(), this._modifyingItem.GetIdForKey()).number;
                this.FixListOnModifyNumber(this._modifyingItem);
                this.UpdateDataView();
            }
        }

        public void OnPowerUpPanelCloseBtnClick()
        {
            Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
        }

        public void OnPowerUpPanelOKBtnClick()
        {
            Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
        }

        public void RefreshOnItemButonClick(StorageDataItemBase item)
        {
            KeyValuePair<System.Type, int> keyByItem = this.GetKeyByItem(item);
            if (this._selectedItemMap.ContainsKey(keyByItem))
            {
                if (this._isMulti)
                {
                    this._modifyingItem = this._selectedItemMap[keyByItem];
                    this.OnIncreaseBtnClick();
                }
                else
                {
                    this._selectedItemList.Remove(this._selectedItemMap[keyByItem]);
                    this._selectedItemMap.Remove(keyByItem);
                }
            }
            else if (this._selectedItemMap.Count < 6)
            {
                StorageDataItemBase base2 = item.Clone();
                base2.number = 1;
                this._selectedItemList.Add(base2);
                this._selectedItemMap.Add(keyByItem, base2);
                if (this._isMulti)
                {
                    this._modifyingItem = base2;
                }
            }
            this.UpdateDataView();
        }

        public void RefreshView(bool isMulti)
        {
            this._isMulti = isMulti;
            this.UpdateDataView();
        }

        public void SetupView(List<StorageDataItemBase> selectedItemList, bool isMulti, StorageDataItemBase powerUpTarget)
        {
            this._selectedItemList = selectedItemList;
            this._powerUpTarget = powerUpTarget;
            this._selectedItemMap = new Dictionary<KeyValuePair<System.Type, int>, StorageDataItemBase>();
            foreach (StorageDataItemBase base2 in this._selectedItemList)
            {
                this._selectedItemMap.Add(this.GetKeyByItem(base2), base2);
            }
            this.RefreshView(isMulti);
        }

        private void UpdateDataView()
        {
            float num;
            float num2;
            UIUtil.CalCulateExpFromItems(out num, out num2, this._selectedItemList, this._powerUpTarget);
            bool flag = UIUtil.CalculateLvWithExp(num2, this._powerUpTarget) >= this._powerUpTarget.GetMaxLevel();
            Text component = base.transform.Find("Content/Exp/Num/Num").GetComponent<Text>();
            component.text = Mathf.RoundToInt(num2).ToString();
            component.color = !flag ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("WarningRed");
            base.transform.Find("Content/Exp/Num/MaxLabel").gameObject.SetActive(flag);
            bool flag2 = num > Singleton<PlayerModule>.Instance.playerData.scoin;
            Text text2 = base.transform.Find("Content/Scoin/Num").GetComponent<Text>();
            text2.text = Mathf.RoundToInt(num).ToString();
            text2.color = !flag2 ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("WarningRed");
        }
    }
}

