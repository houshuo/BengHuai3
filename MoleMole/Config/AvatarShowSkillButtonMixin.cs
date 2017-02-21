namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarShowSkillButtonMixin : ConfigAbilityMixin, IHashable
    {
        public string SkillButtonID;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarShowSkillButtonMixin(instancedAbility, instancedModifier, this);
        }

        public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            BaseAbilityActor actor = (instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner;
            if (Singleton<MPManager>.Instance.GetIdentity<BaseMPIdentity>(actor.runtimeID).isAuthority)
            {
                return new AbilityAvatarShowSkillButtonMixin(instancedAbility, instancedModifier, this);
            }
            return new MPAbilityStubMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto(this.SkillButtonID, ref lastHash);
        }
    }
}

