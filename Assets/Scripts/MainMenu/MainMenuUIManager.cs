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

    /// <summary>
    /// Function that will call loadQuiz event
    /// </summary>
    public void OnQuizButton()
    {
        _loadQuiz.RaiseEvent();
    }

    /// <summary>
    /// Function that will call loadAR event
    /// </summary>
    public void OnARButton()
    {
        _loadAR.RaiseEvent();
    }

    /// <summary>
    /// Function that will open settings panel
    /// </summary>
    public void OnSettingsButton()
    {
        TogglePanel(false); //Close main panel 
        _settingsUIController.TogglePanel(true); //and open settings panel
    }

    /// <summary>
    /// Function that will close application
    /// </summary>
    public void OnQuitButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// Function that will disable AR button if we are not on Android or IOS
    /// </summary>
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
