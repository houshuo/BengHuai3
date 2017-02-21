namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByControlDataHasSteer : ConfigAbilityPredicate, IHashable
    {
        public bool HasSteer = true;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByControlDataHasSteerHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.HasSteer, ref lastHash);
        }
    }
}

