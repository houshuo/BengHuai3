namespace MoleMole.Config
{
    using System;

    public class AuxObjectEntry
    {
        private const string AUX_PREFAB_PATH_PREFIX = "AuxObject/";
        public string name;
        public string prefabPath;

        public string GetPrefabPath()
        {
            return ("AuxObject/" + this.prefabPath);
        }
    }
}

