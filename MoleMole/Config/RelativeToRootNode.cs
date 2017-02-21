namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class RelativeToRootNode : EffectCreationOp
    {
        [NonSerialized, InspectorDisabled]
        public string type = "Relative To Root Node";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            Transform attachPoint = entity.GetAttachPoint("RootNode");
            initPos = attachPoint.position;
        }
    }
}

