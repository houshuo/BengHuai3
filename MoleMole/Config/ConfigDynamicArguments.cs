namespace MoleMole.Config
{
    using FullSerializer;
    using MoleMole;
    using System;
    using System.Collections.Generic;

    [GeneratePartialHash, fsObject(Converter=typeof(ConfigDynamicArgumentsConverter))]
    public class ConfigDynamicArguments : Dictionary<string, object>, IHashable
    {
        public static ConfigDynamicArguments EMPTY = new ConfigDynamicArguments();

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this != null)
            {
                foreach (KeyValuePair<string, object> pair in this)
                {
                    HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                    HashUtils.ContentHashOntoFallback(pair.Value, ref lastHash);
                }
            }
        }
    }
}

