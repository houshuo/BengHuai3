namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByAttackDataType : ConfigAbilityPredicate, IHashable
    {
        public AttackDataType Type;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByAttackDataTypeHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.Type, ref lastHash);
        }

        public enum AttackDataType
        {
            None,
            Breakable,
            EvadeDefendable
        }
    }
}

