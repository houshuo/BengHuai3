namespace MoleMole.Config
{
    using FullSerializer;
    using System;

    [fsObject(Converter=typeof(DynamicFloatConverter))]
    public class DynamicFloat
    {
        public string dynamicKey;
        public float fixedValue;
        public bool isDynamic;
        public static DynamicFloat ONE;
        public static DynamicFloat ZERO;

        static DynamicFloat()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 0f
            };
            ZERO = num;
            num = new DynamicFloat {
                fixedValue = 1f
            };
            ONE = num;
        }
    }
}

