using System.Net.Sockets;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class LogoutRequestInCommand : InCommand
    {
        private ClientConnection _clientConnection;
        
        public LogoutRequestInCommand(ClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            _clientConnection.EndCommandListening();
        }
    }
}