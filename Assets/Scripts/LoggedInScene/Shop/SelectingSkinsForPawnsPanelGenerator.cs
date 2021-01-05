using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LoggedInScene.Shop
{
    public class SelectingSkinsForPawnsPanelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject selectedSkinForPawnPrefab;
        [SerializeField] private GameObject panel;
        [SerializeField] private SkinSelectionLogic skinSelectionLogicInstance;
        private Image[] _skinImages;
        private Text[] _skinNames;
        private Button[] _skinSelectButtons;
        
        public void ShowSelectionPanelAndUpdateContent(int newSkinId)
        {
            for (var i = 0; i < PlayerDataManager.SelectedSkinsIdsForPawns.Length; i++)
            {
                var skinData = SkinsManager.Skins.First(skin => skin.id == PlayerDataManager.SelectedSkinsIdsForPawns[i]);
                
                _skinImages[i].sprite = skinData.image;
                _skinNames[i].text = skinData.skinName;

                int slot = i;
                _skinSelectButtons[i].onClick.RemoveAllListeners();
                _skinSelectButtons[i].onClick.AddListener(() =>
                {
                    panel.SetActive(false);
                    skinSelectionLogicInstance.TryToSelectSkin(newSkinId, slot);
                });
            }
            
            panel.SetActive(true);
        }

        private void Awake()
        {
            GenerateSelectionPanelGameObjects();
        }

        private void GenerateSelectionPanelGameObjects()
        {
            _skinImages = new Image[PlayerDataManager.SelectedSkinsIdsForPawns.Length];
            _skinNames = new Text[PlayerDataManager.SelectedSkinsIdsForPawns.Length];
            _skinSelectButtons = new Button[PlayerDataManager.SelectedSkinsIdsForPawns.Length];
        
            for (var i = 0; i < PlayerDataManager.SelectedSkinsIdsForPawns.Length; i++)
            {
                Transform skinTransform = Instantiate(selectedSkinForPawnPrefab, panel.transform).transform;
                _skinImages[i] = skinTransform.Find("Image").GetComponent<Image>();
                _skinNames[i] = skinTransform.Find("NameText").GetComponent<Text>();
                _skinSelectButtons[i] = skinTransform.Find("SelectButton").GetComponent<Button>();
            }
        }
    }
}
