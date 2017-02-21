namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public class AvatarIdentity : BaseAnimatorEntityIdentity
    {
        protected BaseMonoAvatar _avatar;
        protected AvatarActor _avatarActor;
        private IdentityRemoteMode _remoteMode;

        public override void Init()
        {
            this._avatar = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(base.runtimeID);
            this._avatarActor = Singleton<MPEventManager>.Instance.GetActor<AvatarActor>(base.runtimeID);
            base._animatorEntity = this._avatar;
            base._abilityEntity = this._avatar;
            base._abilityActor = this._avatarActor;
            this._avatarActor.GetPlugin<MPAvatarActorPlugin>().SetupIdentity(this);
            this._avatarActor.GetPluginAs<ActorAbilityPlugin, MPActorAbilityPlugin>().SetupIdentity(this);
            switch (Singleton<MPLevelManager>.Instance.mpMode)
            {
                case MPMode.Normal:
                    this._remoteMode = IdentityRemoteMode.Mute;
                    break;

                case MPMode.PvP_SendNoReceive:
                    this._remoteMode = IdentityRemoteMode.SendAndNoReceive;
                    this._avatar.SetAttackSelectMethod(new Action<BaseMonoAvatar>(AvatarAttackTargetSelectPattern.PvPSelectRemoteAvatar));
                    break;

                case MPMode.PvP_ReceiveNoSend:
                    this._remoteMode = IdentityRemoteMode.ReceiveAndNoSend;
                    this._avatar.SetAttackSelectMethod(new Action<BaseMonoAvatar>(AvatarAttackTargetSelectPattern.PvPSelectRemoteAvatar));
                    break;
            }
            base.Init();
        }

        private void OnRemoteKill(Packet_Entity_Kill packet)
        {
            this._avatarActor.Kill(packet.KillerID, packet.AnimEventID, (KillEffect) packet.KillEffect);
        }

        protected override void OnRemoteReliablePacket(MPRecvPacketContainer pc)
        {
            base.OnRemoteReliablePacket(pc);
            if (pc.packet is Packet_Entity_Kill)
            {
                this.OnRemoteKill(pc.As<Packet_Entity_Kill>());
            }
        }

        public override IdentityRemoteMode remoteMode
        {
            get
            {
                return this._remoteMode;
            }
        }
    }
}

