namespace MoleMole
{
    using FullSerializer;
    using System;

    [fsObject(Converter=typeof(ConfigOverrideListConverter))]
    public class ConfigOverrideList
    {
        public static ConfigOverrideList EMPTY;
        public object[] objects;

        static ConfigOverrideList()
        {
            ConfigOverrideList list = new ConfigOverrideList {
                objects = new object[0]
            };
            EMPTY = list;
        }

        public T GetConfig<T>(int ix)
        {
            return (T) this.objects[ix];
        }

        public int length
        {
            get
            {
                return this.objects.Length;
            }
        }
    }
}

