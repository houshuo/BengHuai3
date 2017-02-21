namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class MonoTriggerProp : BaseMonoPropObject
    {
        private LayerMask _collisionMask;
        private Collider[] _frameExitColliders;
        private int _frameExitIx;
        public List<Tuple<Collider, uint>> _insideColliders;
        protected Collider _triggerCollider;
        private const int FRAME_EXIT_COLLIDER_COUNT = 5;

        protected override void Awake()
        {
            base.Awake();
            this._triggerCollider = base.GetComponent<Collider>();
            this._collisionMask = -1;
            this._insideColliders = new List<Tuple<Collider, uint>>();
            this._frameExitColliders = new Collider[5];
        }

        protected void ClearInsideColliders()
        {
            this._insideColliders.Clear();
            Singleton<EventManager>.Instance.FireEvent(new EvtFieldClear(base._runtimeID), MPEventDispatchMode.Normal);
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            if (base.config.PropArguments.CanAffectMonsters)
            {
                this.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base._runtimeID, MixinTargetting.All));
            }
            else
            {
                this.SetCollisionMask(Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(runtimeID, MixinTargetting.Enemy));
            }
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

        protected override void OnDurationTimeOut()
        {
            for (int i = 0; i < this._insideColliders.Count; i++)
            {
                if ((this._insideColliders[i] != null) && (this._insideColliders[i].Item1 != null))
                {
                    BaseMonoEntity componentInParent = this._insideColliders[i].Item1.GetComponentInParent<BaseMonoEntity>();
                    if (componentInParent != null)
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(base._runtimeID, componentInParent.GetRuntimeID()), MPEventDispatchMode.Normal);
                    }
                }
            }
            this._insideColliders.Clear();
            base.OnDurationTimeOut();
        }

        protected virtual void OnEffectiveTriggerEnter(Collider other)
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

        protected virtual void OnEffectiveTriggerExit(Collider other)
        {
            if (other != null)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if (componentInParent != null)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtFieldExit(base._runtimeID, componentInParent.GetRuntimeID()), MPEventDispatchMode.Normal);
                }
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

        private void OnTriggerEnter(Collider other)
        {
            if ((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0)
            {
                if (this._frameExitIx > 5)
                {
                    this.OnEffectiveTriggerEnter(other);
                }
                else
                {
                    for (int i = 0; i < this._frameExitIx; i++)
                    {
                        if (this._frameExitColliders[i] == other)
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

        public override void SetDied(KillEffect killEffect)
        {
            base.SetDied(killEffect);
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
            if ((base.config.PropArguments.OnKillEffectPattern != null) && base.gameObject.activeSelf)
            {
                this.FireEffect(base.config.PropArguments.OnKillEffectPattern);
            }
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

