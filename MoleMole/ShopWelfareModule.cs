namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class ShopWelfareModule : BaseModule
    {
        private bool _readyToHint;
        private List<WelfareDataItem> _welfareDataItemList;
        [CompilerGenerated]
        private static Comparison<WelfareDataItem> <>f__am$cache3;
        [CompilerGenerated]
        private static Comparison<WelfareDataItem> <>f__am$cache4;
        [CompilerGenerated]
        private static Comparison<WelfareDataItem> <>f__am$cache5;

        public ShopWelfareModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._welfareDataItemList = new List<WelfareDataItem>();
        }

        public List<WelfareDataItem> GetWelfareDataItemList()
        {
            return this._welfareDataItemList;
        }

        public bool HasWelfareCanGet()
        {
            foreach (WelfareDataItem item in this._welfareDataItemList)
            {
                if (item.rewardStatus == 2)
                {
                    return true;
                }
            }
            return false;
        }

        private bool OnGetVipRewardDataRsp(GetVipRewardDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._welfareDataItemList.Clear();
                this.totalPayHCoin = (int) rsp.get_total_pay_hcoin();
                foreach (VipReward reward in rsp.get_vip_reward_list())
                {
                    this._welfareDataItemList.Add(new WelfareDataItem(reward));
                }
                if (!this._readyToHint)
                {
                    this._readyToHint = true;
                    int num = 0;
                    int count = this._welfareDataItemList.Count;
                    while (num < count)
                    {
                        this._readyToHint &= this._welfareDataItemList[num].rewardStatus == 1;
                        num++;
                    }
                }
                this.SortVipRewardList();
            }
            foreach (WelfareDataItem item in this._welfareDataItemList)
            {
            }
            return false;
        }

        private bool OnGetVipRewardRsp(GetVipRewardRsp rsp)
        {
            Singleton<NetworkManager>.Instance.RequestGetVipRewardData();
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0xc6:
                    return this.OnGetVipRewardDataRsp(pkt.getData<GetVipRewardDataRsp>());

                case 200:
                    return this.OnGetVipRewardRsp(pkt.getData<GetVipRewardRsp>());
            }
            return false;
        }

        private void SortVipRewardList()
        {
            List<WelfareDataItem> list = new List<WelfareDataItem>();
            List<WelfareDataItem> list2 = new List<WelfareDataItem>();
            List<WelfareDataItem> list3 = new List<WelfareDataItem>();
            foreach (WelfareDataItem item in this._welfareDataItemList)
            {
                if (item.rewardStatus == 2)
                {
                    list.Add(item);
                }
                else if (item.rewardStatus == 1)
                {
                    list2.Add(item);
                }
                else if (item.rewardStatus == 3)
                {
                    list3.Add(item);
                }
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = (lo, ro) => lo.payHCoin - ro.payHCoin;
            }
            list.Sort(<>f__am$cache3);
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = (lo, ro) => lo.payHCoin - ro.payHCoin;
            }
            list2.Sort(<>f__am$cache4);
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = (lo, ro) => lo.payHCoin - ro.payHCoin;
            }
            list3.Sort(<>f__am$cache5);
            int num = 0;
            for (int i = 0; i < list.Count; i++)
            {
                this._welfareDataItemList[num] = list[i];
                num++;
            }
            for (int j = 0; j < list2.Count; j++)
            {
                this._welfareDataItemList[num] = list2[j];
                num++;
            }
            for (int k = 0; k < list3.Count; k++)
            {
                this._welfareDataItemList[num] = list3[k];
                num++;
            }
        }

        public void TryHintNewWelfare()
        {
            if (this._readyToHint && this.HasWelfareCanGet())
            {
                this._readyToHint = false;
                GeneralConfirmDialogContext dialogContext = new GeneralConfirmDialogContext {
                    type = GeneralConfirmDialogContext.ButtonType.SingleButton,
                    desc = LocalizationGeneralLogic.GetText("Menu_WelfareGuideTitle", new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        public int totalPayHCoin { get; private set; }
    }
}

