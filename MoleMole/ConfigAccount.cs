namespace MoleMole
{
    using System;

    public class ConfigAccount
    {
        public AccountBranch accountBranch;
        public PaymentBranch paymentBranch;

        public ConfigAccount()
        {
            ConfigChannel channel = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
            this.accountBranch = channel.AccountBranch;
            this.paymentBranch = channel.PaymentBranch;
        }

        public enum AccountBranch
        {
            Original,
            UC,
            QIHOO,
            OPPO,
            VIVO,
            HUAWEI,
            XIAOMI,
            TENCENT,
            GIONEE,
            LENOVO,
            BAIDU,
            COOLPAD,
            WANDOUJIA,
            MEIZU,
            BILIBILI
        }

        public enum PaymentBranch
        {
            DEFAULT,
            APPSTORE_CN,
            ORIGINAL_ANDROID_PAY
        }
    }
}

