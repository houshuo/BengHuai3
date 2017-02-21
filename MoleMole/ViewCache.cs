namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ViewCache
    {
        [ShowInInspector]
        private List<ViewCacheEntry> _alwaysCache;
        [ShowInInspector]
        private SimpleLRU<ViewCacheEntry> _lruCache;

        public ViewCache(int lruCapacity)
        {
            this._lruCache = new SimpleLRU<ViewCacheEntry>(lruCapacity);
            this._alwaysCache = new List<ViewCacheEntry>();
        }

        public void ClearLRUCache()
        {
            for (int i = 0; i < this._lruCache.count; i++)
            {
                if (((this._lruCache[i] != null) && !string.IsNullOrEmpty(this._lruCache[i].viewPrefabPath)) && !this._lruCache[i].isInUse)
                {
                    UnityEngine.Object.DestroyImmediate(this._lruCache[i].instancedGameObject);
                    this._lruCache.MarkClear(i);
                }
            }
            this._lruCache.Rebuild();
        }

        private GameObject LoadAndInstantiateView(string path)
        {
            return UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(path, BundleType.RESOURCE_FILE));
        }

        public GameObject LoadInstancedView(ContextPattern config)
        {
            if (config.dontDestroyView || (config.cacheType == ViewCacheType.DontCache))
            {
                return this.LoadAndInstantiateView(config.viewPrefabPath);
            }
            bool flag = false;
            ViewCacheEntry item = null;
            for (int i = 0; i < this._lruCache.count; i++)
            {
                if ((this._lruCache[i] != null) && (this._lruCache[i].viewPrefabPath == config.viewPrefabPath))
                {
                    item = this._lruCache[i];
                    flag = true;
                    break;
                }
            }
            if (item == null)
            {
                for (int j = 0; j < this._alwaysCache.Count; j++)
                {
                    if (this._alwaysCache[j].viewPrefabPath == config.viewPrefabPath)
                    {
                        item = this._alwaysCache[j];
                        flag = true;
                        break;
                    }
                }
            }
            if (item == null)
            {
                GameObject obj2 = this.LoadAndInstantiateView(config.viewPrefabPath);
                item = new ViewCacheEntry {
                    viewPrefabPath = config.viewPrefabPath,
                    instancedGameObject = obj2,
                    isInUse = true
                };
            }
            else
            {
                if (item.isInUse)
                {
                    return this.LoadAndInstantiateView(config.viewPrefabPath);
                }
                item.isInUse = true;
                item.instancedGameObject.SetActive(true);
            }
            if (config.cacheType == ViewCacheType.AlwaysCached)
            {
                if (!flag)
                {
                    this._alwaysCache.Add(item);
                }
            }
            else if (config.cacheType == ViewCacheType.LRUCached)
            {
                ViewCacheEntry entry2;
                this._lruCache.Touch(item, out entry2);
                if (entry2 != null)
                {
                    UnityEngine.Object.Destroy(entry2.instancedGameObject);
                }
            }
            return item.instancedGameObject;
        }

        public void ReleaseInstancedView(GameObject view, ContextPattern config)
        {
            if (((view != null) && !string.IsNullOrEmpty(config.viewPrefabPath)) && !config.dontDestroyView)
            {
                if (config.cacheType == ViewCacheType.DontCache)
                {
                    UnityEngine.Object.Destroy(view);
                }
                else if (config.cacheType == ViewCacheType.LRUCached)
                {
                    for (int i = 0; i < this._lruCache.count; i++)
                    {
                        if (((this._lruCache[i] != null) && (this._lruCache[i].viewPrefabPath == config.viewPrefabPath)) && (this._lruCache[i].instancedGameObject == view))
                        {
                            this._lruCache[i].isInUse = false;
                            view.SetActive(false);
                            return;
                        }
                    }
                    UnityEngine.Object.Destroy(view);
                }
                else if (config.cacheType == ViewCacheType.AlwaysCached)
                {
                    for (int j = 0; j < this._alwaysCache.Count; j++)
                    {
                        if ((this._alwaysCache[j].viewPrefabPath == config.viewPrefabPath) && (this._alwaysCache[j].instancedGameObject == view))
                        {
                            this._alwaysCache[j].isInUse = false;
                            view.SetActive(false);
                            return;
                        }
                    }
                    UnityEngine.Object.Destroy(view);
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < this._lruCache.count; i++)
            {
                if (((this._lruCache[i] != null) && !this._lruCache[i].isInUse) && (this._lruCache[i].instancedGameObject != null))
                {
                    UnityEngine.Object.Destroy(this._lruCache[i].instancedGameObject);
                }
            }
            for (int j = 0; j < this._alwaysCache.Count; j++)
            {
                if (!this._alwaysCache[j].isInUse && (this._alwaysCache[j].instancedGameObject != null))
                {
                    UnityEngine.Object.Destroy(this._alwaysCache[j].instancedGameObject);
                }
            }
            this._lruCache.Clear();
            this._alwaysCache.Clear();
        }
    }
}

