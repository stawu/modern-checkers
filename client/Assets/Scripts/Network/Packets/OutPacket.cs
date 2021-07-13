using System;
using System.Collections.Generic;
using System.Net;

namespace Network.Packets
{
    public abstract class OutPacket
    {
        private readonly Queue<byte> _data = new Queue<byte>();

        public byte[] GetDataAsByteArray()
        {
            return _data.ToArray();
        }

        protected void InsertValue(short value)
        {
            foreach (var b in BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)))
                _data.Enqueue(b);
        }
        
        protected void InsertValue(int value)
        {
            foreach (var b in BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)))
                _data.Enqueue(b);
        }
        
        protected void InsertValue(float value)
        {
            foreach (var b in BitConverter.GetBytes(value))
                _data.Enqueue(b);
        }
        
        protected void InsertValue(string value)
        {
            InsertValue(value.Length);
            foreach (var c in value)
            {
                foreach (var b in BitConverter.GetBytes(c))
                    _data.Enqueue(b);
            }
        }
    }
}