namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;

    public class ActorTriggerFieldPlugin : BaseActorPlugin
    {
        protected BasePluggedActor _actor;
        [ShowInInspector]
        public List<uint> insideIDs = new List<uint>();

        public ActorTriggerFieldPlugin(BasePluggedActor owner)
        {
            this._actor = owner;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (evt is EvtFieldEnter)
            {
                return this.OnFieldEnter((EvtFieldEnter) evt);
            }
            if (evt is EvtFieldExit)
            {
                return this.OnFieldExit((EvtFieldExit) evt);
            }
            return ((evt is EvtFieldClear) && this.OnFieldClear((EvtFieldClear) evt));
        }

        private bool OnFieldClear(EvtFieldClear evt)
        {
            this.insideIDs.Clear();
            return true;
        }

        private bool OnFieldEnter(EvtFieldEnter evt)
        {
            if (!this.insideIDs.Contains(evt.otherID))
            {
                this.insideIDs.Add(evt.otherID);
            }
            return true;
        }

        private bool OnFieldExit(EvtFieldExit evt)
        {
            if (this.insideIDs.Contains(evt.otherID))
            {
                this.insideIDs.Remove(evt.otherID);
            }
            return true;
        }
    }
}

