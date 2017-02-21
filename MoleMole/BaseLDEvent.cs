namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class BaseLDEvent : IDisposable
    {
        protected BaseLDEvent()
        {
        }

        public virtual void Core()
        {
        }

        public virtual void Dispose()
        {
        }

        public void Done()
        {
            this.isDone = true;
        }

        public virtual void OnEvent(BaseEvent evt)
        {
        }

        public bool isDone { get; private set; }
    }
}

