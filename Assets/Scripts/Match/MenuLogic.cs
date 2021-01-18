using Network.Packets.Out.Commands;
using UnityEngine;

namespace Match
{
    public class MenuLogic : MonoBehaviour
    {
        public void Surrender()
        {
            NetworkManager.SendPacket(new SurrenderRequestOutCommand());
        }

        public void QuitGame()
        {
            NetworkManager.SendPacket(new LogoutRequestOutCommand());
            Application.Quit();
        }
    }
}
