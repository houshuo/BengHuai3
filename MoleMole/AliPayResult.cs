namespace MoleMole
{
    using System;

    public class AliPayResult : PayResult
    {
        private string memo;
        private string result;
        private string resultStatus;

        public AliPayResult(string rawResult)
        {
            if (!string.IsNullOrEmpty(rawResult))
            {
                char[] separator = new char[] { ';' };
                foreach (string str in rawResult.Split(separator))
                {
                    if (str.StartsWith("resultStatus"))
                    {
                        this.resultStatus = this.GetValue(str, "resultStatus");
                    }
                    else if (str.StartsWith("result"))
                    {
                        this.result = this.GetValue(str, "result");
                    }
                    else if (str.StartsWith("memo"))
                    {
                        this.memo = this.GetValue(str, "memo");
                    }
                }
                if (this.resultStatus == "9000")
                {
                    base.payRetCode = PayResult.PayRetcode.SUCCESS;
                }
                else if (this.resultStatus == "8000")
                {
                    base.payRetCode = PayResult.PayRetcode.CONFIRMING;
                }
                else if (this.resultStatus == "6001")
                {
                    base.payRetCode = PayResult.PayRetcode.CANCELED;
                }
                else
                {
                    base.payRetCode = PayResult.PayRetcode.FAILED;
                }
            }
        }

        public string GetMemo()
        {
            return this.memo;
        }

        public string GetResult()
        {
            return this.result;
        }

        public override string GetResultShowText()
        {
            if (base.payRetCode == PayResult.PayRetcode.FAILED)
            {
                return LocalizationGeneralLogic.GetText("IAPTransitionFailed", new object[0]);
            }
            return base.GetResultShowText();
        }

        public string GetResultStatus()
        {
            return this.resultStatus;
        }

        private string GetValue(string content, string key)
        {
            string str = key + "={";
            int startIndex = content.IndexOf(str) + str.Length;
            int length = content.LastIndexOf("}") - startIndex;
            return content.Substring(startIndex, length);
        }
    }
}

