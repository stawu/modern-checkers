namespace Network.Packets.Out.Commands
{
    public abstract class OutCommand : OutPacket
    {
        protected OutCommand(short commandId)
        {
            InsertValue(commandId);
        }
    }
}