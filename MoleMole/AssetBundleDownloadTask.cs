namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AssetBundleDownloadTask
    {
        public AssetBundleDownloadTask(MoleMole.AssetBundleInfo assetBundleInfo, Action<long, long, long, float> onProgress, Action<AssetBundleDownloadTask> onFinished)
        {
            this.AssetBundleInfo = assetBundleInfo;
            this.OnProgress = onProgress;
            this.OnFinished = onFinished;
            this.DownloadedBytes = null;
            this.CurrentStatus = DownloadStatus.WAITING;
        }

        [DebuggerHidden]
        public IEnumerator StartDownload()
        {
            return new <StartDownload>c__Iterator2 { <>f__this = this };
        }

        public MoleMole.AssetBundleInfo AssetBundleInfo { get; private set; }

        public DownloadStatus CurrentStatus { get; private set; }

        public byte[] DownloadedBytes { get; private set; }

        public bool IsFailed
        {
            get
            {
                return (this.CurrentStatus == DownloadStatus.FAIL_DOWNLOADED);
            }
        }

        public bool IsSuccess
        {
            get
            {
                return (this.CurrentStatus == DownloadStatus.SUCCESS_DOWNLOADED);
            }
        }

        public Action<AssetBundleDownloadTask> OnFinished { get; private set; }

        public Action<long, long, long, float> OnProgress { get; private set; }

        [CompilerGenerated]
        private sealed class <StartDownload>c__Iterator2 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AssetBundleDownloadTask <>f__this;
            internal long <currentBytesDownloaded>__7;
            internal long <deltaBytesDonloaded>__8;
            internal float <deltaTime>__6;
            internal float <downloadSpeed>__4;
            internal long <previousBytesDownloaded>__1;
            internal float <previousTime>__3;
            internal float <startTime>__2;
            internal string <url>__0;
            internal WWW <www>__5;

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
                        this.<url>__0 = this.<>f__this.AssetBundleInfo.RemoteFilePath;
                        this.<url>__0 = this.<url>__0 + (!this.<url>__0.Contains("?") ? "?" : "&") + "t=" + TimeUtil.Now.ToString("yyyyMMddHHmmss");
                        this.<>f__this.CurrentStatus = DownloadStatus.DOWNLOADING;
                        this.<previousBytesDownloaded>__1 = 0L;
                        this.<startTime>__2 = Time.realtimeSinceStartup;
                        this.<previousTime>__3 = this.<startTime>__2;
                        this.<downloadSpeed>__4 = 0f;
                        if (this.<>f__this.OnProgress != null)
                        {
                            this.<>f__this.OnProgress(0L, this.<>f__this.AssetBundleInfo.FileCompressedSize, 0L, this.<downloadSpeed>__4);
                        }
                        this.<www>__5 = new WWW(this.<url>__0);
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0323;
                }
                while (!this.<www>__5.isDone)
                {
                    this.<deltaTime>__6 = Time.realtimeSinceStartup - this.<previousTime>__3;
                    if (this.<deltaTime>__6 >= 0.5f)
                    {
                        this.<currentBytesDownloaded>__7 = (long) (this.<>f__this.AssetBundleInfo.FileCompressedSize * this.<www>__5.progress);
                        this.<deltaBytesDonloaded>__8 = this.<currentBytesDownloaded>__7 - this.<previousBytesDownloaded>__1;
                        this.<downloadSpeed>__4 = ((float) this.<deltaBytesDonloaded>__8) / this.<deltaTime>__6;
                        this.<previousTime>__3 = Time.realtimeSinceStartup;
                        this.<previousBytesDownloaded>__1 = this.<currentBytesDownloaded>__7;
                        if (this.<>f__this.OnProgress != null)
                        {
                            this.<>f__this.OnProgress(this.<currentBytesDownloaded>__7, this.<>f__this.AssetBundleInfo.FileCompressedSize, this.<deltaBytesDonloaded>__8, this.<downloadSpeed>__4);
                        }
                    }
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<www>__5.error == null)
                {
                    this.<>f__this.CurrentStatus = DownloadStatus.SUCCESS_DOWNLOADED;
                    if (this.<>f__this.OnProgress != null)
                    {
                        this.<>f__this.OnProgress(this.<>f__this.AssetBundleInfo.FileCompressedSize, this.<>f__this.AssetBundleInfo.FileCompressedSize, this.<>f__this.AssetBundleInfo.FileCompressedSize - this.<previousBytesDownloaded>__1, 0f);
                    }
                    this.<>f__this.DownloadedBytes = this.<www>__5.bytes;
                }
                else
                {
                    this.<>f__this.CurrentStatus = DownloadStatus.FAIL_DOWNLOADED;
                    this.<>f__this.DownloadedBytes = null;
                }
                if (this.<www>__5 != null)
                {
                    if (this.<www>__5.assetBundle != null)
                    {
                        this.<www>__5.assetBundle.Unload(true);
                    }
                    this.<www>__5.Dispose();
                    this.<www>__5 = null;
                }
                if (this.<>f__this.OnFinished != null)
                {
                    this.<>f__this.OnFinished(this.<>f__this);
                }
                Singleton<AssetBundleManager>.Instance.Loader.RemoveDownloadTask(this.<>f__this);
                this.$PC = -1;
            Label_0323:
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
    }
}

