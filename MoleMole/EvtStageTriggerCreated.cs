namespace MoleMole
{
    using System;

    public class EvtStageTriggerCreated : BaseLevelEvent
    {
        public uint triggerRuntimeID;

        public EvtStageTriggerCreated(uint triggerRuntimeID)
        {
            this.triggerRuntimeID = triggerRuntimeID;
        }

        public override string ToString()
        {
            return string.Format("{0} gets created.", base.GetDebugName(this.triggerRuntimeID));
        }
    }
}

