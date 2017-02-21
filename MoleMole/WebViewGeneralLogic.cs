namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using UniRx;
    using UnityEngine;

    public static class WebViewGeneralLogic
    {
        private static UniWebView CreateWebview()
        {
            GameObject obj2 = GameObject.Find("WebView");
            if (obj2 == null)
            {
                obj2 = new GameObject("WebView");
            }
            UniWebView view = obj2.AddComponent<UniWebView>();
            view.toolBarShow = false;
            obj2.AddComponent<UniWebviewAndroidReloadHelper>();
            return view;
        }

        private static UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
        {
            if (orientation == UniWebViewOrientation.Portrait)
            {
                return new UniWebViewEdgeInsets(5, 5, 5, 5);
            }
            return new UniWebViewEdgeInsets(5, 5, 5, 5);
        }

        public static void LoadUrl(string url, bool transparent = false)
        {
            UniWebView view = CreateWebview();
            view.url = url;
            view.OnLoadComplete += new UniWebView.LoadCompleteDelegate(WebViewGeneralLogic.OnLoadComplete);
            view.OnReceivedMessage += new UniWebView.ReceivedMessageDelegate(WebViewGeneralLogic.OnReceivedMessage);
            UniWebViewPlugin.TransparentBackground(view.gameObject.name, transparent);
            view.Load();
            view.Show(false, UniWebViewTransitionEdge.None, 0.4f, null);
        }

        private static void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
        {
            if (success)
            {
            }
        }

        private static void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
        {
            if (message.path == "close")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
            }
            else if (message.path == "register")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
                if (message.rawMessage != null)
                {
                    Regex regex = new Regex("username=(.*)&password=(.*)");
                    string str = regex.Match(message.rawMessage).Groups[1].Value;
                    string str2 = regex.Match(message.rawMessage).Groups[2].Value;
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountRegisterSuccess, new Tuple<string, string>(str, str2)));
                }
            }
            else if (message.path == "login")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
                if (message.rawMessage != null)
                {
                    Regex regex2 = new Regex("username=(.*)&password=(.*)");
                    string str3 = regex2.Match(message.rawMessage).Groups[1].Value;
                    string str4 = regex2.Match(message.rawMessage).Groups[2].Value;
                    Singleton<AccountManager>.Instance.manager.LoginUIFinishedCallBack(str3, str4);
                }
            }
            else if (message.path == "bind_email")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
                if (message.rawMessage != null)
                {
                    Regex regex3 = new Regex("email=(.*)");
                    string str5 = regex3.Match(message.rawMessage).Groups[1].Value;
                    Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.email = str5;
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged, null));
                }
            }
            else if (message.path == "bind_mobile")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
                if (message.rawMessage != null)
                {
                    Regex regex4 = new Regex("mobile=(.*)");
                    string str6 = regex4.Match(message.rawMessage).Groups[1].Value;
                    Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.mobile = str6;
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged, null));
                }
            }
            else if (message.path == "bind_identity")
            {
                UnityEngine.Object.Destroy(webView.gameObject);
                if (message.rawMessage != null)
                {
                    Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.isRealNameVerify = true;
                    Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MihoyoAccountInfoChanged, null));
                }
            }
        }
    }
}

