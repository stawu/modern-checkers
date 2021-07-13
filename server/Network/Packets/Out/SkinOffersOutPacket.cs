namespace Warcaby_Server.Network.Packets.Out
{
    public class SkinOffersOutPacket : OutPacket
    {
        public SkinOffersOutPacket(SkinOffer[] skinOffers)
        {
            InsertValue(skinOffers.Length);
            foreach (var offer in skinOffers)
            {
                InsertValue(offer.Id);
                InsertValue(offer.Price);
            }
        }
    }
}