namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginFastDestroy : BaseMonoEffectPlugin
    {
        private float _timer;
        [Header("Fast Destroy Duration")]
        public float fastDestroyDuration = 0.1f;
        [Header("Remove on set destroy")]
        public bool removeImmediatelyOnSetDestroy;

        public override bool IsToBeRemove()
        {
            return (this._timer < 0f);
        }

        public override void SetDestroy()
        {
            this._timer = !this.removeImmediatelyOnSetDestroy ? this.fastDestroyDuration : -1f;
        }

        public override void Setup()
        {
            this._timer = 0f;
        }

        private void Update()
        {
            if (this._timer > 0f)
            {
                this._timer -= Time.deltaTime * base._effect.TimeScale;
            }
        }
    }
}

