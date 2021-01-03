namespace Network.Packets.Out.Commands
{
    public class BuySkinRequestOutCommand : OutCommand
    {
        public BuySkinRequestOutCommand(int skinId) : base(0)
        {
            InsertValue(skinId);
        }
    }
}