namespace MoleMole
{
    using System;

    public class RuntimeIDManager
    {
        private uint _localNextSeqID = 20;
        private uint _networkedNextSeqID = 20;
        private uint _peerID = 1;
        public const ushort AVATAR_CATE = 3;
        public const uint AVATAR_RESERVERED_SEQ_ID = 10;
        public const ushort CAMERA_CATE = 2;
        public const int CATEGORY_COUNT = 7;
        private const uint CATEGORY_MASK = 0x1f000000;
        public const int CATEGORY_SHIFT = 0x18;
        public const ushort DYNAMICOBJECT_CATE = 6;
        public const ushort EFFECT_CATE = 5;
        private const uint IS_SYNCED_MASK = 0x800000;
        public const int IS_SYNCED_SHIFT = 0x17;
        public const uint LEVEL_RUNTIMEID = 0x21800001;
        public const ushort MANAGER_CATE = 1;
        private const ushort MAX_CATEGORY = 0x20;
        private const ushort MAX_PEER = 8;
        private const uint MAX_SQUENCE = 0x800000;
        public const ushort MONSTER_CATE = 4;
        private const uint PEER_MASK = 0xe0000000;
        public const int PEER_SHIFT = 0x1d;
        public const ushort PROPOBJECT_CATE = 7;
        private const uint SEQUENCE_MASK = 0x7fffff;
        public const int SEQUENCE_SHIFT = 0;

        private RuntimeIDManager()
        {
        }

        public uint GetFixedAvatarRuntimeIDForPeer(int peerID)
        {
            return (((uint) (((peerID << 0x1d) | 0x3000000) | 0x800000)) | ((uint) (10L + peerID)));
        }

        public uint GetNextNonSyncedRuntimeID(ushort category)
        {
            this._localNextSeqID++;
            return (((this._peerID << 0x1d) | ((uint) (category << 0x18))) | this._localNextSeqID);
        }

        public uint GetNextRuntimeID(ushort category)
        {
            this._networkedNextSeqID++;
            return (((uint) (((this._peerID << 0x1d) | (category << 0x18)) | 0x800000)) | this._networkedNextSeqID);
        }

        public void InitAtAwake()
        {
        }

        public bool IsSyncedRuntimeID(uint runtimeID)
        {
            return ((runtimeID & 0x800000) != 0);
        }

        public ushort ParseCategory(uint runtimeID)
        {
            return (ushort) ((runtimeID & 0x1f000000) >> 0x18);
        }

        public uint ParsePeerID(uint runtimeID)
        {
            return (uint) ((runtimeID & -536870912) >> 0x1d);
        }

        public uint ParseSequenceID(uint runtimeID)
        {
            return (runtimeID & 0x7fffff);
        }

        public void SetupPeerID(int peerID)
        {
            this._peerID = (uint) peerID;
        }
    }
}

