namespace MoleMole
{
    using FullSerializer;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UniRx;

    public class ConfigOverrideListConverter : fsBaseConfigOverrideConverter
    {
        public override bool CanProcess(System.Type type)
        {
            return (type == typeof(ConfigOverrideList));
        }

        public override object CreateInstance(fsData data, System.Type storageType)
        {
            return new ConfigOverrideList();
        }

        public override bool RequestCycleSupport(System.Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, System.Type storageType)
        {
            fsResult result;
            fsResult result2 = result = fsResult.Success + base.CheckType(data, fsDataType.Array);
            if (result2.Failed)
            {
                return result;
            }
            List<fsData> asList = data.AsList;
            ConfigOverrideList list2 = (ConfigOverrideList) instance;
            list2.objects = new object[asList.Count];
            Tuple<string, bool, fsData>[] tupleArray = new Tuple<string, bool, fsData>[asList.Count];
            for (int i = 0; i < asList.Count; i++)
            {
                string asString;
                bool flag;
                fsResult result3 = result += base.CheckType(asList[i], fsDataType.Object);
                if (result3.Failed)
                {
                    return result;
                }
                Dictionary<string, fsData> asDictionary = asList[i].AsDictionary;
                if (asDictionary.ContainsKey("$refbase"))
                {
                    fsData data2 = asDictionary["$refbase"];
                    fsResult result4 = result += base.CheckType(data2, fsDataType.String);
                    if (result4.Failed)
                    {
                        return result;
                    }
                    asString = data2.AsString;
                    flag = true;
                    tupleArray[i] = Tuple.Create<string, bool, fsData>(asString, flag, asList[i]);
                }
                else if (asDictionary.ContainsKey("$refoverride"))
                {
                    fsData data3 = asDictionary["$refoverride"];
                    fsResult result5 = result += base.CheckType(data3, fsDataType.String);
                    if (result5.Failed)
                    {
                        return result;
                    }
                    asString = data3.AsString;
                    flag = false;
                    tupleArray[i] = Tuple.Create<string, bool, fsData>(asString, flag, asList[i]);
                }
                else
                {
                    tupleArray[i] = Tuple.Create<string, bool, fsData>(null, false, asList[i]);
                }
            }
            for (int j = 0; j < tupleArray.Length; j++)
            {
                Tuple<string, bool, fsData> tuple = tupleArray[j];
                if (tuple.Item2 || (tuple.Item1 == null))
                {
                    result += base.Serializer.TryDeserialize(tuple.Item3, typeof(object), ref list2.objects[j]);
                }
                else
                {
                    fsData data5;
                    fsData defaultData = null;
                    for (int k = 0; k < tupleArray.Length; k++)
                    {
                        Tuple<string, bool, fsData> tuple2 = tupleArray[k];
                        if (tuple2.Item1 == tuple.Item1)
                        {
                            if (!tuple2.Item2)
                            {
                                return (result + fsResult.Fail("$refoverride needs to point to a $refbase entry " + tuple.Item1));
                            }
                            defaultData = tuple2.Item3;
                            break;
                        }
                    }
                    if (defaultData == null)
                    {
                        return (result + fsResult.Fail("missing base entry " + tuple.Item1));
                    }
                    result += base.Override(defaultData, tuple.Item3, out data5);
                    result += base.Serializer.TryDeserialize(data5, typeof(object), ref list2.objects[j]);
                }
                object obj2 = list2.objects[j];
                if (obj2 is IOnLoaded)
                {
                    ((IOnLoaded) obj2).OnLoaded();
                }
            }
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, System.Type storageType)
        {
            fsResult success = fsResult.Success;
            serialized = fsData.CreateList();
            ConfigOverrideList list = (ConfigOverrideList) instance;
            foreach (object obj2 in list.objects)
            {
                fsData data;
                success += base.Serializer.TrySerialize(obj2.GetType(), obj2, out data);
                if (success.Failed)
                {
                    return success;
                }
                serialized.AsList.Add(data);
            }
            return success;
        }
    }
}

