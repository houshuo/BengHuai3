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

    public class SellConfirmDialogContext : BaseDialogContext
    {
        [CompilerGenerated]
        private static Predicate<StorageDataItemBase> <>f__am$cache1;
        public readonly List<StorageDataItemBase> sellList;

        public SellConfirmDialogContext(List<StorageDataItemBase> sellList)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SellConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SellConfirmDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this.sellList = sellList;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        private int CalculateTotalSCoinSell()
        {
            float f = 0f;
            foreach (StorageDataItemBase base2 in this.sellList)
            {
                f += base2.GetPriceForSell() * base2.number;
            }
            return Mathf.FloorToInt(f);
        }

        public void Close()
        {
            this.Destroy();
        }

        private bool HasRareItem()
        {
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = x => x.rarity >= 3;
            }
            return (this.sellList.Find(<>f__am$cache1) != null);
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        public void OnOKButtonCallBack()
        {
            Singleton<NetworkManager>.Instance.RequestEquipmentSell(this.sellList);
            this.Close();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSellViewActive, false));
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/WarningText").gameObject.SetActive(this.HasRareItem());
            base.view.transform.Find("Dialog/Content/SCoin/Num").GetComponent<Text>().text = this.CalculateTotalSCoinSell().ToString();
            return false;
        }
    }
}

