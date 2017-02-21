namespace MoleMole.Config
{
    using FullInspector;
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [GeneratePartialHash]
    public class ConfigAvatar : ConfigAnimatorEntity, IHashable, IEntityConfig, IOnLoaded
    {
        public Dictionary<string, ConfigEntityAbilityEntry> Abilities = new Dictionary<string, ConfigEntityAbilityEntry>();
        public ConfigAvatarAbilityUnlock[] AbilitiesUnlock = ConfigAvatarAbilityUnlock.EMTPY;
        public ConfigAvatarAIArguments AIArguments;
        public Dictionary<string, ConfigAvatarAnimEvent> AnimEvents = new Dictionary<string, ConfigAvatarAnimEvent>();
        public AvatarAttackTargetSelect AttackTargetSelectPattern;
        public Dictionary<AvatarCinemaType, string> CinemaPaths = new Dictionary<AvatarCinemaType, string>();
        public ConfigAvatarCommonArguments CommonArguments;
        public ConfigDebuffResistance DebuffResistance = new ConfigDebuffResistance();
        public Dictionary<string, ConfigAbilityPropertyEntry> EntityProperties = new Dictionary<string, ConfigAbilityPropertyEntry>();
        public ConfigLevelEndAnimation LevelEndAnimation = new ConfigLevelEndAnimation();
        public Dictionary<string, ConfigMultiAnimEvent> MultiAnimEvents = new Dictionary<string, ConfigMultiAnimEvent>();
        public Dictionary<string, ConfigNamedState> NamedStates = new Dictionary<string, ConfigNamedState>();
        public Dictionary<string, ConfigAvatarSkill> Skills = new Dictionary<string, ConfigAvatarSkill>();
        public ConfigAvatarStateMachinePattern StateMachinePattern;
        [NonSerialized, ShowInInspector]
        public Dictionary<int, string> StateToNamedStateMap;
        [NonSerialized, ShowInInspector]
        public Dictionary<int, string> StateToSkillIDMap;
        public ConfigStoryStateSetting StoryCameraSetting = new ConfigStoryStateSetting();

        ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
        {
            ConfigAvatarAnimEvent event2;
            this.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.CommonArguments != null)
            {
                HashUtils.ContentHashOnto(this.CommonArguments.GoodsAttractRadius, ref lastHash);
                if (this.CommonArguments.MaskedSkillButtons != null)
                {
                    foreach (string str in this.CommonArguments.MaskedSkillButtons)
                    {
                        HashUtils.ContentHashOnto(str, ref lastHash);
                    }
                }
                HashUtils.ContentHashOnto(this.CommonArguments.SwitchInCD, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.QTESwitchInCDRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.AttackSPRecoverRatio, ref lastHash);
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
                    foreach (string str2 in this.CommonArguments.PreloadEffectPatternGroups)
                    {
                        HashUtils.ContentHashOnto(str2, ref lastHash);
                    }
                }
                if (this.CommonArguments.RequestSoundBankNames != null)
                {
                    foreach (string str3 in this.CommonArguments.RequestSoundBankNames)
                    {
                        HashUtils.ContentHashOnto(str3, ref lastHash);
                    }
                }
                if (this.CommonArguments.EffectPredicates != null)
                {
                    foreach (string str4 in this.CommonArguments.EffectPredicates)
                    {
                        HashUtils.ContentHashOnto(str4, ref lastHash);
                    }
                }
                HashUtils.ContentHashOnto(this.CommonArguments.HasLowPrefab, ref lastHash);
                HashUtils.ContentHashOnto(this.CommonArguments.CameraMinAngleRatio, ref lastHash);
            }
            if (this.StateMachinePattern != null)
            {
                HashUtils.ContentHashOnto(this.StateMachinePattern.IdleCD, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.SwitchInAnimatorStateName, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.SwitchOutAnimatorStateName, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ConstMoveSpeed, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.AniMinSpeedRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.AniMaxSpeedRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.ChangeDirLerpRatioForMove, ref lastHash);
                HashUtils.ContentHashOnto(this.StateMachinePattern.DefaultAnimDefenceRatio, ref lastHash);
            }
            if ((this.AttackTargetSelectPattern != null) && (this.AttackTargetSelectPattern.selectMethod != null))
            {
            }
            if (this.AIArguments != null)
            {
                HashUtils.ContentHashOnto(this.AIArguments.AttackDistance, ref lastHash);
                HashUtils.ContentHashOnto(this.AIArguments.SupporterAI, ref lastHash);
            }
            if (this.EntityProperties != null)
            {
                foreach (KeyValuePair<string, ConfigAbilityPropertyEntry> pair in this.EntityProperties)
                {
                    HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair.Value.Type, ref lastHash);
                    HashUtils.ContentHashOnto(pair.Value.Default, ref lastHash);
                    HashUtils.ContentHashOnto(pair.Value.Ceiling, ref lastHash);
                    HashUtils.ContentHashOnto(pair.Value.Floor, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair.Value.Stacking, ref lastHash);
                }
            }
            if (this.AbilitiesUnlock != null)
            {
                foreach (ConfigAvatarAbilityUnlock unlock in this.AbilitiesUnlock)
                {
                    HashUtils.ContentHashOnto(unlock.IsUnlockBySkill, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.UnlockBySkillID, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.UnlockBySubSkillID, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.AbilityName, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.AbilityOverride, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.AbilityReplaceID, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.ParamSpecial1, ref lastHash);
                    HashUtils.ContentHashOnto((int) unlock.ParamMethod1, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.ParamSpecial2, ref lastHash);
                    HashUtils.ContentHashOnto((int) unlock.ParamMethod2, ref lastHash);
                    HashUtils.ContentHashOnto(unlock.ParamSpecial3, ref lastHash);
                    HashUtils.ContentHashOnto((int) unlock.ParamMethod3, ref lastHash);
                }
            }
            if (this.Abilities != null)
            {
                foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair2 in this.Abilities)
                {
                    HashUtils.ContentHashOnto(pair2.Key, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.AbilityName, ref lastHash);
                    HashUtils.ContentHashOnto(pair2.Value.AbilityOverride, ref lastHash);
                }
            }
            if (this.Skills != null)
            {
                foreach (KeyValuePair<string, ConfigAvatarSkill> pair3 in this.Skills)
                {
                    HashUtils.ContentHashOnto(pair3.Key, ref lastHash);
                    if (pair3.Value.AnimatorStateNames != null)
                    {
                        foreach (string str5 in pair3.Value.AnimatorStateNames)
                        {
                            HashUtils.ContentHashOnto(str5, ref lastHash);
                        }
                    }
                    HashUtils.ContentHashOnto(pair3.Value.AnimatorEventPattern, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair3.Value.SkillType, ref lastHash);
                    if (pair3.Value.SPCostDelta != null)
                    {
                        HashUtils.ContentHashOnto(pair3.Value.SPCostDelta.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.SPCostDelta.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.SPCostDelta.dynamicKey, ref lastHash);
                    }
                    if (pair3.Value.SkillCDDelta != null)
                    {
                        HashUtils.ContentHashOnto(pair3.Value.SkillCDDelta.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.SkillCDDelta.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.SkillCDDelta.dynamicKey, ref lastHash);
                    }
                    HashUtils.ContentHashOnto(pair3.Value.CanHold, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.MuteHighlighted, ref lastHash);
                    if (pair3.Value.ChargesCountDelta != null)
                    {
                        HashUtils.ContentHashOnto(pair3.Value.ChargesCountDelta.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.ChargesCountDelta.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.ChargesCountDelta.dynamicKey, ref lastHash);
                    }
                    HashUtils.ContentHashOnto(pair3.Value.HaveBranch, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.IsInstantTrigger, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.InstantTriggerEvent, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.ForceMuteSteer, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.BranchHighlightNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.BranchHighlightNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AnimDefenceRatio, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AnimDefenceNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AnimDefenceNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.ComboTimerPauseNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.ComboTimerPauseNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.LastKillCameraAnimation, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AttackNormalizedTimeStart, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.AttackNormalizedTimeStop, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair3.Value.EnterSteer, ref lastHash);
                    if (pair3.Value.EnterSteerOption != null)
                    {
                        HashUtils.ContentHashOnto((int) pair3.Value.EnterSteerOption.SteerType, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.EnterSteerOption.MaxSteeringAngle, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.EnterSteerOption.SteerLerpRatio, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.EnterSteerOption.MaxSteerNormalizedTimeStart, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.EnterSteerOption.MaxSteerNormalizedTimeEnd, ref lastHash);
                        HashUtils.ContentHashOnto(pair3.Value.EnterSteerOption.MuteSteerWhenNoEnemy, ref lastHash);
                    }
                    HashUtils.ContentHashOnto(pair3.Value.HighSpeedMovement, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.MassRatio, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.NeedClearEffect, ref lastHash);
                    HashUtils.ContentHashOnto(pair3.Value.MuteCameraControl, ref lastHash);
                    HashUtils.ContentHashOnto((int) pair3.Value.ReviveCDAction, ref lastHash);
                    if (pair3.Value.SkillCategoryTag != null)
                    {
                        foreach (AttackResult.AttackCategoryTag tag in pair3.Value.SkillCategoryTag)
                        {
                            HashUtils.ContentHashOnto((int) tag, ref lastHash);
                        }
                    }
                }
            }
            if (this.NamedStates != null)
            {
                foreach (KeyValuePair<string, ConfigNamedState> pair4 in this.NamedStates)
                {
                    HashUtils.ContentHashOnto(pair4.Key, ref lastHash);
                    if (pair4.Value.AnimatorStateNames != null)
                    {
                        foreach (string str6 in pair4.Value.AnimatorStateNames)
                        {
                            HashUtils.ContentHashOnto(str6, ref lastHash);
                        }
                    }
                    HashUtils.ContentHashOnto(pair4.Value.HighSpeedMovement, ref lastHash);
                }
            }
            if (this.AnimEvents != null)
            {
                foreach (KeyValuePair<string, ConfigAvatarAnimEvent> pair5 in this.AnimEvents)
                {
                    HashUtils.ContentHashOnto(pair5.Key, ref lastHash);
                    if (pair5.Value.PhysicsProperty != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.PhysicsProperty.IsFreezeDirection, ref lastHash);
                    }
                    if (pair5.Value.CameraAction != null)
                    {
                    }
                    if (pair5.Value.LastKillCameraAnimation != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.LastKillCameraAnimation.AnimationName, ref lastHash);
                    }
                    if (pair5.Value.WitchTimeResume != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.WitchTimeResume.ResumeTime, ref lastHash);
                    }
                    if (pair5.Value.MissionSpecificKill != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.MissionSpecificKill.FinishParaInt, ref lastHash);
                    }
                    HashUtils.ContentHashOnto(pair5.Value.Predicate, ref lastHash);
                    HashUtils.ContentHashOnto(pair5.Value.Predicate2, ref lastHash);
                    if (pair5.Value.AttackPattern != null)
                    {
                    }
                    if (pair5.Value.AttackProperty != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.DamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.AddedDamageValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.NormalDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.NormalDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.FireDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.FireDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.ThunderDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.ThunderDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.IceDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.IceDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.AlienDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.AlienDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.AniDamageRatio, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackProperty.HitType, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackProperty.HitEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackProperty.HitEffectAux, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackProperty.KillEffect, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.FrameHalt, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.RetreatVelocity, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.IsAnimEventAttack, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.IsInComboCount, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.SPRecover, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.WitchTimeRatio, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackProperty.AttackTargetting, ref lastHash);
                        if (pair5.Value.AttackProperty.CategoryTag != null)
                        {
                            foreach (AttackResult.AttackCategoryTag tag2 in pair5.Value.AttackProperty.CategoryTag)
                            {
                                HashUtils.ContentHashOnto((int) tag2, ref lastHash);
                            }
                        }
                    }
                    if (pair5.Value.CameraShake != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.CameraShake.ShakeOnNotHit, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.CameraShake.ShakeRange, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.CameraShake.ShakeTime, ref lastHash);
                        if (pair5.Value.CameraShake.ShakeAngle.HasValue)
                        {
                            HashUtils.ContentHashOnto(pair5.Value.CameraShake.ShakeAngle.Value, ref lastHash);
                        }
                        HashUtils.ContentHashOnto(pair5.Value.CameraShake.ShakeStepFrame, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.CameraShake.ClearPreviousShake, ref lastHash);
                    }
                    if (pair5.Value.AttackEffect != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.AttackEffect.EffectPattern, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackEffect.SwitchName, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.AttackEffect.MuteAttackEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair5.Value.AttackEffect.AttackEffectTriggerPos, ref lastHash);
                    }
                    if (pair5.Value.TriggerAbility != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.TriggerAbility.ID, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.TriggerAbility.Name, ref lastHash);
                    }
                    if (pair5.Value.TriggerEffectPattern != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.TriggerEffectPattern.EffectPattern, ref lastHash);
                    }
                    if (pair5.Value.TimeSlow != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.TimeSlow.Force, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.TimeSlow.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.TimeSlow.SlowRatio, ref lastHash);
                    }
                    if (pair5.Value.TriggerTintCamera != null)
                    {
                        HashUtils.ContentHashOnto(pair5.Value.TriggerTintCamera.RenderDataName, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.TriggerTintCamera.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair5.Value.TriggerTintCamera.TransitDuration, ref lastHash);
                    }
                }
            }
            if (this.MultiAnimEvents != null)
            {
                foreach (KeyValuePair<string, ConfigMultiAnimEvent> pair6 in this.MultiAnimEvents)
                {
                    HashUtils.ContentHashOnto(pair6.Key, ref lastHash);
                    if (pair6.Value.AnimEventNames != null)
                    {
                        foreach (string str7 in pair6.Value.AnimEventNames)
                        {
                            HashUtils.ContentHashOnto(str7, ref lastHash);
                        }
                    }
                }
            }
            if (this.CinemaPaths != null)
            {
                foreach (KeyValuePair<AvatarCinemaType, string> pair7 in this.CinemaPaths)
                {
                    HashUtils.ContentHashOnto(pair7.Key, ref lastHash);
                    HashUtils.ContentHashOnto(pair7.Value, ref lastHash);
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
            if (this.LevelEndAnimation != null)
            {
                HashUtils.ContentHashOnto(this.LevelEndAnimation.LevelWinAnim, ref lastHash);
                HashUtils.ContentHashOnto(this.LevelEndAnimation.LevelLoseAnim, ref lastHash);
            }
            if (this.StoryCameraSetting != null)
            {
                HashUtils.ContentHashOnto(this.StoryCameraSetting.anchorRadius, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.yaw, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.pitch, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.yOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.xOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.fov, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.screenZOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.screenYOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.screenXOffset, ref lastHash);
                HashUtils.ContentHashOnto(this.StoryCameraSetting.screenScale, ref lastHash);
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
            foreach (KeyValuePair<string, ConfigAvatarSkill> pair in this.Skills)
            {
                string key = pair.Key;
                ConfigAvatarSkill skill = pair.Value;
                for (int i = 0; i < skill.AnimatorStateNames.Length; i++)
                {
                    int num2 = Animator.StringToHash(skill.AnimatorStateNames[i]);
                    this.StateToSkillIDMap.Add(num2, key);
                }
            }
            foreach (KeyValuePair<string, ConfigNamedState> pair2 in this.NamedStates)
            {
                string str2 = pair2.Key;
                ConfigNamedState state = pair2.Value;
                for (int j = 0; j < state.AnimatorStateNames.Length; j++)
                {
                    int num4 = Animator.StringToHash(state.AnimatorStateNames[j]);
                    this.StateToNamedStateMap.Add(num4, str2);
                }
            }
            this.StateMachinePattern.SwitchInAnimatorStateHash = Animator.StringToHash(this.StateMachinePattern.SwitchInAnimatorStateName);
            this.StateMachinePattern.SwitchOutAnimatorStateHash = Animator.StringToHash(this.StateMachinePattern.SwitchOutAnimatorStateName);
        }

        public void OnLoaded()
        {
            if (base.MPArguments == null)
            {
                base.MPArguments = MPData.AVATAR_DEFAULT_MP_SETTINGS;
            }
            ConfigCommonEntity entity = new ConfigCommonEntity {
                CommonArguments = this.CommonArguments,
                EntityProperties = this.EntityProperties,
                MPArguments = base.MPArguments
            };
            base.CommonConfig = entity;
        }
    }
}

