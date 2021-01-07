namespace Network.Packets.Out.Commands
{
    public class MatchFoundResponseOutCommand : OutCommand
    {
        public enum ResponseType : short
        {
            Accept = 0,
            Reject = 1
        }
        
        public MatchFoundResponseOutCommand(ResponseType responseType, int matchId) : base(5)
        {
            InsertValue((short) responseType);
            InsertValue(matchId);
        }
    }
}