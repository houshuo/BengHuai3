namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Group")]
    public class MoveKeepDistanceWithEntity : MoveKeepDistance
    {
        protected RaycastHit _hit;
        public SharedFloat offsetAngle;
        public float offsetRadius;
        public float steerSpeedRatio = 1f;
        public SharedEntity targetEntity;

        protected override float CalculateTargetDistance()
        {
            Vector3 xZPosition = base._aiEntity.XZPosition;
            Vector3 targetPostion = this.GetTargetPostion();
            return Vector3.Distance(xZPosition, targetPostion);
        }

        protected override Vector3 GetTargetDirection()
        {
            Vector3 xZPosition = base._aiEntity.XZPosition;
            Vector3 vector3 = this.GetTargetPostion() - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return (Vector3) (vector3 * this.steerSpeedRatio);
        }

        private Vector3 GetTargetPostion()
        {
            Vector3 xZPosition = this.targetEntity.Value.XZPosition;
            if (this.offsetRadius == 0f)
            {
                return xZPosition;
            }
            Vector3 vector2 = (Vector3) (Quaternion.Euler(0f, this.offsetAngle.Value, 0f) * this.targetEntity.Value.transform.forward);
            Vector3 direction = (Vector3) (Quaternion.Euler(0f, this.offsetAngle.Value, 0f) * this.targetEntity.Value.transform.forward);
            Debug.DrawLine(base._aiEntity.RootNodePosition, base._aiEntity.RootNodePosition + ((Vector3) (direction * base.distance)), Color.red, base.distance);
            if (!Physics.Raycast(base._aiEntity.RootNodePosition, direction, out this._hit, base.distance, (((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)) | (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER)))
            {
                return (xZPosition + ((Vector3) (vector2 * this.offsetRadius)));
            }
            direction = (Vector3) (Quaternion.Euler(0f, -this.offsetAngle.Value, 0f) * this.targetEntity.Value.transform.forward);
            Debug.DrawLine(base._aiEntity.RootNodePosition, base._aiEntity.RootNodePosition + ((Vector3) (direction * base.distance)), Color.red, base.distance);
            if (!Physics.Raycast(base._aiEntity.RootNodePosition, direction, out this._hit, base.distance, (((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)) | (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER)))
            {
                return (xZPosition + ((Vector3) (-vector2 * this.offsetRadius)));
            }
            return (xZPosition += ((Vector3) (vector2 * this.offsetRadius)));
        }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void UpdateTargetDistance()
        {
            if (base.updateDistance && ((this.targetEntity.Value != null) && this.targetEntity.Value.IsActive()))
            {
                base.sharedDistance.SetValue(this.CalculateTargetDistance());
            }
        }
    }
}

