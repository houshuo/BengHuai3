namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class AssetBundleInfo
    {
        private AssetBundle _assetBundle;
        private bool _completeness;
        private static string AssetPathSetPattern = "APS";
        private static string FileBundleTypePattern = "BT";
        private static string FileCompressedSizePattern = "CS";
        private static string FileCrcPattern = "CRC";
        private static string FileDownloadModePattern = "DLM";
        private static string FileNamePattern = "N";
        private static string FileUnloadModePattern = "ULM";
        private static string ParentFileNamePattern = "PN";
        private static string RemainPattern = "R";
        private static string RemoteDirPattern = "RD";

        public AssetBundleInfo(string name, long compressedSize, string crc, HashSet<string> parentFileNameSet, HashSet<string> assetPathSet, UnloadMode unloadMode, DownloadMode downloadMode, BundleType bundleType, bool remain, string remoteDir)
        {
            this.FileName = name;
            this.FileCompressedSize = compressedSize;
            this.FileCrc = crc;
            this.ParentFileNameSet = parentFileNameSet;
            this.AssetPathSet = assetPathSet;
            this.FileUnloadMode = unloadMode;
            this.FileDownloadMode = downloadMode;
            this.FileBundleType = bundleType;
            this.RemainInInstallPackage = remain;
            this.RemoteDir = remoteDir;
            this._assetBundle = null;
            this._completeness = false;
        }

        public static AssetBundleInfo FromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            try
            {
                JSONNode node = JSON.Parse(str);
                string name = (string) node[FileNamePattern];
                long asInt = node[FileCompressedSizePattern].AsInt;
                string crc = (string) node[FileCrcPattern];
                UnloadMode unloadMode = (UnloadMode) node[FileUnloadModePattern].AsInt;
                DownloadMode downloadMode = (DownloadMode) node[FileDownloadModePattern].AsInt;
                BundleType bundleType = (BundleType) node[FileBundleTypePattern].AsInt;
                bool asBool = node[RemainPattern].AsBool;
                string remoteDir = (string) node[RemoteDirPattern];
                HashSet<string> assetPathSet = new HashSet<string>();
                JSONArray asArray = node[AssetPathSetPattern].AsArray;
                for (int i = 0; i < asArray.Count; i++)
                {
                    assetPathSet.Add((string) asArray[i]);
                }
                HashSet<string> parentFileNameSet = new HashSet<string>();
                JSONArray array2 = node[ParentFileNamePattern].AsArray;
                for (int j = 0; j < array2.Count; j++)
                {
                    parentFileNameSet.Add((string) array2[j]);
                }
                return new AssetBundleInfo(name, asInt, crc, parentFileNameSet, assetPathSet, unloadMode, downloadMode, bundleType, asBool, remoteDir);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IsDownloaded()
        {
            return File.Exists(this.LocalFilePath);
        }

        public bool IsLoaded()
        {
            return (this._assetBundle != null);
        }

        public T Load<T>(string name) where T: UnityEngine.Object
        {
            this.PreLoad();
            if (this._assetBundle != null)
            {
                return this._assetBundle.LoadAsset<T>(name);
            }
            return null;
        }

        public AssetBundleRequest LoadAsync(string name)
        {
            this.PreLoad();
            if (this._assetBundle != null)
            {
                return this._assetBundle.LoadAssetAsync(name);
            }
            return null;
        }

        private void PreLoad()
        {
            if ((this._assetBundle == null) && File.Exists(this.LocalFilePath))
            {
                foreach (string str in this.ParentFileNameSet)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        Singleton<AssetBundleManager>.Instance.GetAssetBundleInfoByFileName(this.FileBundleType, str).PreLoad();
                    }
                }
                try
                {
                    if (this.FileBundleType == BundleType.DATA_FILE)
                    {
                        byte[] fileBytes = File.ReadAllBytes(this.LocalFilePath);
                        if (!this._completeness)
                        {
                            this._completeness = true;
                            if (AssetBundleUtility.CalculateFileCrc(null, fileBytes) != this.FileCrc)
                            {
                                throw new Exception("File is Not Completeness.");
                            }
                        }
                        AssetBundleUtility.MyAESDecrypted(ref fileBytes);
                        this._assetBundle = AssetBundle.LoadFromMemory(fileBytes);
                    }
                    else
                    {
                        this._assetBundle = AssetBundle.LoadFromFile(this.LocalFilePath);
                    }
                }
                catch (Exception)
                {
                    this._assetBundle = null;
                }
                if (this._assetBundle == null)
                {
                    if (File.Exists(this.LocalFilePath))
                    {
                        File.Delete(this.LocalFilePath);
                    }
                    Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_AssetBundleError", new object[0]), null, new Action(Application.Quit), LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])), UIType.Any);
                }
            }
        }

        public override string ToString()
        {
            JSONClass class2 = new JSONClass();
            class2.Add(FileNamePattern, new JSONData(this.FileName));
            class2.Add(FileCompressedSizePattern, new JSONData((int) this.FileCompressedSize));
            class2.Add(FileCrcPattern, new JSONData(this.FileCrc));
            class2.Add(FileUnloadModePattern, new JSONData((int) this.FileUnloadMode));
            class2.Add(FileDownloadModePattern, new JSONData((int) this.FileDownloadMode));
            class2.Add(FileBundleTypePattern, new JSONData((int) this.FileBundleType));
            class2.Add(RemainPattern, new JSONData(this.RemainInInstallPackage));
            class2.Add(RemoteDirPattern, new JSONData(this.RemoteDir));
            JSONArray aItem = new JSONArray();
            foreach (string str in this.AssetPathSet)
            {
                aItem.Add(new JSONData(str));
            }
            class2.Add(AssetPathSetPattern, aItem);
            JSONArray array2 = new JSONArray();
            foreach (string str2 in this.ParentFileNameSet)
            {
                array2.Add(new JSONData(str2));
            }
            class2.Add(ParentFileNamePattern, array2);
            return class2.ToString();
        }

        public void Unload(bool forceUnload)
        {
            if (this._assetBundle != null)
            {
                this._assetBundle.Unload(forceUnload);
                this._assetBundle = null;
            }
        }

        public HashSet<string> AssetPathSet { get; private set; }

        public BundleType FileBundleType { get; private set; }

        public long FileCompressedSize { get; private set; }

        public string FileCrc { get; private set; }

        public DownloadMode FileDownloadMode { get; private set; }

        public string FileName { get; private set; }

        public UnloadMode FileUnloadMode { get; private set; }

        public string LocalFilePath
        {
            get
            {
                return (AssetBundleUtility.LocalAssetBundleDirectory(this.FileBundleType) + this.FileName + (!string.IsNullOrEmpty(this.FileCrc) ? ("_" + this.FileCrc) : string.Empty) + ".unity3d");
            }
        }

        public HashSet<string> ParentFileNameSet { get; private set; }

        public bool RemainInInstallPackage { get; private set; }

        public string RemoteDir { get; private set; }

        public string RemoteFilePath
        {
            get
            {
                return (AssetBundleUtility.RemoteAssetBundleDirctory(this.FileBundleType, Singleton<AssetBundleManager>.Instance.remoteAssetBundleUrl, this.RemoteDir) + this.FileName + (!string.IsNullOrEmpty(this.FileCrc) ? ("_" + this.FileCrc) : ".unity3d"));
            }
        }
    }
}

