namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveKeepDistance : BaseMove
    {
        private float _backSpeed;
        private float _distance;
        public SharedString backSpeedKey;
        public float distance;
        public float distancePadding;
        public float distanceRandomAdd;

        public override void OnAwake()
        {
            base.OnAwake();
            if (!string.IsNullOrEmpty(this.backSpeedKey.Value))
            {
                BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
                this._backSpeed = (component as BaseMonoMonster).GetOriginMoveSpeed(this.backSpeedKey.Value);
            }
            this._distance = this.distance + UnityEngine.Random.Range(0f, this.distanceRandomAdd);
        }

        protected override TaskStatus OnMoveUpdate()
        {
            this.UpdateTargetDistance();
            if (base.CheckCollided())
            {
                return TaskStatus.Success;
            }
            if ((base._aiEntity.AttackTarget == null) || !base._aiEntity.AttackTarget.IsActive())
            {
                return TaskStatus.Success;
            }
            this.DoFaceToTarget();
            float targetDistance = base.GetTargetDistance();
            if (targetDistance > (this._distance + this.distancePadding))
            {
                if (base._aiEntity.GetProperty("AI_CanTeleport") > 0f)
                {
                    base.TriggerEliteTeleport(targetDistance - this._distance);
                }
                base.DoMoveForward();
                return TaskStatus.Running;
            }
            if (targetDistance >= (this._distance - this.distancePadding))
            {
                return TaskStatus.Success;
            }
            if (base._aiEntity.GetProperty("AI_CanTeleport") > 0f)
            {
                base.TriggerEliteTeleport(targetDistance - this._distance);
            }
            if (!string.IsNullOrEmpty(this.backSpeedKey.Value))
            {
                base._aiController.TryMove(-this._backSpeed);
            }
            else
            {
                base.DoMoveBack();
            }
            return TaskStatus.Running;
        }
    }
}

