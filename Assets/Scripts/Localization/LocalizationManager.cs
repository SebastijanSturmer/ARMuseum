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
        _localizationData.Init();

        SetLanguage(new LanguageMessage(_localizationData.CurrentLanguage)); //Debugging, Selecting language from selected language in inspector

        _firstTextUpdateFinished = true;
    }

    public void AddNewText(LocalizeText newText)
    {
        _textsToUpdate.Add(newText);

        if (_firstTextUpdateFinished)
            UpdateLanguageToText(newText);
    }

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

    private void UpdateLanguage()
    {
        for (int i = 0; i < _textsToUpdate.Count; i++)
        {
            UpdateLanguageToText(_textsToUpdate[i]);
            
        }
    }

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

