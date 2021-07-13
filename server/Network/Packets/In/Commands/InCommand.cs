namespace Warcaby_Server.Network.Packets.In.Commands
{
    public abstract class InCommand : InPacket
    {
        public abstract void Execute();
    }
}