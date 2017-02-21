namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class CheckNotBlockedByAllied : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        private RaycastHit _hit;
        public float distance;

        public override void OnAwake()
        {
            this._aiEntity = (IAIEntity) base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            Debug.DrawLine(this._aiEntity.RootNodePosition, this._aiEntity.RootNodePosition + ((Vector3) (this._aiEntity.FaceDirection * this.distance)), Color.red, 1f);
            if (!Physics.Raycast(this._aiEntity.RootNodePosition, this._aiEntity.FaceDirection, out this._hit, this.distance, (((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER)))
            {
                return TaskStatus.Success;
            }
            if (this._hit.collider.gameObject.layer != this._aiEntity.transform.gameObject.layer)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }
    }
}

