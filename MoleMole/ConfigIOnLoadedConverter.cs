namespace MoleMole
{
    using FullSerializer;
    using System;

    internal class ConfigIOnLoadedConverter : fsObjectProcessor
    {
        public override bool CanProcess(System.Type type)
        {
            return typeof(IOnLoaded).IsAssignableFrom(type);
        }

        public override void OnAfterDeserialize(System.Type storageType, object instance)
        {
            IOnLoaded loaded = (IOnLoaded) instance;
            if (loaded != null)
            {
                loaded.OnLoaded();
            }
        }
    }
}

