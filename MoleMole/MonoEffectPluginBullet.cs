namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginBullet : BaseMonoEffectPlugin
    {
        private RaycastHit _castHit;
        private float _travelDistance;
        [Header("Follow rotation")]
        public bool followRotation;
        [Header("Collision Layer Mask")]
        public LayerMask mask = ((((int) 1) << InLevelData.MONSTER_HITBOX_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER));
        [Header("Max Distance")]
        public float maxDistance;
        [Header("Empty path means start at the most outer transform")]
        public string startTargetPath;
        [Header("Effect Velocity")]
        public float velocity;

        protected override void Awake()
        {
            base.Awake();
        }

        public override bool IsToBeRemove()
        {
            return (this._travelDistance <= 0f);
        }

        public override void SetDestroy()
        {
            this._travelDistance = 0f;
        }

        public override void Setup()
        {
            this._travelDistance = this.maxDistance;
            if (Physics.Raycast(base.transform.position, base.transform.forward, out this._castHit, this.maxDistance, (int) this.mask))
            {
                this._travelDistance = this._castHit.distance;
            }
        }

        public void SetupStartParentTarget(Transform parent)
        {
            Transform transform = parent.Find(this.startTargetPath);
            base.transform.position = transform.position + base.transform.TransformDirection(base._effect.OffsetVec3);
            if (this.followRotation)
            {
                base.transform.rotation = transform.rotation;
            }
        }

        private void Update()
        {
            float z = this.velocity * base._effect.TimeScale;
            base.transform.Translate(new Vector3(0f, 0f, z), Space.Self);
            this._travelDistance -= z;
        }
    }
}

