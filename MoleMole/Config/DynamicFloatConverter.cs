namespace MoleMole.Config
{
    using FullSerializer;
    using System;
    using System.Runtime.InteropServices;

    public class DynamicFloatConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return (type == typeof(DynamicFloat));
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new DynamicFloat();
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            DynamicFloat num = (DynamicFloat) instance;
            if (data.IsDouble)
            {
                num.isDynamic = false;
                num.fixedValue = (float) data.AsDouble;
                return fsResult.Success;
            }
            if (data.IsInt64)
            {
                num.isDynamic = false;
                num.fixedValue = data.AsInt64;
                return fsResult.Success;
            }
            if (!data.IsString)
            {
                return fsResult.Fail("DynamicFloat fields needs to be either a '%key' or a float value");
            }
            string asString = data.AsString;
            if (!asString.StartsWith("%"))
            {
                return fsResult.Fail("DynamicFloat key needs to be like '%key' .");
            }
            num.isDynamic = true;
            char[] trimChars = new char[] { '%' };
            num.dynamicKey = asString.TrimStart(trimChars);
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            DynamicFloat num = instance as DynamicFloat;
            if (num == null)
            {
                serialized = new fsData();
                return fsResult.Fail("Failed to convert field to DynamicFloat on serialization.");
            }
            serialized = !num.isDynamic ? new fsData((double) num.fixedValue) : new fsData("%" + num.dynamicKey);
            return fsResult.Success;
        }
    }
}

