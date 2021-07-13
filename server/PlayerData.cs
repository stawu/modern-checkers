using System;
using System.Collections.Generic;

namespace Warcaby_Server
{
    public class ServerOnlyData
    {
        public bool LoggedIn = false;
        public bool AcceptedMatchInvitation = false;
        public Match PlayingMatch = null;
    }
    public class PlayerData
    {
        public ServerOnlyData TempServerOnlyData;
        public string AccountName;
        public int Coins;
        public List<int> OwnedSkinIds;
        public int[] SelectedSkinsIdsForPawns;
        public DateTime LastLoginDate;
        public bool DailyRewardClaimed = true;

        public PlayerData()
        {
            TempServerOnlyData = new ServerOnlyData();
        }
    }
}