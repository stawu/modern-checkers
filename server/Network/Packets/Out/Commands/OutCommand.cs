namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public abstract class OutCommand : OutPacket
    {
        protected OutCommand(short commandId)
        {
            InsertValue(commandId);
        }
    }
}