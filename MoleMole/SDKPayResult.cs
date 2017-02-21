namespace MoleMole
{
    using System;

    public class SDKPayResult : PayResult
    {
        public override string GetResultShowText()
        {
            if (base.payRetCode == PayResult.PayRetcode.FAILED)
            {
                return LocalizationGeneralLogic.GetText("IAPTransitionFailed", new object[0]);
            }
            return base.GetResultShowText();
        }
    }
}

