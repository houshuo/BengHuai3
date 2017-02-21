namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveToFaceTarget : BaseMove
    {
        private float _backSpeed;
        private bool _isAutoDirectionSet;
        private float _timer;
        public float angelThreshold = 3f;
        public float autoDistanceThreshold;
        public SharedString backSpeedKey;
        public float maxMoveTime = 2f;
        public SpeedDirection speedDirection;
        public float steerSpeedRatio = 1f;

        protected override float GetSteerRatio()
        {
            return (base.GetSteerRatio() * this.steerSpeedRatio);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            if (!string.IsNullOrEmpty(this.backSpeedKey.Value))
            {
                BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
                this._backSpeed = (component as BaseMonoMonster).GetOriginMoveSpeed(this.backSpeedKey.Value);
            }
        }

        protected override TaskStatus OnMoveUpdate()
        {
            this.UpdateTargetDistance();
            if (this._timer >= 0f)
            {
                this._timer -= Time.deltaTime * base._aiEntity.TimeScale;
                if (base.CheckCollided())
                {
                    return TaskStatus.Success;
                }
                if ((base._aiEntity.AttackTarget == null) || !base._aiEntity.AttackTarget.IsActive())
                {
                    return TaskStatus.Success;
                }
                if (!this._isAutoDirectionSet && (this.speedDirection == SpeedDirection.Auto))
                {
                    BaseMonoEntity attackTarget = base._aiEntity.AttackTarget;
                    Vector3 xZPosition = base._aiEntity.XZPosition;
                    Vector3 b = attackTarget.XZPosition;
                    if (Vector3.Distance(xZPosition, b) <= this.autoDistanceThreshold)
                    {
                        this.speedDirection = SpeedDirection.Back;
                    }
                    else
                    {
                        this.speedDirection = SpeedDirection.Forward;
                    }
                    this._isAutoDirectionSet = true;
                }
                Vector3 targetDirection = this.GetTargetDirection();
                base._aiController.TrySteer(targetDirection, this.GetSteerRatio());
                if (this.speedDirection == SpeedDirection.Back)
                {
                    if (!string.IsNullOrEmpty(this.backSpeedKey.Value))
                    {
                        base._aiController.TryMove(-this._backSpeed);
                    }
                    else
                    {
                        base.DoMoveBack();
                    }
                }
                else if (this.speedDirection == SpeedDirection.Forward)
                {
                    base.DoMoveForward();
                }
                if (Mathf.Abs(Miscs.AngleFromToIgnoreY(base._aiEntity.FaceDirection, targetDirection)) >= this.angelThreshold)
                {
                    base._aiController.TrySteer(targetDirection, this.GetSteerRatio());
                    return TaskStatus.Running;
                }
            }
            return TaskStatus.Success;
        }

        public override void OnStart()
        {
            base.OnStart();
            this._timer = this.maxMoveTime;
        }

        public enum SpeedDirection
        {
            Back,
            Forward,
            Auto
        }
    }
}

