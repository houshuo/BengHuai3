namespace MoleMole
{
    using System;
    using UnityEngine;

    public class CollisionResult
    {
        public BaseMonoEntity entity;
        public Vector3 hitForward;
        public Vector3 hitPoint;

        public CollisionResult(BaseMonoEntity entity, Vector3 hitPoint, Vector3 hitForward)
        {
            this.entity = entity;
            this.hitPoint = hitPoint;
            this.hitForward = hitForward;
        }
    }
}

