namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class UpdatePlayerDataOutCommand : OutCommand
    {
        public UpdatePlayerDataOutCommand(PlayerData playerData) : base(2)
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