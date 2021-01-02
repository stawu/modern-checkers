using System.Net.Sockets;

namespace Network.Packets.In
{
    public class LoginResponseInPacket : InPacket
    {
        public int testVal;
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            testVal = ReadInt(networkStream);
        }
    }
}