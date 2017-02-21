namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(MonoEffect))]
    public abstract class BaseMonoEffectPlugin : MonoBehaviour
    {
        protected MonoEffect _effect;

        protected BaseMonoEffectPlugin()
        {
        }

        protected virtual void Awake()
        {
            this._effect = base.GetComponent<MonoEffect>();
        }

        public abstract bool IsToBeRemove();
        protected virtual void OnValidate()
        {
            if (base.GetComponentInParent<MonoEffect>() != null)
            {
            }
        }

        public abstract void SetDestroy();
        public virtual void Setup()
        {
        }
    }
}

