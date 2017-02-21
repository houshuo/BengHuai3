namespace MoleMole
{
    using System;

    public class LDEvtOnSpecificPropObjectDestroyed : BaseLDEvent
    {
        private PropObjectActor _propObjectActor;

        public LDEvtOnSpecificPropObjectDestroyed(double runtimeID)
        {
            this._propObjectActor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>((uint) runtimeID);
        }

        public override void OnEvent(BaseEvent evt)
        {
            if ((evt is EvtKilled) && (evt.targetID == this._propObjectActor.runtimeID))
            {
                base.Done();
            }
        }
    }
}

