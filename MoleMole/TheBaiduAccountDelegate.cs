namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TheBaiduAccountDelegate : TheBaseAccountDelegate
    {
        public TheBaiduAccountDelegate()
        {
            object[] args = new object[] { new AndroidJavaRunnable(this.<TheBaiduAccountDelegate>m__4) };
            base._activity.Call("runOnUiThread", args);
        }

        [CompilerGenerated]
        private void <TheBaiduAccountDelegate>m__4()
        {
            if (base._delegate == null)
            {
                base._delegate = new AndroidJavaObject("com.miHoYo.bh3.baidu.BaiduSDK", new object[0]);
            }
        }

        public override void exit()
        {
            object[] args = new object[] { () => base._delegate.Call("exit", new object[0]) };
            base._activity.Call("runOnUiThread", args);
        }

        public override string getUid()
        {
            return base._delegate.Call<string>("getUid", new object[0]);
        }

        public override void init(bool debugMode, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <init>c__AnonStorey84 storey = new <init>c__AnonStorey84 {
                debugMode = debugMode,
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__5) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, TheBaseAccountDelegate.Function callback)
        {
            <login>c__AnonStorey85 storey = new <login>c__AnonStorey85 {
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__6) };
            base._activity.Call("runOnUiThread", args);
            return null;
        }

        public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <pay>c__AnonStorey86 storey = new <pay>c__AnonStorey86 {
                productID = productID,
                productName = productName,
                productPrice = productPrice,
                tradeNo = tradeNo,
                userID = userID,
                notifyUrl = notifyUrl,
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__7) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, TheBaseAccountDelegate.Function callback)
        {
            return null;
        }

        public void setSwitchAccountListener(string callbackClass, string callbackMethod)
        {
            <setSwitchAccountListener>c__AnonStorey87 storey = new <setSwitchAccountListener>c__AnonStorey87 {
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__8) };
            base._activity.Call("runOnUiThread", args);
        }

        [CompilerGenerated]
        private sealed class <init>c__AnonStorey84
        {
            internal TheBaiduAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal bool debugMode;

            internal void <>m__5()
            {
                object[] args = new object[] { this.debugMode, this.<>f__this._activity, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("init", args);
            }
        }

        [CompilerGenerated]
        private sealed class <login>c__AnonStorey85
        {
            internal TheBaiduAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;

            internal void <>m__6()
            {
                object[] args = new object[] { this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("login", args);
            }
        }

        [CompilerGenerated]
        private sealed class <pay>c__AnonStorey86
        {
            internal TheBaiduAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal string notifyUrl;
            internal string productID;
            internal string productName;
            internal float productPrice;
            internal string tradeNo;
            internal string userID;

            internal void <>m__7()
            {
                object[] args = new object[] { this.productID, this.productName, this.productPrice, this.tradeNo, this.userID, this.notifyUrl, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("pay", args);
            }
        }

        [CompilerGenerated]
        private sealed class <setSwitchAccountListener>c__AnonStorey87
        {
            internal TheBaiduAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;

            internal void <>m__8()
            {
                object[] args = new object[] { this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("setSuspendWindowChangeAccountListener", args);
            }
        }
    }
}

