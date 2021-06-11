using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARAnimalMapInfoUIController : AnimatedBaseGUIController
{
    [SerializeField] private Image _animalMap;

    public void UpdateInfo(Sprite map)
    {
        _animalMap.sprite = map;
    }
}
