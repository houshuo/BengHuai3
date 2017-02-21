namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginDisableOnPropertyBlock : BaseMonoEffectPlugin
    {
        private ParticleSystemRenderer[] _allRenderers;
        private MaterialPropertyBlock _block;
        private bool _disabled;
        private int _targetPropertyHash;
        private Renderer _targetRenderer;
        [Header("Target Property Name")]
        public string PropertyName;
        [Header("Target Renderer Game Object Name")]
        public string RendererGOName;
        [Header("Reverse Threshold Comparision")]
        public bool Reverse;
        [Header("Float Threshold Value")]
        public float Threshold;

        protected override void Awake()
        {
            base.Awake();
            this._allRenderers = base.GetComponentsInChildren<ParticleSystemRenderer>();
            this._disabled = false;
            this._block = new MaterialPropertyBlock();
            base._effect.disableGORecursively = false;
        }

        private bool CheckThreshold(float value)
        {
            return (!this.Reverse ? (value > this.Threshold) : (value < this.Threshold));
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        private void LateUpdate()
        {
            if (this._targetRenderer != null)
            {
                this._targetRenderer.GetPropertyBlock(this._block);
                float @float = this._block.GetFloat(this._targetPropertyHash);
                if (!this._disabled)
                {
                    if (!this.RendererActive() || this.CheckThreshold(@float))
                    {
                        for (int i = 0; i < this._allRenderers.Length; i++)
                        {
                            this._allRenderers[i].enabled = false;
                        }
                        this._disabled = true;
                    }
                }
                else if (this.RendererActive() && !this.CheckThreshold(@float))
                {
                    for (int j = 0; j < this._allRenderers.Length; j++)
                    {
                        this._allRenderers[j].enabled = true;
                    }
                    this._disabled = false;
                }
            }
        }

        private bool RendererActive()
        {
            return (this._targetRenderer.gameObject.activeInHierarchy && this._targetRenderer.enabled);
        }

        public override void SetDestroy()
        {
        }

        public void SetupRenderer(BaseMonoEntity owner)
        {
            BaseMonoAnimatorEntity entity = (BaseMonoAnimatorEntity) owner;
            this._targetRenderer = null;
            for (int i = 0; i < entity.renderers.Length; i++)
            {
                Renderer renderer = entity.renderers[i];
                if (renderer.gameObject.name == this.RendererGOName)
                {
                    this._targetRenderer = renderer;
                }
            }
            this._targetPropertyHash = Shader.PropertyToID(this.PropertyName);
        }
    }
}

