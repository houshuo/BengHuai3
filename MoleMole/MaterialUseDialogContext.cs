namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class MaterialUseDialogContext : BaseDialogContext
    {
        private MaterialDataItem _selectedItem;
        private List<StorageDataItemBase> _showItemList;
        private int _useNumber;
        public AvatarDataItem avatarData;

        public MaterialUseDialogContext(AvatarDataItem avatarData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MaterialUseDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/UseMaterialDialog"
            };
            base.config = pattern;
            this.avatarData = avatarData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Info/UseNum/DecreaseBtn").GetComponent<Button>(), new UnityAction(this.OnDecreaseBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/Info/UseNum/IncreaseBtn").GetComponent<Button>(), new UnityAction(this.OnIncreaseBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnCancelBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnCancelBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
        }

        private int CalculateLevelAfter(AvatarDataItem avatar, float addExp, out bool isAfterLevelMax)
        {
            int avatarLevelLimit = Singleton<PlayerModule>.Instance.playerData.AvatarLevelLimit;
            List<AvatarLevelMetaData> itemList = AvatarLevelMetaDataReader.GetItemList();
            int num2 = Mathf.Min(itemList.Count, avatarLevelLimit);
            float num3 = addExp + avatar.exp;
            int level = avatar.level;
            while ((num3 > 0f) && (level < num2))
            {
                if (itemList[level - 1].exp > num3)
                {
                    break;
                }
                num3 -= itemList[level - 1].exp;
                level++;
            }
            isAfterLevelMax = ((num3 > 0f) && (level == num2)) && (itemList[level - 1].exp <= num3);
            return level;
        }

        private bool OnAddAvatarExpByMaterialRsp(AddAvatarExpByMaterialRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                UIUtil.UpdateAvatarSkillStatusInLocalData(this.avatarData);
            }
            this.Destroy();
            return false;
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public void OnCancelBtnClick()
        {
            this.Destroy();
        }

        private void OnChange(Transform trans, int index)
        {
            bool isSelected = this._showItemList[index] == this._selectedItem;
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.SetupView(this._showItemList[index], MonoItemIconButton.SelectMode.SmallWhenUnSelect, isSelected, false, false);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
        }

        public void OnDecreaseBtnClick()
        {
            if ((this._selectedItem != null) && (this._useNumber > 1))
            {
                this._useNumber--;
                this.UpdateInfo();
            }
        }

        private bool OnGetEquipmentDataRsp(GetEquipmentDataRsp rsp)
        {
            this._useNumber = 1;
            if (Singleton<StorageModule>.Instance.GetStorageItemByTypeAndID(typeof(MaterialDataItem), this._selectedItem.ID) == null)
            {
                this._selectedItem = null;
            }
            if (this._selectedItem == null)
            {
                this._showItemList = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial();
                if (this._showItemList.Count <= 0)
                {
                    this.Destroy();
                    return false;
                }
                this.SetupList();
            }
            this.UpdateSelectedItem(this._selectedItem);
            return false;
        }

        public void OnIncreaseBtnClick()
        {
            if ((this._selectedItem != null) && (this._useNumber < this._selectedItem.number))
            {
                this._useNumber++;
                this.UpdateInfo();
            }
        }

        private void OnItemClick(StorageDataItemBase item, bool selected)
        {
            if (!selected)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectItemIconChange, item));
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SelectItemIconChange)
            {
                this.UpdateSelectedItem((StorageDataItemBase) ntf.body);
            }
            return false;
        }

        public void OnOKBtnClick()
        {
            if ((this._selectedItem != null) && (this._useNumber > 0))
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BeforeAvatarLevelUp, this.avatarData));
                Singleton<NetworkManager>.Instance.RequestAddAvatarExpByMaterial(this.avatarData.avatarID, this._selectedItem.ID, this._useNumber);
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x1b:
                    return this.OnGetEquipmentDataRsp(pkt.getData<GetEquipmentDataRsp>());

                case 0x24:
                    return this.OnAddAvatarExpByMaterialRsp(pkt.getData<AddAvatarExpByMaterialRsp>());
            }
            return false;
        }

        private void SetupList()
        {
            base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
        }

        protected override bool SetupView()
        {
            this._useNumber = 1;
            this._showItemList = Singleton<StorageModule>.Instance.GetAllAvatarExpAddMaterial();
            if (this._showItemList.Count > 0)
            {
                this.SetupList();
                this.UpdateSelectedItem(this._selectedItem);
            }
            return false;
        }

        private void UpdateInfo()
        {
            MaterialAvatarExpBonusMetaData data = MaterialAvatarExpBonusMetaDataReader.TryGetMaterialAvatarExpBonusMetaDataByKey(this._selectedItem.ID);
            float biologyExpBonus = 100f;
            if (data != null)
            {
                switch (this.avatarData.Attribute)
                {
                    case 1:
                        biologyExpBonus = data.biologyExpBonus;
                        break;

                    case 2:
                        biologyExpBonus = data.psychoExpBonus;
                        break;

                    case 3:
                        biologyExpBonus = data.mechanicExpBonus;
                        break;
                }
            }
            biologyExpBonus /= 100f;
            float addExp = (this._selectedItem.GetAvatarExpProvideNum() * biologyExpBonus) * this._useNumber;
            bool isAfterLevelMax = false;
            int num3 = this.CalculateLevelAfter(this.avatarData, addExp, out isAfterLevelMax);
            base.view.transform.Find("Dialog/Content/Info/UseNum/Text").GetComponent<Text>().text = this._useNumber.ToString();
            base.view.transform.Find("Dialog/Content/Info/Exp/Num").GetComponent<Text>().text = addExp.ToString();
            base.view.transform.Find("Dialog/Content/Info/Level/LevelNow").GetComponent<Text>().text = this.avatarData.level.ToString();
            base.view.transform.Find("Dialog/Content/Info/Level/LevelAfter/Num").GetComponent<Text>().text = num3.ToString();
            base.view.transform.Find("Dialog/Content/Info/Level/LevelAfter/LvMax").gameObject.SetActive(isAfterLevelMax);
            base.view.transform.Find("Dialog/Content/Info/UseNum/IncreaseBtn").GetComponent<Button>().interactable = !isAfterLevelMax;
        }

        private void UpdateSelectedItem(StorageDataItemBase selectedItem)
        {
            if (selectedItem != null)
            {
                this._selectedItem = selectedItem as MaterialDataItem;
            }
            else
            {
                this._selectedItem = this._showItemList[0] as MaterialDataItem;
            }
            this._useNumber = 1;
            base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().RefreshCurrent();
            this.UpdateInfo();
        }
    }
}

