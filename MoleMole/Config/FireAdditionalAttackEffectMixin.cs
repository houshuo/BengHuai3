namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class FireAdditionalAttackEffectMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigEntityAttackEffect AttackEffect;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityFireAdditionalAttackEffectMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AttackEffect != null)
            {
                HashUtils.ContentHashOnto(this.AttackEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.SwitchName, ref lastHash);
                HashUtils.ContentHashOnto(this.AttackEffect.MuteAttackEffect, ref lastHash);
                HashUtils.ContentHashOnto((int) this.AttackEffect.AttackEffectTriggerPos, ref lastHash);
            }
        }
    }
}

