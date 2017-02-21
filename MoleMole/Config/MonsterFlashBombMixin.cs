namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class MonsterFlashBombMixin : ConfigAbilityMixin, IHashable
    {
        public float Angle = 120f;
        public float DelayTime;
        public ConfigAbilityAction[] FailActions = ConfigAbilityAction.EMPTY;
        public string[] ModifierNames;
        public ConfigAbilityAction[] SuccessActions = ConfigAbilityAction.EMPTY;
        public MixinEffect TriggerEffect;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityMonsterFlashBombMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.Angle, ref lastHash);
            HashUtils.ContentHashOnto(this.DelayTime, ref lastHash);
            if (this.TriggerEffect != null)
            {
                HashUtils.ContentHashOnto(this.TriggerEffect.EffectPattern, ref lastHash);
                HashUtils.ContentHashOnto(this.TriggerEffect.AudioPattern, ref lastHash);
            }
            if (this.ModifierNames != null)
            {
                foreach (string str in this.ModifierNames)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.SuccessActions != null)
            {
                foreach (ConfigAbilityAction action in this.SuccessActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.FailActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.FailActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
        }
    }
}

