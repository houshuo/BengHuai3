namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByHitDirection : ConfigAbilityPredicate, IHashable
    {
        public float Angle;
        public bool ReverseAngle;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByHitDirectionHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.Angle, ref lastHash);
            HashUtils.ContentHashOnto(this.ReverseAngle, ref lastHash);
        }
    }
}

