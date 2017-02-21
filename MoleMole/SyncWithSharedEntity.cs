namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Group")]
    public class SyncWithSharedEntity : BehaviorDesigner.Runtime.Tasks.Action
    {
        private bool isTeleporting;
        public SharedFloat offsetAngle;
        public SharedFloat offsetRadius;
        public bool syncOneFrame;
        public bool syncWithAttackTarget;
        public SharedEntity targetEntity;
        private const float TELEPORT_DISTANCE = 1f;
        private float teleportTimer;
        public SharedFloat teleportTimeSpan;

        private Vector3 GetTargetPostion()
        {
            Vector3 xZPosition;
            Vector3 forward;
            RaycastHit hit;
            if (this.syncWithAttackTarget)
            {
                xZPosition = (this.targetEntity.Value as BaseMonoMonster).GetAttackTarget().XZPosition;
                forward = -this.targetEntity.Value.transform.forward;
            }
            else
            {
                xZPosition = this.targetEntity.Value.XZPosition;
                forward = this.targetEntity.Value.transform.forward;
            }
            if (this.offsetRadius.Value != 0f)
            {
                Vector3 vector3 = (Vector3) (Quaternion.Euler(0f, this.offsetAngle.Value, 0f) * forward);
                xZPosition += (Vector3) (vector3 * this.offsetRadius.Value);
            }
            bool flag = false;
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            Vector3 start = new Vector3(component.transform.position.x, 0.1f, component.transform.position.z);
            Vector3 end = new Vector3(xZPosition.x, 0.1f, xZPosition.z);
            if (Physics.Linecast(start, end, out hit, (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
            {
                flag = true;
            }
            if (flag)
            {
                Vector3 point = hit.point;
                Vector3 vector11 = start - end;
                Vector3 normalized = vector11.normalized;
                float num = 0.1f;
                Vector3 vector8 = point + ((Vector3) (normalized * num));
                xZPosition = new Vector3(vector8.x, xZPosition.y, vector8.z);
            }
            return xZPosition;
        }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            if (this.targetEntity.Value == null)
            {
                return TaskStatus.Failure;
            }
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (this.syncWithAttackTarget && ((this.targetEntity.Value as BaseMonoMonster).GetAttackTarget() == null))
            {
                return TaskStatus.Failure;
            }
            Vector3 targetPostion = this.GetTargetPostion();
            if (this.isTeleporting)
            {
                if (this.teleportTimeSpan.Value > 0f)
                {
                    if (this.teleportTimer >= this.teleportTimeSpan.Value)
                    {
                        this.isTeleporting = false;
                    }
                }
                else if (component.gameObject.activeSelf)
                {
                    component.FireEffect("Monster_TeleportTo_Small");
                    this.isTeleporting = false;
                }
            }
            if (Vector3.Distance(component.XZPosition, targetPostion) > 1f)
            {
                if (this.teleportTimeSpan.Value > 0f)
                {
                    if (!this.isTeleporting)
                    {
                        this.teleportTimer = 0f;
                        this.isTeleporting = true;
                    }
                }
                else if (component.gameObject.activeSelf)
                {
                    component.FireEffect("Monster_TeleportFrom_Small");
                    this.isTeleporting = true;
                }
            }
            if (this.syncWithAttackTarget)
            {
                Vector3 vector2 = targetPostion - component.transform.position;
                component.transform.forward = vector2.normalized;
            }
            else
            {
                component.transform.forward = this.targetEntity.Value.transform.forward;
            }
            if (this.isTeleporting && (this.teleportTimeSpan.Value > 0f))
            {
                this.teleportTimer += Time.deltaTime;
                component.transform.position = Vector3.Lerp(component.transform.position, targetPostion, this.teleportTimer / this.teleportTimeSpan.Value);
            }
            else
            {
                component.transform.position = targetPostion;
            }
            component.SetLocomotionBool("IsMove", (this.targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionBool("IsMove"), false);
            component.SetLocomotionBool("IsMoveHorizontal", (this.targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionBool("IsMoveHorizontal"), false);
            component.SetLocomotionFloat("MoveSpeed", (this.targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionFloat("MoveSpeed"), false);
            component.SetLocomotionFloat("AbsMoveSpeed", (this.targetEntity.Value as BaseMonoAnimatorEntity).GetLocomotionFloat("AbsMoveSpeed"), false);
            if (this.syncOneFrame && !this.isTeleporting)
            {
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}

