using System;
using System.Linq;
using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class MatchSceneLoadedInCommand : InCommand
    {
        private ClientConnection _playerConnection;
        private PlayerData _playerData;
        
        public MatchSceneLoadedInCommand(ClientConnection playerConnection, PlayerData playerData)
        {
            _playerConnection = playerConnection;
            _playerData = playerData;
        }
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            Match playerMatch = _playerData.TempServerOnlyData.PlayingMatch;
            int playerIndex = playerMatch.PlayersNames[0] == _playerData.AccountName ? 0 : 1;
            int opponentIndex = playerIndex == 1 ? 0 : 1;
            
            _playerConnection.SendPacket(new OpponentPlayerDataOutCommand(playerMatch.PlayersData[opponentIndex].AccountName, playerMatch.PlayersData[opponentIndex].SelectedSkinsIdsForPawns));
            playerMatch.PlayersReadyFlag[playerIndex] = true;

            if (playerMatch.PlayersReadyFlag.All(playerReady => playerReady == true))
            {
                playerMatch.CurrentPlayerIndexTurn = new Random().Next(2);
                playerMatch.ResetPawns(playerMatch.CurrentPlayerIndexTurn);

                bool playerTurn = playerIndex == playerMatch.CurrentPlayerIndexTurn;
                _playerConnection.SendPacket(new MatchRoundStartOutCommand(playerTurn));
                playerMatch.PlayersConnections[opponentIndex].RequestToSendCommand(new MatchRoundStartOutCommand(!playerTurn));
            }
        }
    }
}