namespace MoleMole
{
    using FlatBuffers;
    using System;

    public class MPEventManager : EventManager
    {
        private FlatBufferBuilder _eventBuilder = new FlatBufferBuilder(0x200);

        protected MPEventManager()
        {
        }

        private MPDispatchBehavior CheckForDispatchBehavior(BaseEvent evt)
        {
            System.Type item = evt.GetType();
            if (!MPData.ReplicatedEventTypes.Contains(item))
            {
                return MPDispatchBehavior.NormalDispatch;
            }
            if (evt.remoteState == EventRemoteState.IsAutorityReceiveRedirected)
            {
                return MPDispatchBehavior.NormalDispatch;
            }
            if (evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated)
            {
                return MPDispatchBehavior.NormalDispatch;
            }
            bool flag = false;
            if (evt.remoteState == EventRemoteState.Idle)
            {
                flag = true;
            }
            else if (evt.remoteState == EventRemoteState.NeedCheckForRemote)
            {
                IEvtWithRemoteID eid = (IEvtWithRemoteID) evt;
                uint senderID = eid.GetSenderID();
                uint remoteID = eid.GetRemoteID();
                BaseMPIdentity identity = this.ResolveRemoteModeIdentity(senderID);
                BaseMPIdentity identity2 = this.ResolveRemoteModeIdentity(remoteID);
                if ((identity2 == null) || (identity == null))
                {
                    return MPDispatchBehavior.RemoteModeDropped;
                }
                bool flag2 = identity.isAuthority || ((identity.remoteMode == IdentityRemoteMode.SendAndNoReceive) || (identity.remoteMode == IdentityRemoteMode.SendAndReceive));
                bool flag3 = identity2.isAuthority || ((identity2.remoteMode == IdentityRemoteMode.ReceiveAndNoSend) || (identity2.remoteMode == IdentityRemoteMode.SendAndReceive));
                flag = flag2 && flag3;
            }
            if (!Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(evt.targetID))
            {
                return MPDispatchBehavior.NormalDispatch;
            }
            BaseMPIdentity identity3 = Singleton<MPManager>.Instance.TryGetIdentity(evt.targetID);
            if (!flag || (identity3 == null))
            {
                return MPDispatchBehavior.RemoteModeDropped;
            }
            if (identity3.isAuthority)
            {
                return MPDispatchBehavior.NormalDispatch;
            }
            return MPDispatchBehavior.RemoteModeDirectToAuthority;
        }

        protected override void DispatchEvent(BaseEvent evt)
        {
            switch (this.CheckForDispatchBehavior(evt))
            {
                case MPDispatchBehavior.NormalDispatch:
                    base.DispatchEvent(evt);
                    break;

                case MPDispatchBehavior.RemoteModeDropped:
                    return;

                case MPDispatchBehavior.RemoteModeDirectToAuthority:
                    this.RedirecEventToAuthority(evt);
                    return;
            }
        }

        protected override void DispatchListenEvent(BaseEvent evt)
        {
            if (evt.remoteState != EventRemoteState.IsRedirected)
            {
                base.DispatchListenEvent(evt);
                if ((evt.remoteState == EventRemoteState.NeedToReplicateToRemote) && Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(evt.targetID))
                {
                    this.ReplicateResolvedEventToOthers(evt);
                }
            }
        }

        public override void FireEvent(BaseEvent evt, MPEventDispatchMode mode)
        {
            evt.remoteState = (mode != MPEventDispatchMode.CheckRemoteMode) ? EventRemoteState.Idle : EventRemoteState.NeedCheckForRemote;
            if (mode == MPEventDispatchMode.CheckRemoteMode)
            {
                evt.fromPeerID = Singleton<MPManager>.Instance.peerID;
            }
            base.FireEvent(evt, mode);
        }

        public void InjectReplicatedEvent(BaseEvent evt)
        {
            base.InjectEvent(evt);
        }

        public bool IsIdentityAuthority(uint runtimeID)
        {
            BaseMPIdentity identity = Singleton<MPManager>.Instance.TryGetIdentity(runtimeID);
            return ((identity != null) ? identity.isAuthority : false);
        }

        public void MarkEventReplicate(BaseEvent evt)
        {
            evt.remoteState = EventRemoteState.NeedToReplicateToRemote;
        }

        protected override void ProcessInitedActor(BaseActor actor)
        {
            if (actor is MonsterActor)
            {
                ((MonsterActor) actor).AddPlugin(new MPMonsterActorPlugin(actor));
            }
            else if (actor is AvatarActor)
            {
                ((AvatarActor) actor).AddPlugin(new MPAvatarActorPlugin(actor));
            }
        }

        private void RedirecEventToAuthority(BaseEvent evt)
        {
            BaseMPIdentity identity = Singleton<MPManager>.Instance.TryGetIdentity(evt.targetID);
            evt.remoteState = EventRemoteState.IsRedirected;
            this.RedirectEvent((IEvtWithRemoteID) evt, identity.authorityPeerID);
        }

        private void RedirectEvent(IEvtWithRemoteID evt, int peerID)
        {
            System.Type type = MPMappings.SerializeToProtocol(this._eventBuilder, evt);
            MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket(type, this._eventBuilder);
            pc.state = MPSendContainerState.Finished;
            Singleton<MPManager>.Instance.SendReliableToPeer(evt.GetChannelID(), peerID, pc);
        }

        private void ReplicateResolvedEventToOthers(BaseEvent evt)
        {
            this.RedirectEvent((IEvtWithRemoteID) evt, 7);
        }

        private BaseMPIdentity ResolveRemoteModeIdentity(uint runtimeID)
        {
            if (Singleton<RuntimeIDManager>.Instance.IsSyncedRuntimeID(runtimeID))
            {
                return Singleton<MPManager>.Instance.TryGetIdentity(runtimeID);
            }
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(runtimeID) == 6)
            {
                BaseMonoDynamicObject dynamicObjectByRuntimeID = Singleton<DynamicObjectManager>.Instance.GetDynamicObjectByRuntimeID(runtimeID);
                if (dynamicObjectByRuntimeID.owner != null)
                {
                    return Singleton<MPManager>.Instance.TryGetIdentity(dynamicObjectByRuntimeID.owner.GetRuntimeID());
                }
            }
            return null;
        }

        private enum MPDispatchBehavior
        {
            NormalDispatch,
            RemoteModeDropped,
            RemoteModeDirectToAuthority
        }
    }
}

