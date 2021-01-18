using System;
using Network.Packets.Out.Commands;
using UnityEngine;

namespace LoggedInScene
{
    public class DailyRewardLogic : MonoBehaviour
    {
        [SerializeField] private GameObject dailyRewardPanel;

        public void ClaimDailyReward()
        {
            NetworkManager.SendPacket(new ClaimDailyRewardOutCommand());
        }
        
        private void Start()
        {
            dailyRewardPanel.SetActive(!PlayerDataManager.DailyRewardClaimed);
        }
    }
}
