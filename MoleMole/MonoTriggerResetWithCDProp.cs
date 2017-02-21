namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class MonoTriggerResetWithCDProp : MonoTriggerProp
    {
        private float _CDTimer;
        public float ResetCD = 0.5f;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            this._CDTimer = this.ResetCD;
        }

        protected override void Update()
        {
            base.Update();
            this._CDTimer -= Time.deltaTime;
            if (this._CDTimer <= 0f)
            {
                base._insideColliders.Clear();
                base._triggerCollider.enabled = false;
                base._triggerCollider.enabled = true;
                this._CDTimer = this.ResetCD;
            }
        }
    }
}

