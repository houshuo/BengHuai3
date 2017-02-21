namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TheBiliAccountDelegate : TheBaseAccountDelegate
    {
        private static string APP_ID = "180";
        private static string APP_KEY = "dbf8f1b4496f430b8a3c0f436a35b931";
        private static string MERCHANT_ID = "18";
        private static string SERVER_ID = "378";

        public TheBiliAccountDelegate()
        {
            if (base._delegate == null)
            {
                base._delegate = new AndroidJavaObject("com.miHoYo.bh3.bilibili.BiliAgent", new object[0]);
            }
        }

        public void createRole(string uid)
        {
            <createRole>c__AnonStorey8C storeyc = new <createRole>c__AnonStorey8C {
                uid = uid,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storeyc.<>m__E) };
            base._activity.Call("runOnUiThread", args);
        }

        public override void exit()
        {
        }

        public override string getUid()
        {
            return base._delegate.Call<string>("getUid", new object[0]);
        }

        public override string getUsername()
        {
            return base._delegate.Call<string>("getUsername", new object[0]);
        }

        public override void hideToolBar()
        {
        }

        public override void init(bool debugMode, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <init>c__AnonStorey88 storey = new <init>c__AnonStorey88 {
                debugMode = debugMode,
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__A) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, TheBaseAccountDelegate.Function callback)
        {
            <login>c__AnonStorey89 storey = new <login>c__AnonStorey89 {
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storey.<>m__B) };
            base._activity.Call("runOnUiThread", args);
            return null;
        }

        public override void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, TheBaseAccountDelegate.Function callback)
        {
            <pay>c__AnonStorey8B storeyb = new <pay>c__AnonStorey8B {
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
            object[] args = new object[] { new AndroidJavaRunnable(storeyb.<>m__D) };
            base._activity.Call("runOnUiThread", args);
        }

        public override IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, TheBaseAccountDelegate.Function callback)
        {
            <register>c__AnonStorey8A storeya = new <register>c__AnonStorey8A {
                callbackClass = callbackClass,
                callbackMethod = callbackMethod,
                <>f__this = this
            };
            object[] args = new object[] { new AndroidJavaRunnable(storeya.<>m__C) };
            base._activity.Call("runOnUiThread", args);
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
        private sealed class <createRole>c__AnonStorey8C
        {
            internal TheBiliAccountDelegate <>f__this;
            internal string uid;

            internal void <>m__E()
            {
                object[] args = new object[] { this.uid };
                this.<>f__this._delegate.Call("createRole", args);
            }
        }

        [CompilerGenerated]
        private sealed class <init>c__AnonStorey88
        {
            internal TheBiliAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal bool debugMode;

            internal void <>m__A()
            {
                object[] args = new object[] { this.<>f__this._activity, this.debugMode, TheBiliAccountDelegate.MERCHANT_ID, TheBiliAccountDelegate.APP_ID, TheBiliAccountDelegate.SERVER_ID, TheBiliAccountDelegate.APP_KEY, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("init", args);
            }
        }

        [CompilerGenerated]
        private sealed class <login>c__AnonStorey89
        {
            internal TheBiliAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;

            internal void <>m__B()
            {
                object[] args = new object[] { this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("login", args);
            }
        }

        [CompilerGenerated]
        private sealed class <pay>c__AnonStorey8B
        {
            internal TheBiliAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;
            internal string notifyUrl;
            internal string productID;
            internal string productName;
            internal float productPrice;
            internal string tradeNo;
            internal string userID;

            internal void <>m__D()
            {
                object[] args = new object[] { this.productID, this.productName, this.productPrice, this.tradeNo, this.userID, this.notifyUrl, this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("pay", args);
            }
        }

        [CompilerGenerated]
        private sealed class <register>c__AnonStorey8A
        {
            internal TheBiliAccountDelegate <>f__this;
            internal string callbackClass;
            internal string callbackMethod;

            internal void <>m__C()
            {
                object[] args = new object[] { this.callbackClass, this.callbackMethod };
                this.<>f__this._delegate.Call("register", args);
            }
        }
    }
}

