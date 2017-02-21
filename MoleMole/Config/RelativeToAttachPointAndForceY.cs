namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using UnityEngine;

    public class RelativeToAttachPointAndForceY : EffectCreationOp
    {
        public string AttachPoint;
        public Vector3 OffsetVec3;
        [NonSerialized, InspectorDisabled]
        public string type = "Relative To Attach Point and Force Y";

        public override void Process(ref Vector3 initPos, ref Vector3 initDir, ref Vector3 initScale, BaseMonoEntity entity)
        {
            Transform attachPoint = entity.GetComponent<BaseMonoEntity>().GetAttachPoint(this.AttachPoint);
            initPos = attachPoint.position;
            initPos.y = 0f;
            initPos += this.OffsetVec3;
        }
    }
}

