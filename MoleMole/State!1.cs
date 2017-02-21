namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class State<T>
    {
        protected T _owner;

        public State(T t)
        {
            this._owner = t;
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }

        public void SetActive(bool isActive)
        {
            this.active = isActive;
        }

        public virtual void Update()
        {
        }

        public bool active { get; private set; }
    }
}

