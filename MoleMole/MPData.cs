namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;

    public static class MPData
    {
        public static ConfigMPArguments AVATAR_DEFAULT_MP_SETTINGS;
        public static ConfigMPArguments MONSTER_DEFAULT_MP_SETTINGS;
        public static HashSet<System.Type> ReplicatedEventTypes;
        public static HashSet<System.Type> ReplicatedEventWireTypes;

        static MPData()
        {
            ConfigMPArguments arguments = new ConfigMPArguments {
                SyncSendInterval = 0.06666667f,
                RemoteMode = IdentityRemoteMode.Mute
            };
            arguments.MuteSyncAnimatorTags = new string[] { "AVATAR_HITSUB", "AVATAR_DIESUB", "AVATAR_THROW" };
            AVATAR_DEFAULT_MP_SETTINGS = arguments;
            arguments = new ConfigMPArguments {
                SyncSendInterval = 0.1f,
                RemoteMode = IdentityRemoteMode.SendAndReceive
            };
            arguments.MuteSyncAnimatorTags = new string[] { "MONSTER_HITSUB", "MONSTER_DIESUB", "MONSTER_THROWSUB" };
            MONSTER_DEFAULT_MP_SETTINGS = arguments;
            ReplicatedEventTypes = new HashSet<System.Type> { typeof(EvtHittingOther), typeof(EvtBeingHit), typeof(EvtAttackLanded), typeof(EvtEvadeSuccess) };
            ReplicatedEventWireTypes = new HashSet<System.Type> { typeof(Packet_Event_EvtHittingOther), typeof(Packet_Event_EvtBeingHit), typeof(Packet_Event_EvtAttackLanded), typeof(Packet_Event_EvtEvadeSuccess) };
        }
    }
}

