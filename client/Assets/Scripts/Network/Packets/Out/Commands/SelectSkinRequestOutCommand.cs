namespace Network.Packets.Out.Commands
{
    public class SelectSkinRequestOutCommand : OutCommand
    {
        public SelectSkinRequestOutCommand(int skinId, int slot) : base(1)
        {
            InsertValue(skinId);
            InsertValue(slot);
        }
    }
}