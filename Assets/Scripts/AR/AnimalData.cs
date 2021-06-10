using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public string AnimalNameEN;
    public string AnimalNameHR;

    public string BasicInfoEN;
    public string BasicInfoHR;

    public ShortInfoStruct[] ShortInfoEN;
    public ShortInfoStruct[] ShortInfoHR;
}

[Serializable]
public class ListOfAnimalData
{
    public List<AnimalData> Animals;
}

[Serializable]
public struct ShortInfoStruct
{
    public string Key;
    public string Value;
}
