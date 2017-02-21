namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class BaseActor
    {
        public GameObject gameObject;
        public uint ownerID = 0x21800001;
        public uint runtimeID;

        protected BaseActor()
        {
        }

        public virtual void Core()
        {
        }

        public abstract void Init(BaseMonoEntity entity);
        public virtual bool IsActive()
        {
            return ((this.gameObject != null) && this.gameObject.activeSelf);
        }

        public bool IsEntityExists()
        {
            return (this.gameObject != null);
        }

        public virtual bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        protected void MarkImportantEventIsHandled(BaseEvent evt)
        {
            if (Singleton<MPEventManager>.Instance != null)
            {
                Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
            }
        }

        public virtual bool OnEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnRemoval()
        {
        }
    }
}

