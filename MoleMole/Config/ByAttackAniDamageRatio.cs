namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByAttackAniDamageRatio : ConfigAbilityPredicate, IHashable
    {
        public float AniDamageRatio;
        public LogicType CompareType;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByAttackAniDamageRatioHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AniDamageRatio, ref lastHash);
            HashUtils.ContentHashOnto((int) this.CompareType, ref lastHash);
        }

        public enum LogicType
        {
            MoreThan,
            LessThan
        }
    }
}

