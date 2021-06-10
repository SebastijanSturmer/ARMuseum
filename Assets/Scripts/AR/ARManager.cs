using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ARManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _spawnSpeed;
    [SerializeField] private List<AnimalNameAndPrefab> _availableAnimalPrefabs;
    [SerializeField] private List<AnimalData> _animalsFromJSON;

    [Header("References")]
    [SerializeField] private ARTrackedImageManager _trackedImageManager;
    [SerializeField] private AssetReference _animalsJSON;

    [Header("Events")]
    [SerializeField] private ScriptableEvent _arAnimalDetected;

    private List<AnimalTrackedImageAndGameObject> _spawnedAnimals = new List<AnimalTrackedImageAndGameObject>();
    private ARTrackedImage _lastTrackedImage;

    private bool _loadingAnimalsCompleted;
    
    private void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnImageRecognized;
    }

    public void OnStartAR()
    {
        StartCoroutine(StartARCoroutine());

    }

    private IEnumerator StartARCoroutine()
    {
        GetAnimalsFromJSON();

        yield return new WaitUntil(() => _loadingAnimalsCompleted == true);

        _trackedImageManager.trackedImagesChanged += OnImageRecognized;
    }

    private void GetAnimalsFromJSON()
    {
        _loadingAnimalsCompleted = false;

        _animalsFromJSON = new List<AnimalData>();

        if (!_animalsJSON.RuntimeKeyIsValid())
        {
            Debug.LogError("QuizManager : Json Questions runtime key is not valid!");
            return;
        }


        _animalsJSON.LoadAssetAsync<TextAsset>().Completed += handle =>
        {
            ListOfAnimalData animals = JsonUtility.FromJson<ListOfAnimalData>(handle.Result.text);
            for (int i = 0; i < animals.Animals.Count; i++)
            {
                _animalsFromJSON.Add(animals.Animals[i]);
            }

            _loadingAnimalsCompleted = true;

            Addressables.Release(handle);
        };

    }

    /// <summary>
    /// Event function that is called when TrackedImageManager updates! Used for spawning,positioning and destroying objects
    /// </summary>
    /// <param name="args"></param>
    public void OnImageRecognized(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage trackedImage in args.added)
        {
            StartCoroutine(SpawnObjectOnImage(trackedImage));
        }
        foreach (ARTrackedImage trackedImage in args.updated)
        {
            UpdateObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in args.removed)
        {
            RemoveObjectFromImage(trackedImage);
        }

    }

    /// <summary>
    /// Coroutine that will spawn object depending on tracked image and it will animate its spawn (growing)
    /// </summary>
    /// <param name="trackedImage"></param>
    /// <returns></returns>
    IEnumerator SpawnObjectOnImage(ARTrackedImage trackedImage)
    {
        GameObject prefabToSpawn = null;
        //Finding prefab from list
        for (int i = 0; i < _availableAnimalPrefabs.Count; i++)
        {
            if (_availableAnimalPrefabs[i].AnimalName == trackedImage.referenceImage.name)
            {
                prefabToSpawn = _availableAnimalPrefabs[i].AnimalPrefab;
                break;
            }
        }
        //If we havent found animal in list then break
        if (prefabToSpawn == null)
            yield break;

        //Spawning object
        GameObject spawnedObject = Instantiate(prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation, this.gameObject.transform);
        spawnedObject.transform.localScale = Vector3.zero;
        spawnedObject.name = trackedImage.referenceImage.name;


        _spawnedAnimals.Add(new AnimalTrackedImageAndGameObject(trackedImage, spawnedObject));
        //Animating spawn
        float lerpValue = 0;
        while (true)
        {
            lerpValue += Time.fixedDeltaTime * _spawnSpeed;
            if (lerpValue > 1)
                lerpValue = 1;

            if (spawnedObject.transform.localScale.x >= 1)
                break;

            spawnedObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, lerpValue);

            yield return new WaitForFixedUpdate();
        }

        FindAnimalDataFromImageAndCallAnimalDetectedEvent(trackedImage);
    }

    private void UpdateObject(ARTrackedImage trackedImage)
    {
        //If this image is currently being tracked and it is different from last tracked image then find data and send to UI
        if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking && _lastTrackedImage != trackedImage)
        { 
            FindAnimalDataFromImageAndCallAnimalDetectedEvent(trackedImage);
            _lastTrackedImage = trackedImage;
        }


        for (int i = 0; i < _spawnedAnimals.Count; i++)
        {
            if (_spawnedAnimals[i].Image == trackedImage)
            {
                _spawnedAnimals[i].GameObject.transform.position = trackedImage.transform.position;
                _spawnedAnimals[i].GameObject.transform.rotation = trackedImage.transform.rotation;
            }
        }
    }

    private void FindAnimalDataFromImageAndCallAnimalDetectedEvent(ARTrackedImage trackedImage)
    {
        //Find animal in data
        AnimalData detectedAnimal = null;
        for (int i = 0; i < _animalsFromJSON.Count; i++)
        {
            if (_animalsFromJSON[i].AnimalNameEN == trackedImage.referenceImage.name) //We are searching by english name because pictures will be named on english
            {
                detectedAnimal = _animalsFromJSON[i];
            }
        }

        //Raising event about detected animal
        //Used for GUI info about that animal.
        _arAnimalDetected.RaiseEvent(new AnimalDataMessage(detectedAnimal));
    }

    void RemoveObjectFromImage(ARTrackedImage trackedImage)
    {
        List<AnimalTrackedImageAndGameObject> spawnedAnimalsListCopy = new List<AnimalTrackedImageAndGameObject>();

        for (int i = 0; i < _spawnedAnimals.Count; i++)
        {
            spawnedAnimalsListCopy.Add(_spawnedAnimals[i]);
        }

        for (int i = 0; i < spawnedAnimalsListCopy.Count; i++)
        {
            if (spawnedAnimalsListCopy[i].Image == trackedImage)
            {
                Destroy(spawnedAnimalsListCopy[i].GameObject);
                _spawnedAnimals.Remove(spawnedAnimalsListCopy[i]);
            }

        }
    }

    /// <summary>
    /// Debug function for looking at JSON format
    /// </summary>
    void GenerateAnimals()
    {
        ListOfAnimalData listOfAnimals = new ListOfAnimalData();
        listOfAnimals.Animals = new List<AnimalData>();

        for (int i = 0; i < 2; i++)
        {
            AnimalData animal = new AnimalData();

            animal.AnimalNameEN = "Penguin";
            animal.AnimalNameHR = "Pingvin";

            animal.BasicInfoEN = "Penguins live on Antartica. They like cold!";
            animal.BasicInfoHR = "Penguins live on Antartica. They like cold!";

            animal.ShortInfoEN = new ShortInfoStruct[3];
            animal.ShortInfoHR = new ShortInfoStruct[3];

            animal.ShortInfoEN[0].Key = "Climate";
            animal.ShortInfoEN[0].Value = "Cold";
            animal.ShortInfoEN[1].Key = "Food";
            animal.ShortInfoEN[1].Value = "Meat";
            animal.ShortInfoEN[2].Key = "Height";
            animal.ShortInfoEN[2].Value = "1.5m";

            animal.ShortInfoHR[0].Key = "Climate";
            animal.ShortInfoHR[0].Value = "Cold";
            animal.ShortInfoHR[1].Key = "Food";
            animal.ShortInfoHR[1].Value = "Meat";
            animal.ShortInfoHR[2].Key = "Height";
            animal.ShortInfoHR[2].Value = "1.5m";

            listOfAnimals.Animals.Add(animal);
        }

        string json = JsonUtility.ToJson(listOfAnimals);
        Debug.Log(json);
    }
}

/// <summary>
/// Struct used for holding prefabs with names as identification
/// </summary>
[Serializable]
public struct AnimalNameAndPrefab
{
    public string AnimalName;
    public GameObject AnimalPrefab;
}

public struct AnimalTrackedImageAndGameObject
{
    public ARTrackedImage Image;
    public GameObject GameObject;

    public AnimalTrackedImageAndGameObject(ARTrackedImage image, GameObject gameObject)
    {
        Image = image;
        GameObject = gameObject;
    }
}
