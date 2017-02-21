namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginDisableOnSmallScale : BaseMonoEffectPlugin
    {
        private ParticleSystemRenderer[] _allRenderers;
        private bool _disabled;

        protected override void Awake()
        {
            base.Awake();
            this._allRenderers = base.GetComponentsInChildren<ParticleSystemRenderer>();
            this._disabled = false;
            base._effect.disableGORecursively = false;
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public override void SetDestroy()
        {
        }

        private void Update()
        {
            if (!this._disabled)
            {
                if (base.transform.lossyScale.x < 0.2f)
                {
                    for (int i = 0; i < this._allRenderers.Length; i++)
                    {
                        this._allRenderers[i].enabled = false;
                    }
                    this._disabled = true;
                }
            }
            else if (base.transform.lossyScale.x > 0.2f)
            {
                for (int j = 0; j < this._allRenderers.Length; j++)
                {
                    this._allRenderers[j].enabled = true;
                }
                this._disabled = false;
            }
        }
    }
}

