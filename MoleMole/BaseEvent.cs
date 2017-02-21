namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public class BaseEvent
    {
        public int fromPeerID;
        [NonSerialized]
        public BaseEvent parent;
        public EventRemoteState remoteState;
        public bool requireHandle;
        public bool requireResolve;
        public uint targetID;

        public BaseEvent()
        {
        }

        protected BaseEvent(uint targetID)
        {
            this.targetID = targetID;
        }

        protected BaseEvent(uint targetID, bool requireHandle, bool requireResolve)
        {
            this.targetID = targetID;
            this.requireHandle = requireHandle;
            this.requireResolve = requireResolve;
        }

        protected string GetDebugName(object obj)
        {
            if (obj == null)
            {
                return "<null>";
            }
            return obj.ToString();
        }

        protected string GetDebugName(uint runtimeID)
        {
            BaseActor actor = Singleton<EventManager>.Instance.GetActor(runtimeID);
            if (actor != null)
            {
                return string.Format("<{0}({1:x})>", this.Truncate(actor.gameObject.name), actor.runtimeID);
            }
            return "<unknown!>";
        }

        public virtual void Resolve()
        {
            this.resolved = true;
        }

        public override string ToString()
        {
            return ((this.parent != null) ? string.Format("{0} <- ({1})", base.ToString(), this.parent.ToString()) : base.ToString());
        }

        private string Truncate(string str)
        {
            return ((str.Length <= 10) ? str : str.Substring(0, 10));
        }

        public bool resolved { get; private set; }
    }
}

