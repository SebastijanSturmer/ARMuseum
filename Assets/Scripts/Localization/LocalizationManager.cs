using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private Enums.Language _language;

    public static LocalizationManager Instance;

    void OnAwake()
    {
        Instance = this;
    }

    /// <summary>
    /// Event function that responds to UI language change. It will send update to all texts
    /// </summary>
    /// <param name="languageMessage"></param>
    public void SetLanguage(EventMessage languageMessage)
    {
        _language = ((LanguageMessage)languageMessage).Language;

        UpdateLanguage();
    }

    private void UpdateLanguage()
    {
        throw new NotImplementedException();
    }
}

