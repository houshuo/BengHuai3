namespace MoleMole
{
    using FullSerializer;
    using System;
    using System.Runtime.InteropServices;

    public class DynamicStringConverter : fsConverter
    {
        public override bool CanProcess(System.Type type)
        {
            return (type == typeof(DynamicString));
        }

        public override object CreateInstance(fsData data, System.Type storageType)
        {
            return new DynamicString();
        }

        public override bool RequestInheritanceSupport(System.Type storageType)
        {
            return false;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, System.Type storageType)
        {
            DynamicString str = (DynamicString) instance;
            if (!data.IsString)
            {
                return fsResult.Fail("DynamicString fields needs to be either a '%key' or a string value.");
            }
            string asString = data.AsString;
            if (asString.StartsWith("%"))
            {
                str.isDynamic = true;
                char[] trimChars = new char[] { '%' };
                str.dynamicKey = asString.TrimStart(trimChars);
            }
            else
            {
                str.isDynamic = false;
                str.fixedValue = asString;
            }
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, System.Type storageType)
        {
            DynamicString str = instance as DynamicString;
            if (str == null)
            {
                serialized = new fsData();
                return fsResult.Fail("Failed to convert field to DynamicInt on serialization");
            }
            serialized = !str.isDynamic ? new fsData(str.fixedValue) : new fsData("%" + str.dynamicKey);
            return fsResult.Success;
        }
    }
}

