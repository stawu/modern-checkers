namespace Warcaby_Server.Network.Packets.Out
{
    public class PlayerDataOutPacket : OutPacket
    {
        public PlayerDataOutPacket(PlayerData playerData)
        {
            InsertValue(playerData.AccountName);
            InsertValue(playerData.Coins);
            
            InsertValue(playerData.OwnedSkinIds.Count);
            foreach (var skinId in playerData.OwnedSkinIds)
                InsertValue(skinId);

            foreach (var skinIdForPawn in playerData.SelectedSkinsIdsForPawns)
                InsertValue(skinIdForPawn);
            
            InsertValue((short) (playerData.DailyRewardClaimed ? 1 : 0));
        }
    }
}