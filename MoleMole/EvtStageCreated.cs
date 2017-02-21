namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class EvtStageCreated : BaseLevelEvent
    {
        public List<string> avatarSpawnNameList;
        public bool isBorn;
        public Vector3 offset;

        public EvtStageCreated(List<string> avatarSpawnNameList, bool isBorn, Vector3 offset)
        {
            this.avatarSpawnNameList = avatarSpawnNameList;
            this.isBorn = isBorn;
            this.offset = offset;
        }

        public override string ToString()
        {
            return "stage created";
        }
    }
}

