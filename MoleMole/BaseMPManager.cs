namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;

    public abstract class BaseMPManager
    {
        private SprotoPack _bitPacker = new SprotoPack();
        private bool[] _channelOccupiedMap;
        private FlatBufferBuilder _defaultSendBuilder;
        private Dictionary<uint, BaseMPIdentity> _identities;
        private BaseMPIdentity[] _identitiesBuffer = new BaseMPIdentity[0x40];
        private List<BaseMPIdentity> _identitiesList;
        private FlatBufferBuilder _instantiateBuilder;
        protected bool _isMaster;
        protected MPPeer _peer;
        private int _peerID;
        private Dictionary<uint, int> _pendingInstantiates;
        private byte[] _recvBuffer = new byte[0x400];
        private ByteBuffer _recvByteBuffer;
        private Dictionary<uint, int> _runtimeIDChannelMap;
        private byte[] _sendBuffer = new byte[0x400];
        private byte[] _tmpBuffer = new byte[0x400];
        private const int BUFFER_SIZE = 0x400;
        private const int TOTAL_HEADER_BYTE = 6;

        public BaseMPManager()
        {
            this._recvByteBuffer = new ByteBuffer(this._recvBuffer);
            this._defaultSendBuilder = new FlatBufferBuilder(0x400);
            this._instantiateBuilder = new FlatBufferBuilder(0x20);
            this._identities = new Dictionary<uint, BaseMPIdentity>();
            this._identitiesList = new List<BaseMPIdentity>();
            this._pendingInstantiates = new Dictionary<uint, int>();
            this._peer = IdleMPPeer.IDLE_PEER;
            MPMappings.InitMPMappings();
        }

        private int AllocChannelSequenceForRuntimeID(uint runtimeID)
        {
            if (this._runtimeIDChannelMap.ContainsKey(runtimeID))
            {
                return this._runtimeIDChannelMap[runtimeID];
            }
            int index = 0;
            while (index < (this._channelOccupiedMap.Length - 1))
            {
                if (!this._channelOccupiedMap[index])
                {
                    break;
                }
                index++;
            }
            if (index != (this._peer.channelSequenceCapacity - 1))
            {
                this._channelOccupiedMap[index] = true;
            }
            this._runtimeIDChannelMap.Add(runtimeID, index);
            return index;
        }

        public void BindImportantFixedChannel(uint runtimeID, int channel)
        {
            this._channelOccupiedMap[channel] = true;
            this._runtimeIDChannelMap[runtimeID] = channel;
        }

        public virtual void Core()
        {
            this._peer.Core();
            if (this._identitiesList.Count > this._identitiesBuffer.Length)
            {
                this._identitiesBuffer = new BaseMPIdentity[this._identitiesBuffer.Length * 2];
            }
            int count = this._identitiesList.Count;
            this._identitiesList.CopyTo(this._identitiesBuffer);
            for (int i = 0; i < count; i++)
            {
                this._identitiesBuffer[i].Core();
            }
        }

        private T CreateMPIdentity<T>(uint runtimeID) where T: BaseMPIdentity, new()
        {
            T identity = Activator.CreateInstance<T>();
            this.InitializeIdentity(identity, runtimeID);
            return identity;
        }

        private BaseMPIdentity CreateRemoteMPIdentity(System.Type type, uint runtimeID, MPRecvPacketContainer pc)
        {
            BaseMPIdentity identity = (BaseMPIdentity) Activator.CreateInstance(type);
            identity.PreInitReplicateRemote(pc);
            this.InitializeIdentity(identity, runtimeID);
            return identity;
        }

        public MPSendPacketContainer CreateSendPacket<T>() where T: Table
        {
            return this.CreateSendPacket<T>(this._defaultSendBuilder);
        }

        public MPSendPacketContainer CreateSendPacket<T>(FlatBufferBuilder builder) where T: Table
        {
            return this.CreateSendPacket(typeof(T), builder);
        }

        public MPSendPacketContainer CreateSendPacket(System.Type type, FlatBufferBuilder builder)
        {
            return new MPSendPacketContainer { packetTypeID = MPMappings.MPPacketMapping.Get(type), builder = builder };
        }

        public void Destroy()
        {
        }

        public void DestroyMPIdentity(uint runtimeID)
        {
            MPSendPacketContainer pc = this.CreateSendPacket<Packet_Basic_Destroy>();
            Packet_Basic_Destroy.StartPacket_Basic_Destroy(pc.builder);
            pc.Finish<Packet_Basic_Destroy>(Packet_Basic_Destroy.EndPacket_Basic_Destroy(pc.builder));
            this.SendReliableToOthers(runtimeID, pc);
            this.RemoveMPIdentity(runtimeID);
        }

        protected virtual void DispatchPacket(MPRecvPacketContainer pc)
        {
            if (pc.packet is Packet_Basic_Instantiate)
            {
                Packet_Basic_Instantiate instantiate = pc.As<Packet_Basic_Instantiate>();
                this.BindImportantFixedChannel(pc.runtimeID, instantiate.ChannelSequence);
                this._pendingInstantiates.Add(pc.runtimeID, instantiate.PeerType);
            }
            else if (pc.packet is Packet_Basic_Destroy)
            {
                this.RemoveMPIdentity(pc.runtimeID);
            }
            else if (this._pendingInstantiates.ContainsKey(pc.runtimeID))
            {
                System.Type type = MPMappings.MPPeerMapping.Get(this._pendingInstantiates[pc.runtimeID]);
                this.CreateRemoteMPIdentity(type, pc.runtimeID, pc);
                this._pendingInstantiates.Remove(pc.runtimeID);
            }
            else if (pc.channel == this._peer.reliableChannel)
            {
                this._identities[pc.runtimeID].OnReliablePacket(pc);
            }
            else
            {
                BaseMPIdentity identity;
                if ((pc.channel == this._peer.stateUpdateChannel) && this._identities.TryGetValue(pc.runtimeID, out identity))
                {
                    identity.OnStateUpdatePacket(pc);
                }
            }
        }

        public int GetAllocedChannelID(uint runtimeID)
        {
            return this._runtimeIDChannelMap[runtimeID];
        }

        public T GetIdentity<T>(uint runtimeID) where T: BaseMPIdentity
        {
            return (T) this._identities[runtimeID];
        }

        public void InitAtAwake()
        {
        }

        public void InitAtStart()
        {
        }

        private void InitChannelManagement()
        {
            this._runtimeIDChannelMap = new Dictionary<uint, int>();
            this._channelOccupiedMap = new bool[this._peer.channelSequenceCapacity];
            this._channelOccupiedMap[this._peer.channelSequenceCapacity - 1] = true;
        }

        private void InitializeIdentity(BaseMPIdentity identity, uint runtimeID)
        {
            identity.runtimeID = runtimeID;
            identity.isOwner = identity.GetPeerID() == this._peer.peerID;
            this._identities.Add(runtimeID, identity);
            this._identitiesList.Add(identity);
            identity.Init();
            if (identity.isAuthority)
            {
                identity.OnAuthorityStart();
            }
            else
            {
                identity.OnRemoteStart();
            }
        }

        public T InstantiateMPIdentity<T>(uint runtimeID, MPSendPacketContainer initPc) where T: BaseMPIdentity, new()
        {
            int num = this.AllocChannelSequenceForRuntimeID(runtimeID);
            MPSendPacketContainer pc = this.CreateSendPacket<Packet_Basic_Instantiate>(this._instantiateBuilder);
            Packet_Basic_Instantiate.StartPacket_Basic_Instantiate(pc.builder);
            Packet_Basic_Instantiate.AddPeerType(pc.builder, (byte) MPMappings.MPPeerMapping.Get(typeof(T)));
            Packet_Basic_Instantiate.AddChannelSequence(pc.builder, (byte) num);
            pc.Finish<Packet_Basic_Instantiate>(Packet_Basic_Instantiate.EndPacket_Basic_Instantiate(pc.builder));
            this.SendReliableToOthers(runtimeID, pc);
            this.SendReliableToOthers(runtimeID, initPc);
            return this.CreateMPIdentity<T>(runtimeID);
        }

        private void OnPeerPacketCallback(byte[] buffer, int len, int offset, int channel)
        {
            Buffer.BlockCopy(buffer, offset, this._tmpBuffer, 0, len);
            this._bitPacker.unpack(this._tmpBuffer, len, this._recvBuffer, 0);
            this._recvByteBuffer.Reset();
            int index = this._recvByteBuffer.Get(0);
            int controlByte = this._recvByteBuffer.Get(1);
            int num3 = this.ParseControlByteSenderPeerID(controlByte);
            uint @uint = this._recvByteBuffer.GetUint(2);
            System.Type type = MPMappings.MPPacketMapping.Get(index);
            Table table = MPMappings.cachedRecvPackets[index];
            this._recvByteBuffer.Position = 6;
            table.ResetAndInitTo(this._recvByteBuffer);
            MPRecvPacketContainer pc = new MPRecvPacketContainer {
                runtimeID = @uint,
                channel = channel,
                fromPeerID = num3,
                packet = table
            };
            this.DispatchPacket(pc);
        }

        private byte PackControlByte(int selfPeerID)
        {
            return (byte) (((byte) this.peerID) << 5);
        }

        private int ParseControlByteSenderPeerID(int controlByte)
        {
            return ((controlByte & 0xe0) >> 5);
        }

        public virtual void PostCore()
        {
        }

        public void RegisterIdentity(uint runtimeID, int channelSequence, BaseMPIdentity identity)
        {
            this.BindImportantFixedChannel(runtimeID, channelSequence);
            this.InitializeIdentity(identity, runtimeID);
        }

        private void ReleaseChannelSequenceForRuntimeID(uint runtimeID)
        {
            int index = this._runtimeIDChannelMap[runtimeID];
            if (index != (this._peer.channelSequenceCapacity - 1))
            {
                this._channelOccupiedMap[index] = false;
            }
            this._runtimeIDChannelMap.Remove(runtimeID);
        }

        private void RemoveMPIdentity(BaseMPIdentity identity)
        {
            this.ReleaseChannelSequenceForRuntimeID(identity.runtimeID);
            identity.OnRemoval();
            bool flag = this._identitiesList.Remove(identity) & this._identities.Remove(identity.runtimeID);
        }

        private void RemoveMPIdentity(uint runtimeID)
        {
            this.RemoveMPIdentity(this._identities[runtimeID]);
        }

        private void SendByChannel(int peerID, int channel, MPSendPacketContainer pc)
        {
            ByteBuffer dataBuffer = pc.builder.DataBuffer;
            int offset = dataBuffer.Position - 6;
            dataBuffer.PutByte(offset, (byte) pc.packetTypeID);
            dataBuffer.PutByte(offset + 1, this.PackControlByte(this._peerID));
            dataBuffer.PutUint(offset + 2, pc.runtimeID);
            dataBuffer.Position = offset;
            int count = dataBuffer.Length - dataBuffer.Position;
            Buffer.BlockCopy(dataBuffer.Data, dataBuffer.Position, this._tmpBuffer, 0, count);
            count = this._bitPacker.pack(this._tmpBuffer, this._bitPacker.RoundUpTo8(count), this._sendBuffer, 1);
            pc.builder.Clear();
            this._sendBuffer[0] = this._peer.PackHeader((byte) peerID, 0x1f);
            int channelSequence = this._runtimeIDChannelMap[pc.runtimeID];
            this._peer.SendByChannel(this._sendBuffer, count + 1, channel, channelSequence);
            pc.state = MPSendContainerState.Sent;
        }

        public void SendReliableToOthers(uint runtimeID, MPSendPacketContainer pc)
        {
            pc.runtimeID = runtimeID;
            this.SendByChannel(7, this._peer.reliableChannel, pc);
        }

        public void SendReliableToPeer(uint runtimeID, int peerID, MPSendPacketContainer pc)
        {
            pc.runtimeID = runtimeID;
            this.SendByChannel(peerID, this._peer.reliableChannel, pc);
        }

        public void SendStateUpdateToOthers(uint runtimeID, MPSendPacketContainer pc)
        {
            pc.runtimeID = runtimeID;
            this.SendByChannel(7, this._peer.stateUpdateChannel, pc);
        }

        public void SetupPeer(MPPeer peer, bool isMasterClient)
        {
            this._peer = peer;
            this._isMaster = isMasterClient;
            this._peerID = peer.peerID;
            this._peer.onPacket = new MPPeer.ReceiveHandler(this.OnPeerPacketCallback);
            this.InitChannelManagement();
        }

        public BaseMPIdentity TryGetIdentity(uint runtimeID)
        {
            BaseMPIdentity identity;
            this._identities.TryGetValue(runtimeID, out identity);
            return identity;
        }

        public T TryGetIdentity<T>(uint runtimeID) where T: BaseMPIdentity
        {
            BaseMPIdentity identity;
            this._identities.TryGetValue(runtimeID, out identity);
            return (identity as T);
        }

        public bool isMaster
        {
            get
            {
                return this._isMaster;
            }
        }

        public int peerID
        {
            get
            {
                return this._peerID;
            }
        }
    }
}

