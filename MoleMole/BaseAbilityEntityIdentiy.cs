namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public abstract class BaseAbilityEntityIdentiy : BaseMPIdentity
    {
        protected BaseAbilityActor _abilityActor;
        protected BaseMonoAbilityEntity _abilityEntity;
        protected MPActorAbilityPlugin _mpAbilityPlugin;

        protected BaseAbilityEntityIdentiy()
        {
        }

        public void Command_TryApplyModifier(uint casterID, int instancedAbilityID, int modifierLocalID)
        {
            if (base.isAuthority)
            {
                this._mpAbilityPlugin.MPTryApplyModifierByID(casterID, instancedAbilityID, modifierLocalID);
            }
            else
            {
                RecordInvokeEntryContext context;
                this._mpAbilityPlugin.StartRecordInvokeEntry(instancedAbilityID, 0, casterID, 0xff, out context);
                Offset<MetaArg_Command_ModifierChangeRequest> offset = MetaArg_Command_ModifierChangeRequest.CreateMetaArg_Command_ModifierChangeRequest(context.builder, ModifierAction.Added, (byte) modifierLocalID);
                context.Finish<MetaArg_Command_ModifierChangeRequest>(offset, AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest);
            }
        }

        public void Command_TryRemoveModifier(uint casterID, int instancedAbilityID, int modifierLocalID)
        {
            if (base.isAuthority)
            {
                this._mpAbilityPlugin.MPTryRemoveModifierByID(casterID, instancedAbilityID, modifierLocalID);
            }
            else
            {
                RecordInvokeEntryContext context;
                this._mpAbilityPlugin.StartRecordInvokeEntry(instancedAbilityID, 0, casterID, 0xff, out context);
                Offset<MetaArg_Command_ModifierChangeRequest> offset = MetaArg_Command_ModifierChangeRequest.CreateMetaArg_Command_ModifierChangeRequest(context.builder, ModifierAction.Removed, (byte) modifierLocalID);
                context.Finish<MetaArg_Command_ModifierChangeRequest>(offset, AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest);
            }
        }

        public override void Init()
        {
            this._mpAbilityPlugin = this._abilityActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>();
        }

        private void OnAbilityInvokeTable(MPRecvPacketContainer pc)
        {
            this._mpAbilityPlugin.HandleInvokes(pc.As<Packet_Ability_InvocationTable>(), pc.fromPeerID);
        }

        private void OnAuthorityJustKilled(uint killerID, string animEventID, KillEffect killEffect)
        {
            MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Entity_Kill>();
            StringOffset animEventIDOffset = MPMiscs.CreateString(pc.builder, animEventID);
            pc.Finish<Packet_Entity_Kill>(Packet_Entity_Kill.CreatePacket_Entity_Kill(pc.builder, killerID, animEventIDOffset, (byte) killEffect));
            Singleton<MPManager>.Instance.SendReliableToOthers(base.runtimeID, pc);
        }

        protected virtual void OnAuthorityReliablePacket(MPRecvPacketContainer pc)
        {
            if (pc.packet is Packet_Ability_InvocationTable)
            {
                this.OnAbilityInvokeTable(pc);
            }
        }

        public override void OnAuthorityStart()
        {
            base.OnAuthorityStart();
            this._abilityActor.onJustKilled = (Action<uint, string, KillEffect>) Delegate.Combine(this._abilityActor.onJustKilled, new Action<uint, string, KillEffect>(this.OnAuthorityJustKilled));
        }

        protected virtual void OnAuthorityStateUpdate(MPRecvPacketContainer pc)
        {
        }

        public sealed override void OnReliablePacket(MPRecvPacketContainer pc)
        {
            if (base.isAuthority)
            {
                this.OnAuthorityReliablePacket(pc);
            }
            else
            {
                this.OnRemoteReliablePacket(pc);
            }
        }

        protected virtual void OnRemoteReliablePacket(MPRecvPacketContainer pc)
        {
            if (pc.packet is Packet_Ability_InvocationTable)
            {
                this.OnAbilityInvokeTable(pc);
            }
        }

        protected virtual void OnRemoteStateUpdate(MPRecvPacketContainer pc)
        {
        }

        public sealed override void OnStateUpdatePacket(MPRecvPacketContainer pc)
        {
            if (base.isAuthority)
            {
                this.OnAuthorityStateUpdate(pc);
            }
            else
            {
                this.OnRemoteStateUpdate(pc);
            }
        }

        public override IdentityRemoteMode remoteMode
        {
            get
            {
                return this._abilityEntity.commonConfig.MPArguments.RemoteMode;
            }
        }
    }
}

