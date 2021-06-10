using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizeText : MonoBehaviour
{
    [SerializeField] private string _key;
    private TextMeshProUGUI _text;

    public string Text { get => _text.text; }
    public string Key { get => _key;}

    private void OnEnable()
    {
        _text = gameObject.GetComponent<TextMeshProUGUI>();

        if (Text == null)
            return;

        LocalizationManager.Instance.AddNewText(this);
    }
    private void OnDisable()
    {
        LocalizationManager.Instance.RemoveText(this);
    }

    public void SetText(string newText)
    {
        if (_text == null)
            return;

        _text.text = newText;
    }
}
