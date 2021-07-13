using System.Net.Sockets;
using LoggedInScene.Shop;

namespace Network.Packets.In.Commands
{
    public class UpdatePlayerDataInCommand : InCommand
    {

        private readonly PlayerDataInPacket _playerDataPacket = new PlayerDataInPacket();
        private readonly ShopSkinPanelsGenerator _shopSkinPanelsGeneratorInstance;

        public UpdatePlayerDataInCommand(ShopSkinPanelsGenerator shopSkinPanelsGeneratorInstance)
        {
            _shopSkinPanelsGeneratorInstance = shopSkinPanelsGeneratorInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            _playerDataPacket.FillDataFromStream(networkStream);
        }

        public override void Execute()
        {
            PlayerDataManager.UpdateDataFromPlayerDataPacket(_playerDataPacket);
            _shopSkinPanelsGeneratorInstance.UpdateContentOfSkinsPanels();
        }
    }
}