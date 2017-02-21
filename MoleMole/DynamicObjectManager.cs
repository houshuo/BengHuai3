namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class DynamicObjectManager
    {
        private List<BaseMonoDynamicObject> _dynamicLs = new List<BaseMonoDynamicObject>();
        private Dictionary<uint, BaseMonoDynamicObject> _dynamicObjects = new Dictionary<uint, BaseMonoDynamicObject>();
        private List<PreloadDynamicObjectPrototype> _preloadDynamicObjectProtos;

        private DynamicObjectManager()
        {
        }

        public void CleanWhenStageChange()
        {
            foreach (KeyValuePair<uint, BaseMonoDynamicObject> pair in this._dynamicObjects)
            {
                if (!pair.Value.IsToBeRemove() && !pair.Value.IsOwnerStaticInScene())
                {
                    pair.Value.SetDied();
                }
            }
        }

        public void Core()
        {
            this.RemoveAllRemoveables();
        }

        public AbilityTriggerBullet CreateAbilityLinearTriggerBullet(string bulletType, BaseAbilityActor owner, float speed, MixinTargetting targetting, bool ignoreTimeScale, uint runtimeID, float aliveDuration = -1)
        {
            MonoTriggerBullet entity = this.CreateDynamicObjectEntityInstance<MonoTriggerBullet>(owner.runtimeID, bulletType, runtimeID);
            AbilityTriggerBullet bullet2 = Singleton<EventManager>.Instance.CreateActor<AbilityTriggerBullet>(entity);
            bullet2.Setup(owner, speed, targetting, ignoreTimeScale, aliveDuration);
            return bullet2;
        }

        public AbilityTriggerField CreateAbilityTriggerField(Vector3 initPos, Vector3 initDir, BaseAbilityActor owner, float uniformScale, MixinTargetting targetting, uint runtimeID, bool followOwner = false)
        {
            MonoTriggerField entity = this.CreateDynamicObjectEntityInstance<MonoTriggerField>(owner.runtimeID, "UnitField", initPos, initDir, runtimeID);
            AbilityTriggerField field2 = Singleton<EventManager>.Instance.CreateActor<AbilityTriggerField>(entity);
            field2.Setup(owner, uniformScale, targetting, followOwner);
            return field2;
        }

        public uint CreateAvatarFragmentItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
        {
            int rarity = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, 1).rarity;
            BaseMonoDynamicObject entity = this.CreateDynamicObjectEntityInstance(ownerID, "AvatarFragmentItem", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            MonoGoods goods = entity as MonoGoods;
            goods.actDropAnim = actDropAnim;
            goods.DropItemMetaID = metaId;
            goods.DropItemLevel = level;
            goods.DropItemNum = 1;
            EquipItemActor actor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(entity);
            actor.rarity = rarity;
            return actor.runtimeID;
        }

        public uint CreateBarrierField(uint ownerID, string type, Vector3 initPos, Vector3 initDir, float length)
        {
            Vector3 from = Singleton<AvatarManager>.Instance.GetLocalAvatar().XZPosition - initPos;
            initDir = ((Vector3.Angle(from, initDir) <= 90f) && (Vector3.Angle(from, initDir) >= -90f)) ? initDir : -initDir;
            MonoWall wall = this.CreateDynamicObjectEntityInstance<MonoWall>(ownerID, "Barrier", initPos, initDir, this.GetNextNonSyncedDynamicObjectRuntimeID());
            wall.dynamicType = BaseMonoDynamicObject.DynamicType.Barrier;
            wall.SetCollisionMask(((int) 1) << InLevelData.AVATAR_LAYER);
            Vector3 localScale = wall.transform.localScale;
            localScale.x = length;
            wall.transform.localScale = localScale;
            return wall.GetRuntimeID();
        }

        public uint CreateCoin(uint ownerID, Vector3 initPos, Vector3 initDir, float scoinReward, bool actDropAnim)
        {
            MonoGoods entity = this.CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "Coin", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.actDropAnim = actDropAnim;
            this.TriggerGoodsAttachEffectPattern(entity);
            CoinActor actor = Singleton<EventManager>.Instance.CreateActor<CoinActor>(entity);
            actor.scoinReward = scoinReward;
            return actor.runtimeID;
        }

        private BaseMonoDynamicObject CreateDynamicObjectEntityInstance(uint ownerID, string type, uint runtimeID)
        {
            BaseMonoDynamicObject component = UnityEngine.Object.Instantiate<GameObject>(this.GetDynamicObjectPrototype(type)).GetComponent<BaseMonoDynamicObject>();
            component.Init(runtimeID, ownerID);
            this._dynamicObjects.Add(runtimeID, component);
            this._dynamicLs.Add(component);
            return component;
        }

        private T CreateDynamicObjectEntityInstance<T>(uint ownerID, string type, uint runtimeID) where T: BaseMonoDynamicObject
        {
            return (T) this.CreateDynamicObjectEntityInstance(ownerID, type, runtimeID);
        }

        private BaseMonoDynamicObject CreateDynamicObjectEntityInstance(uint ownerID, string type, Vector3 initPos, Vector3 initDir, uint runtimeID)
        {
            BaseMonoDynamicObject obj2 = this.CreateDynamicObjectEntityInstance(ownerID, type, runtimeID);
            obj2.transform.position = initPos;
            obj2.transform.forward = initDir;
            return obj2;
        }

        private T CreateDynamicObjectEntityInstance<T>(uint ownerID, string type, Vector3 initPos, Vector3 initDir, uint runtimeID) where T: BaseMonoDynamicObject
        {
            return (T) this.CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, runtimeID);
        }

        public uint CreateEquipItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
        {
            int rarity = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, 1).rarity;
            string type = (rarity > 2) ? "EquipItem_02" : "EquipItem_01";
            BaseMonoDynamicObject entity = this.CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            MonoGoods goods = entity as MonoGoods;
            goods.actDropAnim = actDropAnim;
            goods.DropItemMetaID = metaId;
            goods.DropItemLevel = level;
            goods.DropItemNum = 1;
            if (!string.IsNullOrEmpty(goods.InsideEffectPattern))
            {
                List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(goods.InsideEffectPattern, entity, true);
                int num2 = 0;
                int count = list.Count;
                while (num2 < count)
                {
                    this.SetParticleColorByRarity(list[num2].gameObject, rarity);
                    num2++;
                }
            }
            EquipItemActor actor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(entity);
            actor.rarity = rarity;
            return actor.runtimeID;
        }

        public uint CreateEvadeDummy(uint ownerID, string evadeDummyName, Vector3 initPos, Vector3 initDir)
        {
            BaseMonoDynamicObject entity = this.CreateDynamicObjectEntityInstance(ownerID, evadeDummyName, initPos, initDir, this.GetNextNonSyncedDynamicObjectRuntimeID());
            entity.dynamicType = BaseMonoDynamicObject.DynamicType.EvadeDummy;
            EvadeEntityDummy dummy = Singleton<EventManager>.Instance.CreateActor<EvadeEntityDummy>(entity);
            dummy.Setup(ownerID);
            return dummy.runtimeID;
        }

        public uint CreateGood(uint ownerID, string goodType, string abilityName, float argument, Vector3 initPos, Vector3 initDir, bool actDropAnimation, bool forceFlyToAvatar = false)
        {
            MonoGoods entity = this.CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, goodType, initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.actDropAnim = actDropAnimation;
            entity.forceFlyToAvatar = forceFlyToAvatar;
            this.TriggerGoodsAttachEffectPattern(entity);
            AbilityGoodActor actor = Singleton<EventManager>.Instance.CreateActor<AbilityGoodActor>(entity);
            actor.abilityName = abilityName;
            actor.abilityArgument = argument;
            return 0;
        }

        public uint CreateHPMedic(uint ownerID, Vector3 initPos, Vector3 initDir, float healHP, bool actDropAnim)
        {
            MonoGoods entity = this.CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "HPMedic", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.actDropAnim = actDropAnim;
            this.TriggerGoodsAttachEffectPattern(entity);
            HPMedicActor actor = Singleton<EventManager>.Instance.CreateActor<HPMedicActor>(entity);
            actor.healHP = healHP;
            return actor.runtimeID;
        }

        public uint CreateMaterialItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
        {
            int rarity = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, 1).rarity;
            string type = (rarity > 2) ? "MaterialItem_02" : "MaterialItem_01";
            BaseMonoDynamicObject entity = this.CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            MonoGoods goods = entity as MonoGoods;
            goods.actDropAnim = actDropAnim;
            goods.DropItemMetaID = metaId;
            goods.DropItemLevel = level;
            goods.DropItemNum = 1;
            if (!string.IsNullOrEmpty(goods.InsideEffectPattern))
            {
                List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(goods.InsideEffectPattern, entity, true);
                int num2 = 0;
                int count = list.Count;
                while (num2 < count)
                {
                    this.SetParticleColorByRarity(list[num2].gameObject, rarity);
                    num2++;
                }
            }
            EquipItemActor actor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(entity);
            actor.rarity = rarity;
            return actor.runtimeID;
        }

        public uint CreateMonsterExitField(uint ownerID, Vector3 initPos, Vector3 initDir, bool forDefendMode = false)
        {
            MonoTriggerField entity = this.CreateDynamicObjectEntityInstance<MonoTriggerField>(ownerID, "StageMonsterExitField", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.SetCollisionMask(((int) 1) << InLevelData.MONSTER_LAYER);
            MonsterExitFieldActor triggerFieldActor = Singleton<EventManager>.Instance.CreateActor<MonsterExitFieldActor>(entity);
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Prop_LevelMonsterGoal", entity, true);
            if (forDefendMode)
            {
                Singleton<LevelManager>.Instance.levelActor.AddTriggerFieldInDefendMode(triggerFieldActor);
            }
            return triggerFieldActor.runtimeID;
        }

        public uint CreateNavigationArrow(uint ownerID, Vector3 pos, Vector3 forward)
        {
            BaseMonoDynamicObject obj2 = this.CreateDynamicObjectEntityInstance(ownerID, "NavigationArrow", pos, forward, this.GetNextNonSyncedDynamicObjectRuntimeID());
            obj2.dynamicType = BaseMonoDynamicObject.DynamicType.NavigationArrow;
            return obj2.GetRuntimeID();
        }

        public uint CreateSPMedic(uint ownerID, Vector3 initPos, Vector3 initDir, float healSP, bool actDropAnim)
        {
            MonoGoods entity = this.CreateDynamicObjectEntityInstance<MonoGoods>(ownerID, "SPMedic", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.actDropAnim = actDropAnim;
            if (!string.IsNullOrEmpty(entity.AttachEffectPattern))
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(entity.AttachEffectPattern, entity, true);
            }
            SPMedicActor actor = Singleton<EventManager>.Instance.CreateActor<SPMedicActor>(entity);
            actor.healSP = healSP;
            return actor.runtimeID;
        }

        public uint CreateStageExitField(uint ownerID, Vector3 initPos, Vector3 initDir)
        {
            MonoTriggerField entity = this.CreateDynamicObjectEntityInstance<MonoTriggerField>(ownerID, "StageExitField", initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            entity.SetCollisionMask(((int) 1) << InLevelData.AVATAR_LAYER);
            StageExitFieldActor actor = Singleton<EventManager>.Instance.CreateActor<StageExitFieldActor>(entity);
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Prop_LevelGoal", entity, true);
            return actor.runtimeID;
        }

        public uint CreateStigmataItem(uint ownerID, int metaId, Vector3 initPos, Vector3 initDir, bool actDropAnim, int level = 1)
        {
            int rarity = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(metaId, 1).rarity;
            string type = (rarity > 2) ? "StigmataItem_02" : "StigmataItem_01";
            BaseMonoDynamicObject entity = this.CreateDynamicObjectEntityInstance(ownerID, type, initPos, initDir, this.GetNextSyncedDynamicObjectRuntimeID());
            MonoGoods goods = entity as MonoGoods;
            goods.actDropAnim = actDropAnim;
            goods.DropItemMetaID = metaId;
            goods.DropItemLevel = level;
            goods.DropItemNum = 1;
            EquipItemActor actor = Singleton<EventManager>.Instance.CreateActor<EquipItemActor>(entity);
            actor.rarity = rarity;
            return actor.runtimeID;
        }

        public uint CreateStoryScreen(uint ownerID, string type, Vector3 pos, Vector3 dir, int plotID)
        {
            MonoStoryScreen screen = this.CreateDynamicObjectEntityInstance<MonoStoryScreen>(ownerID, type, pos, dir, this.GetNextNonSyncedDynamicObjectRuntimeID());
            screen.SetupView(plotID);
            return screen.GetRuntimeID();
        }

        public void Destroy()
        {
            for (int i = 0; i < this._dynamicLs.Count; i++)
            {
                if (this._dynamicLs[i] != null)
                {
                    UnityEngine.Object.DestroyImmediate(this._dynamicLs[i]);
                }
            }
            this._preloadDynamicObjectProtos.Clear();
            this._preloadDynamicObjectProtos = null;
        }

        public List<MonoGoods> GetAllMonoGoods()
        {
            List<MonoGoods> list = new List<MonoGoods>();
            foreach (KeyValuePair<uint, BaseMonoDynamicObject> pair in this._dynamicObjects)
            {
                if (pair.Value is MonoGoods)
                {
                    list.Add((MonoGoods) pair.Value);
                }
            }
            return list;
        }

        public List<BaseMonoDynamicObject> GetAllNavigationArrows()
        {
            List<BaseMonoDynamicObject> list = new List<BaseMonoDynamicObject>();
            for (int i = 0; i < this._dynamicLs.Count; i++)
            {
                if (this._dynamicLs[i].dynamicType == BaseMonoDynamicObject.DynamicType.NavigationArrow)
                {
                    list.Add(this._dynamicLs[i]);
                }
            }
            return list;
        }

        public BaseMonoDynamicObject GetDynamicObjectByRuntimeID(uint runtimeID)
        {
            return this._dynamicObjects[runtimeID];
        }

        private GameObject GetDynamicObjectPrototype(string type)
        {
            foreach (PreloadDynamicObjectPrototype prototype in this._preloadDynamicObjectProtos)
            {
                if (prototype.type == type)
                {
                    return prototype.gameObj;
                }
            }
            return this.PreloadDynamicObject(type);
        }

        public uint GetNextNonSyncedDynamicObjectRuntimeID()
        {
            return Singleton<RuntimeIDManager>.Instance.GetNextNonSyncedRuntimeID(6);
        }

        public uint GetNextSyncedDynamicObjectRuntimeID()
        {
            return Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
            this.PreloadDynamicObjectPrototypes();
        }

        private GameObject PreloadDynamicObject(string type)
        {
            return this.PreloadDynamicObject(type, DynamicObjectData.dynamicObjectDict[type]);
        }

        private GameObject PreloadDynamicObject(string type, string prefabPath)
        {
            GameObject gameObj = Miscs.LoadResource<GameObject>(prefabPath, BundleType.RESOURCE_FILE);
            if (gameObj != null)
            {
                this._preloadDynamicObjectProtos.Add(new PreloadDynamicObjectPrototype(type, gameObj));
            }
            return gameObj;
        }

        private void PreloadDynamicObjectPrototypes()
        {
            this._preloadDynamicObjectProtos = new List<PreloadDynamicObjectPrototype>();
            ConfigDynamicObjectRegistry dynamicObjectRegistry = DynamicObjectData.GetDynamicObjectRegistry("Entities/DynamicObject/Data/DynamicObject_Level");
            for (int i = 0; i < dynamicObjectRegistry.entries.Length; i++)
            {
                DynamicObjectEntry entry = dynamicObjectRegistry.entries[i];
                this.PreloadDynamicObject(entry.name, entry.prefabPath);
            }
        }

        private BaseMonoDynamicObject RegisterAsDynamicObject(uint ownerID, GameObject go)
        {
            uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
            BaseMonoDynamicObject component = go.GetComponent<BaseMonoDynamicObject>();
            component.Init(nextRuntimeID, ownerID);
            this._dynamicObjects.Add(nextRuntimeID, component);
            this._dynamicLs.Add(component);
            return component;
        }

        public uint RegisterStageEnvTriggerField(uint ownerID, GameObject go)
        {
            MonoTriggerField component = go.GetComponent<MonoTriggerField>();
            if (component == null)
            {
                component = go.AddComponent<MonoTriggerField>();
                BaseMonoDynamicObject entity = this.RegisterAsDynamicObject(ownerID, go);
                Singleton<EventManager>.Instance.CreateActor<TriggerFieldActor>(entity);
            }
            component.SetCollisionMask(((int) 1) << InLevelData.AVATAR_LAYER);
            Collider collider = component.GetComponent<Collider>();
            collider.enabled = false;
            collider.enabled = true;
            return component.GetRuntimeID();
        }

        public void RemoveAllDynamicObjects()
        {
            for (int i = 0; i < this._dynamicLs.Count; i++)
            {
                BaseMonoDynamicObject obj2 = this._dynamicLs[i];
                if (!obj2.IsToBeRemove())
                {
                    obj2.SetDied();
                }
                this.RemoveDynamicObjectByRuntimeID(obj2.GetRuntimeID(), i);
                i--;
            }
        }

        private void RemoveAllRemoveables()
        {
            for (int i = 0; i < this._dynamicLs.Count; i++)
            {
                BaseMonoDynamicObject obj2 = this._dynamicLs[i];
                if (obj2.IsToBeRemove())
                {
                    this.RemoveDynamicObjectByRuntimeID(obj2.GetRuntimeID(), i);
                    i--;
                }
            }
        }

        private void RemoveDynamicObjectByRuntimeID(uint runtimeID, int lsIx)
        {
            Singleton<EventManager>.Instance.TryRemoveActor(runtimeID);
            if (this._dynamicObjects[runtimeID] != null)
            {
                UnityEngine.Object.Destroy(this._dynamicObjects[runtimeID].gameObject);
            }
            this._dynamicObjects.Remove(runtimeID);
            this._dynamicLs.RemoveAt(lsIx);
        }

        public void SetDynamicObjectsVisibility(bool visible)
        {
            foreach (KeyValuePair<uint, BaseMonoDynamicObject> pair in this._dynamicObjects)
            {
                if (!pair.Value.IsToBeRemove())
                {
                    pair.Value.gameObject.SetActive(visible);
                }
            }
        }

        public void SetDynamicObjectsVisibility<T>(bool visible) where T: BaseMonoDynamicObject
        {
            foreach (KeyValuePair<uint, BaseMonoDynamicObject> pair in this._dynamicObjects)
            {
                if ((pair.Value is T) && !pair.Value.IsToBeRemove())
                {
                    pair.Value.gameObject.SetActive(visible);
                }
            }
        }

        public void SetDynamicObjectsVisibilityExept<T>(bool visible) where T: BaseMonoDynamicObject
        {
            foreach (KeyValuePair<uint, BaseMonoDynamicObject> pair in this._dynamicObjects)
            {
                if (!(pair.Value is T) && !pair.Value.IsToBeRemove())
                {
                    pair.Value.gameObject.SetActive(visible);
                }
            }
        }

        public void SetParticleColorByRarity(GameObject obj, int rarity)
        {
            string hexString = MiscData.Config.ItemRarityColorList[Mathf.Clamp(rarity, 0, MiscData.Config.ItemRarityColorList.Count - 1)];
            Color color = Miscs.ParseColor(hexString);
            ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>();
            int index = 0;
            int length = componentsInChildren.Length;
            while (index < length)
            {
                Renderer component = componentsInChildren[index].GetComponent<Renderer>();
                if ((component == null) || (component.material.shader.name.IndexOf("Channel Mix") == -1))
                {
                    componentsInChildren[index].startColor = color;
                }
                index++;
            }
        }

        private void TriggerGoodsAttachEffectPattern(MonoGoods entity)
        {
            if (!string.IsNullOrEmpty(entity.AttachEffectPattern))
            {
                bool flag = true;
                switch (GraphicsSettingData.GetGraphicsRecommendGrade())
                {
                    case GraphicsRecommendGrade.Off:
                    case GraphicsRecommendGrade.Low:
                    {
                        ConfigGraphicsPersonalSetting personalGraphicsSetting = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.PersonalGraphicsSetting;
                        if (!personalGraphicsSetting.IsUserDefinedGrade)
                        {
                            flag = false;
                        }
                        else if (personalGraphicsSetting.RecommendGrade != GraphicsRecommendGrade.High)
                        {
                            flag = false;
                        }
                        break;
                    }
                }
                if (flag)
                {
                    Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(entity.AttachEffectPattern, entity, true);
                }
            }
        }

        public BaseMonoDynamicObject TryGetDynamicObjectByRuntimeID(uint runtimeID)
        {
            BaseMonoDynamicObject obj2;
            this._dynamicObjects.TryGetValue(runtimeID, out obj2);
            return obj2;
        }

        public class PreloadDynamicObjectPrototype
        {
            public GameObject gameObj;
            public string type;

            public PreloadDynamicObjectPrototype(string type, GameObject gameObj)
            {
                this.type = type;
                this.gameObj = gameObj;
            }
        }
    }
}

