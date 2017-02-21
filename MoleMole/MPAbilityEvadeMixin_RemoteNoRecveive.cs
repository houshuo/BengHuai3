namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public class MPAbilityEvadeMixin_RemoteNoRecveive : AbilityEvadeMixin
    {
        private MixinArg_Evade _mixinArg;

        public MPAbilityEvadeMixin_RemoteNoRecveive(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._mixinArg = new MixinArg_Evade();
        }

        public override void Core()
        {
            if (base.selfIdentity.isAuthority)
            {
                base.Core();
            }
            else if (base._state == AbilityEvadeMixin.State.EvadeSuccessed)
            {
                base.Core();
            }
        }

        protected override void EvadeFail()
        {
            if (base.selfIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                base.StartRecordMixinInvokeEntry(out context, 0);
                Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.FailEvade);
                context.Finish<MixinArg_Evade>(offset, AbilityInvokeArgument.MixinArg_Evade);
                base.EvadeFail();
            }
        }

        public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
        {
            this._mixinArg = invokeEntry.GetArgument<MixinArg_Evade>(this._mixinArg);
            if (this._mixinArg.Action == EvadeAction.StartEvade)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtEvadeStart(base.actor.runtimeID), MPEventDispatchMode.Normal);
                if (base._state == AbilityEvadeMixin.State.Idle)
                {
                    base.actor.AddAbilityState(AbilityState.BlockAnimEventAttack, true);
                    base.entity.SetCountedIsGhost(true);
                }
                base._evadeTimer.Reset(true);
                base._extendedBlockAttackTimer.Reset(true);
                base._state = AbilityEvadeMixin.State.Evading;
            }
            else if (this._mixinArg.Action == EvadeAction.SuccessEvade)
            {
                base._state = AbilityEvadeMixin.State.EvadeSuccessed;
                base._extendedBlockAttackTimer.Reset(true);
            }
            else if (this._mixinArg.Action == EvadeAction.FailEvade)
            {
                base._evadeTimer.Reset(false);
                base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
                base.entity.SetCountedIsGhost(false);
                base._state = AbilityEvadeMixin.State.Idle;
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                base.OnAbilityTriggered(evt);
                base.StartRecordMixinInvokeEntry(out context, 0);
                Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.StartEvade);
                context.Finish<MixinArg_Evade>(offset, AbilityInvokeArgument.MixinArg_Evade);
            }
        }

        protected override bool OnEvadeSuccess(EvtEvadeSuccess evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                base.StartRecordMixinInvokeEntry(out context, 0);
                Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.SuccessEvade);
                context.Finish<MixinArg_Evade>(offset, AbilityInvokeArgument.MixinArg_Evade);
                Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
                return base.OnEvadeSuccess(evt);
            }
            return false;
        }
    }
}

