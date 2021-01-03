using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Network.Packets.In;
using Skins;
using UnityEngine;

public static class SkinsManager
{
    public static Skin[] Skins { get; private set; }
    
    private static SkinOffer[] _skinsOffers;

    public static void LoadAllSkins()
    {
        Skins = Resources.LoadAll<Skin>("Skins/");
    }
    
    public static void UpdateOffersFromSkinOffersPacket(SkinOffersInPacket packet)
    {
        _skinsOffers = packet.SkinOffers;
    }

    public static SkinOffer GetOfferById(int id)
    {
        return _skinsOffers.First(offer => offer.Id == id);
    }
}
