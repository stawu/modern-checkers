using Network.Packets.In;
using Network.Packets.Out;
using UnityEngine;

namespace LoginScene
{
    public class LoginLogic : MonoBehaviour
    {
        private string _accountName;
        private string _accountPassword;

        public void ChangeCachedAccountName(string newAccountName)
        {
            _accountName = newAccountName;
        }
    
        public void ChangeCachedAccountPassword(string newAccountPassword)
        {
            _accountPassword = newAccountPassword;
        }
    
        public void TryToLogin()
        {
            string hashedPassword = _accountPassword;//todo hash
            NetworkManager.SendPacket(new LoginRequestOutPacket(_accountName, hashedPassword));

            var loginResponsePacket = new LoginResponseInPacket();
            NetworkManager.FillPacket(loginResponsePacket);
            
            Debug.Log("Login response: " + loginResponsePacket.testVal);
        }
    }
}
