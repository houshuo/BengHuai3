namespace MoleMole
{
    using System;

    public class PayResult
    {
        public PayRetcode payRetCode;

        public PayResult()
        {
        }

        public PayResult(PayRetcode payRetCode)
        {
            this.payRetCode = payRetCode;
        }

        public virtual string GetResultShowText()
        {
            return string.Empty;
        }

        public enum PayRetcode
        {
            SUCCESS,
            CONFIRMING,
            CANCELED,
            FAILED
        }
    }
}

