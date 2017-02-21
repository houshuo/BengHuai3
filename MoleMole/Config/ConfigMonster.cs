namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [GeneratePartialHash]
    public class ConfigMonster : ConfigAnimatorEntity, IHashable, IEntityConfig, IOnLoaded
    {
        public Dictionary<string, ConfigEntityAbilityEntry> Abilities = new Dictionary<string, ConfigEntityAbilityEntry>();
        public ConfigMonsterAIArguments AIArguments;
        public Dictionary<string, bool> AnimatorConfig = new Dictionary<string, bool>();
        public Dictionary<string, ConfigMonsterAnimEvent> AnimEvents = new Dictionary<string, ConfigMonsterAnimEvent>();
        public List<List<string>> ATKRatioNames = new List<List<string>>();
        public ConfigMonsterCommonArguments CommonArguments;
        public ConfigDebuffResistance DebuffResistance = new ConfigDebuffResistance();
        public ConfigDynamicArguments DynamicArguments = new ConfigDynamicArguments();
        public ConfigMonsterEliteArguments EliteArguments;
        public static Dictionary<string, ConfigAbilityPropertyEntry> EMTPY_PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();
        public Dictionary<string, ConfigAbilityPropertyEntry> EntityProperties = EMTPY_PROPERTIES;
        public Dictionary<string, ConfigMultiAnimEvent> MultiAnimEvents = new Dictionary<string, ConfigMultiAnimEvent>();
        public Dictionary<string, ConfigNamedState> NamedStates = new Dictionary<string, ConfigNamedState>();
        public Dictionary<string, ConfigMonsterSkill> Skills = new Dictionary<string, ConfigMonsterSkill>();
        public ConfigMonsterStateMachinePattern StateMachinePattern;
        [NonSerialized, ShowInInspector]
        public Dictionary<int, string> StateToNamedStateMap;
        [NonSerialized, ShowInInspector]
        public Dictionary<int, string> StateToSkillIDMap;

        ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
        {
            ConfigMonsterAnimEvent event2;
            this.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.CommonArguments != null)
            {
                HashUtils.ContentHashOnto(this.CommonArguments.HP, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.Attack, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.Defence, ref lastHash);
                if (this.CommonArguments.FadeInHeight.HasValue)
                {
                    HashUtils.ContentHashOnto(this.CommonArguments.FadeInHeight.Value, ref lastHash);
                }
                HashUtils.ContentHashOnto(this.CommonArguments.BePushedSpeedRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.BePushedSpeedRatioThrow, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.HitboxInactiveDelay, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.UseTransparentShaderDistanceThreshold, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.UseSwitchShader, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.UseEliteShader, ref lastHash);
                HashUtils.ContentHashOnto((int) this.CommonArguments.Nature, ref lastHash);
                HashUtils.ContentHashOnto((int) this.CommonArguments.Class, ref lastHash);
                HashUtils.ContentHashOnto((int) this.CommonArguments.RoleName, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.DefaultAnimEventPredicate, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CreatePosYOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CreateCollisionRadius, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CreateCollisionHeight, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CollisionLevel, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CollisionRadius, ref lastHash);
                if (this.CommonArguments.PreloadEffectPatternGroups != null)
                {
                    foreach (string str in this.CommonArguments.PreloadEffectPatternGroups)
                    {
                        HashUtils.ContentHashOnto(str, ref lastHash);
                    }
                }
                if (this.CommonArguments.RequestSoundBankNames != null)
                {
                    foreach (string str2 in this.CommonArguments.RequestSoundBankNames)
                    {
                        HashUtils.ContentHashOnto(str2, ref lastHash);
                    }
                }
                if (this.CommonArguments.EffectPredicates != null)
                {
                    foreach (string str3 in this.CommonArguments.EffectPredicates)
                    {
                        HashUtils.ContentHashOnto(str3, ref lastHash);
                    }
                }
                HashUtils.ContentHashOnto(this.CommonArguments.HasLowPrefab, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CameraMinAngleRatio, ref lastHash);
            }
            if (this.StateMachinePattern != null)
            {
                HashUtils.ContentHashOnto(this.StateMachinePattern.AIMode, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowAnimDefenceRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowDieEffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.FastDieEffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.FastDieAnimationWaitDuration, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowUpNamedState, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowUpNamedStateRetreatStopNormalizedTime, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowBlowNamedState, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowBlowDieNamedState, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowBlowAirNamedState, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ThrowBlowAirNamedStateRetreatStopNormalizedTime, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.RetreatToVelocityScaleRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.RetreatBlowVelocityRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.HeavyRetreatThreshold, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.UseRandomLeftRightHitEffectAsNormal, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.UseBackHitAngleCheck, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.BackHitDegreeThreshold, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.UseLeftRightHitAngleCheck, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.LeftRightHitAngleRange, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.UseStandByWalkSteer, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.WalkSteerTimeThreshold, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.WalkSteerAnimatorStateName, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.KeepHitboxStanding, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.KeepHitboxStandingMinHeight, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.UseAbsMoveSpeed, ref lastHash);
                if (this.StateMachinePattern.BeHitEffect != null)
                {
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffect.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffect.SwitchName, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffect.MuteAttackEffect, ref lastHash);
                    HashUtils.ContentHashOnto((int) this.StateMachinePattern.BeHitEffect.AttackEffectTriggerPos, ref lastHash);
                }
                if (this.StateMachinePattern.BeHitEffectMid != null)
                {
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectMid.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectMid.SwitchName, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectMid.MuteAttackEffect, ref lastHash);
                    HashUtils.ContentHashOnto((int) this.StateMachinePattern.BeHitEffectMid.AttackEffectTriggerPos, ref lastHash);
                }
                if (this.StateMachinePattern.BeHitEffectBig != null)
                {
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectBig.EffectPattern, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectBig.SwitchName, ref lastHash);
                    HashUtils.ContentHashOnto(this.StateMachinePattern.BeHitEffectBig.MuteAttackEffect, ref lastHash);
                    HashUtils.ContentHashOnto((int) this.StateMachinePattern.BeHitEffectBig.AttackEffectTriggerPos, ref lastHash);
                }
                HashUtils.ContentHashOnto(this.StateMachinePattern.DieAnimEventID, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ConstMoveSpeed, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.AniMinSpeedRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.AniMaxSpeedRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ChangeDirLerpRatioForMove, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.DefaultAnimDefenceRatio, ref lastHash);
            }
            if (this.DynamicArguments != null)
            {
                foreach (KeyValuePair<string, object> pair in this.DynamicArguments)
                {
                    HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                    HashUtils.ContentHashOntoFallback(pair.Value, ref lastHash);
                }
            }
            if (this.EliteArguments != null)
            {
                HashUtils.ContentHashOnto(this.EliteArguments.HPRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.DefenseRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.AttackRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.DebuffResistanceRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.HexColorElite1, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.EliteEmissionScaler1, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.EliteNormalDisplacement1, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.HexColorElite2, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.EliteEmissionScaler2, ref lastHash);
                HashUtils.ContentHashOnto(this.EliteArguments.EliteNormalDisplacement2, ref lastHash);
            }
            if (this.AIArguments != null)
            {
                HashUtils.ContentHashOnto(this.AIArguments.AttackRange, ref lastHash);
            }
            if (this.EntityProperties != null)
            {
                foreach (KeyValuePair<string, ConfigAbilityPropertyEntry> pair2 in this.EntityProperties)
                {
                    HashUtils.ContentHashOnto(pair2.Key, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair2.Value.Type, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.Default, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.Ceiling, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.Floor, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair2.Value.Stacking, ref lastHash);
                }
            }
            if (this.Abilities != null)
            {
                foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair3 in this.Abilities)
                {
                    HashUtils.ContentHashOnto(pair3.Key, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AbilityName, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AbilityOverride, ref lastHash);
                }
            }
            if (this.Skills != null)
            {
                foreach (KeyValuePair<string, ConfigMonsterSkill> pair4 in this.Skills)
                {
                    HashUtils.ContentHashOnto(pair4.Key, ref lastHash);
                    if (pair4.Value.AnimatorStateNames != null)
                    {
                        foreach (string str4 in pair4.Value.AnimatorStateNames)
                        {
                            HashUtils.ContentHashOnto(str4, ref lastHash);
                        }
                    }
                    HashUtils.ContentHashOnto(pair4.Value.AnimatorEventPattern, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.AnimDefenceRatio, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.AnimDefenceNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.AnimDefenceNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.AttackNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.AttackNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.HighSpeedMovement, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.SteerToTargetOnEnter, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.MassRatio, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.NeedClearEffect, ref lastHash);
                    HashUtils.ContentHashOnto(pair4.Value.Unselectable, ref lastHash);
                }
            }
            if (this.NamedStates != null)
            {
                foreach (KeyValuePair<string, ConfigNamedState> pair5 in this.NamedStates)
                {
                    HashUtils.ContentHashOnto(pair5.Key, ref lastHash);
                    if (pair5.Value.AnimatorStateNames != null)
                    {
                        foreach (string str5 in pair5.Value.AnimatorStateNames)
                        {
                            HashUtils.ContentHashOnto(str5, ref lastHash);
                        }
                    }
                    HashUtils.ContentHashOnto(pair5.Value.HighSpeedMovement, ref lastHash);
                }
            }
            if (this.AnimEvents != null)
            {
                foreach (KeyValuePair<string, ConfigMonsterAnimEvent> pair6 in this.AnimEvents)
                {
                    HashUtils.ContentHashOnto(pair6.Key, ref lastHash);
                    if (pair6.Value.AttackHint != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.AttackHint.InnerStartDelay, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackHint.InnerInflateDuration, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackHint.OffsetBase, ref lastHash);
                    }
                    if (pair6.Value.PhysicsProperty != null)
                    {
                    }
                    HashUtils.ContentHashOnto(pair6.Value.Predicate, ref lastHash);
                    HashUtils.ContentHashOnto(pair6.Value.Predicate2, ref lastHash);
                    if (pair6.Value.AttackPattern != null)
                    {
                    }
                    if (pair6.Value.AttackProperty != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.DamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.AddedDamageValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.NormalDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.NormalDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.FireDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.FireDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.ThunderDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.ThunderDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.IceDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.IceDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.AlienDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.AlienDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.AniDamageRatio, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackProperty.HitType, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackProperty.HitEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackProperty.HitEffectAux, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackProperty.KillEffect, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.FrameHalt, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.RetreatVelocity, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.IsAnimEventAttack, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.IsInComboCount, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.SPRecover, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.WitchTimeRatio, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackProperty.AttackTargetting, ref lastHash);
                        if (pair6.Value.AttackProperty.CategoryTag != null)
                        {
                            foreach (AttackResult.AttackCategoryTag tag in pair6.Value.AttackProperty.CategoryTag)
                            {
                                HashUtils.ContentHashOnto((int) tag, ref lastHash);
                            }
                        }
                    }
                    if (pair6.Value.CameraShake != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.CameraShake.ShakeOnNotHit, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.CameraShake.ShakeRange, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.CameraShake.ShakeTime, ref lastHash);
                        if (pair6.Value.CameraShake.ShakeAngle.HasValue)
                        {
                            HashUtils.ContentHashOnto(pair6.Value.CameraShake.ShakeAngle.Value, ref lastHash);
                        }
                        HashUtils.ContentHashOnto(pair6.Value.CameraShake.ShakeStepFrame, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.CameraShake.ClearPreviousShake, ref lastHash);
                    }
                    if (pair6.Value.AttackEffect != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.AttackEffect.EffectPattern, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackEffect.SwitchName, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.AttackEffect.MuteAttackEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair6.Value.AttackEffect.AttackEffectTriggerPos, ref lastHash);
                    }
                    if (pair6.Value.TriggerAbility != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.TriggerAbility.ID, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.TriggerAbility.Name, ref lastHash);
                    }
                    if (pair6.Value.TriggerEffectPattern != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.TriggerEffectPattern.EffectPattern, ref lastHash);
                    }
                    if (pair6.Value.TimeSlow != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.TimeSlow.Force, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.TimeSlow.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.TimeSlow.SlowRatio, ref lastHash);
                    }
                    if (pair6.Value.TriggerTintCamera != null)
                    {
                        HashUtils.ContentHashOnto(pair6.Value.TriggerTintCamera.RenderDataName, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.TriggerTintCamera.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair6.Value.TriggerTintCamera.TransitDuration, ref lastHash);
                    }
                }
            }
            if (this.MultiAnimEvents != null)
            {
                foreach (KeyValuePair<string, ConfigMultiAnimEvent> pair7 in this.MultiAnimEvents)
                {
                    HashUtils.ContentHashOnto(pair7.Key, ref lastHash);
                    if (pair7.Value.AnimEventNames != null)
                    {
                        foreach (string str6 in pair7.Value.AnimEventNames)
                        {
                            HashUtils.ContentHashOnto(str6, ref lastHash);
                        }
                    }
                }
            }
            if (this.AnimatorConfig != null)
            {
                foreach (KeyValuePair<string, bool> pair8 in this.AnimatorConfig)
                {
                    HashUtils.ContentHashOnto(pair8.Key, ref lastHash);
                    HashUtils.ContentHashOnto(pair8.Value, ref lastHash);
                }
            }
            if (this.ATKRatioNames != null)
            {
                foreach (List<string> list in this.ATKRatioNames)
                {
                    if (list != null)
                    {
                        foreach (string str7 in list)
                        {
                            HashUtils.ContentHashOnto(str7, ref lastHash);
                        }
                    }
                }
            }
            if (this.DebuffResistance != null)
            {
                if (this.DebuffResistance.ImmuneStates != null)
                {
                    foreach (AbilityState state in this.DebuffResistance.ImmuneStates)
                    {
                        HashUtils.ContentHashOnto((int) state, ref lastHash);
                    }
                }
                HashUtils.ContentHashOnto(this.DebuffResistance.ResistanceRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.DebuffResistance.DurationRatio, ref lastHash);
            }
            if (base.AnimatorStateParamBinds != null)
            {
                foreach (ConfigBindAnimatorStateToParameter parameter in base.AnimatorStateParamBinds)
                {
                    if (parameter.AnimatorStateNames != null)
                    {
                        foreach (string str8 in parameter.AnimatorStateNames)
                        {
                            HashUtils.ContentHashOnto(str8, ref lastHash);
                        }
                    }
                    if (parameter.ParameterConfig != null)
                    {
                        HashUtils.ContentHashOnto(parameter.ParameterConfig.ParameterID, ref lastHash);
                        HashUtils.ContentHashOnto(parameter.ParameterConfig.ParameterIDSub, ref lastHash);
                        HashUtils.ContentHashOnto(parameter.ParameterConfig.NormalizedTimeStart, ref lastHash);
                        HashUtils.ContentHashOnto(parameter.ParameterConfig.NormalizedTimeStop, ref lastHash);
                    }
                }
            }
            if (base.MPArguments != null)
            {
                HashUtils.ContentHashOnto(base.MPArguments.SyncSendInterval, ref lastHash);
                HashUtils.ContentHashOnto((int) base.MPArguments.RemoteMode, ref lastHash);
                if (base.MPArguments.MuteSyncAnimatorTags != null)
                {
                    foreach (string str9 in base.MPArguments.MuteSyncAnimatorTags)
                    {
                        HashUtils.ContentHashOnto(str9, ref lastHash);
                    }
                }
            }
        }

        public override void OnLevelLoaded()
        {
            base.OnLevelLoaded();
            this.StateToSkillIDMap = new Dictionary<int, string>();
            this.StateToNamedStateMap = new Dictionary<int, string>();
            foreach (KeyValuePair<string, ConfigMonsterSkill> pair in this.Skills)
            {
                string key = pair.Key;
                ConfigMonsterSkill skill = pair.Value;
                if (skill.AnimatorStateNames != null)
                {
                    int index = 0;
                    int length = skill.AnimatorStateNames.Length;
                    while (index < length)
                    {
                        this.StateToSkillIDMap.Add(Animator.StringToHash(skill.AnimatorStateNames[index]), key);
                        index++;
                    }
                }
            }
            foreach (KeyValuePair<string, ConfigNamedState> pair2 in this.NamedStates)
            {
                string str2 = pair2.Key;
                ConfigNamedState state = pair2.Value;
                for (int i = 0; i < state.AnimatorStateNames.Length; i++)
                {
                    int num4 = Animator.StringToHash(state.AnimatorStateNames[i]);
                    this.StateToNamedStateMap.Add(num4, str2);
                }
            }
            ColorUtility.TryParseHtmlString(this.EliteArguments.HexColorElite1, out this.EliteArguments.EliteColor1);
            ColorUtility.TryParseHtmlString(this.EliteArguments.HexColorElite2, out this.EliteArguments.EliteColor2);
        }

        public void OnLoaded()
        {
            if (this.AIArguments == null)
            {
                this.AIArguments = new ConfigMonsterAIArguments();
            }
            if (base.MPArguments == null)
            {
                base.MPArguments = MPData.MONSTER_DEFAULT_MP_SETTINGS;
            }
            ConfigCommonEntity entity = new ConfigCommonEntity {
                EntityProperties = this.EntityProperties,
                CommonArguments = this.CommonArguments,
                MPArguments = base.MPArguments
            };
            base.CommonConfig = entity;
        }
    }
}

