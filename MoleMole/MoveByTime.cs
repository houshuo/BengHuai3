namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveByTime : BaseMove
    {
        private SpeedDirection _currentDirection;
        private float _timer;
        private Vector3 _toDirection;
        private float _wanderHitWallTimer;
        private float _wanderTimer;
        public SharedFloat angel = 0f;
        public AngelType angelType;
        private const float CHECK_WALL_CLIP_OFF = 0.5f;
        private const float CHECK_WALL_DISTANCE_CAP = 10f;
        private const float CHECK_WALL_STEP = 12f;
        public float chooseAngel_1;
        public float chooseAngel_2;
        public bool instantSteer;
        public float moveTime;
        public MoveType moveType = MoveType.AlwaysFaceTarget;
        public float randAngelMax;
        public float randAngelMin;
        public SpeedDirection speedDirection;
        public float steerSpeedRatio = 1f;
        private const float WANDER_HIT_WALL_CHECK_TIME_INTERVAL = 1f;
        public float wanderIntervalTime = 1f;

        private void DoFaceToCertainDirection()
        {
            if (this.instantSteer)
            {
                base._aiController.TrySteerInstant(this._toDirection);
            }
            else
            {
                base._aiController.TrySteer(this._toDirection, this.GetSteerRatio());
            }
        }

        protected override void DoFaceToLocalAvatar()
        {
            Vector3 localAvatarDirection = base.GetLocalAvatarDirection();
            if (this.instantSteer)
            {
                base._aiController.TrySteerInstant(localAvatarDirection);
            }
            else
            {
                base._aiController.TrySteer(localAvatarDirection, this.GetSteerRatio());
            }
        }

        protected override void DoFaceToTarget()
        {
            Vector3 targetDirection = this.GetTargetDirection();
            if (this.instantSteer)
            {
                base._aiController.TrySteerInstant(targetDirection);
            }
            else
            {
                base._aiController.TrySteer(targetDirection, this.GetSteerRatio());
            }
        }

        private void DoMove()
        {
            switch (this._currentDirection)
            {
                case SpeedDirection.Back:
                    base.DoMoveBack();
                    break;

                case SpeedDirection.Forward:
                    base.DoMoveForward();
                    break;

                case SpeedDirection.Left:
                    base.DoMoveLeft();
                    break;

                case SpeedDirection.Right:
                    base.DoMoveRight();
                    break;
            }
        }

        private void GetRandomWanDerDirectionLR()
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                this._currentDirection = SpeedDirection.Left;
            }
            else
            {
                this._currentDirection = SpeedDirection.Right;
            }
        }

        protected override float GetSteerRatio()
        {
            return (base.GetSteerRatio() * this.steerSpeedRatio);
        }

        protected override TaskStatus OnMoveUpdate()
        {
            this.UpdateTargetDistance();
            if (base.CheckCollided() && (this.speedDirection != SpeedDirection.WanderLR))
            {
                return TaskStatus.Success;
            }
            if (this._timer < 0f)
            {
                return TaskStatus.Success;
            }
            this._timer -= Time.deltaTime * base._aiEntity.TimeScale;
            if (this.speedDirection == SpeedDirection.WanderLR)
            {
                if (base.CheckCollided())
                {
                    this.ReverseDirectionLR();
                    this._wanderHitWallTimer = 1f;
                    this._wanderTimer = this.wanderIntervalTime;
                    base.ResetCollided();
                }
                if (this._wanderHitWallTimer < 0f)
                {
                    base.TryStartCollisionCheck();
                    this._wanderHitWallTimer = 1f;
                }
                else
                {
                    this._wanderHitWallTimer -= Time.deltaTime * base._aiEntity.TimeScale;
                }
                if (this._wanderTimer < 0f)
                {
                    this._wanderTimer = this.wanderIntervalTime;
                    this.GetRandomWanDerDirectionLR();
                }
                this._wanderTimer -= Time.deltaTime * base._aiEntity.TimeScale;
            }
            if (this.moveType == MoveType.AlwaysFaceTarget)
            {
                if ((base._aiEntity.AttackTarget == null) || !base._aiEntity.AttackTarget.IsActive())
                {
                    return TaskStatus.Success;
                }
                this.DoFaceToTarget();
            }
            else if (((this.moveType == MoveType.ConstantDirection) || (this.moveType == MoveType.ChooseDirection)) || (this.moveType == MoveType.RandomDirection))
            {
                this.DoFaceToCertainDirection();
            }
            this.DoMove();
            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            base.OnStart();
            this._timer = this.moveTime;
            Vector3 faceDirection = base._aiEntity.FaceDirection;
            switch (this.angelType)
            {
                case AngelType.Local:
                    faceDirection = base._aiEntity.FaceDirection;
                    break;

                case AngelType.TargetRelative:
                    if ((base._aiEntity.AttackTarget != null) && base._aiEntity.AttackTarget.IsActive())
                    {
                        faceDirection = this.GetTargetDirection();
                        break;
                    }
                    this._timer = -1f;
                    break;

                case AngelType.World:
                    faceDirection = Vector3.forward;
                    break;
            }
            if (this.moveType == MoveType.ConstantDirection)
            {
                this.SetDirectionFromAngle(this.angel.Value, faceDirection);
            }
            else if (this.moveType == MoveType.RandomDirection)
            {
                this.SetRandomDirectionWithWallcheck(faceDirection);
            }
            else if (this.moveType == MoveType.ChooseDirection)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    this.angel = this.chooseAngel_1;
                }
                else
                {
                    this.angel = this.chooseAngel_2;
                }
                this.SetDirectionFromAngle(this.angel.Value, faceDirection);
            }
            if (this.speedDirection == SpeedDirection.WanderLR)
            {
                this.GetRandomWanDerDirectionLR();
                this._wanderTimer = this.wanderIntervalTime;
                this._wanderHitWallTimer = 1f;
            }
            else
            {
                this._currentDirection = this.speedDirection;
            }
        }

        private void ReverseDirectionLR()
        {
            if (this._currentDirection == SpeedDirection.Left)
            {
                this._currentDirection = SpeedDirection.Right;
            }
            else if (this._currentDirection == SpeedDirection.Right)
            {
                this._currentDirection = SpeedDirection.Left;
            }
        }

        private void SetDirectionFromAngle(float angle, Vector3 charDirection)
        {
            this._toDirection = (Vector3) (Quaternion.AngleAxis(angle, Vector3.up) * charDirection);
            this._toDirection.y = 0f;
            this._toDirection.Normalize();
        }

        private void SetRandomDirectionWithWallcheck(Vector3 charDirection)
        {
            int num = Mathf.FloorToInt((this.randAngelMax - this.randAngelMin) / 12f);
            float num2 = -1f;
            Vector3 zero = Vector3.zero;
            float num3 = (this.randAngelMax <= this.randAngelMin) ? this.randAngelMax : this.randAngelMin;
            for (int i = 0; i < num; i++)
            {
                Vector3 forward = (Vector3) (Quaternion.AngleAxis(num3 + (i * 12f), Vector3.up) * charDirection);
                float num5 = CollisionDetectPattern.GetRaycastDistance(base._aiEntity.RootNodePosition, forward, 10f, 0.5f, InLevelData.STAGE_COLLIDER_LAYER);
                if (num5 > num2)
                {
                    num2 = num5;
                    zero = forward;
                }
            }
            if (num2 == 10f)
            {
                zero = (Vector3) (Quaternion.AngleAxis(UnityEngine.Random.Range(this.randAngelMin, this.randAngelMax), Vector3.up) * charDirection);
            }
            this._toDirection = zero;
            this._toDirection.y = 0f;
            this._toDirection.Normalize();
        }

        public enum AngelType
        {
            Local,
            TargetRelative,
            World
        }

        public enum MoveType
        {
            ConstantDirection,
            RandomDirection,
            AlwaysFaceTarget,
            ChooseDirection
        }

        public enum SpeedDirection
        {
            Back,
            Forward,
            Left,
            Right,
            WanderLR
        }
    }
}

