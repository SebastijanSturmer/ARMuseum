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
    [SerializeField] private Image _animalImage;
    [SerializeField] private Button _openBasicInfoButton;
    [SerializeField] private Button _openShortInfoButton;
    [SerializeField] private Button _openMapInfoButton;
    [SerializeField] private Button _closePanelButton;

    [SerializeField] private ARBasicAnimalInfoUIController _arBasicAnimalInfoUIController;
    [SerializeField] private ARShortAnimalInfoUIController _arShortAnimalInfoUIController;
    [SerializeField] private ARAnimalMapInfoUIController _arAnimalMapInfoUIController;

    private AnimalData _animalData;
    private Enums.AnimalInfoPanels _activePanel = Enums.AnimalInfoPanels.None;

    private void Start()
    {
        AddOnClickEventsToButtons();
    }

    public override void TogglePanel(bool shouldDisplay, bool shouldBypassAnimation = false, bool shouldDeactivatePanelAfterClose = true)
    {
        base.TogglePanel(shouldDisplay, shouldBypassAnimation, shouldDeactivatePanelAfterClose);

        //If we are opening info panel then we should have all additional info panels closed and we should activate/deactivate close button!
        if (shouldDisplay)
        {
            ForceCloseAdditionalInfoPanels();
        }
        else
        {
            CloseAdditionalInfoPanels(); //If we are closing panel then we should also close additional panels
        }

        _closePanelButton.gameObject.SetActive(shouldDisplay);
        _animalImage.gameObject.SetActive(shouldDisplay);
    }

    /// <summary>
    /// Sets animal data to new animal data and updates info about that animal.
    /// </summary>
    /// <param name="newAnimalData"></param>
    public void SetAnimalData(AnimalData newAnimalData)
    {
        _animalData = newAnimalData;
    }
    
    /// <summary>
    /// Function that will toggle specific panel. If that panel is already opened then it will close.
    /// </summary>
    /// <param name="panel"></param>
    public void ToggleAdditionalInfoPanel(Enums.AnimalInfoPanels panel)
    {
        //If this panel is active one then close it!
        if (_activePanel == panel)
        {
            CloseAdditionalInfoPanels();
            return;
        }

        switch(panel)
        {
            case Enums.AnimalInfoPanels.BasicInfo:
                _arBasicAnimalInfoUIController.TogglePanel(true);
                _arShortAnimalInfoUIController.TogglePanel(false);
                _arAnimalMapInfoUIController.TogglePanel(false);
                _activePanel = Enums.AnimalInfoPanels.BasicInfo;
                break;
            case Enums.AnimalInfoPanels.ShortInfo:
                _arBasicAnimalInfoUIController.TogglePanel(false);
                _arShortAnimalInfoUIController.TogglePanel(true);
                _arAnimalMapInfoUIController.TogglePanel(false);
                _activePanel = Enums.AnimalInfoPanels.ShortInfo;
                break;
            case Enums.AnimalInfoPanels.MapInfo:
                _arBasicAnimalInfoUIController.TogglePanel(false);
                _arShortAnimalInfoUIController.TogglePanel(false);
                _arAnimalMapInfoUIController.TogglePanel(true);
                _activePanel = Enums.AnimalInfoPanels.MapInfo;
                break;
        }
    }

    /// <summary>
    /// Closes all panels with animation
    /// </summary>
    public void CloseAdditionalInfoPanels()
    {
        _arBasicAnimalInfoUIController.TogglePanel(false, false);
        _arShortAnimalInfoUIController.TogglePanel(false, false);
        _arAnimalMapInfoUIController.TogglePanel(false, false);

        _activePanel = Enums.AnimalInfoPanels.None;
    }

    /// <summary>
    /// Closes all panels without animation
    /// </summary>
    public void ForceCloseAdditionalInfoPanels()
    {
        _arBasicAnimalInfoUIController.TogglePanel(false, true);
        _arShortAnimalInfoUIController.TogglePanel(false, true);
        _arAnimalMapInfoUIController.TogglePanel(false, true);

        _activePanel = Enums.AnimalInfoPanels.None;
    }
    
    private void AddOnClickEventsToButtons()
    {
        _openBasicInfoButton.onClick.AddListener(delegate { ToggleAdditionalInfoPanel(Enums.AnimalInfoPanels.BasicInfo); });
        _openShortInfoButton.onClick.AddListener(delegate { ToggleAdditionalInfoPanel(Enums.AnimalInfoPanels.ShortInfo); });
        _openMapInfoButton.onClick.AddListener(delegate { ToggleAdditionalInfoPanel(Enums.AnimalInfoPanels.MapInfo); });
        _closePanelButton.onClick.AddListener(delegate { TogglePanel(false,false,false); });
    }

    /// <summary>
    /// Updates info about that animal for all additional panels aswell as animal name text. Requeires additional image and map for that animal
    /// </summary>
    public void UpdateInfo(Sprite image, Sprite map)
    {
        switch (LocalizationManager.Instance.Language)
        {
            case Enums.Language.English:
                _animalName.text = _animalData.AnimalNameEN;
                _arBasicAnimalInfoUIController.UpdateInfo(_animalData.BasicInfoEN);
                _arShortAnimalInfoUIController.UpdateInfo(_animalData.ShortInfoEN);
                
                break;
            case Enums.Language.Croatian:
                _animalName.text = _animalData.AnimalNameHR;
                _arBasicAnimalInfoUIController.UpdateInfo(_animalData.BasicInfoHR);
                _arShortAnimalInfoUIController.UpdateInfo(_animalData.ShortInfoHR);
                break;
        }

        _animalImage.sprite = image;
        _arAnimalMapInfoUIController.UpdateInfo(map);

    }

}
