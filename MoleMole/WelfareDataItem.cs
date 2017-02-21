namespace MoleMole
{
    using proto;
    using System;

    public class WelfareDataItem
    {
        public int payHCoin;
        public int rewardID;
        public VipRewardStatus rewardStatus;
        public int vipLevel;

        public WelfareDataItem(WelfareDataItem welfareItem)
        {
            this.vipLevel = welfareItem.vipLevel;
            this.payHCoin = welfareItem.payHCoin;
            this.rewardID = welfareItem.rewardID;
            this.rewardStatus = welfareItem.rewardStatus;
        }

        public WelfareDataItem(VipReward welfareItem)
        {
            this.vipLevel = (int) welfareItem.get_vip_level();
            this.payHCoin = (int) welfareItem.get_pay_hcoin();
            this.rewardID = (int) welfareItem.get_reward_id();
            this.rewardStatus = welfareItem.get_status();
        }

        public override string ToString()
        {
            object[] args = new object[] { this.vipLevel, this.payHCoin, this.rewardID, this.rewardStatus };
            return string.Format("<WelfareDataItem>\nID: {0}\npayHCoin: {1}\nrewardID: {2}\nrewardStatus: {3}", args);
        }
    }
}

