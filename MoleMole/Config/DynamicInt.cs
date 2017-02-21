namespace MoleMole.Config
{
    using FullSerializer;
    using System;

    [fsObject(Converter=typeof(DynamicIntConverter))]
    public class DynamicInt
    {
        public string dynamicKey;
        public int fixedValue;
        public bool isDynamic;
        public static DynamicInt ONE;
        public static DynamicInt ZERO;

        static DynamicInt()
        {
            DynamicInt num = new DynamicInt {
                fixedValue = 0
            };
            ZERO = num;
            num = new DynamicInt {
                fixedValue = 1
            };
            ONE = num;
        }
    }
}

