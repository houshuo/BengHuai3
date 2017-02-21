namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginSkinMeshShape : BaseMonoEffectPlugin
    {
        private ParticleSystem.ShapeModule _shapeModule;
        [Header("Skin mesh attach point")]
        public string skinMeshAttachPoint;
        [Header("Target particle system")]
        public ParticleSystem targetParticleSystem;

        public override bool IsToBeRemove()
        {
            return false;
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            base.Setup();
            this._shapeModule = this.targetParticleSystem.shape;
        }

        public void SetupSkinmesh(BaseMonoEntity entity)
        {
            SkinnedMeshRenderer component = entity.GetAttachPoint(this.skinMeshAttachPoint).GetComponent<SkinnedMeshRenderer>();
            if (component != null)
            {
                this._shapeModule.skinnedMeshRenderer = component;
            }
        }
    }
}

