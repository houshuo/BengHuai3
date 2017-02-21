namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveNearThanDistance : BaseMove
    {
        private bool _avoidingObstacle;
        private float _avoidObstacleTime;
        private BaseMonoAnimatorEntity _entity;
        private float _fixDirTimer;
        private Vector3 _lastFixedDir;
        private Vector3 _targetCornerByGetPath;
        public bool avoidObstacle;
        public SharedFloat distance = 0f;
        private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;
        private const float FIX_DIR_HOLD_TIME = 0.4f;
        private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;
        public float fixDirRatio = 1f;
        private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;
        public TargetType targetType;
        public bool useFixedDir;
        public bool useGetPath = true;

        private void DoFaceToTargetMore(bool hasResetTargetPos)
        {
            if (this._fixDirTimer > 0f)
            {
                this._fixDirTimer -= Time.deltaTime * base._aiEntity.TimeScale;
                base._aiController.TrySteer(this._lastFixedDir);
            }
            else if ((!this.avoidObstacle || !this._avoidingObstacle) || ((Time.time - this._avoidObstacleTime) > 0.2f))
            {
                Vector3 targetCornerDirection;
                if (hasResetTargetPos)
                {
                    targetCornerDirection = this.GetTargetCornerDirection();
                }
                else
                {
                    targetCornerDirection = this.GetTargetDirection();
                }
                Vector3 normalized = targetCornerDirection;
                if (this.useFixedDir && (base.GetTargetDistance() > 2f))
                {
                    List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                    for (int i = 1; i < allMonsters.Count; i++)
                    {
                        if (allMonsters[i] != base._monster)
                        {
                            Vector3 to = allMonsters[i].XZPosition - base._monster.XZPosition;
                            float num2 = Vector3.Angle(base._monster.FaceDirection, to);
                            if ((num2 < 90f) && (to.magnitude < 4f))
                            {
                                Vector3 vector4 = (Vector3) (((to.normalized / to.magnitude) * (1f - (num2 / 90f))) * this.fixDirRatio);
                                normalized -= vector4;
                            }
                        }
                    }
                    normalized = normalized.normalized;
                    if (normalized != targetCornerDirection)
                    {
                        this._fixDirTimer = 0.4f;
                        this._lastFixedDir = normalized;
                        targetCornerDirection = normalized;
                    }
                }
                if (this.useFixedDir)
                {
                }
                if (this.avoidObstacle)
                {
                    Vector3 avoidObstacleDir = base.GetAvoidObstacleDir(targetCornerDirection);
                    this._avoidingObstacle = targetCornerDirection != avoidObstacleDir;
                    targetCornerDirection = avoidObstacleDir;
                    this._avoidObstacleTime = Time.time;
                }
                base._aiController.TrySteer(targetCornerDirection);
            }
        }

        protected Vector3 GetTargetCornerDirection()
        {
            Vector3 xZPosition = base._aiEntity.XZPosition;
            Vector3 vector3 = this._targetCornerByGetPath - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        public override void OnAwake()
        {
            base.OnAwake();
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
        }

        protected override TaskStatus OnMoveUpdate()
        {
            float localAvatarDistance;
            float num2;
            this.UpdateTargetDistance();
            if (base.CheckCollided())
            {
                return TaskStatus.Success;
            }
            if (base.CheckStuck())
            {
                return TaskStatus.Failure;
            }
            if (this.targetType == TargetType.LocalAvatar)
            {
                this.DoFaceToLocalAvatar();
                localAvatarDistance = base.GetLocalAvatarDistance();
            }
            else
            {
                if ((base._aiEntity.AttackTarget == null) || !base._aiEntity.AttackTarget.IsActive())
                {
                    return TaskStatus.Success;
                }
                localAvatarDistance = base.GetTargetDistance();
                if (GlobalVars.USE_GET_PATH_SWITCH && this.useGetPath)
                {
                    Vector3 targetCorner = new Vector3();
                    if (!Singleton<DetourManager>.Instance.GetTargetPosition(this._entity, base._aiEntity.transform.position, base._aiEntity.AttackTarget.transform.position, ref targetCorner))
                    {
                        return TaskStatus.Running;
                    }
                    Debug.DrawLine(base._aiEntity.transform.position, targetCorner, Color.yellow, 0.1f);
                    this._targetCornerByGetPath = targetCorner;
                    this.DoFaceToTargetMore(true);
                    num2 = Vector3.Distance(base._aiEntity.XZPosition, this._targetCornerByGetPath);
                }
                else
                {
                    this.DoFaceToTargetMore(false);
                }
            }
            num2 = localAvatarDistance;
            if (localAvatarDistance < this.distance.Value)
            {
                return TaskStatus.Success;
            }
            if (base._aiEntity.GetProperty("AI_CanTeleport") > 0f)
            {
                base.TriggerEliteTeleport(num2 - this.distance.Value);
            }
            base.DoMoveForward();
            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            base.OnStart();
            this._avoidingObstacle = false;
            this._avoidObstacleTime = 0f;
        }

        public enum TargetType
        {
            AttackTarget,
            LocalAvatar
        }
    }
}

