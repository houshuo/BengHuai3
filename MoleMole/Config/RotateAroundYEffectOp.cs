namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class RotateAroundYEffectOp : EffectCreationOp
    {
        public float rotation = 180f;
        [NonSerialized, InspectorDisabled]
        public string type = "RotateAroundYEffectOp";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            initDir = (Vector3) (Quaternion.AngleAxis(this.rotation, Vector3.up) * initDir);
        }
    }
}

