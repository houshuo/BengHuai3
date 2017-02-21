namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class RelativeToTransformPath : EffectCreationOp
    {
        public string TransformPath;
        [NonSerialized, InspectorDisabled]
        public string type = "Relative To Transform Path";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            Transform transform = entity.transform.Find(this.TransformPath);
            initPos = transform.position;
        }
    }
}

