namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Move")]
    public class MoveFarThanDistance : BaseMove
    {
        public float distance;

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
            if (targetDistance > this.distance)
            {
                return TaskStatus.Success;
            }
            if (base._aiEntity.GetProperty("AI_CanTeleport") > 0f)
            {
                base.TriggerEliteTeleport(targetDistance - this.distance);
                return TaskStatus.Success;
            }
            base.DoMoveBack();
            return TaskStatus.Running;
        }
    }
}

