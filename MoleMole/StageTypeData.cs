namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Runtime.CompilerServices;

    public sealed class StageTypeData
    {
        public StageTypeData(JSONNode aJson)
        {
            this.TypeID = (uint) aJson["TypeID"].AsInt;
            this.TypeName = aJson["TypeName"].Value;
            this.PerpStagePrefabPath = aJson["PerpStagePrefabPath"].Value;
            this.OrthStagePrefabPath = aJson["OrthStagePrefabPath"].Value;
        }

        public string OrthStagePrefabPath { get; private set; }

        public string PerpStagePrefabPath { get; private set; }

        public uint TypeID { get; private set; }

        public string TypeName { get; private set; }
    }
}

