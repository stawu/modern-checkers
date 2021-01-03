using System.Net.Sockets;
using Skins;

namespace Network.Packets.In
{
    public class SkinOffersInPacket : InPacket
    {
        public SkinOffer[] SkinOffers;
        
        public override void FillDataFromStream(NetworkStream networkStream)
        {
            SkinOffers = new SkinOffer[ReadInt(networkStream)];
            for (var i = 0; i < SkinOffers.Length; i++)
            {
                SkinOffers[i].Id = ReadInt(networkStream);
                SkinOffers[i].Price = ReadInt(networkStream);
            }
        }
    }
}