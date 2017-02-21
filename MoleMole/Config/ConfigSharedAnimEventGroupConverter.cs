namespace MoleMole.Config
{
    using FullSerializer;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class ConfigSharedAnimEventGroupConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return (type == typeof(ConfigSharedAnimEventGroup));
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new ConfigSharedAnimEventGroup();
        }

        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            fsResult result;
            fsData data2;
            fsData data3;
            fsData data4;
            fsResult result2 = result = fsResult.Success + base.CheckType(data, fsDataType.Object);
            if (result2.Failed)
            {
                return result;
            }
            fsResult result3 = result += base.CheckKey(data, "Prefix", out data2);
            if (result3.Failed)
            {
                return result;
            }
            fsResult result4 = result += base.CheckKey(data, "Type", out data3);
            if (result4.Failed)
            {
                return result;
            }
            fsResult result5 = result += base.CheckKey(data, "AnimEvents", out data4);
            if (result5.Failed)
            {
                return result;
            }
            fsResult result6 = result += base.CheckType(data2, fsDataType.String);
            if (result6.Failed)
            {
                return result;
            }
            fsResult result7 = result += base.CheckType(data3, fsDataType.String);
            if (result7.Failed)
            {
                return result;
            }
            fsResult result8 = result += base.CheckType(data4, fsDataType.Object);
            if (result8.Failed)
            {
                return result;
            }
            ConfigSharedAnimEventGroup group = (ConfigSharedAnimEventGroup) instance;
            group.Prefix = data2.AsString;
            if (!group.Prefix.StartsWith("#"))
            {
                return (result += fsResult.Fail("prefix should starts with '#': " + result));
            }
            group.Type = data3.AsString;
            group.AnimEvents = new Dictionary<string, ConfigEntityAnimEvent>();
            foreach (KeyValuePair<string, fsData> pair in data4.AsDictionary)
            {
                string key = pair.Key;
                fsData data5 = pair.Value;
                fsResult result9 = result += base.CheckType(data4, fsDataType.Object);
                if (result9.Failed)
                {
                    return result;
                }
                data5.AsDictionary["$type"] = data3;
                ConfigEntityAnimEvent event2 = null;
                base.Serializer.TryDeserialize<ConfigEntityAnimEvent>(data5, ref event2);
                group.AnimEvents.Add(string.Format("{0}/{1}", group.Prefix, key), event2);
            }
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            fsResult success = fsResult.Success;
            base.Serializer.TrySerialize(instance.GetType(), instance, out serialized);
            return success;
        }
    }
}

