namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByTargetEntityClass : ConfigAbilityPredicate, IHashable
    {
        public EntityClass TargetClass;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByTargetClassHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.TargetClass, ref lastHash);
        }
    }
}

