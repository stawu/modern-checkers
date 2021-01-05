using System.Collections;
using Network.Packets.In;
using Network.Packets.Out;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace LoginScene
{
    public class LoginLogic : MonoBehaviour
    {
        public UnityEvent onSuccessfulLoginResponse;
        public UnityEvent<string> onNegativeLoginResponse;

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
            if(!NetworkManager.ConnectedToServer)
                return;
            
            string hashedPassword = _accountPassword;//todo hash
            NetworkManager.SendPacket(new LoginRequestOutPacket(_accountName, hashedPassword));

            var loginResponsePacket = new LoginResponseInPacket();
            NetworkManager.FillPacket(loginResponsePacket);

            Debug.Log("Login response: " + loginResponsePacket.Status);
            if (loginResponsePacket.Status == LoginResponseInPacket.LoginStatus.Successful)
            {
                onSuccessfulLoginResponse.Invoke();

                var skinOffersPacket = new SkinOffersInPacket();
                NetworkManager.FillPacket(skinOffersPacket);
                SkinsManager.UpdateOffersFromSkinOffersPacket(skinOffersPacket);
                
                var playerDataPacket = new PlayerDataInPacket();
                NetworkManager.FillPacket(playerDataPacket);
                PlayerDataManager.UpdateDataFromPlayerDataPacket(playerDataPacket);

                StartCoroutine(PlayLoggedInSequence());
            }
            else if(loginResponsePacket.Status == LoginResponseInPacket.LoginStatus.Negative)
                onNegativeLoginResponse.Invoke(loginResponsePacket.ResponseText);
            else
                onNegativeLoginResponse.Invoke("Corrupted network packet");
        }

        private IEnumerator PlayLoggedInSequence()
        {
            //todo hardcoded code
            yield return new WaitForSeconds(3);
            GameObject.FindObjectOfType<Canvas>().gameObject.SetActive(false);
            Camera.main.transform.GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("LoggedIn", LoadSceneMode.Single);
        }
    }
}
