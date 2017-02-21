namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class SuddenTeleportMixin : ConfigAbilityMixin, IHashable
    {
        public DynamicFloat Angle = DynamicFloat.ZERO;
        public TeleportDirectionMode DirectionMode = TeleportDirectionMode.FromTarget;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilitySuddenTeleportMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            HashUtils.ContentHashOnto((int) this.DirectionMode, ref lastHash);
            if (this.Angle != null)
            {
                HashUtils.ContentHashOnto(this.Angle.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.Angle.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.Angle.dynamicKey, ref lastHash);
            }
        }
    }
}

