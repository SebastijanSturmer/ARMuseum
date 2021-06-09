using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Internal References")]
    [SerializeField] private LanguageSelectorController _languageSelectorController;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _loadQuiz;
    [SerializeField] private ScriptableEvent _loadAR;
    [SerializeField] private ScriptableEvent _changeLanguage;

    private int _selectedLanguageIndex = 0;

    void Start()
    {
        
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
}
