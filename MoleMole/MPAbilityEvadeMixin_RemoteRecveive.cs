namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public class MPAbilityEvadeMixin_RemoteRecveive : AbilityEvadeMixin
    {
        private EvtEvadeSuccess _firstSuccess;
        private MixinArg_Evade _mixinArg;
        private PeerReceiveState[] _recvStates;

        public MPAbilityEvadeMixin_RemoteRecveive(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._mixinArg = new MixinArg_Evade();
            this._recvStates = new PeerReceiveState[Singleton<MPManager>.Instance.peer.totalPeerCount + 1];
        }

        public override void Core()
        {
            this.RecvCore();
        }

        public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
        {
            if (base.selfIdentity.isAuthority)
            {
                this._mixinArg = invokeEntry.GetArgument<MixinArg_Evade>(this._mixinArg);
                if ((this._mixinArg.Action == EvadeAction.FailEvade) && (this._recvStates[fromPeerID] == PeerReceiveState.Started))
                {
                    this._recvStates[fromPeerID] = PeerReceiveState.Failed;
                    this.OnAuthorityPeerEvadeStateChanged();
                }
            }
            else
            {
                this._mixinArg = invokeEntry.GetArgument<MixinArg_Evade>(this._mixinArg);
                if (this._mixinArg.Action == EvadeAction.StartEvade)
                {
                    this.RecvStart();
                }
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.RecvStart();
                base.StartRecordMixinInvokeEntry(out context, 0);
                Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.StartEvade);
                context.Finish<MixinArg_Evade>(offset, AbilityInvokeArgument.MixinArg_Evade);
                for (int i = 1; i < this._recvStates.Length; i++)
                {
                    this._recvStates[i] = PeerReceiveState.Started;
                }
                this._firstSuccess = null;
            }
        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        private void OnAuthorityPeerEvadeStateChanged()
        {
            for (int i = 1; i < this._recvStates.Length; i++)
            {
                if (this._recvStates[i] == PeerReceiveState.Started)
                {
                    return;
                }
            }
            if (this._firstSuccess != null)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(base.config.EvadeSuccessActions, base.instancedAbility, base.instancedModifier, Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(this._firstSuccess.attackerID), this._firstSuccess);
            }
            else
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(base.config.EvadeFailActions, base.instancedAbility, base.instancedModifier, null, null);
            }
        }

        protected override bool OnEvadeSuccess(EvtEvadeSuccess evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                this._recvStates[evt.fromPeerID] = PeerReceiveState.Successed;
                if (this._firstSuccess == null)
                {
                    this._firstSuccess = evt;
                }
                Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
                this.OnAuthorityPeerEvadeStateChanged();
            }
            return true;
        }

        private void RecvCore()
        {
            if (base._state == AbilityEvadeMixin.State.Evading)
            {
                base._evadeTimer.Core(1f);
                if (base._evadeTimer.isTimeUp)
                {
                    if (base.selfIdentity.isAuthority)
                    {
                        if (this._recvStates[base.selfIdentity.authorityPeerID] == PeerReceiveState.Started)
                        {
                            this._recvStates[base.selfIdentity.authorityPeerID] = PeerReceiveState.Failed;
                            this.OnAuthorityPeerEvadeStateChanged();
                        }
                    }
                    else
                    {
                        RecordInvokeEntryContext context;
                        base.StartRecordMixinInvokeEntry(out context, 0);
                        Offset<MixinArg_Evade> offset = MixinArg_Evade.CreateMixinArg_Evade(context.builder, EvadeAction.FailEvade);
                        context.Finish<MixinArg_Evade>(offset, AbilityInvokeArgument.MixinArg_Evade);
                    }
                    base._evadeTimer.Reset(false);
                    base._dummyActor.Kill();
                    base.actor.RemoveAbilityState(AbilityState.BlockAnimEventAttack);
                    base.entity.SetCountedIsGhost(false);
                    base._state = AbilityEvadeMixin.State.Idle;
                }
            }
        }

        private void RecvStart()
        {
            if ((base._dummyActor != null) && base._dummyActor.IsEntityExists())
            {
                base._dummyActor.Kill();
            }
            uint runtimeID = Singleton<DynamicObjectManager>.Instance.CreateEvadeDummy(base.actor.runtimeID, base.config.EvadeDummyName, base.actor.entity.XZPosition, base.actor.entity.transform.forward);
            base._dummyActor = Singleton<EventManager>.Instance.GetActor<EvadeEntityDummy>(runtimeID);
            if (base.selfIdentity.isAuthority)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(base.config.EvadeStartActions, base.instancedAbility, base.instancedModifier, null, null);
            }
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

        private enum PeerReceiveState
        {
            Started,
            Successed,
            Failed
        }
    }
}

