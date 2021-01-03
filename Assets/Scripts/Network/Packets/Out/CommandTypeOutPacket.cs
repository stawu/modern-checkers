namespace Network.Packets.Out
{
    public class CommandTypeOutPacket : OutPacket
    {
        public CommandTypeOutPacket(short commandId)
        {
            InsertValue(commandId);
        }
    }
}