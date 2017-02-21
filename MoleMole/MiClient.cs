namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class MiClient : MiClientInterface
    {
        private EndPoint _remoteEndPoint;
        private Thread client_producer_thread_ = null;
        private bool connected_before_ = false;
        private Action disconnect_callback_ = null;
        private Action<NetPacketV1> doCmd_callback_ = null;
        private string host_ = string.Empty;
        private NetPacketV1 keepalive_packet_ = null;
        private int keepalive_time_ms_ = 0;
        private DateTime last_keepalive_time_ = TimeUtil.Now;
        private byte[] left_buf_ = new byte[0x40000];
        private int left_buf_len_ = 0;
        private ushort port_ = 0;
        private Queue<NetPacketV1> recv_queue_ = new Queue<NetPacketV1>();
        private Socket socket_ = null;
        private ManualResetEvent timeout_event_ = new ManualResetEvent(false);
        private int timeout_ms_ = 0;

        private void clientConsumerThreadHandler()
        {
            while (this.isConnected() || (0 < this.recv_queue_.Count))
            {
                NetPacketV1 tv = this.recv(-1);
                if (tv != null)
                {
                    this.doCmd_callback_(tv);
                }
            }
        }

        private void clientProducerThreadHandler()
        {
            while (this.isConnected())
            {
                try
                {
                    List<NetPacketV1> list = this.recvPacketList();
                    if ((list != null) && (list.Count != 0))
                    {
                        foreach (NetPacketV1 tv in list)
                        {
                            Queue<NetPacketV1> queue = this.recv_queue_;
                            lock (queue)
                            {
                                this.recv_queue_.Enqueue(tv);
                            }
                        }
                        this.timeout_event_.Set();
                    }
                    this.keepalive();
                    continue;
                }
                catch (SystemException)
                {
                    continue;
                }
            }
            this.disconnect();
        }

        public bool connect(string host, ushort port, int timeout_ms = 0x7d0)
        {
            try
            {
                if (this.isConnected())
                {
                    return false;
                }
                this.host_ = host;
                this.port_ = port;
                this.timeout_ms_ = timeout_ms;
                IPAddress[] hostAddresses = Dns.GetHostAddresses(this.host_);
                if (hostAddresses.Length == 0)
                {
                    return false;
                }
                this.socket_ = null;
                this._remoteEndPoint = null;
                for (int i = 0; i < hostAddresses.Length; i++)
                {
                    IPAddress iPAddress = GetIPAddress(hostAddresses[i]);
                    IPEndPoint point = new IPEndPoint(iPAddress, this.port_);
                    Socket state = new Socket(point.AddressFamily, SocketType.Stream, ProtocolType.Tcp) {
                        NoDelay = true,
                        Blocking = true,
                        SendTimeout = this.timeout_ms_,
                        ReceiveTimeout = this.timeout_ms_,
                        ReceiveBufferSize = 0x80000
                    };
                    this.timeout_event_.Reset();
                    state.BeginConnect(iPAddress, this.port_, new AsyncCallback(this.connectCallback), state);
                    this.timeout_event_.WaitOne(timeout_ms, false);
                    if (state.Connected)
                    {
                        this.socket_ = state;
                        this._remoteEndPoint = point;
                        break;
                    }
                }
                if (this.socket_ == null)
                {
                    return false;
                }
                this.startClientThread();
                this.connected_before_ = true;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void connectCallback(IAsyncResult asyncresult)
        {
            try
            {
                (asyncresult.AsyncState as Socket).EndConnect(asyncresult);
            }
            catch (SystemException)
            {
            }
            this.timeout_event_.Set();
        }

        public void disconnect()
        {
            this.connected_before_ = false;
            if (this.isConnected())
            {
                if (this.socket_ != null)
                {
                    this.socket_.Close();
                }
                this.socket_ = null;
                this.timeout_event_.Set();
            }
        }

        ~MiClient()
        {
            this.disconnect();
        }

        public static IPAddress GetIPAddress(IPAddress ip)
        {
            return ip;
        }

        private bool isClientThreadRun()
        {
            return ((this.client_producer_thread_ != null) && this.client_producer_thread_.IsAlive);
        }

        public bool isConnected()
        {
            if (this.socket_ == null)
            {
                return false;
            }
            bool flag = true;
            try
            {
                if (this._remoteEndPoint == null)
                {
                    flag = false;
                }
            }
            catch (SystemException)
            {
                flag = false;
            }
            if (!flag && this.connected_before_)
            {
                this.connected_before_ = false;
                if (this.disconnect_callback_ != null)
                {
                    this.disconnect_callback_();
                }
            }
            return flag;
        }

        public static bool IsIPV6()
        {
            return false;
        }

        private void keepalive()
        {
            if ((this.keepalive_time_ms_ != 0) && (this.keepalive_packet_ != null))
            {
                TimeSpan span = (TimeSpan) (TimeUtil.Now - this.last_keepalive_time_);
                if (span.TotalMilliseconds >= this.keepalive_time_ms_)
                {
                    this.send(this.keepalive_packet_);
                    this.last_keepalive_time_ = TimeUtil.Now;
                }
            }
        }

        public NetPacketV1 recv(int timeout_ms = 0)
        {
            Queue<NetPacketV1> queue = this.recv_queue_;
            lock (queue)
            {
                if (0 < this.recv_queue_.Count)
                {
                    return this.recv_queue_.Dequeue();
                }
            }
            if (!this.isConnected())
            {
                return null;
            }
            if (timeout_ms == 0)
            {
                return null;
            }
            this.timeout_event_.Reset();
            this.timeout_event_.WaitOne(timeout_ms, false);
            Queue<NetPacketV1> queue2 = this.recv_queue_;
            lock (queue2)
            {
                if (this.recv_queue_.Count == 0)
                {
                    return null;
                }
                return this.recv_queue_.Dequeue();
            }
        }

        private List<NetPacketV1> recvPacketList()
        {
            if (!this.isConnected())
            {
                return null;
            }
            List<NetPacketV1> list = new List<NetPacketV1>();
            try
            {
                NetPacketV1 tv;
                byte[] buffer = new byte[0x40000];
                SocketError success = SocketError.Success;
                int length = this.socket_.Receive(buffer, 0, buffer.Length, SocketFlags.None, out success);
                switch (success)
                {
                    case SocketError.TimedOut:
                    case SocketError.WouldBlock:
                    case SocketError.IOPending:
                        return list;
                }
                if ((success != SocketError.Success) || (length == 0))
                {
                    this.socket_.Close();
                    this.socket_ = null;
                    return list;
                }
                byte[] destinationArray = new byte[0x40000];
                Array.Copy(this.left_buf_, 0, destinationArray, 0, this.left_buf_len_);
                Array.Copy(buffer, 0, destinationArray, this.left_buf_len_, length);
                int num2 = length + this.left_buf_len_;
                this.left_buf_len_ = 0;
                for (int i = num2; i > 0; i -= tv.getPacketLen())
                {
                    byte[] buffer3 = new byte[i];
                    Array.Copy(destinationArray, num2 - i, buffer3, 0, i);
                    tv = new NetPacketV1();
                    PacketStatus status = tv.deserialize(ref buffer3);
                    if (status != PacketStatus.PACKET_CORRECT)
                    {
                        if (status == PacketStatus.PACKET_NOT_COMPLETE)
                        {
                            this.left_buf_len_ = i;
                            Array.Copy(buffer3, 0, this.left_buf_, 0, this.left_buf_len_);
                        }
                        return list;
                    }
                    list.Add(tv);
                }
            }
            catch (SystemException)
            {
                if (this.isConnected())
                {
                }
            }
            return list;
        }

        public bool send(NetPacketV1 packet)
        {
            if (packet == null)
            {
                return false;
            }
            if (!this.isConnected())
            {
                return false;
            }
            MemoryStream ms = new MemoryStream();
            if (!packet.serialize(ref ms))
            {
                return false;
            }
            try
            {
                SocketError success = SocketError.Success;
                int num = this.socket_.Send(ms.GetBuffer(), 0, (int) ms.Length, SocketFlags.None, out success);
                if (success != SocketError.Success)
                {
                    return false;
                }
                if (num != ms.Length)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void setCmdCallBack(Action<NetPacketV1> callback)
        {
            this.doCmd_callback_ = callback;
        }

        public void setDisconnectCallback(Action callback)
        {
            this.disconnect_callback_ = callback;
        }

        public bool setKeepalive(int time_ms, NetPacketV1 packet)
        {
            if ((time_ms <= 0) || (packet == null))
            {
                return false;
            }
            this.keepalive_time_ms_ = time_ms;
            this.keepalive_packet_ = packet;
            return true;
        }

        private bool startClientThread()
        {
            if (this.isClientThreadRun())
            {
                return false;
            }
            this.client_producer_thread_ = new Thread(new ThreadStart(this.clientProducerThreadHandler));
            this.client_producer_thread_.Start();
            return true;
        }

        public string Host
        {
            get
            {
                return this.host_;
            }
        }

        public ushort Port
        {
            get
            {
                return this.port_;
            }
        }
    }
}

