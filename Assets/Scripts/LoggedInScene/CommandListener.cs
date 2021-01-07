using System;
using Cysharp.Threading.Tasks;
using LoggedInScene.Shop;
using Network.Packets.In;
using Network.Packets.In.Commands;
using UnityEngine;
using UnityEngine.Serialization;

namespace LoggedInScene
{
    public class CommandListener : MonoBehaviour
    {
        [SerializeField] private ShopSkinPanelsGenerator shopSkinPanelsGeneratorInstance;
        [SerializeField] private PlayLogic playLogicInstance;
        
        private async void Start()
        {
            while (enabled)
                await WaitForCommandAndExecuteIt();
        }

        private async UniTask WaitForCommandAndExecuteIt()
        {
            var commandTypePacket = new CommandTypeInPacket();
            await NetworkManager.FillPacketAsync(commandTypePacket);

            InCommand inCommand = commandTypePacket.CommandId switch
            {
                // 0 => new BuySkinRequestInCommand(this, _playerData), <-- SERVER
                // 1 => new SelectSkinRequestInCommand(_playerData), <-- SERVER
                2 => new UpdatePlayerDataInCommand(shopSkinPanelsGeneratorInstance),
                //3 => new PlayCompetitiveRequestInCommand(), <-- SERVER
                4 => new MatchFoundInCommand(playLogicInstance),
                //5 => new MatchFoundResponseInCommand(PlayerData), <-- SERVER
                6 => new StartMatchInCommand(playLogicInstance),
                7 => new MatchRejectedInCommand(playLogicInstance),
                _ => null
            };

            await NetworkManager.FillPacketAsync(inCommand);
            inCommand.Execute();
        }
    }
}