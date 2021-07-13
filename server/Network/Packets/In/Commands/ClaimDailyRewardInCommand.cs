using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class ClaimDailyRewardInCommand : InCommand
    {
        private ClientConnection _clientConnection;
        private PlayerData _playerData;
        
        public ClaimDailyRewardInCommand(ClientConnection clientConnection, PlayerData playerData)
        {
            _clientConnection = clientConnection;
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            if(_playerData.DailyRewardClaimed)
                return;

            _playerData.DailyRewardClaimed = true;
            _playerData.Coins += 300;
            
            _clientConnection.SendPacket(new UpdatePlayerDataOutCommand(_playerData));
            DatabaseManager.UpdatePlayerDataInDatabase(_playerData);
        }
    }
}