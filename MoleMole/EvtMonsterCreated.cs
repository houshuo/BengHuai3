namespace MoleMole
{
    using System;

    public class EvtMonsterCreated : BaseLevelEvent
    {
        public uint monsterID;

        public EvtMonsterCreated(uint monsterID)
        {
            this.monsterID = monsterID;
        }

        public override string ToString()
        {
            return string.Format("{0} monster created", base.GetDebugName(this.monsterID));
        }
    }
}

