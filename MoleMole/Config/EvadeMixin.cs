namespace MoleMole.Config
{
    using MoleMole;
    using System;

    [GeneratePartialHash(CombineGeneratedFile=true)]
    public class EvadeMixin : ConfigAbilityMixin, IHashable
    {
        private static DynamicFloat DEFAULT_EVADE_EXTEND_INVIN;
        public string EvadeDummyName;
        public ConfigAbilityAction[] EvadeFailActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] EvadeStartActions = ConfigAbilityAction.EMPTY;
        public ConfigAbilityAction[] EvadeSuccessActions = ConfigAbilityAction.EMPTY;
        public DynamicFloat EvadeSuccessExtendedInvincibleWindow = DEFAULT_EVADE_EXTEND_INVIN;
        public DynamicFloat EvadeWindow;

        static EvadeMixin()
        {
            DynamicFloat num = new DynamicFloat {
                fixedValue = 0.4f
            };
            DEFAULT_EVADE_EXTEND_INVIN = num;
        }

        public EvadeMixin()
        {
            base.isUnique = true;
        }

        public override BaseAbilityMixin CreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            return new AbilityEvadeMixin(instancedAbility, instancedModifier, this);
        }

        public override ConfigAbilityAction[][] GetAllSubActions()
        {
            return new ConfigAbilityAction[][] { this.EvadeStartActions, this.EvadeSuccessActions, this.EvadeFailActions };
        }

        public override BaseAbilityMixin MPCreateInstancedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier)
        {
            BaseAbilityActor actor = (instancedModifier == null) ? instancedAbility.caster : instancedModifier.owner;
            if (Singleton<MPManager>.Instance.GetIdentity<BaseMPIdentity>(actor.runtimeID).remoteMode.IsRemoteReceive())
            {
                return new MPAbilityEvadeMixin_RemoteRecveive(instancedAbility, instancedModifier, this);
            }
            return new MPAbilityEvadeMixin_RemoteNoRecveive(instancedAbility, instancedModifier, this);
        }

        public void ObjectContentHashOnto(ref int lastHash)
        {
            if (this.EvadeWindow != null)
            {
                HashUtils.ContentHashOnto(this.EvadeWindow.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.EvadeWindow.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.EvadeWindow.dynamicKey, ref lastHash);
            }
            if (this.EvadeSuccessExtendedInvincibleWindow != null)
            {
                HashUtils.ContentHashOnto(this.EvadeSuccessExtendedInvincibleWindow.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.EvadeSuccessExtendedInvincibleWindow.fixedValue, ref lastHash);
                HashUtils.ContentHashOnto(this.EvadeSuccessExtendedInvincibleWindow.dynamicKey, ref lastHash);
            }
            HashUtils.ContentHashOnto(this.EvadeDummyName, ref lastHash);
            if (this.EvadeStartActions != null)
            {
                foreach (ConfigAbilityAction action in this.EvadeStartActions)
                {
                    if (action is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action, ref lastHash);
                    }
                }
            }
            if (this.EvadeSuccessActions != null)
            {
                foreach (ConfigAbilityAction action2 in this.EvadeSuccessActions)
                {
                    if (action2 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action2, ref lastHash);
                    }
                }
            }
            if (this.EvadeFailActions != null)
            {
                foreach (ConfigAbilityAction action3 in this.EvadeFailActions)
                {
                    if (action3 is IHashable)
                    {
                        HashUtils.ContentHashOnto((IHashable) action3, ref lastHash);
                    }
                }
            }
        }
    }
}

