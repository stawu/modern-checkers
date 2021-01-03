using Network.Packets.Out.Commands;
using UnityEngine;

namespace LoggedInScene.Shop
{
    public class BuyLogic : MonoBehaviour
    {
        public void TryToBuySkin(int skinId)
        {
            Debug.Log("Trying to buy skin with id: " + skinId);
            
            if (PlayerDataManager.Coins < SkinsManager.GetOfferById(skinId).Price)
            {
                Debug.LogWarning("Not enough coins!");
                return;
            }
            
            NetworkManager.SendPacket(new BuySkinRequestOutCommand(skinId));
            Debug.Log("Request sended: " + skinId);
        }
    }
}
