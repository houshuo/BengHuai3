namespace MoleMole
{
    using System;

    public class EvtLevelBuffState : BaseLevelEvent
    {
        public LevelBuffType levelBuff;
        public LevelBuffSide side;
        public uint sourceId;
        public LevelBuffState state;

        public EvtLevelBuffState(LevelBuffType levelBuff, LevelBuffState state, LevelBuffSide side, uint ownerRuntimeID)
        {
            this.levelBuff = levelBuff;
            this.state = state;
            this.side = side;
            this.sourceId = ownerRuntimeID;
        }

        public override string ToString()
        {
            object[] args = new object[] { this.levelBuff, this.state, this.side, this.sourceId };
            return string.Format("LevelBuff <{0}> state: <{1}>, side <{2}> sourceId <{3}>", args);
        }
    }
}

