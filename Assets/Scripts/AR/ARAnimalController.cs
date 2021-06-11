using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARAnimalController : MonoBehaviour
{
    [SerializeField] private AnimalData _animalData;

    public AnimalData AnimalData { get => _animalData; }

    public void SetAnimalData(AnimalData newAnimalData)
    {
        _animalData = newAnimalData;
    }
}
