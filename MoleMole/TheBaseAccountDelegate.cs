namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TheBaseAccountDelegate
    {
        protected AndroidJavaObject _activity;
        protected AndroidJavaObject _delegate;
        protected AndroidJavaObject _handler;

        public TheBaseAccountDelegate()
        {
            if (this._activity == null)
            {
                this._activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        public virtual void exit()
        {
        }

        public virtual string getUid()
        {
            return string.Empty;
        }

        public virtual string getUsername()
        {
            return string.Empty;
        }

        public virtual void hideToolBar()
        {
        }

        public virtual void init(bool debugMode, string callbackClass, string callbackMethod, Function callback)
        {
        }

        public virtual IEnumerator login(string callbackClass, string callbackMethod, string arg1, string arg2, Function callback)
        {
            return null;
        }

        public virtual void pay(string productID, string productName, float productPrice, string tradeNo, string userID, string notifyUrl, string callbackClass, string callbackMethod, Function callback)
        {
        }

        public virtual IEnumerator register(string callbackClass, string callbackMethod, string arg1, string arg2, string arg3, string arg4, Function callback)
        {
            return null;
        }

        public virtual void showPausePage()
        {
        }

        public virtual void showToolBar()
        {
        }

        public virtual void showUserCenter()
        {
        }

        public delegate void Function(string param);
    }
}

