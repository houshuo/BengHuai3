namespace MoleMole
{
    using System;

    public class EvtStageReady : BaseLevelEvent
    {
        public bool isBorn;

        public override string ToString()
        {
            return "stage ready";
        }
    }
}

