namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class ByAttackAnimEventID : ConfigAbilityPredicate, IHashable
    {
        public string[] AnimEventIDs = Miscs.EMPTY_STRINGS;
        public bool ByAnyEventID;

        public override bool Call(ActorAbilityPlugin abilityPlugin, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            return abilityPlugin.ByAttackAnimEventIDHandler(this, instancedAbility, instancedModifier, target, evt);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AnimEventIDs != null)
            {
                foreach (string str in this.AnimEventIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.ByAnyEventID, ref lastHash);
        }
    }
}

