namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Ability")]
    public class CheckRaycast : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAnimatorEntity _animatorEntity;
        private BaseMonoEntity _target;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The angle between raycast and start forward.")]
        public float Angle;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The length of raycast.")]
        public float Distance;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("If use target-oriented, the start forward of raycast will based on target and entity; otherwise, the forward of entity will be used.")]
        public RaycastForwardType ForwardType;

        public override void OnAwake()
        {
            BaseMonoEntity component = base.GetComponent<BaseMonoEntity>();
            this._animatorEntity = (BaseMonoAnimatorEntity) component;
            this._target = base.GetComponent<BaseMonoAbilityEntity>().GetAttackTarget();
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 forward = this._animatorEntity.transform.forward;
            if ((this.ForwardType == RaycastForwardType.BasedOnEntityAndTarget) && (this._target != null))
            {
                forward = this._target.transform.position - this._animatorEntity.transform.position;
            }
            forward.y = 0f;
            forward = Vector3.Normalize((Vector3) (Quaternion.AngleAxis(this.Angle, Vector3.up) * forward));
            if (!Physics.Linecast(this._animatorEntity.transform.position, this._animatorEntity.transform.position + ((Vector3) (forward * this.Distance)), (int) (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        public enum RaycastForwardType
        {
            BasedOnEntityForward,
            BasedOnEntityAndTarget
        }
    }
}

