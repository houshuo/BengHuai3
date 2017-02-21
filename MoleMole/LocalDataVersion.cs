namespace MoleMole
{
    using System;

    public class LocalDataVersion
    {
        public static string version;

        public static void LoadFromFile()
        {
            version = ConfigUtil.LoadJSONConfig<ConfigUserLocalDataVersion>("Data/_BothLocalAndAssetBundle/LocalDataVersion").UserLocalDataVersion;
        }
    }
}

