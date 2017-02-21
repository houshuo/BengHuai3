namespace MoleMole
{
    using System;

    public class MPManager : BaseMPManager
    {
        public Action OnFrameEnd;
        public Action OnFrameStart;

        public MPManager()
        {
            IndexedConfig<ConfigEntityCameraShake>.InitializeMapping();
            IndexedConfig<ConfigEntityAttackEffect>.InitializeMapping();
            IndexedConfig<HitExplodeTracingBulletMixinArgument>.InitializeMapping();
        }

        public override void Core()
        {
            if (this.OnFrameStart != null)
            {
                this.OnFrameStart();
            }
            base.Core();
        }

        protected override void DispatchPacket(MPRecvPacketContainer pc)
        {
            if (MPData.ReplicatedEventWireTypes.Contains(pc.packet.GetType()))
            {
                this.RedirectventDispatch(pc);
            }
            else
            {
                base.DispatchPacket(pc);
            }
        }

        public override void PostCore()
        {
            base.PostCore();
            if (this.OnFrameEnd != null)
            {
                this.OnFrameEnd();
            }
        }

        public void RedirectventDispatch(MPRecvPacketContainer pc)
        {
            BaseEvent evt = (BaseEvent) MPMappings.DeserializeToObject(pc.packet, null);
            if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(pc.runtimeID))
            {
                evt.remoteState = EventRemoteState.IsAutorityReceiveRedirected;
            }
            else
            {
                evt.remoteState = EventRemoteState.IsRemoteReceiveHandledReplcated;
            }
            evt.fromPeerID = pc.fromPeerID;
            Singleton<MPEventManager>.Instance.InjectReplicatedEvent(evt);
        }

        public void Setup(MPPeer peer)
        {
            base.SetupPeer(peer, peer.peerID == 1);
            Singleton<RuntimeIDManager>.Instance.SetupPeerID(base.peerID);
            Singleton<MPManager>.Instance.RegisterIdentity(0x21800001, 0, new LevelIdentity());
        }

        public MPPeer peer
        {
            get
            {
                return base._peer;
            }
        }
    }
}

