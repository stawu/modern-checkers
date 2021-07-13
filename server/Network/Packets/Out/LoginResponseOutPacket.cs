namespace Warcaby_Server.Network.Packets.Out
{
    public class LoginResponseOutPacket : OutPacket
    {
        public enum LoginStatus : short
        {
            Negative = 0,
            Successful = 1
        }
        
        public LoginResponseOutPacket(LoginStatus status, string responseText)
        {
            InsertValue((short)status);
            InsertValue(responseText);
        }
    }
}