using System.Net.Sockets;
using Match;

namespace Network.Packets.In.Commands
{
    public class MatchEndInCommand : InCommand
    {
        private MatchLogic _matchLogicInstance;
        private bool _matchWon;

        public MatchEndInCommand(MatchLogic matchLogicInstance)
        {
            _matchLogicInstance = matchLogicInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _matchWon = ReadShort(networkStream) == 1;
        }

        public override void Execute()
        {
            if(_matchWon)
                _matchLogicInstance.onMatchWon.Invoke();
            else
                _matchLogicInstance.onMatchLost.Invoke();
        }
    }
}