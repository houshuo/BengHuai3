namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterTeleportMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat BaselineDistance;
        public MixinEffect TeleportFromEffect;
        public float TeleportInverval = 0.2f;
        public MixinEffect TeleportToEffect;
        public bool towardsTarget = true;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterTeleportMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.BaselineDistance != null)
            {
                HashUtils.ContentHashOnto(this.BaselineDistance.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BaselineDistance.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.BaselineDistance.dynamicKey, ref lastHash);
            }
            if (this.TeleportFromEffect != null)
            {
                HashUtils.ContentHashOnto(this.TeleportFromEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.TeleportFromEffect.AudioPattern, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.TeleportInverval, ref lastHash);
            HashUtils.ContentHashOnto(this.towardsTarget, ref lastHash);
            if (this.TeleportToEffect != null)
            {
                HashUtils.ContentHashOnto(this.TeleportToEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.TeleportToEffect.AudioPattern, ref lastHash);
            }
        }
    }
}

