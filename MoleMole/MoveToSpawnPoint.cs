namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveToSpawnPoint : BaseMove
    {
        private BaseMonoAnimatorEntity _entity;
        private float _fixDirTimer;
        private Vector3 _lastFixedDir;
        private Vector3 _targetCornerByGetPath;
        public float distance = 1f;
        private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;
        private const float FIX_DIR_HOLD_TIME = 0.4f;
        private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;
        public float fixDirRatio = 1f;
        private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;
        public string SpawnPointName;
        public SharedVector3 TargetPosition;
        public bool useFixedDir;
        public bool useGetPath = true;

        private void DoFaceToTargetMore(bool hasResetTargetPos)
        {
            if (this._fixDirTimer > 0f)
            {
                this._fixDirTimer -= Time.deltaTime * base._aiEntity.TimeScale;
                base._aiController.TrySteer(this._lastFixedDir);
            }
            else
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
                base._aiController.TrySteer(targetCornerDirection);
            }
        }

        private float GetCurrentSpawnDistance()
        {
            return Miscs.DistancForVec3IgnoreY(base._aiEntity.transform.position, this.TargetPosition.Value);
        }

        private Vector3 GetSpawnPointPos(string spawnName)
        {
            int namedSpawnPointIx;
            MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
            if (spawnName != null)
            {
                namedSpawnPointIx = stageEnv.GetNamedSpawnPointIx(spawnName);
            }
            else
            {
                namedSpawnPointIx = UnityEngine.Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length);
            }
            return stageEnv.spawnPoints[namedSpawnPointIx].transform.position;
        }

        protected Vector3 GetTargetCornerDirection()
        {
            Vector3 xZPosition = base._aiEntity.XZPosition;
            Vector3 vector3 = this._targetCornerByGetPath - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        private Vector3 GetTargetDir()
        {
            Vector3 xZPosition = base._aiEntity.XZPosition;
            Vector3 vector3 = this.TargetPosition.Value - xZPosition;
            vector3.y = 0f;
            vector3.Normalize();
            return vector3;
        }

        public override void OnAwake()
        {
            base.OnAwake();
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
            this.TargetPosition.Value = this.GetSpawnPointPos(this.SpawnPointName);
        }

        protected override TaskStatus OnMoveUpdate()
        {
            float currentSpawnDistance = this.GetCurrentSpawnDistance();
            if (GlobalVars.USE_GET_PATH_SWITCH && this.useGetPath)
            {
                Vector3 targetCorner = new Vector3();
                if (!Singleton<DetourManager>.Instance.GetTargetPosition(this._entity, base._aiEntity.transform.position, this.TargetPosition.Value, ref targetCorner))
                {
                    return TaskStatus.Running;
                }
                Debug.DrawLine(base._aiEntity.transform.position, targetCorner, Color.yellow, 0.1f);
                this._targetCornerByGetPath = targetCorner;
                this.DoFaceToTargetMore(true);
            }
            else
            {
                this.DoFaceToTargetMore(false);
            }
            if (currentSpawnDistance < this.distance)
            {
                return TaskStatus.Success;
            }
            base.DoMoveForward();
            return TaskStatus.Running;
        }
    }
}

