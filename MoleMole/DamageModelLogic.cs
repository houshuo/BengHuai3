namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class DamageModelLogic
    {
        private static float[,] DEFAULT_AVATAR_NATURE_DAMAGE_TABLE = new float[,] { { 1f, 1f, 1f, 1f }, { 1f, 1f, 1f, 1f }, { 1f, 1f, 1f, 1f }, { 1f, 1f, 1f, 1f } };
        private static float[,] DEFAULT_MONSTER_NATURE_DAMAGE_TABLE = new float[,] { { 1f, 1f, 1f, 1f }, { 1f, 1f, 1.3f, 0.7f }, { 1f, 0.7f, 1f, 1.3f }, { 1f, 1.3f, 0.7f, 1f } };
        private static int[,] DEFAULT_NATURE_CIRCLE = new int[,] { { 0, 0, 0, 0 }, { 0, 0, 1, -1 }, { 0, -1, 0, 1 }, { 0, 1, -1, 0 } };
        public const int MAX_ATTACK_PUNISH_LEVEL_DIFFERENCE = 10;
        public const int MAX_DEFENCE_PUNISH_LEVLE_DIFFERENCE = 10;
        public const int MIN_ATTACK_PUNISH_LEVEL_DIFFERENCE = 0;
        public const int MIN_DEFENCE_PUNISH_LEVEL_DIFFERENCE = 0;

        public static AttackData CreateAttackDataFromAttackerAnimEvent(BaseActor from, string animEventID)
        {
            if (from is AvatarActor)
            {
                AvatarActor actor = (AvatarActor) from;
                ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor.config, animEventID);
                return CreateAttackDataFromAttackProperty(from, event2.AttackProperty, event2.AttackEffect, event2.CameraShake);
            }
            if (from is MonsterActor)
            {
                MonsterActor actor2 = (MonsterActor) from;
                ConfigMonsterAnimEvent event3 = SharedAnimEventData.ResolveAnimEvent(actor2.config, animEventID);
                AttackData attackData = CreateAttackDataFromAttackProperty(from, event3.AttackProperty, event3.AttackEffect, event3.CameraShake);
                actor2.RefillAttackDataDamagePercentage(animEventID, ref attackData);
                return attackData;
            }
            if (from is PropObjectActor)
            {
                PropObjectActor actor3 = (PropObjectActor) from;
                ConfigPropAnimEvent event4 = SharedAnimEventData.ResolveAnimEvent(actor3.config, animEventID);
                return CreateAttackDataFromAttackProperty(from, event4.AttackProperty, event4.AttackEffect, event4.CameraShake);
            }
            return null;
        }

        public static AttackData CreateAttackDataFromAttackProperty(BaseActor from, ConfigEntityAttackProperty attackProperty, ConfigEntityAttackEffect attackEffect, ConfigEntityCameraShake cameraShake)
        {
            AttackData data = new AttackData();
            if (from is AvatarActor)
            {
                AvatarActor actor = (AvatarActor) from;
                data.attackCategoryTag = attackProperty.CategoryTagCombined;
                data.attackerClass = actor.config.CommonArguments.Class;
                data.attackerNature = (EntityNature) actor.avatarDataItem.Attribute;
                data.attackerCategory = 3;
                data.attackerAniDamageRatio = attackProperty.AniDamageRatio;
                data.frameHalt = attackProperty.FrameHalt;
                data.hitType = attackProperty.HitType;
                data.hitEffect = attackProperty.HitEffect;
                data.hitEffectAux = attackProperty.HitEffectAux;
                data.attackerLevel = (int) actor.level;
                data.retreatVelocity = attackProperty.RetreatVelocity;
                data.attackerCritChance = ((actor.critical + actor.GetProperty("Actor_CriticalDelta")) * (1f + actor.GetProperty("Actor_CriticalRatio"))) / ((float) (0x4b + (actor.level * 5)));
                data.attackerCritDamageRatio = 2f;
                data.attackerAttackValue = (float) actor.attack;
                data.attackerAttackPercentage = attackProperty.DamagePercentage;
                data.attackerAddedAttackValue = attackProperty.AddedDamageValue;
                data.attackerNormalDamage = attackProperty.NormalDamage;
                data.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
                data.addedAttackerNormalDamageRatio = actor.GetProperty("Actor_NormalAttackRatio");
                data.attackerFireDamage = attackProperty.FireDamage;
                data.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
                data.addedAttackerFireDamageRatio = actor.GetProperty("Actor_FireAttackRatio");
                data.attackerThunderDamage = attackProperty.ThunderDamage;
                data.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
                data.addedAttackerThunderDamageRatio = actor.GetProperty("Actor_ThunderAttackRatio");
                data.attackerIceDamage = attackProperty.IceDamage;
                data.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
                data.addedAttackerIceDamageRatio = actor.GetProperty("Actor_IceAttackRatio");
                data.attackerAlienDamage = attackProperty.AlienDamage;
                data.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
                data.addedAttackerAlienDamageRatio = actor.GetProperty("Actor_AllienAttackRatio");
                data.killEffect = attackProperty.KillEffect;
                data.hitEffectPattern = AttackResult.HitEffectPattern.Normal;
                data.isInComboCount = attackProperty.IsInComboCount;
                data.isAnimEventAttack = attackProperty.IsAnimEventAttack;
                data.noBreakFrameHaltAdd = attackProperty.NoBreakFrameHaltAdd;
                data.attackEffectPattern = attackEffect;
                if (data.attackEffectPattern == null)
                {
                    data.attackEffectPattern = (data.attackerAniDamageRatio < 0.8f) ? InLevelData.InLevelMiscData.DefaultAvatarAttackSmallEffect : InLevelData.InLevelMiscData.DefaultAvatarAttackBigEffect;
                }
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
                {
                    data.attackCameraShake = cameraShake;
                }
                data.attackerAniDamageRatio += actor.GetProperty("Actor_AniDamageDelta");
                data.attackerCritChance += actor.GetProperty("Actor_CriticalChanceDelta");
                data.attackerCritDamageRatio += actor.GetProperty("Actor_CriticalDamageRatio");
                data.attackerAttackValue = (data.attackerAttackValue + actor.GetProperty("Actor_AttackDelta")) * (1f + actor.GetProperty("Actor_AttackRatio"));
                data.addedDamageRatio = actor.GetProperty("Actor_AddedDamageRatio");
                data.addedAttackRatio = actor.GetProperty("Actor_AddedAttackRatio");
                data.attackerShieldDamageRatio = 1f + actor.GetProperty("Actor_ShieldDamageRatio");
                data.attackerShieldDamageDelta = actor.GetProperty("Actor_ShieldDamageDelta");
                data.retreatVelocity *= 1f + actor.GetProperty("Actor_RetreatRatio");
            }
            else if (from is MonsterActor)
            {
                MonsterActor actor2 = (MonsterActor) from;
                data.attackCategoryTag = attackProperty.CategoryTagCombined;
                data.attackerClass = actor2.config.CommonArguments.Class;
                data.attackerNature = (EntityNature) actor2.metaConfig.nature;
                data.attackerCategory = 4;
                data.attackerAniDamageRatio = attackProperty.AniDamageRatio;
                data.frameHalt = attackProperty.FrameHalt;
                data.hitType = attackProperty.HitType;
                data.hitEffect = attackProperty.HitEffect;
                data.hitEffectAux = attackProperty.HitEffectAux;
                data.hitEffectPattern = AttackResult.HitEffectPattern.Normal;
                data.retreatVelocity = attackProperty.RetreatVelocity;
                data.attackerLevel = (int) actor2.level;
                data.attackerAttackValue = (float) actor2.attack;
                data.attackerAttackPercentage = attackProperty.DamagePercentage;
                data.attackerAddedAttackValue = attackProperty.AddedDamageValue;
                data.attackerNormalDamage = attackProperty.NormalDamage;
                data.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
                data.addedAttackerNormalDamageRatio = actor2.GetProperty("Actor_NormalAttackRatio");
                data.attackerFireDamage = attackProperty.FireDamage;
                data.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
                data.addedAttackerFireDamageRatio = actor2.GetProperty("Actor_FireAttackRatio");
                data.attackerThunderDamage = attackProperty.ThunderDamage;
                data.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
                data.addedAttackerThunderDamageRatio = actor2.GetProperty("Actor_ThunderAttackRatio");
                data.attackerIceDamage = attackProperty.IceDamage;
                data.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
                data.addedAttackerIceDamageRatio = actor2.GetProperty("Actor_IceAttackRatio");
                data.attackerAlienDamage = attackProperty.AlienDamage;
                data.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
                data.addedAttackerAlienDamageRatio = actor2.GetProperty("Actor_AllienAttackRatio");
                data.noTriggerEvadeAndDefend = attackProperty.NoTriggerEvadeAndDefend;
                data.attackEffectPattern = attackEffect;
                if (data.attackEffectPattern == null)
                {
                    data.attackEffectPattern = InLevelData.InLevelMiscData.DefaultMonsterAttackEffect;
                }
                data.attackCameraShake = cameraShake;
                data.isAnimEventAttack = attackProperty.IsAnimEventAttack;
                data.attackerAniDamageRatio += actor2.GetProperty("Actor_AniDamageDelta");
                data.attackerAttackValue = (data.attackerAttackValue + actor2.GetProperty("Actor_AttackDelta")) * (1f + actor2.GetProperty("Actor_AttackRatio"));
                data.addedAttackRatio = actor2.GetProperty("Actor_AddedAttackRatio");
                data.attackerShieldDamageRatio = 1f + actor2.GetProperty("Actor_ShieldDamageRatio");
                data.attackerShieldDamageDelta = actor2.GetProperty("Actor_ShieldDamageDelta");
                data.retreatVelocity *= 1f + actor2.GetProperty("Actor_RetreatRatio");
            }
            else if (from is BaseAbilityActor)
            {
                BaseAbilityActor actor3 = (BaseAbilityActor) from;
                data.attackCategoryTag = attackProperty.CategoryTagCombined;
                data.attackerClass = EntityClass.Default;
                data.attackerNature = EntityNature.Pure;
                data.attackerCategory = 7;
                data.attackerAniDamageRatio = attackProperty.AniDamageRatio;
                data.frameHalt = attackProperty.FrameHalt;
                data.hitType = attackProperty.HitType;
                data.hitEffect = attackProperty.HitEffect;
                data.hitEffectAux = attackProperty.HitEffectAux;
                data.retreatVelocity = attackProperty.RetreatVelocity;
                data.attackerLevel = 0;
                data.attackerAttackPercentage = attackProperty.DamagePercentage;
                data.attackerAttackValue = (float) actor3.attack;
                data.attackerAttackPercentage = attackProperty.DamagePercentage;
                data.attackerAddedAttackValue = attackProperty.AddedDamageValue;
                data.attackerNormalDamage = attackProperty.NormalDamage;
                data.attackerNormalDamagePercentage = attackProperty.NormalDamagePercentage;
                data.addedAttackerNormalDamageRatio = actor3.GetProperty("Actor_NormalAttackRatio");
                data.attackerFireDamage = attackProperty.FireDamage;
                data.attackerFireDamagePercentage = attackProperty.FireDamagePercentage;
                data.addedAttackerFireDamageRatio = actor3.GetProperty("Actor_FireAttackRatio");
                data.attackerThunderDamage = attackProperty.ThunderDamage;
                data.attackerThunderDamagePercentage = attackProperty.ThunderDamagePercentage;
                data.addedAttackerThunderDamageRatio = actor3.GetProperty("Actor_ThunderAttackRatio");
                data.attackerIceDamage = attackProperty.IceDamage;
                data.attackerIceDamagePercentage = attackProperty.IceDamagePercentage;
                data.addedAttackerIceDamageRatio = actor3.GetProperty("Actor_IceAttackRatio");
                data.attackerAlienDamage = attackProperty.AlienDamage;
                data.attackerAlienDamagePercentage = attackProperty.AlienDamagePercentage;
                data.addedAttackerAlienDamageRatio = actor3.GetProperty("Actor_AllienAttackRatio");
                data.attackEffectPattern = attackEffect;
                if (data.attackEffectPattern == null)
                {
                    data.attackEffectPattern = InLevelData.InLevelMiscData.DefaultMonsterAttackEffect;
                }
                data.attackCameraShake = cameraShake;
                data.isAnimEventAttack = attackProperty.IsAnimEventAttack;
                data.attackerAniDamageRatio += actor3.GetProperty("Actor_AniDamageDelta");
                data.attackerAttackValue = (data.attackerAttackValue + actor3.GetProperty("Actor_AttackDelta")) * (1f + actor3.GetProperty("Actor_AttackRatio"));
                data.addedAttackRatio = actor3.GetProperty("Actor_AddedAttackRatio");
                data.attackerShieldDamageRatio = 1f + actor3.GetProperty("Actor_ShieldDamageRatio");
                data.attackerShieldDamageDelta = actor3.GetProperty("Actor_ShieldDamageDelta");
                data.retreatVelocity *= 1f + actor3.GetProperty("Actor_RetreatRatio");
            }
            data.resolveStep = AttackData.AttackDataStep.AttackerResolved;
            return data;
        }

        private static float GetAnimDefenceRatio(BaseActor actor)
        {
            float animDefenceRatio = 0f;
            if (actor is MonsterActor)
            {
                MonsterActor actor2 = (MonsterActor) actor;
                float defaultAnimDefenceRatio = actor2.config.StateMachinePattern.DefaultAnimDefenceRatio;
                string currentSkillID = actor2.monster.CurrentSkillID;
                animDefenceRatio = defaultAnimDefenceRatio;
                if (actor2.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    return (actor2.config.StateMachinePattern.ThrowAnimDefenceRatio + actor2.GetProperty("Actor_ThrowAniDefenceDelta"));
                }
                if (string.IsNullOrEmpty(currentSkillID) || !actor2.config.Skills.ContainsKey(currentSkillID))
                {
                    return animDefenceRatio;
                }
                float num3 = actor2.monster.GetCurrentNormalizedTime() % 1f;
                ConfigMonsterSkill skill = actor2.config.Skills[currentSkillID];
                if ((num3 <= skill.AnimDefenceNormalizedTimeStart) || (num3 >= skill.AnimDefenceNormalizedTimeStop))
                {
                    return animDefenceRatio;
                }
                return skill.AnimDefenceRatio;
            }
            if (actor is AvatarActor)
            {
                AvatarActor actor3 = (AvatarActor) actor;
                animDefenceRatio = actor3.config.StateMachinePattern.DefaultAnimDefenceRatio;
                string str2 = actor3.avatar.CurrentSkillID;
                if (!string.IsNullOrEmpty(str2) && actor3.config.Skills.ContainsKey(str2))
                {
                    float num4 = actor3.avatar.GetCurrentNormalizedTime() % 1f;
                    ConfigAvatarSkill skill2 = actor3.config.Skills[str2];
                    if ((num4 > skill2.AnimDefenceNormalizedTimeStart) && (num4 < skill2.AnimDefenceNormalizedTimeStop))
                    {
                        animDefenceRatio = skill2.AnimDefenceRatio;
                    }
                }
            }
            return animDefenceRatio;
        }

        public static float GetDefenceRatio(float defence, int attackerLevel)
        {
            if (defence < 0f)
            {
                return 0f;
            }
            return (defence / ((300 + (attackerLevel * 20)) + defence));
        }

        private static float GetFixedDamageBonusFactor(float damageBonus)
        {
            float num = damageBonus;
            if (damageBonus > 1f)
            {
                return (num * Mathf.Clamp(Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor, 1f, Singleton<LevelManager>.Instance.levelActor.upLevelNatureBonusFactor));
            }
            if (damageBonus < 1f)
            {
                num *= Mathf.Clamp(Singleton<LevelManager>.Instance.levelActor.downLevelNatureBonusFactor, 0f, 1f);
            }
            return num;
        }

        public static int GetNatureBonusType(EntityNature attackerNature, EntityNature attackeeNature)
        {
            return DEFAULT_NATURE_CIRCLE[(int) attackerNature, (int) attackeeNature];
        }

        public static float GetNatureDamageBonusRatio(EntityNature attackerNature, EntityNature attackeeNature, BaseAbilityActor attackee)
        {
            if (attackee is MonsterActor)
            {
                return GetFixedDamageBonusFactor(DEFAULT_MONSTER_NATURE_DAMAGE_TABLE[(int) attackerNature, (int) attackeeNature]);
            }
            if (attackee is AvatarActor)
            {
                return DEFAULT_AVATAR_NATURE_DAMAGE_TABLE[(int) attackerNature, (int) attackeeNature];
            }
            return 0f;
        }

        public static void ResolveAttackDataByAttackee(BaseActor to, AttackData attackData)
        {
            if (!attackData.rejected)
            {
                if (to is AvatarActor)
                {
                    AvatarActor actor = (AvatarActor) to;
                    bool flag = Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish();
                    int levelDifference = Mathf.Clamp(attackData.attackerLevel - Singleton<PlayerModule>.Instance.playerData.teamLevel, 0, 10);
                    if ((attackData.attackerCategory == 4) && flag)
                    {
                        attackData.attackerAniDamageRatio *= 1f + AvatarDefencePunishMetaDataReader.GetAvatarDefencePunishMetaDataByKey(levelDifference).AttackRatioIncrease;
                    }
                    attackData.attackeeAniDefenceRatio = GetAnimDefenceRatio(actor);
                    attackData.damage = (((attackData.attackerAttackValue * attackData.attackerAttackPercentage) + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio)) * (1f + attackData.addedDamageRatio);
                    float defence = (actor.defense + actor.GetProperty("Actor_DefenceDelta")) * (1f + actor.GetProperty("Actor_DefenceRatio"));
                    float defenceRatio = GetDefenceRatio(defence, attackData.attackerLevel);
                    float num4 = (1f - defenceRatio) * actor.GetProperty("Actor_DamageReduceRatio");
                    attackData.attackeeAddedDamageTakeRatio += actor.GetProperty("Actor_DamageTakeRatio");
                    attackData.damage = (attackData.damage * num4) * (1f + attackData.attackeeAddedDamageTakeRatio);
                    attackData.plainDamage = (((((attackData.attackerNormalDamage + (attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage)) * (1f + attackData.addedAttackerNormalDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor.GetProperty("Actor_ResistAllElementAttackRatio")) * actor.GetProperty("Actor_ResistNormalAttackRatio")) * (1f + actor.GetProperty("Actor_NormalAttackTakeRatio"));
                    attackData.fireDamage = (((((attackData.attackerFireDamage + (attackData.attackerAttackValue * attackData.attackerFireDamagePercentage)) * (1f + attackData.addedAttackerFireDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor.GetProperty("Actor_ResistAllElementAttackRatio")) * actor.GetProperty("Actor_ResistFireAttackRatio")) * (1f + actor.GetProperty("Actor_FireAttackTakeRatio"));
                    attackData.thunderDamage = (((((attackData.attackerThunderDamage + (attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage)) * (1f + attackData.addedAttackerThunderDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor.GetProperty("Actor_ResistAllElementAttackRatio")) * actor.GetProperty("Actor_ResistThunderAttackRatio")) * (1f + actor.GetProperty("Actor_ThunderAttackTakeRatio"));
                    attackData.iceDamage = (((((attackData.attackerIceDamage + (attackData.attackerAttackValue * attackData.attackerIceDamagePercentage)) * (1f + attackData.addedAttackerIceDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor.GetProperty("Actor_ResistAllElementAttackRatio")) * actor.GetProperty("Actor_ResistIceAttackRatio")) * (1f + actor.GetProperty("Actor_IceAttackTakeRatio"));
                    attackData.alienDamage = (((((attackData.attackerAlienDamage + (attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage)) * (1f + attackData.addedAttackerAlienDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor.GetProperty("Actor_ResistAllElementAttackRatio")) * actor.GetProperty("Actor_ResistAllienAttackRatio")) * (1f + actor.GetProperty("Actor_AllienAttackTakeRatio"));
                    attackData.attackeeNature = (EntityNature) actor.avatarDataItem.Attribute;
                    attackData.attackeeClass = actor.config.CommonArguments.Class;
                    float num5 = GetNatureDamageBonusRatio(attackData.attackerNature, attackData.attackeeNature, actor);
                    float damageIncreaseRate = 0f;
                    if ((attackData.attackerCategory == 4) && flag)
                    {
                        damageIncreaseRate = AvatarDefencePunishMetaDataReader.GetAvatarDefencePunishMetaDataByKey(levelDifference).DamageIncreaseRate;
                    }
                    float num7 = Mathf.Clamp((float) (1f - attackData.attackerAddedAllDamageReduceRatio), (float) 0f, (float) 1f);
                    attackData.damage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.plainDamage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.fireDamage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.thunderDamage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.iceDamage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.alienDamage *= (num5 * (1f + damageIncreaseRate)) * num7;
                    attackData.natureDamageRatio = num5;
                    if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(actor.runtimeID))
                    {
                        attackData.attackCameraShake = null;
                    }
                    attackData.attackeeAniDefenceRatio += actor.GetProperty("Actor_AniDefenceDelta");
                }
                else if (to is MonsterActor)
                {
                    MonsterActor actor2 = (MonsterActor) to;
                    int num8 = Mathf.Clamp(((int) actor2.level) - Singleton<PlayerModule>.Instance.playerData.teamLevel, 0, 10);
                    if ((attackData.attackerCategory == 3) && Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
                    {
                        attackData.attackerAniDamageRatio *= 1f - AvatarAttackPunishMetaDataReader.GetAvatarAttackPunishMetaDataByKey(num8).AttackRatioReduce;
                    }
                    attackData.attackeeAniDefenceRatio = GetAnimDefenceRatio(actor2);
                    attackData.damage = (((attackData.attackerAttackValue * attackData.attackerAttackPercentage) + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio)) * (1f + attackData.addedDamageRatio);
                    float num9 = (actor2.defense + actor2.GetProperty("Actor_DefenceDelta")) * (1f + actor2.GetProperty("Actor_DefenceRatio"));
                    float num10 = GetDefenceRatio(num9, attackData.attackerLevel);
                    float num11 = (1f - num10) * actor2.GetProperty("Actor_DamageReduceRatio");
                    attackData.attackeeAddedDamageTakeRatio += actor2.GetProperty("Actor_DamageTakeRatio");
                    attackData.damage = (attackData.damage * num11) * (1f + attackData.attackeeAddedDamageTakeRatio);
                    attackData.plainDamage = (((((attackData.attackerNormalDamage + (attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage)) * (1f + attackData.addedAttackerNormalDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor2.GetProperty("Actor_ResistAllElementAttackRatio")) * actor2.GetProperty("Actor_ResistNormalAttackRatio")) * (1f + actor2.GetProperty("Actor_NormalAttackTakeRatio"));
                    attackData.fireDamage = (((((attackData.attackerFireDamage + (attackData.attackerAttackValue * attackData.attackerFireDamagePercentage)) * (1f + attackData.addedAttackerFireDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor2.GetProperty("Actor_ResistAllElementAttackRatio")) * actor2.GetProperty("Actor_ResistFireAttackRatio")) * (1f + actor2.GetProperty("Actor_FireAttackTakeRatio"));
                    attackData.thunderDamage = (((((attackData.attackerThunderDamage + (attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage)) * (1f + attackData.addedAttackerThunderDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor2.GetProperty("Actor_ResistAllElementAttackRatio")) * actor2.GetProperty("Actor_ResistThunderAttackRatio")) * (1f + actor2.GetProperty("Actor_ThunderAttackTakeRatio"));
                    attackData.iceDamage = (((((attackData.attackerIceDamage + (attackData.attackerAttackValue * attackData.attackerIceDamagePercentage)) * (1f + attackData.addedAttackerIceDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor2.GetProperty("Actor_ResistAllElementAttackRatio")) * actor2.GetProperty("Actor_ResistIceAttackRatio")) * (1f + actor2.GetProperty("Actor_IceAttackTakeRatio"));
                    attackData.alienDamage = (((((attackData.attackerAlienDamage + (attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage)) * (1f + attackData.addedAttackerAlienDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor2.GetProperty("Actor_ResistAllElementAttackRatio")) * actor2.GetProperty("Actor_ResistAllienAttackRatio")) * (1f + actor2.GetProperty("Actor_AllienAttackTakeRatio"));
                    attackData.attackeeNature = (EntityNature) actor2.metaConfig.nature;
                    attackData.attackeeClass = actor2.config.CommonArguments.Class;
                    float num12 = GetNatureDamageBonusRatio(attackData.attackerNature, attackData.attackeeNature, actor2);
                    float damageReduceRate = 0f;
                    if ((attackData.attackerCategory == 3) && Singleton<LevelScoreManager>.Instance.IsAllowLevelPunish())
                    {
                        damageReduceRate = AvatarAttackPunishMetaDataReader.GetAvatarAttackPunishMetaDataByKey(num8).DamageReduceRate;
                    }
                    float num14 = Mathf.Clamp((float) (1f - attackData.attackerAddedAllDamageReduceRatio), (float) 0f, (float) 1f);
                    attackData.damage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.plainDamage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.fireDamage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.thunderDamage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.iceDamage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.alienDamage *= (num12 * (1f - damageReduceRate)) * num14;
                    attackData.natureDamageRatio = num12;
                    if (actor2.monster.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                    {
                        attackData.retreatVelocity *= actor2.config.CommonArguments.BePushedSpeedRatioThrow;
                    }
                    else
                    {
                        attackData.retreatVelocity *= actor2.config.CommonArguments.BePushedSpeedRatio;
                    }
                    attackData.retreatVelocity *= 1f + actor2.GetProperty("Actor_BeRetreatRatio");
                    if (attackData.isAnimEventAttack && (UnityEngine.Random.value < attackData.attackerCritChance))
                    {
                        attackData.damage *= attackData.attackerCritDamageRatio;
                        attackData.hitLevel = AttackResult.ActorHitLevel.Critical;
                    }
                    attackData.attackeeAniDefenceRatio += actor2.GetProperty("Actor_AniDefenceDelta");
                    if (attackData.frameHalt > 1)
                    {
                        attackData.frameHalt += 2;
                    }
                    if ((attackData.attackeeAniDefenceRatio > attackData.attackerAniDamageRatio) && (attackData.frameHalt > 1))
                    {
                        attackData.frameHalt += attackData.noBreakFrameHaltAdd;
                    }
                }
                else if (to is PropObjectActor)
                {
                    PropObjectActor actor3 = (PropObjectActor) to;
                    attackData.attackeeAniDefenceRatio = 0f;
                    attackData.beHitEffectPattern = actor3.config.BeHitEffect;
                    attackData.damage = (((attackData.attackerAttackValue * attackData.attackerAttackPercentage) + attackData.attackerAddedAttackValue) * (1f + attackData.addedAttackRatio)) * (1f + attackData.addedDamageRatio);
                    attackData.plainDamage = (((((attackData.attackerNormalDamage + (attackData.attackerAttackValue * attackData.attackerNormalDamagePercentage)) * (1f + attackData.addedAttackerNormalDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor3.GetProperty("Actor_ResistAllElementAttackRatio")) * actor3.GetProperty("Actor_ResistNormalAttackRatio")) * (1f + actor3.GetProperty("Actor_NormalAttackTakeRatio"));
                    attackData.fireDamage = (((((attackData.attackerFireDamage + (attackData.attackerAttackValue * attackData.attackerFireDamagePercentage)) * (1f + attackData.addedAttackerFireDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor3.GetProperty("Actor_ResistAllElementAttackRatio")) * actor3.GetProperty("Actor_ResistFireAttackRatio")) * (1f + actor3.GetProperty("Actor_FireAttackTakeRatio"));
                    attackData.thunderDamage = (((((attackData.attackerThunderDamage + (attackData.attackerAttackValue * attackData.attackerThunderDamagePercentage)) * (1f + attackData.addedAttackerThunderDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor3.GetProperty("Actor_ResistAllElementAttackRatio")) * actor3.GetProperty("Actor_ResistThunderAttackRatio")) * (1f + actor3.GetProperty("Actor_ThunderAttackTakeRatio"));
                    attackData.iceDamage = (((((attackData.attackerIceDamage + (attackData.attackerAttackValue * attackData.attackerIceDamagePercentage)) * (1f + attackData.addedAttackerIceDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor3.GetProperty("Actor_ResistAllElementAttackRatio")) * actor3.GetProperty("Actor_ResistIceAttackRatio")) * (1f + actor3.GetProperty("Actor_IceAttackTakeRatio"));
                    attackData.alienDamage = (((((attackData.attackerAlienDamage + (attackData.attackerAttackValue * attackData.attackerAlienDamagePercentage)) * (1f + attackData.addedAttackerAlienDamageRatio)) * (1f + attackData.addedDamageRatio)) * actor3.GetProperty("Actor_ResistAllElementAttackRatio")) * actor3.GetProperty("Actor_ResistAllienAttackRatio")) * (1f + actor3.GetProperty("Actor_AllienAttackTakeRatio"));
                    attackData.attackeeAniDefenceRatio += actor3.GetProperty("Actor_AniDefenceDelta");
                    if (attackData.frameHalt > 1)
                    {
                        attackData.frameHalt += 2;
                    }
                }
                attackData.resolveStep = AttackData.AttackDataStep.AttackeeResolved;
            }
        }

        public static AttackResult ResolveAttackDataFinal(BaseActor attackee, AttackData attackData)
        {
            if (!attackData.rejected)
            {
                if ((attackData.attackeeAniDefenceRatio > attackData.attackerAniDamageRatio) && (attackData.hitEffect > AttackResult.AnimatorHitEffect.Light))
                {
                    attackData.hitEffect = AttackResult.AnimatorHitEffect.Light;
                }
                if ((attackee is MonsterActor) && (attackData.beHitEffectPattern == null))
                {
                    MonsterActor actor = (MonsterActor) attackee;
                    if ((attackData.isAnimEventAttack && (attackData.hitEffect == AttackResult.AnimatorHitEffect.Light)) && (attackData.attackerAniDamageRatio > 0.4f))
                    {
                        if (((attackData.attackEffectPattern == null) || !attackData.attackEffectPattern.MuteAttackEffect) && (actor.IsActive() && (actor.monster.CurrentSkillID != null)))
                        {
                            ConfigMonsterSkill skill = actor.config.Skills[actor.monster.CurrentSkillID];
                            if (actor.monster.GetCurrentNormalizedTime() < skill.AttackNormalizedTimeStop)
                            {
                                attackData.beHitEffectPattern = InLevelData.InLevelMiscData.NoBreakBehitEffect;
                            }
                        }
                    }
                    else if (actor.abilityState.ContainsState(AbilityState.Frozen))
                    {
                        attackData.beHitEffectPattern = InLevelData.InLevelMiscData.FrozenBehitEffect;
                    }
                    else if (attackData.attackerAniDamageRatio <= 0.6f)
                    {
                        attackData.beHitEffectPattern = actor.config.StateMachinePattern.BeHitEffect;
                    }
                    else if (attackData.attackerAniDamageRatio <= 0.8f)
                    {
                        attackData.beHitEffectPattern = actor.config.StateMachinePattern.BeHitEffectMid;
                    }
                    else
                    {
                        attackData.beHitEffectPattern = actor.config.StateMachinePattern.BeHitEffectBig;
                    }
                }
                attackData.aniDamageRatio = attackData.attackerAniDamageRatio;
                List<KeyValuePair<AttackResult.ElementType, float>> list = new List<KeyValuePair<AttackResult.ElementType, float>> {
                    new KeyValuePair<AttackResult.ElementType, float>(0, attackData.plainDamage),
                    new KeyValuePair<AttackResult.ElementType, float>(3, attackData.iceDamage),
                    new KeyValuePair<AttackResult.ElementType, float>(1, attackData.fireDamage),
                    new KeyValuePair<AttackResult.ElementType, float>(2, attackData.thunderDamage),
                    new KeyValuePair<AttackResult.ElementType, float>(4, attackData.alienDamage)
                };
                KeyValuePair<AttackResult.ElementType, float> pair = list[0];
                for (int i = 1; i < list.Count; i++)
                {
                    KeyValuePair<AttackResult.ElementType, float> pair2 = list[i];
                    if (pair2.Value > pair.Value)
                    {
                        pair = pair2;
                    }
                }
                attackData.plainDamage = 0f;
                attackData.iceDamage = 0f;
                attackData.fireDamage = 0f;
                attackData.thunderDamage = 0f;
                attackData.alienDamage = 0f;
                switch (pair.Key)
                {
                    case AttackResult.ElementType.Plain:
                        attackData.plainDamage = pair.Value;
                        break;

                    case AttackResult.ElementType.Fire:
                        attackData.fireDamage = pair.Value;
                        break;

                    case AttackResult.ElementType.Thunder:
                        attackData.thunderDamage = pair.Value;
                        break;

                    case AttackResult.ElementType.Ice:
                        attackData.iceDamage = pair.Value;
                        break;

                    case AttackResult.ElementType.Alien:
                        attackData.alienDamage = pair.Value;
                        break;
                }
                attackData.resolveStep = AttackData.AttackDataStep.FinalResolved;
            }
            return attackData;
        }
    }
}

