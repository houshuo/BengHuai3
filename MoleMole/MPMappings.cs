namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class MPMappings
    {
        private static bool _inited;
        public static Dictionary<System.Type, Struct> _structCache;
        public static Dictionary<System.Type, Table> _tableCache;
        public static Table[] cachedRecvPackets;
        public static IntMapping<System.Type> MPPacketMapping;
        public static IntMapping<System.Type> MPPeerMapping;

        static MPMappings()
        {
            System.Type[] arr = new System.Type[] { 
                typeof(Packet_Ability_InvocationTable), typeof(Packet_Basic_Destroy), typeof(Packet_Basic_Instantiate), typeof(Packet_Entity_AnimatorParameterChange), typeof(Packet_Entity_AnimatorStateChange), typeof(Packet_Entity_Kill), typeof(Packet_Entity_TransformSync), typeof(Packet_Event_EvtAttackLanded), typeof(Packet_Event_EvtBeingHit), typeof(Packet_Event_EvtBulletHit), typeof(Packet_Event_EvtEvadeSuccess), typeof(Packet_Event_EvtHittingOther), typeof(Packet_Level_CreateStageFullData), typeof(Packet_Level_PeerStageReady), typeof(Packet_Level_RequestLevelBuff), typeof(Packet_Level_ResultLevelBuff), 
                typeof(Packet_Monster_MonsterCreation)
             };
            MPPacketMapping = new IntMapping<System.Type>(arr);
            System.Type[] typeArray2 = new System.Type[] { typeof(AvatarIdentity), typeof(LevelIdentity), typeof(MonsterIdentity), typeof(PeerIdentity) };
            MPPeerMapping = new IntMapping<System.Type>(typeArray2);
            _tableCache = new Dictionary<System.Type, Table>();
            _structCache = new Dictionary<System.Type, Struct>();
        }

        public static Vector3 Deserialize(MPVector3 table)
        {
            return new Vector3 { x = table.X, y = table.Y, z = table.Z };
        }

        public static AttackData Deserialize(MPAttackData table, AttackData obj)
        {
            if (obj == null)
            {
                obj = new AttackData();
            }
            obj.attackerAniDamageRatio = table.AttackerAniDamageRatio;
            obj.attackerClass = (EntityClass) table.AttackerClass;
            obj.attackerNature = (EntityNature) table.AttackerNature;
            obj.attackerCategory = table.AttackerCategory;
            obj.attackerCritChance = table.AttackerCritChance;
            obj.attackerCritDamageRatio = table.AttackerCritDamageRatio;
            obj.attackerLevel = table.AttackerLevel;
            obj.attackerShieldDamageRatio = table.AttackerShieldDamageRatio;
            obj.attackerShieldDamageDelta = table.AttackerShieldDamageDelta;
            obj.attackerAttackPercentage = table.AttackerAttackPercentage;
            obj.attackerAttackValue = table.AttackerAttackValue;
            obj.addedAttackRatio = table.AddedAttackRatio;
            obj.addedDamageRatio = table.AddedDamageRatio;
            obj.attackerAddedAttackValue = table.AttackerAddedAttackValue;
            obj.attackerNormalDamage = table.AttackerNormalDamage;
            obj.attackerNormalDamagePercentage = table.AttackerNormalDamagePercentage;
            obj.addedAttackerNormalDamageRatio = table.AddedAttackerNormalDamageRatio;
            obj.attackerFireDamage = table.AttackerFireDamage;
            obj.attackerFireDamagePercentage = table.AttackerFireDamagePercentage;
            obj.addedAttackerFireDamageRatio = table.AddedAttackerFireDamageRatio;
            obj.attackerThunderDamage = table.AttackerThunderDamage;
            obj.attackerThunderDamagePercentage = table.AttackerThunderDamagePercentage;
            obj.addedAttackerThunderDamageRatio = table.AddedAttackerThunderDamageRatio;
            obj.attackerIceDamage = table.AttackerIceDamage;
            obj.attackerIceDamagePercentage = table.AttackerIceDamagePercentage;
            obj.addedAttackerIceDamageRatio = table.AddedAttackerIceDamageRatio;
            obj.attackerAlienDamage = table.AttackerAlienDamage;
            obj.attackerAlienDamagePercentage = table.AttackerAlienDamagePercentage;
            obj.addedAttackerAlienDamageRatio = table.AddedAttackerAlienDamageRatio;
            obj.attackeeAniDefenceRatio = table.AttackeeAniDefenceRatio;
            obj.attackeeNature = (EntityNature) table.AttackeeNature;
            obj.attackeeClass = (EntityClass) table.AttackeeClass;
            obj.natureDamageRatio = table.NatureDamageRatio;
            obj.damage = table.Damage;
            obj.plainDamage = table.PlainDamage;
            obj.thunderDamage = table.ThunderDamage;
            obj.iceDamage = table.IceDamage;
            obj.alienDamage = table.AlienDamage;
            obj.aniDamageRatio = table.AniDamageRatio;
            obj.retreatVelocity = table.RetreatVelocity;
            obj.frameHalt = table.FrameHalt;
            obj.isAnimEventAttack = table.IsAnimEventAttack;
            obj.isInComboCount = table.IsInComboCount;
            obj.attackCategoryTag = (AttackResult.AttackCategoryTag) table.AttackCategoryTag;
            obj.hitType = (AttackResult.ActorHitType) table.HitType;
            obj.hitFlag = (AttackResult.ActorHitFlag) table.HitFlag;
            obj.hitLevel = (AttackResult.ActorHitLevel) table.HitLevel;
            obj.hitEffect = (AttackResult.AnimatorHitEffect) table.HitEffect;
            obj.hitEffectAux = (AttackResult.AnimatorHitEffectAux) table.HitEffectAux;
            obj.hitEffectPattern = (AttackResult.HitEffectPattern) table.HitEffectPattern;
            obj.killEffect = (KillEffect) table.KillEffect;
            obj.rejectState = (AttackResult.RejectType) table.RejectState;
            obj.isFromBullet = table.IsFromBullet;
            obj.noTriggerEvadeAndDefend = table.NoTriggerEvadeAndDefend;
            if (table.GetHitCollision(GetCachedTable<MPHitCollision>()) != null)
            {
                obj.hitCollision = Deserialize(GetCachedTable<MPHitCollision>(), new AttackResult.HitCollsion());
            }
            obj.attackEffectPattern = IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(table.AttackEffectPattern);
            obj.beHitEffectPattern = IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(table.BeHitEffectPattern);
            obj.attackCameraShake = IndexedConfig<ConfigEntityCameraShake>.Mapping.TryGet(table.AttackCameraShake);
            return obj;
        }

        public static AttackResult Deserialize(MPAttackResult table, AttackResult obj)
        {
            if (obj == null)
            {
                obj = new AttackResult();
            }
            obj.damage = table.Damage;
            obj.plainDamage = table.PlainDamage;
            obj.thunderDamage = table.ThunderDamage;
            obj.iceDamage = table.IceDamage;
            obj.alienDamage = table.AlienDamage;
            obj.aniDamageRatio = table.AniDamageRatio;
            obj.retreatVelocity = table.RetreatVelocity;
            obj.frameHalt = table.FrameHalt;
            obj.isAnimEventAttack = table.IsAnimEventAttack;
            obj.isInComboCount = table.IsInComboCount;
            obj.attackCategoryTag = (AttackResult.AttackCategoryTag) table.AttackCategoryTag;
            obj.hitType = (AttackResult.ActorHitType) table.HitType;
            obj.hitFlag = (AttackResult.ActorHitFlag) table.HitFlag;
            obj.hitLevel = (AttackResult.ActorHitLevel) table.HitLevel;
            obj.hitEffect = (AttackResult.AnimatorHitEffect) table.HitEffect;
            obj.hitEffectAux = (AttackResult.AnimatorHitEffectAux) table.HitEffectAux;
            obj.hitEffectPattern = (AttackResult.HitEffectPattern) table.HitEffectPattern;
            obj.killEffect = (KillEffect) table.KillEffect;
            obj.rejectState = (AttackResult.RejectType) table.RejectState;
            obj.isFromBullet = table.IsFromBullet;
            obj.noTriggerEvadeAndDefend = table.NoTriggerEvadeAndDefend;
            if (table.GetHitCollision(GetCachedTable<MPHitCollision>()) != null)
            {
                obj.hitCollision = Deserialize(GetCachedTable<MPHitCollision>(), new AttackResult.HitCollsion());
            }
            obj.attackEffectPattern = IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(table.AttackEffectPattern);
            obj.beHitEffectPattern = IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(table.BeHitEffectPattern);
            obj.attackCameraShake = IndexedConfig<ConfigEntityCameraShake>.Mapping.TryGet(table.AttackCameraShake);
            return obj;
        }

        public static MoleMole.MPAvatarDataItem Deserialize(MoleMole.MPProtocol.MPAvatarDataItem table, MoleMole.MPAvatarDataItem obj)
        {
            if (obj == null)
            {
                obj = new MoleMole.MPAvatarDataItem();
            }
            obj.avatarID = table.AvatarID;
            obj.level = table.Level;
            obj.star = table.Star;
            obj.finalHP = table.FinalHP;
            obj.finalSP = table.FinalSP;
            obj.finalAttack = table.FinalAttack;
            obj.finalCritical = table.FinalCritical;
            obj.finalDefense = table.FinalDefense;
            return obj;
        }

        public static AttackResult.HitCollsion Deserialize(MPHitCollision table, AttackResult.HitCollsion obj)
        {
            if (obj == null)
            {
                obj = new AttackResult.HitCollsion();
            }
            if (table.GetHitDir(GetCachedStruct<MPVector3>()) != null)
            {
                obj.hitDir = Deserialize(GetCachedStruct<MPVector3>());
            }
            if (table.GetHitPoint(GetCachedStruct<MPVector3>()) != null)
            {
                obj.hitPoint = Deserialize(GetCachedStruct<MPVector3>());
            }
            return obj;
        }

        public static MoleMole.MPStageData Deserialize(MoleMole.MPProtocol.MPStageData table, MoleMole.MPStageData obj)
        {
            if (obj == null)
            {
                obj = new MoleMole.MPStageData();
            }
            obj.stageName = table.StageName;
            return obj;
        }

        public static EvtAttackLanded Deserialize(Packet_Event_EvtAttackLanded table, EvtAttackLanded obj)
        {
            if (obj == null)
            {
                obj = new EvtAttackLanded();
            }
            obj.targetID = table.TargetID;
            obj.attackeeID = table.AttackeeID;
            obj.animEventID = table.AnimEventID;
            if (table.GetAttackResult(GetCachedTable<MPAttackResult>()) != null)
            {
                obj.attackResult = Deserialize(GetCachedTable<MPAttackResult>(), new AttackResult());
            }
            return obj;
        }

        public static EvtBeingHit Deserialize(Packet_Event_EvtBeingHit table, EvtBeingHit obj)
        {
            if (obj == null)
            {
                obj = new EvtBeingHit();
            }
            obj.targetID = table.TargetID;
            obj.sourceID = table.SourceID;
            obj.animEventID = table.AnimEventID;
            if (table.GetAttackData(GetCachedTable<MPAttackData>()) != null)
            {
                obj.attackData = Deserialize(GetCachedTable<MPAttackData>(), new AttackData());
            }
            obj.beHitEffect = (BeHitEffect) table.BeHitEffect;
            obj.resolvedDamage = table.ResolvedDamage;
            return obj;
        }

        public static EvtBulletHit Deserialize(Packet_Event_EvtBulletHit table, EvtBulletHit obj)
        {
            if (obj == null)
            {
                obj = new EvtBulletHit();
            }
            obj.targetID = table.TargetID;
            obj.otherID = table.OtherID;
            if (table.GetHitCollision(GetCachedTable<MPHitCollision>()) != null)
            {
                obj.hitCollision = Deserialize(GetCachedTable<MPHitCollision>(), new AttackResult.HitCollsion());
            }
            obj.hitEnvironment = table.HitEnvironment;
            obj.hitGround = table.HitGround;
            obj.cannotBeReflected = table.CannotBeReflected;
            return obj;
        }

        public static EvtEvadeSuccess Deserialize(Packet_Event_EvtEvadeSuccess table, EvtEvadeSuccess obj)
        {
            if (obj == null)
            {
                obj = new EvtEvadeSuccess();
            }
            obj.targetID = table.TargetID;
            obj.attackerID = table.AttackerID;
            obj.skillID = table.SkillID;
            if (table.GetAttackData(GetCachedTable<MPAttackData>()) != null)
            {
                obj.attackData = Deserialize(GetCachedTable<MPAttackData>(), new AttackData());
            }
            return obj;
        }

        public static EvtHittingOther Deserialize(Packet_Event_EvtHittingOther table, EvtHittingOther obj)
        {
            if (obj == null)
            {
                obj = new EvtHittingOther();
            }
            obj.targetID = table.TargetID;
            obj.toID = table.ToID;
            obj.animEventID = table.AnimEventID;
            if (table.GetAttackData(GetCachedTable<MPAttackData>()) != null)
            {
                obj.attackData = Deserialize(GetCachedTable<MPAttackData>(), new AttackData());
            }
            if (table.GetHitCollision(GetCachedTable<MPHitCollision>()) != null)
            {
                obj.hitCollision = Deserialize(GetCachedTable<MPHitCollision>(), new AttackResult.HitCollsion());
            }
            return obj;
        }

        public static object DeserializeToObject(Table table, object obj)
        {
            if (table.GetType() == typeof(MoleMole.MPProtocol.MPStageData))
            {
                if (obj == null)
                {
                    obj = new MoleMole.MPStageData();
                }
                return Deserialize((MoleMole.MPProtocol.MPStageData) table, (MoleMole.MPStageData) obj);
            }
            if (table.GetType() == typeof(MoleMole.MPProtocol.MPAvatarDataItem))
            {
                if (obj == null)
                {
                    obj = new MoleMole.MPAvatarDataItem();
                }
                return Deserialize((MoleMole.MPProtocol.MPAvatarDataItem) table, (MoleMole.MPAvatarDataItem) obj);
            }
            if (table.GetType() == typeof(Packet_Event_EvtHittingOther))
            {
                if (obj == null)
                {
                    obj = new EvtHittingOther();
                }
                return Deserialize((Packet_Event_EvtHittingOther) table, (EvtHittingOther) obj);
            }
            if (table.GetType() == typeof(Packet_Event_EvtBeingHit))
            {
                if (obj == null)
                {
                    obj = new EvtBeingHit();
                }
                return Deserialize((Packet_Event_EvtBeingHit) table, (EvtBeingHit) obj);
            }
            if (table.GetType() == typeof(Packet_Event_EvtAttackLanded))
            {
                if (obj == null)
                {
                    obj = new EvtAttackLanded();
                }
                return Deserialize((Packet_Event_EvtAttackLanded) table, (EvtAttackLanded) obj);
            }
            if (table.GetType() == typeof(Packet_Event_EvtEvadeSuccess))
            {
                if (obj == null)
                {
                    obj = new EvtEvadeSuccess();
                }
                return Deserialize((Packet_Event_EvtEvadeSuccess) table, (EvtEvadeSuccess) obj);
            }
            if (table.GetType() == typeof(Packet_Event_EvtBulletHit))
            {
                if (obj == null)
                {
                    obj = new EvtBulletHit();
                }
                return Deserialize((Packet_Event_EvtBulletHit) table, (EvtBulletHit) obj);
            }
            if (table.GetType() == typeof(MPAttackData))
            {
                if (obj == null)
                {
                    obj = new AttackData();
                }
                return Deserialize((MPAttackData) table, (AttackData) obj);
            }
            if (table.GetType() == typeof(MPAttackResult))
            {
                if (obj == null)
                {
                    obj = new AttackResult();
                }
                return Deserialize((MPAttackResult) table, (AttackResult) obj);
            }
            if (table.GetType() != typeof(MPHitCollision))
            {
                return obj;
            }
            if (obj == null)
            {
                obj = new AttackResult.HitCollsion();
            }
            return Deserialize((MPHitCollision) table, (AttackResult.HitCollsion) obj);
        }

        public static T GetCachedStruct<T>() where T: Struct, new()
        {
            Struct struct2;
            if (!_structCache.TryGetValue(typeof(T), out struct2))
            {
                struct2 = Activator.CreateInstance<T>();
                _structCache.Add(typeof(T), struct2);
            }
            return (T) struct2;
        }

        public static T GetCachedTable<T>() where T: Table, new()
        {
            Table table;
            if (!_tableCache.TryGetValue(typeof(T), out table))
            {
                table = Activator.CreateInstance<T>();
                _tableCache.Add(typeof(T), table);
            }
            return (T) table;
        }

        public static void InitMPMappings()
        {
            if (!_inited)
            {
                int length = MPPacketMapping.length;
                cachedRecvPackets = new Table[length + 1];
                for (int i = 1; i <= length; i++)
                {
                    System.Type type = MPPacketMapping.Get(i);
                    cachedRecvPackets[i] = (Table) Activator.CreateInstance(type);
                }
                _inited = true;
            }
        }

        public static Offset<MPAttackData> Serialize(FlatBufferBuilder builder, AttackData obj)
        {
            Offset<MPHitCollision> hitCollisionOffset = new Offset<MPHitCollision>();
            if (obj.hitCollision != null)
            {
                hitCollisionOffset = Serialize(builder, obj.hitCollision);
            }
            MPAttackData.StartMPAttackData(builder);
            MPAttackData.AddAttackerAniDamageRatio(builder, obj.attackerAniDamageRatio);
            MPAttackData.AddAttackerClass(builder, (byte) obj.attackerClass);
            MPAttackData.AddAttackerNature(builder, (byte) obj.attackerNature);
            MPAttackData.AddAttackerCategory(builder, obj.attackerCategory);
            MPAttackData.AddAttackerCritChance(builder, obj.attackerCritChance);
            MPAttackData.AddAttackerCritDamageRatio(builder, obj.attackerCritDamageRatio);
            MPAttackData.AddAttackerLevel(builder, obj.attackerLevel);
            MPAttackData.AddAttackerShieldDamageRatio(builder, obj.attackerShieldDamageRatio);
            MPAttackData.AddAttackerShieldDamageDelta(builder, obj.attackerShieldDamageDelta);
            MPAttackData.AddAttackerAttackPercentage(builder, obj.attackerAttackPercentage);
            MPAttackData.AddAttackerAttackValue(builder, obj.attackerAttackValue);
            MPAttackData.AddAddedAttackRatio(builder, obj.addedAttackRatio);
            MPAttackData.AddAddedDamageRatio(builder, obj.addedDamageRatio);
            MPAttackData.AddAttackerAddedAttackValue(builder, obj.attackerAddedAttackValue);
            MPAttackData.AddAttackerNormalDamage(builder, obj.attackerNormalDamage);
            MPAttackData.AddAttackerNormalDamagePercentage(builder, obj.attackerNormalDamagePercentage);
            MPAttackData.AddAddedAttackerNormalDamageRatio(builder, obj.addedAttackerNormalDamageRatio);
            MPAttackData.AddAttackerFireDamage(builder, obj.attackerFireDamage);
            MPAttackData.AddAttackerFireDamagePercentage(builder, obj.attackerFireDamagePercentage);
            MPAttackData.AddAddedAttackerFireDamageRatio(builder, obj.addedAttackerFireDamageRatio);
            MPAttackData.AddAttackerThunderDamage(builder, obj.attackerThunderDamage);
            MPAttackData.AddAttackerThunderDamagePercentage(builder, obj.attackerThunderDamagePercentage);
            MPAttackData.AddAddedAttackerThunderDamageRatio(builder, obj.addedAttackerThunderDamageRatio);
            MPAttackData.AddAttackerIceDamage(builder, obj.attackerIceDamage);
            MPAttackData.AddAttackerIceDamagePercentage(builder, obj.attackerIceDamagePercentage);
            MPAttackData.AddAddedAttackerIceDamageRatio(builder, obj.addedAttackerIceDamageRatio);
            MPAttackData.AddAttackerAlienDamage(builder, obj.attackerAlienDamage);
            MPAttackData.AddAttackerAlienDamagePercentage(builder, obj.attackerAlienDamagePercentage);
            MPAttackData.AddAddedAttackerAlienDamageRatio(builder, obj.addedAttackerAlienDamageRatio);
            MPAttackData.AddAttackeeAniDefenceRatio(builder, obj.attackeeAniDefenceRatio);
            MPAttackData.AddAttackeeNature(builder, (byte) obj.attackeeNature);
            MPAttackData.AddAttackeeClass(builder, (byte) obj.attackeeClass);
            MPAttackData.AddNatureDamageRatio(builder, obj.natureDamageRatio);
            MPAttackData.AddDamage(builder, obj.damage);
            MPAttackData.AddPlainDamage(builder, obj.plainDamage);
            MPAttackData.AddThunderDamage(builder, obj.thunderDamage);
            MPAttackData.AddIceDamage(builder, obj.iceDamage);
            MPAttackData.AddAlienDamage(builder, obj.alienDamage);
            MPAttackData.AddAniDamageRatio(builder, obj.aniDamageRatio);
            MPAttackData.AddRetreatVelocity(builder, obj.retreatVelocity);
            MPAttackData.AddFrameHalt(builder, obj.frameHalt);
            MPAttackData.AddIsAnimEventAttack(builder, obj.isAnimEventAttack);
            MPAttackData.AddIsInComboCount(builder, obj.isInComboCount);
            MPAttackData.AddAttackCategoryTag(builder, (int) obj.attackCategoryTag);
            MPAttackData.AddHitType(builder, (byte) obj.hitType);
            MPAttackData.AddHitFlag(builder, (int) obj.hitFlag);
            MPAttackData.AddHitLevel(builder, (byte) obj.hitLevel);
            MPAttackData.AddHitEffect(builder, (byte) obj.hitEffect);
            MPAttackData.AddHitEffectAux(builder, (byte) obj.hitEffectAux);
            MPAttackData.AddHitEffectPattern(builder, (byte) obj.hitEffectPattern);
            MPAttackData.AddKillEffect(builder, (byte) obj.killEffect);
            MPAttackData.AddRejectState(builder, (byte) obj.rejectState);
            MPAttackData.AddIsFromBullet(builder, obj.isFromBullet);
            MPAttackData.AddNoTriggerEvadeAndDefend(builder, obj.noTriggerEvadeAndDefend);
            MPAttackData.AddHitCollision(builder, hitCollisionOffset);
            MPAttackData.AddAttackEffectPattern(builder, (ushort) IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(obj.attackEffectPattern));
            MPAttackData.AddBeHitEffectPattern(builder, (ushort) IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(obj.beHitEffectPattern));
            MPAttackData.AddAttackCameraShake(builder, (ushort) IndexedConfig<ConfigEntityCameraShake>.Mapping.TryGet(obj.attackCameraShake));
            return MPAttackData.EndMPAttackData(builder);
        }

        public static Offset<MPAttackResult> Serialize(FlatBufferBuilder builder, AttackResult obj)
        {
            Offset<MPHitCollision> hitCollisionOffset = new Offset<MPHitCollision>();
            if (obj.hitCollision != null)
            {
                hitCollisionOffset = Serialize(builder, obj.hitCollision);
            }
            MPAttackResult.StartMPAttackResult(builder);
            MPAttackResult.AddDamage(builder, obj.damage);
            MPAttackResult.AddPlainDamage(builder, obj.plainDamage);
            MPAttackResult.AddThunderDamage(builder, obj.thunderDamage);
            MPAttackResult.AddIceDamage(builder, obj.iceDamage);
            MPAttackResult.AddAlienDamage(builder, obj.alienDamage);
            MPAttackResult.AddAniDamageRatio(builder, obj.aniDamageRatio);
            MPAttackResult.AddRetreatVelocity(builder, obj.retreatVelocity);
            MPAttackResult.AddFrameHalt(builder, obj.frameHalt);
            MPAttackResult.AddIsAnimEventAttack(builder, obj.isAnimEventAttack);
            MPAttackResult.AddIsInComboCount(builder, obj.isInComboCount);
            MPAttackResult.AddAttackCategoryTag(builder, (int) obj.attackCategoryTag);
            MPAttackResult.AddHitType(builder, (byte) obj.hitType);
            MPAttackResult.AddHitFlag(builder, (int) obj.hitFlag);
            MPAttackResult.AddHitLevel(builder, (byte) obj.hitLevel);
            MPAttackResult.AddHitEffect(builder, (byte) obj.hitEffect);
            MPAttackResult.AddHitEffectAux(builder, (byte) obj.hitEffectAux);
            MPAttackResult.AddHitEffectPattern(builder, (byte) obj.hitEffectPattern);
            MPAttackResult.AddKillEffect(builder, (byte) obj.killEffect);
            MPAttackResult.AddRejectState(builder, (byte) obj.rejectState);
            MPAttackResult.AddIsFromBullet(builder, obj.isFromBullet);
            MPAttackResult.AddNoTriggerEvadeAndDefend(builder, obj.noTriggerEvadeAndDefend);
            MPAttackResult.AddHitCollision(builder, hitCollisionOffset);
            MPAttackResult.AddAttackEffectPattern(builder, (ushort) IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(obj.attackEffectPattern));
            MPAttackResult.AddBeHitEffectPattern(builder, (ushort) IndexedConfig<ConfigEntityAttackEffect>.Mapping.TryGet(obj.beHitEffectPattern));
            MPAttackResult.AddAttackCameraShake(builder, (ushort) IndexedConfig<ConfigEntityCameraShake>.Mapping.TryGet(obj.attackCameraShake));
            return MPAttackResult.EndMPAttackResult(builder);
        }

        public static Offset<MPHitCollision> Serialize(FlatBufferBuilder builder, AttackResult.HitCollsion obj)
        {
            MPHitCollision.StartMPHitCollision(builder);
            Offset<MPVector3> hitDirOffset = Serialize(builder, obj.hitDir);
            MPHitCollision.AddHitDir(builder, hitDirOffset);
            Offset<MPVector3> hitPointOffset = Serialize(builder, obj.hitPoint);
            MPHitCollision.AddHitPoint(builder, hitPointOffset);
            return MPHitCollision.EndMPHitCollision(builder);
        }

        public static Offset<Packet_Event_EvtAttackLanded> Serialize(FlatBufferBuilder builder, EvtAttackLanded obj)
        {
            StringOffset animEventIDOffset = new StringOffset();
            if (obj.animEventID != null)
            {
                animEventIDOffset = builder.CreateString(obj.animEventID);
            }
            Offset<MPAttackResult> attackResultOffset = new Offset<MPAttackResult>();
            if (obj.attackResult != null)
            {
                attackResultOffset = Serialize(builder, obj.attackResult);
            }
            Packet_Event_EvtAttackLanded.StartPacket_Event_EvtAttackLanded(builder);
            Packet_Event_EvtAttackLanded.AddTargetID(builder, obj.targetID);
            Packet_Event_EvtAttackLanded.AddAttackeeID(builder, obj.attackeeID);
            Packet_Event_EvtAttackLanded.AddAnimEventID(builder, animEventIDOffset);
            Packet_Event_EvtAttackLanded.AddAttackResult(builder, attackResultOffset);
            return Packet_Event_EvtAttackLanded.EndPacket_Event_EvtAttackLanded(builder);
        }

        public static Offset<Packet_Event_EvtBeingHit> Serialize(FlatBufferBuilder builder, EvtBeingHit obj)
        {
            StringOffset animEventIDOffset = new StringOffset();
            if (obj.animEventID != null)
            {
                animEventIDOffset = builder.CreateString(obj.animEventID);
            }
            Offset<MPAttackData> attackDataOffset = new Offset<MPAttackData>();
            if (obj.attackData != null)
            {
                attackDataOffset = Serialize(builder, obj.attackData);
            }
            Packet_Event_EvtBeingHit.StartPacket_Event_EvtBeingHit(builder);
            Packet_Event_EvtBeingHit.AddTargetID(builder, obj.targetID);
            Packet_Event_EvtBeingHit.AddSourceID(builder, obj.sourceID);
            Packet_Event_EvtBeingHit.AddAnimEventID(builder, animEventIDOffset);
            Packet_Event_EvtBeingHit.AddAttackData(builder, attackDataOffset);
            Packet_Event_EvtBeingHit.AddBeHitEffect(builder, (byte) obj.beHitEffect);
            Packet_Event_EvtBeingHit.AddResolvedDamage(builder, obj.resolvedDamage);
            return Packet_Event_EvtBeingHit.EndPacket_Event_EvtBeingHit(builder);
        }

        public static Offset<Packet_Event_EvtBulletHit> Serialize(FlatBufferBuilder builder, EvtBulletHit obj)
        {
            Offset<MPHitCollision> hitCollisionOffset = new Offset<MPHitCollision>();
            if (obj.hitCollision != null)
            {
                hitCollisionOffset = Serialize(builder, obj.hitCollision);
            }
            Packet_Event_EvtBulletHit.StartPacket_Event_EvtBulletHit(builder);
            Packet_Event_EvtBulletHit.AddTargetID(builder, obj.targetID);
            Packet_Event_EvtBulletHit.AddOtherID(builder, obj.otherID);
            Packet_Event_EvtBulletHit.AddHitCollision(builder, hitCollisionOffset);
            Packet_Event_EvtBulletHit.AddHitEnvironment(builder, obj.hitEnvironment);
            Packet_Event_EvtBulletHit.AddHitGround(builder, obj.hitGround);
            Packet_Event_EvtBulletHit.AddCannotBeReflected(builder, obj.cannotBeReflected);
            return Packet_Event_EvtBulletHit.EndPacket_Event_EvtBulletHit(builder);
        }

        public static Offset<Packet_Event_EvtEvadeSuccess> Serialize(FlatBufferBuilder builder, EvtEvadeSuccess obj)
        {
            StringOffset skillIDOffset = new StringOffset();
            if (obj.skillID != null)
            {
                skillIDOffset = builder.CreateString(obj.skillID);
            }
            Offset<MPAttackData> attackDataOffset = new Offset<MPAttackData>();
            if (obj.attackData != null)
            {
                attackDataOffset = Serialize(builder, obj.attackData);
            }
            Packet_Event_EvtEvadeSuccess.StartPacket_Event_EvtEvadeSuccess(builder);
            Packet_Event_EvtEvadeSuccess.AddTargetID(builder, obj.targetID);
            Packet_Event_EvtEvadeSuccess.AddAttackerID(builder, obj.attackerID);
            Packet_Event_EvtEvadeSuccess.AddSkillID(builder, skillIDOffset);
            Packet_Event_EvtEvadeSuccess.AddAttackData(builder, attackDataOffset);
            return Packet_Event_EvtEvadeSuccess.EndPacket_Event_EvtEvadeSuccess(builder);
        }

        public static Offset<Packet_Event_EvtHittingOther> Serialize(FlatBufferBuilder builder, EvtHittingOther obj)
        {
            StringOffset animEventIDOffset = new StringOffset();
            if (obj.animEventID != null)
            {
                animEventIDOffset = builder.CreateString(obj.animEventID);
            }
            Offset<MPAttackData> attackDataOffset = new Offset<MPAttackData>();
            if (obj.attackData != null)
            {
                attackDataOffset = Serialize(builder, obj.attackData);
            }
            Offset<MPHitCollision> hitCollisionOffset = new Offset<MPHitCollision>();
            if (obj.hitCollision != null)
            {
                hitCollisionOffset = Serialize(builder, obj.hitCollision);
            }
            Packet_Event_EvtHittingOther.StartPacket_Event_EvtHittingOther(builder);
            Packet_Event_EvtHittingOther.AddTargetID(builder, obj.targetID);
            Packet_Event_EvtHittingOther.AddToID(builder, obj.toID);
            Packet_Event_EvtHittingOther.AddAnimEventID(builder, animEventIDOffset);
            Packet_Event_EvtHittingOther.AddAttackData(builder, attackDataOffset);
            Packet_Event_EvtHittingOther.AddHitCollision(builder, hitCollisionOffset);
            return Packet_Event_EvtHittingOther.EndPacket_Event_EvtHittingOther(builder);
        }

        public static Offset<MoleMole.MPProtocol.MPAvatarDataItem> Serialize(FlatBufferBuilder builder, MoleMole.MPAvatarDataItem obj)
        {
            MoleMole.MPProtocol.MPAvatarDataItem.StartMPAvatarDataItem(builder);
            MoleMole.MPProtocol.MPAvatarDataItem.AddAvatarID(builder, obj.avatarID);
            MoleMole.MPProtocol.MPAvatarDataItem.AddLevel(builder, obj.level);
            MoleMole.MPProtocol.MPAvatarDataItem.AddStar(builder, obj.star);
            MoleMole.MPProtocol.MPAvatarDataItem.AddFinalHP(builder, obj.finalHP);
            MoleMole.MPProtocol.MPAvatarDataItem.AddFinalSP(builder, obj.finalSP);
            MoleMole.MPProtocol.MPAvatarDataItem.AddFinalAttack(builder, obj.finalAttack);
            MoleMole.MPProtocol.MPAvatarDataItem.AddFinalCritical(builder, obj.finalCritical);
            MoleMole.MPProtocol.MPAvatarDataItem.AddFinalDefense(builder, obj.finalDefense);
            return MoleMole.MPProtocol.MPAvatarDataItem.EndMPAvatarDataItem(builder);
        }

        public static Offset<MoleMole.MPProtocol.MPStageData> Serialize(FlatBufferBuilder builder, MoleMole.MPStageData obj)
        {
            StringOffset stageNameOffset = new StringOffset();
            if (obj.stageName != null)
            {
                stageNameOffset = builder.CreateString(obj.stageName);
            }
            MoleMole.MPProtocol.MPStageData.StartMPStageData(builder);
            MoleMole.MPProtocol.MPStageData.AddStageName(builder, stageNameOffset);
            return MoleMole.MPProtocol.MPStageData.EndMPStageData(builder);
        }

        public static Offset<MPVector3> Serialize(FlatBufferBuilder builder, Vector3 obj)
        {
            return MPVector3.CreateMPVector3(builder, obj.x, obj.y, obj.z);
        }

        public static System.Type SerializeToProtocol(FlatBufferBuilder builder, object obj)
        {
            int rootTable = 0;
            System.Type type = null;
            if (obj.GetType() == typeof(MoleMole.MPStageData))
            {
                type = typeof(MoleMole.MPProtocol.MPStageData);
                rootTable = Serialize(builder, (MoleMole.MPStageData) obj).Value;
            }
            else if (obj.GetType() == typeof(MoleMole.MPAvatarDataItem))
            {
                type = typeof(MoleMole.MPProtocol.MPAvatarDataItem);
                rootTable = Serialize(builder, (MoleMole.MPAvatarDataItem) obj).Value;
            }
            else if (obj.GetType() == typeof(EvtHittingOther))
            {
                type = typeof(Packet_Event_EvtHittingOther);
                rootTable = Serialize(builder, (EvtHittingOther) obj).Value;
            }
            else if (obj.GetType() == typeof(EvtBeingHit))
            {
                type = typeof(Packet_Event_EvtBeingHit);
                rootTable = Serialize(builder, (EvtBeingHit) obj).Value;
            }
            else if (obj.GetType() == typeof(EvtAttackLanded))
            {
                type = typeof(Packet_Event_EvtAttackLanded);
                rootTable = Serialize(builder, (EvtAttackLanded) obj).Value;
            }
            else if (obj.GetType() == typeof(EvtEvadeSuccess))
            {
                type = typeof(Packet_Event_EvtEvadeSuccess);
                rootTable = Serialize(builder, (EvtEvadeSuccess) obj).Value;
            }
            else if (obj.GetType() == typeof(EvtBulletHit))
            {
                type = typeof(Packet_Event_EvtBulletHit);
                rootTable = Serialize(builder, (EvtBulletHit) obj).Value;
            }
            else if (obj.GetType() == typeof(AttackData))
            {
                type = typeof(MPAttackData);
                rootTable = Serialize(builder, (AttackData) obj).Value;
            }
            else if (obj.GetType() == typeof(AttackResult))
            {
                type = typeof(MPAttackResult);
                rootTable = Serialize(builder, (AttackResult) obj).Value;
            }
            else if (obj.GetType() == typeof(AttackResult.HitCollsion))
            {
                type = typeof(MPHitCollision);
                rootTable = Serialize(builder, (AttackResult.HitCollsion) obj).Value;
            }
            builder.Finish(rootTable);
            return type;
        }
    }
}

