using System.Linq;
using System.Net.Sockets;
using Warcaby_Server.Network.Packets.Out;
using Warcaby_Server.Network.Packets.Out.Commands;

namespace Warcaby_Server.Network.Packets.In.Commands
{
    public class BuySkinRequestInCommand : InCommand
    {
        private ClientConnection _clientConnection;
        private PlayerData _playerData;
        private int _skinId;

        public BuySkinRequestInCommand(ClientConnection clientConnection, PlayerData playerData)
        {
            _clientConnection = clientConnection;
            _playerData = playerData;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _skinId = ReadInt(networkStream);
        }

        public override void Execute()
        {
            SkinOffer skinOffer = DatabaseManager.GetSkinOffersFromDatabase().First(offer => offer.Id == _skinId);

            if (_playerData.OwnedSkinIds.Contains(skinOffer.Id)) //player already own it
                return;

            if (_playerData.Coins < skinOffer.Price) //player don't have enough coins
                return;
            
            _playerData.Coins -= skinOffer.Price;
            _playerData.OwnedSkinIds.Add(skinOffer.Id);
            DatabaseManager.UpdatePlayerDataInDatabase(_playerData);
                
            _clientConnection.SendPacket(new UpdatePlayerDataOutCommand(_playerData));
        }
    }
}