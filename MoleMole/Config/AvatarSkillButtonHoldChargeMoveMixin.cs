namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class AvatarSkillButtonHoldChargeMoveMixin : ConfigAbilityMixin, IHashable
    {
        public string[] ChargeLoopSkillIDs;
        public bool IsSteer;
        public DynamicFloat MoveSpeed = DynamicFloat.ONE;
        public float SteerSpeed = 1f;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityAvatarSkillButtonChargeMoveMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.ChargeLoopSkillIDs != null)
            {
                foreach (string str in this.ChargeLoopSkillIDs)
                {
                    HashUtils.ContentHashOnto(str, ref lastHash);
                }
            }
            if (this.MoveSpeed != null)
            {
                HashUtils.ContentHashOnto(this.MoveSpeed.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.MoveSpeed.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.MoveSpeed.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.IsSteer, ref lastHash);
            HashUtils.ContentHashOnto(this.SteerSpeed, ref lastHash);
        }
    }
}

