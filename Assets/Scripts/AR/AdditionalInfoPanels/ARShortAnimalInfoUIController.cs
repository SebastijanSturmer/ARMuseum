using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARShortAnimalInfoUIController : AnimatedBaseGUIController
{
    [SerializeField] private GameObject _textPrefab;
    private List<GameObject> _infoTabs = new List<GameObject>();

    /// <summary>
    /// Updates info about animal. Function will instantiate info tabs for each short info.
    /// </summary>
    /// <param name="shortInfoValues"></param>
    public void UpdateInfo(ShortInfoStruct[] shortInfoValues)
    {
        //First we remove all info tabs
        for (int i = 0; i < _infoTabs.Count; i++)
        {
            Destroy(_infoTabs[i]);
        }

        _infoTabs.Clear();

        //Then we add new ones
        foreach (ShortInfoStruct shortInfoValue in shortInfoValues)
        {
            //Show only info with keys and values!
            if (shortInfoValue.Key != "" && shortInfoValue.Value != "")
            {
                var textObj = Instantiate(_textPrefab, _mainPanel.transform);
                textObj.GetComponent<TextMeshProUGUI>().text = shortInfoValue.Key + " : " + shortInfoValue.Value;
                _infoTabs.Add(textObj);
            }
        }
    }
}
