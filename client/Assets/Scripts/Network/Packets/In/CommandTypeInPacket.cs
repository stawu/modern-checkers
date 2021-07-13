using System.Net.Sockets;

namespace Network.Packets.In
{
    public class CommandTypeInPacket : InPacket
    {
        public short CommandId; 
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            CommandId = ReadShort(networkStream);
        }
    }
}