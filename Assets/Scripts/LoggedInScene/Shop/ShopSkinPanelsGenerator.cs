using System.Linq;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace LoggedInScene.Shop
{
    public class ShopSkinPanelsGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject skinPanelPrefab;
        [SerializeField] private RectTransform parentToSkinPanels;
        [SerializeField] private BuyLogic buyLogicInstance;
        [SerializeField] private SelectingSkinsForPawnsPanelGenerator selectingSkinsForPawnsPanelGeneratorInstance;
        
        private Image[] _skinPanelImages;
        private Text[] _skinPanelNameTexts;
        private Text[] _skinPanelPriceTexts;
        private Button[] _skinPanelUnlockButtons; 
        private Button[] _skinPanelSelectButtons;

        public void UpdateContentOfSkinsPanels()
        {
            var numberOfSkins =  SkinsManager.Skins.Length;

            for (var i = 0; i < numberOfSkins; i++)
            {
                var skin = SkinsManager.Skins[i];
                _skinPanelImages[i].sprite = skin.image;
                _skinPanelNameTexts[i].text = skin.skinName;
                
                _skinPanelUnlockButtons[i].onClick.RemoveAllListeners();
                _skinPanelUnlockButtons[i].onClick.AddListener(() => buyLogicInstance.TryToBuySkin(skin.id));
                
                _skinPanelSelectButtons[i].onClick.RemoveAllListeners();
                _skinPanelSelectButtons[i].onClick.AddListener(() => selectingSkinsForPawnsPanelGeneratorInstance.ShowSelectionPanelAndUpdateContent(skin.id));

                bool skinOwned = PlayerDataManager.OwnedSkinIds.Contains(skin.id);
                
                _skinPanelPriceTexts[i].text = skinOwned ? "Owned" : SkinsManager.GetOfferById(skin.id).Price.ToString();
                _skinPanelUnlockButtons[i].gameObject.SetActive(!skinOwned);
                _skinPanelSelectButtons[i].gameObject.SetActive(skinOwned);
            }
        }
        
        private void Awake()
        {
            SkinsManager.LoadAllSkins();
            GenerateSkinPanelsGameObjects();
            UpdateContentOfSkinsPanels();
        }

        private void GenerateSkinPanelsGameObjects()
        {
            var numberOfSkins =  SkinsManager.Skins.Length;
            _skinPanelImages = new Image[numberOfSkins];
            _skinPanelNameTexts = new Text[numberOfSkins];
            _skinPanelPriceTexts = new Text[numberOfSkins];
            _skinPanelUnlockButtons = new Button[numberOfSkins];
            _skinPanelSelectButtons = new Button[numberOfSkins];
            
            for (var i=0; i<numberOfSkins; i++)
            {
                var instantiatedSkinPanelGameObject = Instantiate(skinPanelPrefab, parentToSkinPanels);
                _skinPanelImages[i] = instantiatedSkinPanelGameObject.transform.Find("Image").GetComponent<Image>();
                _skinPanelNameTexts[i] = instantiatedSkinPanelGameObject.transform.Find("NameText").GetComponent<Text>();
                _skinPanelPriceTexts[i] = instantiatedSkinPanelGameObject.transform.Find("PriceText").GetComponent<Text>();
                _skinPanelUnlockButtons[i] = instantiatedSkinPanelGameObject.transform.Find("UnlockButton").GetComponent<Button>();
                _skinPanelSelectButtons[i] = instantiatedSkinPanelGameObject.transform.Find("SelectButton").GetComponent<Button>();
            }
        }
    }
}
