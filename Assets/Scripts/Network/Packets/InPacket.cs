using System;
using System.Net;
using System.Net.Sockets;
using Match;

namespace Network.Packets
{
    public abstract class InPacket
    {
        public abstract void FillDataFromStream(NetworkStream networkStream);
        
        protected short ReadShort(NetworkStream networkStream)
        {
            var buffer = new byte[sizeof(short)];
            networkStream.Read(buffer, 0, buffer.Length);

            var value = BitConverter.ToInt16(buffer, 0);
            return IPAddress.NetworkToHostOrder(value);
        }
        
        protected int ReadInt(NetworkStream networkStream)
        {
            var buffer = new byte[sizeof(int)];
            networkStream.Read(buffer, 0, buffer.Length);

            var value = BitConverter.ToInt32(buffer, 0);
            return IPAddress.NetworkToHostOrder(value);
        }
        
        protected float ReadFloat(NetworkStream networkStream)
        {
            var buffer = new byte[sizeof(float)];
            networkStream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToSingle(buffer, 0);
        }
        
        protected string ReadString(NetworkStream networkStream)
        {
            var stringLength = ReadInt(networkStream);
            var charArray = new char[stringLength];

            for (var i = 0; i < stringLength; i++)
            {
                var buffer = new byte[sizeof(char)];
                networkStream.Read(buffer, 0, buffer.Length);
                charArray[i] = BitConverter.ToChar(buffer, 0);
            }

            return new string(charArray);
        }
    }
}