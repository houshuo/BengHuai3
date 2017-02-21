namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterSuicideAttackMixin : ConfigAbilityMixin, IHashable
    {
        public bool IsTouchExplode;
        public MixinEffect KilledEffect;
        public string KilledHitAlliedAnimEventID;
        public string KilledHitAnimEventID;
        public string OnTouchTriggerID;
        public string SuicicdeModifierName;
        public DynamicFloat SuicideCountDownDuration;
        public MixinEffect SuicideEffect;
        public string SuicideHitAlliedAnimEventID;
        public string SuicideHitAnimEventID;
        public string WarningAudioPattern;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterSuicideAttack(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SuicideHitAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(this.SuicideHitAlliedAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(this.KilledHitAnimEventID, ref lastHash);
            HashUtils.ContentHashOnto(this.KilledHitAlliedAnimEventID, ref lastHash);
            if (this.SuicideEffect != null)
            {
                HashUtils.ContentHashOnto(this.SuicideEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.SuicideEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IsTouchExplode, ref lastHash);
            HashUtils.ContentHashOnto(this.OnTouchTriggerID, ref lastHash);
            HashUtils.ContentHashOnto(this.WarningAudioPattern, ref lastHash);
            if (this.KilledEffect != null)
            {
                HashUtils.ContentHashOnto(this.KilledEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.KilledEffect.AudioPattern, ref lastHash);
            }
            if (this.SuicideCountDownDuration != null)
            {
                HashUtils.ContentHashOnto(this.SuicideCountDownDuration.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.SuicideCountDownDuration.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.SuicideCountDownDuration.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.SuicicdeModifierName, ref lastHash);
        }
    }
}

