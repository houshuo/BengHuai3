namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;

    public class LevelIdentity : BaseAbilityEntityIdentiy
    {
        private MPLevelActor _mpLevelActor;
        private int _readyPeerCount = 1;

        public void DebugCreateStageWithFullDataSync(MPSendPacketContainer pc)
        {
            if (base.isOwner)
            {
                this.DebugCreateStageWithFullDataSync_Impl(pc.ReadAs<Packet_Level_CreateStageFullData>());
                Singleton<MPManager>.Instance.SendReliableToOthers(base.runtimeID, pc);
            }
        }

        private void DebugCreateStageWithFullDataSync_Impl(Packet_Level_CreateStageFullData fullData)
        {
            Singleton<LevelScoreManager>.Instance.SetDevLevelBeginIntent("Lua/Levels/Common/Level 0.lua", LevelActor.Mode.NetworkedMP, 10, 10, null);
            Singleton<LevelScoreManager>.Instance.memberList = new List<AvatarDataItem>();
            for (int i = 0; i < fullData.AvatarsLength; i++)
            {
                MoleMole.MPProtocol.MPAvatarDataItem avatars = fullData.GetAvatars(i);
                AvatarDataItem item = new AvatarDataItem(avatars.AvatarID, avatars.Level, avatars.Star);
                ConfigAvatar avatarConfig = AvatarData.GetAvatarConfig(item.AvatarRegistryKey);
                WeaponDataItem dummyFirstWeaponDataByRole = Singleton<StorageModule>.Instance.GetDummyFirstWeaponDataByRole(avatarConfig.CommonArguments.RoleName, 1);
                item.equipsMap[1] = dummyFirstWeaponDataByRole;
                Singleton<LevelScoreManager>.Instance.memberList.Add(item);
            }
            Singleton<LevelManager>.Instance.levelActor.SuddenLevelStart();
            Singleton<LevelManager>.Instance.levelActor.levelMode = LevelActor.Mode.NetworkedMP;
            Singleton<MPLevelManager>.Instance.mpMode = fullData.MpMode;
            if (fullData.MpMode == MPMode.Normal)
            {
                Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_Default_GameMode();
            }
            else if (fullData.MpMode == MPMode.PvP_ReceiveNoSend)
            {
                Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_PvPTest_GameMode();
            }
            else if (fullData.MpMode == MPMode.PvP_SendNoReceive)
            {
                Singleton<LevelManager>.Instance.gameMode = new NetworkedMP_PvPTest_GameMode();
            }
            List<string> avatarSpawnNameList = new List<string> { "Born" };
            Singleton<StageManager>.Instance.CreateStage(fullData.StageData.StageName, avatarSpawnNameList, null, false);
        }

        public override void Init()
        {
            Singleton<MPLevelManager>.Instance.levelIdentity = this;
            this._mpLevelActor = (MPLevelActor) Singleton<LevelManager>.Instance.levelActor;
            this._mpLevelActor.SetupIdentity(this);
            base._abilityEntity = this._mpLevelActor.levelEntity;
            base._abilityActor = this._mpLevelActor;
            base.Init();
        }

        private void On_Level_CreateStageFullData(Packet_Level_CreateStageFullData packet)
        {
            this.DebugCreateStageWithFullDataSync_Impl(packet);
        }

        private void OnAuthority_PeerStageReady(MPRecvPacketContainer recvPc)
        {
            Packet_Level_PeerStageReady ready = recvPc.As<Packet_Level_PeerStageReady>();
            this._readyPeerCount++;
            if (this._readyPeerCount == Singleton<MPManager>.Instance.peer.totalPeerCount)
            {
                MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_PeerStageReady>();
                Packet_Level_PeerStageReady.StartPacket_Level_PeerStageReady(pc.builder);
                Packet_Level_PeerStageReady.AddState(pc.builder, PingPongEnum.Response);
                pc.Finish<Packet_Level_PeerStageReady>(Packet_Level_PeerStageReady.EndPacket_Level_PeerStageReady(pc.builder));
                Singleton<MPManager>.Instance.SendReliableToOthers(0x21800001, pc);
                EvtStageReady evt = new EvtStageReady {
                    isBorn = true
                };
                Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            }
        }

        private void OnAuthorityLevelBuff_Request(MPRecvPacketContainer pc)
        {
            Packet_Level_RequestLevelBuff buff = pc.As<Packet_Level_RequestLevelBuff>();
            this._mpLevelActor.MPRequestStartLevelBuff((LevelBuffType) buff.LevelBuffType, (LevelBuffSide) buff.Side, buff.OwnerRuntimeID, buff.AllowRefresh, buff.EnteringSlow, buff.NotStartEffect, buff.Duration, buff.InstancedAbilityID, buff.ActionLocalID);
        }

        protected override void OnAuthorityReliablePacket(MPRecvPacketContainer pc)
        {
            base.OnAuthorityReliablePacket(pc);
            if (pc.packet is Packet_Level_PeerStageReady)
            {
                this.OnAuthority_PeerStageReady(pc);
            }
            else if (pc.packet is Packet_Level_RequestLevelBuff)
            {
                this.OnAuthorityLevelBuff_Request(pc);
            }
        }

        public override void OnAuthorityStart()
        {
        }

        private void OnRemote_PeerStageReady(MPRecvPacketContainer recvPc)
        {
            Packet_Level_PeerStageReady ready = recvPc.As<Packet_Level_PeerStageReady>();
            EvtStageReady evt = new EvtStageReady {
                isBorn = true
            };
            Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
        }

        private void OnRemote_Result_LevelBuff(MPRecvPacketContainer pc)
        {
            Packet_Level_ResultLevelBuff buff = pc.As<Packet_Level_ResultLevelBuff>();
            this._mpLevelActor.MPResponseHandleLevelBuff(buff.Action, (LevelBuffType) buff.LevelBuffType, buff.EnteringSlow, buff.NotStartEffect, buff.OwnerRuntimeID, (LevelBuffSide) buff.Side, buff.InstancedAbilityID, buff.ActionLocalID);
        }

        protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
        {
            base.OnRemoteReliablePacket(pc);
            if (pc.packet is Packet_Level_CreateStageFullData)
            {
                this.On_Level_CreateStageFullData(pc.As<Packet_Level_CreateStageFullData>());
            }
            else if (pc.packet is Packet_Level_PeerStageReady)
            {
                this.OnRemote_PeerStageReady(pc);
            }
            else if (pc.packet is Packet_Level_ResultLevelBuff)
            {
                this.OnRemote_Result_LevelBuff(pc);
            }
        }

        public override void OnRemoteStart()
        {
            this._mpLevelActor.witchTimeLevelBuff.muteUpdateDuration = true;
            this._mpLevelActor.stopWorldLevelBuff.muteUpdateDuration = true;
        }

        public override IdentityRemoteMode remoteMode
        {
            get
            {
                return IdentityRemoteMode.Mute;
            }
        }
    }
}

