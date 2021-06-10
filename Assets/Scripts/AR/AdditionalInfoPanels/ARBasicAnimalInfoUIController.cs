using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARBasicAnimalInfoUIController : AnimatedBaseGUIController
{
    [SerializeField] private TextMeshProUGUI _basicInfoText;
    public void UpdateInfo(string newBasicInfo)
    {
        _basicInfoText.text = newBasicInfo;
    }
}
