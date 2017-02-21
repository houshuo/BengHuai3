namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public class MonsterIdentity : BaseAnimatorEntityIdentity
    {
        protected BaseMonoMonster _monster;
        protected MonsterActor _monsterActor;

        protected override void AuthoritySendTransformSyncCore()
        {
            if (this._monster.IsActive())
            {
                base.AuthoritySendTransformSyncCore();
            }
        }

        public override void Init()
        {
            this._monster = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(base.runtimeID);
            this._monsterActor = Singleton<MPEventManager>.Instance.GetActor<MonsterActor>(base.runtimeID);
            base._animatorEntity = this._monster;
            base._abilityEntity = this._monster;
            base._abilityActor = this._monsterActor;
            this._monsterActor.GetPlugin<MPMonsterActorPlugin>().SetupIdentity(this);
            this._monsterActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>().SetupIdentity(this);
            base.Init();
        }

        protected override void OnRemoteEntityAnimatorParameterChanged(Packet_Entity_AnimatorParameterChange packet)
        {
            if (this._monster.IsActive())
            {
                base.OnRemoteEntityAnimatorParameterChanged(packet);
            }
        }

        protected override void OnRemoteEntityAnimatorStateChanged(Packet_Entity_AnimatorStateChange packet)
        {
            if (this._monster.IsActive())
            {
                base.OnRemoteEntityAnimatorStateChanged(packet);
            }
        }

        private void OnRemoteKill(Packet_Entity_Kill packet)
        {
            this._monsterActor.Kill(packet.KillerID, packet.AnimEventID, (KillEffect) packet.KillEffect);
        }

        protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
        {
            base.OnRemoteReliablePacket(pc);
            if (pc.packet is Packet_Entity_Kill)
            {
                this.OnRemoteKill(pc.As<Packet_Entity_Kill>());
            }
        }

        public override void OnRemoteStart()
        {
            base.OnRemoteStart();
            this._monster.destroyMode = BaseMonoMonster.DestroyMode.DeactivateOnly;
        }

        protected override void OnRemoteTransformSync(Packet_Entity_TransformSync packet)
        {
            if ((this._monster.IsActive() && !this._monster.IsRetreating()) && !this._monster.IsFrameHalting())
            {
                base.OnRemoteTransformSync(packet);
            }
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            this._monster.destroyMode = BaseMonoMonster.DestroyMode.SetToBeRemoved;
            this._monster.SetDestroy();
        }

        public override void PreInitReplicateRemote(MPRecvPacketContainer pc)
        {
            Packet_Monster_MonsterCreation creation = pc.As<Packet_Monster_MonsterCreation>();
            Singleton<MonsterManager>.Instance.CreateMonster(creation.MonsterName, creation.MonsterType, creation.Level, true, MPMiscs.Convert(creation.InitPos), pc.runtimeID, creation.IsElite, creation.UniqueMonsterID, true, false, 0);
        }
    }
}

