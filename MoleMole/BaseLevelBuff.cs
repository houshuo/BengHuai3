namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class BaseLevelBuff : BaseActorPlugin
    {
        public bool isActive;
        [HideInInspector]
        public LevelActor levelActor;
        public LevelBuffSide levelBuffSide;
        public LevelBuffType levelBuffType;
        public bool muteUpdateDuration;
        public uint ownerID;

        protected BaseLevelBuff()
        {
        }
    }
}

