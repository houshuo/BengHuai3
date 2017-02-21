namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AttachModifierToAbilityStateMixin : ConfigAbilityMixin, IHashable
    {
        public AbilityState[] AbilityStates = AbilityData.EMPTY;
        public string OffModifierName;
        public string OnModifierName;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAttachModifierToAbilityStateMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.AbilityStates != null)
            {
                foreach (AbilityState state in this.AbilityStates)
                {
                    HashUtils.ContentHashOnto((int) state, ref lastHash);
                }
            }
            HashUtils.ContentHashOnto(this.OnModifierName, ref lastHash);
            HashUtils.ContentHashOnto(this.OffModifierName, ref lastHash);
        }
    }
}

