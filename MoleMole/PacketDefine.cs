namespace MoleMole
{
    using System;

    internal class PacketDefine
    {
        public const uint head_magic = 0x1234567;
        public const ushort head_version_v1 = 1;
        public const int packet_head_len_v1 = 40;
        public const int packet_tail_len_v1 = 4;
        public const uint tail_magic = 0x89abcdef;
    }
}

