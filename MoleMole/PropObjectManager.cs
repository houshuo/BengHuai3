namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class PropObjectManager
    {
        private List<BaseMonoPropObject> _propLs = new List<BaseMonoPropObject>();
        private Dictionary<uint, BaseMonoPropObject> _propObjects = new Dictionary<uint, BaseMonoPropObject>();
        private List<uint> _propsToDestroyOnStageChange = new List<uint>();

        private PropObjectManager()
        {
        }

        public void CleanWhenStageChange()
        {
            this.RemoveAllPropObjects();
            this._propsToDestroyOnStageChange.Clear();
        }

        public void Core()
        {
            this.RemoveAllRemovables();
        }

        public uint CreatePropObject(uint ownerID, string propName, float HP, float attack, Vector3 initPos, Vector3 initDir, bool appearAnim = false)
        {
            ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
            BaseMonoPropObject component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(propObjectConfig.PrefabPath, BundleType.RESOURCE_FILE)).GetComponent<BaseMonoPropObject>();
            uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(7);
            component.transform.position = initPos;
            component.transform.forward = initDir;
            component.Init(ownerID, nextRuntimeID, propObjectConfig.Name, appearAnim);
            if ((propName == "JokeBox") && (attack <= 0f))
            {
                (component as MonoBarrelProp)._toExplode = false;
            }
            this._propObjects.Add(nextRuntimeID, component);
            this._propLs.Add(component);
            PropObjectActor actor = Singleton<EventManager>.Instance.CreateActor<PropObjectActor>(component);
            actor.ownerID = ownerID;
            actor.InitProp(HP, attack);
            actor.PostInit();
            return nextRuntimeID;
        }

        public void Destroy()
        {
            for (int i = 0; i < this._propLs.Count; i++)
            {
                if (this._propLs[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(this._propLs[i]);
                }
            }
        }

        public List<BaseMonoPropObject> GetAllPropObjects()
        {
            List<BaseMonoPropObject> list = new List<BaseMonoPropObject>();
            list.AddRange(this._propObjects.Values);
            return list;
        }

        public BaseMonoPropObject GetPropObjectByRuntimeID(uint runtimeID)
        {
            return this._propObjects[runtimeID];
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        public void RegisterDestroyOnStageChange(uint propID)
        {
            this._propsToDestroyOnStageChange.Add(propID);
        }

        public void RemoveAllPropObjects()
        {
            for (int i = 0; i < this._propLs.Count; i++)
            {
                BaseMonoPropObject obj2 = this._propLs[i];
                if (!obj2.IsToBeRemove())
                {
                    obj2.SetDied(KillEffect.KillImmediately);
                }
                this.RemovePropObjectByRuntimeID(obj2.GetRuntimeID(), i);
                i--;
            }
        }

        public void RemoveAllRemovables()
        {
            for (int i = 0; i < this._propLs.Count; i++)
            {
                BaseMonoPropObject obj2 = this._propLs[i];
                if (obj2.IsToBeRemove())
                {
                    this.RemovePropObjectByRuntimeID(obj2.GetRuntimeID(), i);
                    i--;
                }
            }
        }

        private void RemovePropObjectByRuntimeID(uint runtimeID, int lsIx)
        {
            Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
            if (this._propObjects[runtimeID] != null)
            {
                UnityEngine.Object.Destroy(this._propObjects[runtimeID].gameObject);
            }
            this._propObjects.Remove(runtimeID);
            this._propLs.RemoveAt(lsIx);
        }

        public BaseMonoPropObject TryGetPropObjectByRuntimeID(uint runtimeID)
        {
            BaseMonoPropObject obj2;
            this._propObjects.TryGetValue(runtimeID, out obj2);
            return obj2;
        }
    }
}

