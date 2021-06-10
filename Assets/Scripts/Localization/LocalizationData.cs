using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "My/Localization Data")]
public class LocalizationData : ScriptableObject
{
    [SerializeField] private Enums.Language _currentLanguage = Enums.Language.English;
    [SerializeField] private List<LanguageDataWithKey> _languageForValueInput; //Since we cant serialize dictionaries and we cant see them in editor we are using additional array that will be copied to dictionary

    private Dictionary<string, LanguageData> _languagesForValue = new Dictionary<string, LanguageData>();

    public Dictionary<string, LanguageData> LanguagesForValue { get => _languagesForValue; }
    public Enums.Language CurrentLanguage { get => _currentLanguage;}


    /// <summary>
    /// Must be called because we are adding all language data and keys to dictionary for faster search!
    /// </summary>
    public void Init()
    {
        if (_languageForValueInput == null)
            return;

        if (_languagesForValue == null)
            _languagesForValue = new Dictionary<string, LanguageData>();

        _languagesForValue.Clear();

        //Copy to dictionary
        for (int i = 0; i < _languageForValueInput.Count; i++)
        {
            _languagesForValue.Add(_languageForValueInput[i].Key, _languageForValueInput[i].LanguageData);
        }
    }

    public void SetLanguage(Enums.Language newLanguage)
    {
        _currentLanguage = newLanguage;
    }

}

[Serializable]
public struct LanguageData
{
    public string English;
    public string Croatian;
}

[Serializable]
public struct LanguageDataWithKey
{
    public string Key;
    public LanguageData LanguageData;
}