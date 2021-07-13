namespace Network.Packets.Out
{
    public class LoginRequestOutPacket : OutPacket
    {
        public LoginRequestOutPacket(string login, string password)
        {
            InsertValue(login);
            InsertValue(password);
        }
    }
}