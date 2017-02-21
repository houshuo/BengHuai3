namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    public class EvtAbilityStart : BaseEvent
    {
        public object abilityArgument;
        public string abilityID;
        public string abilityName;
        public AttackResult.HitCollsion hitCollision;
        public uint otherID;
        public BaseEvent TriggerEvent;

        public EvtAbilityStart(uint casterID, BaseEvent triggerEvt = null) : base(casterID)
        {
            if (triggerEvt != null)
            {
                this.TriggerEvent = triggerEvt;
            }
        }

        public EvtAbilityStart(uint casterID, uint otherID, BaseEvent triggerEvt = null) : base(casterID)
        {
            this.otherID = otherID;
            if (triggerEvt != null)
            {
                this.TriggerEvent = triggerEvt;
            }
        }

        public override string ToString()
        {
            object[] args = new object[] { base.GetDebugName(base.targetID), this.abilityID, this.abilityName, base.GetDebugName(this.otherID) };
            return string.Format("{0} triggers special skill ID: <{1}>, name <{2}>, other <{3}>", args);
        }
    }
}

