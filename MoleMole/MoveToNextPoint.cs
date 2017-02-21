namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [TaskCategory("Move")]
    public class MoveToNextPoint : BaseMove
    {
        private bool _avoidingObstacle;
        private float _avoidObstacleTime;
        private BaseMonoAnimatorEntity _entity;
        private float _fixDirTimer;
        private Vector3 _lastFixedDir;
        private int _lastSpawnPointIndex = -1;
        private uint _lastTargetRuntimeID;
        private HashSet<uint> _passedRuntimeIDs = new HashSet<uint>();
        private HashSet<int> _passedSpawnPoints = new HashSet<int>();
        private int _spawnPointIndex = -1;
        private MonoStageEnv _stageEnv;
        private Vector3 _targetCornerByGetPath;
        private uint _targetRuntimeID;
        private bool _useRandomSpawnPoint;
        public bool avoidObstacle = true;
        public float distance = 1f;
        private const float FIX_DIR_DISTANCE_THERSHOLD = 4f;
        private const float FIX_DIR_HOLD_TIME = 0.4f;
        private const float FIX_DIR_IGNORE_ANGEL_THERSHOLD = 90f;
        public float fixDirRatio = 1f;
        private const float NO_FIX_DIR_TARGET_DISTANCE = 2f;
        public SharedString SpawnPointName;
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

        private float GetCurrentSpawnDistance()
        {
            return Miscs.DistancForVec3IgnoreY(base._aiEntity.transform.position, this.TargetPosition.Value);
        }

        private MonoSpawnPoint GetHintSpawnPoint()
        {
            MonoSpawnPoint spawnPoint = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.GetSpawnPoint();
            if (spawnPoint != null)
            {
                this._spawnPointIndex = this._stageEnv.GetNamedSpawnPointIx(spawnPoint.name);
                if (((this._spawnPointIndex >= 0) && (this._spawnPointIndex != this._lastSpawnPointIndex)) && !this._passedSpawnPoints.Contains(this._spawnPointIndex))
                {
                    return spawnPoint;
                }
            }
            return null;
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

        private Vector3 GetTargetPosition(bool useRandom = true)
        {
            this._spawnPointIndex = -1;
            this._targetRuntimeID = 0;
            this._useRandomSpawnPoint = false;
            MonoSpawnPoint hintSpawnPoint = this.GetHintSpawnPoint();
            if (hintSpawnPoint != null)
            {
                return hintSpawnPoint.transform.position;
            }
            BaseActor[] actorByCategory = Singleton<EventManager>.Instance.GetActorByCategory<BaseActor>(6);
            for (int i = 0; i < actorByCategory.Length; i++)
            {
                if ((actorByCategory[i].IsActive() && (actorByCategory[i] is StageExitFieldActor)) && ((actorByCategory[i].runtimeID != this._lastTargetRuntimeID) && !this._passedRuntimeIDs.Contains(actorByCategory[i].runtimeID)))
                {
                    this._targetRuntimeID = actorByCategory[i].runtimeID;
                    return actorByCategory[i].gameObject.transform.position;
                }
            }
            List<BaseMonoDynamicObject> allNavigationArrows = Singleton<DynamicObjectManager>.Instance.GetAllNavigationArrows();
            for (int j = 0; j < allNavigationArrows.Count; j++)
            {
                if ((allNavigationArrows[j].IsActive() && (allNavigationArrows[j].GetRuntimeID() != this._lastTargetRuntimeID)) && !this._passedRuntimeIDs.Contains(allNavigationArrows[j].GetRuntimeID()))
                {
                    RaycastHit hit;
                    Vector3 vector = allNavigationArrows[j].XZPosition - ((Vector3) (allNavigationArrows[j].transform.forward * 100f));
                    if (Physics.Raycast(allNavigationArrows[j].XZPosition + ((Vector3) (0.5f * Vector3.up)), -allNavigationArrows[j].transform.forward, out hit, 100f, (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
                    {
                        vector = allNavigationArrows[j].XZPosition - ((Vector3) (allNavigationArrows[j].transform.forward * hit.distance));
                    }
                    this._targetRuntimeID = allNavigationArrows[j].GetRuntimeID();
                    return vector;
                }
            }
            if (useRandom)
            {
                int num3 = 0;
                do
                {
                    this._spawnPointIndex = UnityEngine.Random.Range(0, this._stageEnv.spawnPoints.Length);
                }
                while (((num3++ < 10) && (this._stageEnv.spawnPoints.Length > 1)) && this._passedSpawnPoints.Contains(this._spawnPointIndex));
                if (this._stageEnv.spawnPoints[this._spawnPointIndex] != null)
                {
                    this._useRandomSpawnPoint = true;
                    return this._stageEnv.spawnPoints[this._spawnPointIndex].transform.position;
                }
            }
            return base._aiEntity.transform.position;
        }

        private bool MonsterExists()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                if (allMonsters[i].IsActive() && !allMonsters[i].denySelect)
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnAwake()
        {
            base.failMoveTime = 0f;
            base.OnAwake();
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
            this._stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
        }

        protected override TaskStatus OnMoveUpdate()
        {
            if (this.MonsterExists())
            {
                return TaskStatus.Failure;
            }
            if (this._useRandomSpawnPoint)
            {
                Vector3 targetPosition = this.GetTargetPosition(false);
                if (targetPosition != base._aiEntity.transform.position)
                {
                    this.TargetPosition.Value = targetPosition;
                }
            }
            Debug.DrawLine(base._aiEntity.transform.position, this.TargetPosition.Value, Color.blue, 0.1f);
            if (this.GetCurrentSpawnDistance() < this.distance)
            {
                if ((this._spawnPointIndex != -1) && !this._passedSpawnPoints.Contains(this._spawnPointIndex))
                {
                    this._passedSpawnPoints.Add(this._spawnPointIndex);
                }
                if ((this._targetRuntimeID != 0) && !this._passedRuntimeIDs.Contains(this._targetRuntimeID))
                {
                    this._passedRuntimeIDs.Add(this._targetRuntimeID);
                }
                this._lastSpawnPointIndex = this._spawnPointIndex;
                this._lastTargetRuntimeID = this._targetRuntimeID;
                return TaskStatus.Success;
            }
            if (base.CheckStuck())
            {
                this._lastSpawnPointIndex = this._spawnPointIndex;
                this._lastTargetRuntimeID = this._targetRuntimeID;
                return TaskStatus.Failure;
            }
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
            base.DoMoveForward();
            return TaskStatus.Running;
        }

        public override void OnStart()
        {
            base.OnStart();
            this._avoidingObstacle = false;
            this._avoidObstacleTime = 0f;
            this.TargetPosition.Value = this.GetTargetPosition(true);
        }
    }
}

