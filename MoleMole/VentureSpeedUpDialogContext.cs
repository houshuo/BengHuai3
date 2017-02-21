namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class VentureSpeedUpDialogContext : BaseDialogContext
    {
        private int _num_materials;
        private MaterialDataItem _selectedItem;
        private List<StorageDataItemBase> _showItemList;
        private VentureDataItem _ventureData;

        public VentureSpeedUpDialogContext(VentureDataItem ventureData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "VentureSpeedUpDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/VentureSpeedUpDialog"
            };
            base.config = pattern;
            this._ventureData = ventureData;
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

        public bool IsEnough(int num)
        {
            int num3 = MaterialVentureSpeedUpDataReader.GetMaterialVentureSpeedUpDataByKey(this._selectedItem.ID).SpeedUpTime * num;
            TimeSpan span = TimeSpan.FromSeconds((double) num3);
            return (this._ventureData.endTime.Subtract(span) <= TimeUtil.Now);
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
            if (this._num_materials > 1)
            {
                this._num_materials--;
                this.UpdateInfo();
            }
        }

        public void OnIncreaseBtnClick()
        {
            if ((this._num_materials < this._selectedItem.number) && !this.IsEnough(this._num_materials))
            {
                this._num_materials++;
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
            if (this._ventureData.status == VentureDataItem.VentureStatus.Done)
            {
                this.Destroy();
            }
            else
            {
                Singleton<NetworkManager>.Instance.RequestSpeedUpIslandVenture(this._ventureData.VentureID, this._selectedItem.ID, this._num_materials);
                this.Destroy();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return false;
        }

        private void SetupList()
        {
            base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
        }

        protected override bool SetupView()
        {
            this._num_materials = 1;
            this._showItemList = Singleton<StorageModule>.Instance.GetAllVentureSpeedUpMaterial();
            this.SetupList();
            this.UpdateSelectedItem(this._selectedItem);
            return false;
        }

        private void UpdateInfo()
        {
            base.view.transform.Find("Dialog/Content/Info/UseNum/Text").GetComponent<Text>().text = this._num_materials.ToString();
            int timeSpan = MaterialVentureSpeedUpDataReader.GetMaterialVentureSpeedUpDataByKey(this._selectedItem.ID).SpeedUpTime * this._num_materials;
            base.view.transform.Find("Dialog/Content/Info/Duration/Group/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(timeSpan);
            TimeSpan span = TimeSpan.FromSeconds((double) timeSpan);
            DateTime targetTime = this._ventureData.endTime.Subtract(span);
            base.view.transform.Find("Dialog/Content/Info/Remain/Group/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(targetTime, null, null, false);
        }

        private void UpdateSelectedItem(StorageDataItemBase selectedItem)
        {
            this._selectedItem = (selectedItem == null) ? (this._showItemList[0] as MaterialDataItem) : (selectedItem as MaterialDataItem);
            this._num_materials = 1;
            base.view.transform.Find("Dialog/Content/Materials").GetComponent<MonoGridScroller>().RefreshCurrent();
            this.UpdateInfo();
        }
    }
}

