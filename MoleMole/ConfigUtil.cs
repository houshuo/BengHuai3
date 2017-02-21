namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class ConfigUtil
    {
        private static object serializationHelperLock = new object();

        public static T LoadConfig<T>(string filePath) where T: ScriptableObject
        {
            return Miscs.LoadResource<T>(filePath, BundleType.RESOURCE_FILE);
        }

        public static AsyncAssetRequst LoadConfigAsync(string filePath, BundleType type = 1)
        {
            return Miscs.LoadResourceAsync(filePath, type);
        }

        public static T LoadJSONConfig<T>(string jsonPath) where T: class
        {
            T local2;
            try
            {
                T local;
                string content = Miscs.LoadTextFileToString(jsonPath);
                object serializationHelperLock = ConfigUtil.serializationHelperLock;
                lock (serializationHelperLock)
                {
                    local = SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(content);
                }
                local2 = local;
            }
            catch
            {
                Debug.LogError("Error during loading json config: " + jsonPath);
                throw;
            }
            return local2;
        }

        public static AsyncAssetRequst LoadJsonConfigAsync(string filePath, BundleType type = 0)
        {
            return Miscs.LoadResourceAsync(filePath, type);
        }

        public static T LoadJSONStrConfig<T>(string str) where T: class
        {
            try
            {
                object serializationHelperLock = ConfigUtil.serializationHelperLock;
                lock (serializationHelperLock)
                {
                    return SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(str);
                }
            }
            catch
            {
                return null;
            }
        }

        public static void LoadJSONStrConfigMultiThread<T>(string jsonText, BackGroundWorker worker, Action<T, string> callback, string param = "") where T: class
        {
            <LoadJSONStrConfigMultiThread>c__AnonStorey10E<T> storeye = new <LoadJSONStrConfigMultiThread>c__AnonStorey10E<T> {
                jsonText = jsonText,
                callback = callback,
                param = param
            };
            try
            {
                worker.AddBackGroundWork(new Action(storeye.<>m__1C9));
            }
            catch
            {
                Debug.LogError("Error during loading json config: " + storeye.jsonText);
                throw;
            }
        }

        public static string SaveJSONStrConfig<T>(T obj) where T: class
        {
            try
            {
                object serializationHelperLock = ConfigUtil.serializationHelperLock;
                lock (serializationHelperLock)
                {
                    return SerializationHelpers.SerializeToContent<T, FullSerializerSerializer>(obj);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        [CompilerGenerated]
        private sealed class <LoadJSONStrConfigMultiThread>c__AnonStorey10E<T> where T: class
        {
            internal Action<T, string> callback;
            internal string jsonText;
            internal string param;

            internal void <>m__1C9()
            {
                T local;
                object serializationHelperLock = ConfigUtil.serializationHelperLock;
                lock (serializationHelperLock)
                {
                    local = SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(this.jsonText);
                }
                this.callback(local, this.param);
            }
        }
    }
}

