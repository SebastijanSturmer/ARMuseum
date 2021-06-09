using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] private LanguageSelectorController _languageSelectorController;
    [SerializeField] private Button _arButton;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _loadQuiz;
    [SerializeField] private ScriptableEvent _loadAR;
    [SerializeField] private ScriptableEvent _changeLanguage;

    private int _selectedLanguageIndex = 0;

    void Start()
    {
        CheckPlatformAndDisableARIfNecessary();
    }

    /// <summary>
    /// Triggered by exiting from 
    /// </summary>
    public void OnLanguageChangeConfirmed()
    {
        _changeLanguage.RaiseEvent(new LanguageMessage(_languageSelectorController.SelectedLanguage));
    }
    
    public void OnQuizButton()
    {
        _loadQuiz.RaiseEvent();
    }
    public void OnARButton()
    {
        _loadAR.RaiseEvent();
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
