namespace MoleMole.Config
{
    using FullSerializer;
    using System;
    using System.Runtime.InteropServices;

    public class DynamicIntConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return (type == typeof(DynamicInt));
        }

        public override object CreateInstance(fsData data, Type storageType)
        {
            return new DynamicInt();
        }

        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            DynamicInt num = (DynamicInt) instance;
            if (data.IsInt64)
            {
                num.isDynamic = false;
                num.fixedValue = (int) data.AsInt64;
                return fsResult.Success;
            }
            if (!data.IsString)
            {
                return fsResult.Fail("DynamicInt fields needs to be either a '%key' or a int value.");
            }
            string asString = data.AsString;
            if (!asString.StartsWith("%"))
            {
                return fsResult.Fail("DynamicInt key needs to be like '%key' .");
            }
            num.isDynamic = true;
            char[] trimChars = new char[] { '%' };
            num.dynamicKey = asString.TrimStart(trimChars);
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            DynamicInt num = instance as DynamicInt;
            if (num == null)
            {
                serialized = new fsData();
                return fsResult.Fail("Failed to convert field to DynamicInt on serialization");
            }
            serialized = !num.isDynamic ? new fsData((long) num.fixedValue) : new fsData("%" + num.dynamicKey);
            return fsResult.Success;
        }
    }
}

