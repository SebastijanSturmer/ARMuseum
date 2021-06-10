using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private Enums.Language _language;
    [SerializeField] private LocalizationData _localizationData;
    [SerializeField] private List<LocalizeText> _textsToUpdate;

    public static LocalizationManager Instance;

    public Enums.Language Language { get => _language; }

    private bool _firstTextUpdateFinished = false;

    void Awake()
    {
        _firstTextUpdateFinished = false;
        Instance = this;
    }

    private void Start()
    {
        _localizationData.Init(); //initializing localization data will copy contents from list of language data with keys to dictionary

        SetLanguage(new LanguageMessage(_localizationData.CurrentLanguage)); //Debugging, Selecting language from selected language in inspector

        _firstTextUpdateFinished = true;
    }

    /// <summary>
    /// Function that is called by LocalizeText script that will be added to TextsToUpdate and if it is added after Start then it will update its text
    /// </summary>
    /// <param name="newText"></param>
    public void AddNewText(LocalizeText newText)
    {
        _textsToUpdate.Add(newText);

        if (_firstTextUpdateFinished)
            UpdateLanguageToText(newText);
    }

    /// <summary>
    /// Function that removes text from TextsToUpdate
    /// </summary>
    /// <param name="newText"></param>
    public void RemoveText(LocalizeText newText)
    {
        if (_textsToUpdate.Contains(newText))
        {
            _textsToUpdate.Remove(newText);
        }
    }

    /// <summary>
    /// Event function that responds to UI language change. It will send update to all texts
    /// </summary>
    /// <param name="languageMessage"></param>
    public void SetLanguage(EventMessage languageMessage)
    {
        _language = ((LanguageMessage)languageMessage).Language;
        _localizationData.SetLanguage(_language);
        UpdateLanguage();
    }

    /// <summary>
    /// Function that goes trought all texts in texts to update and updates its languages
    /// </summary>
    private void UpdateLanguage()
    {
        for (int i = 0; i < _textsToUpdate.Count; i++)
        {
            UpdateLanguageToText(_textsToUpdate[i]);
            
        }
    }

    /// <summary>
    /// Function that checks for key in LocalizationData for LocalizeText and updates its value depending on App language
    /// </summary>
    /// <param name="textToUpdate"></param>
    private void UpdateLanguageToText(LocalizeText textToUpdate)
    {
        if (!_localizationData.LanguagesForValue.ContainsKey(textToUpdate.Key))//If we dont have a key stored in localization data then skip this text
        {
            Debug.LogWarning("LocalizationManager : We dont have language data for key : " + textToUpdate.Key);
        }

        switch (_language)
        {
            case Enums.Language.English:
                textToUpdate.SetText(_localizationData.LanguagesForValue[textToUpdate.Key].English);
                break;
            case Enums.Language.Croatian:
                textToUpdate.SetText(_localizationData.LanguagesForValue[textToUpdate.Key].Croatian);
                break;
        }
    }
}

