namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AssetBundleManager
    {
        private Dictionary<BundleType, Dictionary<string, string>> _assetName2FileNameDict;
        private Dictionary<BundleType, Dictionary<string, AssetBundleInfo>> _fileName2AssetBundleDict = new Dictionary<BundleType, Dictionary<string, AssetBundleInfo>>();
        private Dictionary<BundleType, Dictionary<string, string>> _resPath2AssetNameDict;
        public string remoteAssetBundleUrl;

        private AssetBundleManager()
        {
            this._fileName2AssetBundleDict.Add(BundleType.DATA_FILE, new Dictionary<string, AssetBundleInfo>());
            this._fileName2AssetBundleDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, AssetBundleInfo>());
            this._assetName2FileNameDict = new Dictionary<BundleType, Dictionary<string, string>>();
            this._assetName2FileNameDict.Add(BundleType.DATA_FILE, new Dictionary<string, string>());
            this._assetName2FileNameDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, string>());
            this._resPath2AssetNameDict = new Dictionary<BundleType, Dictionary<string, string>>();
            this._resPath2AssetNameDict.Add(BundleType.DATA_FILE, new Dictionary<string, string>());
            this._resPath2AssetNameDict.Add(BundleType.RESOURCE_FILE, new Dictionary<string, string>());
            this.InitAssetBundleLoader();
        }

        public void CheckSVNVersion()
        {
            if (GlobalVars.DataUseAssetBundle && (TimeUtil.Now >= this.Loader.checkSVNVersionDate.AddSeconds((double) MiscData.Config.BasicConfig.CheckAssetBoundleIntervalSecond)))
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(this.GetAssetBoundleStatusFilePath(), new Action<string>(this.GetAssetBoundleStatusCallBack), null, 5f, 3, null, null));
            }
        }

        public void ClearAssetBundle(BundleType bundleType)
        {
            foreach (AssetBundleInfo info in this._fileName2AssetBundleDict[bundleType].Values)
            {
                if (info != null)
                {
                    info.Unload(true);
                }
            }
            this._fileName2AssetBundleDict[bundleType].Clear();
            this._assetName2FileNameDict[bundleType].Clear();
            this._resPath2AssetNameDict[bundleType].Clear();
        }

        public void Destroy()
        {
            foreach (BundleType type in this._fileName2AssetBundleDict.Keys)
            {
                this.ClearAssetBundle(type);
            }
            if (this.Loader != null)
            {
                UnityEngine.Object.Destroy(this.Loader.gameObject);
            }
        }

        private void GetAssetBoundleStatusCallBack(string version)
        {
            string str = version.Trim();
            this.Loader.checkSVNVersionDate = TimeUtil.Now;
            if (string.IsNullOrEmpty(this.Loader.assetBoundleSVNVersion))
            {
                this.Loader.assetBoundleSVNVersion = str;
            }
            else if (this.Loader.assetBoundleSVNVersion != str)
            {
                this.Loader.assetBoundleSVNVersion = str;
                Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_NeedToRestartGame", new object[0]), null, new Action(GeneralLogicManager.RestartGame), LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])), UIType.Any);
            }
        }

        public string GetAssetBoundleStatusFilePath()
        {
            string str = "/data/build.status";
            return (this.remoteAssetBundleUrl + str);
        }

        public AssetBundleInfo GetAssetBundleInfoByAssetName(BundleType bundleType, string assetName)
        {
            string fileNameByAssetName = this.GetFileNameByAssetName(bundleType, assetName);
            if (fileNameByAssetName != null)
            {
                return this.GetAssetBundleInfoByFileName(bundleType, fileNameByAssetName);
            }
            return null;
        }

        public AssetBundleInfo GetAssetBundleInfoByFileName(BundleType bundleType, string fileName)
        {
            if (this._fileName2AssetBundleDict[bundleType].ContainsKey(fileName))
            {
                return this._fileName2AssetBundleDict[bundleType][fileName];
            }
            return null;
        }

        public AssetBundleInfo GetAssetBundleInfoByResPath(BundleType bundleType, string resPath)
        {
            string assetNameByResPath = Singleton<AssetBundleManager>.Instance.GetAssetNameByResPath(bundleType, resPath);
            return (!string.IsNullOrEmpty(assetNameByResPath) ? Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByAssetName(BundleType.RESOURCE_FILE, assetNameByResPath) : null);
        }

        public Dictionary<string, AssetBundleInfo> GetAssetBundleInfoDict(BundleType bundleType)
        {
            return this._fileName2AssetBundleDict[bundleType];
        }

        public string GetAssetNameByResPath(BundleType bundleType, string resPath)
        {
            if (this._resPath2AssetNameDict[bundleType].ContainsKey(resPath))
            {
                return this._resPath2AssetNameDict[bundleType][resPath];
            }
            return null;
        }

        public string GetDataVersion()
        {
            AssetBundleInfo assetBundleInfoByFileName = this.GetAssetBundleInfoByFileName(BundleType.DATA_FILE, "Data/all");
            if (assetBundleInfoByFileName != null)
            {
                return assetBundleInfoByFileName.FileCrc;
            }
            return "NoData";
        }

        public string GetEventAssetBoundleStatusFilePath()
        {
            string str = "/event/build.status";
            return (this.remoteAssetBundleUrl + str);
        }

        public string GetFileNameByAssetName(BundleType bundleType, string assetName)
        {
            if (this._assetName2FileNameDict[bundleType].ContainsKey(assetName))
            {
                return this._assetName2FileNameDict[bundleType][assetName];
            }
            return null;
        }

        private void InitAssetBundleLoader()
        {
            if (this.Loader == null)
            {
                GameObject target = new GameObject {
                    name = "AssetBundleLoader"
                };
                UnityEngine.Object.DontDestroyOnLoad(target);
                this.Loader = target.AddComponent<MonoAssetBundleLoader>();
            }
        }

        private T Load<T>(BundleType bundleType, string resPath) where T: UnityEngine.Object
        {
            string assetNameByResPath = this.GetAssetNameByResPath(bundleType, resPath);
            AssetBundleInfo info = !string.IsNullOrEmpty(assetNameByResPath) ? this.GetAssetBundleInfoByAssetName(bundleType, assetNameByResPath) : null;
            if ((info == null) || !info.IsDownloaded())
            {
                return Resources.Load<T>(resPath);
            }
            switch ((((!GlobalVars.UseSpliteResources ? 0 : 1) * 3) + info.FileDownloadMode))
            {
                case 0:
                    return Resources.Load<T>(resPath);

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return info.Load<T>(assetNameByResPath);
            }
            return null;
        }

        private AsyncAssetRequst LoadAsync(BundleType bundleType, string resPath)
        {
            string assetNameByResPath = this.GetAssetNameByResPath(bundleType, resPath);
            AssetBundleInfo info = !string.IsNullOrEmpty(assetNameByResPath) ? this.GetAssetBundleInfoByAssetName(bundleType, assetNameByResPath) : null;
            if ((info == null) || !info.IsDownloaded())
            {
                return new AsyncAssetRequst(Resources.LoadAsync(resPath));
            }
            switch ((((!GlobalVars.UseSpliteResources ? 0 : 1) * 3) + info.FileDownloadMode))
            {
                case 0:
                    return new AsyncAssetRequst(Resources.LoadAsync(resPath));

                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                    return new AsyncAssetRequst(info.LoadAsync(assetNameByResPath));
            }
            return null;
        }

        public T LoadData<T>(string resPath) where T: UnityEngine.Object
        {
            if (!GlobalVars.DataUseAssetBundle)
            {
                return Resources.Load<T>(resPath);
            }
            return this.Load<T>(BundleType.DATA_FILE, resPath);
        }

        public AsyncAssetRequst LoadDataAsync(string resPath)
        {
            if (!GlobalVars.DataUseAssetBundle)
            {
                return new AsyncAssetRequst(Resources.LoadAsync(resPath));
            }
            return this.LoadAsync(BundleType.DATA_FILE, resPath);
        }

        public T LoadRes<T>(string resPath) where T: UnityEngine.Object
        {
            if (!GlobalVars.ResourceUseAssetBundle)
            {
                return Resources.Load<T>(resPath);
            }
            return this.Load<T>(BundleType.RESOURCE_FILE, resPath);
        }

        public AsyncAssetRequst LoadResAsync(string resPath)
        {
            if (!GlobalVars.ResourceUseAssetBundle)
            {
                return new AsyncAssetRequst(Resources.LoadAsync(resPath));
            }
            return this.LoadAsync(BundleType.RESOURCE_FILE, resPath);
        }

        public void MergeAssetBundleDictOnRequire(BundleType bundleType, Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict)
        {
            foreach (string str in fileName2AssetBundleDict.Keys)
            {
                if (!this._fileName2AssetBundleDict[bundleType].ContainsKey(str) && (fileName2AssetBundleDict[str].FileDownloadMode == DownloadMode.ON_REQUIRE))
                {
                    this._fileName2AssetBundleDict[bundleType][str] = fileName2AssetBundleDict[str];
                    foreach (string str2 in this._fileName2AssetBundleDict[bundleType][str].AssetPathSet)
                    {
                        this._assetName2FileNameDict[bundleType][str2] = str;
                        string resourcePath = AssetBundleUtility.GetResourcePath(str2);
                        this._resPath2AssetNameDict[bundleType][resourcePath] = str2;
                    }
                }
            }
        }

        public static void RemoveAllAssetBundle(BundleType bundleType)
        {
            string path = AssetBundleUtility.LocalAssetBundleDirectory(bundleType);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void SetAssetBundleDict(BundleType bundleType, Dictionary<string, AssetBundleInfo> fileName2AssetBundleDict)
        {
            this._fileName2AssetBundleDict[bundleType] = fileName2AssetBundleDict;
            foreach (KeyValuePair<string, AssetBundleInfo> pair in fileName2AssetBundleDict)
            {
                foreach (string str in pair.Value.AssetPathSet)
                {
                    this._assetName2FileNameDict[bundleType][str] = pair.Key;
                    string resourcePath = AssetBundleUtility.GetResourcePath(str);
                    this._resPath2AssetNameDict[bundleType][resourcePath] = str;
                }
            }
        }

        public void UnloadUnusedAssetBundle(BundleType bundleType)
        {
            foreach (AssetBundleInfo info in this._fileName2AssetBundleDict[bundleType].Values)
            {
                if (((info != null) && info.IsLoaded()) && (info.FileUnloadMode == UnloadMode.MANUAL_UNLOAD))
                {
                    info.Unload(false);
                }
            }
        }

        public void UpdateEventSVNVersion(Action OnChangedCallback = null)
        {
            <UpdateEventSVNVersion>c__AnonStoreyB1 yb = new <UpdateEventSVNVersion>c__AnonStoreyB1 {
                OnChangedCallback = OnChangedCallback,
                <>f__this = this
            };
            if (GlobalVars.ResourceUseAssetBundle)
            {
                Singleton<ApplicationManager>.Instance.StartCoroutine(Miscs.WWWRequestWithRetry(this.GetEventAssetBoundleStatusFilePath(), new Action<string>(yb.<>m__6B), null, 5f, 3, null, null));
            }
        }

        public MonoAssetBundleLoader Loader { get; private set; }

        [CompilerGenerated]
        private sealed class <UpdateEventSVNVersion>c__AnonStoreyB1
        {
            internal AssetBundleManager <>f__this;
            internal Action OnChangedCallback;

            internal void <>m__6B(string version)
            {
                string str = version.Trim();
                if (this.<>f__this.Loader.eventAssetBoundleSVNVersion != str)
                {
                    this.<>f__this.Loader.eventAssetBoundleSVNVersion = str;
                    if (this.OnChangedCallback != null)
                    {
                        this.OnChangedCallback();
                    }
                }
            }
        }
    }
}

