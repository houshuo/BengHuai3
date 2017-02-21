namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using UnityEngine;

    public class QAManager
    {
        private string _channelName = string.Empty;
        private int _mainThreadID = Thread.CurrentThread.ManagedThreadId;
        private List<MessageEntry> _sendToServerMessages = new List<MessageEntry>();
        private HashSet<int> _sentMessageHashes = new HashSet<int>();

        public QAManager()
        {
            ConfigChannel channel = ConfigUtil.LoadJSONConfig<ConfigChannel>("DataPersistent/BuildChannel/ChannelConfig");
            if (channel != null)
            {
                this._channelName = channel.ChannelName;
            }
            SuperDebug.sendToServerAction = new Action<string, string>(this.SendMessageToSever);
        }

        public void Destroy()
        {
        }

        public bool IsSentMessage(string msg, string stackTrace)
        {
            MessageEntry entry = new MessageEntry(msg, stackTrace);
            return this._sentMessageHashes.Contains(entry.GetHashCode());
        }

        [Conditional("NG_HSOD_DEBUG")]
        public void LogFPS(string context, float fpsAvg, float fpsMin, float fpsMax)
        {
        }

        public void SendFileToServer(string url, string fileType, byte[] buf, Action<string> successCallback, Action failCallback, float timeoutSecond = 5f, string prefix = null, bool needDispose = true)
        {
            string str = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
            string gameVersion = Singleton<NetworkManager>.Instance.GetGameVersion();
            string str3 = TimeUtil.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string str4 = string.Format("{0}-{1}-{2}.txt", str, gameVersion, str3);
            if (!string.IsNullOrEmpty(prefix))
            {
                str4 = string.Format("{0}-{1}", prefix, str4);
            }
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            dictionary2.Add("File-Type", fileType);
            dictionary2.Add("File-Name", str4);
            Dictionary<string, string> headers = dictionary2;
            Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(Miscs.WWWRequestWithTimeOut(url, successCallback, failCallback, timeoutSecond, buf, headers, needDispose));
        }

        public void SendMessageToSever(string msg, string stackTrace)
        {
            MessageEntry entry = new MessageEntry(msg, stackTrace);
            int hashCode = entry.GetHashCode();
            if (!this._sentMessageHashes.Contains(hashCode))
            {
                this._sentMessageHashes.Add(hashCode);
                if ((this._mainThreadID == Thread.CurrentThread.ManagedThreadId) && (Singleton<ApplicationManager>.Instance.applicationBehaviour != null))
                {
                    Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(this.SendMessageToSeverCoroutine(entry));
                }
                else
                {
                    this._sendToServerMessages.Add(entry);
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator SendMessageToSeverCoroutine(MessageEntry msg)
        {
            return new <SendMessageToSeverCoroutine>c__Iterator3E { msg = msg, <$>msg = msg, <>f__this = this };
        }

        [DebuggerHidden]
        public IEnumerator SendMessageToSeverSync(string msg, string stackTrace)
        {
            return new <SendMessageToSeverSync>c__Iterator3D { msg = msg, stackTrace = stackTrace, <$>msg = msg, <$>stackTrace = stackTrace, <>f__this = this };
        }

        [Conditional("NG_HSOD_DEBUG")]
        public void SetFPSContext(string context)
        {
            MonoFPSIndicator indicator = UnityEngine.Object.FindObjectOfType<MonoFPSIndicator>();
            if (indicator != null)
            {
                indicator.GetComponent<MonoFPSIndicator>().logContext = context;
            }
        }

        public void UpdateSendMessageToSever()
        {
            if ((this._sendToServerMessages.Count > 0) && (Singleton<ApplicationManager>.Instance.applicationBehaviour != null))
            {
                List<MessageEntry> list = new List<MessageEntry>(this._sendToServerMessages);
                this._sendToServerMessages.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    Singleton<ApplicationManager>.Instance.applicationBehaviour.StartCoroutine(this.SendMessageToSeverCoroutine(list[i]));
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SendMessageToSeverCoroutine>c__Iterator3E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal QAManager.MessageEntry <$>msg;
            internal QAManager <>f__this;
            internal string <asb>__2;
            internal int <uid>__0;
            internal StringBuilder <url>__3;
            internal string <vid>__1;
            internal QAManager.MessageEntry msg;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<uid>__0 = -1;
                        if ((Singleton<PlayerModule>.Instance != null) && (Singleton<PlayerModule>.Instance.playerData != null))
                        {
                            this.<uid>__0 = Singleton<PlayerModule>.Instance.playerData.userId;
                        }
                        this.<vid>__1 = string.Empty;
                        if (Singleton<NetworkManager>.Instance != null)
                        {
                            this.<vid>__1 = Singleton<NetworkManager>.Instance.GetGameVersion();
                        }
                        this.<asb>__2 = string.Empty;
                        if ((Singleton<AssetBundleManager>.Instance != null) && (Singleton<AssetBundleManager>.Instance.Loader != null))
                        {
                            this.<asb>__2 = Singleton<AssetBundleManager>.Instance.Loader.assetBoundleSVNVersion;
                        }
                        this.<url>__3 = new StringBuilder();
                        this.<url>__3.Append("http://139.196.152.26:10080/recorder?");
                        this.<url>__3.Append("&channelName=" + this.<>f__this._channelName);
                        this.<url>__3.Append("&uid=" + this.<uid>__0);
                        this.<url>__3.Append("&vid=" + this.<vid>__1);
                        this.<url>__3.Append("&asb=" + this.<asb>__2);
                        this.<url>__3.Append("&time=" + WWW.EscapeURL(TimeUtil.Now.ToString()));
                        this.<url>__3.Append("&operatingSystem=" + WWW.EscapeURL(SystemInfo.operatingSystem));
                        this.<url>__3.Append("&deviceModel=" + WWW.EscapeURL(SystemInfo.deviceModel));
                        this.<url>__3.Append("&graphicsDeviceName=" + WWW.EscapeURL(SystemInfo.graphicsDeviceName));
                        this.<url>__3.Append("&graphicsDeviceType=" + SystemInfo.graphicsDeviceType);
                        this.<url>__3.Append("&graphicsDeviceVendor=" + WWW.EscapeURL(SystemInfo.graphicsDeviceVendor));
                        this.<url>__3.Append("&graphicsDeviceVersion=" + WWW.EscapeURL(SystemInfo.graphicsDeviceVersion));
                        this.<url>__3.Append("&graphicsMemorySize=" + SystemInfo.graphicsMemorySize);
                        this.<url>__3.Append("&systemMemorySize=" + SystemInfo.systemMemorySize);
                        this.<url>__3.Append("&processorCount=" + SystemInfo.processorCount);
                        this.<url>__3.Append("&processorFrequency=" + SystemInfo.processorFrequency);
                        this.<url>__3.Append("&processorType=" + WWW.EscapeURL(SystemInfo.processorType));
                        this.<url>__3.Append("&msgId=" + this.msg.id);
                        this.<url>__3.Append("&msg=" + WWW.EscapeURL(this.msg.message));
                        this.<url>__3.Append("&st=" + WWW.EscapeURL(this.msg.stackTrace));
                        this.$current = new WWW(this.<url>__3.ToString());
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <SendMessageToSeverSync>c__Iterator3D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>msg;
            internal string <$>stackTrace;
            internal QAManager <>f__this;
            internal QAManager.MessageEntry <entry>__0;
            internal int <hash>__1;
            internal string msg;
            internal string stackTrace;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<entry>__0 = new QAManager.MessageEntry(this.msg, this.stackTrace);
                        this.<hash>__1 = this.<entry>__0.GetHashCode();
                        if (this.<>f__this._sentMessageHashes.Contains(this.<hash>__1))
                        {
                            break;
                        }
                        this.<>f__this._sentMessageHashes.Add(this.<hash>__1);
                        this.$current = this.<>f__this.SendMessageToSeverCoroutine(this.<entry>__0);
                        this.$PC = 1;
                        return true;

                    case 1:
                        break;

                    default:
                        goto Label_00A5;
                }
                this.$PC = -1;
            Label_00A5:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private class MessageEntry
        {
            private static int _lastId;
            public int id = ++_lastId;
            public string message;
            public string stackTrace;

            public MessageEntry(string msg, string st)
            {
                this.message = msg;
                this.stackTrace = st;
            }

            public override int GetHashCode()
            {
                return (this.message.GetHashCode() ^ this.stackTrace.GetHashCode());
            }
        }
    }
}

