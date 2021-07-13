using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class OptionsLogic : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown windowModeDropdown;
    
    
    public void ApplyOptions()
    {
        Resolution selectedResolution = Screen.resolutions[resolutionDropdown.value];
        FullScreenMode selectedMode = windowModeDropdown.value switch
        {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.MaximizedWindow,
            2 => FullScreenMode.ExclusiveFullScreen,
            3 => FullScreenMode.FullScreenWindow,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, selectedMode, selectedResolution.refreshRate);
        
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    private void OnEnable()
    {
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        foreach (var resolution in Screen.resolutions)
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height + "x" + resolution.refreshRate + "hz", null));
            if (Screen.currentResolution.width == resolution.width && Screen.currentResolution.height == resolution.height && Screen.currentResolution.refreshRate == resolution.refreshRate)
                resolutionDropdown.value = currentResolutionIndex;

            currentResolutionIndex++;
        }
        
        qualityDropdown.ClearOptions();
        foreach (var qualityName in QualitySettings.names)
            qualityDropdown.options.Add(new TMP_Dropdown.OptionData(qualityName, null));
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        
        windowModeDropdown.ClearOptions();
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("Windowed", null));
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("MaximizedWindow", null));
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("ExclusiveFullScreen", null));
        windowModeDropdown.options.Add(new TMP_Dropdown.OptionData("FullScreenWindow", null));
        windowModeDropdown.value = (int) Screen.fullScreenMode;
    }
}
