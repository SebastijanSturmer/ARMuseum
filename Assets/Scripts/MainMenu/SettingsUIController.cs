using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : BaseGUIController
{
    [Header("Internal references")]
    [SerializeField] private List<Button> _languageButtons;
    [Header("Events")]
    [SerializeField] private ScriptableEvent _settingsPanelClosed;
    [SerializeField] private ScriptableEvent _volumeSliderChanged;
    [SerializeField] private ScriptableEvent _changeLanguage;

    private void Start()
    {
        for (int i = 0; i < _languageButtons.Count; i++)
        {
            int index = i;

            if (index < Enum.GetNames(typeof(Enums.Language)).Length) //Just a check if we have more buttons than languages!
            {
                _languageButtons[index].gameObject.SetActive(true);
                _languageButtons[index].onClick.AddListener(delegate { OnLanguageSelected((Enums.Language)index); });
            }
            else
            {
                _languageButtons[index].gameObject.SetActive(false);
            }
        }
    }

    public void OnClosePanelButton()
    {
        TogglePanel(false);
        _settingsPanelClosed.RaiseEvent(); //Main menu manager will respond to this event and toggle his panel back on
    }

    public void OnVolumeSliderChanged(float value)
    {
        _volumeSliderChanged.RaiseEvent(new FloatMessage(value));
    }

    public void OnLanguageSelected(Enums.Language newLanguage)
    {
        _changeLanguage.RaiseEvent(new LanguageMessage(newLanguage));
    }

}
