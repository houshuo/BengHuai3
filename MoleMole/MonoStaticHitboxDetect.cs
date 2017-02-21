namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class MonoStaticHitboxDetect : MonoBehaviour
    {
        private LayerMask _collisionMask;
        private HashSet<uint> _enteredIDs;
        private Transform _followTarget;
        private Rigidbody _rigidbody;
        [Header("Hitpoint will be on the line of this and other collided transform")]
        public Transform collideCenterTransform;
        [NonSerialized]
        public BaseMonoEntity owner;
        public Action<Collider> triggerEnterCallback;
        [Header("Use owner center for retreat direction")]
        public bool useOwnerCenterForRetreatDirection;

        private void Awake()
        {
            this._enteredIDs = new HashSet<uint>();
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._rigidbody.detectCollisions = false;
        }

        public void EnableHitBoxDetect(bool enable)
        {
            this._rigidbody.detectCollisions = enable;
            if (!enable)
            {
                this._enteredIDs.Clear();
            }
        }

        public void Init(BaseMonoEntity owner, LayerMask mask, Transform followTarget)
        {
            this.owner = owner;
            this._collisionMask = mask;
            this._followTarget = (followTarget == null) ? owner.transform : followTarget;
            MonoEffect componentInChildren = base.GetComponentInChildren<MonoEffect>();
            if (componentInChildren != null)
            {
                componentInChildren.SetOwner(owner);
            }
        }

        private void LateUpdate()
        {
            if ((this.owner != null) && this.owner.IsActive())
            {
                base.transform.position = this._followTarget.transform.position;
                base.transform.rotation = this._followTarget.rotation;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if (componentInParent.IsActive() && !this._enteredIDs.Contains(componentInParent.GetRuntimeID()))
                {
                    if (componentInParent is MonoDummyDynamicObject)
                    {
                        BaseMonoDynamicObject obj2 = (BaseMonoDynamicObject) componentInParent;
                        if ((obj2.dynamicType == BaseMonoDynamicObject.DynamicType.EvadeDummy) && (obj2.owner != null))
                        {
                            this._enteredIDs.Add(obj2.owner.GetRuntimeID());
                        }
                    }
                    this._enteredIDs.Add(componentInParent.GetRuntimeID());
                    if (this.triggerEnterCallback != null)
                    {
                        this.triggerEnterCallback(other);
                    }
                }
            }
        }

        public void ResetColliderScale()
        {
            base.transform.localScale = Vector3.one;
        }

        public void ResetTriggerWithoutResetInside()
        {
            this._enteredIDs.Clear();
        }

        public void ResetTriggerWithResetInside()
        {
            this._enteredIDs.Clear();
            this._rigidbody.detectCollisions = false;
            this._rigidbody.detectCollisions = true;
        }

        public void SetColliderScale(float sizeRatio, float lengthRatio)
        {
            base.transform.localScale = new Vector3(sizeRatio, sizeRatio, lengthRatio);
        }
    }
}

