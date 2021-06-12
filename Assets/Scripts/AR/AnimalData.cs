using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimalData
{
    public string AnimalIdentifier; //Its a string for convenience. It should be animal name on english. 
                                     //It could be an int but then we should track IDs in json file and there could be human errors if we are manually inputing new animals.
    public string AnimalName;
    public string BasicInfo;
    public ShortInfoStruct[] ShortInfo;
}

[Serializable]
public struct ListOfAnimalData
{
    public List<AnimalData> Animals;

}

[Serializable]
public struct ShortInfoStruct
{
    public string Key;
    public string Value;

}
