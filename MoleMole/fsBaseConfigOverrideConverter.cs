namespace MoleMole
{
    using FullSerializer;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public abstract class fsBaseConfigOverrideConverter : fsConverter
    {
        protected fsBaseConfigOverrideConverter()
        {
        }

        protected fsResult Override(fsData defaultData, fsData targetData, out fsData outData)
        {
            fsResult success = fsResult.Success;
            outData = fsData.CreateDictionary();
            fsResult result2 = success += base.CheckType(defaultData, fsDataType.Object);
            if (!result2.Failed)
            {
                fsResult result3 = success += base.CheckType(targetData, fsDataType.Object);
                if (result3.Failed)
                {
                    return success;
                }
                Dictionary<string, fsData> asDictionary = defaultData.AsDictionary;
                Dictionary<string, fsData> dictionary2 = targetData.AsDictionary;
                Dictionary<string, fsData> dictionary3 = outData.AsDictionary;
                foreach (string str in asDictionary.Keys)
                {
                    fsData data = asDictionary[str];
                    fsData data2 = null;
                    dictionary2.TryGetValue(str, out data2);
                    if (data.IsDictionary && (data2 != null))
                    {
                        fsData data3;
                        fsResult result4 = success += base.CheckType(data2, fsDataType.Object);
                        if (result4.Failed)
                        {
                            return success;
                        }
                        success += this.Override(data, data2, out data3);
                        dictionary3.Add(str, data3);
                    }
                    else if ((data2 == null) || (data.Type == data2.Type))
                    {
                        dictionary3.Add(str, (data2 == null) ? data : data2);
                    }
                    else if (((data.Type == fsDataType.Double) && (data2.Type == fsDataType.Int64)) || ((data.Type == fsDataType.Int64) && (data2.Type == fsDataType.Double)))
                    {
                        dictionary3.Add(str, data2);
                    }
                    else
                    {
                        success += fsResult.Fail("override value type doesn't match: " + str);
                    }
                }
                foreach (string str2 in dictionary2.Keys)
                {
                    if (!dictionary3.ContainsKey(str2))
                    {
                        dictionary3.Add(str2, dictionary2[str2]);
                    }
                }
            }
            return success;
        }
    }
}

