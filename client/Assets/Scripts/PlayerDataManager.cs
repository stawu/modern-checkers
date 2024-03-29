﻿
using Network.Packets.In;

public static class PlayerDataManager
{
    public static string AccountName { get; private set; }
    public static int Coins { get; private set; }
    public static int[] OwnedSkinIds { get; private set; }
    public static int[] SelectedSkinsIdsForPawns { get; private set; }
    public static bool DailyRewardClaimed { get; private set; }

    public static void UpdateDataFromPlayerDataPacket(PlayerDataInPacket packet)
    {
        AccountName = packet.AccountName;
        Coins = packet.Coins;
        OwnedSkinIds = packet.OwnedSkinIds;
        SelectedSkinsIdsForPawns = packet.SelectedSkinsIdsForPawns;
        DailyRewardClaimed = packet.DailyRewardClaimed;
    }
}