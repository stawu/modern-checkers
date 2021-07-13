using System.Net.Sockets;
using LoggedInScene;
using UnityEngine.SceneManagement;

namespace Network.Packets.In.Commands
{
    public class LogoutInCommand : InCommand
    {
        private CommandListener _commandListenerInstance;
        
        public LogoutInCommand(CommandListener commandListenerInstance)
        {
            _commandListenerInstance = commandListenerInstance;
        }

        public override void FillDataFromStream(NetworkStream networkStream)
        {
        }

        public override void Execute()
        {
            _commandListenerInstance.StopListening();
            NetworkManager.CloseConnectionToServer();
            SceneManager.LoadScene("Login");
        }
    }
}