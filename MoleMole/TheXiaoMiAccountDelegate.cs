namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TheXiaoMiAccountDelegate : TheBaseAccountDelegate
    {
        public TheXiaoMiAccountDelegate()
        {
            object[] args = new object[] { new AndroidJavaRunnable(this.<TheXiaoMiAccountDelegate>m__43) };
            base._activity.Call("runOnUiThread", args);
        }

        [CompilerGenerated]
        private void <TheXiaoMiAccountDelegate>m__43()
        {
            if (base._delegate == null)
            {
                base._delegate = new AndroidJavaObject("com.miHoYo.bh3.mi.misdkutils", new object[0]);
            }
        }

        public override void exit()
        {
        }

        public override string getUid()
        {
            return base._delegate.Call<string>("getUid", new object[0]);
        }

        public override void hideToolBar()
        {
        }

        public override void init(bool debugMode, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <init>c__AnonStoreyAB yab = new <init>c__AnonStoreyAB {
                debugMode = debugMode,
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(yab.<>m__44) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, TheBaseAccountDelegate.Function callback)
        {
            object[] args = new object[] { callbackClass, callbackMethod };
            base._delegate.Call("login", args);
            return null;
        }

        public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <pay>c__AnonStoreyAC yac = new <pay>c__AnonStoreyAC {
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
            object[] args = new object[] { new AndroidJavaRunnable(yac.<>m__45) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, TheBaseAccountDelegate.Function callback)
        {
            return null;
        }

        public override void showPausePage()
        {
        }

        public override void showToolBar()
        {
        }

        public override void showUserCenter()
        {
        }

        [CompilerGenerated]
        private sealed class <init>c__AnonStoreyAB
        {
            internal TheXiaoMiAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal bool debugMode;

            internal void <>m__44()
            {
                object[] args = new object[] { this.debugMode, this.<>f__this._activity, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("init", args);
            }
        }

        [CompilerGenerated]
        private sealed class <pay>c__AnonStoreyAC
        {
            internal TheXiaoMiAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal string notifyUrl;
            internal string productID;
            internal string productName;
            internal float productPrice;
            internal string tradeNo;
            internal string userID;

            internal void <>m__45()
            {
                object[] args = new object[] { this.productID, this.productName, this.productPrice, this.tradeNo, this.userID, this.notifyUrl, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("pay", args);
            }
        }
    }
}

