using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ARAnimalInfoPanelController : AnimatedBaseGUIController
{
    
    [Header("Internal references")]
    [SerializeField] private TextMeshProUGUI _animalName;
    [SerializeField] private Button _openBasicInfoButton;
    [SerializeField] private Button _openShortInfoButton;
    [SerializeField] private Button _openMapInfoButton;

    [SerializeField] private ARBasicAnimalInfoUIController _arBasicAnimalInfoUIController;
    [SerializeField] private ARShortAnimalInfoUIController _arShortAnimalInfoUIController;
    [SerializeField] private ARAnimalMapInfoUIController _arAnimalMapInfoUIController;

    private AnimalData _animalData;

    private void Start()
    {
        AddOnClickEventsToInfoButtons();
    }

    public override void TogglePanel(bool shouldDisplay, bool shouldBypassAnimation = false)
    {
        base.TogglePanel(shouldDisplay, shouldBypassAnimation);

        //If we are opening info panel then we should have all additional info panels closed!
        if (shouldDisplay)
            ForceCloseAdditionalInfoPanel();
    }

    public void SetAnimalData(AnimalData newAnimalData)
    {
        _animalData = newAnimalData;

        UpdateInfo();
    }
    
    public void OpenAdditionalInfoPanel(Enums.AnimalInfoPanels panel)
    {
        switch(panel)
        {
            case Enums.AnimalInfoPanels.BasicInfo:
                _arBasicAnimalInfoUIController.TogglePanel(true);
                _arShortAnimalInfoUIController.TogglePanel(false);
                _arAnimalMapInfoUIController.TogglePanel(false);
                break;
            case Enums.AnimalInfoPanels.ShortInfo:
                _arBasicAnimalInfoUIController.TogglePanel(false);
                _arShortAnimalInfoUIController.TogglePanel(true);
                _arAnimalMapInfoUIController.TogglePanel(false);
                break;
            case Enums.AnimalInfoPanels.MapInfo:
                _arBasicAnimalInfoUIController.TogglePanel(false);
                _arShortAnimalInfoUIController.TogglePanel(false);
                _arAnimalMapInfoUIController.TogglePanel(true);
                break;
        }
    }

    public void ForceCloseAdditionalInfoPanel()
    {
        _arBasicAnimalInfoUIController.TogglePanel(false, true);
        _arShortAnimalInfoUIController.TogglePanel(false, true);
        _arAnimalMapInfoUIController.TogglePanel(false, true);
    }

    private void AddOnClickEventsToInfoButtons()
    {
        _openBasicInfoButton.onClick.AddListener(delegate { OpenAdditionalInfoPanel(Enums.AnimalInfoPanels.BasicInfo); });
        _openShortInfoButton.onClick.AddListener(delegate { OpenAdditionalInfoPanel(Enums.AnimalInfoPanels.ShortInfo); });
        _openMapInfoButton.onClick.AddListener(delegate { OpenAdditionalInfoPanel(Enums.AnimalInfoPanels.MapInfo); });
    }

    private void UpdateInfo()
    {
        _animalName.text = _animalData.AnimalName;
        _arBasicAnimalInfoUIController.UpdateInfo(_animalData.BasicInfo);
        _arShortAnimalInfoUIController.UpdateInfo(_animalData.ShortInfo);
        _arAnimalMapInfoUIController.UpdateInfo();
    }

}
