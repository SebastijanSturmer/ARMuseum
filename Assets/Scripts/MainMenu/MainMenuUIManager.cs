using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : BaseGUIController
{
    [Header("Internal References")]
    [SerializeField] private SettingsUIController _settingsUIController;
    [SerializeField] private Button _arButton;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _loadQuiz;
    [SerializeField] private ScriptableEvent _loadAR;


    void Start()
    {
        TogglePanel(true);
        _settingsUIController.TogglePanel(false);

        CheckPlatformAndDisableARIfNecessary();
    }
    
    public void OnQuizButton()
    {
        _loadQuiz.RaiseEvent();
    }
    public void OnARButton()
    {
        _loadAR.RaiseEvent();
    }
    public void OnSettingsButton()
    {
        TogglePanel(false); //Close main panel 
        _settingsUIController.TogglePanel(true); //and open settings panel
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }


    private void CheckPlatformAndDisableARIfNecessary()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            _arButton.interactable = true;
        }
        else
        {
            _arButton.interactable = false;
        }
    }
}
