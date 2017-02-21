namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MPLevelActor : LevelActor
    {
        private LevelIdentity _levelIdentity;
        private const float STUB_REMOTE_DURATION = 10f;

        private void CreateLevelBuffEffect(LevelBuffType type, uint ownerID, int instancedAbilityID, int actionLocalID)
        {
            ApplyLevelBuff buff = this.LocateApplyLevelBuffConfig(ownerID, instancedAbilityID, actionLocalID);
            if ((buff != null) && !string.IsNullOrEmpty(buff.AttachLevelEffectPattern))
            {
                Singleton<EffectManager>.Instance.CreateUniqueIndexedEffectPattern(buff.AttachLevelEffectPattern, type.ToString(), Singleton<LevelManager>.Instance.levelEntity);
            }
        }

        private void CreateMPAvatars()
        {
            List<AvatarDataItem> memberList = Singleton<LevelScoreManager>.Instance.memberList;
            for (int i = 0; i < memberList.Count; i++)
            {
                int peerID = i + 1;
                bool isLocal = peerID == Singleton<MPManager>.Instance.peerID;
                uint fixedAvatarRuntimeIDForPeer = Singleton<RuntimeIDManager>.Instance.GetFixedAvatarRuntimeIDForPeer(peerID);
                Singleton<AvatarManager>.Instance.CreateAvatar(memberList[i], isLocal, InLevelData.CREATE_INIT_POS, InLevelData.CREATE_INIT_FORWARD, fixedAvatarRuntimeIDForPeer, true, true, false, false);
                Singleton<MPManager>.Instance.RegisterIdentity(fixedAvatarRuntimeIDForPeer, peerID, new AvatarIdentity());
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(fixedAvatarRuntimeIDForPeer);
                actor.CreateAppliedAbility(AbilityData.GetAbilityConfig("Temp_UnlockSKL02Button"));
                actor.CreateAppliedAbility(AbilityData.GetAbilityConfig("Test_UnlockBranchAttack"));
                actor.PushProperty("Actor_MaxHPDelta", 3000f);
            }
        }

        private void DestroyLevelBuffEffect(LevelBuffType type)
        {
            Singleton<EffectManager>.Instance.TrySetDestroyUniqueIndexedEffectPattern(type.ToString());
        }

        protected override void HandleAvatarCreationForStageCreation(EvtStageCreated evt, out bool sendStageReady)
        {
            List<MonoSpawnPoint> avatarSpawnPointList = new List<MonoSpawnPoint>();
            foreach (string str in evt.avatarSpawnNameList)
            {
                int namedSpawnPointIx = Singleton<StageManager>.Instance.GetStageEnv().GetNamedSpawnPointIx(str);
                avatarSpawnPointList.Add(Singleton<StageManager>.Instance.GetStageEnv().spawnPoints[namedSpawnPointIx]);
            }
            if (evt.isBorn)
            {
                this.CreateMPAvatars();
            }
            Singleton<AvatarManager>.Instance.InitAvatarsPos(avatarSpawnPointList);
            Singleton<MonsterManager>.Instance.InitMonstersPos(evt.offset);
            if (!Singleton<MPManager>.Instance.isMaster)
            {
                MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_PeerStageReady>();
                Packet_Level_PeerStageReady.StartPacket_Level_PeerStageReady(pc.builder);
                Packet_Level_PeerStageReady.AddState(pc.builder, PingPongEnum.Request);
                pc.Finish<Packet_Level_PeerStageReady>(Packet_Level_PeerStageReady.EndPacket_Level_PeerStageReady(pc.builder));
                Singleton<MPManager>.Instance.SendReliableToPeer(0x21800001, 1, pc);
            }
            sendStageReady = false;
        }

        protected override void InitAdditionalLevelActorPlugins()
        {
            base.witchTimeLevelBuff = new MPLevelBuffWitchTime(this);
            base.stopWorldLevelBuff = new LevelBuffStopWorld(this);
            base.levelBuffs = new BaseLevelBuff[] { base.witchTimeLevelBuff, base.stopWorldLevelBuff };
            base.AddPlugin(new MPLevelAbilityHelperPlugin(this));
        }

        private ApplyLevelBuff LocateApplyLevelBuffConfig(uint ownerID, int instancedAbilityID, int actionLocalID)
        {
            if (instancedAbilityID == 0)
            {
                return null;
            }
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(ownerID);
            if (actor == null)
            {
                return null;
            }
            ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(instancedAbilityID);
            if (instancedAbilityByID == null)
            {
                return null;
            }
            return (ApplyLevelBuff) instancedAbilityByID.config.InvokeSites[actionLocalID];
        }

        public void MPRequestStartLevelBuff(LevelBuffType type, LevelBuffSide side, uint ownerID, bool allowRefresh, bool enteringSlow, bool notStartEffect, float duration, int instancedAbilityID, int actionLocalID)
        {
            BaseLevelBuff witchTimeLevelBuff;
            LevelBuffType type2 = type;
            if (type2 == LevelBuffType.WitchTime)
            {
                witchTimeLevelBuff = base.witchTimeLevelBuff;
            }
            else if (type2 == LevelBuffType.StopWorld)
            {
                witchTimeLevelBuff = base.stopWorldLevelBuff;
            }
            else
            {
                witchTimeLevelBuff = null;
            }
            bool useMaxDuration = allowRefresh;
            if (witchTimeLevelBuff.isActive)
            {
                if (type == LevelBuffType.WitchTime)
                {
                    bool flag2 = base.witchTimeLevelBuff.Refresh(duration, side, ownerID, enteringSlow, useMaxDuration, notStartEffect);
                    if (flag2)
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Switch, base.witchTimeLevelBuff.levelBuffSide, ownerID), MPEventDispatchMode.Normal);
                        this.DestroyLevelBuffEffect(type);
                        this.CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
                    }
                    this.SendLevelBuffResponse(!flag2 ? LevelBuffAction.SameSideExtend : LevelBuffAction.SwitchSide, type, enteringSlow, notStartEffect, ownerID, base.witchTimeLevelBuff.levelBuffSide, instancedAbilityID, actionLocalID);
                }
                else if (type == LevelBuffType.StopWorld)
                {
                }
            }
            else
            {
                if (type == LevelBuffType.WitchTime)
                {
                    base.witchTimeLevelBuff.ownerID = ownerID;
                    base.witchTimeLevelBuff.Setup(enteringSlow, duration, side, notStartEffect);
                }
                else if (type == LevelBuffType.StopWorld)
                {
                    base.stopWorldLevelBuff.ownerID = ownerID;
                    base.stopWorldLevelBuff.Setup(enteringSlow, duration, ownerID);
                }
                base.AddPlugin(witchTimeLevelBuff);
                witchTimeLevelBuff.isActive = true;
                this.CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
                this.SendLevelBuffResponse(LevelBuffAction.Add, type, enteringSlow, notStartEffect, ownerID, witchTimeLevelBuff.levelBuffSide, instancedAbilityID, actionLocalID);
                Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Start, side, ownerID), MPEventDispatchMode.Normal);
            }
        }

        public void MPResponseHandleLevelBuff(LevelBuffAction action, LevelBuffType type, bool enteringSlow, bool notStartEffect, uint ownerID, LevelBuffSide side, int instancedAbilityID, int actionLocalID)
        {
            BaseLevelBuff witchTimeLevelBuff;
            LevelBuffType type2 = type;
            if (type2 == LevelBuffType.WitchTime)
            {
                witchTimeLevelBuff = base.witchTimeLevelBuff;
            }
            else if (type2 == LevelBuffType.StopWorld)
            {
                witchTimeLevelBuff = base.stopWorldLevelBuff;
            }
            else
            {
                witchTimeLevelBuff = null;
            }
            switch (action)
            {
                case LevelBuffAction.Add:
                    if (type != LevelBuffType.WitchTime)
                    {
                        if (type == LevelBuffType.StopWorld)
                        {
                            base.stopWorldLevelBuff.Setup(enteringSlow, 10f, ownerID);
                        }
                        break;
                    }
                    base.witchTimeLevelBuff.Setup(enteringSlow, 10f, side, notStartEffect);
                    break;

                case LevelBuffAction.Remove:
                    this.StopLevelBuff(witchTimeLevelBuff);
                    return;

                case LevelBuffAction.SwitchSide:
                    base.witchTimeLevelBuff.SwitchSide(enteringSlow, 10f, side, ownerID, notStartEffect);
                    this.DestroyLevelBuffEffect(type);
                    this.CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Switch, base.witchTimeLevelBuff.levelBuffSide, ownerID), MPEventDispatchMode.Normal);
                    return;

                case LevelBuffAction.SameSideExtend:
                    witchTimeLevelBuff.ownerID = ownerID;
                    base.witchTimeLevelBuff.ExtendDuration(10f, enteringSlow, true);
                    return;

                default:
                    return;
            }
            witchTimeLevelBuff.ownerID = ownerID;
            base.AddPlugin(witchTimeLevelBuff);
            witchTimeLevelBuff.isActive = true;
            this.CreateLevelBuffEffect(type, ownerID, instancedAbilityID, actionLocalID);
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelBuffState(type, LevelBuffState.Start, base.witchTimeLevelBuff.levelBuffSide, ownerID), MPEventDispatchMode.Normal);
        }

        private void SendLevelBuffResponse(LevelBuffAction action, LevelBuffType type, bool enteringSlow, bool notStartEffect, uint ownerID, LevelBuffSide side, int instancedAbilityID = 0, int actionLocalID = 0)
        {
            MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_ResultLevelBuff>();
            bool flag = enteringSlow;
            bool flag2 = notStartEffect;
            Offset<Packet_Level_ResultLevelBuff> offset = Packet_Level_ResultLevelBuff.CreatePacket_Level_ResultLevelBuff(pc.builder, action, 0, (byte) side, flag, flag2, ownerID, (byte) instancedAbilityID, (byte) actionLocalID);
            pc.Finish<Packet_Level_ResultLevelBuff>(offset);
            Singleton<MPManager>.Instance.SendReliableToOthers(0x21800001, pc);
        }

        public void SetupIdentity(LevelIdentity levelIdentity)
        {
            this._levelIdentity = levelIdentity;
        }

        public override void StopLevelBuff(BaseLevelBuff buff)
        {
            if (this._levelIdentity.isAuthority)
            {
                this.SendLevelBuffResponse(LevelBuffAction.Remove, buff.levelBuffType, false, false, 0, LevelBuffSide.FromAvatar, 0, 0);
            }
            this.DestroyLevelBuffEffect(buff.levelBuffType);
            base.StopLevelBuff(buff);
        }
    }
}

