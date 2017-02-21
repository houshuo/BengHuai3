namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;

    public class TriggerFieldActor : BaseActor
    {
        [ShowInInspector]
        public List<uint> _insideRuntimes = new List<uint>();

        public override void Init(BaseMonoEntity entity)
        {
            base.runtimeID = entity.GetRuntimeID();
        }

        public virtual void Kill()
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID), MPEventDispatchMode.Normal);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (evt is EvtFieldEnter)
            {
                return this.OnFieldEnter((EvtFieldEnter) evt);
            }
            return ((evt is EvtFieldExit) && this.OnFieldExit((EvtFieldExit) evt));
        }

        public virtual bool OnFieldEnter(EvtFieldEnter evt)
        {
            this._insideRuntimes.Add(evt.otherID);
            return true;
        }

        public virtual bool OnFieldExit(EvtFieldExit evt)
        {
            this._insideRuntimes.Remove(evt.otherID);
            return true;
        }
    }
}

