namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterSummonMixin : ConfigAbilityMixin, IHashable
    {
        public MixinEffect SummonEffect;
        public MixinSummonItem[] SummonMonsters;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterSummonAttack(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.SummonMonsters != null)
            {
                foreach (MixinSummonItem item in this.SummonMonsters)
                {
                    if (item.MonsterName != null)
                    {
                        HashUtils.ContentHashOnto(item.MonsterName.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(item.MonsterName.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(item.MonsterName.dynamicKey, ref lastHash);
                    }
                    if (item.TypeName != null)
                    {
                        HashUtils.ContentHashOnto(item.TypeName.isDynamic, ref lastHash);
                        HashUtils.ContentHashOnto(item.TypeName.fixedValue, ref lastHash);
                        HashUtils.ContentHashOnto(item.TypeName.dynamicKey, ref lastHash);
                    }
                    HashUtils.ContentHashOnto(item.BaseOnTarget, ref lastHash);
                    HashUtils.ContentHashOnto(item.Distance, ref lastHash);
                    HashUtils.ContentHashOnto(item.Angle, ref lastHash);
                    HashUtils.ContentHashOnto(item.EffectDelay, ref lastHash);
                    HashUtils.ContentHashOnto(item.SummonDelay, ref lastHash);
                    HashUtils.ContentHashOnto(item.UseCoffinAnim, ref lastHash);
                    HashUtils.ContentHashOnto(item.CoffinIndex, ref lastHash);
                    if (item.Abilities != null)
                    {
                        foreach (KeyValuePair<string, ConfigEntityAbilityEntry> pair in item.Abilities)
                        {
                            HashUtils.ContentHashOnto(pair.Key, ref lastHash);
                            HashUtils.ContentHashOnto(pair.Value.AbilityName, ref lastHash);
                            HashUtils.ContentHashOnto(pair.Value.AbilityOverride, ref lastHash);
                        }
                    }
                }
            }
            if (this.SummonEffect != null)
            {
                HashUtils.ContentHashOnto(this.SummonEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.SummonEffect.AudioPattern, ref lastHash);
            }
        }
    }
}

