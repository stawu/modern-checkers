using System.Net.Sockets;
using LoggedInScene;

namespace Network.Packets.In.Commands
{
    public class MatchFoundInCommand : InCommand
    {
        private int _matchId;
        private PlayLogic _playLogicInstance;

        public MatchFoundInCommand(PlayLogic playLogicInstance)
        {
            _playLogicInstance = playLogicInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _matchId = ReadInt(networkStream);
        }

        public override void Execute()
        {
            _playLogicInstance.MarkMatchAsFounded(_matchId);
        }
    }
}