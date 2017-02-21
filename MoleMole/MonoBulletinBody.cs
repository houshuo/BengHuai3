namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoBulletinBody : MonoBehaviour
    {
        public Transform linkPrefab;
        public Transform paragraphPrefab;
        private const string PATTERN = "<type=\"(.+?)\" (.+?)/>";
        public Transform textPrefab;

        private void AddLink(string str, bool isWebview)
        {
            <AddLink>c__AnonStorey10A storeya = new <AddLink>c__AnonStorey10A {
                isWebview = isWebview,
                <>f__this = this
            };
            Regex regex = new Regex("text=\"(.*?)\"");
            string str2 = regex.Match(str).Groups[1].Value;
            Regex regex2 = new Regex("href=\"(.*?)\"");
            string sourceUrl = regex2.Match(str).Groups[1].Value;
            storeya.opeUrl = OpeUtil.ConvertEventUrl(sourceUrl);
            Transform transform = base.transform.AddChildFromPrefab(this.linkPrefab, "Link");
            transform.GetComponent<Text>().text = str2;
            transform.GetComponent<Button>().onClick.AddListener(new UnityAction(storeya.<>m__1BE));
        }

        private void AddParagraph(string str)
        {
            Regex regex = new Regex("text=\"(.*?)\"");
            string str2 = regex.Match(str).Groups[1].Value;
            base.transform.AddChildFromPrefab(this.paragraphPrefab, "Paragraph").GetComponent<Text>().text = str2;
        }

        private void AddText(string text)
        {
            base.transform.AddChildFromPrefab(this.textPrefab, "Text").GetComponent<Text>().text = text;
        }

        private void Clear()
        {
            base.transform.DestroyChildren();
        }

        private void OpenUrl(string url, bool isWebview)
        {
            string str = url;
            if (isWebview)
            {
                WebViewGeneralLogic.LoadUrl(str, false);
            }
            else
            {
                Application.OpenURL(str);
            }
        }

        public void SetupView(string input)
        {
            this.Clear();
            int startIndex = 0;
            IEnumerator enumerator = Regex.Matches(input, "<type=\"(.+?)\" (.+?)/>").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    if (startIndex < current.Index)
                    {
                        string text = input.Substring(startIndex, current.Index - startIndex);
                        this.AddText(text);
                    }
                    startIndex = current.Index + current.Value.Length;
                    string str2 = current.Groups[1].Value;
                    if (str2 == "p")
                    {
                        this.AddParagraph(current.Groups[2].Value);
                    }
                    else
                    {
                        if (str2 == "webview")
                        {
                            this.AddLink(current.Groups[2].Value, true);
                            continue;
                        }
                        if (str2 == "browser")
                        {
                            this.AddLink(current.Groups[2].Value, false);
                        }
                    }
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
            if (startIndex < input.Length)
            {
                string str3 = input.Substring(startIndex, input.Length - startIndex);
                this.AddText(str3);
            }
        }

        [CompilerGenerated]
        private sealed class <AddLink>c__AnonStorey10A
        {
            internal MonoBulletinBody <>f__this;
            internal bool isWebview;
            internal string opeUrl;

            internal void <>m__1BE()
            {
                this.<>f__this.OpenUrl(this.opeUrl, this.isWebview);
            }
        }
    }
}

