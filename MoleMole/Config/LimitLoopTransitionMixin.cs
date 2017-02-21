namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class LimitLoopTransitionMixin : ConfigAbilityMixin, IHashable
    {
        public string AllowLoopBoolID;
        public DynamicInt LoopLimitCount;
        public string SkillID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityLimitLoopTransitionMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AllowLoopBoolID, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            if (this.LoopLimitCount != null)
            {
                HashUtils.ContentHashOnto(this.LoopLimitCount.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.LoopLimitCount.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.LoopLimitCount.dynamicKey, ref lastHash);
            }
        }
    }
}

