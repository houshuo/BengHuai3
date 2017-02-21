namespace MoleMole
{
    using System;

    public class LDEvtWaitLocalAvatarEnterField : BaseLDEvent
    {
        private TriggerFieldActor _fieldActor;

        public LDEvtWaitLocalAvatarEnterField(double runtimeID)
        {
            this._fieldActor = Singleton<EventManager>.Instance.GetActor<TriggerFieldActor>((uint) runtimeID);
        }

        public override void OnEvent(BaseEvent evt)
        {
            if (((evt is EvtFieldEnter) && (this._fieldActor != null)) && (this._fieldActor.runtimeID == evt.targetID))
            {
                EvtFieldEnter enter = (EvtFieldEnter) evt;
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(enter.otherID))
                {
                    base.Done();
                }
            }
        }
    }
}

