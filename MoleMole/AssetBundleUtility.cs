namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public class AssetBundleUtility
    {
        public static byte BYTE_SALT = 0xa5;
        private static string DATA_PATH = Application.dataPath;
        public static byte[] ENCRYPTED_KEY;
        private static bool LOCAL_TEST = false;
        private static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
        public static string RSA_SALT = "<RSAKeyValue><Modulus>rXoKWm82JSX4UYihkt2FSjrp3pZqTxt6AyJ0ZexHssStYesCFuUOmDBrk0nxPTY2r7oB4ZC9tDhHzmA66Me56wkD47Z3fCEBfLFxmEVdUCvM1RIFdQxQCB7CMaFWXHoVfBhNcD60OtXD71vFusBLioa6HDHbKk8LdgWdV10OWaE=</Modulus><Exponent>EQ==</Exponent><P>16GiwrgCGvcYbgSZOBJRx4G9kioGgexLSyW62iK4EuT0Xu9xyflBDaC4yooFkxrflqEAIiEfTqNGlYeJks+5qw==</P><Q>zfQY4dWi/Dlo38y6xvX4pUEAj1hbeFo/Qiy7H00P089W0KC6Mdi+GY4UuRGJtgX7UZfGQdHRj8mBjijFyhUl4w==</Q><DP>cihlOejyDkaUdnrntEXvD0Svp7vlU9dzJ8iuNz+OoJdUMkKHiQt8yvq8Lv3Gt0p2Xs20xsY9wDhSi2Xfa9diSw==</DP><DQ>GDrVwDdAWeii7SclCFksT61LXCiDO1XpUxRSP+ryzZ/sGIthMwpwt7ZcynqIrAC0J7eAvHMJmHIPPeat24oEdQ==</DQ><InverseQ>P4/vgq1XF77N8K/OxTbcjWFCC1d+v3W5xWQJbmU3KfVF2wOStZeILT2X12s7AHD+uUfN9O/xdEBIeqcSLVxWjw==</InverseQ><D>o0WvZCxvMgWeatrybBvIvlWQ0X6CLFYYe2u42GXpILkbp3PFuzHvnkuwip/yG35RllS2efGjfHE0hgA3cazrNgM6gBDcFa7iznviIiQTySxFuzy3mXpjSQFaGgdvmuUQLgg5qahcdGgT455Fzo5GSu+IyTpD+dNoKy79NLTbvjE=</D></RSAKeyValue>";

        public static string CalculateFileCrc(string filePath, byte[] fileBytes)
        {
            try
            {
                byte[] destinationArray = new byte[ENCRYPTED_KEY.Length];
                Array.Copy(ENCRYPTED_KEY, 0, destinationArray, 0, ENCRYPTED_KEY.Length);
                SecurityUtil.RSADecrypted(ref destinationArray, RSA_SALT);
                byte[] buffer2 = new byte[8];
                Array.Copy(destinationArray, 0x30, buffer2, 0, 8);
                string str = SecurityUtil.CalculateFileHash(fileBytes, buffer2);
                ClearBuffer(destinationArray);
                ClearBuffer(buffer2);
                return str;
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        private static void ClearBuffer(byte[] buffer)
        {
            if (buffer != null)
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0;
                }
                buffer = null;
            }
        }

        public static void CreateParentDirectory(string path)
        {
            string str = path.Substring(0, path.LastIndexOf('/'));
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
        }

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static string GetAssetPath(string path)
        {
            return path.Replace(Application.dataPath + "/", string.Empty);
        }

        public static List<string> GetFileList(string path)
        {
            List<string> list = new List<string>();
            if (Directory.Exists(path))
            {
                foreach (string str in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    if (((!str.EndsWith(".DS_Store") && !str.EndsWith(".meta")) && !str.EndsWith(".bat")) && !str.EndsWith(".mask"))
                    {
                        list.Add(str.Replace('\\', '/'));
                    }
                }
                return list;
            }
            list.Add(path.Replace('\\', '/'));
            return list;
        }

        public static string GetResourcePath(string path)
        {
            string str = path.Replace("Assets/Resources/", string.Empty);
            if (str.Contains("."))
            {
                str = str.Substring(0, str.LastIndexOf('.'));
            }
            return str;
        }

        public static string GetResourcePathForEditor(string path)
        {
            string str = path.Replace(Application.dataPath + "/Resources/", string.Empty);
            if (str.Contains("."))
            {
                str = str.Substring(0, str.LastIndexOf('.'));
            }
            return str;
        }

        public static AssetBundleInfo[] GetVersionAssetBundleInfo(BundleType bundleType)
        {
            if (bundleType == BundleType.DATA_FILE)
            {
                return new AssetBundleInfo[] { new AssetBundleInfo("DataVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, string.Empty) };
            }
            return new AssetBundleInfo[] { new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, "event"), new AssetBundleInfo("ResourceVersion", 100L, string.Empty, null, null, UnloadMode.MANUAL_UNLOAD, DownloadMode.IMMEDIATELY, bundleType, false, string.Empty) };
        }

        public static string LocalAssetBundleDirectory(BundleType bundleType)
        {
            string str = string.Empty;
            BundleType type = bundleType;
            if (type != BundleType.DATA_FILE)
            {
                if (type != BundleType.RESOURCE_FILE)
                {
                    return str;
                }
            }
            else
            {
                return (PERSISTENT_DATA_PATH + "/Data/");
            }
            return (PERSISTENT_DATA_PATH + "/Resources/");
        }

        public static void MyAESDecrypted(ref byte[] bytes)
        {
            try
            {
                byte[] destinationArray = new byte[ENCRYPTED_KEY.Length];
                Array.Copy(ENCRYPTED_KEY, 0, destinationArray, 0, ENCRYPTED_KEY.Length);
                SecurityUtil.RSADecrypted(ref destinationArray, RSA_SALT);
                byte[] buffer2 = new byte[0x20];
                byte[] buffer3 = new byte[0x10];
                Array.Copy(destinationArray, 0, buffer2, 0, 0x20);
                Array.Copy(destinationArray, 0x20, buffer3, 0, 0x10);
                SecurityUtil.AESDecrypted(ref bytes, buffer2, buffer3);
                ClearBuffer(destinationArray);
                ClearBuffer(buffer2);
                ClearBuffer(buffer3);
            }
            catch (Exception)
            {
            }
        }

        public static void RebuildDirectory(string path)
        {
            string str = path.Substring(0, path.LastIndexOf('/'));
            if (Directory.Exists(str))
            {
                Directory.Delete(str, true);
            }
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
        }

        public static string RemoteAssetBundleDirctory(BundleType bundleType, string serverAddr, string remoteDir)
        {
            string str2;
            string str = string.Empty;
            BundleType type = bundleType;
            if (type != BundleType.DATA_FILE)
            {
                if (type != BundleType.RESOURCE_FILE)
                {
                    throw new Exception("Invalid Type or State!");
                }
            }
            else
            {
                str = "data";
                goto Label_003B;
            }
            str = "resource";
        Label_003B:
            str2 = !string.IsNullOrEmpty(remoteDir) ? remoteDir : str;
            if (LOCAL_TEST)
            {
                string[] textArray1 = new string[] { "file://", DATA_PATH, "/../Packages/", str2, "/editor_compressed/" };
                return string.Concat(textArray1);
            }
            string str3 = "android_compressed";
            string[] textArray2 = new string[] { serverAddr, "/", str2, "/", str3, "/" };
            return string.Concat(textArray2);
        }

        public static bool ValidateAndSaveAssetBundle(BundleType bundleType, AssetBundleDownloadTask task)
        {
            string localFilePath = task.AssetBundleInfo.LocalFilePath;
            try
            {
                CreateParentDirectory(localFilePath);
                DeleteFile(localFilePath);
                byte[] downloadedBytes = task.DownloadedBytes;
                AssetBundleInfo assetBundleInfo = task.AssetBundleInfo;
                if (GlobalVars.DataUseAssetBundle)
                {
                    string str2 = CalculateFileCrc(localFilePath, downloadedBytes);
                    if (str2 != assetBundleInfo.FileCrc)
                    {
                        throw new Exception(string.Format("CRC Mismatch. Local:{0}, Remote:{1}. Retry downloading.", str2, assetBundleInfo.FileCrc));
                    }
                }
                File.WriteAllBytes(localFilePath, downloadedBytes);
                return true;
            }
            catch (Exception)
            {
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }
                return false;
            }
        }
    }
}

