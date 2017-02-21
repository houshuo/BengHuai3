namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Move")]
    public abstract class BaseMove : BehaviorDesigner.Runtime.Tasks.Action
    {
        protected IAIController _aiController;
        protected IAIEntity _aiEntity;
        private Vector3 _checkStuckLastPos;
        private float _checkStuckTime;
        private float _failMoveTimer;
        protected bool _hasCollided;
        protected BaseMonoMonster _monster;
        private float _speed;
        private bool _usefailMoveTime;
        public bool checkStuck;
        public float checkStuckDistance = 1f;
        public float checkStuckTime = 1f;
        public SharedVector3 CollidedAwayForward;
        public SharedLayerMask CollidedLayerMask;
        public LayerMask CollisionLayerMask;
        public float failMoveTime;
        public bool moveByAnim = true;
        public float moveSpeed = 1f;
        public SharedString moveSpeedKey;
        public SharedFloat sharedDistance;
        public bool stopOnFailure;
        public bool stopOnSucceed = true;
        public bool updateDistance;

        protected BaseMove()
        {
        }

        protected virtual float CalculateTargetDistance()
        {
            Vector3 xZPosition = this._aiEntity.XZPosition;
            Vector3 b = this._aiEntity.AttackTarget.XZPosition;
            return Vector3.Distance(xZPosition, b);
        }

        protected bool CheckAndSetHitWall()
        {
            return false;
        }

        protected bool CheckCollided()
        {
            return ((this.CollisionLayerMask != 0) && this._hasCollided);
        }

        protected bool CheckStuck()
        {
            if (this.checkStuck)
            {
                this._checkStuckTime += Time.deltaTime;
                if (this._checkStuckTime > this.checkStuckTime)
                {
                    if (Vector3.SqrMagnitude(this._checkStuckLastPos - this._aiEntity.XZPosition) < this.checkStuckDistance)
                    {
                        return true;
                    }
                    this._checkStuckTime = 0f;
                    this._checkStuckLastPos = this._aiEntity.XZPosition;
                }
            }
            return false;
        }

        protected virtual void DoFaceToLocalAvatar()
        {
            Vector3 localAvatarDirection = this.GetLocalAvatarDirection();
            this._aiController.TrySteer(localAvatarDirection);
        }

        protected virtual void DoFaceToTarget()
        {
            Vector3 targetDirection = this.GetTargetDirection();
            this._aiController.TrySteer(targetDirection);
        }

        protected void DoMoveBack()
        {
            if (this.moveByAnim)
            {
                this._aiController.TryMove(-this._speed);
            }
            else
            {
                Vector3 velocity = -this._aiEntity.transform.forward;
                velocity.y = 0f;
                velocity.Normalize();
                velocity = (Vector3) (velocity * this.moveSpeed);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetOverrideVelocity(velocity);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetNeedOverrideVelocity(true);
            }
        }

        protected void DoMoveForward()
        {
            if (this.moveByAnim)
            {
                this._aiController.TryMove(this._speed);
            }
            else
            {
                Vector3 forward = this._aiEntity.transform.forward;
                forward.y = 0f;
                forward.Normalize();
                forward = (Vector3) (forward * this.moveSpeed);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetOverrideVelocity(forward);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetNeedOverrideVelocity(true);
            }
        }

        protected void DoMoveLeft()
        {
            if (this.moveByAnim)
            {
                this._aiController.TryMoveHorizontal(-this._speed);
            }
            else
            {
                Vector3 velocity = -this._aiEntity.transform.right;
                velocity.y = 0f;
                velocity.Normalize();
                velocity = (Vector3) (velocity * this.moveSpeed);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetOverrideVelocity(velocity);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetNeedOverrideVelocity(true);
            }
        }

        protected void DoMoveRight()
        {
            if (this.moveByAnim)
            {
                this._aiController.TryMoveHorizontal(this._speed);
            }
            else
            {
                Vector3 right = this._aiEntity.transform.right;
                right.y = 0f;
                right.Normalize();
                right = (Vector3) (right * this.moveSpeed);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetOverrideVelocity(right);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetNeedOverrideVelocity(true);
            }
        }

        protected void DoStopMove()
        {
            if (this.moveByAnim)
            {
                this._aiController.TryStop();
            }
            else
            {
                ((BaseMonoAnimatorEntity) this._aiEntity).SetOverrideVelocity(Vector3.zero);
                ((BaseMonoAnimatorEntity) this._aiEntity).SetNeedOverrideVelocity(false);
            }
        }

        protected Vector3 GetAvoidObstacleDir(Vector3 targetFaceDir)
        {
            int layerMask = (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER);
            if (Physics.Raycast(this._aiEntity.transform.position, targetFaceDir, (float) 1f, layerMask))
            {
                for (int i = 30; i <= 150; i += 30)
                {
                    Vector3 direction = (Vector3) (Quaternion.AngleAxis((float) i, Vector3.up) * targetFaceDir);
                    Vector3 vector2 = (Vector3) (Quaternion.AngleAxis((float) -i, Vector3.up) * targetFaceDir);
                    bool flag = Physics.Raycast(this._aiEntity.transform.position, direction, (float) 2f, layerMask);
                    bool flag2 = Physics.Raycast(this._aiEntity.transform.position, vector2, (float) 2f, layerMask);
                    Debug.DrawRay(this._aiEntity.transform.position, direction, Color.green, 0.1f);
                    Debug.DrawRay(this._aiEntity.transform.position, vector2, Color.green, 0.1f);
                    if (!flag && !flag2)
                    {
                        targetFaceDir = (UnityEngine.Random.Range(0, 100) >= 50) ? vector2 : direction;
                        return targetFaceDir;
                    }
                    if (!flag && flag2)
                    {
                        targetFaceDir = direction;
                        return targetFaceDir;
                    }
                    if (flag && !flag2)
                    {
                        targetFaceDir = vector2;
                        return targetFaceDir;
                    }
                }
            }
            return targetFaceDir;
        }

        protected Vector3 GetLocalAvatarDirection()
        {
            Vector3 xZPosition = this._aiEntity.XZPosition;
            Vector3 vector3 = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        protected float GetLocalAvatarDistance()
        {
            Vector3 xZPosition = this._aiEntity.XZPosition;
            Vector3 b = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
            return Vector3.Distance(xZPosition, b);
        }

        protected virtual float GetSteerRatio()
        {
            return ((this._aiEntity.GetProperty("Animator_MoveSpeedRatio") + 1f) * 1.5f);
        }

        protected virtual Vector3 GetTargetDirection()
        {
            Vector3 xZPosition = this._aiEntity.XZPosition;
            Vector3 vector3 = this._aiEntity.AttackTarget.XZPosition - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        protected float GetTargetDistance()
        {
            if (this.updateDistance)
            {
                return this.sharedDistance.Value;
            }
            return this.CalculateTargetDistance();
        }

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoAvatar)
            {
                this._aiEntity = (BaseMonoAvatar) component;
                this.moveSpeedKey.Value = "AvatarSpeed(FIXED)";
                this._speed = 0f;
            }
            else if (component is BaseMonoMonster)
            {
                this._aiEntity = (BaseMonoMonster) component;
                this._speed = (component as BaseMonoMonster).GetOriginMoveSpeed(this.moveSpeedKey.Value);
            }
            this._aiController = this._aiEntity.GetActiveAIController();
            this._monster = this._aiEntity as BaseMonoMonster;
            if (this.failMoveTime <= 0f)
            {
                this._usefailMoveTime = false;
            }
            else
            {
                this._usefailMoveTime = true;
                this._failMoveTimer = this.failMoveTime;
            }
        }

        private void OnCollisionCallback(int layer, Vector3 awayForward)
        {
            this._hasCollided = true;
            this.CollidedLayerMask.SetValue((LayerMask) (((int) 1) << layer));
            this.CollidedAwayForward.SetValue(awayForward);
        }

        public override void OnEnd()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if ((component != null) && (this.CollisionLayerMask != 0))
            {
                component.ClearCheckForCollision();
            }
            if (!this.moveByAnim)
            {
                this.DoStopMove();
            }
        }

        protected abstract TaskStatus OnMoveUpdate();
        public override void OnStart()
        {
            this.TryStartCollisionCheck();
            this._checkStuckTime = 0f;
            this._checkStuckLastPos = this._aiEntity.XZPosition;
        }

        public sealed override TaskStatus OnUpdate()
        {
            if (this._usefailMoveTime && (this._failMoveTimer > 0f))
            {
                this._failMoveTimer -= Time.deltaTime;
                if (this._failMoveTimer < 0f)
                {
                    this._failMoveTimer = this.failMoveTime;
                    if (this.stopOnFailure)
                    {
                        this.DoStopMove();
                    }
                    return TaskStatus.Failure;
                }
            }
            TaskStatus status = this.OnMoveUpdate();
            if ((status == TaskStatus.Success) && this.stopOnSucceed)
            {
                this.DoStopMove();
                return status;
            }
            if ((status == TaskStatus.Failure) && this.stopOnFailure)
            {
                this.DoStopMove();
            }
            return status;
        }

        protected void ResetCollided()
        {
            this._hasCollided = false;
            this.OnEnd();
        }

        protected void TriggerEliteTeleport(float toDistance)
        {
            if (Mathf.Abs(toDistance) >= 0.5f)
            {
                string str = "Elite_Teleport";
                EvtAbilityStart evt = new EvtAbilityStart(this._monster.GetRuntimeID(), null) {
                    abilityName = str,
                    abilityArgument = toDistance
                };
                Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            }
        }

        protected void TryStartCollisionCheck()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if ((component != null) && (this.CollisionLayerMask != 0))
            {
                component.SetCheckForCollision(this.CollisionLayerMask, new BaseMonoAnimatorEntity.CollisionCallback(this.OnCollisionCallback));
            }
            this._hasCollided = false;
        }

        protected virtual void UpdateTargetDistance()
        {
            if (this.updateDistance && ((this._aiEntity.AttackTarget != null) && this._aiEntity.AttackTarget.IsActive()))
            {
                this.sharedDistance.SetValue(this.CalculateTargetDistance());
            }
        }
    }
}

