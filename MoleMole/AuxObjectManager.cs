namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AuxObjectManager
    {
        private Dictionary<uint, List<MonoAuxObject>> _auxObjectMap;

        public void ClearAuxObjects<T>(uint ownerID) where T: MonoAuxObject
        {
            List<T> auxObjects = this.GetAuxObjects<T>(ownerID);
            for (int i = 0; i < auxObjects.Count; i++)
            {
                auxObjects[i].SetDestroy();
            }
        }

        public void ClearHitBoxDetectByOwnerEvade(uint ownerID)
        {
            List<MonoAnimatedHitboxDetect> auxObjects = this.GetAuxObjects<MonoAnimatedHitboxDetect>(ownerID);
            for (int i = 0; i < auxObjects.Count; i++)
            {
                if (!auxObjects[i].dontDestroyWhenOwnerEvade)
                {
                    auxObjects[i].SetDestroy();
                }
            }
        }

        public void Core()
        {
        }

        public T CreateAuxObject<T>(string name) where T: MonoAuxObject
        {
            return this.LoadOrAddAuxObject<T>(name, 0x21800001, false);
        }

        public T CreateAuxObject<T>(string name, uint ownerID) where T: MonoAuxObject
        {
            return this.LoadOrAddAuxObject<T>(name, ownerID, false);
        }

        public MonoAuxObject CreateSimpleAuxObject(string name, uint ownerID)
        {
            return this.LoadOrAddAuxObject<MonoAuxObject>(name, ownerID, true);
        }

        public void Destroy()
        {
        }

        public MonoAuxObject GetAuxObject(uint ownerID, string entryName)
        {
            return this.GetAuxObject<MonoAuxObject>(ownerID, entryName);
        }

        public T GetAuxObject<T>(uint ownerID, string entryName) where T: MonoAuxObject
        {
            if (this._auxObjectMap.ContainsKey(ownerID))
            {
                List<MonoAuxObject> ls = this._auxObjectMap[ownerID];
                bool flag = false;
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i] == null)
                    {
                        flag = true;
                    }
                    else if (ls[i].entryName == entryName)
                    {
                        return ls[i];
                    }
                }
                if (flag)
                {
                    ls.RemoveAllNulls<MonoAuxObject>();
                }
            }
            return null;
        }

        public List<T> GetAuxObjects<T>(uint ownerID) where T: MonoAuxObject
        {
            List<T> list = new List<T>();
            if (this._auxObjectMap.ContainsKey(ownerID))
            {
                List<MonoAuxObject> ls = this._auxObjectMap[ownerID];
                bool flag = false;
                for (int i = 0; i < ls.Count; i++)
                {
                    if (ls[i] == null)
                    {
                        flag = true;
                    }
                    else
                    {
                        T component = ls[i].GetComponent<T>();
                        if (component != null)
                        {
                            list.Add(component);
                        }
                    }
                }
                if (flag)
                {
                    ls.RemoveAllNulls<MonoAuxObject>();
                }
            }
            return list;
        }

        public void InitAtAwake()
        {
            this._auxObjectMap = new Dictionary<uint, List<MonoAuxObject>>();
        }

        public void InitAtStart()
        {
        }

        public GameObject LoadAuxObjectProto(string name)
        {
            return Miscs.LoadResource<GameObject>(AuxObjectData.GetAuxObjectPrefabPath(name), BundleType.RESOURCE_FILE);
        }

        private T LoadOrAddAuxObject<T>(string name, uint ownerID, bool addComponent) where T: MonoAuxObject
        {
            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(this.LoadAuxObjectProto(name));
            T component = obj3.GetComponent<T>();
            if ((component == null) && addComponent)
            {
                component = obj3.AddComponent<T>();
            }
            component.entryName = name;
            component.ownerID = ownerID;
            if (!this._auxObjectMap.ContainsKey(ownerID))
            {
                this._auxObjectMap.Add(ownerID, new List<MonoAuxObject>());
            }
            this._auxObjectMap[ownerID].Add(component);
            return component;
        }
    }
}

