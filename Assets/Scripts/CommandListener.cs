using System;
using Cysharp.Threading.Tasks;
using LoggedInScene.Shop;
using Match;
using Network.Packets.In;
using Network.Packets.In.Commands;
using Network.Packets.Out.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace LoggedInScene
{
    public class CommandListener : MonoBehaviour
    {
        private static CommandListener _singletonInstance = null;
        private bool _listening = true;
        
        //LoggedIn Scene
        private ShopSkinPanelsGenerator _shopSkinPanelsGeneratorInstance;
        private PlayLogic _playLogicInstance;
        
        //Match Scene
        private MatchLogic _matchLogicInstance;
        private BoardController _boardControllerInstance;

        public void StopListening()
        {
            _listening = false;
        }

        private void Awake()
        {
            if (_singletonInstance != null)
            {
                _singletonInstance._listening = true;
                Destroy(this.gameObject);
                return;
            }

            _singletonInstance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (arg0, mode) => InitAllInstances();
        }

        private void InitAllInstances()
        {
            _shopSkinPanelsGeneratorInstance = GameObject.Find("_Logic/_Shop")?.GetComponent<ShopSkinPanelsGenerator>();
            _playLogicInstance = GameObject.Find("_Logic/_Play")?.GetComponent<PlayLogic>();
            
            _matchLogicInstance = GameObject.Find("_Logic/_Match")?.GetComponent<MatchLogic>();
            _boardControllerInstance = GameObject.Find("_Logic")?.GetComponent<BoardController>();
        }

        private async void Start()
        {
            while (_listening)
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
                2 => new UpdatePlayerDataInCommand(_shopSkinPanelsGeneratorInstance),
                //3 => new PlayCompetitiveRequestInCommand(), <-- SERVER
                4 => new MatchFoundInCommand(_playLogicInstance),
                //5 => new MatchFoundResponseInCommand(PlayerData), <-- SERVER
                6 => new StartMatchInCommand(_playLogicInstance),
                7 => new MatchRejectedInCommand(_playLogicInstance),
                9 => new OpponentPlayerDataInCommand(_boardControllerInstance, _matchLogicInstance),
                10 => new MatchRoundStartInCommand(_matchLogicInstance, _boardControllerInstance),
                //11 => new PawnMovesInCommand(), <-- SERVER
                12 => new MatchRoundUpdateInCommand(_matchLogicInstance, _boardControllerInstance), 
                13 => new MatchEndInCommand(_matchLogicInstance),
                //14 => new LogoutRequestInCommand(this), <-- SERVER
                15 => new LogoutInCommand(this),
                _ => null
            };

            await NetworkManager.FillPacketAsync(inCommand);
            inCommand.Execute();
        }
    }
}