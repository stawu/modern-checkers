using System.Net.Sockets;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class CancelCompetitiveRequestInCommand : InCommand
    {
        private PlayerData _playerData;
        
        public CancelCompetitiveRequestInCommand(PlayerData playerData)
        {
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            if(_playerData.TempServerOnlyData.PlayingMatch != null)
                return;

            MatchmakingManager.RemovePlayerFromQueue(_playerData.AccountName);
        }
    }
}