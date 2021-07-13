using System;
using System.Linq;
using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class MatchFoundResponseInCommand : InCommand
    {
        private enum ResponseType : short
        {
            Accept = 0,
            Reject = 1
        }

        private ResponseType _responseType;
        private int _matchId;
        private PlayerData _playerData;

        public MatchFoundResponseInCommand(PlayerData playerData)
        {
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _responseType = (ResponseType) ReadShort(networkStream);
            _matchId = ReadInt(networkStream);
        }

        public override void Execute()
        {
            Match match = MatchmakingManager.TryGetMatch(_matchId);
            if(match == null)
                return;

            if (!match.PlayersNames.Contains(_playerData.AccountName))
                throw new Exception("???");

            int playerMatchIndex = match.PlayersNames[0] == _playerData.AccountName ? 0 : 1;
            int opponentMatchIndex = playerMatchIndex == 0 ? 1 : 0;
            
            if (_responseType == ResponseType.Accept)
            {
                match.PlayersData[playerMatchIndex].TempServerOnlyData.AcceptedMatchInvitation = true;
                if (match.PlayersData[opponentMatchIndex].TempServerOnlyData.AcceptedMatchInvitation)
                {
                    match.PlayersData[playerMatchIndex].TempServerOnlyData.PlayingMatch = match;
                    match.PlayersData[opponentMatchIndex].TempServerOnlyData.PlayingMatch = match;
                    
                    match.PlayersConnections[playerMatchIndex].RequestToSendCommand(new StartMatchOutCommand());
                    match.PlayersConnections[opponentMatchIndex].RequestToSendCommand(new StartMatchOutCommand());
                }
            }
            else if (_responseType == ResponseType.Reject)
            {
                match.PlayersConnections[opponentMatchIndex].RequestToSendCommand(new MatchRejectedOutCommand());
            }
        }
    }
}