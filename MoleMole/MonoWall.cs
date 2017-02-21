namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoWall : BaseMonoDynamicObject
    {
        private LayerMask _collisionMask;
        private List<CollisionEntry> _collisions = new List<CollisionEntry>();
        private bool _isToBeRemoved;
        [Header("Collided entity will have this effect pattern on the wall.")]
        public string CollisionEffectPattern;
        public const float FADE_DURATION = 0.5f;

        private void CreateOrRefreshCollisionEntry(Collision collision)
        {
            for (int i = 0; i < this._collisions.Count; i++)
            {
                CollisionEntry entry = this._collisions[i];
                if ((entry != null) && (entry.collider == collision.collider))
                {
                    entry.timer = 0.5f;
                    return;
                }
            }
            int patternIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(this.CollisionEffectPattern, this.GetCollisoinPointOnWall(collision.collider.transform), base.transform.forward, Vector3.one, this);
            List<MonoEffect> indexedEntityEffectPattern = Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(patternIx);
            int num3 = this._collisions.SeekAddPosition<CollisionEntry>();
            CollisionEntry entry2 = new CollisionEntry {
                collider = collision.collider,
                timer = 0.5f,
                patternIx = patternIx,
                effectLs = indexedEntityEffectPattern
            };
            this._collisions[num3] = entry2;
        }

        private Vector3 GetCollisoinPointOnWall(Transform targetTransform)
        {
            Vector3 vector = targetTransform.position - base.transform.position;
            vector.y = 0f;
            Vector3 vector3 = Vector3.Project(vector, base.transform.right);
            Vector3 vector4 = base.transform.position + vector3;
            vector4.y = 0f;
            return vector4;
        }

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((this._collisionMask.value & (((int) 1) << collision.gameObject.layer)) != 0)
            {
                this.CreateOrRefreshCollisionEntry(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if ((this._collisionMask.value & (((int) 1) << collision.gameObject.layer)) != 0)
            {
                for (int i = 0; i < this._collisions.Count; i++)
                {
                    if ((this._collisions[i] != null) && (this._collisions[i].collider == collision.collider))
                    {
                        this._collisions[i].timer = 0f;
                    }
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if ((this._collisionMask.value & (((int) 1) << collision.gameObject.layer)) != 0)
            {
                this.CreateOrRefreshCollisionEntry(collision);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Singleton<EffectManager>.Instance != null)
            {
                for (int i = 0; i < this._collisions.Count; i++)
                {
                    if (this._collisions[i] != null)
                    {
                        Singleton<EffectManager>.Instance.SetDestroyImmediatelyIndexedEffectPattern(this._collisions[i].patternIx);
                    }
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
            for (int i = 0; i < this._collisions.Count; i++)
            {
                CollisionEntry entry = this._collisions[i];
                if (entry != null)
                {
                    entry.timer -= Time.deltaTime * this.TimeScale;
                    if ((entry.collider == null) || (entry.timer <= 0f))
                    {
                        Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(entry.patternIx);
                        this._collisions[i] = null;
                    }
                    else
                    {
                        for (int j = 0; j < entry.effectLs.Count; j++)
                        {
                            entry.effectLs[j].transform.position = this.GetCollisoinPointOnWall(entry.collider.transform);
                            entry.effectLs[j].transform.forward = base.transform.forward;
                        }
                    }
                }
            }
        }

        private class CollisionEntry
        {
            public Collider collider;
            public List<MonoEffect> effectLs;
            public int patternIx;
            public float timer;
        }
    }
}

