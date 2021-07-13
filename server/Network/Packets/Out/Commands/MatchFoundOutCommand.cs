namespace Warcaby_Server.Network.Packets.Out.Commands
{
    public class MatchFoundOutCommand : OutCommand
    {
        public MatchFoundOutCommand(int matchId) : base(4)
        {
            InsertValue(matchId);
        }
    }
}