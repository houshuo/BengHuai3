namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class MonoTriggerField : BaseMonoDynamicObject
    {
        private LayerMask _collisionMask;
        private Collider[] _frameExitColliders;
        private int _frameExitIx;
        public List<Tuple<Collider, uint>> _insideColliders;
        private bool _isToBeRemoved;
        private const int FRAME_EXIT_COLLIDER_COUNT = 5;
        [NonSerialized]
        public Collider triggerCollider;

        protected void Awake()
        {
            this.triggerCollider = base.GetComponent<Collider>();
            this._collisionMask = -1;
            this._insideColliders = new List<Tuple<Collider, uint>>();
            this._frameExitColliders = new Collider[5];
        }

        public override bool IsActive()
        {
            return this.triggerCollider.enabled;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < this._frameExitIx; i++)
            {
                if (this._frameExitColliders[i] != null)
                {
                    this.OnEffectiveTriggerExit(this._frameExitColliders[i]);
                }
            }
            this._frameExitIx = 0;
        }

        private void OnEffectiveTriggerEnter(Collider other)
        {
            if (other != null)
            {
                for (int i = 0; i < this._insideColliders.Count; i++)
                {
                    if (((this._insideColliders[i] != null) && (this._insideColliders[i].Item1 != null)) && (this._insideColliders[i].Item1.gameObject == other.gameObject))
                    {
                        return;
                    }
                }
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if ((componentInParent != null) && componentInParent.IsActive())
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtFieldEnter(base._runtimeID, componentInParent.GetRuntimeID()), MPEventDispatchMode.Normal);
                    int num2 = this._insideColliders.SeekAddPosition<Tuple<Collider, uint>>();
                    this._insideColliders[num2] = Tuple.Create<Collider, uint>(other, componentInParent.GetRuntimeID());
                }
            }
        }

        private void OnEffectiveTriggerExit(Collider other)
        {
            if (other != null)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if (componentInParent != null)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(base._runtimeID, componentInParent.GetRuntimeID()), MPEventDispatchMode.Normal);
                    int num = -1;
                    for (int i = 0; i < this._insideColliders.Count; i++)
                    {
                        if (((this._insideColliders[i] != null) && (this._insideColliders[i].Item1 != null)) && (this._insideColliders[i].Item1.gameObject == other.gameObject))
                        {
                            num = i;
                        }
                    }
                    if (num >= 0)
                    {
                        this._insideColliders[num] = null;
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((other != null) && (other.gameObject != null)) && ((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0))
            {
                if (this._frameExitIx > 5)
                {
                    this.OnEffectiveTriggerEnter(other);
                }
                else
                {
                    for (int i = 0; i < this._frameExitIx; i++)
                    {
                        if ((this._frameExitColliders[i] != null) && (this._frameExitColliders[i] == other))
                        {
                            this._frameExitColliders[i] = null;
                            return;
                        }
                    }
                    this.OnEffectiveTriggerEnter(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0)
            {
                if (this._frameExitIx >= 5)
                {
                    this.OnEffectiveTriggerExit(other);
                }
                else
                {
                    this._frameExitColliders[this._frameExitIx] = other;
                    this._frameExitIx++;
                }
            }
        }

        public void SetCollisionMask(LayerMask mask)
        {
            this._collisionMask = mask;
        }

        public override void SetDied()
        {
            base.SetDied();
            this._isToBeRemoved = true;
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
        }

        protected override void Update()
        {
            base.Update();
            for (int i = 0; i < this._insideColliders.Count; i++)
            {
                if (this._insideColliders[i] != null)
                {
                    Collider collider = this._insideColliders[i].Item1;
                    if (((collider == null) || !collider.enabled) || !collider.gameObject.activeInHierarchy)
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(base._runtimeID, this._insideColliders[i].Item2), MPEventDispatchMode.Normal);
                        this._insideColliders[i] = null;
                    }
                }
            }
        }
    }
}

