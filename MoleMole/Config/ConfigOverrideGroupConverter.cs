namespace MoleMole.Config
{
    using FullSerializer;
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class ConfigOverrideGroupConverter : fsBaseConfigOverrideConverter
    {
        public override bool CanProcess(System.Type type)
        {
            return (type == typeof(ConfigOverrideGroup));
        }

        public override object CreateInstance(fsData data, System.Type storageType)
        {
            return new ConfigOverrideGroup();
        }

        public override bool RequestCycleSupport(System.Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, System.Type storageType)
        {
            fsResult result;
            fsData data2;
            fsResult result2 = result = fsResult.Success + base.CheckType(data, fsDataType.Object);
            if (result2.Failed)
            {
                return result;
            }
            Dictionary<string, fsData> asDictionary = data.AsDictionary;
            fsResult result3 = result += base.CheckKey(data, "Default", out data2);
            if (result3.Failed)
            {
                return result;
            }
            List<string> list = new List<string>(asDictionary.Keys);
            for (int i = 0; i < list.Count; i++)
            {
                string str = list[i];
                if (str != "Default")
                {
                    fsData data3;
                    result += base.Override(data2, asDictionary[str], out data3);
                    if (result.Failed)
                    {
                        return result;
                    }
                    asDictionary[str] = data3;
                }
            }
            ConfigOverrideGroup group = (ConfigOverrideGroup) instance;
            fsData data4 = asDictionary["Default"];
            object obj2 = null;
            base.Serializer.TryDeserialize(data4, typeof(object), ref obj2);
            group.Default = obj2;
            if (obj2 is IOnLoaded)
            {
                ((IOnLoaded) obj2).OnLoaded();
            }
            bool flag = list.Remove("Default");
            if (list.Count > 0)
            {
                group.Overrides = new Dictionary<string, object>();
                for (int j = 0; j < list.Count; j++)
                {
                    string str2 = list[j];
                    data4 = asDictionary[str2];
                    obj2 = null;
                    base.Serializer.TryDeserialize(data4, typeof(object), ref obj2);
                    group.Overrides[str2] = obj2;
                    if (obj2 is IOnLoaded)
                    {
                        ((IOnLoaded) obj2).OnLoaded();
                    }
                }
            }
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, System.Type storageType)
        {
            fsData data;
            fsResult success = fsResult.Success;
            serialized = fsData.CreateDictionary();
            Dictionary<string, fsData> asDictionary = serialized.AsDictionary;
            ConfigOverrideGroup group = (ConfigOverrideGroup) instance;
            success += base.Serializer.TrySerialize(group.Default.GetType(), group.Default, out data);
            if (!success.Failed)
            {
                asDictionary.Add("Default", data);
                foreach (string str in group.Overrides.Keys)
                {
                    object obj2 = group.Overrides[str];
                    success += base.Serializer.TrySerialize(obj2.GetType(), obj2, out data);
                    if (success.Failed)
                    {
                        return success;
                    }
                    asDictionary.Add(str, data);
                }
            }
            return success;
        }
    }
}

