using System.Net.Sockets;
using Match;
using UnityEngine;

namespace Network.Packets.In.Commands
{
    public class MatchRoundStartInCommand : InCommand
    {
        private MatchLogic _matchLogicInstance;
        private BoardController _boardControllerInstance;
        private bool _playerTurn;

        public MatchRoundStartInCommand(MatchLogic matchLogicInstance, BoardController boardControllerInstance)
        {
            Debug.Log("MatchRoundStartInCommand");
            _matchLogicInstance = matchLogicInstance;
            _boardControllerInstance = boardControllerInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _playerTurn = ReadShort(networkStream) == 1;
        }

        public override void Execute()
        {
            _boardControllerInstance.ResetPawns(_playerTurn);
            _matchLogicInstance.onFirstPlayerTurnSelected.Invoke();
            
            if(_playerTurn)
                _matchLogicInstance.onPlayerIsFirstTurn.Invoke();
            else
                _matchLogicInstance.onPlayerOpponentIsFirstTurn.Invoke();
        }
    }
}