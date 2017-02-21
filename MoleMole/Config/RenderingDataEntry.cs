namespace MoleMole.Config
{
    using System;

    public class RenderingDataEntry
    {
        public string dataPath;
        public string name;
        private const string RENDERING_CONFIG_PATH_PREFIX = "Rendering/";

        public string GetDataPath()
        {
            return ("Rendering/" + this.dataPath);
        }
    }
}

