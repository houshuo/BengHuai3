namespace MoleMole.Config
{
    using FullInspector;
    using System;

    public abstract class BaseActionContainer
    {
        [NonSerialized, ShowInInspector]
        public int localID = -1;

        protected BaseActionContainer()
        {
        }

        public abstract ConfigAbilityAction[][] GetAllSubActions();
    }
}

