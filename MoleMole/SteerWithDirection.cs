namespace MoleMole
{
    using System;
    using UnityEngine;

    public class SteerWithDirection : FaceToTarget
    {
        public float angle = 10f;

        public override Vector3 GetTargetFaceDir()
        {
            return (Vector3) (Quaternion.Euler(0f, this.angle, 0f) * base._aiEntity.FaceDirection);
        }
    }
}

