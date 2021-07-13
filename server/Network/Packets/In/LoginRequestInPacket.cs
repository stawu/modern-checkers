using System.Net.Sockets;

namespace Warcaby_Server.Network.Packets.In
{
    public class LoginRequestInPacket : InPacket
    {

        public string Login;
        public string Password;
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            Login = ReadString(networkStream);
            Password = ReadString(networkStream);
        }
    }
}