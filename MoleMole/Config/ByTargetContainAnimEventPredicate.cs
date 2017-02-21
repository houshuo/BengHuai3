namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByTargetContainAnimEventPredicate : ConfigAbilityPredicate, IHashable
    {
        public string AnimEventPredicate;
        public bool ForceByCaster;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByTargetContainAnimEventPredicateHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.AnimEventPredicate, ref lastHash);
            HashUtils.ContentHashOnto(this.ForceByCaster, ref lastHash);
        }
    }
}

