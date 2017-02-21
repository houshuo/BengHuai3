namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonsterManager
    {
        private Dictionary<uint, BaseMonoMonster> _monsterDict = new Dictionary<uint, BaseMonoMonster>();
        private List<BaseMonoMonster> _monsterLs = new List<BaseMonoMonster>();
        private List<PreloadMonsterItem> _preloadedMonsters = new List<PreloadMonsterItem>();

        private MonsterManager()
        {
        }

        public void Core()
        {
            this.RemoveAllRemoveableMonsters();
        }

        public uint CreateMonster(string monsterName, string typeName, int level, bool isLocal, Vector3 initPos, uint runtimeID, bool isElite = false, uint uniqueMonsterID = 0, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
        {
            BaseMonoMonster component = null;
            GameObject gameObj = null;
            if (uniqueMonsterID != 0)
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
                monsterName = uniqueMonsterMetaData.monsterName;
                typeName = uniqueMonsterMetaData.typeName;
            }
            string str = monsterName + typeName + uniqueMonsterID.ToString() + disableBehaviorWhenInit.ToString();
            int index = 0;
            int num2 = 0;
            int count = this._preloadedMonsters.Count;
            while (num2 < count)
            {
                if (this._preloadedMonsters[num2].name == str)
                {
                    gameObj = this._preloadedMonsters[num2].gameObj;
                    index = num2;
                    break;
                }
                num2++;
            }
            if (gameObj != null)
            {
                gameObj.SetActive(true);
                this._preloadedMonsters.RemoveAt(index);
            }
            else
            {
                gameObj = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(monsterName, typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi)), BundleType.RESOURCE_FILE), InLevelData.CREATE_INIT_POS, Quaternion.identity);
            }
            component = gameObj.GetComponent<BaseMonoMonster>();
            if (runtimeID == 0)
            {
                runtimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4);
            }
            bool flag = checkOutsideWall;
            component.Init(monsterName, typeName, runtimeID, initPos, uniqueMonsterID, null, flag, isElite, disableBehaviorWhenInit, tagID);
            this.RegisterMonster(component);
            MonsterActor actor = Singleton<EventManager>.Instance.CreateActor<MonsterActor>(component);
            actor.InitLevelData(level, isElite);
            actor.PostInit();
            int num4 = 0;
            int length = component.config.CommonArguments.RequestSoundBankNames.Length;
            while (num4 < length)
            {
                Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(component.config.CommonArguments.RequestSoundBankNames[num4]);
                num4++;
            }
            return component.GetRuntimeID();
        }

        public uint CreateMonsterMirror(BaseMonoMonster owner, Vector3 initPos, Vector3 initDir, string AIName, float hpRatio, bool disableBehaviorWhenInit = false)
        {
            BaseMonoMonster component = ((GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(owner.MonsterName, owner.TypeName, false), BundleType.RESOURCE_FILE), initPos, Quaternion.LookRotation(initDir))).GetComponent<BaseMonoMonster>();
            bool flag = disableBehaviorWhenInit;
            component.Init(owner.MonsterName, owner.TypeName, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), initPos, owner.uniqueMonsterID, AIName, true, false, flag, 0);
            this.RegisterMonster(component);
            MonsterMirrorActor actor = Singleton<EventManager>.Instance.CreateActor<MonsterMirrorActor>(component);
            actor.InitFromMonsterActor(Singleton<EventManager>.Instance.GetActor<MonsterActor>(owner.GetRuntimeID()), hpRatio);
            actor.PostInit();
            int index = 0;
            int length = component.config.CommonArguments.RequestSoundBankNames.Length;
            while (index < length)
            {
                Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(component.config.CommonArguments.RequestSoundBankNames[index]);
                index++;
            }
            return component.GetRuntimeID();
        }

        public void Destroy()
        {
            for (int i = 0; i < this._preloadedMonsters.Count; i++)
            {
                if ((this._preloadedMonsters[i] != null) && (this._preloadedMonsters[i].gameObj != null))
                {
                    UnityEngine.Object.DestroyImmediate(this._preloadedMonsters[i].gameObj);
                }
            }
            for (int j = 0; j < this._monsterLs.Count; j++)
            {
                if (this._monsterLs[j] != null)
                {
                    UnityEngine.Object.DestroyImmediate(this._monsterLs[j]);
                }
            }
        }

        public void DestroyUnOccupiedPreloadMonsters()
        {
            for (int i = 0; i < this._preloadedMonsters.Count; i++)
            {
                if (!this._preloadedMonsters[i].occupied)
                {
                    UnityEngine.Object.Destroy(this._preloadedMonsters[i].gameObj);
                    this._preloadedMonsters[i] = null;
                }
            }
            this._preloadedMonsters.RemoveAllNulls<PreloadMonsterItem>();
        }

        public List<BaseMonoMonster> GetAllMonsters()
        {
            return this._monsterLs;
        }

        public BaseMonoMonster GetMonsterByRuntimeID(uint runtimeID)
        {
            return this._monsterDict[runtimeID];
        }

        public List<PreloadMonsterItem> GetPreloadedMonsters()
        {
            return this._preloadedMonsters;
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        public void InitMonstersPos(Vector3 monsterOffset)
        {
            List<BaseMonoMonster> allMonsters = this.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                Transform transform = allMonsters[i].transform;
                transform.position += monsterOffset;
            }
        }

        public int LivingMonsterCount()
        {
            int num = 0;
            foreach (BaseMonoMonster monster in this._monsterDict.Values)
            {
                if (monster.IsActive())
                {
                    num++;
                }
            }
            return num;
        }

        public int MonsterCount()
        {
            return this._monsterDict.Count;
        }

        public void PreloadMonster(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
        {
            string name = monsterName + typeName + uniqueMonsterID.ToString() + disableBehaviorWhenInit.ToString();
            for (int i = 0; i < this._preloadedMonsters.Count; i++)
            {
                if ((this._preloadedMonsters[i].name == name) && !this._preloadedMonsters[i].occupied)
                {
                    this._preloadedMonsters[i].occupied = true;
                    return;
                }
            }
            GameObject gameObj = (GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(MonsterData.GetPrefabResPath(monsterName, typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi)), BundleType.RESOURCE_FILE), InLevelData.CREATE_INIT_POS, Quaternion.identity);
            BaseMonoMonster component = gameObj.GetComponent<BaseMonoMonster>();
            component.PreInit(monsterName, typeName, uniqueMonsterID, disableBehaviorWhenInit);
            gameObj.SetActive(false);
            PreloadMonsterItem item = new PreloadMonsterItem(name, gameObj, component.config);
            this._preloadedMonsters.Add(item);
            ConfigMonster monster2 = MonsterData.GetMonsterConfig(monsterName, typeName, string.Empty);
            for (int j = 0; j < monster2.CommonArguments.PreloadEffectPatternGroups.Length; j++)
            {
                Singleton<EffectManager>.Instance.PreloadEffectGroup(monster2.CommonArguments.PreloadEffectPatternGroups[j], false);
            }
            for (int k = 0; k < monster2.CommonArguments.RequestSoundBankNames.Length; k++)
            {
                Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(component.config.CommonArguments.RequestSoundBankNames[k]);
            }
        }

        [DebuggerHidden]
        public IEnumerator PreloadMonsterAsync(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
        {
            return new <PreloadMonsterAsync>c__Iterator3A { monsterName = monsterName, typeName = typeName, uniqueMonsterID = uniqueMonsterID, disableBehaviorWhenInit = disableBehaviorWhenInit, <$>monsterName = monsterName, <$>typeName = typeName, <$>uniqueMonsterID = uniqueMonsterID, <$>disableBehaviorWhenInit = disableBehaviorWhenInit, <>f__this = this };
        }

        [DebuggerHidden]
        public IEnumerator PreloadMonstersAsync(List<KeyValuePair<string, string>> monsters)
        {
            return new <PreloadMonstersAsync>c__Iterator3B { monsters = monsters, <$>monsters = monsters, <>f__this = this };
        }

        [DebuggerHidden]
        public IEnumerator PreloadUniqueMonstersAsync(List<uint> monsters)
        {
            return new <PreloadUniqueMonstersAsync>c__Iterator3C { monsters = monsters, <$>monsters = monsters, <>f__this = this };
        }

        public void RegisterMonster(BaseMonoMonster monster)
        {
            this._monsterDict.Add(monster.GetRuntimeID(), monster);
            this._monsterLs.Add(monster);
        }

        public void RemoveAllMonsters()
        {
            for (int i = 0; i < this._monsterLs.Count; i++)
            {
                BaseMonoMonster monster = this._monsterLs[i];
                if (!monster.IsToBeRemove())
                {
                    monster.SetDied(KillEffect.KillImmediately);
                }
                this.RemoveMonsterByRuntimeID(monster.GetRuntimeID(), i);
                i--;
            }
        }

        private void RemoveAllRemoveableMonsters()
        {
            for (int i = 0; i < this._monsterLs.Count; i++)
            {
                BaseMonoMonster monster = this._monsterLs[i];
                if (monster.IsToBeRemove())
                {
                    this.RemoveMonsterByRuntimeID(monster.GetRuntimeID(), i);
                    i--;
                }
            }
        }

        private bool RemoveMonsterByRuntimeID(uint runtimeID, int lsIx)
        {
            Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
            BaseMonoMonster monster = this._monsterDict[runtimeID];
            bool flag = this._monsterDict.Remove(runtimeID);
            this._monsterLs.RemoveAt(lsIx);
            UnityEngine.Object.Destroy(monster.gameObject);
            return flag;
        }

        public void SetPause(bool pause)
        {
            foreach (BaseMonoMonster monster in this._monsterDict.Values)
            {
                monster.SetPause(pause);
            }
        }

        public BaseMonoMonster TryGetMonsterByRuntimeID(uint runtimeID)
        {
            BaseMonoMonster monster;
            this._monsterDict.TryGetValue(runtimeID, out monster);
            return monster;
        }

        public void UnOccupyAllPreloadedMonsters()
        {
            for (int i = 0; i < this._preloadedMonsters.Count; i++)
            {
                this._preloadedMonsters[i].occupied = false;
            }
        }

        [CompilerGenerated]
        private sealed class <PreloadMonsterAsync>c__Iterator3A : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal bool <$>disableBehaviorWhenInit;
            internal string <$>monsterName;
            internal string <$>typeName;
            internal uint <$>uniqueMonsterID;
            internal MonsterManager <>f__this;
            internal MonsterManager.PreloadMonsterItem <item>__5;
            internal int <ix>__1;
            internal int <ix>__7;
            internal int <ix>__8;
            internal BaseMonoMonster <monster>__4;
            internal ConfigMonster <monsterConfig>__6;
            internal GameObject <monsterObj>__3;
            internal string <preloadKey>__0;
            internal AsyncAssetRequst <resReq>__2;
            internal bool disableBehaviorWhenInit;
            internal string monsterName;
            internal string typeName;
            internal uint uniqueMonsterID;

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
                        this.<preloadKey>__0 = this.monsterName + this.typeName + this.uniqueMonsterID.ToString() + this.disableBehaviorWhenInit.ToString();
                        this.<ix>__1 = 0;
                        while (this.<ix>__1 < this.<>f__this._preloadedMonsters.Count)
                        {
                            if ((this.<>f__this._preloadedMonsters[this.<ix>__1].name == this.<preloadKey>__0) && !this.<>f__this._preloadedMonsters[this.<ix>__1].occupied)
                            {
                                this.<>f__this._preloadedMonsters[this.<ix>__1].occupied = true;
                                break;
                            }
                            this.<ix>__1++;
                        }
                        this.<resReq>__2 = Miscs.LoadResourceAsync(MonsterData.GetPrefabResPath(this.monsterName, this.typeName, !GlobalVars.MONSTER_USE_DYNAMIC_BONE || (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Multi)), BundleType.RESOURCE_FILE);
                        this.$current = this.<resReq>__2.operation;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<monsterObj>__3 = (GameObject) UnityEngine.Object.Instantiate((GameObject) this.<resReq>__2.asset, InLevelData.CREATE_INIT_POS, Quaternion.identity);
                        this.<monster>__4 = this.<monsterObj>__3.GetComponent<BaseMonoMonster>();
                        this.<monster>__4.PreInit(this.monsterName, this.typeName, this.uniqueMonsterID, this.disableBehaviorWhenInit);
                        this.<monsterObj>__3.SetActive(false);
                        this.<item>__5 = new MonsterManager.PreloadMonsterItem(this.<preloadKey>__0, this.<monsterObj>__3, this.<monster>__4.config);
                        this.<>f__this._preloadedMonsters.Add(this.<item>__5);
                        this.<monsterConfig>__6 = MonsterData.GetMonsterConfig(this.monsterName, this.typeName, string.Empty);
                        this.<ix>__7 = 0;
                        while (this.<ix>__7 < this.<monsterConfig>__6.CommonArguments.PreloadEffectPatternGroups.Length)
                        {
                            Singleton<EffectManager>.Instance.PreloadEffectGroup(this.<monsterConfig>__6.CommonArguments.PreloadEffectPatternGroups[this.<ix>__7], false);
                            this.<ix>__7++;
                        }
                        this.<ix>__8 = 0;
                        while (this.<ix>__8 < this.<monsterConfig>__6.CommonArguments.RequestSoundBankNames.Length)
                        {
                            Singleton<WwiseAudioManager>.Instance.ManualPrepareBank(this.<monster>__4.config.CommonArguments.RequestSoundBankNames[this.<ix>__8]);
                            this.<ix>__8++;
                        }
                        this.$PC = -1;
                        break;
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

        [CompilerGenerated]
        private sealed class <PreloadMonstersAsync>c__Iterator3B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<KeyValuePair<string, string>> <$>monsters;
            internal MonsterManager <>f__this;
            internal int <i>__0;
            internal KeyValuePair<string, string> <keyValuePair>__1;
            internal string <monsterName>__2;
            internal string <typeName>__3;
            internal List<KeyValuePair<string, string>> monsters;

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
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    default:
                        goto Label_00CB;
                }
                if (this.<i>__0 < this.monsters.Count)
                {
                    this.<keyValuePair>__1 = this.monsters[this.<i>__0];
                    this.<monsterName>__2 = this.<keyValuePair>__1.Key;
                    this.<typeName>__3 = this.<keyValuePair>__1.Value;
                    this.$current = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.<>f__this.PreloadMonsterAsync(this.<monsterName>__2, this.<typeName>__3, 0, false));
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_00CB:
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

        [CompilerGenerated]
        private sealed class <PreloadUniqueMonstersAsync>c__Iterator3C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<uint> <$>monsters;
            internal MonsterManager <>f__this;
            internal int <i>__0;
            internal UniqueMonsterMetaData <metaData>__2;
            internal string <monsterName>__3;
            internal string <typeName>__4;
            internal uint <uniqueMonsterID>__1;
            internal List<uint> monsters;

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
                        this.<i>__0 = 0;
                        break;

                    case 1:
                        this.<i>__0++;
                        break;

                    default:
                        goto Label_00E1;
                }
                if (this.<i>__0 < this.monsters.Count)
                {
                    this.<uniqueMonsterID>__1 = this.monsters[this.<i>__0];
                    this.<metaData>__2 = MonsterData.GetUniqueMonsterMetaData(this.<uniqueMonsterID>__1);
                    this.<monsterName>__3 = this.<metaData>__2.monsterName;
                    this.<typeName>__4 = this.<metaData>__2.typeName;
                    this.$current = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.<>f__this.PreloadMonsterAsync(this.<monsterName>__3, this.<typeName>__4, this.<uniqueMonsterID>__1, false));
                    this.$PC = 1;
                    return true;
                }
                this.$PC = -1;
            Label_00E1:
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

        public class PreloadMonsterItem
        {
            public ConfigMonster config;
            public GameObject gameObj;
            public string name;
            public bool occupied;

            public PreloadMonsterItem(string name, GameObject gameObj, ConfigMonster config)
            {
                this.name = name;
                this.gameObj = gameObj;
                this.config = config;
                this.occupied = true;
            }
        }
    }
}

