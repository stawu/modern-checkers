using System.Net.Sockets;
using Match;
using UnityEngine;

namespace Network.Packets.In.Commands
{
    public class OpponentPlayerDataInCommand : InCommand
    {
        private MatchLogic _matchLogicInstance;
        private BoardController _boardControllerInstance;
        private string _opponentAccountName;
        private int[] _opponentSelectedSkinsIdsForPawns;

        public OpponentPlayerDataInCommand(BoardController boardControllerInstance, MatchLogic matchLogicInstance)
        {
            Debug.Log("OpponentPlayerDataInCommand");
            _boardControllerInstance = boardControllerInstance;
            _matchLogicInstance = matchLogicInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _opponentAccountName = ReadString(networkStream);

            _opponentSelectedSkinsIdsForPawns = new int[ReadShort(networkStream)];
            for (var i = 0; i < _opponentSelectedSkinsIdsForPawns.Length; i++)
                _opponentSelectedSkinsIdsForPawns[i] = ReadInt(networkStream);
        }

        public override void Execute()
        {
            _boardControllerInstance.InstantiatePawnsForOpponent(_opponentSelectedSkinsIdsForPawns);
            _matchLogicInstance.onOpponentSentPlayerData.Invoke();
        }
    }
}