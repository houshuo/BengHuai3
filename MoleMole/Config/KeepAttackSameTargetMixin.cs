namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class KeepAttackSameTargetMixin : ConfigAbilityMixin, IHashable
    {
        public ConfigAbilityAction[] OnAttackSameTarget = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] OnTargetFadeOrChanged = ConfigAbilityAction.EMPTY;
        public DynamicFloat TargetFadeWindow;

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityKeepAttackSameTargetMixin(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.TargetFadeWindow != null)
            {
                HashUtils.ContentHashOnto(this.TargetFadeWindow.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.TargetFadeWindow.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.TargetFadeWindow.dynamicKey, ref lastHash);
            }
            if (this.OnAttackSameTarget != null)
            {
                foreach (ConfigAbilityAction action in this.OnAttackSameTarget)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.OnTargetFadeOrChanged != null)
            {
                foreach (ConfigAbilityAction action2 in this.OnTargetFadeOrChanged)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
        }
    }
}

