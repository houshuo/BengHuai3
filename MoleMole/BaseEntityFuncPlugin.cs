namespace MoleMole
{
    using System;

    public abstract class BaseEntityFuncPlugin
    {
        protected BaseMonoEntity _entity;

        public BaseEntityFuncPlugin(BaseMonoEntity entity)
        {
            this._entity = entity;
        }

        public abstract void Core();
        public abstract void FixedCore();
        public abstract bool IsActive();
    }
}

