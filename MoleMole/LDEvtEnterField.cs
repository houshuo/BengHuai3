namespace MoleMole
{
    using System;

    public class LDEvtEnterField : BaseLDEvent
    {
        private TriggerFieldActor _fieldActor;
        private uint _otherID;

        public LDEvtEnterField(double runtimeID, double otherID)
        {
            this._fieldActor = Singleton<EventManager>.Instance.GetActor<TriggerFieldActor>((uint) runtimeID);
            this._otherID = (uint) otherID;
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (((evt is EvtFieldEnter) && (this._fieldActor != null)) && (this._fieldActor.runtimeID == evt.targetID))
            {
                EvtFieldEnter enter = (EvtFieldEnter) evt;
                if (enter.otherID == this._otherID)
                {
                    base.Done();
                }
            }
        }
    }
}

