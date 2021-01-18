using System.Net.Sockets;

namespace Network.Packets.In
{
    public class PlayerDataInPacket : InPacket
    {
        public string AccountName;
        public int Coins;
        public int[] OwnedSkinIds;
        public int[] SelectedSkinsIdsForPawns;
        public bool DailyRewardClaimed;

        public override void FillDataFromStream(NetworkStream networkStream)
        {
            AccountName = ReadString(networkStream);
            Coins = ReadInt(networkStream);

            OwnedSkinIds = new int[ReadInt(networkStream)];
            for (var i = 0; i < OwnedSkinIds.Length; i++)
                OwnedSkinIds[i] = ReadInt(networkStream);

            SelectedSkinsIdsForPawns = new int[12];
            for (var i = 0; i < SelectedSkinsIdsForPawns.Length; i++)
                SelectedSkinsIdsForPawns[i] = ReadInt(networkStream);

            DailyRewardClaimed = ReadShort(networkStream) == 1;
        }
    }
}