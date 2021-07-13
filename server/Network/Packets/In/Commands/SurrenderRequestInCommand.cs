using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class SurrenderRequestInCommand : InCommand
    {
        private ClientConnection _playerConnection;
        private PlayerData _playerData;
        
        public SurrenderRequestInCommand(ClientConnection playerConnection, PlayerData playerData)
        {
            _playerConnection = playerConnection;
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            Match match = _playerData.TempServerOnlyData.PlayingMatch;
            
            if(match == null)
                return;
            
            int playerIndex = match.PlayersNames[0] == _playerData.AccountName ? 0 : 1;
            int opponentIndex = playerIndex == 1 ? 0 : 1;
            
            match.GameEnded = true;
            
            _playerConnection.SendPacket(new MatchEndOutCommand(false));
            match.PlayersConnections[opponentIndex].RequestToSendCommand(new MatchEndOutCommand(true));

            match.PlayersData[opponentIndex].TempServerOnlyData.AcceptedMatchInvitation = false;
            _playerData.TempServerOnlyData.AcceptedMatchInvitation = false;
                
            match.PlayersData[opponentIndex].TempServerOnlyData.PlayingMatch = null;
            _playerData.TempServerOnlyData.PlayingMatch = null;
        }
    }
}