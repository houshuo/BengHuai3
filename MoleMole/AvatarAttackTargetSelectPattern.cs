namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class AvatarAttackTargetSelectPattern
    {
        private static void FilterAvatarTargetByEllipse(BaseMonoAvatar aAvatar, List<BaseMonoAvatar> avatars, Vector3 mainDirection, float eccentricity, ref BaseMonoEntity monsterTarget, ref float monsterScore)
        {
            for (int i = 0; i < avatars.Count; i++)
            {
                BaseMonoAvatar target = avatars[i];
                if ((target != aAvatar) && !target.denySelect)
                {
                    float num2 = GetScoreByEllipse(aAvatar, target, mainDirection, eccentricity);
                    if (num2 < monsterScore)
                    {
                        monsterTarget = target;
                        monsterScore = num2;
                    }
                }
            }
        }

        private static void FilterMonsterTargetByEllipse(BaseMonoAvatar aAvatar, List<BaseMonoMonster> monsters, Vector3 mainDirection, float eccentricity, ref BaseMonoEntity monsterTarget, ref float monsterScore)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                BaseMonoMonster target = monsters[i];
                if (!target.denySelect && target.IsActive())
                {
                    List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = target.GetAllHitboxEnabledBodyParts();
                    if (allHitboxEnabledBodyParts.Count > 0)
                    {
                        foreach (BaseMonoAbilityEntity entity in allHitboxEnabledBodyParts)
                        {
                            float num2 = GetScoreByEllipse(aAvatar, entity, mainDirection, eccentricity);
                            if (num2 < monsterScore)
                            {
                                monsterTarget = entity;
                                monsterScore = num2;
                            }
                        }
                    }
                    else
                    {
                        float num3 = GetScoreByEllipse(aAvatar, target, mainDirection, eccentricity);
                        if (num3 < monsterScore)
                        {
                            monsterTarget = target;
                            monsterScore = num3;
                        }
                    }
                }
            }
        }

        private static void FilterNearestMonsterTarget(BaseMonoAvatar aAvatar, List<BaseMonoMonster> monsters, ref BaseMonoEntity attackTarget)
        {
            float maxValue = float.MaxValue;
            foreach (BaseMonoMonster monster in monsters)
            {
                List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = monster.GetAllHitboxEnabledBodyParts();
                if (allHitboxEnabledBodyParts.Count > 0)
                {
                    foreach (BaseMonoAbilityEntity entity in allHitboxEnabledBodyParts)
                    {
                        float num2 = Vector3.Distance(entity.XZPosition, aAvatar.XZPosition);
                        if (num2 < maxValue)
                        {
                            attackTarget = entity;
                            maxValue = num2;
                        }
                    }
                }
                else
                {
                    float num3 = Vector3.Distance(monster.XZPosition, aAvatar.XZPosition);
                    if (num3 < maxValue)
                    {
                        attackTarget = monster;
                        maxValue = num3;
                    }
                }
            }
        }

        private static float GetScoreByEllipse(BaseMonoAvatar aAvatar, BaseMonoEntity target, Vector3 mainDirection, float eccentricity)
        {
            Vector3 vector = target.XZPosition - aAvatar.XZPosition;
            float num = eccentricity;
            Vector3 vector2 = Vector3.Project(vector, mainDirection);
            float num2 = (vector2.x + vector2.z) / (mainDirection.x + mainDirection.z);
            return (((vector.magnitude / num) - num2) / ((1f / num) - num));
        }

        public static void PvPSelectRemoteAvatar(BaseMonoAvatar aAvatar)
        {
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            if ((allAvatars.Count != 0) || (allPropObjects.Count != 0))
            {
                Vector3 forward = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
                if (aAvatar.GetActiveControlData().hasSteer)
                {
                    forward = aAvatar.GetActiveControlData().steerDirection;
                }
                forward.y = 0f;
                float eccentricity = 0.9f;
                BaseMonoEntity monsterTarget = null;
                float maxValue = float.MaxValue;
                FilterAvatarTargetByEllipse(aAvatar, allAvatars, forward, eccentricity, ref monsterTarget, ref maxValue);
                BaseMonoEntity newTarget = null;
                float num3 = float.MaxValue;
                for (int i = 0; i < allPropObjects.Count; i++)
                {
                    BaseMonoPropObject entity = allPropObjects[i];
                    if ((entity.IsActive() && !entity.denySelect) && Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity))
                    {
                        float num5 = 1.5f * GetScoreByEllipse(aAvatar, entity, forward, eccentricity);
                        if (num5 < num3)
                        {
                            newTarget = entity;
                            num3 = num5;
                        }
                    }
                }
                if ((monsterTarget == null) || (newTarget == null))
                {
                    if (monsterTarget != null)
                    {
                        aAvatar.SetAttackTarget(monsterTarget);
                    }
                    else if (newTarget != null)
                    {
                        aAvatar.SetAttackTarget(newTarget);
                    }
                    else
                    {
                        aAvatar.SetAttackTarget(null);
                    }
                }
                else
                {
                    BaseMonoEntity entity3 = (maxValue >= num3) ? newTarget : monsterTarget;
                    aAvatar.SetAttackTarget(entity3);
                }
            }
        }

        public static void SelectEnemyByEllipse(BaseMonoAvatar aAvatar)
        {
            List<BaseMonoMonster> monsters = new List<BaseMonoMonster>();
            foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
            {
                if (monster.IsActive() && !monster.denySelect)
                {
                    monsters.Add(monster);
                }
            }
            if (monsters.Count != 0)
            {
                BaseMonoEntity entity;
                Vector3 forward = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
                if (aAvatar.GetActiveControlData().hasSteer)
                {
                    forward = aAvatar.GetActiveControlData().steerDirection;
                }
                forward.y = 0f;
                if (monsters.Count > 0)
                {
                    entity = monsters[0];
                    float eccentricity = 0.9f;
                    float monsterScore = GetScoreByEllipse(aAvatar, entity, forward, eccentricity);
                    FilterMonsterTargetByEllipse(aAvatar, monsters, forward, eccentricity, ref entity, ref monsterScore);
                }
                else
                {
                    entity = null;
                }
                aAvatar.SetAttackTarget(entity);
            }
        }

        public static void SelectMonsterAndPropByEllipse(BaseMonoAvatar aAvatar)
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            if ((allMonsters.Count != 0) || (allPropObjects.Count != 0))
            {
                Vector3 forward = Singleton<CameraManager>.Instance.GetMainCamera().Forward;
                if (aAvatar.GetActiveControlData().hasSteer)
                {
                    forward = aAvatar.GetActiveControlData().steerDirection;
                }
                forward.y = 0f;
                float eccentricity = 0.9f;
                BaseMonoEntity monsterTarget = null;
                float maxValue = float.MaxValue;
                FilterMonsterTargetByEllipse(aAvatar, allMonsters, forward, eccentricity, ref monsterTarget, ref maxValue);
                BaseMonoEntity newTarget = null;
                float num3 = float.MaxValue;
                for (int i = 0; i < allPropObjects.Count; i++)
                {
                    BaseMonoPropObject entity = allPropObjects[i];
                    if ((entity.IsActive() && !entity.denySelect) && Singleton<CameraManager>.Instance.GetMainCamera().IsEntityVisible(entity))
                    {
                        float num5 = 1.5f * GetScoreByEllipse(aAvatar, entity, forward, eccentricity);
                        if (num5 < num3)
                        {
                            newTarget = entity;
                            num3 = num5;
                        }
                    }
                }
                if ((monsterTarget == null) || (newTarget == null))
                {
                    if (monsterTarget != null)
                    {
                        aAvatar.SetAttackTarget(monsterTarget);
                    }
                    else if (newTarget != null)
                    {
                        aAvatar.SetAttackTarget(newTarget);
                    }
                    else
                    {
                        aAvatar.SetAttackTarget(null);
                    }
                }
                else
                {
                    BaseMonoEntity entity3 = (maxValue >= num3) ? newTarget : monsterTarget;
                    aAvatar.SetAttackTarget(entity3);
                }
            }
        }

        public static void SelectNearestEnemyV1(BaseMonoAvatar aAvatar)
        {
            List<BaseMonoMonster> monsters = new List<BaseMonoMonster>();
            foreach (BaseMonoMonster monster in Singleton<MonsterManager>.Instance.GetAllMonsters())
            {
                if (monster.IsActive() && !monster.denySelect)
                {
                    monsters.Add(monster);
                }
            }
            if (monsters.Count != 0)
            {
                BaseMonoEntity attackTarget = null;
                if (monsters.Count > 0)
                {
                    FilterNearestMonsterTarget(aAvatar, monsters, ref attackTarget);
                }
                else
                {
                    attackTarget = null;
                }
                aAvatar.SetAttackTarget(attackTarget);
            }
        }
    }
}

