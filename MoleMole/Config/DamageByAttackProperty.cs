namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DamageByAttackProperty : ConfigAbilityAction, IHashable
    {
        public ConfigEntityAttackEffect AttackEffect;
        public ConfigEntityAttackProperty AttackProperty;
        public ConfigEntityCameraShake CameraShake;

        public override void Call(ActorAbilityPlugin abilityPlugin, ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            abilityPlugin.DamageByAttackPropertyHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public override bool GetDebugOutput(ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref string output)
        {
            output = string.Format("{0} 对 {1} 开始通过技能造成攻击 AttackProperty {2}", Miscs.GetDebugActorName(instancedAbility.caster), Miscs.GetDebugActorName(target), this.AttackProperty.GetDebugOutput());
            return true;
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AttackProperty != null)
            {
                HashUtils.ContentHashOnto(this.AttackProperty.DamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.AddedDamageValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.NormalDamage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.NormalDamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.FireDamage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.FireDamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.ThunderDamage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.ThunderDamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.IceDamage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.IceDamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.AlienDamage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.AlienDamagePercentage, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.AniDamageRatio, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackProperty.HitType, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackProperty.HitEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackProperty.HitEffectAux, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackProperty.KillEffect, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.FrameHalt, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.RetreatVelocity, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.IsAnimEventAttack, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.IsInComboCount, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.SPRecover, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.WitchTimeRatio, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.NoTriggerEvadeAndDefend, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackProperty.NoBreakFrameHaltAdd, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackProperty.AttackTargetting, ref lastHash);
                if (this.AttackProperty.CategoryTag != null)
                {
                    foreach (AttackResult.AttackCategoryTag tag in this.AttackProperty.CategoryTag)
                    {
                        HashUtils.ContentHashOnto((int) tag, ref lastHash);
                    }
                }
            }
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

