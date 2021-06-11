using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : BaseGUIController
{
    [Header("Internal references")]
    [SerializeField] private List<Button> _languageButtons;
    [SerializeField] private Slider _volumeSlider;
    [Header("Events")]
    [SerializeField] private ScriptableEvent _settingsPanelClosed;
    [SerializeField] private ScriptableEvent _volumeSliderChanged;
    [SerializeField] private ScriptableEvent _changeLanguage;

    private void Start()
    {
        SetupLanguageButtons();

        if (PlayerPrefs.HasKey("Volume"))
            _volumeSlider.value = PlayerPrefs.GetFloat("Volume"); //Load volume value to slider
    }

    /// <summary>
    /// Fucntion that will close settings panel and raise event that it was closed
    /// </summary>
    public void OnClosePanelButton()
    {
        TogglePanel(false);
        _settingsPanelClosed.RaiseEvent(); //Main menu manager will respond to this event and toggle his panel back on
    }

    /// <summary>
    /// Raises event that volume was changed
    /// </summary>
    /// <param name="value"></param>
    public void OnVolumeSliderChanged(float value)
    {
        _volumeSliderChanged.RaiseEvent(new FloatMessage(value));
    }

    /// <summary>
    /// Raises event that language was changed
    /// </summary>
    /// <param name="newLanguage"></param>
    public void OnLanguageSelected(Enums.Language newLanguage)
    {
        _changeLanguage.RaiseEvent(new LanguageMessage(newLanguage));
    }

    /// <summary>
    /// Function will add listeners to language buttons and if we have more buttons than languages for some reason it will deactivate that buttons
    /// </summary>
    private void SetupLanguageButtons()
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
}
