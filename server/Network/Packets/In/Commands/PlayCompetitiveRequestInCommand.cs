using System;
using System.Net.Sockets;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class PlayCompetitiveRequestInCommand : InCommand
    {
        private readonly PlayerData _playerData;
        private readonly ClientConnection _playerConnection;

        public PlayCompetitiveRequestInCommand(PlayerData playerData, ClientConnection playerConnection)
        {
            _playerConnection = playerConnection;
            _playerData = playerData;
        }
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            
        }

        public override void Execute()
        {
            if(_playerData.TempServerOnlyData.AcceptedMatchInvitation || _playerData.TempServerOnlyData.PlayingMatch != null)
                throw new Exception("Impossible condition");
                
            MatchmakingManager.AddPlayerToQueue(_playerData.AccountName, _playerConnection);
        }
    }
}