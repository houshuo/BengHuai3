namespace MoleMole
{
    using System;

    public class BaseActorPlugin
    {
        public virtual void Core()
        {
        }

        public virtual bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnAdded()
        {
        }

        public virtual bool OnEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual bool OnPostEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void OnRemoved()
        {
        }

        public virtual bool OnResolvedEvent(BaseEvent evt)
        {
            return false;
        }
    }
}

