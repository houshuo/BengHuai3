namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;

    public class MonoMiscDisjointOutputUI : MonoBehaviour
    {
        private StringBuilder _log = new StringBuilder();
        [SerializeField]
        private Transform itemPrefab;

        private void AddItem(int id, int num)
        {
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id, 1);
            dummyStorageDataItem.number = num;
            MonoItemIconButton component = base.transform.AddChildFromPrefab(this.itemPrefab, null).GetComponent<MonoItemIconButton>();
            component.SetupView(dummyStorageDataItem, MonoItemIconButton.SelectMode.None, false, false, false);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemButonClick));
        }

        private void OnItemButonClick(StorageDataItemBase item, bool selected)
        {
            UIUtil.ShowItemDetail(item, true, true);
        }

        private void ResetView()
        {
            base.transform.DestroyChildren();
        }

        private void ReturnMaterial(StorageDataItemBase input)
        {
            WeaponDataItem item = input as WeaponDataItem;
            List<int> evoPath = WeaponMetaDataReaderExtend.GetEvoPath(item.ID);
            int num = 0;
            foreach (int num2 in evoPath)
            {
                int accumulateExp = 0;
                if (num2 == item.ID)
                {
                    int expType = item.GetExpType();
                    int exp = item.exp;
                    accumulateExp = EquipmentLevelMetaDataReaderExtend.GetAccumulateExp(item.level, expType) + exp;
                }
                else
                {
                    WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey(num2);
                    int type = weaponMetaDataByKey.expType;
                    accumulateExp = EquipmentLevelMetaDataReaderExtend.GetAccumulateExp(weaponMetaDataByKey.maxLv, type);
                }
                num += accumulateExp;
            }
            float num9 = Singleton<PlayerModule>.Instance.playerData.disjoin_equipment_back_exp_percent * 0.01f;
            int num11 = (int) (num * num9);
            int[] numArray = new int[] { 0xbbc, 0xbbb, 0xbba, 0xbb9 };
            foreach (int num12 in numArray)
            {
                float gearExpProvideBase = ItemMetaDataReader.GetItemMetaDataByKey(num12).gearExpProvideBase;
                float num15 = MaterialExpBonusMetaDataReader.GetMaterialExpBonusMetaDataByKey(num12).weaponExpBonus * 0.01f;
                int num16 = (int) (gearExpProvideBase * num15);
                int num17 = num11 / num16;
                num11 = num11 % num16;
                if (num17 > 0)
                {
                    this.AddItem(num12, num17);
                }
            }
        }

        public void SetupView(StorageDataItemBase input)
        {
            this.ResetView();
            this._log.Length = 0;
            this._log.Append(string.Format("[Disjoint] {0} --> ", input.ID));
            foreach (CabinDisjointEquipmentMetaData.CabinDisjointOutputItem item in CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(input.ID).Item)
            {
                this.AddItem(item.ID, item.Num);
                this._log.Append(string.Format("{0} x{1}, ", item.ID, item.Num));
            }
            this.ReturnMaterial(input);
        }
    }
}

