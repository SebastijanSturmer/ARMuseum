using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageSelectorController : MonoBehaviour
{
    [SerializeField] private Enums.Language _selectedLanguage;

    public Enums.Language SelectedLanguage { get => _selectedLanguage; }

    void Start()
    {
        
    }

    public void OnLanguageSelected(Enums.Language newLanguage)
    {

    }
}
