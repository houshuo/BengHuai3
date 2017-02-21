namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarAutoUltraModeMixin : ConfigAbilityMixin, IHashable
    {
        public float AutoUltraSPRatio;
        public ConfigAbilityAction[] BeginActions = ConfigAbilityAction.EMPTY;
        public float CostSPSpeed;
        public ConfigAbilityAction[] EndActions = ConfigAbilityAction.EMPTY;
        public float EndUltarSPRatio;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarAutoUltraModeMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AutoUltraSPRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.EndUltarSPRatio, ref lastHash);
            HashUtils.ContentHashOnto(this.CostSPSpeed, ref lastHash);
            if (this.BeginActions != null)
            {
                foreach (ConfigAbilityAction action in this.BeginActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.EndActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.EndActions)
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

