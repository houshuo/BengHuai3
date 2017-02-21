namespace MoleMole
{
    using System;

    public class WeixinPayResult : PayResult
    {
        private string _result;

        public WeixinPayResult(string rawResult)
        {
            if (!string.IsNullOrEmpty(rawResult))
            {
                this._result = rawResult;
                if (this._result == "Succ")
                {
                    base.payRetCode = PayResult.PayRetcode.SUCCESS;
                }
                else if (this._result == "ErrUserCancel")
                {
                    base.payRetCode = PayResult.PayRetcode.CANCELED;
                }
                else
                {
                    base.payRetCode = PayResult.PayRetcode.FAILED;
                }
            }
        }

        public override string GetResultShowText()
        {
            if (base.payRetCode != PayResult.PayRetcode.FAILED)
            {
                return base.GetResultShowText();
            }
            if (this._result == "ErrWXAppNotInstalled")
            {
                return LocalizationGeneralLogic.GetText("IAP_ErrWXAppNotInstalled", new object[0]);
            }
            if (this._result == "ErrWXAppNotSupportAPI")
            {
                return LocalizationGeneralLogic.GetText("IAP_ErrWXAppNotSupportAPI", new object[0]);
            }
            if (this._result == "ErrFailCreateWeiXinOrder")
            {
                return LocalizationGeneralLogic.GetText("IAP_FailCreateWeiXinOrder", new object[0]);
            }
            return LocalizationGeneralLogic.GetText("IAPTransitionFailed", new object[0]);
        }
    }
}

