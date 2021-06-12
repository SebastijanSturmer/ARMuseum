using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARUIManager : MonoBehaviour
{
    [Header("Internal references")]
    [SerializeField] private ARMainMenuPanelController _arMainMenuPanelController;
    [SerializeField] private ARAnimalInfoPanelController _arAnimalInfoPanelController;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _loadMainMenu;
    [SerializeField] private ScriptableEvent _exitAR;

    private AnimalData _currentAnimal;

    private void Start()
    {
        _arMainMenuPanelController.TogglePanel(true);
        _arAnimalInfoPanelController.TogglePanel(false, true, false);
    }
    
    /// <summary>
    /// Exits AR and opens main menu panel
    /// </summary>
    public void OnReturnToMainMenuButtonPressed()
    {
        _exitAR.RaiseEvent();
        _arMainMenuPanelController.TogglePanel(true);
        _arAnimalInfoPanelController.TogglePanel(false, true, false);
    }

    /// <summary>
    /// Event function that is triggered when AR managers finished loading and AR starts
    /// </summary>
    public void OnARStarted()
    {
        _arMainMenuPanelController.TogglePanel(false);
    }

    /// <summary>
    /// Event function that responds to animal focused event from AR Manager. It will send info data about that animal to info panel
    /// </summary>
    /// <param name="animalDataMessage"></param>
    public void OnAnimalFocused(EventMessage animalDataMessage)
    {
        _currentAnimal = ((AnimalDataMessage)animalDataMessage).AnimalData;

        DataManager.Instance.RequestAnimalImageAndMapByIdentifier(_currentAnimal.AnimalIdentifier);
    }

    public void OnAnimalImageAndMapRecieved(EventMessage message)
    {
        AnimalImageAndMapMessage imageAndMapMessage = (AnimalImageAndMapMessage)message;

        _arAnimalInfoPanelController.SetAnimalData(_currentAnimal);
        _arAnimalInfoPanelController.UpdateInfo(imageAndMapMessage.Image, imageAndMapMessage.Map);
        _arAnimalInfoPanelController.TogglePanel(true);
    }

}
