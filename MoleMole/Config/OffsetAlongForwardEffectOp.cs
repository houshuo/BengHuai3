namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class OffsetAlongForwardEffectOp : EffectCreationOp
    {
        public float offset;
        [NonSerialized, InspectorDisabled]
        public string type = "Offset Along Forward Effect";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            initPos = (Vector3) (initPos + (Vector3.Scale(initDir, initScale) * this.offset));
        }
    }
}

