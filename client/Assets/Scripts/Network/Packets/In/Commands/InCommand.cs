using System.Net.Sockets;

namespace Network.Packets.In.Commands
{
    public abstract class InCommand : InPacket
    {
        public abstract void Execute();
    }
}