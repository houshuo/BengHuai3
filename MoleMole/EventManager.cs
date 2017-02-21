namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class EventManager
    {
        public List<uint> _actorIdsToCleanUp = new List<uint>();
        private List<BaseActor> _actorList = new List<BaseActor>();
        private Dictionary<uint, BaseActor> _actors = new Dictionary<uint, BaseActor>();
        private BaseActor[] _actorsBuffer = new BaseActor[0x40];
        private List<BaseActor>[] _categoryActors = new List<BaseActor>[8];
        private BaseEvent _curParentEvt;
        private bool _dispatchPaused;
        private Dictionary<System.Type, List<ListenerRegistry>> _evtListeners = new Dictionary<System.Type, List<ListenerRegistry>>();
        private bool _isStopped;
        private List<System.Type> _maskedEvtTypes = new List<System.Type>();
        private List<BaseEvent> _queuedEvents = new List<BaseEvent>();

        protected EventManager()
        {
            for (int i = 1; i < this._categoryActors.Length; i++)
            {
                this._categoryActors[i] = new List<BaseActor>();
            }
        }

        public void Core()
        {
            if (!this._isStopped)
            {
                for (int i = 0; i < this._actorIdsToCleanUp.Count; i++)
                {
                    this.RemoveAllListener(this._actorIdsToCleanUp[i]);
                }
                if (this._actorIdsToCleanUp.Count > 0)
                {
                    this._actorIdsToCleanUp.Clear();
                }
                if (this._actorList.Count > this._actorsBuffer.Length)
                {
                    this._actorsBuffer = new BaseActor[this._actorsBuffer.Length * 2];
                }
                int count = this._actorList.Count;
                this._actorList.CopyTo(this._actorsBuffer);
                for (int j = 0; j < count; j++)
                {
                    this._actorsBuffer[j].Core();
                }
                if (!this._dispatchPaused)
                {
                    int num4 = 0;
                    do
                    {
                        int num5 = this._queuedEvents.Count;
                        for (int k = 0; k < num5; k++)
                        {
                            this.DispatchEvent(this._queuedEvents[k]);
                            if (this._isStopped)
                            {
                                return;
                            }
                        }
                        for (int m = 0; m < num5; m++)
                        {
                            this.DispatchListenEvent(this._queuedEvents[m]);
                            if (this._isStopped)
                            {
                                return;
                            }
                            Singleton<LevelDesignManager>.Instance.DispatchLevelDesignListenEvent(this._queuedEvents[m]);
                        }
                        this._queuedEvents.RemoveRange(0, num5);
                        num4++;
                    }
                    while (this._queuedEvents.Count > 0);
                }
            }
        }

        public T CreateActor<T>(BaseMonoEntity entity) where T: BaseActor, new()
        {
            T item = Activator.CreateInstance<T>();
            item.runtimeID = entity.GetRuntimeID();
            item.gameObject = entity.gameObject;
            item.Init(entity);
            this._actorList.Add(item);
            this._actors.Add(entity.GetRuntimeID(), item);
            ushort index = Singleton<RuntimeIDManager>.Instance.ParseCategory(item.runtimeID);
            this._categoryActors[index].Add(item);
            this.ProcessInitedActor(item);
            Singleton<LevelManager>.Instance.gameMode.RegisterRuntimeID(item.runtimeID);
            return item;
        }

        [Conditional("UNITY_EDITOR"), Conditional("NG_HSOD_DEBUG")]
        private void DebugLogEvent(BaseEvent evt)
        {
        }

        public void Destroy()
        {
            this.DropEventsAndStop();
        }

        protected virtual void DispatchEvent(BaseEvent evt)
        {
            if (this._actors.ContainsKey(evt.targetID))
            {
                BaseActor actor;
                this._actors.TryGetValue(evt.targetID, out actor);
                this._curParentEvt = evt;
                bool flag = actor.OnEvent(evt);
                this._curParentEvt = null;
                if (evt.requireHandle)
                {
                }
            }
        }

        protected virtual void DispatchListenEvent(BaseEvent evt)
        {
            List<ListenerRegistry> list;
            this._evtListeners.TryGetValue(evt.GetType(), out list);
            if (list != null)
            {
                ListenerRegistry[] registryArray = list.ToArray();
                for (int i = 0; i < registryArray.Length; i++)
                {
                    this._curParentEvt = evt;
                    this._actors[registryArray[i].listenerID].ListenEvent(evt);
                    this._curParentEvt = null;
                }
            }
        }

        public void DropEventsAndStop()
        {
            this._isStopped = true;
            this._queuedEvents.Clear();
        }

        public virtual void FireEvent(BaseEvent evt, MPEventDispatchMode mode = 0)
        {
            if (!this._isStopped && ((this._maskedEvtTypes.Count <= 0) || !this._maskedEvtTypes.Contains(evt.GetType())))
            {
                evt.parent = this._curParentEvt;
                this._queuedEvents.Add(evt);
            }
        }

        public LayerMask GetAbilityHitboxTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            return Singleton<LevelManager>.Instance.gameMode.GetAbilityHitboxTargettingMask(ownerID, targetting);
        }

        public LayerMask GetAbilityTargettingMask(uint ownerID, MixinTargetting targetting)
        {
            return Singleton<LevelManager>.Instance.gameMode.GetAbilityTargettingMask(ownerID, targetting);
        }

        public BaseActor GetActor(uint runtimeID)
        {
            BaseActor actor;
            this._actors.TryGetValue(runtimeID, out actor);
            return actor;
        }

        public T GetActor<T>(uint runtimeID) where T: BaseActor
        {
            BaseActor actor;
            this._actors.TryGetValue(runtimeID, out actor);
            return (actor as T);
        }

        public T[] GetActorByCategory<T>(ushort category) where T: BaseActor
        {
            if (category == 1)
            {
                return new T[0];
            }
            T[] localArray = new T[this._categoryActors[category].Count];
            for (int i = 0; i < localArray.Length; i++)
            {
                localArray[i] = this._categoryActors[category][i];
            }
            return localArray;
        }

        public T[] GetAlliedActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            return Singleton<LevelManager>.Instance.gameMode.GetAlliedActorsOf<T>(actor);
        }

        private string GetDebugEventString(BaseEvent evt)
        {
            string str = evt.ToString();
            if (evt.remoteState != EventRemoteState.Idle)
            {
                str = string.Format("[{0}]", evt.remoteState) + str;
            }
            if (evt is BaseLevelEvent)
            {
                str = "+++LevelEvent+++: " + str;
            }
            if (evt.parent != null)
            {
                str = str + " <---- " + evt.parent.GetType().ToString();
            }
            return str;
        }

        public T[] GetEnemyActorsOf<T>(BaseActor actor) where T: BaseActor
        {
            return Singleton<LevelManager>.Instance.gameMode.GetEnemyActorsOf<T>(actor);
        }

        public BaseMonoEntity GetEntity(uint runtimeID)
        {
            if (runtimeID == 0x21800001)
            {
                return Singleton<LevelManager>.Instance.levelEntity;
            }
            BaseMonoEntity entity = null;
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID))
            {
                case 2:
                    return Singleton<CameraManager>.Instance.GetCameraByRuntimeID(runtimeID);

                case 3:
                    return Singleton<AvatarManager>.Instance.TryGetAvatarByRuntimeID(runtimeID);

                case 4:
                    return Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);

                case 5:
                    return entity;

                case 6:
                    return Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(runtimeID);

                case 7:
                    return Singleton<PropObjectManager>.Instance.TryGetPropObjectByRuntimeID(runtimeID);
            }
            return entity;
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        protected void InjectEvent(BaseEvent evt)
        {
            this._queuedEvents.Add(evt);
        }

        public void MaskEventType(System.Type t)
        {
            this._maskedEvtTypes.Add(t);
        }

        protected virtual void ProcessInitedActor(BaseActor actor)
        {
        }

        public void RegisterEventListener<T>(uint id)
        {
            System.Type key = typeof(T);
            if (!this._evtListeners.ContainsKey(key))
            {
                this._evtListeners.Add(key, new List<ListenerRegistry>());
            }
            List<ListenerRegistry> list = this._evtListeners[key];
            bool flag = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].listenerID == id)
                {
                    ListenerRegistry local1 = list[i];
                    local1.registerCount++;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                list.Add(new ListenerRegistry(id));
            }
        }

        private void RemoveActor(BaseActor actor)
        {
            this._actorIdsToCleanUp.Add(actor.runtimeID);
            this._actorList.Remove(actor);
            this._actors.Remove(actor.runtimeID);
            ushort index = Singleton<RuntimeIDManager>.Instance.ParseCategory(actor.runtimeID);
            this._categoryActors[index].Remove(actor);
            Singleton<LevelManager>.Instance.gameMode.DestroyRuntimeID(actor.runtimeID);
        }

        private void RemoveAllListener(uint id)
        {
            foreach (List<ListenerRegistry> list in this._evtListeners.Values)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].listenerID == id)
                    {
                        list.RemoveAt(i);
                    }
                }
            }
        }

        public void RemoveEventListener<T>(uint id)
        {
            List<ListenerRegistry> list;
            System.Type key = typeof(T);
            this._evtListeners.TryGetValue(key, out list);
            if ((list != null) && (list.Count > 0))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ListenerRegistry item = list[i];
                    if (item.listenerID == id)
                    {
                        item.registerCount--;
                        if (item.registerCount == 0)
                        {
                            list.Remove(item);
                        }
                        return;
                    }
                }
            }
        }

        public void SetPauseDispatching(bool paused)
        {
            this._dispatchPaused = paused;
        }

        public void TryRemoveActor(uint runtimeID)
        {
            BaseActor actor;
            if (this._actors.TryGetValue(runtimeID, out actor))
            {
                actor.OnRemoval();
                this.RemoveActor(actor);
            }
        }

        public void UnmaskEventType(System.Type t)
        {
            this._maskedEvtTypes.Remove(t);
        }

        private class ListenerRegistry
        {
            public uint listenerID;
            public uint registerCount;

            public ListenerRegistry(uint listenerID)
            {
                this.listenerID = listenerID;
                this.registerCount = 1;
            }
        }
    }
}

