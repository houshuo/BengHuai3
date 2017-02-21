namespace MoleMole.Config
{
    using FullSerializer;
    using System;
    using System.Collections.Generic;

    [fsObject(Converter=typeof(ConfigSharedAnimEventGroupConverter))]
    public class ConfigSharedAnimEventGroup
    {
        public Dictionary<string, ConfigEntityAnimEvent> AnimEvents;
        public string Prefix;
        public string Type = "ConfigEntityAnimEvent";
    }
}

