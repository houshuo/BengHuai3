namespace MoleMole
{
    using FullSerializer;
    using System;

    [fsObject(Converter=typeof(DynamicStringConverter))]
    public class DynamicString
    {
        public string dynamicKey;
        public string fixedValue;
        public bool isDynamic;
    }
}

