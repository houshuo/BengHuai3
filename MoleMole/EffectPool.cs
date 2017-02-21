namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class EffectPool
    {
        private int _initialInstanceCount = 1;
        private Transform _instanceRoot;
        private List<PreloadGroupEntry> _preloadedEffectGroupNames = new List<PreloadGroupEntry>();
        private Dictionary<string, ProtoPoolItem> dictInstanceCache = new Dictionary<string, ProtoPoolItem>();
        private static int ProtoPoolItemMaxCount = 10;
        private const string ROOT_NAME = "EffectPool";

        public void CleanAll(bool includeCommon)
        {
            List<string> list = new List<string>(this.dictInstanceCache.Keys);
            foreach (string str in list)
            {
                ProtoPoolItem item = this.dictInstanceCache[str];
                if (includeCommon || !item.isCommon)
                {
                    int num = 0;
                    int count = item.availableList.Count;
                    while (num < count)
                    {
                        UnityEngine.Object.Destroy(item.availableList[num]);
                        num++;
                    }
                    item.availableList.Clear();
                    int num3 = 0;
                    int num4 = item.occupiedList.Count;
                    while (num3 < num4)
                    {
                        UnityEngine.Object.Destroy(item.occupiedList[num3]);
                        num3++;
                    }
                    item.occupiedList.Clear();
                    this.dictInstanceCache.Remove(str);
                }
            }
            if (includeCommon)
            {
                this.dictInstanceCache.Clear();
                if (this._instanceRoot != null)
                {
                    UnityEngine.Object.Destroy(this._instanceRoot.gameObject);
                    this._instanceRoot = null;
                }
                this._preloadedEffectGroupNames.Clear();
            }
            else
            {
                for (int i = 0; i < this._preloadedEffectGroupNames.Count; i++)
                {
                    PreloadGroupEntry entry = this._preloadedEffectGroupNames[i];
                    if (!entry.isCommon)
                    {
                        this._preloadedEffectGroupNames[i] = null;
                    }
                }
                this._preloadedEffectGroupNames.RemoveAllNulls<PreloadGroupEntry>();
            }
        }

        public void CleanByEffectName(string name)
        {
            ProtoPoolItem item;
            this.dictInstanceCache.TryGetValue(name, out item);
            if (item != null)
            {
                int num = 0;
                int count = item.availableList.Count;
                while (num < count)
                {
                    UnityEngine.Object.Destroy(item.availableList[num]);
                    num++;
                }
                int num3 = 0;
                int num4 = item.occupiedList.Count;
                while (num3 < num4)
                {
                    UnityEngine.Object.Destroy(item.occupiedList[num3]);
                    num3++;
                }
                this.dictInstanceCache.Remove(name);
            }
        }

        public void Despawn(GameObject inst)
        {
            inst.SetActive(false);
            if (this.dictInstanceCache.ContainsKey(inst.name))
            {
                ProtoPoolItem item = this.dictInstanceCache[inst.name];
                item.occupiedList.Remove(inst);
                item.availableList.Add(inst);
            }
        }

        public void DestroyGroupByName(string groupName)
        {
            if (EffectData.HasEffectPattern(groupName))
            {
                foreach (EffectPattern pattern in EffectData.GetEffectGroupPatterns(groupName))
                {
                    for (int i = 0; i < pattern.subEffects.Length; i++)
                    {
                        this.CleanByEffectName(Miscs.GetBaseName(pattern.subEffects[i].prefabPath));
                    }
                }
            }
        }

        public List<MonoEffect> GetEffectAliveEffect()
        {
            List<MonoEffect> result = new List<MonoEffect>();
            Transform effectPoolRoot = this.GetEffectPoolRoot();
            if (effectPoolRoot != null)
            {
                effectPoolRoot.GetComponentsInChildren<MonoEffect>(false, result);
            }
            return result;
        }

        private Transform GetEffectPoolRoot()
        {
            if (this._instanceRoot == null)
            {
                GameObject obj2 = GameObject.Find("EffectPool");
                if (obj2 != null)
                {
                    this._instanceRoot = obj2.transform;
                    return this._instanceRoot;
                }
                this._instanceRoot = new GameObject("EffectPool").transform;
                UnityEngine.Object.DontDestroyOnLoad(this._instanceRoot);
            }
            return this._instanceRoot;
        }

        private ProtoPoolItem GetPoolItem(GameObject proto)
        {
            if (!this.dictInstanceCache.ContainsKey(proto.name))
            {
                this.PreloadSingleEffect(proto, false);
            }
            return this.dictInstanceCache[proto.name];
        }

        public void PreloadGroup(string groupName, bool isCommon)
        {
            for (int i = 0; i < this._preloadedEffectGroupNames.Count; i++)
            {
                if (this._preloadedEffectGroupNames[i].groupName == groupName)
                {
                    PreloadGroupEntry local1 = this._preloadedEffectGroupNames[i];
                    local1.refCount++;
                    return;
                }
            }
            this._preloadedEffectGroupNames.Add(new PreloadGroupEntry(groupName, isCommon));
            if (EffectData.HasEffectPattern(groupName))
            {
                foreach (EffectPattern pattern in EffectData.GetEffectGroupPatterns(groupName))
                {
                    for (int j = 0; j < pattern.subEffects.Length; j++)
                    {
                        SubEffect effect = pattern.subEffects[j];
                        GameObject prototype = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(effect.prefabPath), BundleType.RESOURCE_FILE);
                        if (prototype != null)
                        {
                            Singleton<EffectManager>.Instance.AddEffectPrototype(effect.prefabPath, prototype);
                            this.PreloadSingleEffect(prototype, isCommon);
                        }
                    }
                }
            }
        }

        [DebuggerHidden]
        public IEnumerator PreloadGroupAsync(string groupName, bool isCommon)
        {
            return new <PreloadGroupAsync>c__Iterator24 { groupName = groupName, isCommon = isCommon, <$>groupName = groupName, <$>isCommon = isCommon, <>f__this = this };
        }

        private void PreloadSingleEffect(GameObject proto, bool isCommon)
        {
            if (!this.dictInstanceCache.ContainsKey(proto.name))
            {
                ProtoPoolItem item = new ProtoPoolItem {
                    isCommon = isCommon
                };
                this.dictInstanceCache.Add(proto.name, item);
                for (int i = 0; i < this._initialInstanceCount; i++)
                {
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(proto);
                    obj2.transform.parent = this.GetEffectPoolRoot();
                    obj2.name = proto.name;
                    obj2.SetActive(false);
                    item.availableList.Add(obj2);
                }
            }
        }

        public void ResetClearGroupRefCounts()
        {
            for (int i = 0; i < this._preloadedEffectGroupNames.Count; i++)
            {
                this._preloadedEffectGroupNames[i].refCount = 0;
            }
        }

        public void SetEnabled(bool enabled)
        {
            Transform effectPoolRoot = this.GetEffectPoolRoot();
            if (effectPoolRoot != null)
            {
                effectPoolRoot.gameObject.SetActive(enabled);
            }
        }

        public void SetInitialInstanceCount(int cnt)
        {
            this._initialInstanceCount = cnt;
        }

        public GameObject Spawn(GameObject proto)
        {
            GameObject obj2 = null;
            ProtoPoolItem poolItem = this.GetPoolItem(proto);
            if (!poolItem.ReachMaxCount())
            {
                if (poolItem.availableList.Count == 0)
                {
                    obj2 = UnityEngine.Object.Instantiate<GameObject>(proto);
                    obj2.name = proto.name;
                    obj2.transform.parent = this.GetEffectPoolRoot();
                }
                else
                {
                    obj2 = poolItem.availableList[0];
                    poolItem.availableList.RemoveAt(0);
                }
                poolItem.occupiedList.Add(obj2);
                obj2.SetActive(true);
            }
            return obj2;
        }

        public void UnloadGroup(string[] groupNames)
        {
            for (int i = 0; i < this._preloadedEffectGroupNames.Count; i++)
            {
                PreloadGroupEntry entry = this._preloadedEffectGroupNames[i];
                if (Miscs.ArrayContains<string>(groupNames, entry.groupName))
                {
                    entry.refCount--;
                    if (entry.refCount == 0)
                    {
                        this._preloadedEffectGroupNames[i] = null;
                        this.DestroyGroupByName(entry.groupName);
                    }
                }
            }
            this._preloadedEffectGroupNames.RemoveAllNulls<PreloadGroupEntry>();
        }

        public void UnloadNonRefedGroups()
        {
            for (int i = 0; i < this._preloadedEffectGroupNames.Count; i++)
            {
                PreloadGroupEntry entry = this._preloadedEffectGroupNames[i];
                if (entry.refCount == 0)
                {
                    this._preloadedEffectGroupNames[i] = null;
                    this.DestroyGroupByName(entry.groupName);
                }
            }
            this._preloadedEffectGroupNames.RemoveAllNulls<PreloadGroupEntry>();
        }

        [CompilerGenerated]
        private sealed class <PreloadGroupAsync>c__Iterator24 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal string <$>groupName;
            internal bool <$>isCommon;
            internal EffectPool <>f__this;
            internal EffectPattern <effectPattern>__3;
            internal EffectPattern[] <effectPatterns>__1;
            internal int <ix>__0;
            internal int <ix>__2;
            internal int <jx>__4;
            internal GameObject <proto>__6;
            internal SubEffect <subEffect>__5;
            internal string groupName;
            internal bool isCommon;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<ix>__0 = 0;
                        while (this.<ix>__0 < this.<>f__this._preloadedEffectGroupNames.Count)
                        {
                            if (this.<>f__this._preloadedEffectGroupNames[this.<ix>__0].groupName == this.groupName)
                            {
                                EffectPool.PreloadGroupEntry local1 = this.<>f__this._preloadedEffectGroupNames[this.<ix>__0];
                                local1.refCount++;
                                break;
                            }
                            this.<ix>__0++;
                        }
                        this.<>f__this._preloadedEffectGroupNames.Add(new EffectPool.PreloadGroupEntry(this.groupName, this.isCommon));
                        if (EffectData.HasEffectPattern(this.groupName))
                        {
                            this.<effectPatterns>__1 = EffectData.GetEffectGroupPatterns(this.groupName);
                            this.<ix>__2 = 0;
                            while (this.<ix>__2 < this.<effectPatterns>__1.Length)
                            {
                                this.<effectPattern>__3 = this.<effectPatterns>__1[this.<ix>__2];
                                this.<jx>__4 = 0;
                                while (this.<jx>__4 < this.<effectPattern>__3.subEffects.Length)
                                {
                                    this.<subEffect>__5 = this.<effectPattern>__3.subEffects[this.<jx>__4];
                                    this.<proto>__6 = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(this.<subEffect>__5.prefabPath), BundleType.RESOURCE_FILE);
                                    if (this.<proto>__6 != null)
                                    {
                                        Singleton<EffectManager>.Instance.AddEffectPrototype(this.<subEffect>__5.prefabPath, this.<proto>__6);
                                        this.<>f__this.PreloadSingleEffect(this.<proto>__6, this.isCommon);
                                        this.$current = null;
                                        this.$PC = 1;
                                        return true;
                                    }
                                Label_01AA:
                                    this.<jx>__4++;
                                }
                                this.<ix>__2++;
                            }
                            this.$PC = -1;
                        }
                        break;

                    case 1:
                        goto Label_01AA;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private class PreloadGroupEntry
        {
            public string groupName;
            public bool isCommon;
            public int refCount;

            public PreloadGroupEntry(string name, bool isCommon)
            {
                this.groupName = name;
                this.refCount = 1;
                this.isCommon = isCommon;
            }
        }

        private class ProtoPoolItem
        {
            public List<GameObject> availableList = new List<GameObject>();
            public bool isCommon;
            public List<GameObject> occupiedList = new List<GameObject>();

            public bool ReachMaxCount()
            {
                return ((this.availableList.Count == 0) && ((this.occupiedList.Count + this.availableList.Count) >= EffectPool.ProtoPoolItemMaxCount));
            }
        }
    }
}

