namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class LocalizationGeneralLogic
    {
        private static Dictionary<string, string> _textMap;

        private static string CompileMyDefinedPattern<T>(string text, T[] replaceParams)
        {
            string input = text;
            input = input.Replace(@"\n", Environment.NewLine);
            for (int i = 0; i < replaceParams.Length; i++)
            {
                string str2 = "#" + (i + 1);
                if (input.Contains(str2))
                {
                    Regex regex = new Regex(str2 + @"\[f(\d+)\]");
                    Regex regex2 = new Regex(str2 + @"\[f(\d+)\]%");
                    if (input.Contains(str2 + "[i]%"))
                    {
                        string newValue = string.Format("{0:P0}", float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
                        input = input.Replace(str2 + "[i]%", newValue);
                    }
                    else if (regex2.IsMatch(input))
                    {
                        string replacement = string.Format("{0:P" + regex2.Match(input).Groups[1].Value + "}", float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
                        input = regex2.Replace(input, replacement);
                    }
                    else if (input.Contains(str2 + "%"))
                    {
                        string str6 = string.Format("{0:P0}", float.Parse(replaceParams[i].ToString())).Replace(" ", string.Empty);
                        input = input.Replace(str2 + "%", str6);
                    }
                    else if (regex.IsMatch(input))
                    {
                        string format = "{0:N" + regex.Match(input).Groups[1].Value + "}";
                        input = regex.Replace(input, string.Format(format, replaceParams[i]));
                    }
                    else if (input.Contains(str2 + "[i]"))
                    {
                        input = input.Replace(str2 + "[i]", string.Format("{0:N0}", Mathf.Floor(float.Parse(replaceParams[i].ToString()))));
                    }
                    else if (input.Contains("<color=" + str2))
                    {
                        input = input.Replace("<color=#", "<color=").Replace(str2, replaceParams[i].ToString()).Replace("<color=", "<color=#");
                    }
                    else
                    {
                        input = input.Replace(str2, replaceParams[i].ToString());
                    }
                }
            }
            return input;
        }

        public static string GetNetworkErrCodeOutput(object retcode, params object[] replaceParams)
        {
            string input = retcode.GetType().ToString();
            Regex regex = new Regex(@"proto\.(.*)\+Retcode");
            if (!regex.IsMatch(input))
            {
                return string.Empty;
            }
            NetworkErrCodeMetaData networkErrCodeMetaDataByKey = NetworkErrCodeMetaDataReader.GetNetworkErrCodeMetaDataByKey(regex.Match(input).Groups[1].Value.Trim(), retcode.ToString());
            if (networkErrCodeMetaDataByKey != null)
            {
                return GetText(networkErrCodeMetaDataByKey.textMapID, replaceParams);
            }
            return retcode.ToString();
        }

        public static string GetText(string textID, params object[] replaceParams)
        {
            return CompileMyDefinedPattern<object>(GetTextFromTextMap(textID), replaceParams);
        }

        public static string GetText(string textID, Color color, params object[] replaceParams)
        {
            return CompileMyDefinedPattern<object>(PreInsertRichTextCode(GetTextFromTextMap(textID), color), replaceParams);
        }

        private static string GetTextFromTextMap(string textID)
        {
            string key = textID.Trim();
            if (!_textMap.ContainsKey(key))
            {
                return string.Empty;
            }
            return _textMap[key];
        }

        public static string GetTextWithParamArray<T>(string textID, T[] replaceParams)
        {
            return CompileMyDefinedPattern<T>(GetTextFromTextMap(textID), replaceParams);
        }

        public static string GetTextWithParamArray<T>(string textID, Color color, T[] replaceParams)
        {
            return CompileMyDefinedPattern<T>(PreInsertRichTextCode(GetTextFromTextMap(textID), color), replaceParams);
        }

        public static void InitOnDataAssetReady()
        {
            foreach (TextMapMetaData data in TextMapMetaDataReader.GetItemList())
            {
                _textMap[data.ID] = data.Text;
            }
        }

        public static void InitOnGameStart()
        {
            _textMap = new Dictionary<string, string>();
            TextMapGameStartMetaDataReader.LoadFromFile();
            foreach (TextMapMetaData data in TextMapGameStartMetaDataReader.GetItemList())
            {
                _textMap.Add(data.ID, data.Text);
            }
        }

        private static string PreInsertRichTextCode(string text, Color color)
        {
            string str = text;
            string pattern = @"(#\d\[.+?\]%?|#\d%?)";
            IEnumerator enumerator = Regex.Matches(text, pattern).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    string oldValue = current.Groups[1].Value;
                    string str4 = ColorUtility.ToHtmlStringRGBA(color);
                    str = str.Replace(oldValue, string.Format("<color=#{0}>{1}</color>", str4, oldValue));
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
            return str;
        }
    }
}

