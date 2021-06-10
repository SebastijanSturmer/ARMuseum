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

    private AnimalData _currentAnimal;

    private void Start()
    {
        _arMainMenuPanelController.TogglePanel(true);
        _arAnimalInfoPanelController.TogglePanel(false, true);
    }

    public void OnReturnToMainMenuButtonPressed()
    {
        _loadMainMenu.RaiseEvent();
    }

    public void OnAnimalDetected(EventMessage animalDataMessage)
    {
        _currentAnimal = ((AnimalDataMessage)animalDataMessage).AnimalData;

        _arAnimalInfoPanelController.SetAnimalData(_currentAnimal);

        _arAnimalInfoPanelController.TogglePanel(true);
    }

}
