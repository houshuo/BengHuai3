namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class HitExplodeRandomBulletMixin : HitExplodeBulletMixin, IHashable
    {
        public float HoldTime;
        public float LifeTime = 10f;
        public float RandomPosX;
        public float RandomPosY;
        public float RandomPosZ;
        public float SteerCoef = 99f;
        public float TargetOffset;
        public float TraceSpeed;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityHitExplodeRandomBulletMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.RandomPosX, ref lastHash);
            HashUtils.ContentHashOnto(this.RandomPosY, ref lastHash);
            HashUtils.ContentHashOnto(this.RandomPosZ, ref lastHash);
            HashUtils.ContentHashOnto(this.TargetOffset, ref lastHash);
            HashUtils.ContentHashOnto(this.TraceSpeed, ref lastHash);
            HashUtils.ContentHashOnto(this.SteerCoef, ref lastHash);
            HashUtils.ContentHashOnto(this.HoldTime, ref lastHash);
            HashUtils.ContentHashOnto(this.LifeTime, ref lastHash);
            HashUtils.ContentHashOnto(base.BulletTypeName, ref lastHash);
            HashUtils.ContentHashOnto((int) base.Targetting, ref lastHash);
            if (base.BulletSpeed != null)
            {
                HashUtils.ContentHashOnto(base.BulletSpeed.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.BulletSpeed.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.BulletSpeed.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.IgnoreTimeScale, ref lastHash);
            if (base.AliveDuration != null)
            {
                HashUtils.ContentHashOnto(base.AliveDuration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.AliveDuration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.AliveDuration.dynamicKey, ref lastHash);
            }
            if (base.HitExplodeRadius != null)
            {
                HashUtils.ContentHashOnto(base.HitExplodeRadius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.HitExplodeRadius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.HitExplodeRadius.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.IsFixedHeight, ref lastHash);
            if (base.BulletEffect != null)
            {
                HashUtils.ContentHashOnto(base.BulletEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(base.BulletEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.ApplyDistinctHitExplodeEffectPattern, ref lastHash);
            if (base.DistinctHitExplodeHeight != null)
            {
                HashUtils.ContentHashOnto(base.DistinctHitExplodeHeight.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(base.DistinctHitExplodeHeight.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(base.DistinctHitExplodeHeight.dynamicKey, ref lastHash);
            }
            if (base.HitExplodeEffect != null)
            {
                HashUtils.ContentHashOnto(base.HitExplodeEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(base.HitExplodeEffect.AudioPattern, ref lastHash);
            }
            if (base.HitExplodeEffectAir != null)
            {
                HashUtils.ContentHashOnto(base.HitExplodeEffectAir.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(base.HitExplodeEffectAir.AudioPattern, ref lastHash);
            }
            if (base.HitExplodeEffectGround != null)
            {
                HashUtils.ContentHashOnto(base.HitExplodeEffectGround.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(base.HitExplodeEffectGround.AudioPattern, ref lastHash);
            }
            if (base.SelfExplodeEffect != null)
            {
                HashUtils.ContentHashOnto(base.SelfExplodeEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(base.SelfExplodeEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(base.MuteSelfHitExplodeActions, ref lastHash);
            HashUtils.ContentHashOnto(base.IsHitChangeTargetDirection, ref lastHash);
            HashUtils.ContentHashOnto(base.HitAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(base.FaceTarget, ref lastHash);
            HashUtils.ContentHashOnto((int) base.RemoveClearType, ref lastHash);
            HashUtils.ContentHashOnto((int) base.BulletHitType, ref lastHash);
            HashUtils.ContentHashOnto(base.BulletEffectGround, ref lastHash);
            HashUtils.ContentHashOnto(base.ExplodeEffectGround, ref lastHash);
            HashUtils.ContentHashOnto(base.ResetTime, ref lastHash);
            if (base.HitExplodeActions != null)
            {
                foreach (ConfigAbilityAction action in base.HitExplodeActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
        }
    }
}

