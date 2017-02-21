namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class RelativeToAttachPoint : EffectCreationOp
    {
        public string AttachPoint;
        public Vector3 OffsetVec3;
        [NonSerialized, InspectorDisabled]
        public string type = "Relative To Attach Point";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            Transform attachPoint = entity.GetComponent<BaseMonoEntity>().GetAttachPoint(this.AttachPoint);
            initPos = attachPoint.position;
            float angle = Vector3.Angle(attachPoint.forward, Vector3.forward);
            initPos += Quaternion.AngleAxis(angle, Vector3.up) * Vector3.Scale(this.OffsetVec3, initScale);
        }
    }
}

