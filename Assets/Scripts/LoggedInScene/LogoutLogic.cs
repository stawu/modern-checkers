using Network.Packets.Out.Commands;
using UnityEngine;

namespace LoggedInScene
{
    public class LogoutLogic : MonoBehaviour
    {
        public void Logout()
        {
            NetworkManager.SendPacket(new LogoutRequestOutCommand());
        }
    }
}
