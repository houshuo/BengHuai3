namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class GetBestEvadeDirection : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        protected RaycastHit _hit;
        public SharedFloat DirectionAngle;
        public float distance;
        public int TryAngleNum = 6;

        public override void OnAwake()
        {
            this._aiEntity = (IAIEntity) base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this._aiEntity.AttackTarget != null)
            {
                Vector3 vector = this._aiEntity.XZPosition - this._aiEntity.AttackTarget.XZPosition;
                int num = 1;
                for (int i = 0; i < this.TryAngleNum; i++)
                {
                    num = -num;
                    Vector3 direction = (Vector3) (Quaternion.Euler(0f, (num * (360f / ((float) this.TryAngleNum))) * Mathf.Floor((float) ((i + 1) / 2)), 0f) * vector);
                    Debug.DrawLine(this._aiEntity.RootNodePosition, this._aiEntity.RootNodePosition + ((Vector3) (direction * this.distance)), Color.red, this.distance);
                    if (!Physics.Raycast(this._aiEntity.RootNodePosition, direction, out this._hit, this.distance, (((((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)) | (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER)))
                    {
                        if (Vector3.Cross(this._aiEntity.FaceDirection, direction).y > 0f)
                        {
                            this.DirectionAngle.SetValue(Vector3.Angle(this._aiEntity.FaceDirection, direction));
                        }
                        else
                        {
                            this.DirectionAngle.SetValue(-Vector3.Angle(this._aiEntity.FaceDirection, direction));
                        }
                        return TaskStatus.Success;
                    }
                }
            }
            this.DirectionAngle.SetValue(UnityEngine.Random.Range((float) 0f, (float) 360f));
            return TaskStatus.Success;
        }
    }
}

