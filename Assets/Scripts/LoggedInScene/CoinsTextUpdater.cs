using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsTextUpdater : MonoBehaviour
{
    [SerializeField] private Text textToUpdate;
    
    private void Update()
    {
        textToUpdate.text = PlayerDataManager.Coins.ToString();
    }
}
