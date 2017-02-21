namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class DetourManager
    {
        private string _currentLocatorName = string.Empty;
        private Dictionary<uint, DetourElement> _detours = new Dictionary<uint, DetourElement>();
        private float _disReachCornerThreshold = 0.5f;
        private float _disThreshold = 0.01f;
        private float _getPathDisThreshold = 0.5f;
        private int _getPathMaxNumPerFrame = 1;
        private int _getPathNumPerFrame;
        private float _getPathTimeThreshold = 0.1f;
        private int _stageAreaWalkMask = (((int) 1) << NavMesh.GetAreaFromName("Walkable"));

        private DetourManager()
        {
        }

        public void Clear()
        {
            this._detours.Clear();
        }

        public void Core()
        {
            this._getPathNumPerFrame = 0;
        }

        private DetourElement FillDetourElement(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition)
        {
            uint runtimeID = entity.GetRuntimeID();
            DetourElement element = this.GetNewDetourElement(entity, sourcePosition, targetPosition);
            if (element == null)
            {
                this.RemoveDetourElement(runtimeID);
                return null;
            }
            if (this._detours.ContainsKey(runtimeID))
            {
                this._detours[runtimeID] = element;
                return element;
            }
            this._detours.Add(runtimeID, element);
            return element;
        }

        private bool GetCornerAndCalcPathWhenNeed(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
        {
            DetourElement element;
            uint runtimeID = entity.GetRuntimeID();
            if (this._getPathNumPerFrame >= this._getPathMaxNumPerFrame)
            {
                return this.GetTargetCorner(entity, sourcePosition, targetPosition, ref targetCorner);
            }
            this._detours.TryGetValue(runtimeID, out element);
            if (element == null)
            {
                DetourElement element2 = this.FillDetourElement(entity, sourcePosition, targetPosition);
                if (element2 != null)
                {
                    targetCorner = element2.corners[element2.targetCornerIndex];
                    return true;
                }
                targetCorner = targetPosition;
                return true;
            }
            bool flag = this.GetTargetCorner(entity, sourcePosition, targetPosition, ref targetCorner);
            if (flag)
            {
                return flag;
            }
            if ((Time.time - element.lastGetPathTime) <= this._getPathTimeThreshold)
            {
                return flag;
            }
            DetourElement element3 = this.FillDetourElement(entity, sourcePosition, targetPosition);
            if (element3 != null)
            {
                targetCorner = element3.corners[element3.targetCornerIndex];
                return true;
            }
            targetCorner = targetPosition;
            return true;
        }

        private string GetLocatorName(StageEntry newStage)
        {
            char[] separator = new char[] { '/' };
            string[] strArray = newStage.LocationPointName.Split(separator);
            return strArray[strArray.Length - 1];
        }

        private DetourElement GetNewDetourElement(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition)
        {
            NavMeshPath path = new NavMeshPath();
            bool flag = NavMesh.CalculatePath(sourcePosition, targetPosition, this._stageAreaWalkMask, path);
            for (int i = 0; i < (path.corners.Length - 1); i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.green, 0.1f);
            }
            this._getPathNumPerFrame++;
            DetourElement element = new DetourElement {
                id = entity.GetRuntimeID(),
                targetPosition = targetPosition,
                isCompletePath = flag,
                lastGetPathTime = Time.time
            };
            Vector3[] vectorArray = this.SimplifyPath(sourcePosition, path);
            if (vectorArray.Length == 0)
            {
                return null;
            }
            CapsuleCollider componentInChildren = entity.GetComponentInChildren<CapsuleCollider>();
            if (componentInChildren != null)
            {
                element.disReachCornerThreshold = componentInChildren.radius;
            }
            else
            {
                element.disReachCornerThreshold = this._disReachCornerThreshold;
            }
            element.corners = vectorArray;
            element.targetCornerIndex = 0;
            return element;
        }

        private bool GetTargetCorner(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
        {
            DetourElement element;
            this._detours.TryGetValue(entity.GetRuntimeID(), out element);
            if (element == null)
            {
                return false;
            }
            if (Miscs.DistancForVec3IgnoreY(targetPosition, element.targetPosition) > this._getPathDisThreshold)
            {
                this._detours.Remove(entity.GetRuntimeID());
                return false;
            }
            if (Miscs.DistancForVec3IgnoreY(element.corners[element.targetCornerIndex], sourcePosition) <= element.disReachCornerThreshold)
            {
                if (element.targetCornerIndex == (element.corners.Length - 1))
                {
                    this._detours.Remove(entity.GetRuntimeID());
                    targetCorner = targetPosition;
                    return true;
                }
                element.targetCornerIndex++;
                targetCorner = element.corners[element.targetCornerIndex];
                return true;
            }
            targetCorner = element.corners[element.targetCornerIndex];
            return true;
        }

        public bool GetTargetPosition(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
        {
            if (!this.Raycast(entity.GetRuntimeID(), sourcePosition, targetPosition))
            {
                targetCorner = targetPosition;
                return true;
            }
            return this.GetCornerAndCalcPathWhenNeed(entity, sourcePosition, targetPosition, ref targetCorner);
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        public void LoadNavMeshRelatedLevel(string stageTypeName)
        {
            StageEntry stageEntryByName = StageData.GetStageEntryByName(stageTypeName);
            char[] separator = new char[] { '/' };
            string str = stageEntryByName.GetPerpStagePrefabPath().Split(separator)[1];
            if (!string.IsNullOrEmpty(this._currentLocatorName))
            {
                SceneManager.UnloadScene(this._currentLocatorName);
                this._currentLocatorName = string.Empty;
            }
            string locatorName = this.GetLocatorName(stageEntryByName);
            foreach (ConfigNavMeshScenePath path in GlobalDataManager.metaConfig.scenePaths)
            {
                if ((path.MainSceneName == str) && (path.UnitySceneName == locatorName))
                {
                    SceneManager.LoadScene(path.UnitySceneName, LoadSceneMode.Additive);
                    this._currentLocatorName = locatorName;
                    this.ResetStageAreaMask(stageEntryByName);
                    break;
                }
            }
        }

        public bool RandomPosition(Vector3 sourcePosition, float maxDistance, out Vector3 targetPosition)
        {
            int num = 10;
            for (int i = 0; i < num; i++)
            {
                NavMeshHit hit;
                Vector3 vector = sourcePosition + ((Vector3) (UnityEngine.Random.insideUnitSphere * maxDistance));
                if (NavMesh.SamplePosition(vector, out hit, 1f, this._stageAreaWalkMask))
                {
                    targetPosition = hit.position;
                    return true;
                }
            }
            targetPosition = sourcePosition;
            return false;
        }

        private bool Raycast(uint id, Vector3 sourcePosition, Vector3 targetPosition)
        {
            NavMeshHit hit;
            bool flag = NavMesh.Raycast(sourcePosition, targetPosition, out hit, this._stageAreaWalkMask);
            if (!flag)
            {
                Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.1f);
            }
            return flag;
        }

        public void RemoveDetourElement(uint id)
        {
            if (this._detours.ContainsKey(id))
            {
                this._detours.Remove(id);
            }
        }

        public void ResetStageAreaMask(StageEntry newStage)
        {
            uint num;
            string locatorName = this.GetLocatorName(newStage);
            bool flag = uint.TryParse(locatorName.Substring(locatorName.Length - 2), out num);
            if (!flag)
            {
                flag = uint.TryParse(locatorName.Substring(locatorName.Length - 1), out num);
            }
            int areaFromName = NavMesh.GetAreaFromName("Walkable");
            if (flag)
            {
                areaFromName = NavMesh.GetAreaFromName("StageMask" + num.ToString());
            }
            this._stageAreaWalkMask = (((int) 1) << areaFromName) | (((int) 1) << NavMesh.GetAreaFromName("Walkable"));
        }

        private Vector3[] SimplifyPath(Vector3 sourcePosition, NavMeshPath path)
        {
            List<Vector3> list = new List<Vector3>();
            Vector3[] corners = path.corners;
            for (int i = 0; i <= (corners.Length - 1); i++)
            {
                Vector3 vector = corners[i] - sourcePosition;
                vector.y = 0f;
                if (vector.magnitude > this._disThreshold)
                {
                    list.Add(corners[i]);
                }
            }
            return list.ToArray();
        }
    }
}

