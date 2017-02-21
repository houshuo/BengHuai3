namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AnimatorParamLerpMixin : ConfigAbilityMixin, IHashable
    {
        public string AnimatorParamName;
        public float LerpEndNormalizedTime = 1f;
        public float LerpEndValue;
        public float LerpStartNormalizedTime;
        public float LerpStartValue = -1f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAnimatorParamLerpMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AnimatorParamName, ref lastHash);
            HashUtils.ContentHashOnto(this.LerpStartNormalizedTime, ref lastHash);
            HashUtils.ContentHashOnto(this.LerpEndNormalizedTime, ref lastHash);
            HashUtils.ContentHashOnto(this.LerpStartValue, ref lastHash);
            HashUtils.ContentHashOnto(this.LerpEndValue, ref lastHash);
        }
    }
}

