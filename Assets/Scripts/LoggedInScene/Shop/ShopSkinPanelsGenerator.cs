using System;
using System.Linq;
using JetBrains.Annotations;
using LoggedInScene.Shop;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace LoggedInScene
{
    public class ShopSkinPanelsGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject skinPanelPrefab;
        [SerializeField] private RectTransform parentToSkinPanels;
        [SerializeField] private BuyLogic buyLogicInstance;

        private void Awake()
        {
            SkinsManager.LoadAllSkins();
            GenerateSkinPanelsGameObjects();
        }

        private void GenerateSkinPanelsGameObjects()
        {
            foreach (var skin in SkinsManager.Skins)
            {
                var instantiatedSkinPanelGameObject = Instantiate(skinPanelPrefab, parentToSkinPanels);
                var panelImage = instantiatedSkinPanelGameObject.transform.Find("Image").GetComponent<Image>();
                var panelNameText = instantiatedSkinPanelGameObject.transform.Find("NameText").GetComponent<Text>();
                var panelPriceText = instantiatedSkinPanelGameObject.transform.Find("PriceText").GetComponent<Text>();
                var unlockButton = instantiatedSkinPanelGameObject.transform.Find("UnlockButton").GetComponent<Button>();

                panelImage.sprite = skin.image;
                panelNameText.text = skin.skinName;

                panelPriceText.text = PlayerDataManager.OwnedSkinIds.Contains(skin.id) ? "Owned" : SkinsManager.GetOfferById(skin.id).Price.ToString();
                
                unlockButton.onClick.AddListener(() => buyLogicInstance.TryToBuySkin(skin.id));
            }
        }
    }
}
