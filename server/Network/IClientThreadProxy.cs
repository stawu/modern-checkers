using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network
{
    public interface IClientThreadProxy
    {
        public void RequestToSendCommand(OutCommand command);
    }
}