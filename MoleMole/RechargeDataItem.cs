namespace MoleMole
{
    using proto;
    using System;

    public class RechargeDataItem
    {
        public int cardDailyHardCoin;
        public int cardLeftDays;
        public string formattedPrice;
        public int freeHardCoin;
        public int leftBuyTimes;
        public int payHardCoin;
        public string productID;
        public string productName;
        public ProductType productType;
        public int serverPrice;

        public RechargeDataItem(RechargeDataItem rechargeItem)
        {
            this.formattedPrice = string.Empty;
            this.productID = rechargeItem.productID;
            this.productName = rechargeItem.productName;
            this.formattedPrice = rechargeItem.formattedPrice;
            this.productType = rechargeItem.productType;
            this.payHardCoin = rechargeItem.payHardCoin;
            this.freeHardCoin = rechargeItem.freeHardCoin;
            this.serverPrice = rechargeItem.serverPrice;
            this.leftBuyTimes = rechargeItem.leftBuyTimes;
            this.cardDailyHardCoin = rechargeItem.cardDailyHardCoin;
            this.cardLeftDays = rechargeItem.cardLeftDays;
        }

        public RechargeDataItem(Product product)
        {
            this.formattedPrice = string.Empty;
            this.productID = product.get_name();
            this.productName = product.get_desc();
            this.formattedPrice = "\x00a5" + ((((float) product.get_price()) / 100f)).ToString();
            this.productType = product.get_type();
            this.payHardCoin = (int) product.get_pay_hcoin();
            this.freeHardCoin = (int) product.get_free_hcoin();
            this.serverPrice = (int) product.get_price();
            this.leftBuyTimes = (int) product.get_left_buy_times();
            this.cardDailyHardCoin = (int) product.get_card_daily_hcoin();
            this.cardLeftDays = (int) product.get_card_left_days();
        }

        public bool CanPurchase()
        {
            if ((this.productType == 2) && (this.leftBuyTimes <= 0))
            {
                return false;
            }
            if ((this.productType == 3) && (this.cardLeftDays >= 180))
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.productID, this.productName, this.formattedPrice, this.productType, this.payHardCoin, this.freeHardCoin, this.serverPrice, this.leftBuyTimes, this.cardDailyHardCoin, this.cardLeftDays };
            return string.Format("<RechargeDataItem>\nID: {0}\nname: {1}\nformattedPrice: {2}\ntype: {3}\npayHCoin: {4}\nfreeHCoin: {5}\nserverPrice: {6}\nleftBuyTimes: {7}\ncardDailyHCoin: {8}\ncardLeftDays: {9}", args);
        }

        public void UpdateFromProduct(Product product)
        {
            this.productName = product.get_desc();
            this.productType = product.get_type();
            this.payHardCoin = (int) product.get_pay_hcoin();
            this.freeHardCoin = (int) product.get_free_hcoin();
            this.serverPrice = (int) product.get_price();
            this.leftBuyTimes = (int) product.get_left_buy_times();
            this.cardDailyHardCoin = (int) product.get_card_daily_hcoin();
            this.cardLeftDays = (int) product.get_card_left_days();
        }
    }
}

