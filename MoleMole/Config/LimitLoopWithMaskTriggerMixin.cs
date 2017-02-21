namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class LimitLoopWithMaskTriggerMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicInt LoopLimitCount;
        public float MaskDuration;
        public string MaskTriggerID;
        public int OverCountResetLoopLimitCount;
        public float ResetOverCountTime;
        public string SkillID;
        public bool UseOverCount;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityLimitLoopWithMaskTriggerMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.MaskTriggerID, ref lastHash);
            HashUtils.ContentHashOnto(this.MaskDuration, ref lastHash);
            HashUtils.ContentHashOnto(this.SkillID, ref lastHash);
            if (this.LoopLimitCount != null)
            {
                HashUtils.ContentHashOnto(this.LoopLimitCount.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.LoopLimitCount.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.LoopLimitCount.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.UseOverCount, ref lastHash);
            HashUtils.ContentHashOnto(this.ResetOverCountTime, ref lastHash);
            HashUtils.ContentHashOnto(this.OverCountResetLoopLimitCount, ref lastHash);
        }
    }
}

