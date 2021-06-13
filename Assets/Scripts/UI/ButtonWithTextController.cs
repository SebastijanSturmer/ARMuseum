using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonWithTextController : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    public void SetText(string newText)
    {
        _text.text = newText;
    }

    public void TogglePanel(bool shouldDisplay)
    {
        _panel.SetActive(shouldDisplay);
    }

    public void SetOnClickToButton(UnityAction method)
    {
        _button.onClick.AddListener(method);
    }
}
