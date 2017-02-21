namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class EffectManager
    {
        private Dictionary<uint, BaseMonoEffect> _effectDict = new Dictionary<uint, BaseMonoEffect>();
        private List<BaseMonoEffect> _effectLs = new List<BaseMonoEffect>();
        public EffectPool _effectPool;
        private Dictionary<string, GameObject> _effectPrefabCache = new Dictionary<string, GameObject>();
        private Transform _effectRootOutsidePool;
        private List<List<MonoEffect>> _indexedEffectPatterns = new List<List<MonoEffect>>();
        private HashSet<uint> _managedEffectSet = new HashSet<uint>();
        private Dictionary<string, int> _uniqueEffectPatternMap = new Dictionary<string, int>();
        private const string Effect_OUTSIDE_POOL_ROOT_NAME = "EffectOutsidePool";
        public bool mute;

        private EffectManager()
        {
        }

        public void AddEffectPrototype(string effectPath, GameObject prototype)
        {
            if (!this._effectPrefabCache.ContainsKey(effectPath))
            {
                this._effectPrefabCache.Add(effectPath, prototype);
            }
        }

        public void Clear()
        {
            this.RemoveAllEffects();
            this._effectPool.CleanAll(false);
            this.ClearInternal();
        }

        private void ClearEffectPrefabCache()
        {
            this._effectPrefabCache.Clear();
        }

        public void ClearEffectsByOwner(uint entityID)
        {
            foreach (MonoEffect effect in this._effectDict.Values)
            {
                if (((effect != null) && (effect.owner != null)) && ((effect.owner.GetRuntimeID() == entityID) && !this._managedEffectSet.Contains(effect.GetRuntimeID())))
                {
                    effect.SetDestroy();
                }
            }
        }

        public void ClearEffectsByOwnerImmediately(uint entityID)
        {
            foreach (MonoEffect effect in this._effectDict.Values)
            {
                if (((effect != null) && (effect.owner != null)) && ((effect.owner.GetRuntimeID() == entityID) && !this._managedEffectSet.Contains(effect.GetRuntimeID())))
                {
                    effect.SetDestroyImmediately();
                }
            }
        }

        private void ClearInternal()
        {
            this._effectDict.Clear();
            this._effectLs.Clear();
            this._managedEffectSet.Clear();
            this._effectPrefabCache.Clear();
            this._uniqueEffectPatternMap.Clear();
            this._indexedEffectPatterns.Clear();
            if (this._effectRootOutsidePool != null)
            {
                UnityEngine.Object.Destroy(this._effectRootOutsidePool.gameObject);
                this._effectRootOutsidePool = null;
            }
        }

        public void Core()
        {
            this.RemoveAllRemoveableEffects();
        }

        public BaseMonoEffect CreateEffectInstance(string effectPath, bool isLocal, Vector3 initPos, Vector3 faceDir, Vector3 initScale)
        {
            BaseMonoEffect item = null;
            GameObject obj2;
            if (this.mute)
            {
                return null;
            }
            this._effectPrefabCache.TryGetValue(effectPath, out obj2);
            if (obj2 == null)
            {
                obj2 = Miscs.LoadResource<GameObject>(EffectData.GetPrefabResPath(effectPath), BundleType.RESOURCE_FILE);
                this._effectPrefabCache.Add(effectPath, obj2);
            }
            bool isFromEffectPool = false;
            GameObject obj3 = null;
            if (this._effectPool != null)
            {
                GameObject obj4 = this._effectPool.Spawn(obj2);
                if (obj4 != null)
                {
                    obj3 = obj4;
                }
            }
            isFromEffectPool = obj3 != null;
            if (!isFromEffectPool)
            {
                obj3 = UnityEngine.Object.Instantiate<GameObject>(obj2);
                obj3.transform.parent = this.GetEffectRootOutsidePool();
                obj3.name = obj2.name;
            }
            item = obj3.GetComponent<BaseMonoEffect>();
            item.Init(effectPath, Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(5), initPos, faceDir, initScale, isFromEffectPool);
            item.Setup();
            if (item == null)
            {
                throw new Exception("Invalid Type or State!");
            }
            this._effectLs.Add(item);
            this._effectDict.Add(item.GetRuntimeID(), item);
            return item;
        }

        private BaseMonoEffect CreateEffectInstanceBySubEffectConfig(SubEffect subEffect, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
        {
            Vector3 vector = initPos;
            Vector3 vector2 = initDir;
            Vector3 vector3 = initScale;
            if (subEffect.onCreate != null)
            {
                subEffect.onCreate.Process(ref vector, ref vector2, ref vector3, entity);
            }
            return this.CreateEffectInstance(subEffect.prefabPath, true, vector, vector2, initScale);
        }

        public GameObject CreateGroupedEffectPattern(string patternName, BaseMonoEntity entity = null)
        {
            List<MonoEffect> list;
            if (entity == null)
            {
                entity = Singleton<LevelManager>.Instance.levelEntity;
            }
            GameObject obj2 = new GameObject(patternName) {
                transform = { position = InLevelData.CREATE_INIT_POS }
            };
            Vector3 position = obj2.transform.position;
            Vector3 forward = obj2.transform.forward;
            this.TriggerEntityEffectPatternRaw(patternName, position, forward, Vector3.one, entity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(entity);
                effect.SetupPlugin();
                effect.transform.parent = obj2.transform;
                this._managedEffectSet.Add(effect.GetRuntimeID());
            }
            return obj2;
        }

        public int CreateIndexedEntityEffectPattern(string patternName, BaseMonoEntity entity)
        {
            return this.CreateIndexedEntityEffectPattern(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
        }

        public int CreateIndexedEntityEffectPattern(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
        {
            List<MonoEffect> list;
            this.TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(entity);
                effect.SetupPlugin();
                this._managedEffectSet.Add(effect.GetRuntimeID());
            }
            int num2 = this._indexedEffectPatterns.SeekAddPosition<List<MonoEffect>>();
            this._indexedEffectPatterns[num2] = list;
            return num2;
        }

        public int CreateIndexedEntityEffectPatternWithOffset(string patternName, BaseMonoEntity entity, Vector3 offset)
        {
            return this.CreateIndexedEntityEffectPattern(patternName, entity.XZPosition + offset, entity.transform.forward, Vector3.one, entity);
        }

        public void CreateUniqueIndexedEffectPattern(string patternName, string uniqueKeyName, BaseMonoEntity entity)
        {
            int num = this.CreateIndexedEntityEffectPattern(patternName, entity);
            if (!this._uniqueEffectPatternMap.ContainsKey(uniqueKeyName))
            {
                this._uniqueEffectPatternMap.Add(uniqueKeyName, num);
            }
        }

        public void Destroy()
        {
            this.RemoveAllEffects();
            this.DisableEffectPool();
            this.ClearInternal();
        }

        public void DisableEffectPool()
        {
            if (this._effectPool != null)
            {
                this._effectPool.CleanAll(true);
                this._effectPool = null;
            }
        }

        public int EffectCount()
        {
            return this._effectDict.Count;
        }

        public void EnableEffectPool()
        {
            if (this._effectPool == null)
            {
                this._effectPool = new EffectPool();
                this._effectPool.SetInitialInstanceCount(1);
            }
        }

        public List<BaseMonoEffect> GetEffectList()
        {
            return this._effectLs;
        }

        private Transform GetEffectRootOutsidePool()
        {
            if (this._effectRootOutsidePool == null)
            {
                GameObject obj2 = GameObject.Find("EffectOutsidePool");
                if (obj2 != null)
                {
                    this._effectRootOutsidePool = obj2.transform;
                    return this._effectRootOutsidePool;
                }
                this._effectRootOutsidePool = new GameObject("EffectOutsidePool").transform;
            }
            return this._effectRootOutsidePool;
        }

        public List<MonoEffect> GetEffectsByOwner(uint entityID)
        {
            List<MonoEffect> list = new List<MonoEffect>();
            foreach (MonoEffect effect in this._effectDict.Values)
            {
                if ((effect.owner != null) && (effect.owner.GetRuntimeID() == entityID))
                {
                    list.Add(effect);
                }
            }
            return list;
        }

        public List<MonoEffect> GetIndexedEntityEffectPattern(int patternIx)
        {
            if (patternIx < this._indexedEffectPatterns.Count)
            {
                return this._indexedEffectPatterns[patternIx];
            }
            return null;
        }

        public void InitAtAwake()
        {
            this.EnableEffectPool();
        }

        public void InitAtStart()
        {
        }

        public bool IsEffectPoolEnabled()
        {
            return (this._effectPool != null);
        }

        private void PreloadCommonEffectGroups()
        {
            this.PreloadEffectGroup("InLevelUI_Effects", true);
            this.PreloadEffectGroup("Common_Effects", true);
            this.PreloadEffectGroup("Monster_Common_Effects", true);
            this.PreloadEffectGroup("Ability_Effects", true);
            this.PreloadEffectGroup("DynamicObject_Effects", true);
            this.PreloadEffectGroup("PropCommon_Effects", true);
        }

        public void PreloadEffectGroup(string effectGroupName, bool isCommon = false)
        {
            if (this._effectPool != null)
            {
                this._effectPool.PreloadGroup(effectGroupName, isCommon);
            }
        }

        public void PreloadEffectGroup(string[] effectGroupNames, bool isCommon = false)
        {
            if (this._effectPool != null)
            {
                for (int i = 0; i < effectGroupNames.Length; i++)
                {
                    this._effectPool.PreloadGroup(effectGroupNames[i], isCommon);
                }
            }
        }

        public void ReloadEffectPool()
        {
            if (this._effectPool != null)
            {
                this._effectPool.ResetClearGroupRefCounts();
                this.PreloadCommonEffectGroups();
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
                {
                    this.PreloadEffectGroup(avatar.config.CommonArguments.PreloadEffectPatternGroups, false);
                }
                foreach (MonsterManager.PreloadMonsterItem item in Singleton<MonsterManager>.Instance.GetPreloadedMonsters())
                {
                    this.PreloadEffectGroup(item.config.CommonArguments.PreloadEffectPatternGroups, false);
                }
                this._effectPool.UnloadNonRefedGroups();
            }
        }

        private void RemoveAllEffects()
        {
            for (int i = 0; i < this._effectLs.Count; i++)
            {
                this.RemoveEffectByRuntimeID(this._effectLs[i].GetRuntimeID(), i);
                i--;
            }
        }

        private void RemoveAllRemoveableEffects()
        {
            for (int i = 0; i < this._effectLs.Count; i++)
            {
                BaseMonoEffect effect = this._effectLs[i];
                if (effect.IsToBeRemove())
                {
                    this.RemoveEffectByRuntimeID(effect.GetRuntimeID(), i);
                    i--;
                }
            }
        }

        private bool RemoveEffectByRuntimeID(uint runtimeID, int lsIx)
        {
            BaseMonoEffect effect = this._effectDict[runtimeID];
            if (effect != null)
            {
                if ((this._effectPool != null) && effect.isFromEffectPool)
                {
                    this._effectPool.Despawn(effect.gameObject);
                }
                else
                {
                    UnityEngine.Object.Destroy(effect.gameObject);
                }
            }
            bool flag = this._effectDict.Remove(runtimeID);
            this._effectLs.RemoveAt(lsIx);
            return flag;
        }

        public void ResetEffectPatternPosition(int patternIx, Vector3 pos)
        {
            if (patternIx < this._indexedEffectPatterns.Count)
            {
                List<MonoEffect> list = this._indexedEffectPatterns[patternIx];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list[i].transform.position = pos;
                    }
                }
            }
        }

        public void SetAllAliveEffectPause(bool pause)
        {
            foreach (BaseMonoEffect effect in this.GetEffectList())
            {
                MonoEffect effect2 = effect as MonoEffect;
                if (effect2 != null)
                {
                    if (pause)
                    {
                        effect2.Pause();
                    }
                    else
                    {
                        effect2.Resume();
                    }
                }
            }
        }

        public void SetAllParticleSystemVisible(bool visible)
        {
            foreach (ParticleSystem system in UnityEngine.Object.FindObjectsOfType<ParticleSystem>())
            {
                system.emission.enabled = visible;
            }
        }

        public void SetDestroyImmediatelyIndexedEffectPattern(int patternIx)
        {
            if (patternIx < this._indexedEffectPatterns.Count)
            {
                List<MonoEffect> list = this._indexedEffectPatterns[patternIx];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list[i].SetDestroyImmediately();
                        this._managedEffectSet.Remove(list[i].GetRuntimeID());
                    }
                }
                this._indexedEffectPatterns[patternIx] = null;
            }
        }

        public void SetDestroyIndexedEffectPattern(int patternIx)
        {
            if (patternIx < this._indexedEffectPatterns.Count)
            {
                List<MonoEffect> list = this._indexedEffectPatterns[patternIx];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list[i].SetDestroy();
                        this._managedEffectSet.Remove(list[i].GetRuntimeID());
                    }
                }
                this._indexedEffectPatterns[patternIx] = null;
            }
        }

        public void SetDestroyUniqueIndexedEffectPattern(string uniqueKeyName)
        {
            int patternIx = this._uniqueEffectPatternMap[uniqueKeyName];
            this.SetDestroyIndexedEffectPattern(patternIx);
            this._uniqueEffectPatternMap.Remove(uniqueKeyName);
        }

        public void SetEffectPoolEnabled(bool enabled)
        {
            if (this._effectPool != null)
            {
                this._effectPool.SetEnabled(enabled);
            }
        }

        public void SetIndexedEntityEffectPatternActive(int patternIx, bool active)
        {
            if (patternIx < this._indexedEffectPatterns.Count)
            {
                List<MonoEffect> list = this._indexedEffectPatterns[patternIx];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        list[i].gameObject.SetActive(active);
                    }
                }
            }
        }

        public void TriggerEntityEffectPattern(string patternName, BaseMonoEntity entity, bool ignoreYPosition = true)
        {
            if (ignoreYPosition)
            {
                this.TriggerEntityEffectPattern(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
            }
            else
            {
                this.TriggerEntityEffectPattern(patternName, entity.transform.position, entity.transform.forward, Vector3.one, entity);
            }
        }

        public void TriggerEntityEffectPattern(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
        {
            List<MonoEffect> list;
            this.TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(entity);
                effect.SetupPlugin();
            }
        }

        public void TriggerEntityEffectPatternFromTo(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity fromEntity, BaseMonoEntity toEntity)
        {
            List<MonoEffect> list;
            this.TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, fromEntity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(fromEntity);
                effect.SetupPluginFromTo(toEntity);
            }
        }

        public void TriggerEntityEffectPatternRaw(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity, out List<MonoEffect> effects)
        {
            MonoEffectOverride component = entity.GetComponent<MonoEffectOverride>();
            if ((component != null) && component.effectOverrides.ContainsKey(patternName))
            {
                patternName = component.effectOverrides[patternName];
            }
            EffectPattern effectPattern = EffectData.GetEffectPattern(patternName);
            effects = new List<MonoEffect>();
            if (effectPattern.randomOneFromSubs)
            {
                int[] list = new int[effectPattern.subEffects.Length];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = i;
                }
                list.Shuffle<int>();
                for (int j = 0; j < list.Length; j++)
                {
                    if (((component == null) || string.IsNullOrEmpty(effectPattern.subEffects[j].predicate)) || component.effectPredicates.Contains(effectPattern.subEffects[j].predicate))
                    {
                        BaseMonoEffect effect = this.CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[j], initPos, initDir, initScale, entity);
                        if ((effect != null) && (effect is MonoEffect))
                        {
                            effects.Add((MonoEffect) effect);
                            break;
                        }
                    }
                }
            }
            else if (effectPattern.subEffects.Length == 1)
            {
                BaseMonoEffect effect2 = this.CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[0], initPos, initDir, initScale, entity);
                if ((effect2 != null) && (effect2 is MonoEffect))
                {
                    effects.Add((MonoEffect) effect2);
                }
            }
            else
            {
                for (int k = 0; k < effectPattern.subEffects.Length; k++)
                {
                    if (((component == null) || string.IsNullOrEmpty(effectPattern.subEffects[k].predicate)) || component.effectPredicates.Contains(effectPattern.subEffects[k].predicate))
                    {
                        BaseMonoEffect effect3 = this.CreateEffectInstanceBySubEffectConfig(effectPattern.subEffects[k], initPos, initDir, initScale, entity);
                        if ((effect3 != null) && (effect3 is MonoEffect))
                        {
                            effects.Add((MonoEffect) effect3);
                        }
                    }
                }
            }
        }

        public List<MonoEffect> TriggerEntityEffectPatternReturnValue(string patternName, BaseMonoEntity entity, bool ignoreYPosition = true)
        {
            if (ignoreYPosition)
            {
                return this.TriggerEntityEffectPatternReturnValue(patternName, entity.XZPosition, entity.transform.forward, Vector3.one, entity);
            }
            return this.TriggerEntityEffectPatternReturnValue(patternName, entity.transform.position, entity.transform.forward, Vector3.one, entity);
        }

        public List<MonoEffect> TriggerEntityEffectPatternReturnValue(string patternName, Vector3 initPos, Vector3 initDir, Vector3 initScale, BaseMonoEntity entity)
        {
            List<MonoEffect> list;
            this.TriggerEntityEffectPatternRaw(patternName, initPos, initDir, initScale, entity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(entity);
                effect.SetupPlugin();
            }
            return list;
        }

        public bool TrySetDestroyUniqueIndexedEffectPattern(string uniqueKeyName)
        {
            if (!this._uniqueEffectPatternMap.ContainsKey(uniqueKeyName))
            {
                return false;
            }
            this.SetDestroyUniqueIndexedEffectPattern(uniqueKeyName);
            return true;
        }

        public void UnloadEffectGroups(string[] effectGroupNames)
        {
            if (this._effectPool != null)
            {
                this._effectPool.UnloadGroup(effectGroupNames);
            }
        }
    }
}

