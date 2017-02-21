namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    [GeneratePartialHash]
    public class ConfigPropObject : IHashable, IEntityConfig, IOnLoaded
    {
        public ConfigEntityAbilityEntry[] Abilities = ConfigEntityAbilityEntry.EMPTY;
        public Dictionary<string, ConfigPropAnimEvent> AnimEvents;
        public ConfigEntityAttackEffect BeHitEffect;
        public ConfigPropObjectCommonArguments CommonArguments = ConfigPropObjectCommonArguments.EMPTY;
        [NonSerialized]
        public ConfigCommonEntity CommonConfig;
        public static Dictionary<string, ConfigAbilityPropertyEntry> EMTPY_PROPERTIES = new Dictionary<string, ConfigAbilityPropertyEntry>();
        public string Name;
        public string PrefabPath;
        public ConfigPropArguments PropArguments;

        ConfigEntityAnimEvent IEntityConfig.TryGetAnimEvent(string animEventID)
        {
            ConfigPropAnimEvent event2;
            this.AnimEvents.TryGetValue(animEventID, out event2);
            return event2;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.Name, ref lastHash);
            HashUtils.ContentHashOnto(this.PrefabPath, ref lastHash);
            if (this.CommonArguments != null)
            {
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
            if (this.PropArguments != null)
            {
                HashUtils.ContentHashOnto(this.PropArguments.IsTargetable, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.IsTriggerField, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.OnlyReduceHPByOne, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.HP, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.Attack, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.UseOwnerAttack, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.EffectDuration, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.CD, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.Length, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.TriggerHitWhenFieldEnter, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.AnimEventIDForHit, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.Acceleration, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.MaxMoveSpeed, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.WarningRange, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.EscapeRange, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.DieWhenFieldEnter, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.OnKillEffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.OnDestroyEffectPattern, ref lastHash);
                HashUtils.ContentHashOnto((int) this.PropArguments.RetreatType, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.Duration, ref lastHash);
                HashUtils.ContentHashOnto(this.PropArguments.CanAffectMonsters, ref lastHash);
            }
            if (this.BeHitEffect != null)
            {
                HashUtils.ContentHashOnto(this.BeHitEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.BeHitEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.BeHitEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.BeHitEffect.AttackEffectTriggerPos, ref lastHash);
            }
            if (this.Abilities != null)
            {
                foreach (ConfigEntityAbilityEntry entry in this.Abilities)
                {
                    HashUtils.ContentHashOnto(entry.AbilityName, ref lastHash);
                    HashUtils.ContentHashOnto(entry.AbilityOverride, ref lastHash);
                }
            }
            if (this.AnimEvents != null)
            {
                foreach (KeyValuePair<string, ConfigPropAnimEvent> pair in this.AnimEvents)
                {
                    HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                    HashUtils.ContentHashOnto(pair.Value.Predicate, ref lastHash);
                    HashUtils.ContentHashOnto(pair.Value.Predicate2, ref lastHash);
                    if (pair.Value.AttackPattern != null)
                    {
                    }
                    if (pair.Value.AttackProperty != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.DamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.AddedDamageValue, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.NormalDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.NormalDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.FireDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.FireDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.ThunderDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.ThunderDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.IceDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.IceDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.AlienDamage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.AlienDamagePercentage, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.AniDamageRatio, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackProperty.HitType, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackProperty.HitEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackProperty.HitEffectAux, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackProperty.KillEffect, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.FrameHalt, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.RetreatVelocity, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.IsAnimEventAttack, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.IsInComboCount, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.SPRecover, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.WitchTimeRatio, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackProperty.AttackTargetting, ref lastHash);
                        if (pair.Value.AttackProperty.CategoryTag != null)
                        {
                            foreach (AttackResult.AttackCategoryTag tag in pair.Value.AttackProperty.CategoryTag)
                            {
                                HashUtils.ContentHashOnto((int) tag, ref lastHash);
                            }
                        }
                    }
                    if (pair.Value.CameraShake != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.CameraShake.ShakeOnNotHit, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.CameraShake.ShakeRange, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.CameraShake.ShakeTime, ref lastHash);
                        if (pair.Value.CameraShake.ShakeAngle.HasValue)
                        {
                            HashUtils.ContentHashOnto(pair.Value.CameraShake.ShakeAngle.Value, ref lastHash);
                        }
                        HashUtils.ContentHashOnto(pair.Value.CameraShake.ShakeStepFrame, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.CameraShake.ClearPreviousShake, ref lastHash);
                    }
                    if (pair.Value.AttackEffect != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.AttackEffect.EffectPattern, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackEffect.SwitchName, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.AttackEffect.MuteAttackEffect, ref lastHash);
                        HashUtils.ContentHashOnto((int) pair.Value.AttackEffect.AttackEffectTriggerPos, ref lastHash);
                    }
                    if (pair.Value.TriggerAbility != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.TriggerAbility.ID, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.TriggerAbility.Name, ref lastHash);
                    }
                    if (pair.Value.TriggerEffectPattern != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.TriggerEffectPattern.EffectPattern, ref lastHash);
                    }
                    if (pair.Value.TimeSlow != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.TimeSlow.Force, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.TimeSlow.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.TimeSlow.SlowRatio, ref lastHash);
                    }
                    if (pair.Value.TriggerTintCamera != null)
                    {
                        HashUtils.ContentHashOnto(pair.Value.TriggerTintCamera.RenderDataName, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.TriggerTintCamera.Duration, ref lastHash);
                        HashUtils.ContentHashOnto(pair.Value.TriggerTintCamera.TransitDuration, ref lastHash);
                    }
                }
            }
        }

        public void OnLoaded()
        {
            ConfigCommonEntity entity = new ConfigCommonEntity {
                EntityProperties = EMTPY_PROPERTIES,
                CommonArguments = this.CommonArguments
            };
            this.CommonConfig = entity;
        }

        public enum E_RetreatType
        {
            Common,
            Spike
        }
    }
}

