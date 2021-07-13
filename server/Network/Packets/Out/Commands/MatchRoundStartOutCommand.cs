namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class MatchRoundStartOutCommand : OutCommand
    {
        public MatchRoundStartOutCommand(bool receiverRoundTurn) : base(10)
        {
            InsertValue((short) (receiverRoundTurn ? 1 : 0));
        }
    }
}