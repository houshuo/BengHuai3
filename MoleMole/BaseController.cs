namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class BaseController
    {
        public BaseController(uint controllerType, BaseMonoEntity entity)
        {
            this.ControllerType = controllerType;
        }

        public abstract void Core();

        public uint ControllerType { get; private set; }
    }
}

