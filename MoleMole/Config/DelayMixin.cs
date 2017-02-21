namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class DelayMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Delay;
        public ConfigAbilityAction[] OnTimeUp = ConfigAbilityAction.EMPTY;

        public DelayMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 60f
            };
            this.Delay = num;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityDelayMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.OnTimeUp != null)
            {
                foreach (ConfigAbilityAction action in this.OnTimeUp)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.Delay != null)
            {
                HashUtils.ContentHashOnto(this.Delay.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Delay.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Delay.dynamicKey, ref lastHash);
            }
        }
    }
}

