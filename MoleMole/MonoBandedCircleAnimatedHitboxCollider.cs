namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class MonoBandedCircleAnimatedHitboxCollider : MonoAnimatedHitboxDetect
    {
        private List<Tuple<BaseMonoEntity, Collider>> _offColliders;
        private RaycastHit _rayHit;
        [Header("Off distance ratio, 0 means a full circle, 1 means no collision, this can be animated")]
        public float offDistanceRatio = 0.8f;
        private static Vector3 UNIT_FIELD_CENTER = new Vector3(0f, 1f, 0f);
        [Header("Unit Field Mesh Collider, must use 'UnitField''s mesh or it won't work.")]
        public MeshCollider unitFieldMeshCollider;

        protected override void Awake()
        {
            base.Awake();
            this._offColliders = new List<Tuple<BaseMonoEntity, Collider>>();
        }

        public override Vector3 CalculateFixedRetreatDirection(Vector3 hitPoint)
        {
            Vector3 vector = hitPoint - base.collideCenterTransform.position;
            vector = (Vector3) (vector * this.fixedRetreatDirection.z);
            vector.y = 0f;
            return vector;
        }

        protected override void FireTriggerCallback(Collider other, BaseMonoEntity entity)
        {
            if (((other != null) && (entity != null)) && !this.IsColliderOff(other))
            {
                base.FireTriggerCallback(other, entity);
            }
        }

        public bool IsColliderOff(Collider other)
        {
            Vector3 origin = this.unitFieldMeshCollider.gameObject.transform.TransformPoint(UNIT_FIELD_CENTER);
            Vector3 center = other.bounds.center;
            origin.y = center.y;
            Vector3 vector = Vector3.Normalize(center - origin);
            Vector3 vector4 = this.unitFieldMeshCollider.gameObject.transform.TransformVector(vector);
            Ray ray = new Ray(origin, vector);
            return (other.Raycast(ray, out this._rayHit, float.MaxValue) && (this._rayHit.distance < (vector4.magnitude * this.offDistanceRatio)));
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            for (int i = 0; i < this._offColliders.Count; i++)
            {
                if (this._offColliders[i] != null)
                {
                    if ((this._offColliders[i].Item1 == null) || (this._offColliders[i].Item2 == null))
                    {
                        this._offColliders[i] = null;
                    }
                    else if (!this.IsColliderOff(this._offColliders[i].Item2) && base._enteredIDs.Contains(this._offColliders[i].Item1.GetRuntimeID()))
                    {
                        base.FireTriggerCallback(this._offColliders[i].Item2, this._offColliders[i].Item1);
                        this._offColliders[i] = null;
                    }
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            foreach (Collider collider in base.GetComponentsInChildren<Collider>())
            {
                if (collider.enabled)
                {
                    Matrix4x4 matrix = Gizmos.matrix;
                    Gizmos.matrix = collider.transform.localToWorldMatrix;
                    Gizmos.color = Color.blue;
                    if (collider == this.unitFieldMeshCollider)
                    {
                        Gizmos.DrawWireMesh(this.unitFieldMeshCollider.sharedMesh);
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireMesh(this.unitFieldMeshCollider.sharedMesh, Vector3.zero, Quaternion.identity, new Vector3(this.offDistanceRatio, 1f, this.offDistanceRatio));
                    }
                    else if (collider is BoxCollider)
                    {
                        BoxCollider collider2 = (BoxCollider) collider;
                        Gizmos.DrawWireCube(collider2.center, collider2.size);
                    }
                    else if (collider is MeshCollider)
                    {
                        MeshCollider collider3 = (MeshCollider) collider;
                        Gizmos.DrawWireMesh(collider3.sharedMesh);
                    }
                    else if (collider is CapsuleCollider)
                    {
                        CapsuleCollider collider4 = (CapsuleCollider) collider;
                        Gizmos.DrawWireSphere(collider4.center, collider4.radius);
                    }
                    Gizmos.matrix = matrix;
                }
            }
        }

        protected override void OnEnteredReset()
        {
            this._offColliders.Clear();
        }

        protected override void OnEntityEntered(Collider other, BaseMonoEntity entity)
        {
            if (this.IsColliderOff(other))
            {
                int num = this._offColliders.SeekAddPosition<Tuple<BaseMonoEntity, Collider>>();
                this._offColliders[num] = Tuple.Create<BaseMonoEntity, Collider>(entity, other);
            }
        }
    }
}

