namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class ProtectedShieldAnimation : MonoBehaviour
    {
        public float _hitAnmTimer;
        private MaterialPropertyBlock _mpb;
        private Renderer _renderer;
        public float hitAnmEndDuration = 0.2f;
        public float hitAnmStartDuration = 0.2f;
        public Vector3 hitPosition;

        private void Awake()
        {
            this._renderer = base.GetComponent<Renderer>();
            this._mpb = new MaterialPropertyBlock();
            this._renderer.GetPropertyBlock(this._mpb);
        }

        public void PlayHitAnimation()
        {
        }

        private void Update()
        {
            this.UpdateHitAnimation();
            this._renderer.SetPropertyBlock(this._mpb);
        }

        private void UpdateHitAnimation()
        {
            if (this._hitAnmTimer <= (this.hitAnmStartDuration + this.hitAnmEndDuration))
            {
                this._hitAnmTimer += Time.deltaTime;
                this._mpb.SetVector("_HitPosition", this.hitPosition);
                if (this._hitAnmTimer > this.hitAnmStartDuration)
                {
                    float num = Mathf.Clamp01((this._hitAnmTimer - this.hitAnmStartDuration) / this.hitAnmEndDuration);
                    this._mpb.SetFloat("_HitAnmStartTime", 1f);
                    this._mpb.SetFloat("_HitAnmEndTime", num);
                }
                else
                {
                    float num2 = Mathf.Clamp01(this._hitAnmTimer / this.hitAnmStartDuration);
                    this._mpb.SetFloat("_HitAnmStartTime", num2);
                    this._mpb.SetFloat("_HitAnmEndTime", 0f);
                }
            }
        }
    }
}

