namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class BaseMonoEntity : MonoBehaviour
    {
        protected uint _runtimeID;

        protected BaseMonoEntity()
        {
        }

        public abstract Transform GetAttachPoint(string name);
        public uint GetRuntimeID()
        {
            return this._runtimeID;
        }

        public abstract bool IsActive();
        public abstract bool IsToBeRemove();
        protected virtual void OnDestroy()
        {
        }

        public abstract float TimeScale { get; }

        public abstract Vector3 XZPosition { get; }
    }
}

