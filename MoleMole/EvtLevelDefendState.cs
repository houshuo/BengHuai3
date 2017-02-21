namespace MoleMole
{
    using System;

    public class EvtLevelDefendState : BaseLevelEvent
    {
        public readonly DefendModeType modeType;
        public readonly int targetValue;
        public readonly int uniqueID;

        public EvtLevelDefendState(int uniqueID)
        {
            this.modeType = DefendModeType.Certain;
            this.targetValue = 0;
            this.uniqueID = uniqueID;
        }

        public EvtLevelDefendState(DefendModeType defendModeType, int targetValue)
        {
            this.modeType = defendModeType;
            this.targetValue = targetValue;
            this.uniqueID = 0;
        }

        public override string ToString()
        {
            if ((this.modeType == DefendModeType.Single) || (this.modeType == DefendModeType.Group))
            {
                return string.Format("DefendMode : {0}. Target Value : {1}", this.modeType.ToString(), this.targetValue);
            }
            if (this.modeType == DefendModeType.Certain)
            {
                return string.Format("DefendMode : {0}. Target monster tag id : {1}", this.modeType.ToString(), this.uniqueID.ToString());
            }
            return string.Format("DefendMode : {0}. Defend Target Value : {1}", this.modeType.ToString(), this.targetValue);
        }
    }
}

