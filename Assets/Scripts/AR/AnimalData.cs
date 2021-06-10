using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public string AnimalName;
    public string BasicInfo;
    public ShortInfoStruct[] ShortInfo;
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
