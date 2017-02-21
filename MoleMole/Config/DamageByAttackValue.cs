namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DamageByAttackValue : ConfigAbilityAction, IHashable
    {
        public DynamicFloat AddedDamageValue = DynamicFloat.ZERO;
        public DynamicFloat AlienDamage = DynamicFloat.ZERO;
        public DynamicFloat AlienDamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat AniDamageRatio = DynamicFloat.ZERO;
        public ConfigEntityAttackEffect AttackEffect;
        public ConfigEntityCameraShake CameraShake;
        public DynamicFloat DamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat FireDamage = DynamicFloat.ZERO;
        public DynamicFloat FireDamagePercentage = DynamicFloat.ZERO;
        public DynamicInt FrameHalt = DynamicInt.ZERO;
        public AttackResult.AnimatorHitEffect HitEffect = AttackResult.AnimatorHitEffect.Normal;
        public AttackResult.ActorHitLevel HitLevel;
        public DynamicFloat IceDamage = DynamicFloat.ZERO;
        public DynamicFloat IceDamagePercentage = DynamicFloat.ZERO;
        public bool IsAnimEventAttack;
        public bool IsInComboCount;
        public DynamicFloat PlainDamage = DynamicFloat.ZERO;
        public DynamicFloat PlainDamagePercentage = DynamicFloat.ZERO;
        public DynamicFloat RetreatVelocity = DynamicFloat.ZERO;
        public DynamicFloat ThunderDamage = DynamicFloat.ZERO;
        public DynamicFloat ThunderDamagePercentage = DynamicFloat.ZERO;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.DamageByAttackValueHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override MPActorAbilityPlugin.MPAuthorityActionHandler MPGetAuthorityHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPAuthorityActionHandler(mpAbilityPlugin.DamageByAttackValue_AuthorityHandler);
        }

        public override MPActorAbilityPlugin.MPRemoteActionHandler MPGetRemoteHandler(MPActorAbilityPlugin mpAbilityPlugin)
        {
            return new MPActorAbilityPlugin.MPRemoteActionHandler(MPActorAbilityPlugin.STUB_RemoteMute);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.DamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.DamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AddedDamageValue != null)
            {
                HashUtils.ContentHashOnto(this.AddedDamageValue.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageValue.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AddedDamageValue.dynamicKey, ref lastHash);
            }
            if (this.PlainDamage != null)
            {
                HashUtils.ContentHashOnto(this.PlainDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PlainDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PlainDamage.dynamicKey, ref lastHash);
            }
            if (this.PlainDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.PlainDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.PlainDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.PlainDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.FireDamage != null)
            {
                HashUtils.ContentHashOnto(this.FireDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamage.dynamicKey, ref lastHash);
            }
            if (this.FireDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.FireDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.FireDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.ThunderDamage != null)
            {
                HashUtils.ContentHashOnto(this.ThunderDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamage.dynamicKey, ref lastHash);
            }
            if (this.ThunderDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.ThunderDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.IceDamage != null)
            {
                HashUtils.ContentHashOnto(this.IceDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamage.dynamicKey, ref lastHash);
            }
            if (this.IceDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.IceDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.IceDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AlienDamage != null)
            {
                HashUtils.ContentHashOnto(this.AlienDamage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AlienDamage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AlienDamage.dynamicKey, ref lastHash);
            }
            if (this.AlienDamagePercentage != null)
            {
                HashUtils.ContentHashOnto(this.AlienDamagePercentage.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AlienDamagePercentage.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AlienDamagePercentage.dynamicKey, ref lastHash);
            }
            if (this.AniDamageRatio != null)
            {
                HashUtils.ContentHashOnto(this.AniDamageRatio.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatio.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AniDamageRatio.dynamicKey, ref lastHash);
            }
            if (this.RetreatVelocity != null)
            {
                HashUtils.ContentHashOnto(this.RetreatVelocity.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.RetreatVelocity.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.RetreatVelocity.dynamicKey, ref lastHash);
            }
            if (this.FrameHalt != null)
            {
                HashUtils.ContentHashOnto(this.FrameHalt.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.FrameHalt.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.FrameHalt.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto((int) this.HitEffect, ref lastHash);
            if (this.AttackEffect != null)
            {
                HashUtils.ContentHashOnto(this.AttackEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackEffect.AttackEffectTriggerPos, ref lastHash);
            }
            if (this.CameraShake != null)
            {
                HashUtils.ContentHashOnto(this.CameraShake.ShakeOnNotHit, ref lastHash);
                HashUtils.ContentHashOnto(this.CameraShake.ShakeRange, ref lastHash);
                HashUtils.ContentHashOnto(this.CameraShake.ShakeTime, ref lastHash);
                if (this.CameraShake.ShakeAngle.HasValue)
                {
                    HashUtils.ContentHashOnto(this.CameraShake.ShakeAngle.Value, ref lastHash);
                }
                HashUtils.ContentHashOnto(this.CameraShake.ShakeStepFrame, ref lastHash);
                HashUtils.ContentHashOnto(this.CameraShake.ClearPreviousShake, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IsAnimEventAttack, ref lastHash);
            HashUtils.ContentHashOnto(this.IsInComboCount, ref lastHash);
            HashUtils.ContentHashOnto((int) this.HitLevel, ref lastHash);
            HashUtils.ContentHashOnto((int) base.Target, ref lastHash);
            if ((base.TargetOption != null) && (base.TargetOption.Range != null))
            {
                HashUtils.ContentHashOnto(base.TargetOption.Range.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.TargetOption.Range.dynamicKey, ref lastHash);
            }
            if (base.Predicates != null)
            {
                foreach (ConfigAbilityPredicate predicate in base.Predicates)
                {
                    if (predicate is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) predicate, ref lastHash);
                    }
                }
            }
        }
    }
}

