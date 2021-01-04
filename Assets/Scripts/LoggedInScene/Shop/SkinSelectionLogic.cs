using Network.Packets.Out.Commands;
using UnityEngine;

namespace LoggedInScene.Shop
{
    public class SkinSelectionLogic : MonoBehaviour
    {
        public void TryToSelectSkin(int skinId, int slot)
        {
            Debug.Log("Trying to select skin with id: " + skinId + " in slot: " + slot);

            NetworkManager.SendPacket(new SelectSkinRequestOutCommand(skinId, slot));
            Debug.Log("Request sended");
        } 
    }
}
