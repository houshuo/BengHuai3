namespace MoleMole
{
    using System;

    public class EvtBuffRemove : BaseEvent
    {
        public AbilityState abilityState;

        public EvtBuffRemove(uint targetID, AbilityState abilityState) : base(targetID)
        {
            this.abilityState = abilityState;
        }

        public override string ToString()
        {
            return string.Format("{0} remove buff {1}", base.GetDebugName(base.targetID), this.abilityState);
        }
    }
}

