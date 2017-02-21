namespace MoleMole
{
    using System;

    public class EvtQTEFire : BaseEvent
    {
        public string QTEName;

        public EvtQTEFire(uint targetID, string qteName) : base(targetID)
        {
            this.QTEName = qteName;
        }

        public override string ToString()
        {
            return string.Format("{0} qte fired {1}", base.GetDebugName(base.targetID), this.QTEName);
        }
    }
}

