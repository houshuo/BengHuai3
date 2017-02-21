namespace MoleMole.Config
{
    using FullInspector;
    using FullSerializer;
    using System;
    using System.Collections.Generic;

    [fsObject(Converter=typeof(ConfigOverrideGroupConverter))]
    public class ConfigOverrideGroup
    {
        public object Default;
        [InspectorNullable]
        public Dictionary<string, object> Overrides;

        public T GetConfig<T>(string name)
        {
            object obj2;
            if (name == "Default")
            {
                obj2 = this.Default;
            }
            else if (this.Overrides == null)
            {
                obj2 = this.Default;
            }
            else
            {
                this.Overrides.TryGetValue(name, out obj2);
                if (obj2 == null)
                {
                    obj2 = this.Default;
                }
            }
            return (T) obj2;
        }
    }
}

