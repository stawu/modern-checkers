namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class OpponentPlayerDataOutCommand : OutCommand
    {
        public OpponentPlayerDataOutCommand(string accountName, int[] selectedSkinsIdsForPawns) : base(9)
        {
            InsertValue(accountName);
            
            InsertValue((short)selectedSkinsIdsForPawns.Length);
            foreach (var skinIdForPawn in selectedSkinsIdsForPawns)
                InsertValue(skinIdForPawn);
        }
    }
}