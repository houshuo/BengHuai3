namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByIsPlayerAvatar : ConfigAbilityPredicate, IHashable
    {
        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByIsPlayerAvatarHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
        }
    }
}

