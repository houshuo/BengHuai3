namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CameraManager
    {
        private Dictionary<uint, BaseMonoCamera> _cameraDict = new Dictionary<uint, BaseMonoCamera>();
        private uint _inlevelCameraRuntimeID = 0;
        private uint _mainCameraRuntimeID = 0;

        private CameraManager()
        {
        }

        public void Core()
        {
        }

        public uint CreateCamera(uint cameraType)
        {
            BaseMonoCamera component = null;
            component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(CameraData.GetPrefabResPath(cameraType), BundleType.RESOURCE_FILE)).GetComponent<BaseMonoCamera>();
            uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(2);
            this.RegisterCameraData(cameraType, component, nextRuntimeID);
            switch (cameraType)
            {
                case 1:
                    ((MonoMainCamera) component).Init(nextRuntimeID);
                    break;

                case 2:
                    ((MonoInLevelUICamera) component).Init(nextRuntimeID);
                    break;

                default:
                    throw new Exception("Invalid Type or State!");
            }
            return component.GetRuntimeID();
        }

        public uint CreateCamera(uint cameraType, uint followEntityRuntimeID, uint followMode)
        {
            BaseMonoCamera camera = null;
            return camera.GetRuntimeID();
        }

        public void DisableBossCamera()
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarState, true);
            if (!mainCamera.followState.isCameraLocateRatioUserDefined)
            {
                mainCamera.SetCameraLocateRatio(0.535f);
            }
        }

        public void DisableCrowdCamera()
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarState, true);
        }

        public void EnableBossCamera(uint targetId)
        {
            BaseMonoEntity monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(targetId);
            if (monsterByRuntimeID != null)
            {
                MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
                mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarAndBossState, false);
                mainCamera.followState.followAvatarAndBossState.bossTarget = monsterByRuntimeID;
                if (!mainCamera.followState.isCameraLocateRatioUserDefined)
                {
                    mainCamera.SetCameraLocateRatio(0.735f);
                }
            }
        }

        public void EnableCrowdCamera()
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            mainCamera.followState.TransitBaseState(mainCamera.followState.followAvatarAndCrowdState, false);
        }

        public List<BaseMonoCamera> GetAllCameras()
        {
            List<BaseMonoCamera> list = new List<BaseMonoCamera>();
            list.AddRange(this._cameraDict.Values);
            return list;
        }

        public BaseMonoCamera GetCameraByFollowEntityRuntimeID(uint entityRuntimeID)
        {
            return Singleton<CameraManager>.Instance.GetMainCamera();
        }

        public BaseMonoCamera GetCameraByRuntimeID(uint runtimeID)
        {
            return this._cameraDict[runtimeID];
        }

        public MonoInLevelUICamera GetInLevelUICamera()
        {
            return (MonoInLevelUICamera) this._cameraDict[this._inlevelCameraRuntimeID];
        }

        public MonoMainCamera GetMainCamera()
        {
            return (MonoMainCamera) this._cameraDict[this._mainCameraRuntimeID];
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
            Singleton<CameraManager>.Instance.CreateCamera(1);
            Singleton<CameraManager>.Instance.CreateCamera(2);
            Singleton<CameraManager>.Instance.GetMainCamera().gameObject.SetActive(false);
            Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.SetActive(false);
        }

        public void RegisterCameraData(uint cameraType, BaseMonoCamera camera, uint runtimeID)
        {
            this._cameraDict.Add(runtimeID, camera);
            if (cameraType == 1)
            {
                this._mainCameraRuntimeID = runtimeID;
            }
            else if (cameraType == 2)
            {
                this._inlevelCameraRuntimeID = runtimeID;
            }
        }

        public void RemoveAllCameras()
        {
            List<uint> list = new List<uint>();
            foreach (KeyValuePair<uint, BaseMonoCamera> pair in this._cameraDict)
            {
                list.Add(pair.Key);
            }
            foreach (uint num in list)
            {
                this.RemoveCameraByRuntimeID(num);
            }
        }

        public bool RemoveCameraByRuntimeID(uint runtimeID)
        {
            Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
            UnityEngine.Object.Destroy(this._cameraDict[runtimeID].gameObject);
            return this._cameraDict.Remove(runtimeID);
        }

        public bool controlledRotateKeepManual { get; set; }
    }
}

