namespace MoleMole
{
    using Com.Alipay;
    using System;
    using UnityEngine;

    public class TheOriginalAccountDelegate : TheBaseAccountDelegate
    {
        protected AndroidJavaObject _delegate_weixin;
        private static string ALIPAY_PARTNER = "2088021131947549";
        private static string ALIPAY_RSAPRIVATE = "MIICdgIBADANBgkqhkiG9w0BAQEFAASCAmAwggJcAgEAAoGBALLr6IbybR4f7HkAHbtVUI3+fNcN8BGtfDQyP8TzFj80E2xTC8wrdrUj+2oRTVqcK1l7QA08pZr4SgGx/8WtBr5wUEPiKcO5IG1/A9D4s06t3/Gw2uxoxB2fF5I0ci7QwYVI0H8mS/j9GX2z1lqvPXCcGhJq5FMyIDCzHEMibwAXAgMBAAECgYA5fBH5SWpFg3w2ZBMpXP/Enz782T2IcHS3UG2smW1MYS7cXtIrhstc53KfYW+47PQAi9jIZ/PNFniwkr/agvznKKqD5K95g1z0DGtkKIs5rMGrjS04FtRRUw7SCXJODhbH2VLZ+QJj9fLnbPQe23lPrdcns7oVges1bLEyWjn6AQJBAOjpz9kYcZFMwj8lddd2f3qu0B6rSGdaRdrWt9GHRwipqZQFJB+DW9nA3r5xq9+TNg9p38r17A00+tGALaSqboUCQQDEqA0jrVrf95ERk9gp/7NJCSbWW+3glO3abYhjpJbaUfM+MjilLKN0xTMw+SjJJWymrAuWLt/M+6f5QwBX4RzrAkA+Nh2XTikfd1I3DalxOKyKN2FNn9CCEqGv90Q4ChsWHEM4Tzs705lYC2UzlyciW67H5S6qho9bY7hO9x656fAFAkBH7fvYV9kMYH30QvJm8jr+dNV6xGcupOqW4UdowtPWiPECh9YGPFyRImwF9qx/XivujrEyPnTnggi/eE1Q12r/AkEAnsIov6ihXXMEN1/xEerJV3bze3MSga2APRNDwEEMXn3BSUaaQP4iU67QWTmLrTUv9l+uPHqMqqpxCm0Kl5ASgg==";
        private static string ALIPAY_SELLER = "2088021131947549";
        private static string WEIXIN_PAY_APP_ID = "wxc090bb4ff5c352e4";
        private static string WEIXIN_PAY_PACKAGE_VALUE = "Sign=WXPay";
        public WeixinPrepayOrderInfo weixinPrepayOrderInfo;

        public TheOriginalAccountDelegate()
        {
            if (base._delegate == null)
            {
                object[] args = new object[] { base._activity };
                base._delegate = new AndroidJavaObject("com.miHoYo.originpaydelegate.AlipayDelegate", args);
            }
            if (this._delegate_weixin == null)
            {
                object[] objArray2 = new object[] { base._activity, base._handler, WEIXIN_PAY_APP_ID };
                this._delegate_weixin = new AndroidJavaObject("com.miHoYo.originpaydelegate.WeixinPayDelegate", objArray2);
            }
        }

        public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            if ((Singleton<AccountManager>.Instance.accountConfig.paymentBranch != ConfigAccount.PaymentBranch.APPSTORE_CN) && (Singleton<AccountManager>.Instance.accountConfig.paymentBranch == ConfigAccount.PaymentBranch.ORIGINAL_ANDROID_PAY))
            {
                if (Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.ALIPAY)
                {
                    string[] textArray1 = new string[] { "subject=", productID, "&out_trade_no=", tradeNo, "&uid=", userID };
                    string str = SecurityUtil.Md5(string.Concat(textArray1)) + userID;
                    string str4 = (((string.Empty + "partner=\"" + ALIPAY_PARTNER + "\"") + "&out_trade_no=\"" + tradeNo + "\"") + "&subject=\"" + productID + "\"") + "&body=\"" + str + "\"";
                    string content = (((((string.Concat(new object[] { str4, "&total_fee=\"", productPrice, "\"" }) + "&notify_url=\"" + WWW.EscapeURL(notifyUrl) + "\"") + "&service=\"mobile.securitypay.pay\"" + "&_input_charset=\"utf-8\"") + "&return_url=\"" + WWW.EscapeURL("http://m.alipay.com") + "\"") + "&payment_type=\"1\"") + "&seller_id=\"" + ALIPAY_SELLER + "\"") + "&it_b_pay=\"1m\"";
                    string s = RSAFromPkcs8.sign(content, ALIPAY_RSAPRIVATE, "utf-8");
                    content = content + "&sign=\"" + WWW.EscapeURL(s) + "\"&sign_type=\"RSA\"";
                    object[] args = new object[] { content };
                    base._delegate.Call("pay", args);
                }
                else if (Singleton<ChannelPayModule>.Instance.GetPayMethodId() == ChannelPayModule.PayMethod.WEIXIN_PAY)
                {
                    object[] objArray3 = new object[] { WEIXIN_PAY_APP_ID, this.weixinPrepayOrderInfo.partnerID, this.weixinPrepayOrderInfo.prepayID, WEIXIN_PAY_PACKAGE_VALUE, this.weixinPrepayOrderInfo.nonceStr, this.weixinPrepayOrderInfo.timestamp, this.weixinPrepayOrderInfo.sign };
                    this._delegate_weixin.Call("pay", objArray3);
                }
            }
        }

        public class WeixinPrepayOrderInfo
        {
            public string nonceStr;
            public string partnerID;
            public string prepayID;
            public string productID;
            public string productName;
            public float productPrice;
            public string sign;
            public string timestamp;
        }
    }
}

