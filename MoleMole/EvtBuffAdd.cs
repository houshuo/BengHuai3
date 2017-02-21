namespace MoleMole
{
    using System;

    public class EvtBuffAdd : BaseEvent
    {
        public AbilityState abilityState;

        public EvtBuffAdd(uint targetID, AbilityState abilityState) : base(targetID)
        {
            this.abilityState = abilityState;
        }

        public override string ToString()
        {
            return string.Format("{0} add buff {1}", base.GetDebugName(base.targetID), this.abilityState);
        }
    }
}

