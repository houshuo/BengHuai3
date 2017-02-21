namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByTargetWithinAbilityState : ConfigAbilityPredicate, IHashable
    {
        public AbilityState TargetState;
        public AbilityState[] TargetStates;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByTargetWithinAbilityStateHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.TargetState, ref lastHash);
            if (this.TargetStates != null)
            {
                foreach (AbilityState state in this.TargetStates)
                {
                    HashUtils.ContentHashOnto((int) state, ref lastHash);
                }
            }
        }
    }
}

