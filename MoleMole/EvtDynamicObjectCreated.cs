namespace MoleMole
{
    using System;

    public class EvtDynamicObjectCreated : BaseEvent
    {
        public BaseMonoDynamicObject.DynamicType dynamicType;
        public readonly uint objectID;

        public EvtDynamicObjectCreated(uint ownerID, uint objectID, BaseMonoDynamicObject.DynamicType dynamicType) : base(ownerID)
        {
            this.objectID = objectID;
            this.dynamicType = dynamicType;
        }

        public override string ToString()
        {
            return string.Format("{0} created {1}", base.GetDebugName(base.targetID), base.GetDebugName(this.objectID));
        }
    }
}

