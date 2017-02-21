namespace MoleMole
{
    using System;

    public class LDEvtWaitMonsterEnterField : BaseLDEvent
    {
        private MonsterExitFieldActor _fieldActor;

        public LDEvtWaitMonsterEnterField(double runtimeID)
        {
            this._fieldActor = Singleton<EventManager>.Instance.GetActor<MonsterExitFieldActor>((uint) runtimeID);
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (((evt is EvtFieldEnter) && (this._fieldActor != null)) && (this._fieldActor.runtimeID == evt.targetID))
            {
                EvtFieldEnter enter = (EvtFieldEnter) evt;
                if (Singleton<RuntimeIDManager>.Instance.ParseCategory(enter.otherID) == 4)
                {
                    base.Done();
                }
            }
        }
    }
}

