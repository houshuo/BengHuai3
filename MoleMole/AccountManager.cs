namespace MoleMole
{
    using System;

    public class AccountManager
    {
        public readonly ConfigAccount accountConfig = new ConfigAccount();
        public OpeUtil.ApkCommentInfo apkCommentInfo;
        public string apkSignature;
        public readonly TheBaseAccountManager manager;

        private AccountManager()
        {
            switch (this.accountConfig.accountBranch)
            {
                case ConfigAccount.AccountBranch.Original:
                    this.manager = new TheOriginalAccountManager();
                    break;

                case ConfigAccount.AccountBranch.UC:
                    this.manager = new TheUCAccountManager();
                    break;

                case ConfigAccount.AccountBranch.QIHOO:
                    this.manager = new TheQihooAccountManager();
                    break;

                case ConfigAccount.AccountBranch.OPPO:
                    this.manager = new TheOppoAccountManager();
                    break;

                case ConfigAccount.AccountBranch.VIVO:
                    this.manager = new TheVivoAccountManager();
                    break;

                case ConfigAccount.AccountBranch.HUAWEI:
                    this.manager = new TheHuaweiAccountManager();
                    break;

                case ConfigAccount.AccountBranch.XIAOMI:
                    this.manager = new TheXiaoMiAccountManager();
                    break;

                case ConfigAccount.AccountBranch.TENCENT:
                    this.manager = new TheTencentAccountManager();
                    break;

                case ConfigAccount.AccountBranch.GIONEE:
                    this.manager = new TheAmigoAccountManager();
                    break;

                case ConfigAccount.AccountBranch.LENOVO:
                    this.manager = new TheLenovoAccountManager();
                    break;

                case ConfigAccount.AccountBranch.BAIDU:
                    this.manager = new TheBaiduAccountManager();
                    break;

                case ConfigAccount.AccountBranch.COOLPAD:
                    this.manager = new TheCoolpadAccountManager();
                    break;

                case ConfigAccount.AccountBranch.WANDOUJIA:
                    this.manager = new TheWandouAccountManager();
                    break;

                case ConfigAccount.AccountBranch.MEIZU:
                    this.manager = new TheMeizuAccountManager();
                    break;

                case ConfigAccount.AccountBranch.BILIBILI:
                    this.manager = new TheBiliAccountManager();
                    break;

                default:
                    throw new Exception("Invalid Type or State!: " + this.accountConfig.accountBranch);
            }
        }

        public bool AllowTryUserLogin()
        {
            return (this.accountConfig.accountBranch == ConfigAccount.AccountBranch.Original);
        }

        public void SetupApkCommentInfo()
        {
            this.apkCommentInfo = null;
            this.apkCommentInfo = OpeUtil.GetApkComment();
        }
    }
}

