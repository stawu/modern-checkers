namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class MatchEndOutCommand : OutCommand
    {
        public MatchEndOutCommand(bool matchWon) : base(13)
        {
            InsertValue((short) (matchWon ? 1 : 0));
        }
    }
}