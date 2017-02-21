namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(MonoEffect))]
    public class MonoEffectPluginMoveToTarget : BaseMonoEffectPlugin
    {
        private bool _active;
        private Transform _targetTransform;
        [Header("Emit By Distance Particle System MUST be put into a child and set here")]
        public GameObject ActivateOnStart;
        [Header("Destroy immediately upon arrival")]
        public bool DestroyImmediatelyUponArrival;
        [Header("To Attach Point")]
        public string ToAttachPoint = "RootNode";
        [Header("Velocity")]
        public float Velocity = 20f;

        protected override void Awake()
        {
            base.Awake();
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(false);
            }
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void OnDisable()
        {
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(false);
            }
        }

        public override void SetDestroy()
        {
        }

        public void SetMoveToTarget(BaseMonoEntity toEntity)
        {
            this._targetTransform = toEntity.GetAttachPoint(this.ToAttachPoint);
        }

        public override void Setup()
        {
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(true);
            }
            this._active = true;
        }

        private void Update()
        {
            if (this._active)
            {
                if (this._targetTransform == null)
                {
                    base._effect.SetDestroy();
                    this._active = false;
                }
                else
                {
                    Vector3 vector = this._targetTransform.position - base.transform.position;
                    float magnitude = vector.magnitude;
                    float num2 = (this.Velocity * Time.deltaTime) * base._effect.TimeScale;
                    vector.Normalize();
                    base.transform.forward = vector;
                    if (num2 > magnitude)
                    {
                        base.transform.position = this._targetTransform.position;
                        if (this.DestroyImmediatelyUponArrival)
                        {
                            base._effect.SetDestroyImmediately();
                        }
                        else
                        {
                            base._effect.SetDestroy();
                        }
                        this._active = false;
                    }
                    else
                    {
                        Transform transform = base.transform;
                        transform.position += (Vector3) (vector * num2);
                    }
                }
            }
        }
    }
}

