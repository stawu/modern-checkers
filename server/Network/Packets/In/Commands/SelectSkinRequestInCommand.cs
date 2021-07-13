using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class SelectSkinRequestInCommand : InCommand
    {
        private readonly ClientConnection _clientConnection;
        private readonly PlayerData _playerData;
        private int _skinId;
        private int _slot;

        public SelectSkinRequestInCommand(ClientConnection clientConnection, PlayerData playerData)
        {
            _clientConnection = clientConnection;
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _skinId = ReadInt(networkStream);
            _slot = ReadInt(networkStream);
        }

        public override void Execute()
        {
            if (!_playerData.OwnedSkinIds.Contains(_skinId))//Player don't own skin with received id
                return;
            
            if(_slot < 0 || _slot >= _playerData.SelectedSkinsIdsForPawns.Length)//Value out of scope
                return;

            _playerData.SelectedSkinsIdsForPawns[_slot] = _skinId;
            DatabaseManager.UpdatePlayerDataInDatabase(_playerData);
            
            _clientConnection.SendPacket(new UpdatePlayerDataOutCommand(_playerData));
        }
    }
}