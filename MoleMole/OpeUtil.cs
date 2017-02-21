namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class OpeUtil
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map8;
        private const string COMMENT_MAGIC_CODE = "BINLAO";
        private const byte COMMENT_SALT = 0xcc;
        private static string[] RESERVED_WORDS = new string[] { "_version", "_account_uid", "_account_token", "_uid", "_nickname", "_level", "_vip_point", "_hcoin" };
        private const string SIGN_KEY = "1Sdfl0D98jc983BJG8O8fba";

        public static string ConvertEventUrl(string sourceUrl)
        {
            string str;
            Dictionary<string, string> dictionary;
            if (string.IsNullOrEmpty(sourceUrl))
            {
                return sourceUrl;
            }
            ParseUrl(sourceUrl, out str, out dictionary);
            SetupReservedParam(ref dictionary);
            dictionary["_time"] = Miscs.GetTimeStampFromDateTime(TimeUtil.Now).ToString();
            string str4 = SwapStr(SecurityUtil.Base64Encoder(GeneralUrlParamString(dictionary)));
            string str5 = SecurityUtil.SHA256(str4 + "1Sdfl0D98jc983BJG8O8fba");
            Dictionary<string, string> paramDict = new Dictionary<string, string>();
            paramDict.Add("auth_key", str4);
            paramDict.Add("sign", str5);
            return GeneralUrl(str, paramDict);
        }

        public static string GeneralUrl(string baseUrl, Dictionary<string, string> paramDict)
        {
            return (baseUrl + "?" + GeneralUrlParamString(paramDict));
        }

        private static string GeneralUrlParamString(Dictionary<string, string> paramDict)
        {
            if ((paramDict == null) || (paramDict.Count <= 0))
            {
                return string.Empty;
            }
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> pair in paramDict)
            {
                list.Add(pair.Key + "=" + WWW.EscapeURL(pair.Value));
            }
            return string.Join("&", list.ToArray());
        }

        public static ApkCommentInfo GetApkComment()
        {
            ApkCommentInfo info = new ApkCommentInfo();
            string str = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<string>("getPackageResourcePath", new object[0]);
            object[] args = new object[] { str, "BINLAO" };
            AndroidJavaObject obj3 = new AndroidJavaObject("com.miHoYo.ApkCommentReader", args);
            if (obj3 != null)
            {
                byte[] bytes = obj3.Call<byte[]>("getApkComment", new object[0]);
                info.isMihoyoComment = obj3.Call<bool>("isMihoyoComment", new object[0]);
                if ((bytes == null) || (bytes.Length <= 0))
                {
                    return info;
                }
                if (info.isMihoyoComment)
                {
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = (byte) (0xcc ^ bytes[i]);
                    }
                    info.cps = Encoding.Default.GetString(bytes);
                    return info;
                }
                info.checksum = SecurityUtil.Md5(bytes);
            }
            return info;
        }

        public static void ParseUrl(string url, out string baseUrl, out Dictionary<string, string> paramDict)
        {
            baseUrl = string.Empty;
            paramDict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(url))
            {
                int index = url.IndexOf('?');
                if (index == -1)
                {
                    baseUrl = url;
                }
                else
                {
                    baseUrl = url.Substring(0, index);
                    if (index != (url.Length - 1))
                    {
                        string input = url.Substring(index + 1);
                        Regex regex = new Regex(@"(^|&)?([\w]+)=([^&]+)(&|$)?");
                        IEnumerator enumerator = regex.Matches(input).GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                Match current = (Match) enumerator.Current;
                                paramDict[current.Result("$2")] = current.Result("$3");
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                    }
                }
            }
        }

        private static void SetupReservedParam(ref Dictionary<string, string> paramDict)
        {
            if ((paramDict != null) && (paramDict.Count > 0))
            {
                int index = 0;
                int length = RESERVED_WORDS.Length;
                while (index < length)
                {
                    if (paramDict.ContainsKey(RESERVED_WORDS[index]))
                    {
                        paramDict[RESERVED_WORDS[index]] = string.Empty;
                        string key = RESERVED_WORDS[index];
                        if (key != null)
                        {
                            int num3;
                            if (<>f__switch$map8 == null)
                            {
                                Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
                                dictionary.Add("_version", 0);
                                dictionary.Add("_account_uid", 1);
                                dictionary.Add("_account_token", 2);
                                dictionary.Add("_uid", 3);
                                dictionary.Add("_nickname", 4);
                                dictionary.Add("_level", 5);
                                dictionary.Add("_vip_point", 6);
                                dictionary.Add("_hcoin", 7);
                                <>f__switch$map8 = dictionary;
                            }
                            if (<>f__switch$map8.TryGetValue(key, out num3))
                            {
                                switch (num3)
                                {
                                    case 0:
                                        if (Singleton<NetworkManager>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<NetworkManager>.Instance.GetGameVersion();
                                        }
                                        break;

                                    case 1:
                                        if ((Singleton<AccountManager>.Instance != null) && (Singleton<MiHoYoGameData>.Instance != null))
                                        {
                                            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
                                            paramDict[RESERVED_WORDS[index]] = string.IsNullOrEmpty(accountUid) ? Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.uid : accountUid;
                                        }
                                        break;

                                    case 2:
                                        if ((Singleton<AccountManager>.Instance != null) && (Singleton<MiHoYoGameData>.Instance != null))
                                        {
                                            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
                                            paramDict[RESERVED_WORDS[index]] = string.IsNullOrEmpty(accountToken) ? Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account.token : accountToken;
                                        }
                                        break;

                                    case 3:
                                        if (Singleton<PlayerModule>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
                                        }
                                        break;

                                    case 4:
                                        if (Singleton<PlayerModule>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<PlayerModule>.Instance.playerData.NickNameText;
                                        }
                                        break;

                                    case 5:
                                        if (Singleton<PlayerModule>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<PlayerModule>.Instance.playerData.teamLevel.ToString();
                                        }
                                        break;

                                    case 6:
                                        if (Singleton<ShopWelfareModule>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<ShopWelfareModule>.Instance.totalPayHCoin.ToString();
                                        }
                                        break;

                                    case 7:
                                        if (Singleton<PlayerModule>.Instance != null)
                                        {
                                            paramDict[RESERVED_WORDS[index]] = Singleton<PlayerModule>.Instance.playerData.hcoin.ToString();
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    index++;
                }
            }
        }

        private static string SwapStr(string sourceStr)
        {
            if (string.IsNullOrEmpty(sourceStr))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(sourceStr);
            int length = sourceStr.Length;
            for (int i = 0; i < (length - 1); i += 2)
            {
                builder[i] = sourceStr[i + 1];
                builder[i + 1] = sourceStr[i];
            }
            return builder.ToString();
        }

        public class ApkCommentInfo
        {
            public string checksum;
            public string cps;
            public bool isMihoyoComment;
        }
    }
}

