namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TheUCAccountDelegate : TheBaseAccountDelegate
    {
        public TheUCAccountDelegate()
        {
            object[] args = new object[] { new AndroidJavaRunnable(this.<TheUCAccountDelegate>m__35) };
            base._activity.Call("runOnUiThread", args);
        }

        [CompilerGenerated]
        private void <TheUCAccountDelegate>m__35()
        {
            if (base._delegate == null)
            {
                base._delegate = new AndroidJavaObject("com.miHoYo.bh3.uc.UCSDKUtils", new object[0]);
            }
        }

        public override void exit()
        {
            object[] args = new object[] { () => base._delegate.Call("ucExit", new object[0]) };
            base._activity.Call("runOnUiThread", args);
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
            <init>c__AnonStoreyA2 ya = new <init>c__AnonStoreyA2 {
                debugMode = debugMode,
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(ya.<>m__36) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, TheBaseAccountDelegate.Function callback)
        {
            <login>c__AnonStoreyA3 ya = new <login>c__AnonStoreyA3 {
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(ya.<>m__37) };
            base._activity.Call("runOnUiThread", args);
            return null;
        }

        public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <pay>c__AnonStoreyA4 ya = new <pay>c__AnonStoreyA4 {
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
            object[] args = new object[] { new AndroidJavaRunnable(ya.<>m__38) };
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
        private sealed class <init>c__AnonStoreyA2
        {
            internal TheUCAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal bool debugMode;

            internal void <>m__36()
            {
                object[] args = new object[] { this.debugMode, this.<>f__this._activity, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("init", args);
            }
        }

        [CompilerGenerated]
        private sealed class <login>c__AnonStoreyA3
        {
            internal TheUCAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;

            internal void <>m__37()
            {
                object[] args = new object[] { this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("login", args);
            }
        }

        [CompilerGenerated]
        private sealed class <pay>c__AnonStoreyA4
        {
            internal TheUCAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal string notifyUrl;
            internal string productID;
            internal string productName;
            internal float productPrice;
            internal string tradeNo;
            internal string userID;

            internal void <>m__38()
            {
                object[] args = new object[] { this.productID, this.productName, this.productPrice, this.tradeNo, this.userID, this.notifyUrl, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("pay", args);
            }
        }
    }
}

