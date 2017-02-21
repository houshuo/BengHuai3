namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoAssetBundleLoader : MonoBehaviour
    {
        private List<AssetBundleInfo> _downloadAssetBundleList;
        private Dictionary<string, AssetBundleDownloadTask> _downloadTaskDict;
        private List<AssetBundleDownloadTask> _downloadTaskList;
        private BackGroundWorker _zipBackGroundWorker;
        public string assetBoundleSVNVersion;
        public DateTime checkSVNVersionDate;
        public string eventAssetBoundleSVNVersion;

        public bool AddDownloadTask(AssetBundleDownloadTask downloadTask)
        {
            if (downloadTask == null)
            {
                return false;
            }
            string remoteFilePath = downloadTask.AssetBundleInfo.RemoteFilePath;
            if (this._downloadTaskDict.ContainsKey(remoteFilePath))
            {
                return false;
            }
            this._downloadTaskDict.Add(remoteFilePath, downloadTask);
            this._downloadTaskList.Add(downloadTask);
            base.StartCoroutine(downloadTask.StartDownload());
            return true;
        }

        public void Awake()
        {
            this._downloadTaskList = new List<AssetBundleDownloadTask>();
            this._downloadTaskDict = new Dictionary<string, AssetBundleDownloadTask>();
            this._zipBackGroundWorker = new BackGroundWorker();
        }

        private void CheckEventVersionAndDownloadOneAssetBundle(string resPath, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
        {
            <CheckEventVersionAndDownloadOneAssetBundle>c__AnonStoreyB6 yb = new <CheckEventVersionAndDownloadOneAssetBundle>c__AnonStoreyB6 {
                resPath = resPath,
                onDownloadProgress = onDownloadProgress,
                onFinished = onFinished,
                <>f__this = this
            };
            AssetBundleInfo assetBundleInfo = new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, BundleType.RESOURCE_FILE, false, "event");
            this.AddDownloadTask(new AssetBundleDownloadTask(assetBundleInfo, null, new Action<AssetBundleDownloadTask>(yb.<>m__6F)));
        }

        private void DoStartDownloadOneAssetBundle(AssetBundleInfo asbInfo, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
        {
            <DoStartDownloadOneAssetBundle>c__AnonStoreyB7 yb = new <DoStartDownloadOneAssetBundle>c__AnonStoreyB7 {
                onDownloadProgress = onDownloadProgress,
                asbInfo = asbInfo,
                onFinished = onFinished
            };
            AssetBundleDownloadTask downloadTask = new AssetBundleDownloadTask(yb.asbInfo, new Action<long, long, long, float>(yb.<>m__70), new Action<AssetBundleDownloadTask>(yb.<>m__71));
            this.AddDownloadTask(downloadTask);
        }

        public long GetDownloadAssetBundleTotalSize()
        {
            long num = 0L;
            if (this._downloadAssetBundleList != null)
            {
                for (int i = 0; i < this._downloadAssetBundleList.Count; i++)
                {
                    num += this._downloadAssetBundleList[i].FileCompressedSize;
                }
            }
            return num;
        }

        private byte[] GetEncryptedKeyBytes(string keys)
        {
            byte[] buffer = new byte[keys.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = byte.Parse(keys.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return buffer;
        }

        public bool IsDownloadCompleted()
        {
            return ((this._downloadAssetBundleList == null) || (this._downloadAssetBundleList.Count == 0));
        }

        public void LoadVersionFile(BundleType bundleType, Action<long, long, long, float> onProgress, Action<bool> onFinished)
        {
            <LoadVersionFile>c__AnonStoreyB2 yb = new <LoadVersionFile>c__AnonStoreyB2 {
                bundleType = bundleType,
                onFinished = onFinished,
                <>f__this = this
            };
            AssetBundleInfo[] versionAssetBundleInfo = AssetBundleUtility.GetVersionAssetBundleInfo(yb.bundleType);
            if ((versionAssetBundleInfo != null) && (versionAssetBundleInfo.Length > 0))
            {
                yb.fileName2AssetBundleDict = new Dictionary<string, AssetBundleInfo>();
                AssetBundleDownloadTask[] taskArray = new AssetBundleDownloadTask[versionAssetBundleInfo.Length];
                for (int i = versionAssetBundleInfo.Length - 1; i >= 0; i--)
                {
                    if (i == (versionAssetBundleInfo.Length - 1))
                    {
                        taskArray[i] = new AssetBundleDownloadTask(versionAssetBundleInfo[i], onProgress, new Action<AssetBundleDownloadTask>(yb.<>m__6C));
                    }
                    else
                    {
                        <LoadVersionFile>c__AnonStoreyB3 yb2 = new <LoadVersionFile>c__AnonStoreyB3 {
                            <>f__ref$178 = yb,
                            <>f__this = this,
                            nextTask = taskArray[i + 1]
                        };
                        taskArray[i] = new AssetBundleDownloadTask(versionAssetBundleInfo[i], onProgress, new Action<AssetBundleDownloadTask>(yb2.<>m__6D));
                    }
                }
                this.AddDownloadTask(taskArray[0]);
            }
        }

        public void MakeDownloadList(BundleType bundleType)
        {
            string path = AssetBundleUtility.LocalAssetBundleDirectory(bundleType);
            AssetBundleUtility.CreateParentDirectory(path);
            string[] strArray = Directory.GetFiles(path, "*.unity3d", SearchOption.AllDirectories);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < strArray.Length; i++)
            {
                string key = strArray[i].Substring(path.Length, strArray[i].LastIndexOf("_") - path.Length).Replace('\\', '/');
                string str3 = strArray[i].Substring(strArray[i].LastIndexOf("_") + 1, (strArray[i].LastIndexOf(".") - strArray[i].LastIndexOf("_")) - 1);
                if (dictionary.ContainsKey(key))
                {
                    File.Delete(strArray[i].Replace('\\', '/'));
                }
                else
                {
                    dictionary.Add(key, str3);
                }
            }
            Dictionary<string, AssetBundleInfo> assetBundleInfoDict = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoDict(bundleType);
            HashSet<string> set = new HashSet<string>();
            set.UnionWith(dictionary.Keys);
            set.UnionWith(assetBundleInfoDict.Keys);
            List<string> list = new List<string>();
            List<AssetBundleInfo> list2 = new List<AssetBundleInfo>();
            foreach (string str4 in set)
            {
                bool flag = dictionary.ContainsKey(str4);
                if (!assetBundleInfoDict.ContainsKey(str4))
                {
                    list.Add(path + str4 + "_" + dictionary[str4] + ".unity3d");
                }
                else if (!flag)
                {
                    switch ((((!GlobalVars.UseSpliteResources ? 0 : 1) * 3) + assetBundleInfoDict[str4].FileDownloadMode))
                    {
                        case 1:
                        case 3:
                        case 4:
                            list2.Add(assetBundleInfoDict[str4]);
                            break;
                    }
                }
                else
                {
                    switch ((((!GlobalVars.UseSpliteResources ? 0 : 1) * 3) + assetBundleInfoDict[str4].FileDownloadMode))
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            if (dictionary[str4] != assetBundleInfoDict[str4].FileCrc)
                            {
                                list.Add(path + str4 + "_" + dictionary[str4] + ".unity3d");
                                list2.Add(assetBundleInfoDict[str4]);
                            }
                            break;
                    }
                }
            }
            foreach (string str5 in list)
            {
                if (File.Exists(str5))
                {
                    File.Delete(str5);
                }
            }
            this._downloadAssetBundleList = new List<AssetBundleInfo>();
            List<AssetBundleInfo> list3 = new List<AssetBundleInfo>();
            for (int j = 0; j < list2.Count; j++)
            {
                AssetBundleInfo item = list2[j];
                if (item.ParentFileNameSet != null)
                {
                    foreach (string str6 in item.ParentFileNameSet)
                    {
                        AssetBundleInfo assetBundleInfoByFileName = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByFileName(bundleType, str6);
                        if ((assetBundleInfoByFileName != null) && !list3.Contains(assetBundleInfoByFileName))
                        {
                            list3.Add(assetBundleInfoByFileName);
                        }
                    }
                }
                this._downloadAssetBundleList.Add(item);
            }
            for (int k = 0; k < list3.Count; k++)
            {
                string fileName = list3[k].FileName;
                bool flag3 = dictionary.ContainsKey(fileName);
                bool flag4 = assetBundleInfoDict.ContainsKey(fileName);
                if ((!flag3 || (flag4 && (dictionary[fileName] != assetBundleInfoDict[fileName].FileCrc))) && !this._downloadAssetBundleList.Contains(list3[k]))
                {
                    this._downloadAssetBundleList.Add(list3[k]);
                }
            }
        }

        private void ParseVersionFile(BundleType bundleType, AssetBundleDownloadTask task, ref Dictionary<string, AssetBundleInfo> fileNames)
        {
            byte[] downloadedBytes = task.DownloadedBytes;
            for (int i = 0; i < downloadedBytes.Length; i++)
            {
                downloadedBytes[i] = (byte) (AssetBundleUtility.BYTE_SALT ^ downloadedBytes[i]);
            }
            AssetBundle bundle = AssetBundle.LoadFromMemory(downloadedBytes);
            TextAsset asset = bundle.LoadAsset("PackageVersion") as TextAsset;
            int num2 = 0;
            char[] separator = new char[] { "\n"[0] };
            string[] strArray = asset.text.Split(separator);
            if ((strArray.Length > 0) && (bundleType == BundleType.DATA_FILE))
            {
                AssetBundleUtility.ENCRYPTED_KEY = this.GetEncryptedKeyBytes(strArray[0]);
                num2 = 1;
            }
            for (int j = num2; j < strArray.Length; j++)
            {
                AssetBundleInfo info = AssetBundleInfo.FromString(strArray[j]);
                if (info != null)
                {
                    fileNames[info.FileName] = info;
                }
            }
            bundle.Unload(true);
        }

        public void RemoveDownloadTask(AssetBundleDownloadTask downloadTask)
        {
            if (downloadTask != null)
            {
                string remoteFilePath = downloadTask.AssetBundleInfo.RemoteFilePath;
                if (this._downloadTaskDict.ContainsKey(remoteFilePath))
                {
                    this._downloadTaskDict.Remove(remoteFilePath);
                }
                for (int i = 0; i < this._downloadTaskList.Count; i++)
                {
                    if (this._downloadTaskList[i] == downloadTask)
                    {
                        this._downloadTaskList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        [DebuggerHidden]
        public IEnumerator StartDownloadAssetBundle(BundleType bundleType, Action<long, long, long, float> onDownloadProgress, Action<int> onZipProgress, Action<bool> onFinished)
        {
            return new <StartDownloadAssetBundle>c__Iterator3 { bundleType = bundleType, onDownloadProgress = onDownloadProgress, onZipProgress = onZipProgress, onFinished = onFinished, <$>bundleType = bundleType, <$>onDownloadProgress = onDownloadProgress, <$>onZipProgress = onZipProgress, <$>onFinished = onFinished, <>f__this = this };
        }

        public void TryStartDownloadOneAssetBundle(string resPath, Action<long, long, long, float> onDownloadProgress, Action<bool> onFinished)
        {
            <TryStartDownloadOneAssetBundle>c__AnonStoreyB5 yb = new <TryStartDownloadOneAssetBundle>c__AnonStoreyB5 {
                resPath = resPath,
                onDownloadProgress = onDownloadProgress,
                onFinished = onFinished,
                <>f__this = this
            };
            AssetBundleInfo assetBundleInfoByResPath = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByResPath(BundleType.RESOURCE_FILE, yb.resPath);
            if (assetBundleInfoByResPath == null)
            {
                Singleton<AssetBundleManager>.Instance.UpdateEventSVNVersion(new Action(yb.<>m__6E));
            }
            else
            {
                this.DoStartDownloadOneAssetBundle(assetBundleInfoByResPath, yb.onDownloadProgress, yb.onFinished);
            }
        }

        [CompilerGenerated]
        private sealed class <CheckEventVersionAndDownloadOneAssetBundle>c__AnonStoreyB6
        {
            internal MonoAssetBundleLoader <>f__this;
            internal Action<long, long, long, float> onDownloadProgress;
            internal Action<bool> onFinished;
            internal string resPath;

            internal void <>m__6F(AssetBundleDownloadTask task)
            {
                try
                {
                    if (task.IsSuccess)
                    {
                        Dictionary<string, AssetBundleInfo> fileNames = new Dictionary<string, AssetBundleInfo>();
                        this.<>f__this.ParseVersionFile(BundleType.RESOURCE_FILE, task, ref fileNames);
                        Singleton<AssetBundleManager>.Instance.MergeAssetBundleDictOnRequire(BundleType.RESOURCE_FILE, fileNames);
                        AssetBundleInfo assetBundleInfoByResPath = Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByResPath(BundleType.RESOURCE_FILE, this.resPath);
                        if (assetBundleInfoByResPath != null)
                        {
                            this.<>f__this.DoStartDownloadOneAssetBundle(assetBundleInfoByResPath, this.onDownloadProgress, this.onFinished);
                        }
                        else
                        {
                            this.onFinished(false);
                        }
                    }
                    else if (task.IsFailed)
                    {
                        this.onFinished(false);
                    }
                }
                catch (Exception)
                {
                    this.onFinished(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DoStartDownloadOneAssetBundle>c__AnonStoreyB7
        {
            internal AssetBundleInfo asbInfo;
            internal Action<long, long, long, float> onDownloadProgress;
            internal Action<bool> onFinished;

            internal void <>m__70(long current, long total, long delta, float speed)
            {
                if (this.onDownloadProgress != null)
                {
                    this.onDownloadProgress(current, total, delta, speed);
                }
            }

            internal void <>m__71(AssetBundleDownloadTask task)
            {
                bool flag = false;
                if ((task.CurrentStatus == DownloadStatus.SUCCESS_DOWNLOADED) && AssetBundleUtility.ValidateAndSaveAssetBundle(BundleType.RESOURCE_FILE, task))
                {
                    flag = true;
                }
                if (this.onFinished != null)
                {
                    this.onFinished(flag);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadVersionFile>c__AnonStoreyB2
        {
            internal MonoAssetBundleLoader <>f__this;
            internal BundleType bundleType;
            internal Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict;
            internal Action<bool> onFinished;

            internal void <>m__6C(AssetBundleDownloadTask task)
            {
                try
                {
                    if (task.IsSuccess)
                    {
                        this.<>f__this.ParseVersionFile(this.bundleType, task, ref this.fileName2AssetBundleDict);
                        Singleton<AssetBundleManager>.Instance.ClearAssetBundle(this.bundleType);
                        Singleton<AssetBundleManager>.Instance.SetAssetBundleDict(this.bundleType, this.fileName2AssetBundleDict);
                        this.<>f__this.MakeDownloadList(this.bundleType);
                        this.onFinished(true);
                    }
                    else if (task.IsFailed)
                    {
                        this.onFinished(false);
                    }
                }
                catch (Exception)
                {
                    this.onFinished(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LoadVersionFile>c__AnonStoreyB3
        {
            internal MonoAssetBundleLoader.<LoadVersionFile>c__AnonStoreyB2 <>f__ref$178;
            internal MonoAssetBundleLoader <>f__this;
            internal AssetBundleDownloadTask nextTask;

            internal void <>m__6D(AssetBundleDownloadTask task)
            {
                try
                {
                    if (task.IsSuccess)
                    {
                        this.<>f__this.ParseVersionFile(this.<>f__ref$178.bundleType, task, ref this.<>f__ref$178.fileName2AssetBundleDict);
                        this.<>f__this.AddDownloadTask(this.nextTask);
                    }
                    else if (task.IsFailed)
                    {
                        this.<>f__ref$178.onFinished(false);
                    }
                }
                catch (Exception)
                {
                    this.<>f__ref$178.onFinished(false);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <StartDownloadAssetBundle>c__Iterator3 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BundleType <$>bundleType;
            internal Action<long, long, long, float> <$>onDownloadProgress;
            internal Action<bool> <$>onFinished;
            internal Action<int> <$>onZipProgress;
            internal MonoAssetBundleLoader <>f__this;
            internal AssetBundleInfo <asbInfo>__4;
            internal int <currentTaskIndex>__3;
            internal long <downloadedAssetBundleBytes>__1;
            internal DownloadStatus <downloadStatus>__5;
            internal long <totalAssetBundleBytes>__0;
            internal ZipStatus <zipStatus>__2;
            internal BundleType bundleType;
            internal Action<long, long, long, float> onDownloadProgress;
            internal Action<bool> onFinished;
            internal Action<int> onZipProgress;

            internal void <>m__72(long current, long total, long delta, float speed)
            {
                this.onDownloadProgress(this.<downloadedAssetBundleBytes>__1 + current, this.<totalAssetBundleBytes>__0, delta, speed);
            }

            internal void <>m__73(AssetBundleDownloadTask downloadTask)
            {
                <StartDownloadAssetBundle>c__AnonStoreyB4 yb = new <StartDownloadAssetBundle>c__AnonStoreyB4 {
                    <>f__ref$3 = this,
                    downloadTask = downloadTask
                };
                this.<downloadStatus>__5 = yb.downloadTask.CurrentStatus;
                if (this.<downloadStatus>__5 == DownloadStatus.SUCCESS_DOWNLOADED)
                {
                    this.<>f__this._zipBackGroundWorker.AddBackGroundWork(new Action(yb.<>m__74));
                    this.<downloadedAssetBundleBytes>__1 += yb.downloadTask.AssetBundleInfo.FileCompressedSize;
                }
            }

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
                        Screen.sleepTimeout = -1;
                        if (this.<>f__this._downloadAssetBundleList == null)
                        {
                            goto Label_0233;
                        }
                        this.<totalAssetBundleBytes>__0 = this.<>f__this.GetDownloadAssetBundleTotalSize();
                        this.<downloadedAssetBundleBytes>__1 = 0L;
                        this.<zipStatus>__2 = ZipStatus.ZIPPING;
                        this.<>f__this._zipBackGroundWorker.StartBackGroundWork(string.Empty);
                        this.<currentTaskIndex>__3 = 0;
                        goto Label_012C;

                    case 1:
                        break;

                    case 2:
                        goto Label_0185;

                    default:
                        goto Label_0241;
                }
            Label_00E6:
                while ((this.<downloadStatus>__5 == DownloadStatus.DOWNLOADING) && (this.<zipStatus>__2 == ZipStatus.ZIPPING))
                {
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_0243;
                }
                if ((this.<downloadStatus>__5 != DownloadStatus.SUCCESS_DOWNLOADED) || (this.<zipStatus>__2 != ZipStatus.ZIPPING))
                {
                    goto Label_0185;
                }
                this.<currentTaskIndex>__3++;
            Label_012C:
                if (this.<currentTaskIndex>__3 < this.<>f__this._downloadAssetBundleList.Count)
                {
                    this.<asbInfo>__4 = this.<>f__this._downloadAssetBundleList[this.<currentTaskIndex>__3];
                    this.<downloadStatus>__5 = DownloadStatus.DOWNLOADING;
                    this.<>f__this.AddDownloadTask(new AssetBundleDownloadTask(this.<asbInfo>__4, new Action<long, long, long, float>(this.<>m__72), new Action<AssetBundleDownloadTask>(this.<>m__73)));
                    goto Label_00E6;
                }
            Label_0185:
                while ((this.<zipStatus>__2 == ZipStatus.ZIPPING) && (this.<>f__this._zipBackGroundWorker.RemainCount > 0))
                {
                    if (this.onZipProgress != null)
                    {
                        this.onZipProgress(this.<>f__this._zipBackGroundWorker.RemainCount);
                    }
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_0243;
                }
                if (this.<zipStatus>__2 == ZipStatus.ZIPPING)
                {
                    this.<zipStatus>__2 = ZipStatus.SUCCESS_ZIPPED;
                    this.<>f__this._zipBackGroundWorker.StopBackGroundWork(true);
                }
                else if (this.<zipStatus>__2 == ZipStatus.FAIL_ZIPPED)
                {
                }
                if (this.onFinished != null)
                {
                    this.onFinished((this.<currentTaskIndex>__3 >= this.<>f__this._downloadAssetBundleList.Count) && (this.<zipStatus>__2 == ZipStatus.SUCCESS_ZIPPED));
                }
                this.<>f__this._downloadAssetBundleList.Clear();
                this.<>f__this._downloadAssetBundleList = null;
            Label_0233:
                Screen.sleepTimeout = -2;
                this.$PC = -1;
            Label_0241:
                return false;
            Label_0243:
                return true;
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

            private sealed class <StartDownloadAssetBundle>c__AnonStoreyB4
            {
                internal MonoAssetBundleLoader.<StartDownloadAssetBundle>c__Iterator3 <>f__ref$3;
                internal AssetBundleDownloadTask downloadTask;

                internal void <>m__74()
                {
                    if (!AssetBundleUtility.ValidateAndSaveAssetBundle(this.<>f__ref$3.bundleType, this.downloadTask))
                    {
                        this.<>f__ref$3.<zipStatus>__2 = ZipStatus.FAIL_ZIPPED;
                        this.<>f__ref$3.<>f__this._zipBackGroundWorker.StopBackGroundWork(true);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <TryStartDownloadOneAssetBundle>c__AnonStoreyB5
        {
            internal MonoAssetBundleLoader <>f__this;
            internal Action<long, long, long, float> onDownloadProgress;
            internal Action<bool> onFinished;
            internal string resPath;

            internal void <>m__6E()
            {
                this.<>f__this.CheckEventVersionAndDownloadOneAssetBundle(this.resPath, this.onDownloadProgress, this.onFinished);
            }
        }
    }
}

