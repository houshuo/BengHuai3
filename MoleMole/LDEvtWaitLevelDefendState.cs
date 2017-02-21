namespace MoleMole
{
    using System;

    public class LDEvtWaitLevelDefendState : BaseLDEvent
    {
        public DefendModeType defendModeType;
        public int targetValue;
        public int uniqueID;

        public LDEvtWaitLevelDefendState(string typeStr, double value)
        {
            this.defendModeType = (DefendModeType) ((int) Enum.Parse(typeof(DefendModeType), typeStr));
            if ((this.defendModeType == DefendModeType.Single) || (this.defendModeType == DefendModeType.Group))
            {
                this.targetValue = (int) value;
                this.uniqueID = 0;
            }
            else if (this.defendModeType == DefendModeType.Certain)
            {
                this.targetValue = 0;
                this.uniqueID = (int) value;
            }
            else
            {
                this.targetValue = (int) value;
                this.uniqueID = 0;
            }
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (evt is EvtLevelDefendState)
            {
                EvtLevelDefendState state = evt as EvtLevelDefendState;
                if (state.modeType == this.defendModeType)
                {
                    if (state.modeType == DefendModeType.Certain)
                    {
                        if (this.uniqueID == state.uniqueID)
                        {
                            base.Done();
                        }
                    }
                    else if (state.modeType == DefendModeType.Result)
                    {
                        if (this.targetValue == state.targetValue)
                        {
                            base.Done();
                        }
                    }
                    else if (this.targetValue == state.targetValue)
                    {
                        base.Done();
                    }
                }
            }
        }
    }
}

