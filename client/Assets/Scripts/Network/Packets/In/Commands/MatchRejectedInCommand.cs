using System.Net.Sockets;
using LoggedInScene;

namespace Network.Packets.In.Commands
{
    public class MatchRejectedInCommand : InCommand
    {
        private PlayLogic _playLogicInstance;

        public MatchRejectedInCommand(PlayLogic playLogicInstance)
        {
            _playLogicInstance = playLogicInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
   
        }

        public override void Execute()
        {
            _playLogicInstance.onMatchRejectedByOpponent.Invoke();
            _playLogicInstance.SearchCompetitiveMatch();
        }
    }
}