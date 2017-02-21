namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class FaceToTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIController _aiController;
        protected IAIEntity _aiEntity;
        private float _timer;
        public float angelThreshold = 3f;
        public bool forceAndInstant;
        public bool keepFacing;
        public float maxTurnTime = 2f;
        public float steerSpeedRatio = 1f;

        public virtual Vector3 GetTargetFaceDir()
        {
            Vector3 xZPosition = this._aiEntity.XZPosition;
            Vector3 vector3 = this._aiEntity.AttackTarget.XZPosition - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoAvatar)
            {
                this._aiEntity = (BaseMonoAvatar) component;
            }
            else if (component is BaseMonoMonster)
            {
                this._aiEntity = (BaseMonoMonster) component;
            }
            this._aiController = this._aiEntity.GetActiveAIController();
        }

        public override void OnStart()
        {
            this._timer = this.maxTurnTime;
        }

        public override TaskStatus OnUpdate()
        {
            if (this._timer >= 0f)
            {
                this._timer -= Time.deltaTime * this._aiEntity.TimeScale;
                if ((this._aiEntity.AttackTarget == null) || !this._aiEntity.AttackTarget.IsActive())
                {
                    return TaskStatus.Success;
                }
                Vector3 targetFaceDir = this.GetTargetFaceDir();
                if (this.forceAndInstant)
                {
                    base.GetComponent<BaseMonoAnimatorEntity>().SteerFaceDirectionTo(targetFaceDir);
                    return TaskStatus.Success;
                }
                if (Mathf.Abs(Miscs.AngleFromToIgnoreY(this._aiEntity.FaceDirection, targetFaceDir)) >= this.angelThreshold)
                {
                    this._aiController.TrySteer(targetFaceDir, (this.steerSpeedRatio * (this._aiEntity.GetProperty("Animator_MoveSpeedRatio") + 1f)) * 1.5f);
                    return TaskStatus.Running;
                }
                if (this.keepFacing)
                {
                    return TaskStatus.Running;
                }
            }
            return TaskStatus.Success;
        }
    }
}

