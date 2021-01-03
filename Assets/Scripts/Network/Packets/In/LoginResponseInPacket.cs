using System.Net.Sockets;

namespace Network.Packets.In
{
    public class LoginResponseInPacket : InPacket
    {
        public enum LoginStatus : short
        {
            Negative = 0,
            Successful = 1
        }
        
        public LoginStatus Status;
        public string ResponseText;
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            Status = (LoginStatus)ReadShort(networkStream);
            ResponseText = ReadString(networkStream);
        }
    }
}