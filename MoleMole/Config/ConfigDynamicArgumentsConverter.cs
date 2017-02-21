namespace MoleMole.Config
{
    using FullSerializer;
    using System;
    using System.Collections.Generic;

    public class ConfigDynamicArgumentsConverter : fsDirectConverter<ConfigDynamicArguments>
    {
        public override object CreateInstance(fsData data, Type storageType)
        {
            return new ConfigDynamicArguments();
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref ConfigDynamicArguments model)
        {
            foreach (string str in data.Keys)
            {
                fsData data2 = data[str];
                if (data2.IsBool)
                {
                    model.Add(str, data2.AsBool);
                }
                else if (data2.IsDouble)
                {
                    model.Add(str, (float) data2.AsDouble);
                }
                else if (data2.IsInt64)
                {
                    model.Add(str, (int) data2.AsInt64);
                }
                else if (data2.IsString)
                {
                    model.Add(str, data2.AsString);
                }
                else
                {
                    return fsResult.Fail(string.Format("invalid dynamic argument type {0} - {1} ", str, data2.Type.ToString()));
                }
            }
            return fsResult.Success;
        }

        protected override fsResult DoSerialize(ConfigDynamicArguments model, Dictionary<string, fsData> serialized)
        {
            foreach (string str in model.Keys)
            {
                object obj2 = model[str];
                if (obj2 is bool)
                {
                    serialized.Add(str, new fsData((bool) obj2));
                }
                else if (obj2 is float)
                {
                    serialized.Add(str, new fsData((double) ((float) obj2)));
                }
                else if (obj2 is int)
                {
                    serialized.Add(str, new fsData((long) ((int) obj2)));
                }
                else if (obj2 is string)
                {
                    serialized.Add(str, new fsData((string) obj2));
                }
                else
                {
                    return fsResult.Fail(string.Format("invalid dynamic argument type {0} - {1] ", str));
                }
            }
            return fsResult.Success;
        }
    }
}

