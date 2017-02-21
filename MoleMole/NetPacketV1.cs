namespace MoleMole
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class NetPacketV1
    {
        private MemoryStream body_ = new MemoryStream();
        private uint body_len_ = 0;
        private ushort client_version_ = 0;
        private ushort cmd_id_ = 0;
        private uint gateway_ip_ = 0;
        private uint head_magic_ = NetUtils.HostToNetworkOrder((uint) 0x1234567);
        private ushort packet_version_ = NetUtils.HostToNetworkOrder((ushort) 1);
        private uint sign_ = 0;
        private ushort sign_type_ = 0;
        private uint tail_magic_ = NetUtils.HostToNetworkOrder((uint) 0x89abcdef);
        private uint time_ = 0;
        private uint user_id_ = 0;
        private uint user_ip_ = 0;
        private uint user_session_id_ = 0;

        public PacketStatus deserialize(ref byte[] buf)
        {
            this.body_.SetLength(0L);
            this.body_.Position = 0L;
            if (buf == null)
            {
                return PacketStatus.PACKET_NOT_CORRECT;
            }
            if (buf.Length < 0x2c)
            {
                return PacketStatus.PACKET_NOT_COMPLETE;
            }
            int startIndex = 0;
            this.head_magic_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.head_magic_);
            this.packet_version_ = BitConverter.ToUInt16(buf, startIndex);
            startIndex += Marshal.SizeOf(this.packet_version_);
            this.client_version_ = BitConverter.ToUInt16(buf, startIndex);
            startIndex += Marshal.SizeOf(this.client_version_);
            this.time_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.time_);
            this.user_id_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.user_id_);
            this.user_ip_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.user_ip_);
            this.user_session_id_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.user_session_id_);
            this.gateway_ip_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.gateway_ip_);
            this.cmd_id_ = BitConverter.ToUInt16(buf, startIndex);
            startIndex += Marshal.SizeOf(this.cmd_id_);
            this.body_len_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.body_len_);
            this.sign_type_ = BitConverter.ToUInt16(buf, startIndex);
            startIndex += Marshal.SizeOf(this.sign_type_);
            this.sign_ = BitConverter.ToUInt32(buf, startIndex);
            startIndex += Marshal.SizeOf(this.sign_);
            if (this.getHeadMagic() != 0x1234567)
            {
                return PacketStatus.PACKET_NOT_CORRECT;
            }
            if (buf.Length < this.getPacketLen())
            {
                return PacketStatus.PACKET_NOT_COMPLETE;
            }
            this.tail_magic_ = BitConverter.ToUInt32(buf, startIndex + ((int) this.getBodyLen()));
            if (this.getTailMagic() != 0x89abcdef)
            {
                return PacketStatus.PACKET_NOT_CORRECT;
            }
            this.body_.Write(buf, startIndex, (int) this.getBodyLen());
            this.body_.Position = 0L;
            return PacketStatus.PACKET_CORRECT;
        }

        public uint getBodyLen()
        {
            return NetUtils.NetworkToHostOrder(this.body_len_);
        }

        public ushort getClientVersion()
        {
            return NetUtils.NetworkToHostOrder(this.client_version_);
        }

        public ushort getCmdId()
        {
            return NetUtils.NetworkToHostOrder(this.cmd_id_);
        }

        public T getData<T>()
        {
            if (this.body_.Length == 0)
            {
                return default(T);
            }
            try
            {
                this.body_.Position = 0L;
                return (T) Singleton<NetworkManager>.Instance.serializer.Deserialize(this.body_, null, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public uint getHeadMagic()
        {
            return NetUtils.NetworkToHostOrder(this.head_magic_);
        }

        public int getPacketLen()
        {
            return ((40 + ((int) this.getBodyLen())) + 4);
        }

        public ushort getPacketVersion()
        {
            return NetUtils.NetworkToHostOrder(this.packet_version_);
        }

        public uint getSign()
        {
            return NetUtils.NetworkToHostOrder(this.sign_);
        }

        public ushort getSignType()
        {
            return NetUtils.NetworkToHostOrder(this.sign_type_);
        }

        public uint getTailMagic()
        {
            return NetUtils.NetworkToHostOrder(this.tail_magic_);
        }

        public uint getTime()
        {
            return NetUtils.NetworkToHostOrder(this.time_);
        }

        public uint getUserId()
        {
            return NetUtils.NetworkToHostOrder(this.user_id_);
        }

        public bool serialize(ref MemoryStream ms)
        {
            if (ms == null)
            {
                return false;
            }
            if (this.body_ == null)
            {
                return false;
            }
            ms.SetLength(0L);
            ms.Position = 0L;
            ms.Write(BitConverter.GetBytes(this.head_magic_), 0, Marshal.SizeOf(this.head_magic_));
            ms.Write(BitConverter.GetBytes(this.packet_version_), 0, Marshal.SizeOf(this.packet_version_));
            ms.Write(BitConverter.GetBytes(this.client_version_), 0, Marshal.SizeOf(this.client_version_));
            ms.Write(BitConverter.GetBytes(this.time_), 0, Marshal.SizeOf(this.time_));
            ms.Write(BitConverter.GetBytes(this.user_id_), 0, Marshal.SizeOf(this.user_id_));
            ms.Write(BitConverter.GetBytes(this.user_ip_), 0, Marshal.SizeOf(this.user_ip_));
            ms.Write(BitConverter.GetBytes(this.user_session_id_), 0, Marshal.SizeOf(this.user_session_id_));
            ms.Write(BitConverter.GetBytes(this.gateway_ip_), 0, Marshal.SizeOf(this.gateway_ip_));
            ms.Write(BitConverter.GetBytes(this.cmd_id_), 0, Marshal.SizeOf(this.cmd_id_));
            ms.Write(BitConverter.GetBytes(this.body_len_), 0, Marshal.SizeOf(this.body_len_));
            ms.Write(BitConverter.GetBytes(this.sign_type_), 0, Marshal.SizeOf(this.sign_type_));
            ms.Write(BitConverter.GetBytes(this.sign_), 0, Marshal.SizeOf(this.sign_));
            this.body_.WriteTo(ms);
            ms.Write(BitConverter.GetBytes(this.tail_magic_), 0, Marshal.SizeOf(this.tail_magic_));
            return true;
        }

        private void setBodyLen()
        {
            this.body_len_ = NetUtils.HostToNetworkOrder((uint) this.body_.Length);
        }

        public void setClientVersion(ushort client_version)
        {
            this.client_version_ = NetUtils.HostToNetworkOrder(client_version);
        }

        public void setCmdId(ushort cmd_id)
        {
            this.cmd_id_ = NetUtils.HostToNetworkOrder(cmd_id);
        }

        public bool setData<T>(T data)
        {
            if (data == null)
            {
                return false;
            }
            try
            {
                this.body_.SetLength(0L);
                this.body_.Position = 0L;
                Singleton<NetworkManager>.Instance.serializer.Serialize(this.body_, data);
            }
            catch (Exception)
            {
                return false;
            }
            this.setBodyLen();
            this.setSign();
            return true;
        }

        private void setSign()
        {
            this.sign_ = NetUtils.HostToNetworkOrder(Crc32Utils.crc32(this.body_));
        }

        public void setTime(uint time)
        {
            this.time_ = NetUtils.HostToNetworkOrder(time);
        }

        public void setUserId(uint user_id)
        {
            this.user_id_ = NetUtils.HostToNetworkOrder(user_id);
        }
    }
}

