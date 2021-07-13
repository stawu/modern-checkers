using System.Net.Sockets;
using LoggedInScene;

namespace Network.Packets.In.Commands
{
    public class StartMatchInCommand : InCommand
    {
        private readonly PlayLogic _playLogicInstance;

        public StartMatchInCommand(PlayLogic playLogicInstance)
        {
            _playLogicInstance = playLogicInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            
        }

        public override void Execute()
        {
            _playLogicInstance.StartMatch();
        }
    }
}