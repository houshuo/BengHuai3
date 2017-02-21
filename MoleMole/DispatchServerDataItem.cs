namespace MoleMole
{
    using SimpleJSON;
    using System;

    public class DispatchServerDataItem
    {
        public readonly string accountUrl;
        public readonly string assetBundleUrl;
        public readonly bool dataUseAssetBundle;
        public readonly bool dataUseAssetBundleUseSever;
        public readonly bool forbidNewUser;
        public readonly bool forbidRecharge;
        public readonly string host;
        public readonly bool isReview;
        public readonly string oaServerUrl;
        public readonly ushort port;
        public readonly int rechargeMaxLimit;
        public readonly bool resUseAssetBundle;
        public readonly bool resUseAssetBundleUseSever;
        public readonly bool showVersionText;

        public DispatchServerDataItem(JSONNode json)
        {
            this.host = (string) json["gateway"]["ip"];
            this.port = (ushort) json["gateway"]["port"].AsInt;
            this.assetBundleUrl = (string) json["asset_boundle_url"];
            this.accountUrl = (string) json["account_url"];
            this.isReview = !string.IsNullOrEmpty((string) json["ext"]["is_xxxx"]) && (json["ext"]["is_xxxx"].AsInt == 1);
            this.forbidNewUser = !string.IsNullOrEmpty((string) json["ext"]["forbid_new_user"]) && (json["ext"]["forbid_new_user"].AsInt == 1);
            this.forbidRecharge = !string.IsNullOrEmpty((string) json["ext"]["forbid_recharge"]) && (json["ext"]["forbid_recharge"].AsInt == 1);
            this.rechargeMaxLimit = !string.IsNullOrEmpty((string) json["ext"]["recharge_max_limit"]) ? json["ext"]["recharge_max_limit"].AsInt : 0;
            this.oaServerUrl = (string) json["oaserver_url"];
            this.dataUseAssetBundleUseSever = !string.IsNullOrEmpty((string) json["ext"]["data_use_asset_boundle"]);
            this.resUseAssetBundleUseSever = !string.IsNullOrEmpty((string) json["ext"]["res_use_asset_boundle"]);
            this.dataUseAssetBundle = !string.IsNullOrEmpty((string) json["ext"]["data_use_asset_boundle"]) && (json["ext"]["data_use_asset_boundle"].AsInt == 1);
            this.resUseAssetBundle = !string.IsNullOrEmpty((string) json["ext"]["res_use_asset_boundle"]) && (json["ext"]["res_use_asset_boundle"].AsInt == 1);
            this.showVersionText = !string.IsNullOrEmpty((string) json["ext"]["show_version_text"]) && (json["ext"]["show_version_text"].AsInt == 1);
        }
    }
}

