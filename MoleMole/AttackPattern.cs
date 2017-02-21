namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class AttackPattern
    {
        public const float ATTACK_EFFECT_BIG_DAMAGE_RATIO = 0.8f;
        private const float ATTACK_RANDOM_RANGE_H = 0.1f;
        private const float ATTACK_RANDOM_RANGE_V = 0.25f;

        public static void ActAttackEffects(ConfigEntityAttackEffect attackEffectConfig, BaseMonoEntity entity, Vector3 hitPoint, Vector3 hitForward)
        {
            if ((attackEffectConfig != null) && !attackEffectConfig.MuteAttackEffect)
            {
                if (attackEffectConfig.EffectPattern != null)
                {
                    if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtHitPoint)
                    {
                        TriggerAttackEffectsTo(attackEffectConfig.EffectPattern, hitPoint, hitForward, Vector3.one, entity);
                    }
                    else if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtHitPointRandom)
                    {
                        float max = 0.25f;
                        float num2 = 0.1f;
                        Vector3 vector = Vector3.Cross(Vector3.up, new Vector3(hitForward.x, 0f, hitForward.y));
                        Vector3.Normalize(vector);
                        vector = (Vector3) (vector * UnityEngine.Random.Range(-num2, num2));
                        Vector3 vector2 = new Vector3(0f, UnityEngine.Random.Range(-max, max), 0f);
                        TriggerAttackEffectsTo(attackEffectConfig.EffectPattern, (hitPoint + vector2) + vector, hitForward, Vector3.one, entity);
                    }
                    else if (attackEffectConfig.AttackEffectTriggerPos == AttackEffectTriggerAt.TriggerAtEntity)
                    {
                        TriggerAttackEffectsTo(attackEffectConfig.EffectPattern, entity.XZPosition, entity.transform.forward, entity.transform.localScale, entity);
                    }
                }
                if (!string.IsNullOrEmpty(attackEffectConfig.SwitchName))
                {
                    string switchName = attackEffectConfig.SwitchName;
                    BaseMonoMonster monster = entity as BaseMonoMonster;
                    if ((monster != null) && monster.hasArmor)
                    {
                        if (switchName == "Bullet")
                        {
                            switchName = "Bullet_Armor";
                        }
                        else if (switchName == "Punch")
                        {
                            switchName = "Shield_Default";
                        }
                        else if (switchName == "Sword")
                        {
                            switchName = "Shield_Sword";
                        }
                    }
                    Singleton<WwiseAudioManager>.Instance.SetSwitch("Damage_Type", switchName, entity.gameObject);
                    Singleton<WwiseAudioManager>.Instance.Post("DR_IMP", entity.gameObject, null, null);
                }
            }
        }

        public static void ActCameraShake(ConfigEntityCameraShake cameraShake)
        {
            if (cameraShake != null)
            {
                bool hasValue = cameraShake.ShakeAngle.HasValue;
                float angle = !cameraShake.ShakeAngle.HasValue ? 0f : cameraShake.ShakeAngle.Value;
                Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(cameraShake.ShakeTime, cameraShake.ShakeRange * 1.5f, angle, cameraShake.ShakeStepFrame, hasValue, cameraShake.ClearPreviousShake);
            }
        }

        public static void CylinderCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            CylinderCollisionDetect detect = (CylinderCollisionDetect) patternConfig;
            Vector3 cylinderCenterPoint = attacker.XZPosition + ((Vector3) (attacker.FaceDirection * detect.CenterZOffset));
            List<CollisionResult> results = CollisionDetectPattern.CylinderCollisionDetectBySphere(attacker.XZPosition, cylinderCenterPoint, detect.Radius, detect.Height, layerMask);
            TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void CylinderCollisionDetectTargetLockedAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            Vector3 xZPosition;
            CylinderCollisionDetectTargetLocked locked = (CylinderCollisionDetectTargetLocked) patternConfig;
            if ((attacker.AttackTarget != null) && attacker.AttackTarget.IsActive())
            {
                xZPosition = attacker.AttackTarget.XZPosition;
            }
            else
            {
                xZPosition = attacker.XZPosition;
            }
            List<CollisionResult> results = CollisionDetectPattern.CylinderCollisionDetectBySphere(attacker.XZPosition, xZPosition, locked.Radius * (1f + attacker.Evaluate(locked.RadiusRatio)), locked.Height, layerMask);
            TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void FanCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            FanCollisionDetect detect = (FanCollisionDetect) patternConfig;
            Vector3 fanCenterPoint = new Vector3(attacker.XZPosition.x, (attacker.XZPosition.y + detect.CenterYOffset) + (!detect.FollowRootNodeY ? 0f : attacker.GetAttachPoint("RootNode").transform.position.y), attacker.XZPosition.z);
            List<CollisionResult> results = CollisionDetectPattern.MeleeFanCollisionDetectBySphere(fanCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Radius * (1f + attacker.Evaluate(detect.RadiusRatio)), detect.FanAngle, detect.MeleeRadius, detect.MeleeFanAngle, layerMask);
            TestAndActHit(attackName, patternConfig, attacker, results);
        }

        private static List<CollisionResult> FilterRealAttackeesByBodyParts(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, List<CollisionResult> results)
        {
            ConfigEntityAttackPattern pattern = patternConfig;
            Dictionary<BaseMonoEntity, List<CollisionResult>> dictionary = new Dictionary<BaseMonoEntity, List<CollisionResult>>();
            List<CollisionResult> list = new List<CollisionResult>();
            foreach (CollisionResult result in results)
            {
                BaseMonoEntity entity = result.entity;
                if (entity != null)
                {
                    if (entity is MonoBodyPartEntity)
                    {
                        BaseMonoEntity owner = ((MonoBodyPartEntity) entity).owner;
                        if (!dictionary.ContainsKey(owner))
                        {
                            dictionary[owner] = new List<CollisionResult>();
                        }
                        dictionary[owner].Add(result);
                    }
                    else
                    {
                        list.Add(result);
                    }
                }
            }
            List<CollisionResult> list3 = new List<CollisionResult>();
            foreach (CollisionResult result2 in list)
            {
                if (!dictionary.ContainsKey(result2.entity))
                {
                    list3.Add(result2);
                }
            }
            foreach (BaseMonoEntity entity3 in dictionary.Keys)
            {
                CollisionResult result3 = null;
                if (((pattern is FanCollisionDetect) || (pattern is CylinderCollisionDetect)) || ((pattern is CylinderCollisionDetectTargetLocked) || (pattern is RectCollisionDetect)))
                {
                    Vector3 b = new Vector3();
                    if (pattern is RectCollisionDetect)
                    {
                        RectCollisionDetect detect = (RectCollisionDetect) pattern;
                        Vector3 vector2 = new Vector3(attacker.XZPosition.x, 0f, attacker.XZPosition.z);
                        Vector3 faceDirection = attacker.FaceDirection;
                        faceDirection.y = 0f;
                        b = vector2 + ((Vector3) (detect.OffsetZ * faceDirection.normalized));
                    }
                    else
                    {
                        b = attacker.XZPosition;
                    }
                    float maxValue = float.MaxValue;
                    foreach (CollisionResult result4 in dictionary[entity3])
                    {
                        float num2 = Vector3.Distance(result4.entity.XZPosition, b);
                        if (num2 < maxValue)
                        {
                            result3 = result4;
                            maxValue = num2;
                        }
                    }
                }
                else
                {
                    result3 = dictionary[entity3][0];
                }
                CollisionResult item = new CollisionResult(entity3, result3.hitPoint, result3.hitForward);
                list3.Add(item);
            }
            return list3;
        }

        public static BaseMonoEntity GetCollisionResultEntity(BaseMonoEntity entity)
        {
            if (entity is MonoBodyPartEntity)
            {
                return ((MonoBodyPartEntity) entity).owner;
            }
            if ((entity is BaseMonoAbilityEntity) && ((BaseMonoAbilityEntity) entity).isGhost)
            {
                return null;
            }
            return entity;
        }

        public static LayerMask GetLayerMask(IAttacker attacker)
        {
            return GetTargetLayerMask(attacker.GetRuntimeID());
        }

        public static LayerMask GetTargetLayerMask(uint runtimeID)
        {
            return Singleton<LevelManager>.Instance.gameMode.GetAttackPatternDefaultLayerMask(runtimeID);
        }

        public static void RectCollisionDetectAttack(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, LayerMask layerMask)
        {
            RectCollisionDetect detect = (RectCollisionDetect) patternConfig;
            Vector3 recCenterPoint = new Vector3(attacker.XZPosition.x, attacker.XZPosition.y + detect.CenterYOffset, attacker.XZPosition.z);
            List<CollisionResult> results = CollisionDetectPattern.RectCollisionDetectByRay(recCenterPoint, detect.OffsetZ, attacker.FaceDirection, detect.Width, detect.Distance, layerMask);
            TestAndActHit(attackName, patternConfig, attacker, results);
        }

        public static void SendHitEvent(uint attackerID, uint beHitID, string attackName, AttackResult.HitCollsion hitCollision, AttackData attackData, bool forceSkipAttackerResolve = false, MPEventDispatchMode mode = 0)
        {
            if (forceSkipAttackerResolve || Singleton<LevelManager>.Instance.gameMode.ShouldAttackPatternSendBeingHit(beHitID))
            {
                EvtBeingHit evt = new EvtBeingHit {
                    targetID = beHitID,
                    sourceID = attackerID,
                    animEventID = attackName
                };
                if (attackData != null)
                {
                    if (attackData.hitCollision == null)
                    {
                        attackData.hitCollision = hitCollision;
                    }
                    evt.attackData = attackData;
                }
                else
                {
                    evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(Singleton<EventManager>.Instance.GetActor(attackerID), attackName);
                    evt.attackData.resolveStep = AttackData.AttackDataStep.AttackerResolved;
                    evt.attackData.hitCollision = hitCollision;
                }
                Singleton<EventManager>.Instance.FireEvent(evt, mode);
            }
            else
            {
                EvtHittingOther other = new EvtHittingOther {
                    hitCollision = hitCollision,
                    targetID = attackerID,
                    toID = beHitID,
                    animEventID = attackName
                };
                if (attackData != null)
                {
                    if (attackData.hitCollision == null)
                    {
                        attackData.hitCollision = hitCollision;
                    }
                    other.attackData = attackData;
                }
                else
                {
                    other.hitCollision = hitCollision;
                }
                Singleton<EventManager>.Instance.FireEvent(other, mode);
            }
        }

        public static void TestAndActHit(string attackName, ConfigEntityAttackPattern patternConfig, IAttacker attacker, List<CollisionResult> results)
        {
            AttackResult.HitCollsion collsion4;
            List<CollisionResult> list = FilterRealAttackeesByBodyParts(attackName, patternConfig, attacker, results);
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(attacker.GetRuntimeID()))
            {
                case 3:
                    foreach (CollisionResult result in list)
                    {
                        BaseMonoEntity entity = result.entity;
                        if (((entity != null) && (entity.GetRuntimeID() != attacker.GetRuntimeID())) && (entity.IsActive() || (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity.GetRuntimeID()) == 4)))
                        {
                            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity.GetRuntimeID()))
                            {
                                case 3:
                                case 4:
                                case 6:
                                case 7:
                                    if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity.GetRuntimeID()))
                                    {
                                        goto Label_0103;
                                    }
                                    break;
                            }
                        }
                        continue;
                    Label_0103:
                        collsion4 = new AttackResult.HitCollsion();
                        collsion4.hitDir = result.hitForward;
                        collsion4.hitPoint = result.hitPoint;
                        AttackResult.HitCollsion hitCollision = collsion4;
                        SendHitEvent(attacker.GetRuntimeID(), entity.GetRuntimeID(), attackName, hitCollision, null, false, MPEventDispatchMode.CheckRemoteMode);
                    }
                    return;

                case 4:
                    foreach (CollisionResult result2 in list)
                    {
                        BaseMonoEntity entity2 = result2.entity;
                        if (((entity2 != null) && (entity2.GetRuntimeID() != attacker.GetRuntimeID())) && entity2.IsActive())
                        {
                            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity2.GetRuntimeID()))
                            {
                                case 3:
                                case 4:
                                case 6:
                                    if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity2.GetRuntimeID()))
                                    {
                                        goto Label_021F;
                                    }
                                    break;
                            }
                        }
                        continue;
                    Label_021F:
                        collsion4 = new AttackResult.HitCollsion();
                        collsion4.hitDir = result2.hitForward;
                        collsion4.hitPoint = result2.hitPoint;
                        AttackResult.HitCollsion collsion2 = collsion4;
                        SendHitEvent(attacker.GetRuntimeID(), entity2.GetRuntimeID(), attackName, collsion2, null, false, MPEventDispatchMode.CheckRemoteMode);
                    }
                    return;

                case 7:
                    foreach (CollisionResult result3 in list)
                    {
                        BaseMonoEntity entity3 = result3.entity;
                        if (((entity3 != null) && (entity3.GetRuntimeID() != attacker.GetRuntimeID())) && entity3.IsActive())
                        {
                            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(entity3.GetRuntimeID()))
                            {
                                case 3:
                                case 4:
                                case 6:
                                    if (Singleton<LevelManager>.Instance.gameMode.IsEnemy(attacker.GetRuntimeID(), entity3.GetRuntimeID()))
                                    {
                                        goto Label_033F;
                                    }
                                    break;
                            }
                        }
                        continue;
                    Label_033F:
                        collsion4 = new AttackResult.HitCollsion();
                        collsion4.hitDir = result3.hitForward;
                        collsion4.hitPoint = result3.hitPoint;
                        AttackResult.HitCollsion collsion3 = collsion4;
                        SendHitEvent(attacker.GetRuntimeID(), entity3.GetRuntimeID(), attackName, collsion3, null, false, MPEventDispatchMode.CheckRemoteMode);
                    }
                    return;
            }
            throw new Exception("Invalid Type or State!");
        }

        private static void TriggerAttackEffectsTo(string patternName, Vector3 initPos, Vector3 initForward, Vector3 initScale, BaseMonoEntity entity)
        {
            List<MonoEffect> list;
            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(patternName, initPos, initForward, initScale, entity, out list);
            for (int i = 0; i < list.Count; i++)
            {
                MonoEffect effect = list[i];
                effect.SetOwner(Singleton<LevelManager>.Instance.levelEntity);
                effect.SetupPluginFromTo(entity);
            }
        }
    }
}

