namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using CinemaDirector;
    using LuaInterface;
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public class LevelDesignManager
    {
        private List<BaseLDEvent> _allLDEvents;
        private List<EvtTrigger> _evtTriggers;
        private List<LDEvtTrigger> _ldEvtTriggers;
        private LuaFunction _ldMain;
        private List<Tuple<LuaThread, int>> _luaManualCoroutines;
        private LuaState _luaState;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map5;
        private MonoSpawnPoint centerPoint;
        private static string OVERRIDE_NAME = "OverrideName";

        public void AddBGMByName(string bgmName)
        {
        }

        private void AddGalTouchGoodFeel()
        {
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            int num = 0;
            int count = allPlayerAvatars.Count;
            while (num < count)
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(allPlayerAvatars[num].GetRuntimeID());
                if (actor != null)
                {
                    int avatarID = actor.avatarDataItem.avatarID;
                    int amount = GalTouchData.QueryBattleGain(Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel(avatarID));
                    Singleton<GalTouchModule>.Instance.IncreaseBattleGoodFeel(avatarID, amount);
                }
                num++;
            }
        }

        public void AddHintArrowForPath(string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.AddHintArrowForPath(spawnPoint);
        }

        public void AddLevelDefendModeData(int uniqueID, bool isKey)
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
            {
                LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
                if (plugin != null)
                {
                    plugin.AddModeData(uniqueID, isKey);
                }
            }
        }

        public void AddLevelDefendModeData(string modeType, int modeValue, bool isKey)
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
            {
                DefendModeType type = (DefendModeType) ((int) Enum.Parse(typeof(DefendModeType), modeType));
                LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
                if (plugin != null)
                {
                    plugin.AddModeData(type, modeValue, isKey);
                }
            }
        }

        public void AddLevelDefendModeData(string modeType, int targetModeValue, int currentModeValue, bool isKey)
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
            {
                DefendModeType type = (DefendModeType) ((int) Enum.Parse(typeof(DefendModeType), modeType));
                LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
                if (plugin != null)
                {
                    plugin.AddModeData(type, targetModeValue, currentModeValue, isKey);
                }
            }
        }

        public void AddTheRemainTimeInLevel(float timeDelta)
        {
            LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
            if (plugin != null)
            {
                plugin.AddRemainTime(timeDelta);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowAddTimeText, timeDelta));
            }
        }

        public bool AllowEnterStoryMode(int plotID)
        {
            PlotMetaData plotMetaDataByKey = PlotMetaDataReader.GetPlotMetaDataByKey(plotID);
            if (plotMetaDataByKey != null)
            {
                if (!Singleton<LevelModule>.Instance.ContainLevelById(plotMetaDataByKey.levelID))
                {
                    return true;
                }
                LevelDataItem item = Singleton<LevelModule>.Instance.TryGetLevelById(plotMetaDataByKey.levelID);
                if (item != null)
                {
                    return (item.LevelType == 1);
                }
            }
            return true;
        }

        public bool AllowSkipVideo(int cgId)
        {
            return Singleton<CGModule>.Instance.GetFinishedCGIDList().Contains(cgId);
        }

        public void BattleBegin()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.BattleBegin, null));
        }

        public void CleanWhenStageChange()
        {
            this.ClearAllEventsAndTriggers();
        }

        private void ClearAllEventsAndTriggers()
        {
            this._ldEvtTriggers.Clear();
            this._evtTriggers.Clear();
            for (int i = 0; i < this._allLDEvents.Count; i++)
            {
                this._allLDEvents[i].Dispose();
            }
            this._allLDEvents.Clear();
        }

        public void ClearAllMonsters()
        {
            foreach (MonsterActor actor in Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4))
            {
                if (!actor.monster.isStaticInScene)
                {
                    actor.ForceRemoveImmediatelly();
                }
            }
        }

        public void ClearLevelCombo()
        {
            Singleton<LevelManager>.Instance.levelActor.ResetCombo();
        }

        private void ClearLuaCoroutines()
        {
            for (int i = 0; i < this._luaManualCoroutines.Count; i++)
            {
                Tuple<LuaThread, int> tuple = this._luaManualCoroutines[i];
                tuple.Item1.Dispose();
                Singleton<ApplicationManager>.Instance.StopCoroutineManual(tuple.Item2);
            }
            this._luaManualCoroutines.Clear();
        }

        public void ClearPalsyBombProps()
        {
            foreach (BaseMonoPropObject obj2 in Singleton<PropObjectManager>.Instance.GetAllPropObjects())
            {
                if (obj2 is MonoPalsyBombProp)
                {
                    ((MonoPalsyBombProp) obj2).SetDied(KillEffect.KillImmediately);
                }
            }
        }

        public void CollectAllGoods()
        {
            foreach (MonoGoods goods in Singleton<DynamicObjectManager>.Instance.GetAllMonoGoods())
            {
                if (goods.IsActive())
                {
                    goods.forceFlyToAvatar = true;
                    goods.muteSound = true;
                    goods.state = MonoGoods.GoodsState.Attract;
                    goods.SetAttractTimerActive(true);
                    if (!string.IsNullOrEmpty(goods.AttachEffectPattern) && (Singleton<EventManager>.Instance.GetActor<EquipItemActor>(goods.GetRuntimeID()) != null))
                    {
                        foreach (MonoEffect effect in goods.OutsideEffects)
                        {
                            if (effect != null)
                            {
                                effect.SetDestroyImmediately();
                            }
                        }
                    }
                }
            }
        }

        private int CompareByDistance(MonoSpawnPoint pointA, MonoSpawnPoint pointB)
        {
            if (Vector3.Distance(pointA.transform.localPosition, this.centerPoint.transform.localPosition) < Vector3.Distance(pointB.transform.localPosition, this.centerPoint.transform.localPosition))
            {
                return -1;
            }
            return 1;
        }

        public void ControlLevelDamageStastics(string type)
        {
            string str = "DamageStastics";
            DamageStastcisControlType type2 = (DamageStastcisControlType) ((int) Enum.Parse(typeof(DamageStastcisControlType), str + type));
            Singleton<LevelManager>.Instance.levelActor.ControlLevelDamageStastics(type2);
        }

        public void Core()
        {
            if (this.state != LDState.End)
            {
                for (int i = 0; i < this._allLDEvents.Count; i++)
                {
                    this._allLDEvents[i].Core();
                    if (this._allLDEvents[i].isDone)
                    {
                        this._allLDEvents.RemoveAt(i);
                        i--;
                    }
                }
                for (int j = 0; j < this._ldEvtTriggers.Count; j++)
                {
                    if (this._ldEvtTriggers[j].ldEvent.isDone)
                    {
                        if (this._ldEvtTriggers[j].isCallbackCoroutine)
                        {
                            this.MakeAndStartCoroutine(this._ldEvtTriggers[j].callback);
                        }
                        else
                        {
                            this._ldEvtTriggers[j].callback.Call(new object[0]);
                        }
                        if (this._ldEvtTriggers.Count == 0)
                        {
                            break;
                        }
                        this._ldEvtTriggers[j].ldEvent.Dispose();
                        this._ldEvtTriggers.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        public uint CreateBarrier(string type, float length, string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            return Singleton<DynamicObjectManager>.Instance.CreateBarrierField(0x21800001, type, spawnPoint.transform.position, spawnPoint.transform.forward, length);
        }

        public void CreateCoroutine(LuaFunction luaFunc)
        {
            this.MakeAndStartCoroutine(luaFunc);
        }

        public void CreateDropGoods(LuaTable dropTable, string spawnName, bool actDropAnim = false)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            Vector3 position = spawnPoint.transform.position;
            position.y = 0.4f;
            IEnumerator enumerator = dropTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    (current as LDDropDataItem).CreateDropGoods(position, spawnPoint.transform.forward, actDropAnim);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public void CreateDropGoodsBetweenTwoPoint(LuaTable dropTable, string spawnName1, string spawnName2, int dropNum, bool actDropAnim = false)
        {
            Vector3 position = this.GetSpawnPoint(spawnName1).transform.position;
            Vector3 vector2 = this.GetSpawnPoint(spawnName2).transform.position;
            for (int i = 0; i < dropNum; i++)
            {
                IEnumerator enumerator = dropTable.Values.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object current = enumerator.Current;
                        (current as LDDropDataItem).CreateDropGoods(position + ((Vector3) (((vector2 - position) * i) / ((float) (dropNum - 1)))), Vector3.forward, actDropAnim);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
        }

        public void CreateDropGoodsByMonsterID(LuaTable dropTable, uint monsterID, bool actDropAnim = true)
        {
            Vector3 dropPosition = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID).GetDropPosition();
            IEnumerator enumerator = dropTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    (current as LDDropDataItem).CreateDropGoods(dropPosition, Vector3.forward, actDropAnim);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public void CreateEffectPatternWithMapKey(string effectPattern, string effectListMapKey)
        {
            Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(effectPattern, effectListMapKey, Singleton<LevelManager>.Instance.levelEntity);
        }

        public uint CreateFireProp(string spawnName, int numberX, int numberZ, float attack, float rotation)
        {
            return this.CreateUnitFieldProp(spawnName, numberX, numberZ, "Trap_Fire", attack, rotation);
        }

        public void CreateGood(string goodType, string spawnName, string abilityname, float argument = 1, bool actDropAnim = false)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            Singleton<DynamicObjectManager>.Instance.CreateGood(0x21800001, goodType, abilityname, argument, spawnPoint.transform.position, spawnPoint.transform.forward, actDropAnim, false);
        }

        public bool CreateHelperAvatar()
        {
            if (Singleton<LevelScoreManager>.Instance.friendDetailItem != null)
            {
                Singleton<AvatarManager>.Instance.ShowHelperAvater();
                return true;
            }
            return false;
        }

        public BaseLDEvent CreateLDEventFromTable(LuaTable luaTable)
        {
            object[] array = new object[luaTable.Values.Count];
            luaTable.Values.CopyTo(array, 0);
            List<object> list = new List<object>(array);
            System.Type type = System.Type.GetType("MoleMole." + ((string) list[0]));
            list.RemoveAt(0);
            BaseLDEvent item = (BaseLDEvent) Activator.CreateInstance(type, list.ToArray());
            this._allLDEvents.Add(item);
            return item;
        }

        public uint CreateMonster(string monsterName, string typeName, int level, Vector3 spawnPos, bool isElite, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward = 0, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
        {
            bool flag = checkOutsideWall;
            uint monsterID = Singleton<MonsterManager>.Instance.CreateMonster(monsterName, typeName, level, true, spawnPos, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), isElite, 0, flag, disableBehaviorWhenInit, tagID);
            return this.CreateMonsterInternal(monsterID, skillConfigsTable, dropTable, avatarExpReward);
        }

        public uint CreateMonsterExitField(string spawnName, bool forDefendMode)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            return Singleton<DynamicObjectManager>.Instance.CreateMonsterExitField(0x21800001, spawnPoint.transform.position, spawnPoint.transform.forward, forDefendMode);
        }

        private uint CreateMonsterInternal(uint monsterID, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterID);
            if (skillConfigsTable != null)
            {
                IDictionaryEnumerator enumerator = skillConfigsTable.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ConfigAbility abilityConfig;
                        DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                        string key = (string) current.Key;
                        LuaTable table = (LuaTable) current.Value;
                        string overrideName = (string) table[OVERRIDE_NAME];
                        if (overrideName == null)
                        {
                            abilityConfig = AbilityData.GetAbilityConfig(key);
                        }
                        else
                        {
                            abilityConfig = AbilityData.GetAbilityConfig(key, overrideName);
                        }
                        Dictionary<string, object> overrideMap = new Dictionary<string, object>();
                        IDictionaryEnumerator enumerator2 = table.GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                DictionaryEntry entry2 = (DictionaryEntry) enumerator2.Current;
                                string str3 = (string) entry2.Key;
                                if (str3 != OVERRIDE_NAME)
                                {
                                    if (entry2.Value is double)
                                    {
                                        overrideMap.Add(str3, (float) ((double) entry2.Value));
                                    }
                                    else if (entry2.Value is string)
                                    {
                                        overrideMap.Add(str3, (string) entry2.Value);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            IDisposable disposable = enumerator2 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        actor.abilityPlugin.AddAbility(abilityConfig, overrideMap);
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator as IDisposable;
                    if (disposable2 == null)
                    {
                    }
                    disposable2.Dispose();
                }
            }
            BaseMonoMonster monster = actor.monster;
            Vector3 vector = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - monster.XZPosition;
            monster.transform.forward = vector;
            List<LDDropDataItem> list = new List<LDDropDataItem>();
            IEnumerator enumerator3 = dropTable.Values.GetEnumerator();
            try
            {
                while (enumerator3.MoveNext())
                {
                    object obj2 = enumerator3.Current;
                    list.Add(obj2 as LDDropDataItem);
                }
            }
            finally
            {
                IDisposable disposable3 = enumerator3 as IDisposable;
                if (disposable3 == null)
                {
                }
                disposable3.Dispose();
            }
            actor.dropDataItems = list;
            actor.avatarExpReward = !float.IsNaN(avatarExpReward) ? Mathf.Max(0f, avatarExpReward) : 0f;
            return monsterID;
        }

        public uint CreateNavigationArrow(string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            Vector3 position = spawnPoint.transform.position;
            position.y += 0.1f;
            Vector3 forward = -spawnPoint.transform.forward;
            return Singleton<DynamicObjectManager>.Instance.CreateNavigationArrow(0x21800001, position, forward);
        }

        public uint CreateNavigationArrow(string spawnName_start, string spawnName_end)
        {
            Vector3 position = this.GetSpawnPoint(spawnName_start).transform.position;
            position.y += 0.1f;
            Vector3 vector4 = this.GetSpawnPoint(spawnName_end).transform.position - position;
            Vector3 forward = -vector4.normalized;
            return Singleton<DynamicObjectManager>.Instance.CreateNavigationArrow(0x21800001, position, forward);
        }

        public uint CreatePalsyBombProp(string spawnName)
        {
            return this.CreatePropObject(spawnName, "Trap_Palsy_Bomb");
        }

        public uint CreatePalsyProp(string spawnName)
        {
            return this.CreatePropObject(spawnName, "Trap_Palsy");
        }

        public uint CreatePropObject(string spawnName, string propName)
        {
            ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
            float hP = propObjectConfig.PropArguments.HP;
            float attack = propObjectConfig.PropArguments.Attack;
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            return Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, propName, hP, attack, spawnPoint.transform.position, spawnPoint.transform.forward, false);
        }

        public uint CreatePropObject(string propName, string spawnName, LuaTable dropTable, float hp, float attack, bool appearAnim = false)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            uint propID = Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, propName, hp, attack, spawnPoint.transform.position, spawnPoint.transform.forward, appearAnim);
            this.InitPropObject(propID, dropTable);
            return propID;
        }

        public uint CreateSlowProp(string spawnName, int numberX, int numberZ, float rotation = 0f)
        {
            return this.CreateUnitFieldProp(spawnName, numberX, numberZ, "Trap_Slow", 0f, rotation);
        }

        public uint CreateSpikeProp(string spawnName, int number, float attack, float rotation)
        {
            return this.CreateUnitFieldProp(spawnName, 1, number, "Trap_Spike", attack, rotation);
        }

        public void CreateStage(string stageTypeName, LuaTable spawnNames, string baseWeatherName, bool continued)
        {
            List<string> avatarSpawnNameList = new List<string>();
            IEnumerator enumerator = spawnNames.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    avatarSpawnNameList.Add(current as string);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            Singleton<StageManager>.Instance.CreateStage(stageTypeName, avatarSpawnNameList, baseWeatherName, continued);
        }

        public uint CreateStageExitField(string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            return Singleton<DynamicObjectManager>.Instance.CreateStageExitField(0x21800001, spawnPoint.transform.position, spawnPoint.transform.forward);
        }

        public uint CreateUniqueMonster(uint uniqueMonsterID, int level, Vector3 spawnPos, bool isElite, LuaTable skillConfigsTable, LuaTable dropTable, float avatarExpReward = 0, bool checkOutsideWall = true, bool disableBehaviorWhenInit = false, int tagID = 0)
        {
            UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
            string monsterName = uniqueMonsterMetaData.monsterName;
            string typeName = uniqueMonsterMetaData.typeName;
            uint monsterID = Singleton<MonsterManager>.Instance.CreateMonster(monsterName, typeName, level, true, spawnPos, Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), isElite, uniqueMonsterID, checkOutsideWall, disableBehaviorWhenInit, tagID);
            return this.CreateMonsterInternal(monsterID, skillConfigsTable, dropTable, avatarExpReward);
        }

        private uint CreateUnitFieldProp(string spawnName, int numberX, int numberZ, string propName, float rotation = 0f)
        {
            float attack = PropObjectData.GetPropObjectConfig(propName).PropArguments.Attack;
            return this.CreateUnitFieldProp(spawnName, numberX, numberZ, propName, attack, rotation);
        }

        private uint CreateUnitFieldProp(string spawnName, int numberX, int numberZ, string propName, float atk, float rotation = 0f)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            float hP = PropObjectData.GetPropObjectConfig(propName).PropArguments.HP;
            float attack = atk;
            uint runtimeID = Singleton<PropObjectManager>.Instance.CreatePropObject(0x21800001, propName, hP, attack, spawnPoint.transform.position, spawnPoint.transform.forward, false);
            MonoTriggerUnitFieldProp propObjectByRuntimeID = (MonoTriggerUnitFieldProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
            propObjectByRuntimeID.InitUnitFieldPropRange(numberX, numberZ);
            propObjectByRuntimeID.transform.RotateAround(propObjectByRuntimeID.transform.position, Vector3.up, rotation);
            propObjectByRuntimeID.EnableProp();
            return runtimeID;
        }

        public void DebugDestroyDynamicObject(uint objectID)
        {
            BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(objectID);
            dynamicObjectByRuntimeID.SetDied();
            dynamicObjectByRuntimeID.gameObject.SetActive(false);
        }

        public int DebugGetLocalAvatarLevel()
        {
            return (int) Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.GetLocalAvatarID()).level;
        }

        public int DebugGetLocalAvatarWeaponRarity()
        {
            return Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.GetLocalAvatarID()).avatarDataItem.GetWeapon().rarity;
        }

        public void DebugSetAvatarAutoBattleBehavior(uint runtimeID)
        {
            (Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID).GetActiveAIController() as BTreeAvatarAIController).ChangeToAutoBattleBehavior();
        }

        public void DebugSetAvatarAutoMoveBehavior(uint runtimeID, string spawnPoint)
        {
            (Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID).GetActiveAIController() as BTreeAvatarAIController).ChangeToMoveBehavior(this.GetSpawnPointPos(spawnPoint));
        }

        public void DebugSetAvatarAutoMoveBehaviorWithPosition(uint runtimeID, Vector3 position)
        {
            (Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID).GetActiveAIController() as BTreeAvatarAIController).ChangeToMoveBehavior(position);
        }

        public void DebugSetAvatarSupporterBehavior(uint runtimeID)
        {
            (Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID).GetActiveAIController() as BTreeAvatarAIController).ChangeToSupporterBehavior();
        }

        public void DebugSetBehaviorTree(uint runtimeID, string treeAssetPath)
        {
            BehaviorDesigner.Runtime.BehaviorTree component = Singleton<EventManager>.Instance.GetEntity(runtimeID).GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            if (component != null)
            {
                ExternalBehaviorTree tree2 = Miscs.LoadResource<ExternalBehaviorTree>(treeAssetPath, BundleType.RESOURCE_FILE);
                component.ExternalBehavior = tree2;
                component.EnableBehavior();
            }
        }

        public void DebugSetLocalAvatar(string avatarTypeName, LuaTable abilities, int weaponID, int level = 1, int star = 0)
        {
            Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.DebugSetLocalAvatarIter(avatarTypeName, abilities, weaponID, level, star));
        }

        public void DebugSetLocalAvatarByAvatarModule(string avatarTypeName, LuaTable abilities)
        {
            foreach (AvatarDataItem item in Singleton<AvatarModule>.Instance.UserAvatarList)
            {
                if (item.AvatarRegistryKey == avatarTypeName)
                {
                    AvatarDataItem avatarDataItem = item.Clone();
                    Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.DebugSetLocalAvatarIter(avatarDataItem, abilities));
                    return;
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator DebugSetLocalAvatarIter(AvatarDataItem avatarDataItem, LuaTable abilities)
        {
            return new <DebugSetLocalAvatarIter>c__Iterator37 { avatarDataItem = avatarDataItem, abilities = abilities, <$>avatarDataItem = avatarDataItem, <$>abilities = abilities };
        }

        [DebuggerHidden]
        private IEnumerator DebugSetLocalAvatarIter(string avatarTypeName, LuaTable abilities, int weaponID, int level, int star)
        {
            return new <DebugSetLocalAvatarIter>c__Iterator36 { avatarTypeName = avatarTypeName, level = level, star = star, weaponID = weaponID, abilities = abilities, <$>avatarTypeName = avatarTypeName, <$>level = level, <$>star = star, <$>weaponID = weaponID, <$>abilities = abilities };
        }

        public void DebugShowLevelDisplayText(string text, LuaTable paramTable = null)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, text));
        }

        public void Destroy()
        {
            this.StopLevelDesign();
            this._luaState.Dispose();
        }

        public void DestroyHelperAvatar()
        {
            Singleton<AvatarManager>.Instance.HideHelperAvatar(false);
        }

        public void DisableAllMonsterRootMotionAndAI()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                BaseMonoMonster monster = allMonsters[i];
                if (monster != null)
                {
                    monster.SetNeedOverrideVelocity(true);
                    monster.SetUseAIController(false);
                    MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monster.GetRuntimeID());
                    actor.config.CommonArguments.BePushedSpeedRatio = 0f;
                    actor.config.CommonArguments.BePushedSpeedRatioThrow = 0f;
                }
            }
        }

        public void DisableAvatarRootMotion()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar != null)
            {
                localAvatar.SetNeedOverrideVelocity(true);
            }
        }

        public void DisableBossCamera()
        {
            Singleton<CameraManager>.Instance.DisableBossCamera();
        }

        public void DisableCrowdCamera()
        {
            Singleton<CameraManager>.Instance.DisableCrowdCamera();
        }

        public void DisableFireProp(uint runtimeID)
        {
            ((MonoFireTriggerProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID)).ForceDisableFire();
        }

        public void DisableMonsterRootMotionAndAI(uint runtimeID)
        {
            BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
            if (monster != null)
            {
                monster.SetNeedOverrideVelocity(true);
                monster.SetUseAIController(false);
                MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monster.GetRuntimeID());
                actor.config.CommonArguments.BePushedSpeedRatio = 0f;
                actor.config.CommonArguments.BePushedSpeedRatioThrow = 0f;
            }
        }

        public void DispatchLevelDesignListenEvent(BaseEvent evt)
        {
            for (int i = 0; i < this._allLDEvents.Count; i++)
            {
                this._allLDEvents[i].OnEvent(evt);
            }
            System.Type type = evt.GetType();
            for (int j = 0; j < this._evtTriggers.Count; j++)
            {
                if ((this._evtTriggers[j].evtType == type) && !this._evtTriggers[j].isCallbackCoroutine)
                {
                    object[] args = new object[] { evt };
                    this._evtTriggers[j].callback.Call(args);
                }
            }
        }

        public void DoTutorialStep(int tutorialStepID, bool toPauseGame, float holdSeconds = 0)
        {
            <DoTutorialStep>c__AnonStoreyC3 yc = new <DoTutorialStep>c__AnonStoreyC3 {
                holdSeconds = holdSeconds,
                toPauseGame = toPauseGame
            };
            TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
            string targetUIPath = tutorialStepDataByKey.targetUIPath;
            Transform transform = null;
            if (targetUIPath != string.Empty)
            {
                transform = Singleton<MainUIManager>.Instance.SceneCanvas.transform.FindChild(targetUIPath);
            }
            NewbieDialogContext dialogContext = new NewbieDialogContext {
                destroyByOthers = true,
                delayInputTime = 0.5f,
                disableMask = tutorialStepDataByKey.stepType == 2,
                highlightTrans = transform,
                highlightPath = targetUIPath,
                bubblePosType = (NewbieDialogContext.BubblePosType) tutorialStepDataByKey.bubblePosType,
                handIconPosType = (NewbieDialogContext.HandIconPosType) tutorialStepDataByKey.handIconPosType,
                disableHighlightEffect = !tutorialStepDataByKey.playEffect,
                guideDesc = LocalizationGeneralLogic.GetText(tutorialStepDataByKey.guideDesc, new object[0]),
                delayShowTime = tutorialStepDataByKey.delayTime
            };
            if (yc.toPauseGame && !Singleton<LevelManager>.Instance.IsPaused())
            {
                Singleton<LevelManager>.Instance.SetPause(true);
            }
            yc.pointerDownTime = TimeUtil.Now;
            dialogContext.pointerDownCallback = new Action(yc.<>m__B7);
            dialogContext.pointerUpCallback = new Func<bool>(yc.<>m__B8);
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public void EnableBossCamera(uint targetId)
        {
            Singleton<CameraManager>.Instance.EnableBossCamera(targetId);
        }

        public void EnableCrowdCamera()
        {
            Singleton<CameraManager>.Instance.EnableCrowdCamera();
        }

        public void EnableFireProp(uint runtimeID, float effectDuration, float CD)
        {
            MonoFireTriggerProp propObjectByRuntimeID = (MonoFireTriggerProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID);
            propObjectByRuntimeID.DisableProp();
            propObjectByRuntimeID.EnableFire(effectDuration, CD);
        }

        public void EndlessLevelEnd(string result, bool collectAllGoods = true)
        {
            Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.LevelEndNewIter(result, true, collectAllGoods, 0));
        }

        private void EndLevel(EvtLevelState.LevelEndReason reason = 0, int endCgId = 0)
        {
            this.StopLevelDesign();
            Singleton<EventManager>.Instance.DropEventsAndStop();
            Singleton<LevelManager>.Instance.SetPause(true);
            LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
            bool flag = reason == EvtLevelState.LevelEndReason.EndWin;
            if (plugin != null)
            {
                flag &= Singleton<AvatarManager>.Instance.GetLocalAvatar().IsActive();
            }
            if (flag)
            {
                this.AddGalTouchGoodFeel();
            }
            if (Singleton<LevelScoreManager>.Instance.LevelType == 4)
            {
                Singleton<MainUIManager>.Instance.ShowPage(new EndlessFloorEndPageContext(reason), UIType.Page);
            }
            else
            {
                bool forceEnableWhenSetup = !Singleton<AvatarManager>.Instance.GetLocalAvatar().IsActive();
                Singleton<MainUIManager>.Instance.ShowPage(new LevelEndPageContext(reason, forceEnableWhenSetup, endCgId), UIType.Page);
            }
            Singleton<WwiseAudioManager>.Instance.SetSwitch("Level_Result", (reason != EvtLevelState.LevelEndReason.EndWin) ? "Lose" : "Win");
            Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off", null, null, null);
            Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_End", null, null, null);
        }

        public void EnterLevelTransition()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(0.18f, false, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EnterTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
        }

        public void EnterStoryMode(int plotID, bool lerpIn = true, bool lerpOut = true, bool needFadeIn = true, bool backFollow = true, bool pauseLevel = false)
        {
            if (!Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
            {
                this.SetPause(true);
                Singleton<CameraManager>.Instance.GetMainCamera().PlayStoryCameraState(plotID, lerpIn, lerpOut, needFadeIn, backFollow, pauseLevel);
            }
        }

        public void EnterStoryModeWithFollowingAnimation(int plotID, string followAnimationName)
        {
            if (!string.IsNullOrEmpty(followAnimationName))
            {
                if (AuxObjectData.ContainAuxObjectPrefabPath(followAnimationName))
                {
                    this.EnterStoryMode(plotID, true, false, false, true, false);
                }
                else
                {
                    this.EnterStoryMode(plotID, true, true, true, true, false);
                }
            }
        }

        public void ExitLevelTransition()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, null);
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
        }

        public void ExitStoryMode()
        {
            if (!Singleton<CameraManager>.Instance.GetMainCamera().IsInTransitionLerp() && Singleton<CameraManager>.Instance.GetMainCamera().storyState.active)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().storyState.StartQuit();
            }
        }

        public void Fade(bool waitFadeOutEnd = false, bool waitFadeInEnd = false)
        {
            <Fade>c__AnonStoreyC2 yc = new <Fade>c__AnonStoreyC2 {
                waitFadeOutEnd = waitFadeOutEnd,
                waitFadeInEnd = waitFadeInEnd
            };
            Action fadeEndCallback = new Action(yc.<>m__B6);
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeOutStageTransitPanel(0.18f, false, null, fadeEndCallback);
        }

        public void ForceUseAvatarAI()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar != null)
            {
                localAvatar.ForceUseAIController();
            }
        }

        public string[] GetAppliedToolList()
        {
            List<string> appliedToolList = Singleton<LevelScoreManager>.Instance.appliedToolList;
            if (appliedToolList != null)
            {
                appliedToolList.Insert(0, appliedToolList.Count.ToString());
            }
            else
            {
                string[] collection = new string[] { "0" };
                appliedToolList = new List<string>(collection);
            }
            return appliedToolList.ToArray();
        }

        public string GetAvatarConfigField(string type, string query)
        {
            ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(type);
            query = query.Trim();
            char[] separator = new char[] { '.' };
            string[] fieldStrs = query.Split(separator);
            if (fieldStrs.Length <= 0)
            {
                return string.Empty;
            }
            return this.GetClassFieldOrDictValue(avatarConfig, fieldStrs, 0);
        }

        public string GetAvatarShortName(Transform actor)
        {
            return actor.GetComponent<BaseMonoAvatar>().AvatarTypeName;
        }

        public string GetClassFieldOrDictValue(object obj, string[] fieldStrs, int currentIndex)
        {
            if ((fieldStrs.Length - 1) < currentIndex)
            {
                return obj.ToString();
            }
            string key = fieldStrs[currentIndex];
            System.Type c = obj.GetType();
            if (typeof(IDictionary).IsAssignableFrom(c))
            {
                IDictionary dictionary = (IDictionary) obj;
                if (!dictionary.Contains(key))
                {
                    return string.Empty;
                }
                return this.GetClassFieldOrDictValue(dictionary[key], fieldStrs, currentIndex + 1);
            }
            FieldInfo field = c.GetField(key);
            if (field == null)
            {
                return string.Empty;
            }
            return this.GetClassFieldOrDictValue(field.GetValue(obj), fieldStrs, currentIndex + 1);
        }

        public int GetCurrentProgress()
        {
            return Singleton<LevelScoreManager>.Instance.progress;
        }

        public string GetCurrentStageFirstName()
        {
            char[] separator = new char[] { '_' };
            return Singleton<StageManager>.Instance.GetStageTypeName().Split(separator)[0];
        }

        public string GetCurrentStageName()
        {
            return Singleton<StageManager>.Instance.GetStageTypeName();
        }

        public string GetDropEquipItemTypeName(int metaId)
        {
            if (ItemMetaDataReader.TryGetItemMetaDataByKey(metaId) != null)
            {
                return "material";
            }
            if (WeaponMetaDataReader.TryGetWeaponMetaDataByKey(metaId) != null)
            {
                return "weapon";
            }
            if (StigmataMetaDataReader.TryGetStigmataMetaDataByKey(metaId) != null)
            {
                return "stigmata";
            }
            if (AvatarCardMetaDataReader.TryGetAvatarCardMetaDataByKey(metaId) != null)
            {
                return "avatar_card";
            }
            if (AvatarFragmentMetaDataReader.TryGetAvatarFragmentMetaDataByKey(metaId) != null)
            {
                return "avatar_fragment";
            }
            if (EndlessToolMetaDataReader.TryGetEndlessToolMetaDataByKey(metaId) != null)
            {
                return "endless_tool";
            }
            return string.Empty;
        }

        public int GetEndlessGroupLevel()
        {
            if (Singleton<EndlessModule>.Instance == null)
            {
                return 0;
            }
            return Singleton<EndlessModule>.Instance.currentGroupLevel;
        }

        public float GetEndlessLevelTimeCountDown()
        {
            return Singleton<LevelScoreManager>.Instance.levelTimer;
        }

        public int GetEndlessRandomSeed()
        {
            if (Singleton<EndlessModule>.Instance == null)
            {
                return 0;
            }
            return Singleton<EndlessModule>.Instance.randomSeed;
        }

        public float GetInLevelTimeCountDown()
        {
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            return (float) Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().countDownTimer;
        }

        public bool GetIsDebugDynamicHardLevel()
        {
            return Singleton<LevelScoreManager>.Instance.isDebugDynamicLevel;
        }

        public int GetLevelDefendModeMonsterEnterAmount()
        {
            return Singleton<LevelManager>.Instance.levelActor.GetLevelDefendModeMonsterEnterAmount();
        }

        public int GetLevelDefendModeMonsterKillAmount()
        {
            return Singleton<LevelManager>.Instance.levelActor.GetLevelDefendModeMonsterKillAmount();
        }

        public string GetLevelDifficulty()
        {
            switch (Singleton<LevelScoreManager>.Instance.difficulty)
            {
                case 1:
                    return "Normal";

                case 2:
                    return "Hard";

                case 3:
                    return "Hell";
            }
            return "Normal";
        }

        public LuaTable GetLevelLuaTableByLevelName(string levelScript)
        {
            return (LuaTable) this._luaState.DoString(Miscs.LoadTextFileToString(levelScript), levelScript, null)[0];
        }

        public int GetLevelMode()
        {
            return (int) Singleton<LevelScoreManager>.Instance.levelMode;
        }

        public string GetLevelStasticsResult()
        {
            string str = string.Empty;
            LevelDamageStasticsPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDamageStasticsPlugin>();
            return (((str + "Time: " + Mathf.FloorToInt(plugin.stageTime)) + " DMG: " + Mathf.FloorToInt(plugin.avatarDamage)) + " MDMG: " + Mathf.FloorToInt(plugin.monsterDamage));
        }

        public Transform GetLocalAvatar()
        {
            return Singleton<AvatarManager>.Instance.GetLocalAvatar().transform;
        }

        public uint GetLocalAvatarID()
        {
            return Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
        }

        public object GetMonsterBasicParameters(string MonsterName, string ConfigTypeName, int level, bool isElite)
        {
            MonsterConfigMetaData monsterConfigMetaData = MonsterData.GetMonsterConfigMetaData(MonsterName, ConfigTypeName);
            ConfigMonster monster = MonsterData.GetMonsterConfig(MonsterName, ConfigTypeName, string.Empty);
            float num = 0f;
            float num2 = 0f;
            float num3 = 0f;
            NPCLevelMetaData nPCLevelMetaDataByKey = NPCLevelMetaDataReader.GetNPCLevelMetaDataByKey(level);
            num = monsterConfigMetaData.HP * nPCLevelMetaDataByKey.HPRatio;
            num2 = monsterConfigMetaData.defense * nPCLevelMetaDataByKey.DEFRatio;
            num3 = monsterConfigMetaData.attack * nPCLevelMetaDataByKey.ATKRatio;
            if (isElite)
            {
                num *= monster.EliteArguments.HPRatio;
                num2 *= monster.EliteArguments.DefenseRatio;
                num3 *= monster.EliteArguments.AttackRatio;
            }
            return new float[] { num, num2, num3 };
        }

        public string GetMonsterConfigField(string category, string subType, string query)
        {
            ConfigMonster monster = MonsterData.GetMonsterConfig(category, subType, string.Empty);
            query = query.Trim();
            char[] separator = new char[] { '.' };
            string[] fieldStrs = query.Split(separator);
            if (fieldStrs.Length <= 0)
            {
                return string.Empty;
            }
            return this.GetClassFieldOrDictValue(monster, fieldStrs, 0);
        }

        public int GetMonsterCount()
        {
            return Singleton<MonsterManager>.Instance.MonsterCount();
        }

        public uint GetNearestMonsterID()
        {
            uint runtimeID = 0;
            float maxValue = float.MaxValue;
            foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
            {
                if (monster.IsActive())
                {
                    float num3 = Vector3.Distance(monster.XZPosition, Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition);
                    if (num3 < maxValue)
                    {
                        runtimeID = monster.GetRuntimeID();
                        maxValue = num3;
                    }
                }
            }
            return runtimeID;
        }

        public int GetNPCHardLevel()
        {
            return Singleton<LevelScoreManager>.Instance.NPCHardLevel;
        }

        public object GetPointsAroundSpecificPoint(string targetPoint, int num)
        {
            List<MonoSpawnPoint> list = new List<MonoSpawnPoint>(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints);
            this.centerPoint = this.GetSpawnPoint(targetPoint);
            list.Sort(new Comparison<MonoSpawnPoint>(this.CompareByDistance));
            string[] strArray = new string[num];
            for (int i = 0; i < num; i++)
            {
                strArray[i] = list[i].name;
            }
            return strArray;
        }

        public Vector3 GetRandomInSightPoint(LuaTable pointList)
        {
            List<Vector3> list = new List<Vector3>();
            IEnumerator enumerator = pointList.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(current as string);
                    Vector3 position = Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx].transform.position;
                    if (this.IsPointInCameraFov(position))
                    {
                        list.Add(position);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            if (list.Count > 0)
            {
                return list[UnityEngine.Random.Range(0, list.Count - 1)];
            }
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            Vector3 vector2 = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - mainCamera.XZPosition;
            return (Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition + vector2.normalized);
        }

        public int GetServerDayOfWeek()
        {
            return (int) TimeUtil.Now.DayOfWeek;
        }

        private MonoSpawnPoint GetSpawnPoint(string spawnName)
        {
            int namedSpawnPointIx;
            MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
            if (spawnName != null)
            {
                namedSpawnPointIx = stageEnv.GetNamedSpawnPointIx(spawnName);
                if (namedSpawnPointIx < 0)
                {
                    namedSpawnPointIx = UnityEngine.Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length);
                }
            }
            else
            {
                namedSpawnPointIx = UnityEngine.Random.Range(0, Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length);
            }
            return stageEnv.spawnPoints[namedSpawnPointIx];
        }

        public Vector3 GetSpawnPointPos(string spawnName)
        {
            Vector3 xZPosition = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition;
            string key = spawnName;
            if (key != null)
            {
                int num;
                if (<>f__switch$map5 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
                    dictionary.Add("OnAvatar", 0);
                    dictionary.Add("TutorialPos", 1);
                    <>f__switch$map5 = dictionary;
                }
                if (<>f__switch$map5.TryGetValue(key, out num))
                {
                    if (num == 0)
                    {
                        return xZPosition;
                    }
                    if (num == 1)
                    {
                        return (xZPosition - ((Vector3) (xZPosition.normalized * 2.1f)));
                    }
                }
            }
            return this.GetSpawnPoint(spawnName).transform.position;
        }

        public string GetStageBaseRenderingDataName()
        {
            return Singleton<StageManager>.Instance.GetPerpStage().GetCurrentBaseWeatherName();
        }

        public string GetStageBaseWeatherName()
        {
            return Singleton<StageManager>.Instance.GetPerpStage().GetCurrentBaseWeatherName();
        }

        public bool HasAbility(string abilityName)
        {
            if (!string.IsNullOrEmpty(abilityName))
            {
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
                {
                    AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID());
                    if (((actor != null) && (actor.abilityPlugin != null)) && actor.abilityPlugin.HasAbility(abilityName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasAbilityOverrideName(string abilityName, string overrideName)
        {
            bool flag = false;
            ConfigOverrideGroup group = null;
            AbilityData.GetAbilityGroupMap().TryGetValue(abilityName, out group);
            if ((group != null) && (group.Overrides != null))
            {
                flag = group.Overrides.ContainsKey(overrideName);
            }
            return flag;
        }

        public bool HasHelperAvatar()
        {
            return (Singleton<LevelScoreManager>.Instance.friendDetailItem != null);
        }

        public bool HasLevelActorCountDownPlugin()
        {
            return Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelActorCountDownPlugin>();
        }

        public void InitAtAwake()
        {
            this._luaManualCoroutines = new List<Tuple<LuaThread, int>>();
            this._ldEvtTriggers = new List<LDEvtTrigger>();
            this._evtTriggers = new List<EvtTrigger>();
            this._allLDEvents = new List<BaseLDEvent>();
            this._luaState = new LuaState();
            this._luaState["LevelDesign"] = this;
            string luaFile = Singleton<LevelScoreManager>.Instance.luaFile;
            LuaTable table = (LuaTable) this._luaState.DoString(Miscs.LoadTextFileToString(luaFile), luaFile, null)[0];
            this._ldMain = (LuaFunction) table["main"];
            this.state = LDState.Start;
        }

        public void InitAtStart()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().gameObject.SetActive(true);
            Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.SetActive(true);
        }

        private void InitPropObject(uint propID, LuaTable dropTable)
        {
            PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(propID);
            List<LDDropDataItem> list = new List<LDDropDataItem>();
            IEnumerator enumerator = dropTable.Values.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    list.Add(current as LDDropDataItem);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            actor.dropDataItems = list;
        }

        public bool IsLevelDone()
        {
            return Singleton<LevelScoreManager>.Instance.IsLevelDone();
        }

        public bool IsMonsterAlive(uint runtimeID)
        {
            return (Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID) != null);
        }

        public bool IsPlayerBehaviourDone(string key)
        {
            return Singleton<PlayerModule>.Instance.IsBehaviourDone(key);
        }

        public bool IsPointInCameraFov(Vector3 pos)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            Vector3 from = pos - mainCamera.transform.position;
            return (Vector3.Angle(from, mainCamera.transform.forward) < (mainCamera.cameraComponent.fieldOfView - 10f));
        }

        public void KillAllMonsters(bool dropReward)
        {
            foreach (MonsterActor actor in Singleton<EventManager>.Instance.GetActorByCategory<MonsterActor>(4))
            {
                if (!actor.monster.isStaticInScene)
                {
                    actor.needDropReward = dropReward;
                    actor.ForceKill();
                }
            }
        }

        public void LevelDefendModePluginReset(int targetValue)
        {
            Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginReset(targetValue);
        }

        public void LevelDefendModePluginStop()
        {
            Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStop();
        }

        public void LevelDesignEndWithResult(EvtLevelState.LevelEndReason reason = 0, int endCgId = 0)
        {
            this.EndLevel(reason, endCgId);
        }

        public void LevelDesignStart()
        {
            LevelActor actor = (LevelActor) Singleton<EventManager>.Instance.GetActor(0x21800001);
            actor.levelMode = Singleton<LevelScoreManager>.Instance.levelMode;
            this.MakeAndStartCoroutine(this._ldMain);
            this.state = LDState.Running;
        }

        public void LevelEndNew(string result, bool collectAllGoods = true, int endCgId = 0)
        {
            bool flag = collectAllGoods;
            Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.LevelEndNewIter(result, false, flag, endCgId));
        }

        [DebuggerHidden]
        private IEnumerator LevelEndNewIter(string result, bool isEndlessLevel = false, bool collectAllGoods = true, int endCgId = 0)
        {
            return new <LevelEndNewIter>c__Iterator35 { result = result, collectAllGoods = collectAllGoods, endCgId = endCgId, isEndlessLevel = isEndlessLevel, <$>result = result, <$>collectAllGoods = collectAllGoods, <$>endCgId = endCgId, <$>isEndlessLevel = isEndlessLevel, <>f__this = this };
        }

        public void LevelEndWithResult(bool isWin, int endCgId = 0)
        {
            EvtLevelState.State state = !isWin ? EvtLevelState.State.EndLose : EvtLevelState.State.EndWin;
            EvtLevelState.LevelEndReason reason = !isWin ? EvtLevelState.LevelEndReason.EndLoseNotMeetCondition : EvtLevelState.LevelEndReason.EndWin;
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(state, reason, endCgId), MPEventDispatchMode.Normal);
        }

        public void ListenEvent(string evtTypeName, LuaFunction callback)
        {
            EvtTrigger item = new EvtTrigger(evtTypeName, callback, false);
            this._evtTriggers.Add(item);
        }

        public void ListenLDEvent(LuaTable table, LuaFunction callback)
        {
            BaseLDEvent ldEvent = this.CreateLDEventFromTable(table);
            this._ldEvtTriggers.Add(new LDEvtTrigger(ldEvent, callback, false));
        }

        public void ListenLDEventCoroutine(LuaTable table, LuaFunction callback)
        {
            BaseLDEvent ldEvent = this.CreateLDEventFromTable(table);
            this._ldEvtTriggers.Add(new LDEvtTrigger(ldEvent, callback, true));
        }

        public object LoadLDDropDataItem(string goodsTypeName)
        {
            return LDDropDataItem.GetLDDropDataItemByName(goodsTypeName).Clone();
        }

        public bool LoadVideo(int cgId)
        {
            CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(cgId);
            if (cgDataItem == null)
            {
                return false;
            }
            Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas is MonoInLevelUICanvas)
            {
                (mainCanvas as MonoInLevelUICanvas).LoadVideo(cgDataItem);
            }
            return true;
        }

        [DebuggerHidden]
        private IEnumerator LuaCoroutineIter(LuaThread luaThread)
        {
            return new <LuaCoroutineIter>c__Iterator38 { luaThread = luaThread, <$>luaThread = luaThread, <>f__this = this };
        }

        public void MainCameraFollowLookAt(uint runtimeID, bool mute = false)
        {
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
            if ((entity != null) && entity.IsActive())
            {
                Singleton<CameraManager>.Instance.GetMainCamera().FollowLookAtPosition(entity.XZPosition, mute, false);
            }
        }

        public void MainCameraFollowLookAtPoint(string pointName, bool mute = false)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(pointName);
            Singleton<CameraManager>.Instance.GetMainCamera().FollowLookAtPosition(spawnPoint.transform.position, mute, false);
        }

        public void MainCameraFollowSetFar()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Far, false);
        }

        public void MainCameraFollowSetFurther()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Furter, false);
        }

        public void MainCameraFollowSetHigh()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.High, false);
        }

        public void MainCameraFollowSetHigher()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Higher, false);
        }

        public void MainCameraFollowSetNear()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetFollowRange(MainCameraFollowState.RangeState.Near, false);
        }

        private LuaThread MakeAndStartCoroutine(LuaFunction luaFunc)
        {
            LuaThread luaThread = new LuaThread(this._luaState, luaFunc);
            int num = Singleton<ApplicationManager>.Instance.StartCoroutineManual(this.LuaCoroutineIter(luaThread));
            this._luaManualCoroutines.Add(Tuple.Create<LuaThread, int>(luaThread, num));
            return luaThread;
        }

        public void MoveLocalAvatarTo(string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            localAvatar.transform.position = spawnPoint.transform.position;
            localAvatar.transform.forward = spawnPoint.transform.forward;
            Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(localAvatar.GetRuntimeID(), false);
        }

        public void MuteInput()
        {
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(true);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, false));
        }

        public void PlayBGMByName(string bgmName)
        {
            Singleton<WwiseAudioManager>.Instance.SetSwitch("Game_Stage_Type", bgmName);
            Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_Start", null, null, null);
        }

        public void PlayBGMByNameWithDelay(string bgmName, float delay)
        {
        }

        public void PlayCameraAnimationOnAnimatorEntity(string animationName, uint entityID, bool enterLerp, bool exitLerp, bool pauselevel, bool diableEntityFadeMatieral = false, bool cullingAvatar = false)
        {
            <PlayCameraAnimationOnAnimatorEntity>c__AnonStoreyC1 yc = new <PlayCameraAnimationOnAnimatorEntity>c__AnonStoreyC1 {
                diableEntityFadeMatieral = diableEntityFadeMatieral
            };
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (!mainCamera.levelAnimState.active)
            {
                MonoSimpleAnimation animation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 0x21800001);
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(entityID);
                animation.transform.parent = entity.transform;
                animation.transform.localPosition = Vector3.zero;
                animation.transform.localRotation = Quaternion.identity;
                Action startCallback = null;
                Action endCallback = null;
                yc.animatorEntity = entity as BaseMonoAnimatorEntity;
                yc.abilityActor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entity.GetRuntimeID());
                if (yc.animatorEntity != null)
                {
                    startCallback = new Action(yc.<>m__B4);
                    endCallback = new Action(yc.<>m__B5);
                }
                CameraAnimationCullingType cullType = !cullingAvatar ? CameraAnimationCullingType.CullNothing : CameraAnimationCullingType.CullAvatars;
                mainCamera.PlayLevelAnimationThenTransitBack(animation.GetComponent<Animation>(), true, enterLerp, exitLerp, pauselevel, cullType, startCallback, endCallback);
            }
        }

        public void PlayCameraAnimationOnAnimatorEntityThenStay(string animationName, uint entityID, bool pauseLevel)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (!mainCamera.levelAnimState.active)
            {
                MonoSimpleAnimation animation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 0x21800001);
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(entityID);
                animation.transform.parent = entity.transform;
                animation.transform.localPosition = Vector3.zero;
                animation.transform.localRotation = Quaternion.identity;
                mainCamera.PlayAvatarCameraAnimationThenStay(animation.GetComponent<Animation>(), Singleton<AvatarManager>.Instance.GetLocalAvatar());
            }
        }

        public void PlayCameraAnimationOnEnv(string animationName, bool enterLerp, bool exitLerp, bool pauseLevel, CameraAnimationCullingType cullType = 0)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (!mainCamera.levelAnimState.active)
            {
                MonoSimpleAnimation animation = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(animationName, 0x21800001);
                Transform transform = Singleton<StageManager>.Instance.GetStageEnv().transform;
                animation.transform.parent = transform;
                animation.transform.localPosition = Vector3.zero;
                animation.transform.localRotation = Quaternion.identity;
                mainCamera.PlayLevelAnimationThenTransitBack(animation.GetComponent<Animation>(), true, enterLerp, exitLerp, pauseLevel, cullType, null, null);
            }
        }

        public bool PlayVideo(int cgId, bool withFade = false)
        {
            CgDataItem cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(cgId);
            if (cgDataItem == null)
            {
                return false;
            }
            Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas is MonoLoadingCanvas)
            {
                mainCanvas.PlayVideo(cgDataItem);
            }
            else if (mainCanvas is MonoInLevelUICanvas)
            {
                if (withFade)
                {
                    (mainCanvas as MonoInLevelUICanvas).StartPlayVideo(cgDataItem);
                }
                else
                {
                    mainCanvas.PlayVideo(cgDataItem);
                }
            }
            else
            {
                mainCanvas.PlayVideo(cgDataItem);
            }
            return true;
        }

        public void PopRenderingData(int ix)
        {
            Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(ix);
        }

        public void PopWeather(int ix)
        {
            Singleton<StageManager>.Instance.GetPerpStage().PopWeather(ix);
        }

        public void PreloadMonster(string monsterName, string typeName, bool disableBehaviorWhenInit = false)
        {
            Singleton<MonsterManager>.Instance.PreloadMonster(monsterName, typeName, 0, false);
        }

        public void PreloadUniqueMonster(uint uniqueMonsterID, bool disableBehaviorWhenInit = false)
        {
            UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
            string monsterName = uniqueMonsterMetaData.monsterName;
            string typeName = uniqueMonsterMetaData.typeName;
            Singleton<MonsterManager>.Instance.PreloadMonster(monsterName, typeName, uniqueMonsterID, false);
        }

        public int PushRenderingDataWithTransition(string stageRenderingDataName, float transitDuration)
        {
            return Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData(stageRenderingDataName, transitDuration);
        }

        public int PushWeatherWithTransition(string stageWeatherName, float transitDuration)
        {
            return Singleton<StageManager>.Instance.GetPerpStage().PushWeather(stageWeatherName, transitDuration);
        }

        public void RecoveryInput()
        {
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(false);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, true));
        }

        public uint RegisterStageEnvTriggerField(string fieldName)
        {
            GameObject gameObject = Singleton<StageManager>.Instance.GetStageEnv().transform.Find(fieldName).gameObject;
            return Singleton<DynamicObjectManager>.Instance.RegisterStageEnvTriggerField(0x21800001, gameObject);
        }

        public void RemoveBarrier(uint runtimeID)
        {
            BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(runtimeID);
            dynamicObjectByRuntimeID.GetComponentInChildren<FadeAnimation>().StartFadeOut(new Action(dynamicObjectByRuntimeID.SetDied));
        }

        public void RemoveHintArrowForPath()
        {
            Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.RemoveHintArrowForPath();
        }

        public void RemoveInLevelTimeCountDown()
        {
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
            {
                levelActor.RemovePlugin<LevelActorCountDownPlugin>();
                this.SetInLevelTimeCountDownVisible(false);
            }
        }

        public void RemoveLevelDamageStastics()
        {
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            if (levelActor.HasPlugin<LevelDamageStasticsPlugin>())
            {
                levelActor.RemoveLevelDamageStastics();
            }
        }

        public void ResetAudioListener()
        {
            Singleton<WwiseAudioManager>.Instance.ResetListener();
        }

        public void ResetStageBaseRenderingData(float duration)
        {
            Singleton<StageManager>.Instance.GetPerpStage().ResetBaseRenderingData(duration);
        }

        public void ResetStageBaseWeather(float transitDuration)
        {
            Singleton<StageManager>.Instance.GetPerpStage().ResetBaseWeather(transitDuration);
        }

        public void RestartLuaLogic(string luaPath)
        {
            this.RemoveLevelDamageStastics();
            Singleton<LevelScoreManager>.Instance.luaFile = luaPath;
            this.InitAtAwake();
            this.LevelDesignStart();
        }

        public void SetAbilitySpecials(string abilityName, LuaTable overrideTable)
        {
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
            {
                if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).abilityPlugin.HasAbility(abilityName))
                {
                    foreach (ActorAbility ability in Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).abilityPlugin.GetAppliedAbilities())
                    {
                        if (ability.config.AbilityName == abilityName)
                        {
                            IDictionaryEnumerator enumerator = overrideTable.GetEnumerator();
                            try
                            {
                                while (enumerator.MoveNext())
                                {
                                    DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                                    string key = (string) current.Key;
                                    if (current.Value is double)
                                    {
                                        ability.SetOverrideMapValue(key, (float) ((double) current.Value));
                                    }
                                    else if (current.Value is string)
                                    {
                                        ability.SetOverrideMapValue(key, (string) current.Value);
                                    }
                                }
                            }
                            finally
                            {
                                IDisposable disposable = enumerator as IDisposable;
                                if (disposable == null)
                                {
                                }
                                disposable.Dispose();
                            }
                        }
                    }
                }
            }
        }

        public void SetAvatarAttackRatio(float ratio)
        {
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
            {
                Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID()).SetAvatarAttackRatio(ratio);
            }
        }

        public void SetAvatarBeAttackMaxNum(int maxNum)
        {
            Singleton<LevelManager>.Instance.levelActor.SetAvatarBeAttackMaxNum(maxNum);
        }

        public void SetBGMState(string bgmState)
        {
            if (bgmState == "Battle")
            {
                Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_ResumeVol_B", null, null, null);
            }
            else if (bgmState == "NonBattle")
            {
                Singleton<WwiseAudioManager>.Instance.Post("BGM_Stage_TurnDownVol_B", null, null, null);
            }
        }

        public void SetCameraLocateRatio(float ratio)
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetUserDefinedCameraLocateRatio(ratio);
        }

        public void SetEntityAllowSelected(uint runtimeID, bool allowSelected)
        {
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
            if (entity != null)
            {
                (entity as BaseMonoAbilityEntity).SetCountedDenySelect(!allowSelected, true);
            }
        }

        public void SetEntityIsGhost(uint runtimeID, bool isGhost)
        {
            BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
            if (entity != null)
            {
                (entity as BaseMonoAbilityEntity).SetCountedIsGhost(isGhost);
            }
        }

        public void SetEnvCollisionActive(bool isActive)
        {
            if (Singleton<StageManager>.Instance.GetStageEnv().transform.FindChild("Collision") != null)
            {
                Singleton<StageManager>.Instance.GetStageEnv().transform.FindChild("Collision").gameObject.SetActive(isActive);
            }
        }

        public void SetInLevelTimeCountDown(float time)
        {
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            if (levelActor.HasPlugin<LevelActorCountDownPlugin>())
            {
                levelActor.GetPlugin<LevelActorCountDownPlugin>().ResetPlugin(time);
            }
            else
            {
                levelActor.AddPlugin(new LevelActorCountDownPlugin(levelActor, time, false));
            }
            this.SetInLevelTimeCountDownVisible(true);
        }

        public void SetInLevelTimeCountDownSpeedRatio(float ratioInNormalTime, float ratioInWitchTime)
        {
            LevelActorCountDownPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>();
            if (plugin != null)
            {
                plugin.SetCountDownSpeedRatio(ratioInNormalTime, ratioInWitchTime);
            }
        }

        private void SetInLevelTimeCountDownVisible(bool isVisible)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetTimeCountDownTextActive, isVisible));
        }

        public void SetInLevelTimesUpWin(bool timesUpWin)
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelActorCountDownPlugin>())
            {
                Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().timeUpWin = timesUpWin;
            }
        }

        public void SetInLevelUIActive(bool isActive)
        {
            MonoInLevelUICanvas inLevelUICanvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
            if (inLevelUICanvas != null)
            {
                inLevelUICanvas.SetInLevelUIActive(isActive);
            }
        }

        public void SetLevelDefendModePluginStart()
        {
            Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStart();
        }

        public void SetLevelDefendModePluginStart(int targetValue)
        {
            if (targetValue > 0)
            {
                Singleton<LevelManager>.Instance.levelActor.SetLevelDefendModePluginStart(targetValue);
            }
        }

        public void SetLevelDefendModeStop()
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelDefendModePlugin>())
            {
                LevelDefendModePlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelDefendModePlugin>();
                plugin.SetActive(false);
                plugin.Stop();
            }
        }

        public void SetLocalAvatarFaceTo(string spawnName)
        {
            MonoSpawnPoint spawnPoint = this.GetSpawnPoint(spawnName);
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            Vector3 forward = spawnPoint.transform.position - localAvatar.transform.position;
            localAvatar.SteerFaceDirectionTo(forward);
        }

        public void SetMonsterAnimatorSpeed(uint runtimeID, float speed)
        {
            BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
            if (monster != null)
            {
                if (speed != 1f)
                {
                    if (monster.timeScaleStack.IsOccupied(7))
                    {
                        monster.SetTimeScale(speed, 7);
                    }
                    else
                    {
                        monster.PushTimeScale(speed, 7);
                    }
                }
                else
                {
                    monster.PopTimeScale(7);
                }
            }
        }

        public void SetMonsterAttackRatio(uint monsterRuntimeID, float ratio)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
            if (actor != null)
            {
                actor.SetMonsterAttackRatio(ratio);
            }
        }

        public void SetMonsterEnabled(uint monsterID, bool enabled)
        {
            if (Singleton<EventManager>.Instance.GetActor(monsterID) != null)
            {
                BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID);
                if (!enabled)
                {
                    monsterByRuntimeID.OrderMove = false;
                    monsterByRuntimeID.ClearHitTrigger();
                    monsterByRuntimeID.ClearAttackTriggers();
                    monsterByRuntimeID.SetUseAIController(false);
                }
                else
                {
                    monsterByRuntimeID.SetUseAIController(true);
                }
            }
        }

        public void SetMonsterFaceTo(uint runtimeID, string spawnName)
        {
            Vector3 zero = Vector3.zero;
            Vector3 one = Vector3.one;
            BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
            if (monster != null)
            {
                one = monster.XZPosition;
                if (spawnName == "Avatar")
                {
                    BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
                    if (avatar != null)
                    {
                        zero = avatar.XZPosition;
                    }
                }
                else
                {
                    zero = this.GetSpawnPoint(spawnName).transform.position;
                }
                monster.SteerFaceDirectionTo(zero - one);
            }
        }

        public void SetMonsterHPRatio(uint monsterRuntimeID, float ratio)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
            if (actor != null)
            {
                actor.SetMonsterHPRatio(ratio);
            }
        }

        public void SetMonsterMaterialFadeEnabled(uint monsterID, bool enabled)
        {
            if (Singleton<EventManager>.Instance.GetActor(monsterID) != null)
            {
                Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(monsterID).SetMonsterMaterialFadeEnabled(enabled);
            }
        }

        public void SetMonsterSteerTo(uint runtimeID, Vector3 pos)
        {
            BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
            if (monster != null)
            {
                Vector3 dir = pos - monster.transform.position;
                monster.SteerFaceDirectionTo(dir);
            }
        }

        public void SetMonsterTrigger(uint runtimeID, string triggerName)
        {
            BaseMonoMonster monster = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(runtimeID);
            if (monster != null)
            {
                monster.SetTrigger(triggerName);
            }
        }

        public void SetMonsterUnremovable(uint runtimeID, bool unremovable)
        {
            Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(runtimeID).isStaticInScene = unremovable;
        }

        public void SetMonsterWarningRange(uint runtimeID, float warningRange, float escapeRange)
        {
            (Singleton<EventManager>.Instance.GetActor(runtimeID) as MonsterActor).EnableWarningFieldActor(warningRange, escapeRange);
        }

        public void SetMuteAvatarVoice(bool mute)
        {
            string str = string.Empty;
            str = !mute ? "UI_Exit_StoryMode" : "UI_Enter_StoryMode";
            if (!string.IsNullOrEmpty(str))
            {
                Singleton<WwiseAudioManager>.Instance.Post(str, null, null, null);
            }
        }

        public void SetNatureBonusFactor(float upFactor, float downFactor)
        {
            Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor = Mathf.Clamp(upFactor, 0f, upFactor);
            Singleton<LevelManager>.Instance.levelActor.downLevelNatureBonusFactor = Mathf.Clamp(downFactor, 0f, downFactor);
        }

        public void SetPause(bool pause)
        {
            this.state = !pause ? LDState.Running : LDState.Paused;
        }

        public void SetPlayerBehaviourDone(string key)
        {
            Singleton<PlayerModule>.Instance.SetBehaviourDone(key);
        }

        public void SetRotateToFaceDirection()
        {
            Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
        }

        public void SetSpikeProp(uint runtimeID, float effectDuration, float CD)
        {
            ((MonoSpikeTriggerProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID)).SetSpikePropDurationAndCD(effectDuration, CD);
        }

        public uint SetSpikePropContineousState(uint runtimeID, bool isActive)
        {
            ((MonoSpikeTriggerProp) Singleton<PropObjectManager>.Instance.GetPropObjectByRuntimeID(runtimeID)).SetContinuousState(isActive);
            return runtimeID;
        }

        public void SetStageAnimatorSpeed(float speed)
        {
            if (Singleton<StageManager>.Instance.GetPerpStage().gameObject.GetComponent<Animator>() != null)
            {
                Singleton<StageManager>.Instance.GetPerpStage().gameObject.GetComponent<Animator>().speed = speed;
            }
        }

        public void SetStageBaseRenderingDataWithTransition(string stageRenderingDataName, float transitDuration)
        {
            Singleton<StageManager>.Instance.GetPerpStage().SetBaseRenderingData(stageRenderingDataName, transitDuration);
        }

        public void SetStageBaseWeatherWithTransition(string weatherName, float transitDuration)
        {
            Singleton<StageManager>.Instance.GetPerpStage().SetBaseWeather(weatherName, transitDuration);
        }

        public void SetStageNodeVisible(string nodeNames, bool isVisible)
        {
            StageManager.SetPerpstageNodeVisibilityByNode(Singleton<StageManager>.Instance.GetPerpStage(), nodeNames, isVisible);
        }

        public void SetupBGM(LuaTable table, string initState)
        {
        }

        public void SetupLastKilLCameraCloseUp()
        {
            Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID()).SetupLastKillCloseUp();
        }

        public void SetupLevelDamageStastics()
        {
            Singleton<LevelManager>.Instance.levelActor.SetupLevelDamageStastics();
        }

        public void SetupLevelReward(LuaTable dropTable, LuaTable otherReward)
        {
            List<DropItem> configLevelDrops = Singleton<LevelScoreManager>.Instance.configLevelDrops;
            if (configLevelDrops != null)
            {
                for (int i = 0; i < configLevelDrops.Count; i++)
                {
                    dropTable[i] = configLevelDrops[i];
                }
            }
            otherReward["AvatarExpInside"] = Singleton<LevelScoreManager>.Instance.configAvatarExpInside;
            otherReward["ScoinInside"] = Singleton<LevelScoreManager>.Instance.configScoinInside;
        }

        public void ShowLevelDisplayText(string textMapKey, LuaTable paramTable = null)
        {
            string body = string.Empty;
            if (paramTable != null)
            {
                object[] array = new object[paramTable.Values.Count];
                paramTable.Values.CopyTo(array, 0);
                body = LocalizationGeneralLogic.GetTextWithParamArray<object>(textMapKey, array);
            }
            else
            {
                body = LocalizationGeneralLogic.GetText(textMapKey, new object[0]);
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowLevelDisplayText, body));
        }

        public void ShowSubHpBar(uint runtimeID)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor(runtimeID) as MonsterActor;
            if (actor != null)
            {
                actor.showSubHpBarWhenAttackLanded = true;
            }
        }

        public void StartInLevelLevelTimer()
        {
            LevelActor levelActor = Singleton<LevelManager>.Instance.levelActor;
            if (!levelActor.HasPlugin<LevelActorTimerPlugin>())
            {
                levelActor.AddPlugin(new LevelActorTimerPlugin(levelActor));
            }
            Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>().StartTiming();
        }

        public void StartInLevelTimeCountDown()
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelActorCountDownPlugin>())
            {
                Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().isTiming = true;
            }
        }

        public void StartTutorial()
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelTutorialHelperPlugin>())
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtTutorialState(EvtTutorialState.State.Start), MPEventDispatchMode.Normal);
            }
        }

        public void StopBGMWithDelay(string bgmName, float delay)
        {
        }

        public void StopInLevelLevelTimer()
        {
            LevelActorTimerPlugin plugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>();
            if (plugin != null)
            {
                plugin.StopTiming();
            }
        }

        public void StopInLevelTimeCountDown()
        {
            if (Singleton<LevelManager>.Instance.levelActor.HasPlugin<LevelActorCountDownPlugin>())
            {
                Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorCountDownPlugin>().isTiming = false;
            }
        }

        public void StopLevelDesign()
        {
            if (this.state != LDState.End)
            {
                this.state = LDState.End;
                this.ClearLuaCoroutines();
                this.ClearAllEventsAndTriggers();
            }
        }

        public void SwitchPlayMode(string playMode)
        {
            Singleton<LevelManager>.Instance.levelActor.levelMode = (LevelActor.Mode) ((int) Enum.Parse(typeof(LevelActor.Mode), playMode));
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                avatar.RefreshController();
            }
        }

        public void TriggerAbilityOnAvatar(bool toAllAvatars, string abilityName, float abilityArgument = 0)
        {
            LevelActor actor = (LevelActor) Singleton<EventManager>.Instance.GetActor(0x21800001);
            if (toAllAvatars)
            {
                foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
                {
                    if (actor.abilityPlugin != null)
                    {
                        actor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), avatar.GetRuntimeID(), abilityArgument);
                    }
                }
            }
            else if (actor.abilityPlugin != null)
            {
                actor.abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), abilityArgument);
            }
        }

        public void TriggerAbilityOnMonster(uint targetID, string abilityName, float abilityArgument = 0)
        {
            Singleton<EventManager>.Instance.GetActor<MonsterActor>(targetID).abilityPlugin.AddOrGetAbilityAndTriggerOnTarget(AbilityData.GetAbilityConfig(abilityName), targetID, abilityArgument);
        }

        public Cutscene TriggerCinema(string name, Transform actor)
        {
            AvatarCinemaType type = name.ToEnum<AvatarCinemaType>(AvatarCinemaType.Victory);
            ICinema cinemaDataByAvatar = Singleton<CinemaDataManager>.Instance.GetCinemaDataByAvatar(this.GetAvatarShortName(actor), type);
            cinemaDataByAvatar.Init(actor);
            cinemaDataByAvatar.Play();
            return cinemaDataByAvatar.GetCutscene();
        }

        public void TryClearEffectListByMapKey(string effectListMapKey)
        {
            Singleton<EffectManager>.Instance.TrySetDestroyUniqueIndexedEffectPattern(effectListMapKey);
        }

        public LDState state { get; private set; }

        [CompilerGenerated]
        private sealed class <DebugSetLocalAvatarIter>c__Iterator36 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LuaTable <$>abilities;
            internal string <$>avatarTypeName;
            internal int <$>level;
            internal int <$>star;
            internal int <$>weaponID;
            internal List<BaseMonoAvatar>.Enumerator <$s_1228>__2;
            internal IEnumerator <$s_1229>__11;
            internal List<MonoAvatarButton>.Enumerator <$s_1230>__13;
            internal List<BaseMonoAvatar> <allAvatars>__0;
            internal BaseMonoAvatar <avatar>__3;
            internal ConfigAvatar <avatarConfig>__9;
            internal AvatarDataItem <avatarDataItem>__8;
            internal MonoAvatarButton <button>__14;
            internal Vector3 <localAvatarForward>__7;
            internal Vector3 <localAvatarXZPosition>__6;
            internal object <obj>__12;
            internal BaseMonoAvatar <targetAvatar>__1;
            internal AvatarActor <targetAvatarActor>__5;
            internal uint <targetAvatarID>__4;
            internal WeaponDataItem <weaponData>__10;
            internal LuaTable abilities;
            internal string avatarTypeName;
            internal int level;
            internal int star;
            internal int weaponID;

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
                        this.<allAvatars>__0 = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                        this.<targetAvatar>__1 = null;
                        this.<$s_1228>__2 = this.<allAvatars>__0.GetEnumerator();
                        try
                        {
                            while (this.<$s_1228>__2.MoveNext())
                            {
                                this.<avatar>__3 = this.<$s_1228>__2.Current;
                                if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(this.<avatar>__3.GetRuntimeID()))
                                {
                                    this.<targetAvatar>__1 = this.<avatar>__3;
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1228>__2.Dispose();
                        }
                        break;

                    case 1:
                        this.<avatarDataItem>__8 = Singleton<AvatarModule>.Instance.GetDummyAvatarDataItem(this.avatarTypeName, this.level, this.star);
                        this.<avatarConfig>__9 = AvatarData.GetAvatarConfig(this.avatarTypeName);
                        this.<weaponData>__10 = (this.weaponID != 0) ? Singleton<StorageModule>.Instance.GetDummyWeaponDataItem(this.weaponID, 1) : Singleton<StorageModule>.Instance.GetDummyFirstWeaponDataByRole(this.<avatarConfig>__9.CommonArguments.RoleName, 1);
                        this.<avatarDataItem>__8.equipsMap[1] = this.<weaponData>__10;
                        this.<targetAvatarID>__4 = Singleton<AvatarManager>.Instance.CreateAvatar(this.<avatarDataItem>__8, false, this.<localAvatarXZPosition>__6, this.<localAvatarForward>__7, this.<targetAvatarID>__4, false, false, false, false);
                        this.<targetAvatar>__1 = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(this.<targetAvatarID>__4);
                        this.<targetAvatar>__1.gameObject.SetActive(false);
                        this.<targetAvatarActor>__5 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.<targetAvatarID>__4);
                        this.<$s_1229>__11 = this.abilities.Values.GetEnumerator();
                        try
                        {
                            while (this.<$s_1229>__11.MoveNext())
                            {
                                this.<obj>__12 = this.<$s_1229>__11.Current;
                                this.<targetAvatarActor>__5.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(this.<obj>__12.ToString()));
                            }
                        }
                        finally
                        {
                            IDisposable disposable = this.<$s_1229>__11 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        this.<$s_1230>__13 = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.avatarButtonContainer.avatarBtnList.GetEnumerator();
                        try
                        {
                            while (this.<$s_1230>__13.MoveNext())
                            {
                                this.<button>__14 = this.<$s_1230>__13.Current;
                                if (this.<button>__14.avatarRuntimeID == this.<targetAvatarID>__4)
                                {
                                    this.<button>__14.SetupAvatar(this.<targetAvatarID>__4);
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1230>__13.Dispose();
                        }
                        Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtAvatarCreated));
                        Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtKilled));
                        Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), this.<targetAvatarID>__4, true);
                        this.$PC = -1;
                        goto Label_03A5;

                    default:
                        goto Label_03A5;
                }
                this.<targetAvatarID>__4 = this.<targetAvatar>__1.GetRuntimeID();
                this.<targetAvatarActor>__5 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.<targetAvatarID>__4);
                this.<localAvatarXZPosition>__6 = this.<targetAvatar>__1.XZPosition;
                this.<localAvatarForward>__7 = this.<targetAvatar>__1.FaceDirection;
                Singleton<EventManager>.Instance.MaskEventType(typeof(EvtAvatarCreated));
                Singleton<EventManager>.Instance.MaskEventType(typeof(EvtKilled));
                this.<targetAvatarActor>__5.ForceKill(0x21800001, KillEffect.KillImmediately);
                Singleton<AvatarManager>.Instance.RemoveAvatarByRuntimeID(this.<targetAvatarID>__4);
                this.$current = null;
                this.$PC = 1;
                return true;
            Label_03A5:
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
        private sealed class <DebugSetLocalAvatarIter>c__Iterator37 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LuaTable <$>abilities;
            internal AvatarDataItem <$>avatarDataItem;
            internal List<BaseMonoAvatar>.Enumerator <$s_1231>__2;
            internal IEnumerator <$s_1232>__8;
            internal List<MonoAvatarButton>.Enumerator <$s_1233>__10;
            internal List<BaseMonoAvatar> <allAvatars>__0;
            internal BaseMonoAvatar <avatar>__3;
            internal MonoAvatarButton <button>__11;
            internal Vector3 <localAvatarForward>__7;
            internal Vector3 <localAvatarXZPosition>__6;
            internal object <obj>__9;
            internal BaseMonoAvatar <targetAvatar>__1;
            internal AvatarActor <targetAvatarActor>__5;
            internal uint <targetAvatarID>__4;
            internal LuaTable abilities;
            internal AvatarDataItem avatarDataItem;

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
                        this.<allAvatars>__0 = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                        this.<targetAvatar>__1 = null;
                        this.<$s_1231>__2 = this.<allAvatars>__0.GetEnumerator();
                        try
                        {
                            while (this.<$s_1231>__2.MoveNext())
                            {
                                this.<avatar>__3 = this.<$s_1231>__2.Current;
                                if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(this.<avatar>__3.GetRuntimeID()))
                                {
                                    this.<targetAvatar>__1 = this.<avatar>__3;
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1231>__2.Dispose();
                        }
                        break;

                    case 1:
                        this.<targetAvatarID>__4 = Singleton<AvatarManager>.Instance.CreateAvatar(this.avatarDataItem, false, this.<localAvatarXZPosition>__6, this.<localAvatarForward>__7, this.<targetAvatarID>__4, false, false, false, false);
                        this.<targetAvatar>__1 = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(this.<targetAvatarID>__4);
                        this.<targetAvatar>__1.gameObject.SetActive(false);
                        this.<targetAvatarActor>__5 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.<targetAvatarID>__4);
                        this.<$s_1232>__8 = this.abilities.Values.GetEnumerator();
                        try
                        {
                            while (this.<$s_1232>__8.MoveNext())
                            {
                                this.<obj>__9 = this.<$s_1232>__8.Current;
                                this.<targetAvatarActor>__5.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(this.<obj>__9.ToString()));
                            }
                        }
                        finally
                        {
                            IDisposable disposable = this.<$s_1232>__8 as IDisposable;
                            if (disposable == null)
                            {
                            }
                            disposable.Dispose();
                        }
                        this.<$s_1233>__10 = Singleton<MainUIManager>.Instance.GetInLevelUICanvas().mainPageContext.avatarButtonContainer.avatarBtnList.GetEnumerator();
                        try
                        {
                            while (this.<$s_1233>__10.MoveNext())
                            {
                                this.<button>__11 = this.<$s_1233>__10.Current;
                                if (this.<button>__11.avatarRuntimeID == this.<targetAvatarID>__4)
                                {
                                    this.<button>__11.SetupAvatar(this.<targetAvatarID>__4);
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1233>__10.Dispose();
                        }
                        Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtAvatarCreated));
                        Singleton<EventManager>.Instance.UnmaskEventType(typeof(EvtKilled));
                        Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID(), this.<targetAvatarID>__4, true);
                        this.$PC = -1;
                        goto Label_0319;

                    default:
                        goto Label_0319;
                }
                this.<targetAvatarID>__4 = this.<targetAvatar>__1.GetRuntimeID();
                this.<targetAvatarActor>__5 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.<targetAvatarID>__4);
                this.<localAvatarXZPosition>__6 = this.<targetAvatar>__1.XZPosition;
                this.<localAvatarForward>__7 = this.<targetAvatar>__1.FaceDirection;
                Singleton<EventManager>.Instance.MaskEventType(typeof(EvtAvatarCreated));
                Singleton<EventManager>.Instance.MaskEventType(typeof(EvtKilled));
                this.<targetAvatarActor>__5.ForceKill(0x21800001, KillEffect.KillImmediately);
                Singleton<AvatarManager>.Instance.RemoveAvatarByRuntimeID(this.<targetAvatarID>__4);
                this.$current = null;
                this.$PC = 1;
                return true;
            Label_0319:
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
        private sealed class <DoTutorialStep>c__AnonStoreyC3
        {
            internal float holdSeconds;
            internal DateTime pointerDownTime;
            internal bool toPauseGame;

            internal void <>m__B7()
            {
                this.pointerDownTime = TimeUtil.Now;
            }

            internal bool <>m__B8()
            {
                bool flag = TimeUtil.Now >= this.pointerDownTime.AddSeconds((double) this.holdSeconds);
                if ((flag && this.toPauseGame) && Singleton<LevelManager>.Instance.IsPaused())
                {
                    Singleton<LevelManager>.Instance.SetPause(false);
                }
                return flag;
            }
        }

        [CompilerGenerated]
        private sealed class <Fade>c__AnonStoreyC2
        {
            internal bool waitFadeInEnd;
            internal bool waitFadeOutEnd;

            internal void <>m__B6()
            {
                if (this.waitFadeOutEnd)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
                }
                else
                {
                    Action fadeEndCallback = delegate {
                        if (this.waitFadeInEnd)
                        {
                            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
                        }
                    };
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().FadeInStageTransitPanel(0.18f, false, null, fadeEndCallback);
                }
            }

            internal void <>m__B9()
            {
                if (this.waitFadeInEnd)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.ExitTransition, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
                }
            }
        }

        [CompilerGenerated]
        private sealed class <LevelEndNewIter>c__Iterator35 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal bool <$>collectAllGoods;
            internal int <$>endCgId;
            internal bool <$>isEndlessLevel;
            internal string <$>result;
            internal List<BaseMonoAvatar>.Enumerator <$s_1214>__5;
            internal LevelDesignManager <>f__this;
            internal GameObject <anchor>__17;
            internal string <animationName>__10;
            internal BaseMonoAvatar <avatar>__6;
            internal string <avatarStateName>__11;
            internal int <bornIx>__13;
            internal MonoSimpleAnimation <cameraAnimation>__15;
            internal MonoStageEnv <env>__12;
            internal Vector3 <forward>__16;
            internal CameraActorLastKillCloseUpPlugin <lastKillPlugin>__1;
            internal BaseMonoAvatar <localAvatar>__4;
            internal MonoMainCamera <mainCamera>__2;
            internal MainCameraActor <mainCameraActor>__0;
            internal MonoInLevelUICanvas <mainCanvas>__8;
            internal bool <playAnimation>__9;
            internal MonoSpawnPoint <spawn>__14;
            internal float <time>__7;
            internal bool <wasInLastKill>__3;
            internal bool collectAllGoods;
            internal int endCgId;
            internal bool isEndlessLevel;
            internal string result;

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
                        this.result = (this.result != null) ? this.result : "win";
                        this.<mainCameraActor>__0 = Singleton<EventManager>.Instance.GetActor<MainCameraActor>(Singleton<CameraManager>.Instance.GetMainCamera().GetRuntimeID());
                        this.<>f__this.MuteInput();
                        this.<>f__this.StopLevelDesign();
                        Singleton<AvatarManager>.Instance.HideHelperAvatar(true);
                        this.<>f__this.ClearPalsyBombProps();
                        this.<lastKillPlugin>__1 = this.<mainCameraActor>__0.GetPlugin<CameraActorLastKillCloseUpPlugin>();
                        this.<mainCamera>__2 = Singleton<CameraManager>.Instance.GetMainCamera();
                        if ((this.<lastKillPlugin>__1 != null) && ((this.result != "win") || !this.<lastKillPlugin>__1.IsPending()))
                        {
                            this.<mainCameraActor>__0.RemovePlugin(this.<lastKillPlugin>__1);
                            this.<lastKillPlugin>__1 = null;
                        }
                        this.<wasInLastKill>__3 = false;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_01D3;

                    case 3:
                        goto Label_0236;

                    case 4:
                        this.<localAvatar>__4.CleanOwnedObjects();
                        Singleton<DynamicObjectManager>.Instance.CleanWhenStageChange();
                        Singleton<CameraManager>.Instance.GetMainCamera().TransitToStatic();
                        this.<time>__7 = 0.45f;
                        this.<mainCanvas>__8 = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
                        this.<mainCanvas>__8.SetWhiteTransitPanelActive(true);
                        this.$current = new WaitForSeconds(this.<time>__7);
                        this.$PC = 5;
                        goto Label_06E0;

                    case 5:
                        this.<localAvatar>__4 = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                        if (!this.<localAvatar>__4.IsAlive())
                        {
                            this.result = "lose";
                        }
                        this.<>f__this.LevelEndWithResult(this.result == "win", this.endCgId);
                        if (this.isEndlessLevel)
                        {
                            this.<playAnimation>__9 = false;
                        }
                        else
                        {
                            this.<playAnimation>__9 = this.<localAvatar>__4.IsAlive();
                        }
                        if (!this.<playAnimation>__9)
                        {
                            goto Label_06C7;
                        }
                        this.<animationName>__10 = !(this.result == "win") ? this.<localAvatar>__4.config.LevelEndAnimation.LevelLoseAnim : this.<localAvatar>__4.config.LevelEndAnimation.LevelWinAnim;
                        this.<avatarStateName>__11 = !(this.result == "win") ? "Fail" : "Victory";
                        this.<env>__12 = Singleton<StageManager>.Instance.GetStageEnv();
                        this.<bornIx>__13 = this.<env>__12.GetNamedSpawnPointIx("Born");
                        if (this.<bornIx>__13 == -1)
                        {
                            this.<bornIx>__13 = 0;
                        }
                        this.<spawn>__14 = this.<env>__12.spawnPoints[this.<bornIx>__13];
                        this.<cameraAnimation>__15 = Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoSimpleAnimation>(this.<animationName>__10, this.<localAvatar>__4.GetRuntimeID());
                        this.<localAvatar>__4.DisableRootMotionAndCollision();
                        this.<localAvatar>__4.transform.position = this.<spawn>__14.XZPosition;
                        this.<forward>__16 = this.<spawn>__14.transform.forward;
                        this.<forward>__16.y = 0f;
                        this.<localAvatar>__4.transform.forward = this.<forward>__16;
                        this.<anchor>__17 = new GameObject("CameraAnchor");
                        this.<anchor>__17.transform.position = this.<spawn>__14.transform.position;
                        this.<anchor>__17.transform.rotation = this.<spawn>__14.transform.rotation;
                        this.<cameraAnimation>__15.SetOwnedParent(this.<anchor>__17.transform);
                        this.<mainCamera>__2.PlayAvatarCameraAnimationThenTransitToFollow(this.<cameraAnimation>__15.GetComponent<Animation>(), this.<localAvatar>__4, MainCameraFollowState.EnterPolarMode.AlongTargetPolar, false);
                        this.<localAvatar>__4.PlayState(this.<avatarStateName>__11);
                        if (this.<avatarStateName>__11 == "Fail")
                        {
                            if (this.<localAvatar>__4 is MonoBronya)
                            {
                                ((MonoBronya) this.<localAvatar>__4).SetMCVisible(false);
                            }
                            this.<localAvatar>__4.DetachWeapon();
                        }
                        goto Label_068B;

                    case 6:
                        goto Label_068B;

                    default:
                        goto Label_06DE;
                }
                while (((this.<lastKillPlugin>__1 != null) && this.<lastKillPlugin>__1.IsPending()) || this.<mainCamera>__2.followState.slowMotionKillState.active)
                {
                    this.<wasInLastKill>__3 = true;
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_06E0;
                }
                this.<lastKillPlugin>__1 = this.<mainCameraActor>__0.GetPlugin<CameraActorLastKillCloseUpPlugin>();
                if (this.<lastKillPlugin>__1 != null)
                {
                    UnityEngine.Debug.LogError("removing last kill plugin again, which should not happen. monster count: " + Singleton<MonsterManager>.Instance.MonsterCount());
                    this.<mainCameraActor>__0.RemovePlugin(this.<lastKillPlugin>__1);
                }
                if (!this.<wasInLastKill>__3)
                {
                    this.<>f__this.ClearAllMonsters();
                    goto Label_01F3;
                }
            Label_01D3:
                while (Singleton<MonsterManager>.Instance.MonsterCount() > 0)
                {
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_06E0;
                }
            Label_01F3:
                if ((this.result == "win") && this.collectAllGoods)
                {
                    this.<>f__this.CollectAllGoods();
                }
            Label_0236:
                while (this.<mainCamera>__2.avatarAnimState.active || this.<mainCamera>__2.levelAnimState.active)
                {
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_06E0;
                }
                if (Singleton<LevelManager>.Instance.IsPaused())
                {
                    Singleton<LevelManager>.Instance.SetPause(false);
                }
                Singleton<LevelManager>.Instance.SetMutePause(true);
                this.<>f__this.SetInLevelUIActive(false);
                this.<localAvatar>__4 = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                Singleton<LevelManager>.Instance.levelActor.abilityPlugin.StopAndDropAll();
                this.<$s_1214>__5 = Singleton<AvatarManager>.Instance.GetAllAvatars().GetEnumerator();
                try
                {
                    while (this.<$s_1214>__5.MoveNext())
                    {
                        this.<avatar>__6 = this.<$s_1214>__5.Current;
                        if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(this.<avatar>__6))
                        {
                            Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.<avatar>__6.GetRuntimeID()).abilityPlugin.StopAndDropAll();
                            this.<avatar>__6.SetHasAdditiveVelocity(false);
                            this.<avatar>__6.SetNeedOverrideVelocity(false);
                        }
                    }
                }
                finally
                {
                    this.<$s_1214>__5.Dispose();
                }
                this.$current = new WaitForSeconds(1.4f);
                this.$PC = 4;
                goto Label_06E0;
            Label_068B:
                while (((this.<cameraAnimation>__15 != null) && (this.<cameraAnimation>__15.GetComponent<Animation>() != null)) && this.<cameraAnimation>__15.GetComponent<Animation>().isPlaying)
                {
                    this.$current = null;
                    this.$PC = 6;
                    goto Label_06E0;
                }
            Label_06C7:
                Singleton<LevelManager>.Instance.SetMutePause(false);
                goto Label_06DE;
                this.$PC = -1;
            Label_06DE:
                return false;
            Label_06E0:
                return true;
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
        private sealed class <LuaCoroutineIter>c__Iterator38 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal LuaThread <$>luaThread;
            internal LevelDesignManager <>f__this;
            internal BaseLDEvent <ldEvent>__2;
            internal int <ret>__0;
            internal LuaTable <yieldRet>__1;
            internal LuaThread luaThread;

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
                        this.luaThread.Start();
                        break;

                    case 1:
                        goto Label_0054;

                    case 2:
                        goto Label_00C7;

                    default:
                        goto Label_00E8;
                }
            Label_0030:
                if (this.<>f__this.state == LevelDesignManager.LDState.Paused)
                {
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_00EA;
                }
            Label_0054:
                this.<ret>__0 = this.luaThread.Resume();
                if (this.<ret>__0 != 1)
                {
                    goto Label_00E8;
                }
                this.<yieldRet>__1 = this.luaThread.translator.getTable(this.luaThread.L, -1);
                this.<ldEvent>__2 = this.<>f__this.CreateLDEventFromTable(this.<yieldRet>__1);
            Label_00C7:
                while (!this.<ldEvent>__2.isDone)
                {
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_00EA;
                }
                goto Label_0030;
                this.$PC = -1;
            Label_00E8:
                return false;
            Label_00EA:
                return true;
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
        private sealed class <PlayCameraAnimationOnAnimatorEntity>c__AnonStoreyC1
        {
            internal BaseAbilityActor abilityActor;
            internal BaseMonoAnimatorEntity animatorEntity;
            internal bool diableEntityFadeMatieral;

            internal void <>m__B4()
            {
                if (this.diableEntityFadeMatieral)
                {
                    this.animatorEntity.SetMonsterMaterialFadeEnabled(false);
                }
                if (this.abilityActor != null)
                {
                    this.abilityActor.isInLevelAnim = true;
                }
                Singleton<EffectManager>.Instance.SetAllAliveEffectPause(true);
            }

            internal void <>m__B5()
            {
                if (this.diableEntityFadeMatieral)
                {
                    this.animatorEntity.SetMonsterMaterialFadeEnabled(true);
                }
                if (this.abilityActor != null)
                {
                    this.abilityActor.isInLevelAnim = false;
                }
                Singleton<EffectManager>.Instance.SetAllAliveEffectPause(false);
            }
        }

        private class EvtTrigger
        {
            public LuaFunction callback;
            public System.Type evtType;
            public bool isCallbackCoroutine;

            public EvtTrigger(string evtTypeName, LuaFunction callback, bool isCallbackCoroutine)
            {
                this.evtType = System.Type.GetType("MoleMole." + evtTypeName);
                this.callback = callback;
                this.isCallbackCoroutine = isCallbackCoroutine;
            }
        }

        private class LDEvtTrigger
        {
            public LuaFunction callback;
            public bool isCallbackCoroutine;
            public BaseLDEvent ldEvent;

            public LDEvtTrigger(BaseLDEvent ldEvent, LuaFunction callback, bool isCallbackCoroutine)
            {
                this.ldEvent = ldEvent;
                this.callback = callback;
                this.isCallbackCoroutine = isCallbackCoroutine;
            }
        }

        public enum LDState
        {
            Start,
            Running,
            End,
            Paused
        }
    }
}

