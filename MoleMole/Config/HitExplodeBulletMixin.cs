namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class HitExplodeBulletMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat AliveDuration;
        public bool ApplyDistinctHitExplodeEffectPattern;
        public MixinEffect BulletEffect;
        public bool BulletEffectGround;
        public BulletHitBehavior BulletHitType;
        public DynamicFloat BulletSpeed;
        public string BulletTypeName;
        public DynamicFloat DistinctHitExplodeHeight;
        public bool ExplodeEffectGround;
        public bool FaceTarget;
        public string HitAnimEventID;
        public ConfigAbilityAction[] HitExplodeActions;
        public MixinEffect HitExplodeEffect;
        public MixinEffect HitExplodeEffectAir;
        public MixinEffect HitExplodeEffectGround;
        public DynamicFloat HitExplodeRadius;
        public bool IgnoreTimeScale;
        public bool IsFixedHeight;
        public bool IsHitChangeTargetDirection;
        public bool MuteSelfHitExplodeActions;
        public BulletClearBehavior RemoveClearType;
        public float ResetTime;
        public MixinEffect SelfExplodeEffect;
        public MixinTargetting Targetting = MixinTargetting.Enemy;

        public HitExplodeBulletMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = -1f
            };
            this.AliveDuration = num;
            this.DistinctHitExplodeHeight = DynamicFloat.ONE;
            this.BulletEffectGround = true;
            this.ExplodeEffectGround = true;
            this.ResetTime = 0.4f;
            this.HitExplodeActions = ConfigAbilityAction.EMPTY;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityHitExplodeBulletMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.HitExplodeActions };
        }

        public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new MPAbilityHitExplodeBulletMixin_TotallyLocal(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.BulletTypeName, ref lastHash);
            HashUtils.ContentHashOnto((int) this.Targetting, ref lastHash);
            if (this.BulletSpeed != null)
            {
                HashUtils.ContentHashOnto(this.BulletSpeed.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BulletSpeed.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BulletSpeed.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IgnoreTimeScale, ref lastHash);
            if (this.AliveDuration != null)
            {
                HashUtils.ContentHashOnto(this.AliveDuration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.AliveDuration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.AliveDuration.dynamicKey, ref lastHash);
            }
            if (this.HitExplodeRadius != null)
            {
                HashUtils.ContentHashOnto(this.HitExplodeRadius.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.HitExplodeRadius.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.HitExplodeRadius.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IsFixedHeight, ref lastHash);
            if (this.BulletEffect != null)
            {
                HashUtils.ContentHashOnto(this.BulletEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.BulletEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.ApplyDistinctHitExplodeEffectPattern, ref lastHash);
            if (this.DistinctHitExplodeHeight != null)
            {
                HashUtils.ContentHashOnto(this.DistinctHitExplodeHeight.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.DistinctHitExplodeHeight.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.DistinctHitExplodeHeight.dynamicKey, ref lastHash);
            }
            if (this.HitExplodeEffect != null)
            {
                HashUtils.ContentHashOnto(this.HitExplodeEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.HitExplodeEffect.AudioPattern, ref lastHash);
            }
            if (this.HitExplodeEffectAir != null)
            {
                HashUtils.ContentHashOnto(this.HitExplodeEffectAir.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.HitExplodeEffectAir.AudioPattern, ref lastHash);
            }
            if (this.HitExplodeEffectGround != null)
            {
                HashUtils.ContentHashOnto(this.HitExplodeEffectGround.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.HitExplodeEffectGround.AudioPattern, ref lastHash);
            }
            if (this.SelfExplodeEffect != null)
            {
                HashUtils.ContentHashOnto(this.SelfExplodeEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.SelfExplodeEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.MuteSelfHitExplodeActions, ref lastHash);
            HashUtils.ContentHashOnto(this.IsHitChangeTargetDirection, ref lastHash);
            HashUtils.ContentHashOnto(this.HitAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(this.FaceTarget, ref lastHash);
            HashUtils.ContentHashOnto((int) this.RemoveClearType, ref lastHash);
            HashUtils.ContentHashOnto((int) this.BulletHitType, ref lastHash);
            HashUtils.ContentHashOnto(this.BulletEffectGround, ref lastHash);
            HashUtils.ContentHashOnto(this.ExplodeEffectGround, ref lastHash);
            HashUtils.ContentHashOnto(this.ResetTime, ref lastHash);
            if (this.HitExplodeActions != null)
            {
                foreach (ConfigAbilityAction action in this.HitExplodeActions)
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

